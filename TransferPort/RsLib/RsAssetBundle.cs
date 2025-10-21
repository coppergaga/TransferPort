using System.IO;
using UnityEngine;

namespace RsLib.Unity
{
    public class RsAssetBundle
    {
        public static AssetBundle LoadAssetBundle(string basePath, string assetBundleName, string path = null, bool platformSpecific = false)
        {
            foreach (AssetBundle assetBundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (assetBundle.name == assetBundleName)
                {
                    return assetBundle;
                }
            }
            if (path.IsNullOrWhiteSpace())
            {
                path = Path.Combine(basePath, "assets");
            }
            if (platformSpecific)
            {
                RuntimePlatform platform = Application.platform;
                if (platform != RuntimePlatform.OSXPlayer)
                {
                    if (platform != RuntimePlatform.WindowsPlayer)
                    {
                        if (platform == RuntimePlatform.LinuxPlayer)
                        {
                            path = Path.Combine(path, "linux");
                        }
                    }
                    else
                    {
                        path = Path.Combine(path, "windows");
                    }
                }
                else
                {
                    path = Path.Combine(path, "mac");
                }
            }
            path = Path.Combine(path, assetBundleName);
            AssetBundle assetBundle2 = AssetBundle.LoadFromFile(path);
            if (assetBundle2 == null)
            {
                Debug.LogWarning("Failed to load AssetBundle from path " + path);
                return null;
            }
            return assetBundle2;
        }
    }
}