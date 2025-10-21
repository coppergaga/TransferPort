using RsLib.Components;
using UnityEngine;

namespace RsTransferPort.Assets
{
    [CreateAssetMenu(fileName = "BodyAsset", menuName = "TransferPor/BodyAssett",order = 1)]
    public class BodyAsset : ScriptableObject
    {
        [Header("Sprites")]
        public Sprite unconnectedChannelIcon;
        public Sprite globalConnectivityIcon;
        public Sprite planetaryIsolationIcon;
        public Sprite showOverlayButton;
        public Sprite portOverlayButton;

        [Header("Prefabs")]
        public PortChannelSideScreen portChannelSideScreen;
        public LineArrow lineArrow;
        public LineCenterImage lineCenterImage;
        public RsHierarchyReferences portChannelName;

    }
}