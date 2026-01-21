using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RsLib {
    public static class RsUtil {
        public static Vector2 ArrowV2Offset = new Vector2(0, 0.5f);
        public static Vector3 ArrowV3Offset = new Vector3(0, 0.5f);

        public static string GetModPath(Assembly modDLL) {
            if (modDLL == null)
                throw new ArgumentNullException(nameof(modDLL));
            string str = null;
            try {
                str = Directory.GetParent(modDLL.Location)?.FullName;
            }
            catch (Exception ex) {
                Debug.LogWarning(ex);
            }

            if (str == null)
                str = Path.Combine(Manager.GetDirectory(), modDLL.GetName()?.Name ?? "");
            return str;
        }

        public static void ContrastSet<T>(ISet<T> source, ISet<T> target, Action<T> onAdd = null, Action<T> onRemove = null) {
            ISet<T> oldTarget = new HashSet<T>(target);
            foreach (T x1 in source) {
                if (!target.Contains(x1)) {
                    target.Add(x1);
                    onAdd?.Invoke(x1);
                }
                else {
                    oldTarget.Remove(x1);
                }
            }
            foreach (T x1 in oldTarget) {
                target.Remove(x1);
                onRemove?.Invoke(x1);
            }
        }

        /// <summary>
        /// 散点就近有序连接排序
        /// </summary>
        public static void NearestSort(Vector2[] points) {
            int len = points.Length;
            if (len < 2) {
                return;
            }
            //先获取最近零点
            int sp = NearestPoint(Vector2.zero, points, 0);
            //交换
            (points[0], points[sp]) = (points[sp], points[0]);
            for (int i = 1; i < len; i++) {
                //查找最近的点
                int point = NearestPoint(points[i - 1], points, i);
                //交换
                (points[point], points[i]) = (points[i], points[point]);
            }
        }

        public static void NearestSort(IList<GameObject> points) {
            int len = points.Count;
            if (len < 2) {
                return;
            }
            //先获取最近零点
            int sp = NearestGo(null, points, 0);
            //交换
            (points[0], points[sp]) = (points[sp], points[0]);
            for (int i = 1; i < len; i++) {
                //查找最近的点
                int point = NearestGo(points[i - 1], points, i);
                //交换
                (points[point], points[i]) = (points[i], points[point]);
            }
        }

        public static int NearestGo(GameObject target, IList<GameObject> points, int startIndex) {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Count; i++) {
                float magnitude = Vector2.SqrMagnitude((target == null ? Vector3.zero : target.transform.position) - points[i].transform.position);
                if (magnitude < shortest) {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }

        public static void NearestSort(GameObject[] points) {
            int len = points.Length;
            if (len < 2) {
                return;
            }
            //先获取最近零点
            int sp = NearestGo(null, points, 0);
            //交换
            (points[0], points[sp]) = (points[sp], points[0]);
            for (int i = 1; i < len; i++) {
                //查找最近的点
                int point = NearestGo(points[i - 1], points, i);
                //交换
                (points[point], points[i]) = (points[i], points[point]);
            }
        }

        public static int NearestGo(GameObject target, GameObject[] points, int startIndex) {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Length; i++) {
                float magnitude = Vector2.SqrMagnitude((target == null ? Vector3.zero : target.transform.position) - points[i].transform.position);
                if (magnitude < shortest) {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }

        public static int NearestPoint(Vector2 target, Vector2[] points, int startIndex) {
            float shortest = float.MaxValue;
            int shortestIdx = startIndex;
            for (var i = startIndex; i < points.Length; i++) {
                float magnitude = Vector2.SqrMagnitude(target - points[i]);
                if (magnitude < shortest) {
                    shortest = magnitude;
                    shortestIdx = i;
                }
            }

            return shortestIdx;
        }

        public static void SetParent(this GameObject target, GameObject parent, bool worldPositionStays = true) {
            if (target == null) {
                Debug.LogWarning("target is null");
                return;
            }
            target.transform.SetParent(parent != null ? parent.transform : null, worldPositionStays);
        }

        /// <summary>
        /// 不重复设置active
        /// </summary>
        /// <param name="go"></param>
        /// <param name="active"></param>
        public static void SetActiveNR(this GameObject go, bool active) {
            if (go.activeSelf == active) {
                return;
            }
            go.SetActive(active);
        }

        public static bool IsNullOrDestroyed(Object obj) {
            if (obj == null)
                return true;
            return (obj as UnityEngine.Object) is object && obj as UnityEngine.Object == (UnityEngine.Object)null;
        }

        /// <summary>
        ///     获取指定的class下的所有静态属性（深度遍历）
        /// </summary>
        public static Dictionary<string, object> FlatFields(Type type) {
            var dir = new Dictionary<string, object>();
            FlatFields(type, "", dir);
            return dir;
        }

        private static void FlatFields(Type type, string prefix, Dictionary<string, object> container) {
            if (string.IsNullOrEmpty(prefix))
                prefix = type.Name;
            else
                prefix = prefix + "." + type.Name;

            //普通属性
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var t in fieldInfos) { container.Add(prefix + "." + t.Name, t.GetValue(null)); }

            //类
            var nestedTypes = type.GetNestedTypes(BindingFlags.Public);
            foreach (var nestedType in nestedTypes) FlatFields(nestedType, prefix, container);
        }

        public static Vector3 Center(this ICollection<GameObject> points) {
            if (points == null || points.Count <= 0) { return Vector3.zero; }

            var total = points.Count;
            var center = Vector3.zero;

            foreach (var point in points) {
                if (point != null) { center += point.transform.position; }
                else { total -= 1; }
            }
            center /= total;
            return center;
        }

        public static Vector2 Center(this ICollection<Vector2> points) {
            if (points == null || points.Count <= 0) { return Vector2.zero; }

            var total = points.Count;
            var center = Vector2.zero;

            foreach (var point in points) {
                if (point != null) { center += point; }
                else { total -= 1; }
            }
            center /= total;
            return center;
        }

        public static void SetPositionXY(this Transform transform, Vector3 xy) {
            var position = transform.position;
            position.x = xy.x;
            position.y = xy.y;
            transform.position = position;
        }
    }
}