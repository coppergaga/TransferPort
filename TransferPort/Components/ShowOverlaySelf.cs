namespace RsTransferPort {
    public class ShowOverlaySelf : KMonoBehaviour {
        [MyCmpReq] private PortItem channelItem;

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            Unsubscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
        }

        private void OnRefreshUserMenu(object data) =>
            Game.Instance.userMenu.AddButton(this.gameObject,
                new KIconButtonMenu.ButtonInfo(
                    "show_overlay_self_icon",
                    RsTransferPort.STRINGS.UI.USERMENU.SHOWOVERLAYSELF_BUTTON.NAME,
                    ShowOverlay,
                    Action.BuildingUtility1,
                    tooltipText: RsTransferPort.STRINGS.UI.USERMENU.SHOWOVERLAYSELF_BUTTON.TOOLTIP)
                );


        public void ShowOverlay() {
            if (OverlayScreen.Instance == null) {
                return;
            }
            MyOverlayModes.PortChannel.ActiveChannel(channelItem.ChannelKey);
            if (OverlayScreen.Instance.mode != MyOverlayModes.PortChannel.ID) {
                OverlayScreen.Instance.ToggleOverlay(MyOverlayModes.PortChannel.ID);
            }
        }
    }
}