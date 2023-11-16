using System;
using UnityEngine;

namespace MagicaVoxels.Runtimes.Files
{
    [Serializable]
    public class MagicaVoxelData
    {
        [field: SerializeField] public Vector3Int Position { get; set; }
        [field: SerializeField] public byte ColorIndex { get; set; }
        
        public MagicaVoxelData(Vector3Int position, byte colorIndex)
        {
            Position = position;
            ColorIndex = colorIndex;
        }
    }
}