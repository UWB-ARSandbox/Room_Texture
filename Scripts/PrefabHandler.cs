using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace UWB_Texturing
{
    public class PrefabHandler : MonoBehaviour
    {
        public static class Messages
        {
            public static string GameObjectDoesNotExist = "Room prefab generation failed. Does Room object exist in the scene object hierarchy?";
        }

#if UNITY_EDITOR
        public static void CreatePrefab(GameObject obj, string roomName)
        {
            if (obj != null)
            {
                //AssetDatabase.CreateAsset(obj, "Assets/Room Texture/Resources/Test.obj");
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();

                if (!Directory.Exists(Config.Prefab.CompileAbsoluteAssetDirectory(roomName)))
                {
                    //Directory.CreateDirectory(Config.Prefab.CompileAbsoluteAssetDirectory());
                    AbnormalDirectoryHandler.CreateDirectory(Config.Prefab.CompileAbsoluteAssetDirectory(roomName));
                    Debug.Log("Prefab folder created: " + Config.Prefab.CompileAbsoluteAssetDirectory(roomName));
                }
                //var emptyPrefab = PrefabUtility.CreateEmptyPrefab(CrossPlatformNames.Prefab.CompileCompatibleOutputFolder() + '/' + CrossPlatformNames.Prefab.Filename);
                //PrefabUtility.ReplacePrefab()
                //PrefabUtility.CreatePrefab(CrossPlatformNames.Prefab.CompileCompatibleOutputFolder() + '/' + CrossPlatformNames.Prefab.Filename, AssetDatabase.Find)

                PrefabUtility.CreatePrefab(Config.Prefab.CompileUnityAssetDirectory(roomName) + '/' + Config.Prefab.CompileFilename(), obj); // CompileCompatibleOutputFolder
                //PrefabUtility.CreatePrefab(CrossPlatformNames.Prefab.OutputFilepath, obj);
                Debug.Log("Room prefab generated at " + Config.Prefab.CompileUnityAssetDirectory(roomName));
                //Debug.Log("Room path = " + CrossPlatformNames.Prefab.OutputFilepath);
                Debug.Log("Room path = " + Config.Prefab.CompileUnityAssetDirectory(roomName) + '/' + Config.Prefab.CompileFilename()); // CompileCompatibleOutputFolder
            }
            else
            {
                Debug.Log(Messages.GameObjectDoesNotExist);
            }
        }

        private static void CreatePrefab(string gameObjectName, string roomName)
        {
            GameObject obj = GameObject.Find(gameObjectName);
            CreatePrefab(obj, roomName);
        }

        public static void DeletePrefabs(string roomName)
        {
            string absoluteFilepath = Path.Combine(Config.Prefab.CompileAbsoluteAssetDirectory(roomName), Config.Prefab.CompileFilename());
            File.Delete(absoluteFilepath);
            //absoluteFilepath = Path.Combine(CrossPlatformNames.Prefab.AbsoluteOutputFolder, CrossPlatformNames.Prefab.StandaloneRoom.Filename);
            //File.Delete(absoluteFilepath);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
#endif
    }
}