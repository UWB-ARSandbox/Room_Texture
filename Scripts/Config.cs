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

//            public static string InputFolder = AssetBundle.InputFolder;
//            public static string Extension = ".txt";
//            public static string FilenameWithoutExtension = "RoomMesh";
//            public static string Filename = FilenameWithoutExtension + Extension;
//#if WINDOWS_UWP
////#if UNITY_WSA_10_0
//            public static string Filepath = Application.persistentDataPath + "/" + OutputFilename;
//#else
//            public static string Filepath = Path.Combine(AssetBundle.InputFolder, Filename);
//#endif

//            public static string ResourcesSubFolder = Config.ResourcesSubFolder;
//            public static string LocalAssetFolder = Config.LocalAssetFolder;
//            public static string AbsoluteAssetFolder = Config.AbsoluteAssetFolder;
//            public static string AssetExtension = ".obj";
//            public static char Separator = '_';

//            public static string MeshPrefix = "RoomMesh";
//            public static string CompileMeshName(int index)
//            {
//                return MeshPrefix + Separator + index;
//            }

//            public static string CompileAssetPath(string meshName)
//            {
//                return LocalAssetFolder + '/' + meshName + AssetExtension;
//            }
//            public static string CompileAbsoluteAssetPath(string meshName)
//            {
//                return Path.Combine(AbsoluteAssetFolder, meshName + AssetExtension);
//            }

//            public static string GetResourcesLoadPath(string assetNameWithoutExtension)
//            {
//                return ResourcesSubFolder + '/' + assetNameWithoutExtension;
//            }

//            public static class SupplementaryInfo
//            {
//                public static string Extension = ".txt";
//                public static string FilenameWithoutExtension = "SupplementaryInfo";
//                public static string Filename = FilenameWithoutExtension + Extension;

