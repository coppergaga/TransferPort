using System.Collections.Generic;
using RsLib;
using UnityEngine;
using UnityEngine.UI;

namespace RsTransferPort {
    /// <summary>
    /// <see cref="LogicBroadcastChannelSideScreen"/>
    /// </summary>
    public class WorldDiscoveredSideScreen : RsSideScreenContent {
        // private LogicClusterLocationSensor sensor;

        [RsSideScreen.CopyField, SerializeField] private GameObject rowPrefab;
        [RsSideScreen.CopyField, SerializeField] private GameObject listContainer;
        [RsSideScreen.CopyField, SerializeField] private LocText headerLabel;

        private Dictionary<AxialI, GameObject> worldRows = new Dictionary<AxialI, GameObject>();

        public override bool IsValidForTarget(GameObject target) =>
            (UnityEngine.Object)target.GetComponent<TransferPortCenter>() != (UnityEngine.Object)null;

        public override void SetTarget(GameObject target) {
            base.SetTarget(target);
            this.Build();
        }

        private void ClearRows() {
            foreach (KeyValuePair<AxialI, GameObject> worldRow in this.worldRows)
                Util.KDestroyGameObject(worldRow.Value);
            this.worldRows.Clear();
        }

        private void Build() {
            this.headerLabel.SetText(STRINGS.UI.SIDESCREEN.WORLDDISCOVEREDSIDESCREEN.HEADE);
            this.ClearRows();
            foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>)ClusterManager.Instance
                .WorldContainers) {

                if (!worldContainer.IsModuleInterior && !worldContainer.IsStartWorld) {
                    GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer);
                    gameObject.gameObject.name = worldContainer.GetProperName();
                    AxialI myWorldLocation = worldContainer.GetMyWorldLocation();
                    Debug.Assert(!this.worldRows.ContainsKey(myWorldLocation),
                        (object)(
                            "Adding two worlds/POI with the same cluster location to ClusterLocationFilterSideScreen UI: " +
                            worldContainer.GetProperName()));
                    this.worldRows.Add(myWorldLocation, gameObject);
                }
            }

            this.Refresh();
        }

        private void Refresh() {
            foreach (KeyValuePair<AxialI, GameObject> worldRow in this.worldRows) {
                KeyValuePair<AxialI, GameObject> kvp = worldRow;
                ClusterGridEntity cmp = ClusterGrid.Instance.cellContents[kvp.Key][0];
                WorldContainer worldContainer = cmp.GetComponent<WorldContainer>();
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")
                    .SetText(cmp.GetProperName());
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite =
                    Def.GetUISprite((object)cmp).first;
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").color =
                    Def.GetUISprite((object)cmp).second;
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick =
                    (System.Action)(() => {
                        ToggleWorldDiscovered(worldContainer);
                        this.Refresh();
                    });
                kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle")
                    .ChangeState(worldContainer.IsDiscovered ? 1 : 0);
                kvp.Value.SetActive(ClusterGrid.Instance.GetCellRevealLevel(kvp.Key) == ClusterRevealLevel.Visible);
            }
        }

        private void ToggleWorldDiscovered(WorldContainer worldContainer) {
            if (!worldContainer.IsDiscovered) {
                worldContainer.SetDiscovered();
            }
            else {
                RsField.SetValue(worldContainer, "isDiscovered", false);
                // GameHashes.DiscoveredWorldsChanged = -521212405;
                Game.Instance.Trigger(-521212405, worldContainer);
            }
        }

        public override string GetTitle() => STRINGS.UI.SIDESCREEN.WORLDDISCOVEREDSIDESCREEN.TITLE;
    }
}