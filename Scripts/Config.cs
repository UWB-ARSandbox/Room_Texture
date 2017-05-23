using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UWB_Texturing
{
    /// <summary>
    /// Stores constants or logic for items that are, or could potentially be, 
    /// shared between different platforms. Also stores constants or logic 
    /// regarding items that are shared between different classes to avoid 
    /// potential dependency issues.
    /// 
    /// If you can't find a constant in a file, it's probably here.
    /// </summary>
    public static class Config
    {
        //public static string ResourcesSubFolder = "Room";
        //public static string AssetSubFolder = "Room_Texture" + '/' + "Resources" + '/' + ResourcesSubFolder;
        //public static string LocalAssetFolder = "Assets" + '/' + AssetSubFolder;
        //public static string AbsoluteAssetFolder = Path.Combine(Application.dataPath, AssetSubFolder);

        /// <summary>
        /// Information regarding naming convention for room texture images 
        /// passed around.
        /// </summary>
        public class Images : Config_Base
        {
            /// <summary>
            /// Extension for the room texture images that will be 
            /// saved/transported.
            /// </summary>
            public static string Extension = ".png";
            /// <summary>
            /// Prefix for the room texture images that will be saved/transported.
            /// </summary>
            public static string FilenameWithoutExtension = "Room";
            public static char Separator = '_';
            
            public static int GetIndex(string name)
            {
                return int.Parse(name.Split(Separator)[1]);
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
                return FilenameWithoutExtension + Separator + imageIndex + Extension;
            }
        }

        public class CustomMesh : Config_Base
        {
            public static string Extension = ".txt";
            public static string FilenameWithoutExtension = "RoomMesh";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
        }

        public class CustomOrientation : Config_Base
        {
            public static string Extension = ".txt";
            public static string FilenameWithoutExtension = "RoomOrientation";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
        }

        public class UnityMeshes : Config_Base
        {
            public static string Extension = ".asset";
            public static string FilenameWithoutExtension = "RoomMesh";
            public static char Separator = '_';
            
            public static int GetIndex(string filename)
            {
                return int.Parse(filename.Split(Separator)[1]);
            }
            public static string CompileFilename(int index)
            {
                return FilenameWithoutExtension + Separator + index + Extension;
            }

            public static string CompileMeshName(int index)
            {
                return FilenameWithoutExtension + Separator + index;
            }
        }

        public class Material : Config_Base
        {
            public static string Extension = ".mat";
            /// <summary>
            /// Name of the material that will be generated. Will be followed 
            /// by a suffix specified during material generation.
            /// </summary>
            public static string FilenameWithoutExtension = "RoomMaterial";
            public static char Separator = '_';

            public static int GetIndex(string materialName)
            {
                return int.Parse(materialName.Split(Separator)[1]);
            }
            public static string CompileFilename(int roomChildIndex)
            {
                return FilenameWithoutExtension + Separator + roomChildIndex + Extension;
            }
            public static string CompileMaterialName(int roomChildIndex)
            {
                return FilenameWithoutExtension + Separator + roomChildIndex;
            }
        }

        public class MatrixArray : Config_Base
        {
            public static string Extension = ".txt";
            public static string FilenameWithoutExtension = "RoomMatrices";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
        }

        public class Texture2DArray : Config_Base
        {
            public static string Extension = ".asset";
            public static string FilenameWithoutExtension = "RoomTextureArray";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
        }

        public class AssetBundle
        {
            public class RawPackage : Config_Base
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
                public static string Name = "roomtexture";

                public static string CompileFilename()
                {
                    return Name;
                }

                public static string GetExtraBundleName()
                {
                    string[] pass1 = CompileUnityAssetDirectory().Split('/');
                    string[] pass2 = pass1[pass1.Length - 1].Split('\\');
                    return pass2[pass2.Length - 1];
                }
            }

            public class RoomPackage : Config_Base
            {
                /// <summary>
                /// Name for the standalone, textured room mesh GameObject asset 
                /// bundle.
                /// </summary>
                public static string Name = "roomprefab";

                public static string CompileFilename()
                {
                    return Name;
                }
            }
        }

        public class Prefab : Config_Base
        {
            public static string Extension = ".prefab";
            public static string FilenameWithoutExtension = "Room";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
        }

        public class RoomObject
        {
            public static string GameObjectName = "Room";
            public static float RecommendedShaderRefreshTime = 5.0f;
        }

        public class Shader : Config_Base
        {
            public static string Extension = ".shader";
            public static string FilenameWithoutExtension = "MyRoomShader";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }



            public static new string AssetSubFolder = Config_Base.AssetSubFolder + "/Shaders";
            public static string QualifiedFilenameWithoutExtension = "Unlit/" + FilenameWithoutExtension;
            
            public static new string CompileUnityAssetDirectory()
            {
                return "Assets/" + AssetSubFolder;
            }
            public static new string CompileUnityAssetPath(string filename)
            {
                return CompileUnityAssetDirectory() + '/' + filename;
            }
            public static new string CompileAbsoluteAssetDirectory()
            {
                return Path.Combine(AbsoluteAssetRootFolder, AssetSubFolder);
            }
            public static new string CompileAbsoluteAssetPath(string filename)
            {
                return Path.Combine(CompileAbsoluteAssetDirectory(), filename);
            }
            public static new string CompileResourcesLoadPath(string assetNameWithoutExtension)
            {
                return AssetSubFolder.Substring(AssetSubFolder.IndexOf("Resources") + "Resources".Length + 1);
            }
        }
    }
}