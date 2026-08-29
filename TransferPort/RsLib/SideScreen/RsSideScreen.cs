using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RsLib {
    public class RsSideScreen : RsModule<RsSideScreen> {
        protected List<ItemInfo> itemInfos = new List<ItemInfo>();

        public RsSideScreen CopyAndCreate<TSourceScreen, TNewScreen>()
            where TSourceScreen : SideScreenContent
            where TNewScreen : RsSideScreenContent {
            var info = new ItemInfo {
                sourceScreen = typeof(TSourceScreen),
                newScreen = typeof(TNewScreen)
            };
            itemInfos.Add(info);
            return this;
        }

        public RsSideScreen Create<TScreen>()
            where TScreen : SideScreenContent {
            ItemInfo info = new ItemInfo {
                newScreen = typeof(TScreen)
            };
            itemInfos.Add(info);
            return this;
        }

        public RsSideScreen Add(Func<SideScreenContent> add, bool isPrefab) {
            ItemInfo info = new ItemInfo {
                add = add,
                isPrefab = isPrefab
            };
            itemInfos.Add(info);
            return this;
        }
        public RsSideScreen Add(SideScreenContent prefab) {
            ItemInfo info = new ItemInfo {
                prefab = prefab,
                isPrefab = true
            };
            itemInfos.Add(info);
            return this;
        }

        protected override void Initialized() {
            Harmony.Patch(
                typeof(DetailsScreen),
                "OnPrefabInit",
                null,
                new HarmonyMethod(typeof(RsSideScreen), nameof(DetailsScreen_OnPrefabInit_Patch))
            );
        }

        private static void DetailsScreen_OnPrefabInit_Patch(List<DetailsScreen.SideScreenRef> ___sideScreens) {
            var configBody = DetailsScreen.Instance?.GetTabOfType(DetailsScreen.SidescreenTabTypes.Config)?.bodyInstance;
            if (configBody is null) { return; }
            foreach (var itemInfo in Instance.itemInfos) {
                if (itemInfo.sourceScreen != null && itemInfo.newScreen != null) {
                    CreateSideScreen(___sideScreens, configBody, itemInfo.sourceScreen,
                        itemInfo.newScreen);
                }
                else if (itemInfo.newScreen != null) {
                    CreateSideScreen(___sideScreens, configBody, itemInfo.newScreen);
                }
                else if (itemInfo.add != null) {
                    AddSideScreen(___sideScreens, configBody, itemInfo.add(), itemInfo.isPrefab);
                }
                else if (itemInfo.prefab != null) {
                    AddSideScreen(___sideScreens, configBody, itemInfo.prefab, true);
                }
            }
        }


        /// <summary>
        ///     创建一个侧边栏面板
        /// </summary>
        /// <param name="existing">已经存在的面板</param>
        /// <param name="parent"></param>
        /// <typeparam name="TSourceScreen"></typeparam>
        /// <typeparam name="TNewScreen"></typeparam>
        /// <returns></returns>
        public static RsSideScreenContent CreateSideScreen(
            IList<DetailsScreen.SideScreenRef> existing, GameObject parent, Type sourceScreen, Type newScreen) {
            if (sourceScreen.IsAssignableFrom(typeof(SideScreenContent)))
                throw new TypeLoadException("参数sourceScreen不可用的，该类型必须继承" + typeof(SideScreenContent).FullName);

            if (newScreen.IsAssignableFrom(typeof(RsSideScreenContent)))
                throw new TypeLoadException("参数newScreen不可用的，该类型必须继承" + typeof(RsSideScreenContent).FullName);

            DetailsScreen.SideScreenRef retScreenRef = null;
            RsSideScreenContent ret = null;
            bool isCopySuccess = false;
            foreach (var existScreen in existing) {
                if (existScreen.screenPrefab.GetType() != sourceScreen) { continue; }
                if (existScreen.screenPrefab == null) { continue;  }
                retScreenRef = new DetailsScreen.SideScreenRef();
                ret = CopySideScreen(existScreen.screenPrefab.gameObject, parent, sourceScreen, newScreen);
                retScreenRef.name = newScreen.Name;
                retScreenRef.screenPrefab = ret;
                retScreenRef.screenInstance = ret;
                isCopySuccess = true;
                break;
            }
            if (isCopySuccess) {
                existing.Insert(0, retScreenRef);
            }
            return ret;
        }

        public static SideScreenContent CreateSideScreen(IList<DetailsScreen.SideScreenRef> existing, GameObject parent, Type newScreen) {
            if (newScreen.IsAssignableFrom(typeof(SideScreenContent)))
                throw new TypeLoadException("参数newScreen不可用的，该类型必须继承" + typeof(SideScreenContent).FullName);

            GameObject gameObject = new GameObject();
            gameObject.SetActive(true);
            gameObject.name = newScreen.Name;
            gameObject.transform.SetParent(parent.transform, false);
            SideScreenContent sideScreenContent = (SideScreenContent)gameObject.AddComponent(newScreen);

            var sideScreenRef = new DetailsScreen.SideScreenRef();
            sideScreenRef.name = gameObject.name;
            sideScreenRef.screenPrefab = sideScreenContent;
            sideScreenRef.screenInstance = sideScreenContent;
            existing.Add(sideScreenRef);
            return sideScreenContent;
        }

        private static RsSideScreenContent CopySideScreen(GameObject originalGo, GameObject parent, Type originalScreenType, Type newScreenType) {
            var newGo = UnityEngine.Object.Instantiate(originalGo, parent.transform, false);
            newGo.name = originalGo.name;
            var activeSelf = newGo.activeSelf;
            newGo.SetActive(false);

            var originalScreenCmp = (SideScreenContent)newGo.GetComponent(originalScreenType);
            var newScreenCmp = (RsSideScreenContent)newGo.AddComponent(newScreenType);

            var copyFieldDict = GetCopyFieldDict(newScreenType);

            if (copyFieldDict != null && copyFieldDict.Count > 0)
                foreach (var (newName, sourceName) in copyFieldDict) {
                    var sourceField = originalScreenType.GetField(sourceName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var newField = newScreenType.GetField(newName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (sourceField == null) throw new Exception("not found sourceField, name: " + sourceName);
                    if (newField == null) throw new Exception("not found newField, name: " + newName);

                    newField.SetValue(newScreenCmp, sourceField.GetValue(originalScreenCmp));
                }

            newScreenCmp.CopyFieldAfter();

            UnityEngine.Object.DestroyImmediate(originalScreenCmp);
            newGo.SetActive(activeSelf);
            newGo.transform.localScale = Vector3.one;
            return newScreenCmp;
        }


        public static void AddSideScreen(IList<DetailsScreen.SideScreenRef> existing, GameObject parent, SideScreenContent gameObject, bool isPrefab) {
            var sideScreenRef = new DetailsScreen.SideScreenRef();
            sideScreenRef.name = gameObject.name;
            sideScreenRef.screenPrefab = gameObject;
            if (!isPrefab) {
                sideScreenRef.screenInstance = gameObject;
            }
            existing.Add(sideScreenRef);
        }

        /// <summary>
        ///     new -> source
        /// </summary>
        public static Dictionary<string, string> GetCopyFieldDict(Type type) {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var info in fields) {
                var copyField = (CopyField)Attribute.GetCustomAttribute(info, typeof(CopyField));
                if (copyField != null) {
                    if (string.IsNullOrWhiteSpace(copyField.alias))
                        dic.Add(info.Name, info.Name);
                    else
                        dic.Add(info.Name, copyField.alias);
                }
            }

            return dic;
        }

        protected class ItemInfo {
            /// <summary>
            /// 新的sideScreen
            /// </summary>
            public Type newScreen;
            /// <summary>
            /// 原sidescreen,如果为null则直接创建
            /// </summary>
            public Type sourceScreen;

            public Func<SideScreenContent> add;

            public bool isPrefab = false;

            public SideScreenContent prefab;
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class CopyField : Attribute {
            public string alias;
            public CopyField() { }
            public CopyField(string alias) {
                this.alias = alias;
            }
        }
    }
}