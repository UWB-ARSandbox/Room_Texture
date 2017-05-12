using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Encapsulates behavior interacting with the RoomMesh object and settings 
    /// its shader parameters.
    /// </summary>
    public class RoomModel : MonoBehaviour
    {
        #region Constants
        public static string GameObjectName = "RoomManager";
        public static string RoomName = "RoomMesh";
        public static float RecommendedShaderRefreshTime = 10.0f;
        #endregion

        #region Fields
        private static GameObject room;
        private static Matrix4x4[] worldToCameraMatrixArray;
        private static Matrix4x4[] projectionMatrixArray;
        private static Matrix4x4[] localToWorldMatrixArray;
        public static Texture2DArray TextureArray;
        private static Material[] MeshMaterials;
        #endregion

        #region Methods

        /// <summary>
        /// Initial startup behavior. Sets the locally pointed to GameObject 
        /// signifying the room mesh, and forces this script's GameObject to 
        /// use the GameObjectName as its name.
        /// </summary>
        void Start()
        {
            room = GameObject.Find(RoomName);
            gameObject.name = GameObjectName;
        }
        
        /// <summary>
        /// Since shaders don't store information locally, textures reset 
        /// inappropriately if not refreshed every so often. This function is a 
        /// wrapper to start calling SetShaderParams every x seconds.
        /// </summary>
        public void BeginShaderRefreshCycle(float refreshTime, Texture2DArray tex2DArr, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        {
            DeepCopyTextureItems(tex2DArr, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);
            SetStoredRoomMaterialArray();

            InvokeRepeating("SetShaderParams", 0.0f, refreshTime);
        }

        /// <summary>
        /// Handle the logic generating and storing materials for repetitive 
        /// shader parameter setting.
        /// </summary>
        public void SetStoredRoomMaterialArray()
        {
            if (!RoomExists())
            {
                UpdateRoom();
                if (!RoomExists())
                    return;
            }
            
            MaterialManager.GenerateRoomMaterials(out MeshMaterials, TextureArray, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);
        }

        ///// <summary>
        ///// When game is played, reapply those matrices. Returns true if they got set (i.e. room existed). If false, try checking the room name.
        ///// </summary>
        //public static bool SetShaderParams(Texture2DArray tex2DArr, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        //{
        //    if (!RoomExists())
        //    {
        //        UpdateRoom();
        //        if (!RoomExists())
        //            return false;
        //    }

        //    // Deep copy items
        //    DeepCopyTextureItems(tex2DArr, worldToCameraMatrixArray, projectionMatrixArray);
        //    // ERROR TESTING - HAven't copied localtoworldmatrixarray yet

        //    //Material roomMaterial = MaterialManager.GenerateRoomMaterial(tex2DArr, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);

        //    Material[] roomMaterials;
        //    MaterialManager.GenerateRoomMaterials(out roomMaterials, tex2DArr, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray);
        //    //MeshRenderer roomMeshRenderer = room.GetComponent<MeshRenderer>();
        //    //if(roomMeshRenderer == null)
        //    //{
        //    //    roomMeshRenderer = room.AddComponent<MeshRenderer>();
        //    //}
        //    //roomMeshRenderer.sharedMaterials = roomMaterials;


        //    //roomMeshRenderer.material = roomMaterial;
        //    //SetChildShaderParams(roomMaterial, room);

        //    SetChildShaderParams(roomMaterials, room);

        //    return true;
        //}

        /// <summary>
        /// Logic for setting the shader parameters on the child objects of the 
        /// pointed-to room mesh object.
        /// </summary>
        private void SetShaderParams()
        {
            for (int i = 0; i < room.transform.childCount; i++)
            {
                GameObject child = room.transform.GetChild(i).gameObject;
                MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
                if (childMeshRenderer != null)
                {
                    childMeshRenderer.sharedMaterial = MeshMaterials[i];
                }
            }
        }

        /// <summary>
        /// Searches the scene object hierarchy for a GameObject with a name 
        /// specified by RoomName and updates "room" to point to the found 
        /// GameObject.
        /// </summary>
        /// <returns>
        /// True if room was updated. False otherwise.
        /// </returns>
        public static bool UpdateRoom()
        {
            GameObject newRoom = GameObject.Find(RoomName);

            if (newRoom != null)
            {
                room = newRoom;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Getter function for grabbing the GameObject that is stored as the 
        /// room mesh parent GameObject.
        /// </summary>
        /// <returns>
        /// GameObject stored locally as the room mesh object.
        /// </returns>
        public static GameObject GetRoom()
        {
            return room;
        }

        /// <summary>
        /// Determines if the room mesh is not null.
        /// </summary>
        /// <returns>
        /// True if the room mesh exists/was set at some point. False if null.
        /// </returns>
        public static bool RoomExists()
        {
            return room != null;
        }

        /// <summary>
        /// Deep copies objects passed in to the corresponding arrays and 
        /// Texture2DArray for reference during repetitive shader parameter 
        /// setting.
        /// </summary>
        /// <param name="texArr">
        /// Texture2DArray to copy.
        /// </param>
        /// <param name="worldToCamArr">
        /// WorldToCamera matrix array to copy. (Matrix that translates 
        /// vertices from world space to camera/view space.)
        /// </param>
        /// <param name="projectorArray">
        /// Projection matrix array to copy. (Matrix that translates from
        /// camera/view space to clip space.)
        /// </param>
        /// <param name="localToWorldArr">
        /// LocalToWorld matrix array to copy. (Matrix that translates from
        /// model coordinate space to world space.)
        /// </param>
        public static void DeepCopyTextureItems(Texture2DArray texArr, Matrix4x4[] worldToCamArr, Matrix4x4[] projectorArray, Matrix4x4[] localToWorldArr)
        {
            // Deep copy Texture2DArray
            TextureArray = new Texture2DArray(texArr.width, texArr.height, texArr.depth, TextureFormat.RGBA32, false);
            for(int i = 0; i < texArr.depth; i++)
            {
                TextureArray.SetPixels(texArr.GetPixels(i), i);
            }
            TextureArray.Apply();
            // Deep copy worldToCameraMatrixArray
            worldToCameraMatrixArray = new Matrix4x4[worldToCamArr.Length];
            worldToCamArr.CopyTo(worldToCameraMatrixArray, 0);
            // Deep copy projectionMatrixArray
            projectionMatrixArray = new Matrix4x4[projectorArray.Length];
            projectorArray.CopyTo(projectionMatrixArray, 0);
            // Deep copy localToWorldMatrixArray
            localToWorldMatrixArray = new Matrix4x4[localToWorldArr.Length];
            localToWorldArr.CopyTo(localToWorldMatrixArray, 0);
        }

        #endregion
    }
}