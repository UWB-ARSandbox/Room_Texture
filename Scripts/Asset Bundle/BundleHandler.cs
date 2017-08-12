using System.Collections;
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
            string roomName = Config.RoomObject.GameObjectName;
            PackRawRoomTextureBundle(destinationDirectory, targetPlatform, roomName);
        }

        public static void PackRawRoomTextureBundle(string destinationDirectory, BuildTarget targetPlatform, string roomName)
        {
            // Ensure that resources exist in project
            if (!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName))
                || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName))
                || !File.Exists(Config.Images.CompileFilename(0)))
            {
                //Debug.Log("Raw room resources do not exist. Ensure that items have been saved from room texturing device (e.g. Hololens)");
                // Grab the raw resources
                if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName)))
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
            int numTextures = MaterialManager.GetNumTextures(roomName);
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
                textureAssets[index] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.Images.CompileFilename(index)); //Config.AssetBundle.InputFolder
            }
            // Mesh
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.CustomMesh.CompileFilename()); //Config.AssetBundle.InputFolder
            // Mesh Supplementary Info
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.CustomOrientation.CompileFilename()); //Config.AssetBundle.InputFolder
            // Matrix Arrays
            textureAssets[index++] = Path.Combine(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName), Config.MatrixArray.CompileFilename()); //Config.AssetBundle.InputFolder
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

        public static void PackAllFinalRoomBundles(string destinationDirectory, BuildTarget targetPlatform)
        {
            string[] roomNames = RoomManager.GetAllRoomNames();
            foreach (string roomName in roomNames)
            {
                PackFinalRoomBundle(destinationDirectory, targetPlatform, roomName);
            }
        }

        public static void PackFinalRoomBundle(string destinationDirectory, BuildTarget targetPlatform)
        {
            string roomName = Config.RoomObject.GameObjectName;
            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            PackFinalRoomBundle(destinationDirectory, targetPlatform, roomName);
        }

        public static void PackFinalRoomBundle(string destinationDirectory, BuildTarget targetPlatform, string roomName)
        {
            GameObject room = GameObject.Find(roomName);
            if(room == null)
            {
                string roomPrefabPath = Config.Prefab.CompileAbsoluteAssetPath(Config.Prefab.CompileFilename(), roomName);
                if (!File.Exists(roomPrefabPath))
                {
                    InstantiateRoom(roomName);
                    room = GameObject.Find(roomName);
                    if (room != null)
                    {
                        PrefabHandler.CreatePrefab(GameObject.Find(roomName), roomName);
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
                int numMaterials = MaterialManager.GetNumMaterials(roomName);
                string meshDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
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
                    roomAssets[index++] = Config.Material.CompileUnityAssetPath(materialName, roomName);
                }
                // Meshes
                for (int i = 0; i < numMeshFiles; i++)
                {
                    string meshName = Config.UnityMeshes.CompileMeshName(i);
                    // ERROR TESTING - reference Mesh, not CustomMesh
                    roomAssets[index++] = Config.CustomMesh.CompileUnityAssetPath(meshName, roomName);
                }
                // Texture2DArray
                roomAssets[index++] = Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename(), roomName);
                // Room Prefab
                roomAssets[index++] = Config.Prefab.CompileUnityAssetPath(Config.Prefab.CompileFilename(), roomName);
                // Shaders
                roomAssets[index++] = Config.Shader.CompileUnityAssetPath(Config.Shader.CompileFilename(), roomName);
                // Matrix Files
                //roomAssets[index++] = CrossPlatformNames.Matrices.CompileAssetPath(); // ERROR TESTING - Currently sitting outside Resources folder. Fix this.
                //Debug.Log("Matrix filepath = " + Config.AssetBundle.RawPackage.CompileUnityAssetDirectory() + '/' + Config.MatrixArray.CompileFilename());
                roomAssets[index++] = Config.AssetBundle.RawPackage.CompileUnityAssetDirectory(roomName) + '/' + Config.MatrixArray.CompileFilename();
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
        public static void UnpackRawResourceTextureBundle(string roomName, string rawResourceBundlePath, string customMatricesDestinationDirectory, string customOrientationDestinationDirectory, string customMeshesDestinationDirectory, string textureImagesDestinationDirectory)
        {
            AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(rawResourceBundlePath);

            // Extract specific text file assets
            // NOTE: Asset name has to be hardcoded.
            //TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
            //TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset("RoomOrientation".ToLower()) as TextAsset;
            //TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;

            TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset(Config.MatrixArray.FilenameRoot.ToLower()) as TextAsset;
            TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset(Config.CustomOrientation.FilenameRoot.ToLower()) as TextAsset;
            TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset(Config.CustomMesh.FilenameRoot.ToLower()) as TextAsset;

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

        public static void UnpackRawResourceTextureBundle(string roomName)
        {
            string bundlePath = Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName);
            string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
            string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
            string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            string imagesDestinationDirectory = Config.Images.CompileAbsoluteAssetDirectory(roomName);

            UnpackRawResourceTextureBundle(roomName, bundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, imagesDestinationDirectory);
        }

        public static void CreateRoomResources(string roomName, string matrixArrayFilepath, string materialsDirectory, string meshesDirectory, string texturesDirectory, string imagesDirectory)
        {
            //string materialsDirectory = Config.Material.CompileAbsoluteAssetDirectory(roomName);
            //string materialsDirectory = resourcesDirectory;
            //string meshesDirectory = materialsDirectory;
            //string texturesDirectory = materialsDirectory;

            if (!Config.Images.FilenameRoot.Contains(roomName))
            {
                UnityEngine.Debug.Log("Room name of \'" + roomName + "\' does not exist in image filenameRoot of \'" + Config.Images.FilenameRoot + "\'");
                UWB_Texturing.Config.RoomObject.GameObjectName = roomName;
                UnityEngine.Debug.Log("Changed room name in UWB_Texturing Config file to match. BE SURE TO SWITCH BACK IF NECESSARY");
                
                return;
            }

            // Ensure that previous room items (resources) are deleted
            RemoveRoomResources(materialsDirectory, meshesDirectory, texturesDirectory);

            // Extract camera matricesstring matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename());
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(roomName, matrixArrayFilepath, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Extract textures
            Queue<Texture2D> rawTexQueue = new Queue<Texture2D>();
            //foreach(string filepath in Directory.GetFiles(Config.Images.CompileAbsoluteAssetDirectory(roomName)))

            Debug.Log("Images directory = " + imagesDirectory);

            foreach(string filepath in Directory.GetFiles(imagesDirectory))
            {
                if(filepath.Contains(Config.Images.FilenameRoot) && filepath.EndsWith(Config.Images.Extension))
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
                // Copy textures into texture2Darray
                for (int i = 0; i < sortedTexArray.Length; i++)
                {
                    Graphics.CopyTexture(sortedTexArray[i], 0, 0, TextureArray, i, 0);
                }

#if UNITY_EDITOR
                if (!Directory.Exists(Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName)))
                {
                    //Directory.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
                    AbnormalDirectoryHandler.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory(roomName));
                }

                // Save Texture2DArray as asset if appropriate
                AssetDatabase.CreateAsset(TextureArray, Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename(), roomName));
                //AssetDatabase.CreateAsset(TextureArray, texturesDirectory);
                AssetDatabase.SaveAssets();
#endif

                // Extract room mesh
                //CustomMesh.LoadMesh(Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename(), roomName), roomName);
                CustomMesh.LoadMesh(Path.Combine(meshesDirectory, Config.CustomMesh.CompileFilename()), meshesDirectory, roomName);
                
                Debug.Log("Mesh load path = " + Path.Combine(meshesDirectory, Config.CustomMesh.CompileFilename()));

                Debug.Log("Length of cam array = " + WorldToCameraMatrixArray.Length);
                Debug.Log("Length of proj array = " + ProjectionMatrixArray.Length);
                Debug.Log("Length of Local array = " + LocalToWorldMatrixArray.Length);
                Debug.Log("Length of texture array = " + TextureArray.depth);

                // Generate materials
                MaterialManager.GenerateRoomMaterials(roomName, materialsDirectory, TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);
            }
            else
            {
                if(WorldToCameraMatrixArray == null)
                {
                    UnityEngine.Debug.Log("WorldToCameraMatrixArray is null");
                }
                if(LocalToWorldMatrixArray == null)
                {
                    UnityEngine.Debug.Log("LocalToWorldMatrixArray is null");
                }
                if(ProjectionMatrixArray == null)
                {
                    UnityEngine.Debug.Log("ProjectionMatrixArray is null");
                }
                if(sortedTexArray == null)
                {
                    UnityEngine.Debug.Log("SortedTexArray is null");
                }
                if(TextureArray == null)
                {
                    UnityEngine.Debug.Log("TextureArray is null");
                }
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void InstantiateRoom(string roomName)
        {
            string bundlePath = Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName);
            string customMatricesDestinationDirectory = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
            string customOrientationDestinationDirectory = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
            string customMeshesDestinationDirectory = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            string imagesDestinationDirectory = Config.Images.CompileAbsoluteAssetDirectory(roomName);
            string matrixArrayFilepath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            
            Debug.Log("Raw bundle path = " + bundlePath);
            Debug.Log("Custom matrices destination directory = " + customMatricesDestinationDirectory);
            Debug.Log("Custom orientation destiantion directory = " + customOrientationDestinationDirectory);
            Debug.Log("Custom meshes destination directory = " + customMeshesDestinationDirectory);
            Debug.Log("Image destination directory = " + imagesDestinationDirectory);
            Debug.Log("Matrix array filepath = " + matrixArrayFilepath);

            InstantiateRoomFromResources(roomName, bundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, imagesDestinationDirectory, matrixArrayFilepath);
        }

        public static void InstantiateRoomFromResources(string roomName, string rawResourceBundlePath, string customMatricesDestinationDirectory, string customOrientationDestinationDirectory, string customMeshesDestinationDirectory, string textureImagesDestinationDirectory, string matrixArrayFilepath)
        {
            // Ensure resources are available
            if(!File.Exists(Config.UnityMeshes.CompileAbsoluteAssetPath(Config.UnityMeshes.CompileFilename(0), roomName))
                || !File.Exists(Config.Material.CompileAbsoluteAssetPath(Config.Material.CompileFilename(0), roomName))
                || !File.Exists(Config.Texture2DArray.CompileAbsoluteAssetPath(Config.Texture2DArray.CompileFilename(), roomName)))
            {
                if(!File.Exists(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName))
                    || !File.Exists(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName)))
                {
                    // Grab the raw resources
                    if (!File.Exists(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename(), roomName)))
                    {
                        Debug.Log("Room resources for instantiating room cannot be found!");
                        return;
                    }
                    UnpackRawResourceTextureBundle(roomName, rawResourceBundlePath, customMatricesDestinationDirectory, customOrientationDestinationDirectory, customMeshesDestinationDirectory, textureImagesDestinationDirectory);
                }
                // Compile the necessary resources
                string materialDirectory = customMatricesDestinationDirectory;
                string meshesDirectory = customMeshesDestinationDirectory;
                string texturesDirectory = customMatricesDestinationDirectory;
                CreateRoomResources(roomName, matrixArrayFilepath, materialDirectory, meshesDirectory, texturesDirectory, textureImagesDestinationDirectory);
            }
            
            // Ensure that previous room items (resources & game objects) are deleted
            RemoveRoomObject();
            
            // Extract camera matrices
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays_AssetsStored(roomName, matrixArrayFilepath, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Build room
            //RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
            string[] customOrientationFileLines = File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename(), roomName));
            string unityMeshesResourceFolder = string.Join("/", customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.IndexOf("Assets")).Split('\\'));// customMeshesDestinationDirectory.Remove(0, customMeshesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);//Config.UnityMeshes.CompileUnityAssetDirectory(roomName);
            string materialsResourceFolder = string.Join("/", textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.IndexOf("Assets")).Split('\\'));//textureImagesDestinationDirectory.Remove(0, textureImagesDestinationDirectory.LastIndexOf("Resources") + "Resources".Length + 1);// Config.Material.CompileUnityAssetDirectory(roomName);
            //RoomModel.BuildRoomObject(roomName, customOrientationFileLines, Config.UnityMeshes.AssetSubFolder, Config.Material.AssetSubFolder);

            Debug.Log("Unity meshes resource folder (Unity path) = " + unityMeshesResourceFolder);
            Debug.Log("Unity materials resources folder (Unity path) = " + materialsResourceFolder);
            
            RoomModel.BuildRoomObject(roomName, customOrientationFileLines, unityMeshesResourceFolder, materialsResourceFolder);
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
                //TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
                TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset(Config.MatrixArray.FilenameRoot.ToLower()) as TextAsset;
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

                Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameRoot + Config.Texture2DArray.Extension) as Texture2DArray;
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
            string roomName = Config.RoomObject.GameObjectName;
            string bundlePath = Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName);
            //string roomMatrixPath = Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename(), roomName);
            //UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename()), roomMatrixPath);
            UnpackFinalRoomTextureBundle(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename(), roomName), Config.RoomObject.CompileAbsoluteAssetDirectory(roomName));
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
            string roomName = Config.RoomObject.GameObjectName;
            RemoveRoomObject(roomName);
        }

        public static void RemoveRoomObject(string roomName)
        {
            GameObject room = GameObject.Find(roomName);
            if (room != null)
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
                    if (files[i].Contains(Config.Material.FilenameRoot)
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
                    if (files[i].Contains(Config.UnityMeshes.FilenameRoot)
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
                    if (files[i].Contains(Config.Texture2DArray.FilenameRoot)
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

        public static void RemoveRawInfo(string roomName)
        {
            // Remove images
            string imageFolder = Config.Images.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(imageFolder))
            {
                string[] files = Directory.GetFiles(imageFolder);
                for(int i = 0; i < files.Length; i++)
                {
                    if(files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }

            // Remove text assets
            string matrixFileFolder = Config.MatrixArray.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(matrixFileFolder))
            {
                string[] files = Directory.GetFiles(matrixFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string meshFileFolder = Config.CustomMesh.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(meshFileFolder))
            {
                string[] files = Directory.GetFiles(meshFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
                        && files[i].Contains(Config.Images.Extension))
                    {
                        File.Delete(files[i]);
                    }
                }
            }
            string orientationFileFolder = Config.CustomOrientation.CompileAbsoluteAssetDirectory(roomName);
            if (Directory.Exists(orientationFileFolder))
            {
                string[] files = Directory.GetFiles(orientationFileFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(Config.Images.FilenameRoot)
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