using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    public static class CrossPlatformNames
    {
        /// <summary>
        /// Information regarding naming convention for room texture images 
        /// passed around.
        /// </summary>
        public static class Images
        {
            /// <summary>
            /// Prefix for the room texture images that will be saved/transported.
            /// </summary>
            public static string Prefix = "Room";
            /// <summary>
            /// Extension for the room texture images that will be 
            /// saved/transported.
            /// </summary>
            public static string Extension = ".png";
            
            /// <summary>
            /// Suffix for the room texture images that will be saved/transported. 
            /// Adjoins directly after the prefix with no delimiters in between 
            /// (i.e. fullFilename = prefix + suffix + extension;).
            /// </summary>
            /// <param name="imageIndex">
            /// The index of the image that will help uniquely identify it.
            /// </param>
            /// <returns>
            /// A string identifying the room's suffix
            /// </returns>
            public static string GetSuffix(int imageIndex)
            {
                return imageIndex.ToString();
            }

            /// <summary>
            /// Compile the full file name (without directories), given an image 
            /// index.
            /// </summary>
            /// <param name="imageIndex">
            /// The index of the image that will help uniquely identify it.
            /// </param>
            /// <returns>
            /// A string identifying the room texture image.
            /// </returns>
            public static string CompileFilename(int imageIndex)
            {
                return Prefix + GetSuffix(imageIndex) + Extension;
            }
        }

        public static class Mesh
        {
            public static string OutputFilename = "RoomMesh.txt";
            public static string SupplementaryOutputFilename = "SupplementaryInfo.txt";
#if UNITY_WSA_10_0
            public static string OutputFilepath = Application.persistentDataPath + "/" + OutputFilename;
            public static string SupplementaryOutputFilepath = Application.persistentDataPath + "/" + SupplementaryOutputFilename;
#else
            public static string OutputFilepath = Path.Combine(AssetBundle.InputFolder, OutputFilename);
            public static string SupplementaryOutputFilepath = Path.Combine(AssetBundle.InputFolder, SupplementaryOutputFilename);
#endif
        }

        public static class Matrices
        {
            public static string Filename = "RoomMatrices.txt";
#if UNITY_WSA_10_0
            public static string Filepath = Application.persistentDataPath + "/" + Filename;
#else
            public static string Filepath = Path.Combine(AssetBundle.InputFolder, Filename);
#endif
        }

        public static class AssetBundle
        {
            /// <summary>
            /// Name for the room texture information asset bundle. Bundle 
            /// includes textures and text files representing the room mesh, 
            /// localToWorld matrices (transforms model coordinate space to 
            /// world coordinate space), worldToCamera matrices (transforms 
            /// world coordinate space to camera/view space), projection 
            /// matrices (transforms camera/view space to clip space), and 
            /// supplementary information regarding the mesh (positions & 
            /// rotations).
            /// 
            /// This information is designed to be used by the UWB_Texturing 
            /// namespace classes to generate a final RoomMesh object from 
            /// the components described.
            /// </summary>
            public static string RoomTextureInfo = "roomtexture";
            /// <summary>
            /// Name for the standalone, textured room mesh GameObject asset 
            /// bundle.
            /// </summary>
            public static string Room = "room";


            public static string InputSubFolder = "TextureData";
            public static string InputFolder = Path.Combine("Assets", InputSubFolder);
            public static string OutputSubFolder = "RoomTextureBundle";
            public static string OutputFolder = Path.Combine(Application.streamingAssetsPath, OutputSubFolder);
            public static string FilePath = Path.Combine(OutputFolder, RoomTextureInfo);
        }
    }
}