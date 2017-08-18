using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public class Config_Base
    {
        #region Asset Path Event Handling
        public static event AssetPathChangedEventHandler AssetPathChanged;
        
        protected static void OnAssetPathChanged(AssetPathChangedEventArgs e)
        {
            if (AssetPathChanged != null)
                AssetPathChanged(e);
        }

        public static event AssetSubFolderChangedEventHandler AssetSubFolderChanged;

        protected static void OnAssetSubFolderChanged(AssetSubFolderChangedEventArgs e)
        {
            if (AssetSubFolderChanged != null)
                AssetSubFolderChanged(e);
        }
        #endregion

        #region Fields/Properties
#if UNITY_WSA_10_0
        private static string absoluteAssetRootFolder = Application.persistentDataPath;
#else
        //public static string absoluteAssetRootFolder = Application.dataPath;
        private static string absoluteAssetRootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
#endif
        public static string AbsoluteAssetRootFolder
        {
            get
            {
                return absoluteAssetRootFolder;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string oldAbsoluteAssetRootFolder = absoluteAssetRootFolder;
                    string newAbsoluteAssetRootFolder = value;
                    OnAssetPathChanged(new AssetPathChangedEventArgs(oldAbsoluteAssetRootFolder, newAbsoluteAssetRootFolder));
                }
            }
        }

        //public static string AssetSubFolder = "ASL/Room_Texture/Resources/Room";
        //public static string assetSubFolder = (Directory.Exists(Path.Combine(AbsoluteAssetRootFolder, "ASL"))) // Check if this is freestanding or exists inside of the ASL library
        //    ? "ASL/Room_Texture/Resources" 
        //    : "Room_Texture/Resources";
        private static string assetSubFolder = "Room_Texture/Resources";
        public static string AssetSubFolder
        {
            get
            {
                return assetSubFolder;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string oldAssetSubFolder = assetSubFolder;
                    string newAssetSubFolder = value;
                    OnAssetSubFolderChanged(new AssetSubFolderChangedEventArgs(oldAssetSubFolder, newAssetSubFolder));
                }
            }
        }

#endregion

        #region Methods

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
            int loadPathStartIndex = assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1;
            if (loadPathStartIndex < assetSubDirectory.Length)
            {
                //return assetSubDirectory.Substring(assetSubDirectory.IndexOf("Resources") + "Resources".Length + 1) + '/' + assetNameWithoutExtension;
                return assetSubDirectory.Substring(loadPathStartIndex) + '/' + assetNameWithoutExtension;
            }
            else
            {
                return assetNameWithoutExtension;
            }
        }
#endregion
    }
}
