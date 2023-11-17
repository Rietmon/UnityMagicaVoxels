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
            if (voxels == default)
                throw new Exception("Use custom constructor with voxels parameter!");
                    
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
                        if (voxel.Color is { x: 0, y: 0, z: 0, w: 0 })
                            continue;

                        var vertexIndex = (uint)vertices.Length;
                        switch (upDirection)
                        {
                            case 0: AddCube(vertexIndex, new float3(y, x, z), voxel.Color); break;
                            case 1: AddCube(vertexIndex, new float3(x, y, z), voxel.Color); break;
                            case 2: AddCube(vertexIndex, new float3(x, z, y), voxel.Color); break;
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
        
        private void AddCube(uint vertexIndex, float3 position, float4 color)
        {
            vertices.Add((position + new float3(0, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 0)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 0)) * voxelSize);
            vertices.Add((position + new float3(0, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 0, 1)) * voxelSize);
            vertices.Add((position + new float3(1, 1, 1)) * voxelSize);
            vertices.Add((position + new float3(0, 1, 1)) * voxelSize);
            
            normals.Add(new float3(0, 0, -1));
            normals.Add(new float3(0, 0, -1));
            normals.Add(new float3(0, 0, -1));
            normals.Add(new float3(0, 0, -1));
            normals.Add(new float3(0, 0, 1)); 
            normals.Add(new float3(0, 0, 1)); 
            normals.Add(new float3(0, 0, 1)); 
            normals.Add(new float3(0, 0, 1)); 

            for (var i = 0; i < 8; i++)
                colors.Add(color);

            AddQuadFace(vertexIndex, 0, 1, 2, 3);
            AddQuadFace(vertexIndex, 5, 4, 7, 6);
            AddQuadFace(vertexIndex, 1, 0, 4, 5);
            AddQuadFace(vertexIndex, 3, 2, 6, 7);
            AddQuadFace(vertexIndex, 0, 3, 7, 4);
            AddQuadFace(vertexIndex, 2, 1, 5, 6);
        }
        
        private void AddQuadFace(uint baseVertexIndex, uint o1, uint o2, uint o3, uint o4)
        {
            indices.Add(baseVertexIndex + o1);
            indices.Add(baseVertexIndex + o2);
            indices.Add(baseVertexIndex + o3);
            indices.Add(baseVertexIndex + o1);
            indices.Add(baseVertexIndex + o3);
            indices.Add(baseVertexIndex + o4);
        }

        private bool IsEmptyVoxel(VoxelData voxel) => voxel.Color is { x: 0, y: 0, z: 0, w: 0 };

        private bool IsEmptyPosition(int3 position)
        {
            var voxelIndex = position.x * size.y * size.z + position.y * size.z + position.z;
            return voxelIndex >= 0 && voxelIndex < voxels.Length && IsEmptyVoxel(voxels[voxelIndex]);
        }
    }
}