using System;
using System.Reflection;
using HarmonyLib;

namespace RsLib
{
    public static class RsHarmonyExtension
    {
        public static void Patch(
            this Harmony instance,
            Type type,
            string methodName,
            HarmonyMethod prefix = null,
            HarmonyMethod postfix = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));
            try
            {
                var method = type.GetMethod(methodName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                    instance.Patch(method, prefix, postfix);
                else
                    Debug.LogWarningFormat("Unable to find method {0} on type {1}", methodName, type.FullName);
            }
            catch (AmbiguousMatchException ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}