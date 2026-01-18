using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RsLib {
    public static class RsUITuning {
        public static class Images {
            private static readonly IDictionary<string, Sprite> SPRITES = new Dictionary<string, Sprite>(512);

            public static Sprite Arrow { get; }

            public static Sprite BoxBorder { get; }

            public static Sprite BoxBorderWhite { get; }

            public static Sprite ButtonBorder { get; }

            public static Sprite CheckBorder { get; }

            public static Sprite Checked { get; }

            public static Sprite Close { get; }

            public static Sprite Contract { get; }

            public static Sprite Expand { get; }

            public static Sprite Partial { get; }

            public static Sprite ScrollBorderHorizontal { get; }

            public static Sprite ScrollHandleHorizontal { get; }

            public static Sprite ScrollBorderVertical { get; }

            public static Sprite ScrollHandleVertical { get; }

            public static Sprite SliderHandle { get; }

            static Images() {
                foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>()) {
                    string name = sprite?.name;
                    if (!string.IsNullOrEmpty(name) && !SPRITES.ContainsKey(name))
                        SPRITES.Add(name, sprite);
                }

                Arrow = GetSpriteByName("game_speed_play");
                BoxBorder = GetSpriteByName("web_box");
                BoxBorderWhite = GetSpriteByName("web_border");
                ButtonBorder = GetSpriteByName("web_button");
                CheckBorder = GetSpriteByName("overview_jobs_skill_box");
                Checked = GetSpriteByName("overview_jobs_icon_checkmark");
                Close = GetSpriteByName("cancel");
                Contract = GetSpriteByName("iconDown");
                Expand = GetSpriteByName("iconRight");
                Partial = GetSpriteByName("overview_jobs_icon_mixed");
                ScrollBorderHorizontal = GetSpriteByName("build_menu_scrollbar_frame_horizontal");
                ScrollHandleHorizontal = GetSpriteByName("build_menu_scrollbar_inner_horizontal");
                ScrollBorderVertical = GetSpriteByName("build_menu_scrollbar_frame");
                ScrollHandleVertical = GetSpriteByName("build_menu_scrollbar_inner");
                SliderHandle = GetSpriteByName("game_speed_selected_med");
            }

            public static Sprite GetSpriteByName(string name) {
                Sprite sprite;
                if (!SPRITES.TryGetValue(name, out sprite))
                    sprite = null;
                return sprite;
            }
        }

        public static class Colors {
            public static Color BackgroundLight { get; } =
                new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

            public static ColorStyleSetting ButtonPinkStyle { get; }

            public static ColorStyleSetting ButtonBlueStyle { get; }

            public static ColorStyleSetting ComponentDarkStyle { get; }

            public static ColorStyleSetting ComponentLightStyle { get; }

            public static Color DialogBackground { get; } = new Color32(0, 0, 0, byte.MaxValue);

            public static Color DialogDarkBackground { get; } = new Color32(48, 52, 67, byte.MaxValue);

            public static Color OptionsBackground { get; } = new Color32(31, 34, 43, byte.MaxValue);

            public static ColorBlock ScrollbarColors { get; }

            public static Color SelectionBackground { get; } = new Color32(189, 218, byte.MaxValue, byte.MaxValue);

            public static Color SelectionForeground { get; } = new Color32(0, 0, 0, byte.MaxValue);

            public static Color Transparent { get; } = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

            public static Color UITextDark { get; }

            public static Color UITextLight { get; } =
                new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

            static Colors() {
                UITextDark = new Color32(0, 0, 0, byte.MaxValue);
                Color color1 = new Color(0.0f, 0.0f, 0.0f);
                Color color2 = new Color(0.784f, 0.784f, 0.784f, 1f);
                ComponentLightStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                ComponentLightStyle.activeColor = color1;
                ComponentLightStyle.inactiveColor = color1;
                ComponentLightStyle.hoverColor = color1;
                ComponentLightStyle.disabledActiveColor = color2;
                ComponentLightStyle.disabledColor = color2;
                ComponentLightStyle.disabledhoverColor = color2;
                color1 = new Color(1f, 1f, 1f);
                ComponentDarkStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                ComponentDarkStyle.activeColor = color1;
                ComponentDarkStyle.inactiveColor = color1;
                ComponentDarkStyle.hoverColor = color1;
                ComponentDarkStyle.disabledActiveColor = color2;
                ComponentDarkStyle.disabledColor = color2;
                ComponentDarkStyle.disabledhoverColor = color2;
                ButtonPinkStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                ButtonPinkStyle.activeColor = new Color(0.7941176f, 0.4496107f, 0.6242238f);
                ButtonPinkStyle.inactiveColor = new Color(0.5294118f, 0.2724914f, 0.4009516f);
                ButtonPinkStyle.disabledColor = new Color(0.4156863f, 0.4117647f, 0.4f);
                ButtonPinkStyle.disabledActiveColor = Transparent;
                ButtonPinkStyle.hoverColor = new Color(0.6176471f, 0.3315311f, 0.4745891f);
                ButtonPinkStyle.disabledhoverColor = new Color(0.5f, 0.5f, 0.5f);
                ButtonBlueStyle = ScriptableObject.CreateInstance<ColorStyleSetting>();
                ButtonBlueStyle.activeColor = new Color(0.5033521f, 0.5444419f, 0.6985294f);
                ButtonBlueStyle.inactiveColor = new Color(0.2431373f, 0.2627451f, 0.3411765f);
                ButtonBlueStyle.disabledColor = new Color(0.4156863f, 0.4117647f, 0.4f);
                ButtonBlueStyle.disabledActiveColor = new Color(0.625f, 0.6158088f, 0.5882353f);
                ButtonBlueStyle.hoverColor = new Color(0.3461289f, 0.3739619f, 0.4852941f);
                ButtonBlueStyle.disabledhoverColor = new Color(0.5f, 0.4898898f, 0.4595588f);
                ScrollbarColors = new ColorBlock {
                    colorMultiplier = 1f,
                    fadeDuration = 0.1f,
                    disabledColor = new Color(0.392f, 0.392f, 0.392f),
                    highlightedColor = new Color32(161, 163, 174, byte.MaxValue),
                    normalColor = new Color32(161, 163, 174, byte.MaxValue),
                    pressedColor = BackgroundLight
                };
            }
        }

        public class Prefabs {
            [Resource("LocTextInputField")]
            public static KInputTextField InputTextField;

            /// <summary>
            /// 	|- 	InputField	[-403866 active 0x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.LayoutElement | KInputTextField | UnityEngine.UI.Image | KNumberInputField]
            /// 			|- 	Text Area	[-403880 active 0x0]	[UnityEngine.RectTransform]
            /// 				|- 	InputField(Clone) Input Caret	[-403900 active 0x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | TMPro.TMP_SelectionCaret | UnityEngine.UI.LayoutElement]
            /// 				|- 	Placeholder	[-403884 active 0x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | LocText | SetTextStyleSetting]
            /// </summary>

            [Resource("InputField")]
            public static KInputField InputField;


            /// <summary>
            /// |- 	Checkbox	[29124 active 24x24 anchor:0x1x0x1 rect:0x12x24x-12 offset:196x-28.5x220x-4.5]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | MultiToggle | UnityEngine.UI.LayoutElement | ToolTip | HierarchyReferences]
            ///     |- 	BG	[32418 active -20.43x-3.08 anchor:0x0x1x1 rect:-1.785x10.46x1.785x-10.46 offset:19x1.38x-1.43x-1.7]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            ///     |- 	Border	[32452 active 0x0 anchor:0x0x1x1 rect:-12x12x12x-12 offset:0x0x0x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            ///     |- 	ToggledIcon	[30084 active -4x-4 anchor:0x0x1x1 rect:-10x10x10x-10 offset:2x2x-2x-2]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            /// </summary>
            [Resource("Checkbox")]
            public static MultiToggle Checkbox;

            /// <summary>
            ///|- 	toggleEntry	[-398960 noActive 310x22 anchor:0x1x0x1 rect:-155x11x155x-11 offset:0x-24x310x-2]	[UnityEngine.RectTransform | UnityEngine.UI.HorizontalLayoutGroup | UnityEngine.UI.LayoutElement]
            ///	|- 	Label	[-398968 active 207x20.73 anchor:0x1x0x1 rect:-103.5x10.365x103.5x-10.365 offset:10x-21.365x217x-0.6350002]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | LocText | UnityEngine.UI.LayoutElement]
            ///		>> [text:Parameter]
            ///	|- 	Toggle	[-398978 active 89x20 anchor:0x1x0x1 rect:-44.5x10x44.5x-10 offset:221x-21x310x-1]	[UnityEngine.RectTransform | MultiToggle | UnityEngine.UI.LayoutElement]
            ///		|- 	collider	[-398986 active 150x0 anchor:0x0x1x1 rect:-119.5x10x119.5x-10 offset:-150x0x0x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            ///		|- 	Background	[-398994 active 20x20 anchor:0x1x0x1 rect:-10x10x10x-10 offset:0x-20x20x0]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            ///			|- 	Checkmark	[-399002 active -10x-10 anchor:0x0x1x1 rect:-5x5x5x-5 offset:5x5x-5x-5]	[UnityEngine.RectTransform | UnityEngine.CanvasRenderer | UnityEngine.UI.Image]
            /// </summary>
            [Resource("toggleEntry")]
            public static GameObject ToggleEntry;


            static Prefabs() {
                GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject>();
                Dictionary<string, FieldInfo> dictionary = ResourceAttribute.GetFields(typeof(Prefabs));
                foreach (GameObject go in gos) {
                    if (dictionary.TryGetValue(go.name, out FieldInfo fieldInfo)) {
                        if (fieldInfo.FieldType == typeof(GameObject)) {
                            fieldInfo.SetValue(null, go);
                        }
                        else {
                            Component component = go.GetComponent(fieldInfo.FieldType);
                            if (component != null) {
                                fieldInfo.SetValue(null, component);
                            }
                        }
                    }
                }
            }
        }

        public class ScriptableObjects {
            [Resource] public static TextStyleSetting style_labelText;
            [Resource] public static TextStyleSetting style_titleTextBlack;

            static ScriptableObjects() {
                TextStyleSetting[] settings = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
                Dictionary<string, FieldInfo> dictionary = ResourceAttribute.GetFields(typeof(ScriptableObjects));
                foreach (TextStyleSetting setting in settings) {
                    if (dictionary.TryGetValue(setting.name, out FieldInfo fieldInfo)) {
                        fieldInfo.SetValue(null, setting);
                    }
                }
            }
        }

        public class TextStyleSettings {
            private static Dictionary<string, TextStyleSetting> textStyleSettings = new Dictionary<string, TextStyleSetting>();

            [Resource]
            public static TextStyleSetting style_labelText;
            [Resource]
            public static TextStyleSetting style_titleText_NoWrapping;
            [Resource]
            public static TextStyleSetting style_titleTextBlack;
            [Resource]
            public static TextStyleSetting style_bodyText;
            [Resource]
            public static TextStyleSetting style_bodyTextSmall;

            static TextStyleSettings() {
                TextStyleSetting[] settings = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
                foreach (TextStyleSetting setting in settings) {
                    if (!textStyleSettings.ContainsKey(setting.name)) {
                        textStyleSettings[setting.name] = setting;
                    }
                }

                Dictionary<string, FieldInfo> dictionary = ResourceAttribute.GetFields(typeof(TextStyleSettings));
                foreach (var keyValuePair in dictionary) {
                    keyValuePair.Value.SetValue(null, GetTextStyleSetting(keyValuePair.Key));
                }
            }

            public static TextStyleSetting GetTextStyleSetting(string name) {
                textStyleSettings.TryGetValue(name, out var setting);
                return setting;
            }

            public static void AddTextStyleSetting(TextStyleSetting setting) {
                textStyleSettings[setting.name] = setting;
            }
        }


        [AttributeUsage(AttributeTargets.Field)]
        private class ResourceAttribute : Attribute {
            public string alias;

            public ResourceAttribute() {
            }

            public ResourceAttribute(string alias) {
                this.alias = alias;
            }

            public static Dictionary<string, FieldInfo> GetFields(Type type) {
                FieldInfo[] fieldInfos =
                    type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                Dictionary<string, FieldInfo> map = new Dictionary<string, FieldInfo>();
                foreach (FieldInfo fieldInfo in fieldInfos) {
                    ResourceAttribute resourceAttribute = fieldInfo.GetCustomAttribute<ResourceAttribute>();
                    if (resourceAttribute != null) {
                        if (resourceAttribute.alias != null) {
                            map[resourceAttribute.alias] = fieldInfo;
                        }
                        else {
                            map[fieldInfo.Name] = fieldInfo;
                        }
                    }
                }

                return map;
            }

        }



    }
}