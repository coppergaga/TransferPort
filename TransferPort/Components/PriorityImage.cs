using RsLib;
using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort {
    public class PriorityImage : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private int m_priority = 0;

        private void Awake() {
            if (image == null) {
                image = GetComponent<Image>();
            }
        }

        private void Start() {
            UpdateSprite();
        }

        public int Priority {
            get => m_priority;
            set {
                if (m_priority != value) {
                    m_priority = value;
                    UpdateSprite();
                }
            }
        }

        private void UpdateSprite() {
            int clamp = Mathf.Clamp(m_priority, 1, 9);
            image.sprite = RsLib.RsUITuning.Images.GetSpriteByName("priority_" + clamp);
            image.SetNativeSize();
        }

        public static void AddLoadPrefabTask() {
            RsResources.AddLoadPrefabTask("prefabs/priority_image", (parent) => {
                GameObject root = RsUIBuilder.UIGameObject("Priority", parent);
                root.rectTransform().localScale = new Vector3(0.4f, 0.4f, 0.4f);
                root.AddComponent<Image>();
                root.AddComponent<PriorityImage>();
                CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return root;
            });
        }
    }
}