using Unity.Burst;
using Unity.Mathematics;

namespace MagicaVoxels.Runtimes.Types
{
    [BurstCompile]
    public struct VoxelData
    {
        public float4 Color { get; set; }

        public VoxelData(float4 color)
        {
            Color = color;
        }
    }
}