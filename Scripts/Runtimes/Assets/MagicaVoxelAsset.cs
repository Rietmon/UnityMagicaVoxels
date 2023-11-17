using UnityEngine;
using UnityMagicaVoxels.Runtimes.Files;

namespace UnityMagicaVoxels.Runtimes.Assets
{
    public class MagicaVoxelAsset : ScriptableObject
    {
        [field: SerializeField] public MagicaVoxelFile MagicaVoxelFile { get; set; }
    }
}