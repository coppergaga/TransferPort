using RsLib;
using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort {
    public class LineArrow : MonoBehaviour {
        public RawImage graphic;
        private Vector3 end;
        private Vector3 start;

        private bool enableAnim = true;
        private bool needUpdateUV = true;
        private RectTransform rectTs;

        public bool EnableAnim {
            get => enableAnim;
            set => enableAnim = value;
        }

        public void OnEnable() {
            rectTs = (RectTransform)transform;
            rectTs.pivot = RsUtil.ArrowV2Offset;
            // if (graphic == null) graphic = GetComponent<Graphic>();
        }

        public void SetTwoPoint(Vector3 start, Vector3 end) {
            this.start = start;
            this.end = end;
            UpdateChange();
        }

        public void SetColor(Color color) {
            if (graphic != null && graphic.color != color) graphic.color = color;
        }

        private void UpdateChange() {
            var parent = rectTs.parent;
            rectTs.position = start;
            rectTs.right = end - rectTs.position;
            var distance = Vector2.Distance(parent.InverseTransformVector(start), parent.InverseTransformVector(end));
            rectTs.sizeDelta = new Vector2(distance, 0.2f);
            needUpdateUV = true;
        }

        private void UpdateUV() {
            if (graphic != null && graphic.texture != null) {
                var uvRect = graphic.uvRect;
                var sizeDelta = rectTs.sizeDelta;
                var whb = (float)graphic.texture.width / graphic.texture.height; //单元的高宽比
                var iW = whb * sizeDelta.y; //单元大小
                var wn = sizeDelta.x / iW; //单元的数量
                uvRect.width = wn;
                if (enableAnim) {
                    uvRect.x = (uvRect.x - Time.unscaledDeltaTime * 2) % 1;
                }
                else {
                    uvRect.x = 0;
                }
                graphic.uvRect = uvRect;
            }
            needUpdateUV = false;
        }

        private void LateUpdate() {
            if (enableAnim || needUpdateUV) {
                UpdateUV();
            }
        }
    }
}