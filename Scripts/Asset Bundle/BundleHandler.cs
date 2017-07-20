﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if WINDOWS_UWP
//#if UNITY_WSA_10_0
using Windows.Storage;
using System.Threading.Tasks;
using System;
#endif

namespace UWB_Texturing
{
    /// <summary>
    /// Handles logic with unbundling asset bundle.
    /// </summary>
    public static class BundleHandler
    {
        #region Methods

        #region Packing Room Texture Bundles
#if UNITY_EDITOR
        public static void PackRawRoomTextureBundle(string destinationDirectory, BuildTarget targetPlatform)
        {
            // Ensure that resources exist in project
            if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()))
                || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename()))
                || !File.Exists(Config.Images.CompileFilename(0)))
            {
                //Debug.Log("Raw room resources do not exist. Ensure that items have been saved from room texturing device (e.g. Hololens)");
                // Grab the raw resources
                if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename())))
                {
                    Debug.Log("Room resources for instantiating room cannot be found!");
                    return;
                }
                //UnpackRoomTextureBundle();
            }

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
            if (!Directory.Exists(destinationDirectory))
            {
                //Directory.CreateDirectory(destinationDirectory);
                AbnormalDirectoryHandler.CreateDirectory(destinationDirectory);
                Debug.Log("Asset Bundle folder created: " + destinationDirectory);
            }
            BuildPipeline.BuildAssetBundles(destinationDirectory, buildMap, BuildAssetBundleOptions.StrictMode, targetPlatform);

            //try
            //{
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}
            //catch (System.ArgumentException)
            //{
            //    Directory.CreateDirectory(Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
            //    Debug.Log("Asset Bundle folder created: " + Config.AssetBundle.RawPackage.CompileAbsoluteAssetDirectory());
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}

            Debug.Log("Raw Room Resources Bundle generated at " + Path.Combine(destinationDirectory, buildMap[0].assetBundleName));

            BundleHandler.CleanAssetBundleGeneration(destinationDirectory);

            AssetDatabase.Refresh();
        }

        public static void PackFinalRoomBundle(string destinationDirectory, BuildTarget targetPlatform)
        {
            GameObject room = GameObject.Find(Config.RoomObject.GameObjectName);
            if(room == null)
            {
                string roomPrefabPath = Config.Prefab.CompileAbsoluteAssetPath(Config.Prefab.CompileFilename());
                if (!File.Exists(roomPrefabPath))
                {
                    InstantiateRoom();
                    room = GameObject.Find(Config.RoomObject.GameObjectName);
                    if (room != null)
                    {
                        PrefabHandler.CreatePrefab(GameObject.Find(Config.RoomObject.GameObjectName));
                        RemoveRoomObject();
                    }
                    else
                    {
                        Debug.Log("Couldn't pack room prefab.");
                        return;
                    }
                }
            }

            //if (room != null)
            //{

                AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

                // Bundle room texture items together
                buildMap[0] = new AssetBundleBuild();
                buildMap[0].assetBundleName = Config.AssetBundle.RoomPackage.Name;

                // Gather number of assets to place into asset bundle
                int numMaterials = MaterialManager.GetNumMaterials();
                string meshDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory();
                int numMeshFiles = CustomMesh.GetNumMeshes(meshDirectory);
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
                for (int i = 0; i < numMeshFiles; i++)
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
                //Debug.Log("Matrix filepath = " + Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename());
                roomAssets[index++] = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename();
                buildMap[0].assetNames = roomAssets;

                // Write asset bundle
                
                if (!Directory.Exists(destinationDirectory))
                {
                //Directory.CreateDirectory(destinationDirectory);
                AbnormalDirectoryHandler.CreateDirectory(destinationDirectory);
                    Debug.Log("Asset Bundle folder created: " + destinationDirectory);
                }
                BuildPipeline.BuildAssetBundles(destinationDirectory, buildMap, BuildAssetBundleOptions.StrictMode, targetPlatform);

            //try
            //{
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RoomPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}
            //catch (System.ArgumentException)
            //{
            //    Directory.CreateDirectory(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory());
            //    Debug.Log("Asset Bundle folder created: " + Config.AssetBundle.RoomPackage.CompileAbsoluteAssetDirectory());
            //    BuildPipeline.BuildAssetBundles(Config.AssetBundle.RoomPackage.CompileUnityAssetDirectory(), buildMap, BuildAssetBundleOptions.StrictMode, BuildTarget.StandaloneWindows);
            //}


            Debug.Log("Room Prefab Bundle generated at " + Path.Combine(destinationDirectory, buildMap[0].assetBundleName));

            BundleHandler.CleanAssetBundleGeneration(destinationDirectory);

                AssetDatabase.Refresh();
            //}
            //else
            //{
            //    Debug.Log("Asset bundle of processed room failed. Does the room object exist in the scene?");
            //}
        }

