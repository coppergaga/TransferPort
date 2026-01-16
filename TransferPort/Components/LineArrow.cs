using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort {
    public class LineArrow : MonoBehaviour {
        public RawImage graphic;
        private Vector3 end;

        private Vector3 start;

        private bool enableAnim = true;
        private bool needUpdateUV = true;


        public bool EnableAnim {
            get => enableAnim;
            set => enableAnim = value;
        }

        public void Start() {
            ((RectTransform)transform).pivot = new Vector2(0, 0.5f);
            // if (graphic == null) graphic = GetComponent<Graphic>();
        }

        public void SetTwoPoint(Vector3 start, Vector3 end) {
            if (this.start == start && this.end == end) return;
            this.start = start;
            this.end = end;
            if (start == end) return;
            UpdateChange();
        }

        public void SetColor(Color color) {
            if (graphic != null && graphic.color != color) graphic.color = color;
        }

        private void UpdateChange() {
            var rectTransform = (RectTransform)transform;
            var parent = rectTransform.parent;
            rectTransform.position = start;
            rectTransform.right = end - rectTransform.position;
            var distance = Vector2.Distance(parent.InverseTransformVector(start), parent.InverseTransformVector(end));
            rectTransform.sizeDelta = new Vector2(distance, 0.2f);
            needUpdateUV = true;
        }

        private void UpdateUV() {
            if (graphic != null && graphic.texture != null) {
                var uvRect = graphic.uvRect;
                var sizeDelta = ((RectTransform)transform).sizeDelta;
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
            if (enableAnim) {
                UpdateUV();
            }
            else if (needUpdateUV) {
                UpdateUV();
            }
        }
    }
}