using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RsLib.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RsLib.Builder
{
    public class RsGameObjectBuilder
    {
        private static BuilderManager builderManager;


        static RsGameObjectBuilder()
        {
            builderManager = new BuilderManager();
            
            builderManager.Add(new GameObjectBuilder());
            
            builderManager.Add(new BuilderFn<Component>(o =>
            {
                o.AppendPrefix();
                o.Append("[")
                    .Append(o.target.GetType().FullName);
                o.Append("]");
            }));

            
            builderManager.Add(new BuilderFn<MonoBehaviour>(o =>
            {
                o.AppendKeyValue("enabled", o.target.enabled);
            }));
            
            builderManager.Add(new BuilderFn<Transform>(o =>
            {
                o.AppendKeyValue("position", o.target.position);
                o.AppendKeyValue("localPosition", o.target.localPosition);
                o.AppendKeyValue("localScale", o.target.localScale);
            }));
            
            builderManager.Add(new BuilderFn<RectTransform>(o =>
            {
                o.AppendKeyValue("sizeDelta", o.target.sizeDelta);
                o.AppendKeyValue("anchorMax", o.target.anchorMax);
                o.AppendKeyValue("anchorMin", o.target.anchorMin);
                o.AppendKeyValue("anchoredPosition", o.target.anchoredPosition);
                o.AppendKeyValue("pivot", o.target.pivot);
                o.AppendKeyValue("rect", o.target.rect);
            }));

            builderManager.Add(new BuilderFn<Image>(o => { o.AppendKeyValue("sprite", o.target.sprite != null ? o.target.sprite.name : null, true); }));

            builderManager.Add(new BuilderFn<TMP_Text>(o =>
            {
                o.AppendKeyValue("text", o.target.text, true);
                // o.AppendKeyValue("font", o.target.font);
                // if (o.target.font != null)
                // {
                //     o.AppendKeyValue("atlas", o.target.font.atlas);
                // }
                // o.AppendKeyValue("fontMaterial", o.target.fontMaterial);
                o.AppendKeyValue("autoSize", o.target.autoSizeTextContainer);
                o.AppendKeyValue("fontSize", o.target.fontSize);
                o.AppendKeyValue("fontSizeMax", o.target.fontSizeMax);
                o.AppendKeyValue("fontSizeMin", o.target.fontSizeMin);
            }));

            builderManager.Add(new BuilderFn<LayoutElement>(o =>
            {
                IEnumerable<FieldInfo> fieldInfos = GetSerializeFields(typeof(LayoutElement));
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    o.AppendKeyValue(fieldInfo.Name, fieldInfo.GetValue(o.target));
                }
            }));
            
            builderManager.Add(new BuilderFn<MultiToggle>(o =>
            {
                o.CheckAddendSpace();
                o.Append("states:{");
                foreach (ToggleState toggleState in o.target.states)
                {
                    o.Append("(");
                    o.AppendKeyValue("name",toggleState.Name);
                    o.AppendKeyValue("sprite",toggleState.sprite != null ? toggleState.sprite.name : null);
                    o.Append(")");
                }
                o.Append("}");
            }));
            
            builderManager.Add(new BuilderFn<LocText>(o =>
            {
               o.AppendKeyValue("textStyleSetting", o.target.textStyleSetting != null ? o.target.textStyleSetting.name : null);
               o.AppendKeyValue("allowOverride", o.target.allowOverride);
            }));
            
            builderManager.Add(new BuilderFn<SetTextStyleSetting>(o =>
            {
                TextStyleSetting styleSetting = RsField.GetValue(o.target,"style") as TextStyleSetting;
                o.AppendKeyValue("textStyleSetting", styleSetting != null ? styleSetting.name : null );
            }));
            builderManager.Add(new BuilderFn<RsHierarchyReferences>(o =>
            {
                GameObject[] references = o.target.references;
                if (references != null)
                {
                    o.CheckAddendSpace();
                    o.Append("references:[");
                    foreach (GameObject reference in references)
                    {
                        o.Append("{");
                        o.AppendKeyValue("name", reference.name);
                        o.Append("}");
                        o.Append(",");
                    }
                    o.Append("]");
                }
                else
                {
                    o.AppendKeyValue("references", null);
                }

            }));
            
        }

        public static string BuildToString(GameObject go)
        {
            return builderManager.BuildToString(go, "", true);
        }

        public static IEnumerable<FieldInfo> GetSerializeFields(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            IEnumerable<FieldInfo> sField = fields.Where(o =>
                (o.IsPublic && o.GetCustomAttribute<NonSerializedAttribute>() == null)
                || (!o.IsPublic && o.GetCustomAttribute<SerializeField>() != null));
            return sField;
        }
    }
}