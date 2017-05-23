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
        /// <summary>
        /// Runs through the logic of unpacking the room texture bundle, then 
        /// takes the information extracted to generate the room mesh/room 
        /// mesh material appropriately. Assumes certain asset names, asset 
        /// bundle names, and folder locations.
        /// 
        /// NOTE: There is some issue with using constants when specifying 
        /// asset names in a bundle, so names are HARDCODED in the method.
        /// </summary>
        public static void UnpackRoomTextureBundle()
        {
            RemoveRoomResources();

            AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(Config.AssetBundle.RawPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RawPackage.CompileFilename()));

            // Extract specific text file assets
            // NOTE: Asset name has to be hardcoded.
            TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
            TextAsset roomOrientationTextAsset = roomTextureBundle.LoadAsset("RoomOrientation".ToLower()) as TextAsset;
            TextAsset roomMeshesTextAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;
            File.WriteAllLines(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()), roomMatricesTextAsset.text.Split('\n'));
            
            //// Extract camera matrices
            //Matrix4x4[] WorldToCameraMatrixArray;
            //Matrix4x4[] ProjectionMatrixArray;
            //Matrix4x4[] LocalToWorldMatrixArray;
            //MatrixArray.LoadMatrixArrays_FromAssetBundle(roomMatricesTextAsset, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray);

            // Extract textures
            Texture2D[] rawBundledTexArray = roomTextureBundle.LoadAllAssets<Texture2D>();
            for(int i = 0; i < rawBundledTexArray.Length; i++)
            {
                File.WriteAllBytes(Config.Images.CompileAbsoluteAssetPath(Config.Images.CompileFilename(Config.Images.GetIndex(rawBundledTexArray[i].name))), rawBundledTexArray[i].EncodeToPNG());
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

        public static void CreateRoomResources()
        {
            // Ensure that previous room items (resources) are deleted
            RemoveRoomResources();
            
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
                    Directory.CreateDirectory(Config.Texture2DArray.CompileAbsoluteAssetDirectory());
                }

                // Save Texture2DArray as asset if appropriate
                AssetDatabase.CreateAsset(TextureArray, Config.Texture2DArray.CompileUnityAssetPath(Config.Texture2DArray.CompileFilename()));
                AssetDatabase.SaveAssets();
#endif

                // Extract room mesh
                CustomMesh.LoadMesh(Config.CustomMesh.CompileAbsoluteAssetPath(Config.CustomMesh.CompileFilename()));

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
            RoomModel.BuildRoomObject(File.ReadAllLines(Config.CustomOrientation.CompileAbsoluteAssetPath(Config.CustomOrientation.CompileFilename())));
        }

        public static void UnpackFinalRoomTextureBundle()
        {
            string bundleFilepath = Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename());
            if (File.Exists(bundleFilepath)) {

                // Ensure that previous room items (resources & game objects) are deleted
                RemoveRoomObject();
                RemoveRoomResources();
                
                AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(bundleFilepath);
                TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;
                File.WriteAllText(Config.MatrixArray.CompileAbsoluteAssetPath(Config.MatrixArray.CompileFilename()), roomMatricesTextAsset.text);
                
                GameObject room = roomTextureBundle.LoadAsset(Config.Prefab.CompileFilename()) as GameObject;
                room = GameObject.Instantiate(room);
                room.name = Config.RoomObject.GameObjectName;
                // Destroy existing room script (Unity bug causes bad script reference? Or script HAS to be in resources?)
                GameObject.Destroy(room.GetComponent<RoomModel>());
                RoomModel roomModel = room.AddComponent<RoomModel>();
                
                Texture2DArray texArray = roomTextureBundle.LoadAsset(Config.Texture2DArray.FilenameWithoutExtension + Config.Texture2DArray.Extension) as Texture2DArray;
                for(int i = 0; i < room.transform.childCount; i++)
                {
                    GameObject child = room.transform.GetChild(i).gameObject;
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
        
#region Helper Functions

        public static void CleanAssetBundleGeneration()
        {
            // Clean up erroneous asset bundle generation
            string[] bundleFilepaths = Directory.GetFiles(Config.AssetBundle.RawPackage.CompileUnityAssetDirectory());
            for (int i = 0; i < bundleFilepaths.Length; i++)
            {
                string bundleFilepath = bundleFilepaths[i];
                string bundleFilename = Path.GetFileNameWithoutExtension(bundleFilepath);
                if (bundleFilename.Equals(Config.AssetBundle.RawPackage.GetExtraBundleName()))
                {
                    File.Delete(bundleFilepath);
                }
            }
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
        public static void RemoveRoomResources()
        {
            // Remove materials
            string materialAssetFolder = Config.Material.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(materialAssetFolder))
            {
                string[] files = Directory.GetFiles(materialAssetFolder);
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
            string meshAssetFolder = Config.UnityMeshes.AbsoluteAssetRootFolder + '/' + Config.CustomMesh.AssetSubFolder;
            if (Directory.Exists(meshAssetFolder))
            {
                string[] files = Directory.GetFiles(meshAssetFolder);
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
            string textureArrayFolder = Config.Texture2DArray.CompileAbsoluteAssetDirectory();
            if (Directory.Exists(textureArrayFolder))
            {
                string[] files = Directory.GetFiles(textureArrayFolder);
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