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

        //public static string AssetSubFolder = "ASL/Room_Texture/Resources/Room";
        public static string AssetSubFolder = (Directory.Exists(Path.Combine(AbsoluteAssetRootFolder, "ASL"))) // Check if this is freestanding or exists inside of the ASL library
            ? "ASL/Room_Texture/Resources" 
            : "Room_Texture/Resources";

        public static string CompileUnityAssetDirectory(string roomName)
        {
            return "Assets/" + AssetSubFolder + '/' + roomName;
        }
        public static string CompileUnityAssetPath(string filename, string roomName)
        {
            return CompileUnityAssetDirectory(roomName) + '/' + filename;
        }
        public static string CompileAbsoluteAssetDirectory(string roomName)
        {
#if UNITY_WSA_10_0
            return AbsoluteAssetRootFolder;
#else
            //return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
            return Path.Combine(AbsoluteAssetRootFolder, Path.Combine(AssetSubFolder, roomName));
#endif
        }
        public static string CompileAbsoluteAssetPath(string filename, string roomName)
        {
            return Path.Combine(CompileAbsoluteAssetDirectory(roomName), filename);
        }
        public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
        {
            int loadPathStartIndex = AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1;
            if (loadPathStartIndex < AssetSubFolder.Length)
            {
                //return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
                return AssetSubFolder.Substring(loadPathStartIndex) + '/' + assetNameWithoutExtension;
            }
            else
            {
                return assetNameWithoutExtension;
            }
            //return ResourcesSubFolder + '/' + assetNameWithoutExtension;
        }
        public static string CompileResourcesLoadPath(string assetSubDirectory, string assetNameWithoutExtension)
        {
            int loadPathStartIndex = AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1;
            if (loadPathStartIndex < AssetSubFolder.Length)
            {
                //return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
                return assetSubDirectory.Substring(loadPathStartIndex) + '/' + assetNameWithoutExtension;
            }
            else
            {
                return assetNameWithoutExtension;
            }
        }
    }
}
