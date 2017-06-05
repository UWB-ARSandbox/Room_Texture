using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

internal sealed class CustomImageImporter : AssetPostprocessor { 
#if UNITY_EDITOR
    private void OnPreprocessTexture()
    {
        var filename = assetPath.Substring(assetPath.LastIndexOf('/') + 1);

        var importer = assetImporter as TextureImporter;
        //importer.SetPlatformTextureSettings("Standalone", standaloneTextureSize, importer.DoesSourceTextureHaveAlpha() ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1, 100);
        importer.textureType = TextureImporterType.Default;
        importer.isReadable = false;
        importer.filterMode = FilterMode.Trilinear;
        importer.anisoLevel = 9;
    }
    #endif
}
