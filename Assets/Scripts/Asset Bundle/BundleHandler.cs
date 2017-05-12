using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_WSA_10_0
using System.IO;
#endif

#if UNITY_WSA_10_0
using Windows.Storage;
using System.Threading.Tasks;
using System;
#endif

namespace UWB_Texturing
{
    /// <summary>
    /// Handles logic with unbundling asset bundle.
    /// </summary>
    public class BundleHandler : MonoBehaviour
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
            // established bundleFolder (use static string
            AssetBundle roomTextureBundle = AssetBundle.LoadFromFile(CrossPlatformNames.AssetBundle.FilePath);

            // Extract specific text file assets
            // NOTE: Asset name has to be hardcoded.
            TextAsset roomMatricesTextAsset = roomTextureBundle.LoadAsset("RoomMatrices".ToLower()) as TextAsset;

            // Extract camera matrices
            Matrix4x4[] WorldToCameraMatrixArray;
            Matrix4x4[] ProjectionMatrixArray;
            Matrix4x4[] LocalToWorldMatrixArray;
            MatrixArray.LoadMatrixArrays(true, out WorldToCameraMatrixArray, out ProjectionMatrixArray, out LocalToWorldMatrixArray, roomMatricesTextAsset);

            // Extract room mesh & info
            // NOTE: Asset names have to be hardcoded.
            TextAsset supplementaryInfoTextAsset = roomTextureBundle.LoadAsset("SupplementaryInfo".ToLower()) as TextAsset;
            TextAsset roomMeshTextAsset = roomTextureBundle.LoadAsset("RoomMesh".ToLower()) as TextAsset;
            GameObject RoomMesh = CustomMesh.LoadMesh(roomMeshTextAsset, supplementaryInfoTextAsset);

            // Extract textures
            //Texture2D[] bundledTexArray = roomTextureBundle.LoadAllAssets<Texture2D>();
            Texture2D[] rawBundledTexArray = roomTextureBundle.LoadAllAssets<Texture2D>();
            Texture2D[] bundledTexArray = new Texture2D[rawBundledTexArray.Length];
            for(int i = 0; i < rawBundledTexArray.Length; i++)
            {
                int imageIndex = int.Parse(rawBundledTexArray[i].name.Substring(CrossPlatformNames.Images.Prefix.Length));
                bundledTexArray[imageIndex] = rawBundledTexArray[i];
            }

            if (bundledTexArray == null)
            {
                Debug.Log("Null tex array");
            }
            else
            {
                Debug.Log("Bundled tex array size = " + bundledTexArray.Length);
            }

            // Create Texture2DArray, copy items from text asset into array, 
            // and push into shader
            Texture2DArray TextureArray = new Texture2DArray(bundledTexArray[0].width, bundledTexArray[0].height, bundledTexArray.Length, bundledTexArray[0].format, false);
            if (WorldToCameraMatrixArray != null
                && ProjectionMatrixArray != null
                && LocalToWorldMatrixArray != null
                && bundledTexArray != null)
            {
                for (int i = 0; i < bundledTexArray.Length; i++)
                {
                    Graphics.CopyTexture(bundledTexArray[i], 0, 0, TextureArray, i, 0);
                }

                GameObject.Find(RoomModel.GameObjectName).GetComponent<RoomModel>().BeginShaderRefreshCycle(RoomModel.RecommendedShaderRefreshTime, TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);

                // ERROR TESTING - REMOVE
                //RoomModel.SetShaderParams(TextureArray, WorldToCameraMatrixArray, ProjectionMatrixArray, LocalToWorldMatrixArray);
            }
            else
            {
                roomTextureBundle.Unload(true);
                throw new System.Exception("Asset bundle unload failed.");
            }

            // Unload asset bundle so future loads will not fail
            roomTextureBundle.Unload(true);
        }
        
        #region Helper Functions

        /// <summary>
        /// Helper function to count the number of raw texture files 
        /// stored at the appropriate folder. This will not show the number
        /// of textures loaded into an asset bundle, but is meant to be
        /// used during bundling logic.
        /// </summary>
        /// <returns></returns>
        public static int GetNumTextures()
        {
            return GetTextureFiles().Count;
        }

        /// <summary>
        /// Grabs a list of the raw, unpacked texture files stored at the
        /// appropriate folder. This will not grab the textures bundled
        /// in an asset bundle, but is meant to be used during bundling
        /// logic.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTextureFiles()
        {
            List<string> textureFilenameList = new List<string>();

            foreach (string filepath in Directory.GetFiles(Path.Combine(Application.dataPath, "TextureData"))) //Path.Combine(Application.dataPath, "TextureData")))
            {
                if (Path.GetExtension(filepath).Equals(".png"))
                {
                    textureFilenameList.Add(Path.GetFileName(filepath));
                }
            }

            return textureFilenameList;
        }
        #endregion
        #endregion
    }
}