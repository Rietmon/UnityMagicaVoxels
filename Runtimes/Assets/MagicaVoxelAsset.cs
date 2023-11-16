using MagicaVoxels.Runtimes.Files;
using UnityEngine;

namespace MagicaVoxels.Runtimes.Assets
{
    public class MagicaVoxelAsset : ScriptableObject
    {
        [field: SerializeField] public MagicaVoxelFile MagicaVoxelFile { get; set; }
    }
}