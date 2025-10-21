using System.Collections.Generic;
using HarmonyLib;
using TUNING;

namespace RsLib
{
    public class RsBuilding : RsModule<RsBuilding>
    {

        private static void Db_Initialize_Postfix()
        {
            foreach (BuildingInfo buildingInfo in Instance.planScreenInfos)
            {
                if (buildingInfo.buildingID == null)
                {
                    return;
                }

                if (buildingInfo.techID != null)
                {
                    AddBuildingToTech(buildingInfo.techID, buildingInfo.buildingID);
                }
                
                if (buildingInfo.category != null)
                {
                    AddPlanScreen(buildingInfo.category, buildingInfo.subcategoryID, buildingInfo.buildingID);
                }
                
            }
        }
        
        /// <summary>
        ///     添加建筑到研究中
        /// </summary>
        /// <param name="techID"></param>
        /// <param name="buildingID"></param>
        public static void AddBuildingToTech(string techID, string buildingID)
        {
            var tech = Db.Get().Techs.Get(techID);
            var flag = tech != null;
            if (flag)
                tech.unlockedItemIDs.Add(buildingID);
            else
                Debug.LogWarning("AddBuildingToTech() Failed to find tech ID: " + techID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID,
            string subcategoryID = null)
        {
            AddPlanScreen(category, subcategoryID, buildingID);
            AddBuildingToTech(techID, buildingID);
        }
        public static void AddPlanScreen(HashedString category, string subcategoryID, string buildingID)
        {
            if (subcategoryID!= null && BUILDINGS.PLANSUBCATEGORYSORTING != null)
            {
                if (!BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(buildingID))
                {
                    BUILDINGS.PLANSUBCATEGORYSORTING[buildingID] = subcategoryID;
                }
            }
            ModUtil.AddBuildingToPlanScreen(category, buildingID, subcategoryID);
        }

        protected override void Initialized()
        {
            Harmony.Patch(typeof(Db), "Initialize",
                postfix: new HarmonyMethod(typeof(RsBuilding), nameof(Db_Initialize_Postfix)));
        }


        private List<BuildingInfo> planScreenInfos = new List<BuildingInfo>();

        public RsBuilding RegisterPlanScreen(string buildingID, HashedString category, string subcategoryID )
        {
            RegisterPlanScreenAndTech(buildingID, category, subcategoryID, null);
            return this;
        }

        public RsBuilding RegisterPlanScreenAndTech(string buildingID, HashedString category, string subcategoryID, string techID)
        {
            planScreenInfos.Add(new BuildingInfo(buildingID, category, subcategoryID, techID));
            return this;
        }
        
        public RsBuilding RegisterTech(string buildingID, string techID)
        {
            planScreenInfos.Add(new BuildingInfo(buildingID, null, null, techID));
            return this;
        }

        public Advanced ToAdvanced()
        {
            return new Advanced(this);
        }

        public class BuildingInfo
        {
            public HashedString category;
            public string techID;
            public string buildingID;
            public string subcategoryID;
        
            public BuildingInfo()
            {
            }
        
            public BuildingInfo(string buildingID, HashedString category, string subcategoryID,  string techID)
            {
                this.category = category;
                this.techID = techID;
                this.buildingID = buildingID;
                this.subcategoryID = subcategoryID;
            }
        }

        public class Advanced
        {
            private RsBuilding rsBuilding;

            private bool plan = false;
            private bool tech = false;

            private HashedString category;
            private string subcategoryID;
            private string techID;
            private bool onlyDlc1;

            public Advanced(RsBuilding rsBuilding)
            {
                this.rsBuilding = rsBuilding;
            }

            public Advanced PlanAndTech(HashedString category , string subcategoryID, string techID, bool onlyDlc1 = false)
            {
                this.onlyDlc1 = onlyDlc1;
                this.category = category;
                this.subcategoryID = subcategoryID;
                this.techID = techID;
                plan = true;
                tech = true;
                return this;
            }
            
            public Advanced OnlyPlan(HashedString category, string subcategoryID, bool onlyDlc1 = false)
            {
                this.onlyDlc1 = onlyDlc1;
                this.category = category;
                this.subcategoryID = subcategoryID;
                plan = true;
                tech = false;
                return this;
            }
            
            public Advanced OnlyTech(string techID, bool onlyDlc1 = false)
            {
                this.onlyDlc1 = onlyDlc1;
                this.techID = techID;
                plan = false;
                tech = true;
                return this;
            }
            
            public Advanced AddBuilding(string buildingId)
            {
                if (onlyDlc1 && !DlcManager.IsExpansion1Active())
                {
                    return this;
                }
                
                if (plan && tech)
                {
                    rsBuilding.RegisterPlanScreenAndTech(buildingId, category, subcategoryID, techID);
                } else if (plan)
                {
                    rsBuilding.RegisterPlanScreen(buildingId, category, subcategoryID);
                } else if (tech)
                {
                    rsBuilding.RegisterTech(buildingId, techID);
                }
                return this;
            }

            public RsBuilding ToNormal()
            {
                return rsBuilding;
            }
        }
    }
}