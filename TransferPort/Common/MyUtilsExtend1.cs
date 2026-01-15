using System.Collections.Generic;
using UnityEngine;

namespace RsTransferPort {
    public static partial class MyUtils {
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