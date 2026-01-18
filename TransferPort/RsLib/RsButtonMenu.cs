using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace RsLib {
    /// <summary>
    /// 在使用自定义用户菜单按钮需要的图标可以在这里添加
    /// </summary>
    public class RsButtonMenu : RsModule<RsButtonMenu> {
        private readonly HashSet<Sprite> sprites = new HashSet<Sprite>();

        public RsButtonMenu AddIcon(Sprite sprite) {
            sprites.Add(sprite);
            return this;
        }

        public RsButtonMenu AddIcon(string name, Sprite sprite) {
            if (sprite.name != name) {
                sprite = Object.Instantiate(sprite);
                sprite.name = name;
            }
            sprites.Add(sprite);
            return this;
        }

        protected override void Initialized() {
            Harmony.Patch(typeof(KIconButtonMenu), "OnPrefabInit",
                prefix: new HarmonyMethod(typeof(RsButtonMenu), nameof(KIconButtonMenu_OnPrefabInit_Prefix)));
        }

        protected static void KIconButtonMenu_OnPrefabInit_Prefix(KIconButtonMenu __instance) {
            Sprite[] icons = (Sprite[])RsField.GetValue(__instance, "icons");
            int newLen = Instance.sprites.Count + icons.Length;

            List<Sprite> newIcons = new List<Sprite>(newLen);
            newIcons.AddRange(icons);
            newIcons.AddRange(Instance.sprites);

            RsField.SetValue(__instance, "icons", newIcons.ToArray());
        }
    }
}