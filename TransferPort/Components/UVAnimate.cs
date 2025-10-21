using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort
{
    [RequireComponent(typeof(RawImage))]
    public class UVAnimate : MonoBehaviour
    {
        public RawImage image;

        /// <summary>
        ///     uv.x 的 speed;
        /// </summary>
        public float speed = 1;

        private RectTransform rectTransform => (RectTransform) transform;

        
        private void Start()
        {
            if (image == null) image = GetComponent<RawImage>();
        }

        private void Update()
        {
            if (image != null && image.texture != null)
            {
                var uvRect = image.uvRect;
                var sizeDelta = rectTransform.sizeDelta;
                var whb = (float) image.texture.width / image.texture.height; //单元的高宽比
                var iW = whb * sizeDelta.y; //单元大小
                var wn = sizeDelta.x / iW; //单元的数量
                uvRect.width = wn;
                uvRect.x = (uvRect.x - Time.unscaledDeltaTime * speed) % 1;
                image.uvRect = uvRect;
            }
        }
    }
}