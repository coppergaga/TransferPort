using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KMod;
using UnityEngine;

namespace RsLib {
    public static class RsUtil {
        public static Vector2 ArrowV2Offset = new Vector2(0, 0.5f);
        public static Vector3 ArrowV3Offset = new Vector3(0, 0.5f);

        public static int IntFrom(bool b) => b ? 1 : -1;
        public static bool BoolFrom(int i) => i > 0;

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
            if (str == null) { str = Path.Combine(Manager.GetDirectory(), modDLL.GetName()?.Name ?? ""); }
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

        public static void NearestSort<T>(IList<T> points, Func<T, GameObject> getGo) {
            int len = points.Count;
            if (len < 2) { return; }
            int sp = NearestGo(null, points, 0, getGo);
            (points[0], points[sp]) = (points[sp], points[0]);
            for (int i = 1; i < len; i++) {
                int p = NearestGo(getGo(points[i - 1]), points, i, getGo);
                (points[i], points[p]) = (points[p], points[i]);
            }
        }

        public static int NearestGo<T>(GameObject target, IList<T> points, int startIndex, Func<T, GameObject> getGo) {
            int nearestIdx = startIndex;
            float minDistanceSq = float.MaxValue;
            Vector3 tarPos = Util.IsNullOrDestroyed(target) ? Vector3.zero : target.transform.position;
            for (int i=0; i<points.Count; ++i) {
                var curGo = getGo(points[i]);
                if (Util.IsNullOrDestroyed(curGo)) continue;
                float distSq = (curGo.transform.position - tarPos).sqrMagnitude;
                if (distSq < minDistanceSq) { minDistanceSq = distSq; nearestIdx = i; }
            }
            return nearestIdx;
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