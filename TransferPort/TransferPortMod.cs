using System.Collections.Generic;
using HarmonyLib;
using KMod;
using RsLib;
using RsLib.Unity;
using RsTransferPort.Assets;
using TMPro;
using UnityEngine;

namespace RsTransferPort {
    public class TransferPortMod : UserMod2 {
        public static BodyAsset BodyAsset;

        private void BeforeRsInit() {
            TextStyleSetting baseStyle = RsUITuning.TextStyleSettings.style_bodyText;

            TextStyleSetting style_rs_channel_name = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_channel_name.name = "style_rs_channel_name";
            style_rs_channel_name.style = FontStyles.Bold;
            style_rs_channel_name.sdfFont = baseStyle.sdfFont;
            style_rs_channel_name.textColor = Color.black;
            style_rs_channel_name.enableWordWrapping = false;
            style_rs_channel_name.fontSize = baseStyle.fontSize;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_channel_name);

            TextStyleSetting style_rs_channel_num = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_channel_num.name = "style_rs_channel_num";
            style_rs_channel_num.style = baseStyle.style;
            style_rs_channel_num.sdfFont = baseStyle.sdfFont;
            style_rs_channel_num.textColor = new Color(0.55f, 0.26f, 0.3f);
            style_rs_channel_num.enableWordWrapping = false;
            style_rs_channel_num.fontSize = baseStyle.fontSize;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_channel_num);

