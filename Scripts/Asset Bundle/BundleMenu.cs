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
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            // Bundle room texture together
            buildMap[0] = new AssetBundleBuild();
            buildMap[0].assetBundleName = Config.AssetBundle.RawPackage.Name;

            // Gather number of assets to place into asset bundle
            int numTextures = MaterialManager.GetNumTextures();
            int numMeshFiles = 1;
            int numSupplementaryMeshInfoFiles = 1;
            int numMatrixFiles = 1;
            int numAssets = numTextures + numMeshFiles + numSupplementaryMeshInfoFiles + numMatrixFiles;
            string[] textureAssets = new string[numAssets];

            // Assign assets to asset bundle
            int index = 0;
            // Textures
            for (; index < numTextures; index++)
            {
                textureAssets[index] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), Config.Images.CompileFilename(index)); //Config.AssetBundle.InputFolder
            }
            // Mesh
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), Config.CustomMesh.CompileFilename()); //Config.AssetBundle.InputFolder
            // Mesh Supplementary Info
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), Config.CustomOrientation.CompileFilename()); //Config.AssetBundle.InputFolder
            // Matrix Arrays
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), Config.MatrixArray.CompileFilename()); //Config.AssetBundle.InputFolder
            buildMap[0].assetNames = textureAssets;

            // Write asset bundle
            try
            {
                BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            }
            catch (System.ArgumentException)
            {
                Directory.CreateDirectory(Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
                Debug.Log("Asset Bundle folder created: " + Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
                BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            }

            BundleHandler.CleanAssetBundleGeneration();

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Unity Editor hook for unpacking room texture bundle and pushing through
        /// logic of generating material/meshes appropriately.
        /// </summary>
        [UnityEditor.MenuItem("Room Texture/Intermediary Processing/Import Asset Bundle", false, 0)]
        public static void UnbundleRoomTexture()
        {
            BundleHandler.UnpackRoomTextureBundle();
        }

        [UnityEditor.MenuItem("Room Texture/Instantiate Room")]
        public static void InstantiateRoom()
        {
            BundleHandler.InstantiateRoom();
            //string[] orientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename()));
            //GameObject room = RoomModel.BuildRoomObject(orientationFileLines);
        }

        
        [UnityEditor.MenuItem("Room Texture/Remove Assets/All")]
        public static void ClearAllRoomAssets()
        {
            BundleHandler.RemoveRoomObject();
            BundleHandler.RemoveRoomResources();
            PrefabHandler.DeletePrefabs();
            BundleHandler.RemoveRawInfo();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/All Finished Room Resources")]
        public static void ClearAllFinishedRoomAssets()
        {
            BundleHandler.RemoveRoomObject();
            BundleHandler.RemoveRoomResources();
            PrefabHandler.DeletePrefabs();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Room Object")]
        public static void ClearRoomObject()
        {
            BundleHandler.RemoveRoomObject();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Stored Assets")]
        public static void ClearRoomResources()
        {
            BundleHandler.RemoveRoomResources();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Prefabs")]
        public static void ClearRoomPrefabs()
        {
            PrefabHandler.DeletePrefabs();
        }

        [UnityEditor.MenuItem("Room Texture/Remove Assets/Raw Info")]
        public static void ClearRawRoomInfo()
        {
            BundleHandler.RemoveRawInfo();
        }

        // ERROR TESTING - UPDATE W/ CORRECT LOGIC
        [UnityEditor.MenuItem("Room Texture/Build Finished Room Bundle")]
        public static void BundleRoom_WindowsStandalone()
        {
            GameObject room = GameObject.Find(Config.RoomObject.GameObjectName);
            if (room != null)
            {

                AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

                // Bundle room texture items together
                buildMap[0] = new AssetBundleBuild();
                buildMap[0].assetBundleName = Config.AssetBundle.RoomPackage.Name;

                // Gather number of assets to place into asset bundle
                int numMaterials = MaterialManager.GetNumMaterials();
                int numMeshFiles = CustomMesh.GetNumMeshes();
                int numTexArrays = 1;
                int numRoomPrefabs = 1;
                int numShaders = 1;
                int numMatrixFiles = 1;
                int numAssets = numMaterials + numMeshFiles + numTexArrays + numRoomPrefabs + numShaders + numMatrixFiles;
                string[] roomAssets = new string[numAssets];

                // Assign assets to asset bundle
                int index = 0;
                // Materials
                for (int i = 0; i < numMaterials; i++)
                {
                    string materialName = Config.Material.CompileMaterialName(i);
                    roomAssets[index++] = Config.Material.CompileUnityAssetPath(materialName);
                }
                // Meshes
                for(int i = 0; i < numMeshFiles; i++)
                {
                    string meshName = Config.UnityMeshes.CompileMeshName(i);
                    // ERROR TESTING - reference Mesh, not CustomMesh
                    roomAssets[index++] = Config.CustomMesh.CompileUnityAssetPath(meshName);
                }
                // Texture2DArray
                roomAssets[index++] = Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename());
                // Room Prefab
                roomAssets[index++] = Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename());
                // Shaders
                roomAssets[index++] = Config.Shader.CompileUnityAssetPath(Config.Shader.CompileFilename());
                // Matrix Files
                //roomAssets[index++] = CrossPlatformNames.Matrices.CompileAssetPath(); // ERROR TESTING - Currently sitting outside Resources folder. Fix this.
                Debug.Log("Matrix filepath = " + Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename());
                roomAssets[index++] = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename();
                buildMap[0].assetNames = roomAssets;

                // Write asset bundle
                try
                {
                    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RoomPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
                }
                catch (System.ArgumentException)
                {
                    Directory.CreateDirectory(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory());
                    Debug.Log("Asset Bundle folder created: " + Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory());
                    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RoomPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
                }

                BundleHandler.CleanAssetBundleGeneration();

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Asset bundle of processed room failed. Does the room object exist in the scene?");
            }
        }

        [UnityEditor.MenuItem("Room Texture/Unpack Finished Room Bundle")]
        public static void UnbundleRoom()
        {
            BundleHandler.UnpackFinalRoomTextureBundle();
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