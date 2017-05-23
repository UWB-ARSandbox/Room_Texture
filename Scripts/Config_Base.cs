using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public class Config_Base
    {

#if UNITY_WSA_10_0
        public static string AbsoluteAssetRootFolder = Application.persistentDataPath;
#else
        public static string AbsoluteAssetRootFolder = Application.dataPath;
#endif

        public static string AssetSubFolder = "ASL/Room_Texture/Resources/Room";

        public static string CompileUnityAssetDirectory()
        {
            return "Assets/" + AssetSubFolder;
        }
        public static string CompileUnityAssetPath(string filename)
        {
            return CompileUnityAssetDirectory() + '/' + filename;
        }
        public static string CompileAbsoluteAssetDirectory()
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
#endif
        }
        public static string CompileAbsoluteAssetPath(string filename)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
        }
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
    }
}
