using MagicaVoxels.Runtimes.Types;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace MagicaVoxels.Runtimes.Converters
{
    [BurstCompile]
    public struct MagicaVoxelMeshGeneratorJob : IJob
    {
        public int3 size;
        public NativeArray<VoxelData> voxels;
        public float voxelSize;
        public float3 upDirection;
        public NativeList<float3> vertices;
        public NativeList<uint> indices;
        
        // Rietmon: Split to maximum performance
        public void Execute()
        {
            vertices = new NativeList<float3>(Allocator.Temp);
            indices = new NativeList<uint>(Allocator.Temp);
            if (upDirection.x == 1f)
                ProcessForX();
            else if (upDirection.y == 1f)
                ProcessForY();
            else
                ProcessForZ();
        }

        private void ProcessForX()
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var voxel = voxels[x * size.y * size.z + y * size.z + z];
                        if (voxel.Color is { x: 0, y: 0, z: 0, w: 0 })
                            continue;

                        var vertexIndex = (uint)vertices.Length;
                        AddCube(vertexIndex, new float3(z, y, x));
                    }
                }
            }
        }

        private void ProcessForY()
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var voxel = voxels[x * size.y * size.z + y * size.z + z];
                        if (voxel.Color is { x: 0, y: 0, z: 0, w: 0 })
                            continue;

                        var vertexIndex = (uint)vertices.Length;
                        AddCube(vertexIndex, new float3(x, y, z));
                    }
                }
            }
        }
        
        private void ProcessForZ()
        {
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var voxel = voxels[x * size.y * size.z + y * size.z + z];
                        if (voxel.Color is { x: 0, y: 0, z: 0, w: 0 })
                            continue;

                        var vertexIndex = (uint)vertices.Length;
                        AddCube(vertexIndex, new float3(x, z, y));
                    }
                }
            }
        }
        
        [BurstCompile]
        private void AddCube(uint vertexIndex, float3 position)
        {
            vertices.Add((position + new float3(0, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 1)) * voxelSize);

            AddQuadFace(vertexIndex, 0, 1, 2, 3);
            AddQuadFace(vertexIndex, 5, 4, 7, 6);
            AddQuadFace(vertexIndex, 1, 0, 4, 5);
            AddQuadFace(vertexIndex, 3, 2, 6, 7);
            AddQuadFace(vertexIndex, 0, 3, 7, 4);
            AddQuadFace(vertexIndex, 2, 1, 5, 6);
        }
        
        [BurstCompile]
        private void AddQuadFace(uint baseVertexIndex, uint o1, uint o2, uint o3, uint o4)
        {
            indices.Add(baseVertexIndex + o1);
            indices.Add(baseVertexIndex + o2);
            indices.Add(baseVertexIndex + o3);
            indices.Add(baseVertexIndex + o1);
            indices.Add(baseVertexIndex + o3);
            indices.Add(baseVertexIndex + o4);
        }
    }
}