using RsLib;
using RsLib.Builder;
using RsTransferPort;
using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort {
    public class MyResource {
        public static void InitAllTask() {
            PortChannelDiagram.InitPrefab();

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