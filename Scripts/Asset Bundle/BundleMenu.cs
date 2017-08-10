using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Handles Unity Editor hooks for bundling and interpreting asset bundles 
    /// related to the Hololens room mesh texturing.
    /// </summary>
    public static class BundleMenu
    {
#if UNITY_EDITOR
        /// <summary>
        /// Unity Editor hook for packing room texture bundle. Gathers the 
        /// textures, camera matrices, room mesh text file, and supplementary 
        /// room mesh information together into an asset bundle for convenient 
        /// storage and transportation between Unity nodes.
        /// 
        /// NOTE: This logic should not be separated from this method, as 
        /// bundling asset bundles can ONLY OCCUR INSIDE OF THE UNITY EDITOR
        /// /WITH THE UNITY EDITOR NAMESPACE. This means that this logic cannot 
        /// ever operate on any ASL node outside of a client that is running 
        /// the editor.
        /// </summary>
        [UnityEditor.MenuItem("Room Texture/Intermediary Processing/Export Asset Bundle", false, 0)]
        public static void BundleRoomTexture_WindowsStandalone()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string destinationDirectory = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName);
            BuildTarget targetPlatform = BuildTarget.StandaloneWindows;

            BundleHandler.PackRawRoomTextureBundle(destinationDirectory, targetPlatform);
        }

        /// <summary>
        /// Unity Editor hook for unpacking room texture bundle and pushing through
        /// logic of generating material/meshes appropriately.
        /// </summary>
        [UnityEditor.MenuItem("Room Texture/Intermediary Processing/Import Asset Bundle", false, 0)]
        public static void UnbundleRoomTexture()
        {
            string roomName = Config.RoomObject.GameObjectName;
            BundleHandler.UnpackRawResourceTextureBundle(roomName);
        }

        [UnityEditor.MenuItem("Room Texture/Instantiate Room")]
        public static void InstantiateRoom()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            BundleHandler.InstantiateRoom(roomName);
            //string[] orientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename()));
            //GameObject room = RoomModel.BuildRoomObject(orientationFileLines);
        }
        
        [UnityEditor.MenuItem("Room Texture/Remove Assets/All")]
        public static void ClearAllRoomAssets()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
            string meshesDirectory = materialsDirectory;
            string texturesDirectory = materialsDirectory;

            BundleHandler.RemoveRoomObject();
            BundleHandler.RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);
            PrefabHandler.DeletePrefabs(roomName);
            BundleHandler.RemoveRawInfo(roomName);
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/All Finished Room Resources")]
        public static void ClearAllFinishedRoomAssets()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
            string meshesDirectory = materialsDirectory;
            string texturesDirectory = materialsDirectory;

            BundleHandler.RemoveRoomObject();
            BundleHandler.RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);
            PrefabHandler.DeletePrefabs(roomName);
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Room Object")]
        public static void ClearRoomObject()
        {
            BundleHandler.RemoveRoomObject();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Stored Assets")]
        public static void ClearRoomResources()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
            string meshesDirectory = materialsDirectory;
            string texturesDirectory = materialsDirectory;

            BundleHandler.RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Prefabs")]
        public static void ClearRoomPrefabs()
        {
            string roomName = Config.RoomObject.GameObjectName;
            PrefabHandler.DeletePrefabs(roomName);
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Raw Info")]
        public static void ClearRawRoomInfo()
        {
            string roomName = Config.RoomObject.GameObjectName;
            BundleHandler.RemoveRawInfo(roomName);
        }

        // ERROR TESTING - UPDATE W/ CORRECT LOGIC
        [UnityEditor.MenuItem("Room Texture/Build Finished Room Bundle")]
        public static void BundleRoom_WindowsStandalone()
        {
            string roomName = Config.RoomObject.GameObjectName;
            string destinationDirectory = Config.AssetBundle.RoomPackage.CompileUnityAssetDirectory(roomName);
            BuildTarget targetPlatform = BuildTarget.StandaloneWindows;

            BundleHandler.PackFinalRoomBundle(destinationDirectory, targetPlatform);
        }

        [UnityEditor.MenuItem("Room Texture/Unpack Finished Room Bundle")]
        public static void UnbundleRoom()
        {
            BundleHandler.UnpackFinalRoomTextureBundle();
        }
        
        [UnityEditor.MenuItem("Room Texture/Prefab/Generate Finished Room Prefab", false, 0)]
        public static void CreatePrefab_Room()
        {
            GameObject roomModel = GameObject.Find(Config.RoomObject.GameObjectName);
            if (roomModel != null)
            {
                string roomName = Config.RoomObject.GameObjectName;
                PrefabHandler.CreatePrefab(roomModel, roomName);
            }
            else
            {
                Debug.Log(PrefabHandler.Messages.GameObjectDoesNotExist);
            }
        }

        /// <summary>
        /// Mashes together the menu and submenu names, separated by '/' for 
        /// use with Unity's interface.
        /// </summary>
        /// <param name="menuAndSubMenus">
        /// The menu name and submenu strings, not including any delimiters.
        /// </param>
        /// <returns>
        /// A string that is parsable by Unity's interface to properly name a 
        /// menu item.
        /// </returns>
        public static string CompileMenuName(string[] menuAndSubMenus)
        {
            if(menuAndSubMenus == null
                || menuAndSubMenus.Length < 1)
            {
                return string.Empty;
            }
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(menuAndSubMenus[0]);
            for(int i = 1; i < menuAndSubMenus.Length; i++)
            {
                sb.Append('/');
                sb.Append(menuAndSubMenus[i]);
            }

            return sb.ToString();
        }
#endif
    }
}