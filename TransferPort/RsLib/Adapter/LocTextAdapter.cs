using System.Reflection;
using TMPro;

namespace RsLib.Adapter {
    public class LocTextAdapter : LocText {
        private MethodInfo LoclTextAwake = typeof(LocText).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        public string textStyleSettingName;

        protected override void Awake() {
            if (string.IsNullOrWhiteSpace(textStyleSettingName)
                || (textStyleSetting = RsUITuning.TextStyleSettings.GetTextStyleSetting(textStyleSettingName)) == null) {
                textStyleSetting = RsUITuning.TextStyleSettings.style_bodyText;

            }

            allowOverride = false;

            font = TMP_FontAsset.defaultFontAsset;

            LoclTextAwake.Invoke(this, new object[0]);

        }

        public void SetTextNoRepeat(string text) {
            if (this.text != text) {
                SetText(text);
            }
        }
    }

}