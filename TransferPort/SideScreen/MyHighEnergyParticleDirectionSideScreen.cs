using RsLib;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace RsTransferPort {
    /// <summary>
    /// 辐射端口侧边方向选择面板
    /// </summary>
    public class MyHighEnergyParticleDirectionSideScreen : RsSideScreenContent {
        private IMyHighEnergyParticleDirection target;

        [RsSideScreen.CopyField] public List<KButton> Buttons;
        [RsSideScreen.CopyField] private KButton activeButton;
        [RsSideScreen.CopyField] public LocText directionLabel;

        private readonly string[] directionStrings = new string[8]
        {
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_N,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NW,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_W,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SW,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_S,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SE,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_E,
            (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NE
        };

        public override string GetTitle() => (string)UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.TITLE;

        protected override void OnSpawn() {
            base.OnSpawn();
            for (int index = 0; index < Buttons.Count; ++index) {
                var button = Buttons[index];
                int num = index;
                button.onClick += () => {
                    if (activeButton != null) { activeButton.isInteractable = true; }
                    button.isInteractable = false;
                    activeButton = button;

                    if (target == null) { return; }
                    target.Direction = EightDirectionUtil.AngleToDirection(num * 45);
                    Game.Instance.ForceOverlayUpdate(true);
                    Refresh();
                };
            }
        }

        protected override void OnDeactivate() {
            base.OnDeactivate();
            foreach (var button in Buttons) { button.ClearOnClick(); }
        }

        public override int GetSideScreenSortOrder() => 10;

        public override bool IsValidForTarget(GameObject target) {
            return target.GetComponent<IMyHighEnergyParticleDirection>() != null;
        }

        public override void SetTarget(GameObject new_target) {
            if (Util.IsNullOrDestroyed(new_target)) {
                return;
            }
            target = new_target.GetComponent<IMyHighEnergyParticleDirection>();
            Refresh();
        }

        private void Refresh() {
            int directionIndex = EightDirectionUtil.GetDirectionIndex(target.Direction);
            if (directionIndex >= 0 && directionIndex < Buttons.Count) {
                Buttons[directionIndex].SignalClick(KKeyCode.Mouse0);
            }
            else {
                if (!Util.IsNullOrDestroyed(activeButton)) { activeButton.isInteractable = true; }
                activeButton = null;
            }

            directionLabel.SetText(string.Format(
                (string)UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.SELECTED_DIRECTION,
                directionStrings[directionIndex]));
        }
    }
}