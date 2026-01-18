using OUI = STRINGS.UI;

namespace RsTransferPort {
    public static class STRINGS {
        public static class BUILDING {
            public static class STATUSITEMS {
                public class RSRADIANTPARTICLESTRANSFERSENDERINFO {
                    public static LocString NAME = (LocString)"Transfer outlet blocked, closed trapping radiation particles";
                    public static LocString TOOLTIP = (LocString)"Transfer outlet blocked, closed trapping radiation particles";
                }
                public class RSTRANSFERPORTCHANNELCONNECTION {
                    public static LocString NAME = (LocString)"Unconnected channel";
                    public static LocString TOOLTIP = (LocString)"Create or join a channel by channel name";
                }

                public class RSCHANNELPLANETARYISOLATIONMODEL {
                    public static LocString NAME = (LocString)"Planetary isolation mode";
                    public static LocString TOOLTIP = "This mode is not capable of transplanetary or rocket transmission, but can be switched to planetary interworking mode in the channel setting.";
                }
                public class RSCHANNELGLOBALCONNECTIVITYMODEL {
                    public static LocString NAME = (LocString)"Planetary interworking mode";
                    public static LocString TOOLTIP = "This mode can be used for transplanetary or rocket transmissions, and can be switched to planetary isolation mode in channel Settings";
                }
            }
        }

        public static class BUILDINGS {
            public static class PREFABS {
                public static class RSLIQUIDTRANSFERCONDUITSENDER {
                    public static LocString NAME = OUI.FormatAsLink("Liquid Pipe Transfer Inlet",
                        LiquidTransferConduitSenderConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. ",
                        OUI.FormatAsLink("Liquid Pipe Transfer Outlet", LiquidTransferConduitReceiverConfig.ID),
                        " which transmits liquid to the same channel."
                    );

                    public static LocString DEFAULTNAME = "Liquid Pipe Transfer Inlet";
                }

                public static class RSLIQUIDTRANSFERCONDUITRECEIVER {
                    public static LocString NAME = OUI.FormatAsLink("Liquid Pipe Transfer Outlet",
                        LiquidTransferConduitReceiverConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. Receive liquid on the same channel as ",
                        OUI.FormatAsLink("Liquid Pipe Transfer Inlet", LiquidTransferConduitSenderConfig.ID), "."
                    );

                    public static LocString DEFAULTNAME = "Liquid Pipe Transfer Outlet";
                }


                public static class RSGASTRANSFERCONDUITSENDER {
                    public static LocString NAME =
                        OUI.FormatAsLink("Gas Pipe Transfer Inlet", GasTransferConduitSenderConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. ",
                        OUI.FormatAsLink("Gas Pipe Transfer Outlet", GasTransferConduitReceiverConfig.ID),
                        " which transmits liquid to the same channel."
                    );

                    public static LocString DEFAULTNAME = "Gas Pipe Transfer Inlet";
                }

                public static class RSGASTRANSFERCONDUITRECEIVER {
                    public static LocString NAME =
                        OUI.FormatAsLink("Gas Pipe Transfer Outlet", GasTransferConduitReceiverConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. Receive gas on the same channel as ",
                        OUI.FormatAsLink("Gas Pipe Transfer Inlet", GasTransferConduitSenderConfig.ID), "."
                    );

                    public static LocString DEFAULTNAME = "Gas Pipe Transfer Outlet";
                }

                public static class RSSOLIDTRANSFERCONDUITSENDER {
                    public static LocString NAME = OUI.FormatAsLink("Solid Pipe Transfer Inlet",
                        SolidTransferConduitSenderConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. ",
                        OUI.FormatAsLink("Solid Pipe Transfer Outlet", SolidTransferConduitReceiverConfig.ID),
                        " which transmits solid to the same channel."
                    );

                    public static LocString DEFAULTNAME = "Solid Pipe Transfer Inlet";
                }

                public static class RSSOLIDTRANSFERCONDUITRECEIVER {
                    public static LocString NAME = OUI.FormatAsLink("Solid Pipe Transfer Outlet",
                        SolidTransferConduitReceiverConfig.ID);

                    public static LocString DESC =
                        "Do you still worry about the pipe is too long, try this, to solve the pipe is too long, affect the planning, performance and other problems.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. Receive solid on the same channel as ",
                        OUI.FormatAsLink("Gas Pipe Transfer Inlet", SolidTransferConduitSenderConfig.ID), "."
                    );