#endif
#endregion

        /// <summary>
        /// Runs through the logic of unpacking the room texture bundle, then 
        /// takes the information extracted to generate the room mesh/room 
        /// mesh material appropriately. Assumes certain asset names, asset 
        /// bundle names, and folder locations.
        /// 
        /// NOTE: There is some issue with using constants when specifying 
        /// asset names in a bundle, so names are HARDCODED in the method.
        /// </summary>
        public static void UnpackRoomTextureBundle(string bundlePath, string customMatricesDestinationDirectory, string customOrientationDestinationDirectory, string customMeshesDestinationDirectory, string textureImagesDestinationDirectory)
        {
            AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(bundlePath);

            // Extract specific text file assets
            // NOTE: Asset name has to be hardcoded.
            //TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
            //TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset("RoomOrientation".ToLower()) as TextAsset;
            //TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;

            TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset(Config.MatrixArray.FilenameWithoutExtension.ToLower()) as TextAsset;
            TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset(Config.CustomOrientation.FilenameWithoutExtension.ToLower()) as TextAsset;
            TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset(Config.CustomMesh.FilenameWithoutExtension.ToLower()) as TextAsset;

            string customMatricesFilepath = Path.Combine(customMatricesDestinationDirectory, Config.MatrixArray.CompileFilename());
            File.WriteAllLines(customMatricesFilepath, roomMatricesTextAsset.text.Split('\n'));
            //File.WriteAllLines(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()), roomMatricesTextAsset.text.Split('\n'));
            string customOrientationFilepath = Path.Combine(customOrientationDestinationDirectory, Config.CustomOrientation.CompileFilename());
            File.WriteAllLines(customOrientationFilepath, roomOrientationTextAsset.text.Split('\n'));
            string customMeshesFilepath = Path.Combine(customMeshesDestinationDirectory, Config.CustomMesh.CompileFilename());
            File.WriteAllLines(customMeshesFilepath, roomMeshesTextAsset.text.Split('\n'));
            
            //// Extract camera matrices
            //Matrix4x4[] WorldToCameraMatrixArray;
            //Matrix4x4[] ProjectionMatrixArray;
            //Matrix4x4[] LocalToWorldMatrixArray;
            //MatrixArray.LoadMatrixArrays_FromAssetBundle(roomMatricesTextAsset, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Extract textures
            Texture2D[] rawBundledTexArray = roomTextureBundle.LoadAllAssets<Texture2D>();
            for(int i = 0; i < rawBundledTexArray.Length; i++)
            {
                string textureImageFilepath = Path.Combine(textureImagesDestinationDirectory, Config.Images.CompileFilename(Config.Images.GetIndex(rawBundledTexArray[i].name)));
                //File.WriteAllBytes(Config.Images.CompileAbsoluteAssetPath(Config.Images.CompileFilename(Config.Images.GetIndex(rawBundledTexArray[i].name))), rawBundledTexArray[i].EncodeToPNG());
                File.WriteAllBytes(textureImageFilepath, rawBundledTexArray[i].EncodeToPNG());
            }

            //Texture2D[] bundledTexArray = new Texture2D[rawBundledTexArray.Length];
            //for(int i = 0; i < rawBundledTexArray.Length; i++)
            //{
            //    int imageIndex = Config.Images.GetIndex(rawBundledTexArray[i].name);
            //    bundledTexArray[imageIndex] = rawBundledTexArray[i];
            //}

            //if (bundledTexArray == null)
            //{
            //    Debug.Log("Null tex array");
            //}
            //else
            //{
            //    Debug.Log("Bundled tex array size = " + bundledTexArray.Length);
            //}

//            // Create Texture2DArray, copy items from text asset into array, 
//            // and push into shader
//            Texture2DArray TextureArray = new Texture2DArray(bundledTexArray[0].width, bundledTexArray[0].height, bundledTexArray.Length, bundledTexArray[0].format, false);
//            if (WorldToCameraMatrixArray != null
//                && ProjectionMatrixArray != null
//                && LocalToWorldMatrixArray != null
//                && bundledTexArray != null
//                && TextureArray != null)
//            {
//                for (int i = 0; i < bundledTexArray.Length; i++)
//                {
//                    Graphics.CopyTexture(bundledTexArray[i], 0, 0, TextureArray, i, 0);
//                }

//#if UNITY_EDITOR
//                if (!Directory.Exists(Config.Texture2DArray.CompileAbsoluteAssetDirectory()))
//                {
//                    Directory.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
//                }

//                // Save Texture2DArray as asset if appropriate
//                AssetDatabase.CreateAsset(TextureArray, Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename()));
//                AssetDatabase.SaveAssets();
//#endif
                
//                // Extract room mesh
//                TextAsset meshAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;
//                CustomMesh.LoadMesh(meshAsset);

//                // Generate materials
//                MaterialManager.GenerateRoomMaterials(TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);
//            }
//            else
//            {
//                roomTextureBundle.Unload(true);
//                throw new System.Exception("Asset bundle unload failed.");
//            }

            // Unload asset bundle so future loads will not fail
            roomTextureBundle.Unload(true);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void UnpackRoomTextureBundle()
        {
            string bundlePath = Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename());
            string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory();
            string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory();
            string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory();
            string imagesDestinationDirectory = Config.Images.CompileAbsoluteAssetDirectory();

            UnpackRoomTextureBundle(bundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, imagesDestinationDirectory);
        }

        public static void CreateRoomResources()
        {
            string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory();
            string meshesDirectory = materialsDirectory;
            string texturesDirectory = materialsDirectory;

            // Ensure that previous room items (resources) are deleted
            RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);
            
            // Extract camera matrices
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Extract textures
            Queue<Texture2D> rawTexQueue = new Queue<Texture2D>();
            foreach(string filepath in Directory.GetFiles(Config.Images.CompileAbsoluteAssetDirectory()))
            {
                if(filepath.Contains(Config.Images.FilenameWithoutExtension) && filepath.EndsWith(Config.Images.Extension))
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.LoadImage(File.ReadAllBytes(filepath));
                    tex.name = Path.GetFileNameWithoutExtension(filepath);
                    rawTexQueue.Enqueue(tex);
                }
            }
            Texture2D[] rawTexArray = rawTexQueue.ToArray();
            Texture2D[] sortedTexArray = new Texture2D[rawTexArray.Length];
            for (int i = 0; i < rawTexArray.Length; i++)
            {
                int imageIndex = Config.Images.GetIndex(Path.GetFileNameWithoutExtension(rawTexArray[i].name));
                sortedTexArray[imageIndex] = rawTexArray[i];
            }

            if (sortedTexArray == null)
            {
                Debug.Log("Null tex array");
            }
            else
            {
                Debug.Log("Bundled tex array size = " + sortedTexArray.Length);
            }

            Debug.Log("About to create texture array");

            // Create Texture2DArray
            Texture2DArray TextureArray = new Texture2DArray(sortedTexArray[0].width, sortedTexArray[0].height, sortedTexArray.Length, sortedTexArray[0].format, false);
            if (WorldToCameraMatrixArray != null
                && ProjectionMatrixArray != null
                && LocalToWorldMatrixArray != null
                && sortedTexArray != null
                && TextureArray != null)
            {
                Debug.Log("If statement entered.");

                // Copy textures into texture2Darray
                for (int i = 0; i < sortedTexArray.Length; i++)
                {
                    Graphics.CopyTexture(sortedTexArray[i], 0, 0, TextureArray, i, 0);
                }

#if UNITY_EDITOR
                if (!Directory.Exists(Config.Texture2DArray.CompileAbsoluteAssetDirectory()))
                {
                    //Directory.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
                    AbnormalDirectoryHandler.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
                }

                // Save Texture2DArray as asset if appropriate
                AssetDatabase.CreateAsset(TextureArray, Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename()));
                AssetDatabase.SaveAssets();
