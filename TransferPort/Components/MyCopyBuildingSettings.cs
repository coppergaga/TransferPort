using STRINGS;

namespace RsTransferPort
{
    public class MyCopyBuildingSettings : CopyBuildingSettings
    {
        private static readonly EventSystem.IntraObjectHandler<MyCopyBuildingSettings> OnRefreshUserMenuDelegate2
            = new EventSystem.IntraObjectHandler<MyCopyBuildingSettings>((component, data) => component.OnRefreshUserMenu(data));

        public string buttonText = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME;
        public string tooltipText = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP;

        protected override void OnPrefabInit()
        {
            Subscribe(493375141, OnRefreshUserMenuDelegate2);
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(gameObject,
                new KIconButtonMenu.ButtonInfo("action_mirror", buttonText,
                    ActivateCopyTool, Action.BuildingUtility1,
                    tooltipText: tooltipText));
        }

        private void ActivateCopyTool()
        {
            CopySettingsTool.Instance.SetSourceObject(gameObject);
            PlayerController.Instance.ActivateTool(CopySettingsTool.Instance);
        }
    }
}