                    public static LocString DEFAULTNAME = "Solid Pipe Transfer Outlet";
                }

                public static class RSWIRELESSPOWERPORT {
                    public static LocString NAME = OUI.FormatAsLink("Wireless Power Port", WirelessPowerPortConfig.ID);

                    public static LocString DESC =
                        "Are you still bothered by the length of the wire? Try this to solve the problem of long wires, affecting planning, performance and so on.";

                    public static LocString EFFECT =
                        "Channel by building name. The same channel can be regarded as the same line.";

                    public static LocString DEFAULTNAME = "Wireless Power Port";
                }

                public static class RSWIRELESSLOGICSENDER {
                    public static LocString NAME =
                        OUI.FormatAsLink("Wireless Signal Broadcast Port", WirelessLogicSenderConfig.ID);

                    public static LocString DESC =
                        "Are you still worried about the signal cable is too long? Try this to solve the problem that the signal is too long, affecting the planning, performance and so on.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. Broadcast the signal to ",
                        OUI.FormatAsLink("Wireless Signal Receiving Port", WirelessLogicSenderConfig.ID),
                        " on the same channel."
                    );

                    public static LocString DEFAULTNAME = "Wireless Signal Broadcast Port";
                }

                public static class RSWIRELESSLOGICRECEIVER {
                    public static LocString NAME = OUI.FormatAsLink("Wireless Signal Receiving Port",
                        WirelessLogicReceiverConfig.ID);

                    public static LocString DESC =
                        "Are you still worried about the signal cable is too long? Try this to solve the problem that the signal is too long, affecting the planning, performance and so on.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by building name. Receive a Signal from ",
                        OUI.FormatAsLink("Wireless Signal Receiving Port", WirelessLogicSenderConfig.ID),
                        " on the same channel."
                    );

                    public static LocString DEFAULTNAME = "Wireless Signal Receiving Port";
                }

                public static class RSRADIANTPARTICLESTRANSFERSENDER {
                    public static LocString NAME = OUI.FormatAsLink("Radiant Particles Transfer Inlet",
                        RadiantParticlesTransferSenderConfig.ID);

                    public static LocString DESC =
                        "Are you still worried about the signal cable is too long? Try this to solve the problem that the signal is too long, affecting the planning, performance and so on.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by channel name. Receive radiant particles on the same channel as ",
                        OUI.FormatAsLink("Radiant Particles Transfer Outlet", RadiantParticlesTransferReceiverConfig.ID),
                        " on the same channel."
                    );

                    public static LocString DEFAULTNAME = "Radiant Particles Transfer  Inlet";


