using Unity.Burst;
using Unity.Mathematics;

namespace UnityMagicaVoxels.Runtimes.Types
{
    [BurstCompile]
    internal struct VoxelDataWithPosition
    {
        public int3 Position { get; set; }
        public float4 Color { get; set; }

        public VoxelDataWithPosition(int3 position, float4 color)
        {
            Position = position;
            Color = color;
        }
    }
}