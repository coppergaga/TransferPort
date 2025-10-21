using STRINGS;
using System.Collections.Generic;
using RsLib;
using UnityEngine;

namespace RsTransferPort
{
    public class MyHighEnergyParticleDirectionSideScreen : RsSideScreenContent
    {
        private IMyHighEnergyParticleDirection target;

        [RsSideScreen.CopyField] public List<KButton> Buttons;
        [RsSideScreen.CopyField] private KButton activeButton;
        [RsSideScreen.CopyField] public LocText directionLabel;

        private string[] directionStrings = new string[8]
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

        public override string GetTitle() => (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.TITLE;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            for (int index = 0; index < this.Buttons.Count; ++index)
            {
                KButton button = this.Buttons[index];
                button.onClick += (System.Action) (() =>
                {
                    int num = this.Buttons.IndexOf(button);
                    if ((UnityEngine.Object) this.activeButton != (UnityEngine.Object) null)
                        this.activeButton.isInteractable = true;
                    button.isInteractable = false;
                    this.activeButton = button;
                    if (this.target == null)
                        return;
                    this.target.Direction = EightDirectionUtil.AngleToDirection(num * 45);
                    Game.Instance.ForceOverlayUpdate(true);
                    this.Refresh();
                });
            }
        }

        public override int GetSideScreenSortOrder() => 10;

        public override bool IsValidForTarget(GameObject target)
        {
            return target.GetComponent<IMyHighEnergyParticleDirection>() != null;
        }

        public override void SetTarget(GameObject new_target)
        {
            if (RsUtil.IsNullOrDestroyed(new_target))
            {
                return;
            }
            target = new_target.GetComponent<IMyHighEnergyParticleDirection>();
            this.Refresh();
        }

        private void Refresh()
        {
            int directionIndex = EightDirectionUtil.GetDirectionIndex(this.target.Direction);
            if (directionIndex >= 0 && directionIndex < this.Buttons.Count)
            {
                this.Buttons[directionIndex].SignalClick(KKeyCode.Mouse0);
            }
            else
            {
                if ((bool) (UnityEngine.Object) this.activeButton)
                    this.activeButton.isInteractable = true;
                this.activeButton = (KButton) null;
            }

            this.directionLabel.SetText(string.Format(
                (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.SELECTED_DIRECTION,
                (object) this.directionStrings[directionIndex]));
        }
    }
}