                    public static LocString LOGIC_PORT = "Need Radiation Particles";
                    public static LocString LOGIC_PORT_ACTIVE = "<b><style=\"logic_on\">Green Signal</style></b>: Green signal received from Outlet";
                    public static LocString LOGIC_PORT_INACTIVE = "<b><style=\"logic_off\">Red Signal</style></b>: Red signal received from Outlet";

                }

                public static class RSRADIANTPARTICLESTRANSFERRECEIVER {
                    public static LocString NAME = OUI.FormatAsLink("Radiant Particles Transfer Outlet",
                        RadiantParticlesTransferReceiverConfig.ID);

                    public static LocString DESC =
                        "Are you still worried about the signal cable is too long? Try this to solve the problem that the signal is too long, affecting the planning, performance and so on.";

                    public static LocString EFFECT = string.Concat(
                        "Channel by channel name. Receive radiant particles on the same channel as ",
                        OUI.FormatAsLink("Radiant Particles Transfer Inlet", RadiantParticlesTransferSenderConfig.ID),
                        "."
                    );

                    public static LocString DEFAULTNAME = "Radiant Particles Transfer Outlet";

                    public static LocString LOGIC_PORT = "Need Radiation Particles";
                    public static LocString LOGIC_PORT_ACTIVE = "<b><style=\"logic_on\">Green Signal</style></b>: Enable transfer and send a green signal to the Inlet";
                    public static LocString LOGIC_PORT_INACTIVE = "<b><style=\"logic_off\">Red Signal</style></b>:  Disable the transfer and send a red signal to the Inlet";
                }

                public static class RSTRANSFERPORTCENTER {
                    public static LocString NAME = OUI.FormatAsLink("Transfer Port Center",
                        TransferPortCenterConfig.ID);
                    public static LocString DESC =
                        "Discovered worlds are computed one by one, and setting developed worlds to undiscovered helps optimize performance";
                    public static LocString EFFECT = "Sets whether certain worlds have been discovered";

                }

                // public static class WORLDDISCOVEREDCENTER
                // {
                //     public static LocString NAME = OUI.FormatAsLink("世界发现设置中心", WorldDiscoveredCenterConfig.ID);
                //
                //     public static LocString DESC = "发现的世界是会进行物理计算，将开发过完成的世界设置为未发现有助于优化性能";
                //
                //     public static LocString EFFECT = "设置发现或未发现的世界";
                // }
            }
        }

        public static class UI {
            public static class NEWBUILDCATEGORIES {
                public static class RS_TRANSFER_PORT {
                    public static LocString NAME = "Transfer Port";
                    public static LocString BUILDMENUTITLE = "Transfer Port";
                }
            }

            public static class TOOLTIPS {
                public static LocString PORTCHANNELMODE_OVERLAY_STRING = "Displays channel establishment for the transfer port.";
            }

            public static class TOOLS {
                public static class FILTERLAYERS {
                    public static LocString RS_ALL_PORT = "All PORT";
                    public static LocString RS_GAS_PORT = "Gas Port";
                    public static LocString RS_LIQUID_PORT = "Liquid Port";
                    public static LocString RS_SOLID_PORT = "Solid Port";
                    public static LocString RS_POWER_PORT = "Power Port";
                    public static LocString RS_LOGIC_PORT = "Logic Port";
                    public static LocString RS_HEP_PORT = "Radiating Particle Port";

                    public static LocString RS_CENTER_LINE = "Central Connection";
                    public static LocString RS_NEAR_LINE = "Nearby Connection";
                    public static LocString RS_Hide_LINE = "Hide Connection";

                    public static LocString RS_DISABLE_LINE_ANIM = "Disable Connection Anim";
                    public static LocString RS_SHOW_PRIORITY = "Show Priority";
                    public static LocString RS_ONLY_NULL_CHANNEL = "Only (empty channel)";
                    public static LocString RS_ONLY_GLOBAL_CHANNEL = "Only Global Channel";
                }
            }


            public static class USERMENU {
                public static class SHOWOVERLAYSELF_BUTTON {
                    public static LocString NAME = "Channel Overlay";
                    //显示该频道概览
                    public static LocString TOOLTIP = "Displays the channel overlay";
                }
            }

            public static class OVERLAYS {
                public static class PORTCHANNELMODE {
                    public static LocString NAME = "Transfer channel overlays";
                    public static LocString BUTTON = "Transfer channel overlays";
                }
            }

            public static class SIDESCREEN {

                public class RS_PORT_CHANNEL {
                    public static LocString TITLE = "Transfer Port Channel Setting";
                    public static LocString CHANNEL_NAME = "Channel Name";
                    public static LocString CHANNEL_LIST = "Channel List";

                    public static LocString CHANNEL_NULL = "(empty channel)";
                    public static LocString DETAIL_LEVEL_TOOLTIP = "Change the list display level";

                    public static LocString WARIN_BATCH_MODE = "Batch Mode";

                    public static LocString BATCH_NAME_TOOLTIP = "In Batch change mode, you can change the channel name or channel mode of all ports on the same channel";
                    public static LocString CANDIDATE_NAME_TOOLTIP = "Open or close the candidate name";

                    public static LocString GLOBAL_TOOLTIP = "Enable Or Disable Planetary interworking mode.Batch operable";

                    public static LocString PRIORITY_TOOLTIP = "Transport priority.It can be viewed at the Transport Channel Overlay.Batch operable\n<color=#888888>Gray</color>: none\n<color=#4ABEC9>Blue</color>: The priority has been used\n<color=#EFB258>Yellow</color>: The priority of the current port";

                    public static LocString PRIORITY_LINE_INFO = "Priority:  {0}   Inlet:  {1}   Outlet:  {2}";

                }

                public class RS_CANDIDATE_NAME {
                    public static LocString TITLE = "Candidate Name";

                    public static LocString SUPPLY_STATE_0 = "Switch supply or recycle,Current: <style=\"KKeyword\">None</style>";
                    public static LocString SUPPLY_STATE_1 = "Switch supply or recycle,Current: <style=\"KKeyword\">Supply</style>";
                    public static LocString SUPPLY_STATE_2 = "Switch supply or recycle,Current: <style=\"KKeyword\">Recycle</style>";

                    public static LocString TEMPERATURE_STATE_0 = "Switch supply or recycle,Current: <style=\"KKeyword\">None</style>";
                    public static LocString TEMPERATURE_STATE_1 = "Switch supply or recycle,Current: <style=\"KKeyword\">Low Temperature</style>";
                    public static LocString TEMPERATURE_STATE_2 = "Switch supply or recycle,Current: <style=\"KKeyword\">High Temperature</style>";


                    public static class S_NAMES {
                        public static LocString SUPPLY = " Supply";
                        public static LocString RECYCLE = " Recycle";

                        public static LocString HIGH_TEMPERATURE = "H-T ";
                        public static LocString LOW_TEMPERATURE = "L-T ";
                    }

                    public static class LABELS {
                        public static LocString GAS_0 = "Base Oxygen";
                        public static LocString GAS_1 = "Industrial Oxygen";
                        public static LocString GAS_2 = "Carbon Dioxide";
                        public static LocString GAS_3 = "Chlorine Gas";
                        public static LocString GAS_4 = "hydrogen";
                        public static LocString GAS_5 = "Natural Gas";
                        public static LocString GAS_6 = "Steam";
                        public static LocString GAS_7 = "Miscellaneous Gas";

                        public static LocString LIQUID_0 = "Water";
                        public static LocString LIQUID_1 = "Polluted Water";
                        public static LocString LIQUID_2 = "Concentrated Brine";
                        public static LocString LIQUID_3 = "Salt Water";
                        public static LocString LIQUID_4 = "Petroleum";
                        public static LocString LIQUID_5 = "Crude Oil";
                        public static LocString LIQUID_6 = "Coolant";
                        public static LocString LIQUID_7 = "Ethanol";
                        public static LocString LIQUID_8 = "Mixed Water";
                        public static LocString LIQUID_9 = "Liquid Hydrogen"; //液氢
                        public static LocString LIQUID_10 = "Liquid Oxygen"; //液氧
                        public static LocString LIQUID_11 = "Visco-Gel"; //粘性凝胶
                        public static LocString LIQUID_12 = "Naphtha"; //石脑油

                        public static LocString SOLID_0 = "Base Storage";
                        public static LocString SOLID_1 = "Food Refrigeration";
                        public static LocString SOLID_2 = "Anti-Virus";
                        public static LocString SOLID_3 = "Soil";
                        public static LocString SOLID_4 = "Sulfur";
                        public static LocString SOLID_5 = "Phosphorus";
                        public static LocString SOLID_6 = "Slime Mold";
                        public static LocString SOLID_7 = "Wandering Stone";
                        public static LocString SOLID_8 = "Organics";
                        public static LocString SOLID_9 = "Filtration Medium";

                        public static LocString POWER_0 = "Base Heavi-Watt Conductive Wire";
                        public static LocString POWER_1 = "Heavi-Watt Conductive Wire";
                        public static LocString POWER_2 = "Base Conductive Wire";
                        public static LocString POWER_3 = "Conductive Wire";
                        public static LocString POWER_4 = "Base Wire";
                        public static LocString POWER_5 = "Wire";

                        public static LocString HEP_0 = "Particle 1";
                        public static LocString HEP_1 = "Particle 25";
                        public static LocString HEP_2 = "Particle 50";
                        public static LocString HEP_3 = "Particle 100";
                        public static LocString HEP_4 = "Particle 500";
                        public static LocString HEP_5 = "Particle Recycle";

                        public static LocString LOGIC_0 = "Meteor Rain";
                        public static LocString LOGIC_1 = "Rocket 1";
                        public static LocString LOGIC_2 = "Rocket 2";
                        public static LocString LOGIC_3 = "Rocket 3";
                        public static LocString LOGIC_4 = "Rocket 4";

                    }
                }

                public static class WORLDDISCOVEREDSIDESCREEN {
                    public static LocString TITLE = "World Discovery Settings";
                    public static LocString HEADE = "Whether the world has been discovered or not, the undiscovered world will not calculate a lattice-by-object simulation";
                }

            }
        }
    }
}