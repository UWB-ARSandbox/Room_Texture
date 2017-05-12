using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWB_Texturing
{
    /// <summary>
    /// Handles the generation of materials for the room mesh object.
    /// </summary>
    public static class MaterialManager
    {
        /// <summary>
        /// Sub-class for handling string constants viewed in the 
        /// MaterialManager class.
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// Name of the shader without the type listed or extension used.
            /// </summary>
            public static string ShaderName = "MyRoomShader";
            /// <summary>
            /// Name of the shader prefaced by the the type of shader it is. 
            /// Required when using Shader.Find();
            /// </summary>
            public static string ShaderName_Full = "Unlit/" + ShaderName;
            /// <summary>
            /// Name of the material that will be generated. Will be followed 
            /// by a suffix specified during material generation.
            /// </summary>
            public static string MaterialName = "RoomMaterial";
        }

        /// <summary>
        /// Generates array of room materials to be assigned to room mesh objects.
        /// Each material will be associated with a specific mesh, since each mesh
        /// has its own localToWorld matrix.
        /// </summary>
        /// <param name="roomMaterials">
        /// Material array to hold the generated room mesh materials.
        /// </param>
        /// <param name="texArray">
        /// Texture2DArray to apply to each material.
        /// </param>
        /// <param name="worldToCameraMatrixArray">
        /// Array of matrices describing translation of vertices to change 
        /// vertex world space coordinates to camera/view space.
        /// </param>
        /// <param name="projectionMatrixArray">
        /// Array of matrices describing translation of camera/view space
        /// vertex coordinates to clip space.
        /// </param>
        /// <param name="localToWorldMatrixArray">
        /// Array of matrices describing translation of vertices from
        /// local space to world space. Each array is specific to a
        /// single generated mesh and cannot be applied to meshes in general.
        /// </param>
        public static void GenerateRoomMaterials(out Material[] roomMaterials, Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4[] localToWorldMatrixArray)
        {
            roomMaterials = new Material[localToWorldMatrixArray.Length];
            for(int i = 0; i < roomMaterials.Length; i++)
            {
                string materialNameSuffix = "_" + i;
                roomMaterials[i] = GenerateRoomMaterial(texArray, worldToCameraMatrixArray, projectionMatrixArray, localToWorldMatrixArray[i]);
                roomMaterials[i].name = roomMaterials[i].name + materialNameSuffix;
            }
        }

        /// <summary>
        /// Generate a single material for a specific room mesh.
        /// </summary>
        /// <param name="texArray">
        /// Texture2DArray to set for the material.
        /// </param>
        /// <param name="worldToCameraMatrixArray">
        /// Array of matrices describing translation of vertices to change 
        /// vertex world space coordinates to camera/view space.
        /// </param>
        /// <param name="projectionMatrixArray">
        /// Array of matrices describing translation of camera/view space 
        /// vertex coordinates to clip space.
        /// </param>
        /// <param name="localToWorldMatrix">
        /// Array of matrices describing translation of vertices from
        /// local space to world space. Each array is specific to a
        /// single generated mesh and cannot be applied to meshes in general.
        /// </param>
        /// <returns>
        /// The material generated using the passed-in parameters.
        /// </returns>
        public static Material GenerateRoomMaterial(Texture2DArray texArray, Matrix4x4[] worldToCameraMatrixArray, Matrix4x4[] projectionMatrixArray, Matrix4x4 localToWorldMatrix)
        {
            Material mat = new Material(Shader.Find(Constants.ShaderName_Full));
            mat.name = Constants.MaterialName;
            mat.SetTexture("_MyArr", texArray);
            mat.SetMatrixArray("_WorldToCameraMatrixArray", worldToCameraMatrixArray);
            mat.SetMatrixArray("_CameraProjectionMatrixArray", projectionMatrixArray);
            mat.SetMatrix("_MyObjectToWorld", localToWorldMatrix);

            return mat;
        }
    }
}