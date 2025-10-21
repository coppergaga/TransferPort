using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using KMod;
using Newtonsoft.Json.Linq;

namespace RsLib
{
    public class RsLocalization : RsModule<RsLocalization>
    {
        protected HashSet<Type> addStringsTypes = new HashSet<Type>();


        protected HashSet<Type> loadTypes = new HashSet<Type>();

        protected override void Initialized()
        {
            Harmony.Patch(typeof(Localization), "Initialize",
                postfix: new HarmonyMethod(typeof(RsLocalization), nameof(Localization_Initialize_Patch)));
        }

        private static void Localization_Initialize_Patch()
        {
            foreach (var type in Instance.loadTypes) Load(type, Instance.Mod);
            foreach (var type in Instance.addStringsTypes) CreateLocStringKeys(type);
        }

        /// <summary>
        ///     注册从mod位置的translations位置读取翻译内容
        /// </summary>
        /// <param name="type"></param>
        public RsLocalization RegisterLoad(Type type)
        {
            loadTypes.Add(type);
            return this;
        }

        public RsLocalization RegisterAddStrings(Type type)
        {
            addStringsTypes.Add(type);
            return this;
        }

        public static void Load(Type type, Mod mod)
        {
            var localeCode = Localization.GetLocale()?.Code;
            if (localeCode.IsNullOrWhiteSpace()) return;
            //json文件
            var path = Path.Combine(mod.ContentPath, "translations", localeCode + ".json");
            if (File.Exists(path)) {
                var parameter_errors = "";
                var link_errors = "";
                var link_count_errors = "";
                Localization.OverloadStrings(ReadLocaleJson(path), type.Name, type, ref parameter_errors, ref link_errors, ref link_count_errors);
                if (!string.IsNullOrEmpty(parameter_errors))
                    DebugUtil.LogArgs("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + parameter_errors);
                if (!string.IsNullOrEmpty(link_errors))
                    DebugUtil.LogArgs("TRANSLATION ERROR! The following have mismatched <link> tags:\n" + link_errors);
                if (string.IsNullOrEmpty(link_count_errors))
                    return;
                DebugUtil.LogArgs(
                    "TRANSLATION ERROR! The following do not have the same amount of <link> tags as the english string which can cause nested link errors:\n" +
                    link_count_errors);
            }
        }

        public static Dictionary<string, string> ReadLocaleJson(string path) {
            string jsonStr = File.ReadAllText(path);
            var jsonObj = JObject.Parse(jsonStr);
            Dictionary<string, string> flattenedDict = new Dictionary<string, string>();
            FlatJsonObj(jsonObj, "", flattenedDict);
            return flattenedDict;
        }

        public static void FlatJsonObj(JObject jobj, string parentKey, Dictionary<string, string> ret) {
            foreach (var prop in jobj.Properties()) {
                string newKey = string.IsNullOrEmpty(parentKey) ? prop.Name : $"{parentKey}.{prop.Name}";
                if (prop.Value.Type == JTokenType.Object) { FlatJsonObj((JObject)prop.Value, newKey, ret); }
                if (prop.Value.Type == JTokenType.String) { ret[newKey] = prop.Value.ToString(); }
            }
        }

        public static void CreateLocStringKeys(Type type)
        {
            LocString.CreateLocStringKeys(type);
        }
    }
}