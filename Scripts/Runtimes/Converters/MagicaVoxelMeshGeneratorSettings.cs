using System;
using UnityEngine;

namespace UnityMagicaVoxels.Runtimes.Converters
{
    [Serializable]
    public class MagicaVoxelMeshGeneratorSettings
    {
        [field: SerializeField] public float VoxelSize { get; set; } = 0.1f;
    }
}