using System;
using UnityEngine;

namespace MagicaVoxels.Runtimes.Converters
{
    [Serializable]
    public class MagicaVoxelMeshGeneratorSettings
    {
        [field: SerializeField] public Direction UpDirection { get; set; } = Direction.Z;
        
        [field: SerializeField] public float VoxelSize { get; set; } = 0.1f;
        
        public enum Direction { X, Y, Z }
    }
}