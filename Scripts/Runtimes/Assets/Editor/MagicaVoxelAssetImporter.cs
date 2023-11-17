using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityMagicaVoxels.Parsers;
using UnityMagicaVoxels.Runtimes.Converters;

namespace UnityMagicaVoxels.Runtimes.Assets.Editor
{
    [ScriptedImporter(1, "vox")]
    public class MagicaVoxelAssetImporter : ScriptedImporter
    {
        [field: SerializeField] public float VoxelSize { get; set; } = 0.1f;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var asset = ScriptableObject.CreateInstance<MagicaVoxelAsset>();
            var assetName = Path.GetFileName(assetPath);
            asset.name = assetName;
            asset.MagicaVoxelFile = MagicaVoxelParser.Parse(assetPath);
            var mesh = asset.MagicaVoxelFile.GenerateSingleMesh(0, new MagicaVoxelMeshGeneratorSettings
            {
                VoxelSize = VoxelSize
            });
            mesh.RecalculateBounds();
            mesh.name = assetName;
            ctx.AddObjectToAsset(assetName, asset);
            ctx.SetMainObject(asset);
            var assetDirectory = Path.GetDirectoryName(assetPath);

            EditorApplication.delayCall += () => 
                AssetImportPostProcess(assetDirectory, assetName, mesh);
        }

        private static void AssetImportPostProcess(string assetPath, string assetName, Object mesh)
        {
            var assetFullPath = $"{assetPath}/{assetName}.asset";
            if (File.Exists(assetFullPath))
            {
                var loadedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetFullPath);
                loadedMesh.Clear();
                EditorUtility.CopySerialized(mesh, loadedMesh);
            }
            else
            {
                AssetDatabase.CreateAsset(mesh, assetFullPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}