            TextStyleSetting style_rs_input = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_input.name = "style_rs_input";
            style_rs_input.style = baseStyle.style;
            style_rs_input.sdfFont = baseStyle.sdfFont;
            style_rs_input.textColor = Color.black;
            style_rs_input.enableWordWrapping = false;
            style_rs_input.fontSize = 16;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_input);

            TextStyleSetting style_rs_placeholder = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_placeholder.name = "style_rs_placeholder";
            style_rs_placeholder.style = baseStyle.style;
            style_rs_placeholder.sdfFont = baseStyle.sdfFont;
            style_rs_placeholder.textColor = Color.gray;
            style_rs_placeholder.enableWordWrapping = false;
            style_rs_placeholder.fontSize = 16;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_placeholder);

            TextStyleSetting style_rs_detail_label = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_detail_label.name = "style_rs_detail_label";
            style_rs_detail_label.style = baseStyle.style;
            style_rs_detail_label.sdfFont = baseStyle.sdfFont;
            style_rs_detail_label.textColor = new Color(0.35f, 0.14f, 0.57f);
            style_rs_detail_label.enableWordWrapping = false;
            style_rs_detail_label.fontSize = 14;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_detail_label);

            TextStyleSetting style_rs_warning_label = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_warning_label.name = "style_rs_warning_label";
            style_rs_warning_label.style = baseStyle.style;
            style_rs_warning_label.sdfFont = baseStyle.sdfFont;
            style_rs_warning_label.textColor = new Color(0.88f, 0.38f, 0.27f);
            style_rs_warning_label.enableWordWrapping = false;
            style_rs_warning_label.fontSize = 14;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_warning_label);

            TextStyleSetting style_rs_port_channel_name = ScriptableObject.CreateInstance<TextStyleSetting>();
            style_rs_port_channel_name.name = "style_rs_port_channel_name";
            style_rs_port_channel_name.style = FontStyles.Bold;
            style_rs_port_channel_name.sdfFont = baseStyle.sdfFont;
            style_rs_port_channel_name.textColor = Color.white;
            style_rs_port_channel_name.enableWordWrapping = false;
            style_rs_port_channel_name.fontSize = 14;
            RsUITuning.TextStyleSettings.AddTextStyleSetting(style_rs_port_channel_name);
        }

        public override void OnLoad(Harmony harmony) {
            BeforeRsInit();
            MyResource.InitAllTask();

            AssetBundle assetBundle = RsAssetBundle.LoadAssetBundle(mod.ContentPath, "transferport", null, true);
            RsResources.AddAssetBundle(assetBundle);

            BodyAsset = assetBundle.LoadAsset<BodyAsset>("BodyAsset");

            RsAssets.Initialize(mod, harmony)
                .AddSprite(MyOverlayModes.PortChannel.Icon, BodyAsset.portOverlayButton)
                .AddStatusItemIcon("unconnected_channel_icon", BodyAsset.unconnectedChannelIcon)
                .AddStatusItemIcon("global_connectivity_icon", BodyAsset.globalConnectivityIcon)
                .AddStatusItemIcon("planetary_isolation_icon", BodyAsset.planetaryIsolationIcon);

            RsButtonMenu.Initialize(mod, harmony)
                .AddIcon("show_overlay_self_icon", BodyAsset.showOverlayButton);

            RsLocalization.Initialize(mod, harmony)
                .RegisterLoad(typeof(STRINGS))
                .RegisterAddStrings(typeof(STRINGS.BUILDINGS))
                .RegisterAddStrings(typeof(STRINGS.UI))
                .RegisterAddStrings(typeof(STRINGS.BUILDING));

            RsSideScreen.Initialize(mod, harmony)
                // .CopyAndCreate<LogicBroadcastChannelSideScreen, TransferPortChannelSideScreen>()
                .CopyAndCreate<ClusterLocationFilterSideScreen, WorldDiscoveredSideScreen>()
                .CopyAndCreate<HighEnergyParticleDirectionSideScreen, MyHighEnergyParticleDirectionSideScreen>()
                // .Add(() => ChannelNameSettingSideScreen.Prefab, true)
                .Add(() => BodyAsset.portChannelSideScreen, true);

            RsOverlay.Initialize(mod, harmony)
                .AddOverlayMode((screen) => new MyOverlayModes.PortChannel(screen.powerLabelParent))
                .AddOverlayMenuToggleInfo(() => new RsOverlay.RsOverlayToggleInfo(
                    STRINGS.UI.OVERLAYS.PORTCHANNELMODE.BUTTON.ToString(),
                    MyOverlayModes.PortChannel.Icon,
                    MyOverlayModes.PortChannel.ID, "",
                    Action.NumActions,
                    STRINGS.UI.TOOLTIPS.PORTCHANNELMODE_OVERLAY_STRING.ToString(),
                    STRINGS.UI.OVERLAYS.PORTCHANNELMODE.BUTTON.ToString()
                ))
                .AddOverlayLegendInfo(() =>
                    new OverlayLegend.OverlayInfo() {
                        mode = MyOverlayModes.PortChannel.ID,
                        name = "STRINGS.UI.OVERLAYS.PORTCHANNELMODE.NAME",
                        infoUnits = new List<OverlayLegend.OverlayInfoUnit>()
                        {
                            new OverlayLegend.OverlayInfoUnit(null, "", Color.gray, Color.black)
                        },
                        isProgrammaticallyPopulated = true,
                        diagrams = new List<GameObject>()
                        {
                            PortChannelDiagram.Prefab
                        }
                    }
                )
                .AddHoverTextCardOverlayFilterMap(MyOverlayModes.PortChannel.ID, () => {
                    // int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
                    return false;
                });

            RsBuilding.Initialize(mod, harmony)
                .AddBuilding(LiquidTransferConduitSenderConfig.ID, RsTypes.PlanType.Plumbing, "rs_transfer_port", "LiquidPiping")
                .AddBuilding(LiquidTransferConduitReceiverConfig.ID, RsTypes.PlanType.Plumbing, "rs_transfer_port", "LiquidPiping")
                .AddBuilding(GasTransferConduitSenderConfig.ID, RsTypes.PlanType.HVAC, "rs_transfer_port", "GasPiping")
                .AddBuilding(GasTransferConduitReceiverConfig.ID, RsTypes.PlanType.HVAC, "rs_transfer_port", "GasPiping")
                .AddBuilding(SolidTransferConduitSenderConfig.ID, RsTypes.PlanType.Conveyance, "rs_transfer_port", "SolidTransport")
                .AddBuilding(SolidTransferConduitReceiverConfig.ID, RsTypes.PlanType.Conveyance, "rs_transfer_port", "SolidTransport")
                .AddBuilding(WirelessLogicSenderConfig.ID, RsTypes.PlanType.Automation, "rs_transfer_port", "LogicControl")
                .AddBuilding(WirelessLogicReceiverConfig.ID, RsTypes.PlanType.Automation, "rs_transfer_port", "LogicControl")
                .AddBuilding(WirelessPowerPortConfig.ID, RsTypes.PlanType.Power, "rs_transfer_port", "PrettyGoodConductors")
                .AddBuilding(RadiantParticlesTransferSenderConfig.ID, RsTypes.PlanType.HEP, "rs_transfer_port", "AdvancedNuclearResearch", true)
                .AddBuilding(RadiantParticlesTransferReceiverConfig.ID, RsTypes.PlanType.HEP, "rs_transfer_port", "AdvancedNuclearResearch", true)
                .AddBuilding(TransferPortCenterConfig.ID, RsTypes.PlanType.Base, "rs_transfer_port", null, true);

            base.OnLoad(harmony);
        }
    }
}
