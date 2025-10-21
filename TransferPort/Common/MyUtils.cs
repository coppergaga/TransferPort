using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TUNING;
using UnityEngine;

namespace RsTransferPort
{
    public static partial class MyUtils
    {
        public static MyIdGenerate ID = new MyIdGenerate();

        public static long CurrentSecond =>
            (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        
        public static string UniqueSaveName(string name)
        {
            return name + "-" + ID.Next();
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

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID)
        {
            ModUtil.AddBuildingToPlanScreen(category, buildingID);
            AddBuildingToTech(techID, buildingID);
        }

        public static void AddPlanScreenAndTech(HashedString category, string techID, string buildingID,
            string subcategoryID)
        {
            ModUtil.AddBuildingToPlanScreen(category, buildingID, subcategoryID);
            AddBuildingToTech(techID, buildingID);
        }


        /// <summary>
        ///     获取指定的class下的所有静态属性（深度遍历）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> FlatFields(Type type)
        {
            var dir = new Dictionary<string, object>();
            FlatFields(type, "", dir);
            return dir;
        }

        private static void FlatFields(Type type, string prefix, Dictionary<string, object> container)
        {
            if (string.IsNullOrEmpty(prefix))
                prefix = type.Name;
            else
                prefix = prefix + "." + type.Name;

            //普通属性
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var t in fieldInfos) container.Add(prefix + "." + t.Name, t.GetValue(null));

            //类
            var nestedTypes = type.GetNestedTypes(BindingFlags.Public);
            foreach (var nestedType in nestedTypes) FlatFields(nestedType, prefix, container);
        }


        /// <summary>
        ///     通过一个类添加到Strings里
        /// </summary>
        /// <param name="type"></param>
        public static void AddStrings(Type type)
        {
            var flatFields = FlatFields(type);
            foreach (var keyValuePair in flatFields) Strings.Add(keyValuePair.Key, (LocString) keyValuePair.Value);
        }

        public static int Count(this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (source is ICollection) return ((ICollection) source).Count;

            var num = 0;
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                checked
                {
                    ++num;
                }

            return num;
        }

        public static Vector3 Center(this ICollection<GameObject> points)
        {
            var total = points.Count;
            var center = Vector3.zero;

            foreach (var point in points) center += point.transform.position;
            center /= total;
            return center;
        }

        public static Vector2 Center(this ICollection<Vector2> points)
        {
            var total = points.Count;
            var center = Vector2.zero;

            foreach (var point in points) center += point;
            center /= total;
            return center;
        }
        
        public static bool IsUsePriority(BuildingType buildingType)
        {
            return buildingType == BuildingType.Gas || buildingType == BuildingType.Liquid ||
                   buildingType == BuildingType.Solid;
        }

        public static BuildingDef CreateTransferBuildingDef(
            string id,
            string anim,
            float[] construction_mass,
            string[] construction_materials
        )
        {
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

        public class BuildingTemplates
        {
            /// <summary>
            ///     动画
            /// </summary>
            public string anim;

            /// <summary>
            ///     建筑放置规则
            /// </summary>
            public BuildLocationRule build_location_rule;

            /// <summary>
            ///     建造数量
            /// </summary>
            public float[] construction_mass;

            /// <summary>
            ///     建造材料
            /// </summary>
            public string[] construction_materials;

            /// <summary>
            ///     建造时间
            /// </summary>
            public float construction_time;

            /// <summary>
            ///     装饰度
            /// </summary>
            public EffectorValues decor;

            /// <summary>
            ///     建筑高度
            /// </summary>
            public int height;

            /// <summary>
            ///     建筑生命值(不确定)
            /// </summary>
            public int hitpoints;


            /// <summary>
            ///     唯一标识
            /// </summary>
            public string id;

            /// <summary>
            ///     熔点
            /// </summary>
            public float melting_point;

            /// <summary>
            ///     噪音污染,目前没用
            /// </summary>
            public EffectorValues noise;

            /// <summary>
            ///     热导率
            /// </summary>
            public float temperature_modification_mass_scale = 0.2f;

            /// <summary>
            ///     建筑宽度
            /// </summary>
            public int width;

            public static BuildingTemplates OneAnyWhere()
            {
                var templates = new BuildingTemplates();

                templates.width = 1;
                templates.height = 1;
                templates.hitpoints = 30;
                templates.construction_time = 30;
                templates.melting_point = 2400f;
                templates.construction_mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER6;
                templates.construction_materials = MATERIALS.ALL_MINERALS;
                templates.build_location_rule = BuildLocationRule.Anywhere;
                templates.decor = BUILDINGS.DECOR.NONE;
                templates.noise = NOISE_POLLUTION.NONE;
                return templates;
            }

            public BuildingDef ToBuildingDef()
            {
                return global::BuildingTemplates.CreateBuildingDef(
                    id,
                    width,
                    height,
                    anim,
                    hitpoints,
                    construction_time,
                    construction_mass,
                    construction_materials,
                    melting_point,
                    build_location_rule,
                    decor,
                    noise,
                    temperature_modification_mass_scale
                );
            }
        }
    }


    public class MyIdGenerate
    {
        private const string ID_T = "0123456789abcdefghijklnmopqrstuvwxyzABCDEFGHIJKLNMOPQRSTUVWXYZ";
        private const int ID_LEN = 62;

        private int _nextBit;
        private long lastTime;


        public static string NumberToAscall(long number)
        {
            var builder = new StringBuilder();
            while (number > 0)
            {
                var n = (int) (number % ID_LEN);
                number = number / ID_LEN;
                builder.Append(ID_T[n]);
            }

            return new string(builder.ToString().Reverse().ToArray());
        }


        public string Next()
        {
            var currentSecond = MyUtils.CurrentSecond;

            if (lastTime == currentSecond)
            {
                _nextBit++;
            }
            else
            {
                _nextBit = 0;
                lastTime = currentSecond;
            }

            var id = currentSecond * 100 + _nextBit;

            return NumberToAscall(id);
        }
    }
}