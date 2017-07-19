using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Encapsulates behavior interacting with the RoomMesh object and settings 
    /// its shader parameters. Expected to be attached to the parent room model 
    /// game object.
    /// </summary>
    public class RoomModel : MonoBehaviour
    {
        #region Fields
        private static Matrix4x4[] worldToCameraMatrixArray;
        private static Matrix4x4[] projectionMatrixArray;
        private static Matrix4x4[] localToWorldMatrixArray;
        //public static Texture2DArray TextureArray;
        //private static Material[] MeshMaterials;
        #endregion

        #region Methods

        void Start()
        {
            // Deep copy matrix data
            GetMatrixData(out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);

            // Start the refresh cycle for the matrix data
            BeginShaderRefreshCycle(Config.RoomObject.RecommendedShaderRefreshTime);
        }

        public void GetMatrixData(out Matrix4x4[] worldToCameraMatrixArray, out Matrix4x4[] projectionMatrixArray, out Matrix4x4[] localToWorldMatrixArray)
        {
            // Try loading from file
            bool loaded = MatrixArray.LoadMatrixArrays_AssetsStored(out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);
            // If file not available, try loading from asset bundle
            if (!loaded)
            {
                // NOTE: If running into an error with the asset bundle here, you MUST DELETE THE BUNDLE, and rebundle it so the open reference closes
                AssetBundle bundle = AssetBundle.LoadFromFile(Config.AssetBundle.RoomPackage.CompileAbsoluteAssetPath(Config.AssetBundle.RoomPackage.CompileFilename()));
                TextAsset matrixAsset = bundle.LoadAsset<TextAsset>("RoomMatrices".ToLower());
                MatrixArray.LoadMatrixArrays_FromAssetBundle(matrixAsset, out worldToCameraMatrixArray, out projectionMatrixArray, out localToWorldMatrixArray);
                loaded = true;
            }
            else
            {
                Debug.Log("RoomModel matrix data loaded from stored assets.");
            }
        }

        public void BeginShaderRefreshCycle(float refreshTime)
        {
            InvokeRepeating("RefreshShader", 0.0f, refreshTime);
        }

        public void RefreshShader()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
                if (childMeshRenderer != null)
                {
                    Material childMaterial = childMeshRenderer.sharedMaterial;
                    if (childMaterial != null)
                    {
                        MaterialManager.RefreshShaderMatrices(childMaterial, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray[i]);
                        //ERROR TESTING - REMOVE // childMeshRenderer.sharedMaterial = childMaterial;
                    }
                }
            }
        }
        
        public static GameObject BuildRoomObject(string[] orientationFileLines)
        {
            if (!Directory.Exists(Config.CustomMesh.CompileAbsoluteAssetDirectory()))
            {
                //Directory.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory());
                AbnormalDirectoryHandler.CreateDirectory(Config.CustomMesh.CompileAbsoluteAssetDirectory());
            }

            Vector3[] positionArray;
            Quaternion[] rotationArray;
            
            // Load up the information stored from mesh supplementary information 
            // to correct for Hololens mesh translation and orientation.
            CustomOrientation.Load(orientationFileLines, out positionArray, out rotationArray);

            GameObject roomObject = new GameObject();
            roomObject.name = Config.RoomObject.GameObjectName;
            for(int i = 0; i < positionArray.Length; i++)
            {
                GameObject child = new GameObject();

                // Set mesh
                Mesh childMesh = Resources.Load(Config.UnityMeshes.CompileResourcesLoadPath(Config.UnityMeshes.CompileMeshName(i))) as Mesh;
                MeshFilter mf = child.AddComponent<MeshFilter>();
                mf.sharedMesh = childMesh;

                // Set material
                MeshRenderer mr = child.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load<Material>(Config.Material.CompileResourcesLoadPath(Config.Material.CompileMaterialName(i)));

                // Set position and rotation
                child.transform.position = positionArray[i];
                child.transform.rotation = rotationArray[i];

                // Set name
                child.name = childMesh.name;
                child.transform.parent = roomObject.transform;

                // Add mesh collider
                MeshCollider mc = child.AddComponent<MeshCollider>();
                mc.sharedMesh = childMesh;
            }
            roomObject.AddComponent<RoomModel>();

            // Integrate it into the UWB Network
            roomObject.AddComponent<UWBPhotonTransformView>();

            return roomObject;
        }


        //============================================================



        //public void FirstTimeSetup(float refreshTime, Texture2DArray tex2DArr, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        //{
        //    Debug.Log("WorldToCam in FirstTimeSetup is null " + ((worldToCameraMatrixArray == null) ? "true" : "false"));

        //    //DeepCopyTextureItems(tex2DArr, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);
        //    DeepCopyTextureItems_AssetsStored();
        //    MaterialManager.GenerateRoomMaterials(tex2DArr, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);
        //    //InvokeRepeating("SetShaderParams_AssetsStored", 0.0f, refreshTime);
        //    BeginShaderRefreshCycle(refreshTime);
        //}

        

       
        //public static void DeepCopyTextureItems_AssetsStored()
        //{
        //    // Grab the Texture2DArray from the asset hierarchy
        //    string texArrName = CrossPlatformNames.Texture2DArray.ArrayName;
        //    Texture2DArray texArr = Resources.Load(CrossPlatformNames.Texture2DArray.GetResourcesLoadPath(texArrName)) as Texture2DArray;

        //    // Grab the worldToCameraMatrixArray
        //    Matrix4x4[] wtcMA;
        //    // Grab the projectionMatrixArray
        //    Matrix4x4[] pMA;
        //    // Grab the localToWorldMatrixArray
        //    Matrix4x4[] ltwMA;
        //    MatrixArray.LoadMatrixArrays_AssetsStored(out wtcMA, out pMA, out ltwMA);

        //    DeepCopyTextureItems(texArr, wtcMA, pMA, ltwMA);
        //}

        ///// <summary>
        ///// Deep copies objects passed in to the corresponding arrays and 
        ///// Texture2DArray for reference during repetitive shader parameter 
        ///// setting.
        ///// </summary>
        ///// <param name="texArr">
        ///// Texture2DArray to copy.
        ///// </param>
        ///// <param name="worldToCamArr">
        ///// WorldToCamera matrix array to copy. (Matrix that translates 
        ///// vertices from world space to camera/view space.)
        ///// </param>
        ///// <param name="projectorArray">
        ///// Projection matrix array to copy. (Matrix that translates from
        ///// camera/view space to clip space.)
        ///// </param>
        ///// <param name="localToWorldArr">
        ///// LocalToWorld matrix array to copy. (Matrix that translates from
        ///// model coordinate space to world space.)
        ///// </param>
        //public static void DeepCopyTextureItems(Texture2DArray texArr, Matrix4x4[] worldToCamArr, Matrix4x4[] projectorArray, Matrix4x4[] localToWorldArr)
        //{
        //    // Deep copy Texture2DArray
        //    TextureArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth, TextureFormat.RGBA32, false);
        //    for (int i = 0; i < texArr.depth; i++)
        //    {
        //        TextureArray.SetPixels(texArr.GetPixels(i), i);
        //    }
        //    TextureArray.Apply();
        //    // Deep copy worldToCameraMatrixArray
        //    worldToCameraMatrixArray = new Matrix4x4[worldToCamArr.Length];
        //    worldToCamArr.CopyTo(worldToCameraMatrixArray, 0);
        //    // Deep copy projectionMatrixArray
        //    projectionMatrixArray = new Matrix4x4[projectorArray.Length];
        //    projectorArray.CopyTo(projectionMatrixArray, 0);
        //    // Deep copy localToWorldMatrixArray
        //    localToWorldMatrixArray = new Matrix4x4[localToWorldArr.Length];
        //    localToWorldArr.CopyTo(localToWorldMatrixArray, 0);
        //}

        #endregion
    }
}