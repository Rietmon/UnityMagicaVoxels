using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace MagicaVoxels.Runtimes.Types
{
    [BurstCompile]
    public struct VoxelGroupData : IDisposable
    {
        public int3 Size { get; set; }

        public NativeArray<VoxelData> Voxels
        {
            get => voxels;
            set => voxels = value;
        }

        private NativeArray<VoxelData> voxels;
        
        public VoxelGroupData(int3 size, NativeArray<VoxelData> voxels)
        {
            Size = size;
            this.voxels = voxels;
            if (size.x * size.y * size.z != voxels.Length)
                throw new Exception($"Size and voxels length mismatch. Size={size.x * size.y * size.z}, Length={voxels.Length}");
        }

        public VoxelData this[int x, int y, int z]
        {
            get => voxels[x * Size.x * Size.z + y * Size.z + z];
            set => voxels[x * Size.x * Size.z + y * Size.z + z] = value;
        }

        public VoxelData this[int3 position]
        {
            get => this[position.x, position.y, position.z];
            set => this[position.x, position.y, position.z] = value;
        }

        public void Dispose()
        {
            voxels.Dispose();
        }
    }
}