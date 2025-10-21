using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RsLib.Adapter
{
    public class KInputTextFieldAdapter : TMP_InputField
    {
        public KInputTextFieldAdapter() => this.onFocus = this.onFocus + (System.Action) (() =>
        {
            onEndEdit.AddListener(FixInput);
            
            if (!SteamGamepadTextInput.IsActive())
                return;
            SteamGamepadTextInput.ShowTextInputScreen("", this.text, new System.Action<SteamGamepadTextInputData>(this.OnGamepadInputDismissed));
        });

        private void OnGamepadInputDismissed(SteamGamepadTextInputData data)
        {
            if (data.submitted)
                this.text = data.input;
            this.OnDeselect((BaseEventData) null);
        }

        private void FixInput(string str)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Input.ResetInputAxes();
            }
        }
    }
}