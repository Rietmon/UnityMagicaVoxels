using System;
using UnityEngine;

namespace UnityMagicaVoxels.Runtimes.Files
{
    [Serializable]
    public class MagicaVoxelModel
    {
        [field: SerializeField] public Vector3Int Size { get; set; }
        
        [field: SerializeField] public MagicaVoxelData[] Voxels { get; set; }
        
        public MagicaVoxelModel(Vector3Int size, MagicaVoxelData[] voxels)
        {
            Size = size;
            Voxels = voxels;
        }
    }
}