#endif

                // Extract room mesh
                CustomMesh.LoadMesh(Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename()));

                Debug.Log("Length of cam array = " + WorldToCameraMatrixArray.Length);
                Debug.Log("Length of proj array = " + ProjectionMatrixArray.Length);
                Debug.Log("Length of Local array = " + LocalToWorldMatrixArray.Length);
                Debug.Log("Length of texture array = " + TextureArray.depth);

                // Generate materials
                MaterialManager.GenerateRoomMaterials(TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void InstantiateRoom()
        {
            // Ensure resources are available
            if(!File.Exists(Config.UnityMeshes.CompileAbsoluteAssetPath(Config.UnityMeshes.CompileFilename(0)))
                || !File.Exists(Config.Material.CompileAbsoluteAssetPath(Config.Material.CompileFilename(0)))
                || !File.Exists(Config.Texture2DArray.CompileAbsoluteAssetPath(Config.Texture2DArray.CompileFilename())))
            {
                if(!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()))
                    || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())))
                {
                    // Grab the raw resources
                    if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename())))
                    {
                        Debug.Log("Room resources for instantiating room cannot be found!");
                        return;
                    }
                    UnpackRoomTextureBundle();
                }
                // Compile the necessary resources
                CreateRoomResources();
            }
            
            // Ensure that previous room items (resources & game objects) are deleted
            RemoveRoomObject();
            
            // Extract camera matrices
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Build room
            //RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
            string[] customOrientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename()));
            RoomModel.BuildRoomObject(customOrientationFileLines, Config.UnityMeshes.AssetSubFolder, Config.Material.AssetSubFolder);
        }

        public static void UnpackFinalRoomTextureBundle(string bundlePath, string destinationDirectory)
        {
            if (File.Exists(bundlePath)) {

                string materialsDirectory = destinationDirectory;
                string meshesDirectory = materialsDirectory;
                string texturesDirectory = materialsDirectory;

                // Ensure that previous room items (resources & game objects) are deleted
                RemoveRoomObject();
                RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);
                
                AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(bundlePath);
                TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
                //Directory.CreateDirectory(roomMatrixPath);
                AbnormalDirectoryHandler.CreateDirectoryFromFile(destinationDirectory);
                File.WriteAllText(destinationDirectory, roomMatricesTextAsset.text);
                
                GameObject room = roomTextureBundle.LoadAsset(Config.Prefab.CompileFilename()) as GameObject;
                room = GameObject.Instantiate(room);
                room.name = Config.RoomObject.GameObjectName;
                // Destroy existing room script (Unity bug causes bad script reference? Or script HAS to be in resources?)
                GameObject.Destroy(room.GetComponent<RoomModel>());
                RoomModel roomModel = room.AddComponent<RoomModel>();

                // Grab the matrices and set them
                Matrix4x4[] worldToCameraArray;
                Matrix4x4[] projectionArray;
                Matrix4x4[] localToWorldArray;
                MatrixArray.LoadMatrixArrays_FromAssetBundle(roomMatricesTextAsset, out worldToCameraArray, out projectionArray, out localToWorldArray);
                roomModel.SetMatrixData(worldToCameraArray, projectionArray, localToWorldArray);

                //// Get the materials
                //// Update by resetting the texture arrays to them to get them to display
                //Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameWithoutExtension + Config.Texture2DArray.Extension) as Texture2DArray;
                //for(int i = 0; i < room.transform.childCount; i++)
                //{
                //    GameObject child = room.transform.GetChild(i).gameObject;
                //    child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);
                //}

                Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameWithoutExtension + Config.Texture2DArray.Extension) as Texture2DArray;
                for(int i = 0; i < room.transform.childCount; i++)
                {
                    GameObject child = room.transform.GetChild(i).gameObject;
                    Material childMaterial = MaterialManager.GenerateRoomMaterial(i, texArray, worldToCameraArray, projectionArray, localToWorldArray[i]);
                    child.GetComponent<MeshRenderer>().sharedMaterial = childMaterial;
                    child.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MyArr", texArray);
                }

                // Unload the room texture bundle to reduce memory usage
                // false indicates you are creating a COPY of the items inside 
                // the room bundle, so you can delete or remove the texture bundle 
                // and not keep it open afterwards as long as the matrices are 
                // dynamically saved
                roomTextureBundle.Unload(false);
            }
            else
            {
                throw new System.Exception("Asset bundle unload failed.");
            }
        }
        
        public static void UnpackFinalRoomTextureBundle()
        {
            string bundlePath = Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename());
            string roomMatrixPath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename());
            UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename()), roomMatrixPath);
        }

