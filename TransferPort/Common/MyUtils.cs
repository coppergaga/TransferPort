using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TUNING;

namespace RsTransferPort {
    public static partial class MyUtils {
        public static MyIdGenerate ID = new MyIdGenerate();

        public static string UniqueSaveName(string name) {
            return name + "-" + ID.Next();
        }

        public static void AddBuildingToTech(string techID, string buildingID) {
            var tech = Db.Get().Techs.Get(techID);
            if (tech != null)
                tech.unlockedItemIDs.Add(buildingID);
            else
                Debug.LogWarning("AddBuildingToTech() Failed to find tech ID: " + techID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID) {
            ModUtil.AddBuildingToPlanScreen(category, buildingID);
            AddBuildingToTech(techID, buildingID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID,
            string subcategoryID) {
            ModUtil.AddBuildingToPlanScreen(category, buildingID, subcategoryID);
            AddBuildingToTech(techID, buildingID);
        }


        /// <summary>
        ///     获取指定的class下的所有静态属性（深度遍历）
        /// </summary>
        public static Dictionary<string, object> FlatFields(Type type) {
            var dir = new Dictionary<string, object>();
            FlatFields(type, "", dir);
            return dir;
        }

        private static void FlatFields(Type type, string prefix, Dictionary<string, object> container) {
            if (string.IsNullOrEmpty(prefix))
                prefix = type.Name;
            else
                prefix = prefix + "." + type.Name;

            //普通属性
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var t in fieldInfos) { container.Add(prefix + "." + t.Name, t.GetValue(null)); }

            //类
            var nestedTypes = type.GetNestedTypes(BindingFlags.Public);
            foreach (var nestedType in nestedTypes) FlatFields(nestedType, prefix, container);
        }


        /// <summary>
        ///     通过一个类添加到Strings里
        /// </summary>
        public static void AddStrings(Type type) {
            var flatFields = FlatFields(type);
            foreach (var kv in flatFields) { Strings.Add(kv.Key, (LocString)kv.Value); }
        }

        public static int Count(this IEnumerable source) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is ICollection collection) return collection.Count;

            var num = 0;
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                checked {
                    ++num;
                }

            return num;
        }

        public static bool IsUsePriority(BuildingType buildingType) {
            return buildingType == BuildingType.Gas
                || buildingType == BuildingType.Liquid
                || buildingType == BuildingType.Solid;
        }

        public static BuildingDef CreateTransferBuildingDef(
            string id,
            string anim,
            float[] construction_mass,
            string[] construction_materials
        ) {
            var buildingDef = global::BuildingTemplates.CreateBuildingDef(
                id,
                1,
                1,
                anim,
                30,
                15,
                construction_mass,
                construction_materials,
                2400f,
                BuildLocationRule.Anywhere,
                BUILDINGS.DECOR.NONE,
                NOISE_POLLUTION.NONE
            );
            buildingDef.DefaultAnimState = "idle";
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            return buildingDef;
        }
    }


    public class MyIdGenerate {
        private const string ID_T = "0123456789abcdefghijklnmopqrstuvwxyzABCDEFGHIJKLNMOPQRSTUVWXYZ";
        private const int ID_LEN = 62;

        private int nextBit;
        private long lastTime;

        public static long CurrentSecond => System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //(System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        public static string NumberToAscall(long number) {
            var builder = new StringBuilder();
            while (number > 0) {
                var n = (int)(number % ID_LEN);
                number /= ID_LEN;
                builder.Append(ID_T[n]);
            }

            return new string(builder.ToString().Reverse().ToArray());
        }

        public string Next() {
            var currentSecond = CurrentSecond;

            if (lastTime == currentSecond) {
                nextBit++;
            }
            else {
                nextBit = 0;
                lastTime = currentSecond;
            }

            var id = currentSecond * 100 + nextBit;

            return NumberToAscall(id);
        }
    }
}