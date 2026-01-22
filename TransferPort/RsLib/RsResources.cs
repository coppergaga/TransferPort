using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace RsLib {
    public class RsResources {
        public delegate Object CreateDelegate();

        public delegate GameObject CreatePrefabDelegate(GameObject prefabParent);

        private static GameObject m_prefabParent;
        private static readonly Dictionary<ResourcesKey, Object> container = new Dictionary<ResourcesKey, Object>();
        private static readonly Dictionary<string, MulticastDelegate> loadTask = new Dictionary<string, MulticastDelegate>();
        private static readonly List<AssetBundle> assetBundles = new List<AssetBundle>();
        private static readonly List<SpriteAtlas> spriteAtlases = new List<SpriteAtlas>();

        public static GameObject PrefabParent {
            get {
                if (m_prefabParent == null) {
                    m_prefabParent = new GameObject();
                    m_prefabParent.SetActive(false);
                    m_prefabParent.name = "RsPrefabParent";
                    Object.DontDestroyOnLoad(m_prefabParent);
                }

                return m_prefabParent;
            }
        }

        public static GameObject Create() {
            GameObject parent = PrefabParent;
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(parent.transform);
            return gameObject;
        }

        public static Object LoadByAssetBundle(string path, Type tType) {
            Object obj = null;
            foreach (AssetBundle assetBundle in assetBundles) {
                obj = assetBundle.LoadAsset(path, tType);
                if (obj != null) {
                    break;
                }
            }
            return obj;
        }

        public static Object LoadByAssetBundle(string path) {
            Object obj = null;
            foreach (AssetBundle assetBundle in assetBundles) {
                obj = assetBundle.LoadAsset(path);
                if (obj != null) {
                    break;
                }
            }

            return obj;
        }

        public static T Load<T>(string path)
        where T : Object {
            Type tType = typeof(T);
            ResourcesKey resourcesKey = new ResourcesKey(path, tType);

            if (container.TryGetValue(resourcesKey, out Object obj)) {
                if (!Util.IsNullOrDestroyed(obj)) {
                    return (T)obj;
                }
            }

            obj = LoadByTask(path, tType);

            if (obj == null) {
                obj = LoadByAssetBundle(path, tType);
            }

            if (obj == null) {
                Debug.LogErrorFormat("RsResources no find, path:{0}, type:{1}", path, tType.FullName);
            }

            container[resourcesKey] = obj;
            return (T)obj;
        }

        private static Object LoadByTask(string path, Type tType) {
            Object obj = null;
            //从任务中获取
            if (loadTask.TryGetValue(path, out var create)) {
                if (create is CreatePrefabDelegate createPrefab) {
                    obj = createPrefab.Invoke(PrefabParent);
                }
                else if (create is CreateDelegate createPrefab2) {
                    obj = createPrefab2.Invoke();
                }

                //转为GameObject
                if (typeof(Component).IsAssignableFrom(tType) && obj is GameObject go) {
                    obj = go.GetComponent(tType);
                }

            }

            return obj;
        }

        public static void AddLoadTask(string path, CreateDelegate create) {
            loadTask[path] = create;
        }

        public static void AddLoadPrefabTask(string path, CreatePrefabDelegate create) {
            loadTask[path] = create;
        }

        public static void AddAssetBundle(AssetBundle assetBundle, bool autoLoadSpriteAtlas = true) {
            if (!assetBundles.Contains(assetBundle)) {
                assetBundles.Add(assetBundle);
                if (autoLoadSpriteAtlas) {
                    SpriteAtlas[] atlases = assetBundle.LoadAllAssets<SpriteAtlas>();
                    if (atlases != null) {
                        spriteAtlases.AddRange(atlases);
                    }
                }
            }
        }

        public static void AddSpriteAtlas(SpriteAtlas spriteAtlas) {
            spriteAtlases.Add(spriteAtlas);
        }

        private static Sprite LoadSpriteBySpriteAtlas(string name) {
            foreach (SpriteAtlas atlases in spriteAtlases) {
                Sprite sprite = atlases.GetSprite(name);
                if (sprite != null) {
                    sprite.name = name;
                    return sprite;
                }
            }
            return null;
        }


        struct ResourcesKey : IEquatable<ResourcesKey> {
            private int hash;
            private string name;
            private Type type;

            public ResourcesKey(string name, Type type) : this() {
                this.name = name;
                this.type = type;
                this.hash = ((name != null ? name.GetHashCode() : 0) * 397) ^ (type != null ? type.GetHashCode() : 0);
            }


            public bool Equals(ResourcesKey other) {
                return name == other.name && Equals(type, other.type);
            }

            public override bool Equals(object obj) {
                return obj is ResourcesKey other && Equals(other);
            }

            public override int GetHashCode() {
                return hash;
            }
        }
    }
}