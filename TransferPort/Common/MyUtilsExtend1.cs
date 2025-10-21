using UnityEngine;

namespace RsTransferPort
{
    public static partial class MyUtils
    {
        public static void SetPositionXY(this Transform transform, Vector3 xy)
        {
            var position = transform.position;
            position.x = xy.x;
            position.y = xy.y;
            transform.position = position;
        }
    }
}