#region Helper Functions

        // ERROR TESTING - Removing the manifest is unwanted IF there will be more than one asset bundle
        // associated with something (like the room). Remove the Path.GetExtension(bundleFilepath).Equals(".manifest")
        // line if you want to organize the asset bundles or expand them better by having multiple asset
        // bundles per thing
        public static void CleanAssetBundleGeneration(string bundleDestinationDirectory)
        {
            // Clean up erroneous asset bundle generation
            //string[] bundleFilepaths = Directory.GetFiles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory());
            string[] bundleFilepaths = Directory.GetFiles(bundleDestinationDirectory);

            //Debug.Log("bundle destination directory = " + bundleDestinationDirectory);
            string extraBundleName = GetExtraBundleName(bundleDestinationDirectory);
            for (int i = 0; i < bundleFilepaths.Length; i++)
            {
                string bundleFilepath = bundleFilepaths[i];
                string bundleFilename = Path.GetFileNameWithoutExtension(bundleFilepath);
                if (bundleFilename.Equals(extraBundleName))
                {
                    if (!Path.HasExtension(bundleFilepath) 
                        || Path.GetExtension(bundleFilepath).Equals(".meta")
                        || Path.GetExtension(bundleFilepath).Equals(".manifest"))
                    {
                        //Debug.Log("Deleting " + bundleFilepath);
                        //Debug.Log("filename to compare = " + bundleFilename);

                        File.Delete(bundleFilepath);
                    }
                }
            }
        }
        
        public static string GetExtraBundleName(string bundleDirectory)
        {
            //string[] pass1 = CompileUnityAssetDirectory().Split('/');
            //string[] pass2 = pass1[pass1.Length - 1].Split('\\');
            //return pass2[pass2.Length - 1];

            string[] pass1 = bundleDirectory.Split('/');
            string[] pass2 = pass1[pass1.Length - 1].Split('\\');
            return pass2[pass2.Length - 1];
        }

        public static void RemoveRoomObject()
        {
            GameObject room = GameObject.Find(Config.RoomObject.GameObjectName);
            if(room != null)
            {
                GameObject.DestroyImmediate(room);
            }
        }
        
        // Assumes all resources sit in a subfolder in the resources folder
        public static void RemoveRoomResources(string materialsDirectory, string meshesDirectory, string texturesDirectory)
        {
            // Remove materials
            //string materialAssetFolder = Config.Material.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(materialsDirectory))
            {
                string[] files = Directory.GetFiles(materialsDirectory);
                for(int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Material.FilenameWithoutExtension)
                        && files[i].Contains(Config.Material.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove meshes
            //string meshAssetFolder = Config.UnityMeshes.AbsoluteAssetRootFolder + '/' + Config.CustomMesh.AssetSubFolder;
            if (Directory.Exists(meshesDirectory))
            {
                string[] files = Directory.GetFiles(meshesDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.UnityMeshes.FilenameWithoutExtension)
                        && files[i].Contains(Config.UnityMeshes.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove Texture2DArray
            //string textureArrayFolder = Config.Texture2DArray.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(texturesDirectory))
            {
                string[] files = Directory.GetFiles(texturesDirectory);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Texture2DArray.FilenameWithoutExtension)
                        && files[i].Contains(Config.Texture2DArray.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void RemoveRawInfo()
        {
            // Remove images
            string imageFolder = Config.Images.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(imageFolder))
            {
                string[] files = Directory.GetFiles(imageFolder);
                for(int i = 0; i < files.Length; i++)
                {
                    if(files[i].Contains(Config.Images.FilenameWithoutExtension)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove text assets
            string matrixFileFolder = Config.MatrixArray.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(matrixFileFolder))
            {
                string[] files = Directory.GetFiles(matrixFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameWithoutExtension)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string meshFileFolder = Config.CustomMesh.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(meshFileFolder))
            {
                string[] files = Directory.GetFiles(meshFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameWithoutExtension)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string orientationFileFolder = Config.CustomOrientation.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(orientationFileFolder))
            {
                string[] files = Directory.GetFiles(orientationFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameWithoutExtension)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
        }
#endregion
#endregion
    }
}