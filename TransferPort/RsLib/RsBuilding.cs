using HarmonyLib;
using System.Collections.Generic;
using TUNING;

namespace RsLib {
    public class RsBuilding : RsModule<RsBuilding> {
        private static void Db_Initialize_Postfix() {
            foreach (BuildingInfo buildingInfo in Instance.planScreenInfos) {
                if (!buildingInfo.IsInfoValid) {
                    return;
                }
                if (buildingInfo.IsAddTech) {
                    AddBuildingToTech(buildingInfo.techID, buildingInfo.buildingID);
                }
                if (buildingInfo.IsAddPlan) {
                    AddPlanScreen(buildingInfo.category, buildingInfo.subcategoryID, buildingInfo.buildingID);
                }
            }
        }

        /// <summary>
        ///     添加建筑到研究中
        /// </summary>
        public static void AddBuildingToTech(string techID, string buildingID) {
            var tech = Db.Get().Techs.Get(techID);
            if (tech != null)
                tech.unlockedItemIDs.Add(buildingID);
            else
                Debug.LogWarning("AddBuildingToTech() Failed to find tech ID: " + techID);
        }

        public static void AddPlanScreen(HashedString category, string subcategoryID, string buildingID) {
            if (subcategoryID != null && BUILDINGS.PLANSUBCATEGORYSORTING != null) {
                if (!BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(buildingID)) {
                    BUILDINGS.PLANSUBCATEGORYSORTING[buildingID] = subcategoryID;
                }
            }
            ModUtil.AddBuildingToPlanScreen(category, buildingID, subcategoryID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID, string subcategoryID = null) {
            AddPlanScreen(category, subcategoryID, buildingID);
            AddBuildingToTech(techID, buildingID);
        }

        protected override void Initialized() {
            Harmony.Patch(typeof(Db), "Initialize",
                postfix: new HarmonyMethod(typeof(RsBuilding), nameof(Db_Initialize_Postfix)));
        }

        private readonly List<BuildingInfo> planScreenInfos = new List<BuildingInfo>();

        public RsBuilding AddBuilding(BuildingInfo bi) {
            if (bi.onlyDlc1 && !DlcManager.IsExpansion1Active()) {
                return this;
            }
            planScreenInfos.Add(bi);
            return this;
        }

        public RsBuilding AddBuilding(string buildingID, HashedString category, string subcategoryID, string techID, bool onlyDlc1 = false) {
            return AddBuilding(BuildingInfo.BI(buildingID, category, subcategoryID, techID, onlyDlc1));
        }

        public class BuildingInfo {
            public bool IsInfoValid => buildingID != null;
            public bool IsAddPlan => category != null;
            public bool IsAddTech => techID != null;
            public bool onlyDlc1;

            public HashedString category;   // 为null时不添加到建造菜单中
            public string techID;           // 为null时不添加到研究中
            public string buildingID;       // 为null时不添加该建筑
            public string subcategoryID;

            public static BuildingInfo BI(string buildingID, HashedString category, string subcategoryID, string techID, bool onlyDlc1 = false) {
                return new BuildingInfo {
                    buildingID = buildingID,
                    category = category,
                    subcategoryID = subcategoryID,
                    techID = techID,
                    onlyDlc1 = onlyDlc1
                };
            }
        }
    }
}