using MagicaVoxels.Runtimes.Files;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace MagicaVoxels.Runtimes.Converters
{
    public static class MagicaVoxelConverter
    {
        public static Mesh ConvertToMesh(this MagicaVoxelFile file, int index, MagicaVoxelMeshGeneratorSettings settings)
        {
            var data = file.CreateGroupData(index);
            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArray[0];

            var job = new MagicaVoxelMeshGeneratorJob
            {
                size = data.Size,
                voxels = data.Voxels,
                voxelSize = settings.VoxelSize,
                upDirection = settings.UpDirection switch
                {
                    MagicaVoxelMeshGeneratorSettings.Direction.X => new float3(1, 0, 0),
                    MagicaVoxelMeshGeneratorSettings.Direction.Y => new float3(0, 1, 0),
                    MagicaVoxelMeshGeneratorSettings.Direction.Z => new float3(0, 0, 1),
                    _ => throw new System.NotImplementedException()
                }
            };
            job.Execute();
            data.Dispose();
            
            meshData.SetVertexBufferParams(job.vertices.Length, new VertexAttributeDescriptor(VertexAttribute.Position));
            meshData.SetIndexBufferParams(job.indices.Length, IndexFormat.UInt32);

            var meshDataVertices = meshData.GetVertexData<float3>();
            meshDataVertices.CopyFrom(job.vertices.AsArray());

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