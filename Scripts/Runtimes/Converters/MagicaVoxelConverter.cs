using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityMagicaVoxels.Runtimes.Files;

namespace UnityMagicaVoxels.Runtimes.Converters
{
    public static class MagicaVoxelConverter
    {
        public static Mesh ConvertToMesh(this MagicaVoxelFile file, int index, MagicaVoxelMeshGeneratorSettings settings)
        {
            var data = file.CreateGroupData(index);
            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArray[0];

            var job = new MagicaVoxelMeshGeneratorJob(data.Size, 
                data.Voxels, 
                settings.VoxelSize);
            job.Execute();
            data.Dispose();

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
            
            var mesh = new Mesh();
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}