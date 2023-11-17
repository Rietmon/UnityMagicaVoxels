using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityMagicaVoxels.Runtimes.Files;
using UnityMagicaVoxels.Runtimes.Types;

namespace UnityMagicaVoxels.Runtimes.Converters
{
    public static class MagicaVoxelConverter
    {
        public static Mesh GenerateSingleMesh(this MagicaVoxelFile file, int index,
            MagicaVoxelMeshGeneratorSettings settings = default)
        {
            var data = file.CreateGroupData(index);
            var mesh = data.GenerateMesh(settings);
            data.Dispose();
            return mesh;
        }

        public static void UpdateSingleMesh(this MagicaVoxelFile file, int index, Mesh mesh,
            MagicaVoxelMeshGeneratorSettings settings = default)
        {
            var data = file.CreateGroupData(index);
            data.UpdateMesh(mesh, settings);
            data.Dispose();
        }
        
        public static Mesh GenerateMesh(this VoxelGroupData data, MagicaVoxelMeshGeneratorSettings settings = default)
        {
            var mesh = new Mesh();
            data.UpdateMesh(mesh, settings);
            return mesh;
        }

        public static void UpdateMesh(this VoxelGroupData data, Mesh mesh, MagicaVoxelMeshGeneratorSettings settings = default)
        {
            settings ??= new MagicaVoxelMeshGeneratorSettings();
            
            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArray[0];

            meshData.FillMeshDataFromWithVoxels(data, settings);
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
            mesh.RecalculateBounds();
        }

        public static void FillMeshDataFromWithVoxels(this Mesh.MeshData meshData, VoxelGroupData data, 
            MagicaVoxelMeshGeneratorSettings settings)
        {
            var job = new MagicaVoxelMeshGeneratorJob(data.Size, 
                data.Voxels, 
                settings.VoxelSize);
            job.Execute();

            meshData.SetVertexBufferParams(job.vertices.Length,
                new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4));
            meshData.SetIndexBufferParams(job.indices.Length, IndexFormat.UInt32);

            var meshDataVertices = meshData.GetVertexData<float>();
            meshDataVertices.CopyFrom(job.verticesBuffer);

            var meshDataIndices = meshData.GetIndexData<uint>();
            meshDataIndices.CopyFrom(job.indices.AsArray());
            
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, job.indices.Length));
        }
    }
}