//#if WINDOWS_UWP
//                // #if UNITY_WSA_10_0
//                public static string Filepath = Application.persistentDataPath + "/" + Filename;
//#else
//                public static string Filepath = Path.Combine(InputFolder, Filename);
//#endif
//            }
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

        //public class Mesh : Config_Base
        //{

        //    public static char Separator = '_';
        //    public static int GetIndex(string filename)
        //    {
        //        return int.Parse(filename.Split(Separator)[1]);
        //    }
        //}

        public class UnityMeshes : Config_Base
        {
            //public static string ResourcesSubFolder = Config.ResourcesSubFolder;
            //public static string LocalAssetFolder = Config.LocalAssetFolder;
            //public static string AbsoluteAssetFolder = Config.AbsoluteAssetFolder;

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
            
            //public static string CompileMeshName(int index)
            //{
            //    return FilenameWithoutExtension + Separator + index;
            //}

            //public static string CompileLocalAssetPath(string meshName)
            //{
            //    return LocalAssetFolder + '/' + meshName + Extension;
            //}
            //public static string CompileAbsoluteAssetPath(string meshName)
            //{
            //    return Path.Combine(AbsoluteAssetFolder, meshName + Extension);
            //}

            //public static int GetIndex(string meshName)
            //{
            //    int index;
            //    string[] components = meshName.Split(Separator);
            //    if(int.TryParse(components[components.Length - 1], out index))
            //    {
            //        return index;
            //    }
            //    else
            //    {
            //        return -1;
            //    }
            //}
        }

        public class Material : Config_Base
        {
            //public static string ResourcesSubFolder = Config.ResourcesSubFolder;
            //public static string LocalAssetFolder = Config.LocalAssetFolder;
            //public static string AbsoluteAssetFolder = Config.AbsoluteAssetFolder;
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


            //public static string CompileAssetPath(string materialName)
            //{
            //    return LocalAssetFolder + '/' + materialName + Extension;
            //}
            //public static string CompileAbsoluteAssetPath(string materialName)
            //{
            //    return Path.Combine(AbsoluteAssetFolder, FilenameWithoutExtension + Extension);
            //}
            
            //public static int GetIndex(string materialName)
            //{
            //    int index;
            //    string[] components = materialName.Split(Separator);
            //    if(int.TryParse(components[components.Length - 1], out index))
            //    {
            //        return index;
            //    }
            //    else
            //    {
            //        return -1;
            //    }
            //}
        }

        public class MatrixArray : Config_Base
        {
            public static string Extension = ".txt";
            public static string FilenameWithoutExtension = "RoomMatrices";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
//            public static string Filename = FilenameWithoutExtension + Extension;
//            public static string LocalAssetFolder = Config.LocalAssetFolder;
//#if WINDOWS_UWP
////#if UNITY_WSA_10_0
//            public static string Filepath = Application.persistentDataPath + "/" + Filename;
//#else
//            public static string Filepath = Path.Combine(AssetBundle.InputFolder, Filename);
//#endif
            
//            public static string CompileAssetPath()
//            {
//                return LocalAssetFolder + '/' + Filename;
//            }
//            public static string CompileAbsoluteAssetPath()
//            {
//                return Path.Combine(AbsoluteAssetFolder, Filename);
//            }
        }

        public class Texture2DArray : Config_Base
        {
            public static string Extension = ".asset";
            public static string FilenameWithoutExtension = "RoomTextureArray";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }
            //public static string CompileResourcesLoadPath(string assetNameWithoutExtension)
            //{
            //    return ResourcesSubFolder + '/' + assetNameWithoutExtension;
            //}

            //public static string ResourcesSubFolder = Config.ResourcesSubFolder;
            //public static string LocalAssetFolder = Config.LocalAssetFolder;
            //public static string AbsoluteAssetFolder = Config.AbsoluteAssetFolder;
            //public static string LocalAssetPath = LocalAssetFolder + '/' + FilenameWithoutExtension + Extension;
            //public static string CompileAssetPath()
            //{
            //    return LocalAssetFolder + '/' + FilenameWithoutExtension + Extension;
            //}
            //public static string CompileAbsoluteAssetPath()
            //{
            //    return Path.Combine(AbsoluteAssetFolder, FilenameWithoutExtension + Extension);
            //}
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

            ///// <summary>
            ///// Name for the room texture information asset bundle. Bundle 
            ///// includes textures and text files representing the room mesh, 
            ///// localToWorld matrices (transforms model coordinate space to 
            ///// world coordinate space), worldToCamera matrices (transforms 
            ///// world coordinate space to camera/view space), projection 
            ///// matrices (transforms camera/view space to clip space), and 
            ///// supplementary information regarding the mesh (positions & 
            ///// rotations).
            ///// 
            ///// This information is designed to be used by the UWB_Texturing 
            ///// namespace classes to generate a final RoomMesh object from 
            ///// the components described.
            ///// </summary>
            //public static string IntermediaryProcessingName = "roomtexture";
            ///// <summary>
            ///// Name for the standalone, textured room mesh GameObject asset 
            ///// bundle.
            ///// </summary>
            //public static string StandaloneRoomName = "room";


            //public static string InputSubFolder = "TextureData";
            //public static string InputFolder = Path.Combine(Path.Combine("Assets", "Room_Texture"), InputSubFolder);
            //public static string OutputSubFolder = "Asset Bundles";
            //public static string OutputFolder = Path.Combine(Path.Combine(Application.dataPath, "Room_Texture"), OutputSubFolder);
            //public static string FilePath = Path.Combine(OutputFolder, IntermediaryProcessingName);
            //public static string GetFilePath(string bundleName)
            //{
            //    return Path.Combine(OutputFolder, bundleName);
            //}
        }

        public class Prefab : Config_Base
        {
            public static string Extension = ".prefab";
            public static string FilenameWithoutExtension = "Room";

            public static string CompileFilename()
            {
                return FilenameWithoutExtension + Extension;
            }




            ////public static string OutputSubFolder = "Resources"
            ////public static string OutputFolder = Path.Combine(Path.Combine("Assets", "Room_Texture"), OutputSubFolder);
            //public static string OutputFolder = Config.LocalAssetFolder;
            ////public static string AbsoluteOutputFolder = Path.Combine(Path.Combine(Application.dataPath, "Room_Texture"), OutputSubFolder);



            //public static string CompileCompatibleOutputFolder()
            //{
            //    string[] components = OutputFolder.Split('\\');
            //    if (components.Length > 0)
            //    {
            //        string compatibleOutputFolder = components[0];
            //        for (int i = 1; i < components.Length; i++)
            //        {
            //            compatibleOutputFolder += '/';
            //            compatibleOutputFolder += components[i];
            //        }

            //        return compatibleOutputFolder;
            //    }
            //    else
            //    {
            //        return string.Empty;
            //    }
            //}

            //public static string AbsoluteOutputFolder = Config.AbsoluteAssetFolder;
            
            ////public static class IntermediaryProcessing {
            //public static string Filename = FilenameWithoutExtension + Extension;
            //public static string CompileAbsoluteAssetPath()
            //{
            //    return Path.Combine(AbsoluteOutputFolder, Filename);
            //}
            //public static string CompileAssetPath()
            //{
            //    return OutputFolder + '/' + Filename;
            //}
            //}

            //public static class StandaloneRoom
            //{
            //    public static string FilenameWithoutExtension = "Room";
            //    public static string FileExtension = ".prefab";
            //    public static string Filename = FilenameWithoutExtension + FileExtension;

            //    public static string CompileAbsoluteAssetPath()
            //    {
            //        return Path.Combine(AbsoluteOutputFolder, Filename);
            //    }
            //}

            //public static string GetAbsoluteOutputFolder()
            //{
            //    string[] directories = CrossPlatformNames.LocalAssetFolder.Split('/');

            //    string folder = "";

            //    // Assumes "Assets" is the first directory to come up
            //    if (directories.Length > 0)
            //    {
            //        folder = Path.Combine(Application.dataPath, directories[1]);
            //        for (int i = 2; i < directories.Length; i++)
            //        {
            //            Path.Combine(folder, directories[i]);
            //        }
            //    }

            //    return folder;
            //}
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



            //public static string LocalAssetFolder = Config.LocalAssetFolder + '/' + ShaderSubFolder;
            //public static string AbsoluteAssetFolder = Path.Combine(Config.AbsoluteAssetFolder, ShaderSubFolder);


            ///// <summary>
            ///// Name of the shader without the type listed or extension used.
            ///// </summary>
            //public static string FilenameWithoutExtension = "MyRoomShader";
            ///// <summary>
            ///// Name of the shader prefaced by the the type of shader it is. 
            ///// Required when using Shader.Find();
            ///// </summary>
            //public static string QualifiedFilenameWithoutExtension = "Unlit/" + FilenameWithoutExtension;
            //public static string Extension = ".shader";

            //public static string CompileAbsoluteAssetPath()
            //{
            //    return Path.Combine(AbsoluteAssetFolder, FilenameWithoutExtension + Extension);
            //}

            //public static string CompileAssetPath()
            //{
            //    return LocalAssetFolder + '/' + FilenameWithoutExtension + Extension;
            //}
        }
    }
}