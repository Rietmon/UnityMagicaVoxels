#define DEBUG_MAGICA_VOXELS
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityMagicaVoxels.Runtimes.Types;

namespace UnityMagicaVoxels.Runtimes.Converters
{
    [BurstCompile
#if DEBUG_MAGICA_VOXELS
    (Debug = true)
#endif
    ]
    public struct MagicaVoxelMeshGeneratorJob : IJob
    {
        public NativeList<float3> vertices;
        public NativeList<float3> normals;
        public NativeList<float4> colors;
        public NativeList<uint> indices;
        public NativeArray<float> verticesBuffer;

        private readonly int3 size;
        private readonly NativeArray<VoxelData> voxels;
        private readonly float voxelSize;
        private readonly int upDirection;

        public MagicaVoxelMeshGeneratorJob(int3 size, NativeArray<VoxelData> voxels, float voxelSize, int upDirection)
        {
            this = default;
            this.size = size;
            this.voxels = voxels;
            this.voxelSize = voxelSize;
            this.upDirection = upDirection;
        }

        public void Execute()
        {
            vertices = new NativeList<float3>(Allocator.Temp);
            normals = new NativeList<float3>(Allocator.Temp);
            colors = new NativeList<float4>(Allocator.Temp);
            indices = new NativeList<uint>(Allocator.Temp);
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var voxel = voxels[x * size.y * size.z + y * size.z + z];
                        if (IsEmptyVoxel(voxel))
                            continue;

                        switch (upDirection)
                        {
                            case 0: AddCube(new int3(y, x, z), voxel.Color); break;
                            case 1: AddCube(new int3(x, y, z), voxel.Color); break;
                            case 2: AddCube(new int3(x, z, y), voxel.Color); break;
                        }
                    }
                }
            }

            verticesBuffer = new NativeArray<float>(vertices.Length * 3
                                                    + colors.Length * 4
                                                    + normals.Length * 3, Allocator.Temp);
            for (var i = 0; i < vertices.Length; i++)
            {
                verticesBuffer[i * 10 + 0] = vertices[i].x;
                verticesBuffer[i * 10 + 1] = vertices[i].y;
                verticesBuffer[i * 10 + 2] = vertices[i].z;
                verticesBuffer[i * 10 + 3] = normals[i].x;
                verticesBuffer[i * 10 + 4] = normals[i].y;
                verticesBuffer[i * 10 + 5] = normals[i].z;
                verticesBuffer[i * 10 + 6] = colors[i].x;
                verticesBuffer[i * 10 + 7] = colors[i].y;
                verticesBuffer[i * 10 + 8] = colors[i].z;
                verticesBuffer[i * 10 + 9] = colors[i].w;
            }
        }

        private void AddCube(int3 position, float4 color)
        {
            if (IsEmptyPosition(position + new int3(0, -1, 0)))
                AddBottomFace(position, color);
            
            if (IsEmptyPosition(position + new int3(0, 0, -1)))
                AddFrontFace(position, color);

            if (IsEmptyPosition(position + new int3(-1, 0, 0)))
                AddLeftFace(position, color);

            if (IsEmptyPosition(position + new int3(0, 0, 1)))
                AddBackFace(position, color);

            if (IsEmptyPosition(position + new int3(1, 0, 0)))
                AddRightFace(position, color);

            if (IsEmptyPosition(position + new int3(0, 1, 0)))
                AddTopFace(position, color);
        }

        private void AddFrontFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(0, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 0)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(0, 0, -1));
                colors.Add(color);
            }
            
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 3);
            indices.Add(baseVertexIndex + 2);
        }
        private void AddBackFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(0, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 1)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(0, 0, 1));
                colors.Add(color);
            }
            
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 3);
        }
        private void AddLeftFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(0, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 0)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(-1, 0, 0));
                colors.Add(color);
            }

            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 3);
        }
        private void AddRightFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(1, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 0)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(1, 0, 0));
                colors.Add(color);
            }
            
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 3);
            indices.Add(baseVertexIndex + 2);
        }
        private void AddTopFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(0, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 1)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(0, 1, 0));
                colors.Add(color);
            }
            
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 3);
            indices.Add(baseVertexIndex + 2);
        }
        private void AddBottomFace(int3 position, float4 color)
        {
            var baseVertexIndex = (uint)vertices.Length;
            vertices.Add((position + new float3(0, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 0, 1)) * voxelSize);

            for (var i = 0; i < 4; i++)
            {
                normals.Add(new float3(0, -1, 0));
                colors.Add(color);
            }

            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 1);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 0);
            indices.Add(baseVertexIndex + 2);
            indices.Add(baseVertexIndex + 3);
        }

        private bool IsEmptyPosition(int3 position)
        {
            position = upDirection switch
            {
                0 => position.yxz,
                1 => position.xyz,
                2 => position.xzy,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (position.x < 0 || position.x >= size.x
                               || position.y < 0 || position.y >= size.y
                               || position.z < 0 || position.z >= size.z)
                return true;
            
            var voxelIndex = position.x * size.y * size.z + position.y * size.z + position.z;
            return voxelIndex < 0 || voxelIndex > voxels.Length || IsEmptyVoxel(voxels[voxelIndex]);
        }

        private static bool IsEmptyVoxel(VoxelData voxel) => voxel.Color is { x: 0, y: 0, z: 0, w: 0 };
    }
}