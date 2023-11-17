using System.IO;
using UnityEngine;

namespace UnityMagicaVoxels.Parsers
{
    public class MagicaVoxelParserSizeChunk : MagicaVoxelParserChunk
    {
        public Vector3Int Size { get; set; }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            var x = reader.ReadInt32();
            var z = reader.ReadInt32();
            var y = reader.ReadInt32();
            
#if MAGICA_VOXELS_Z_UP
            Size = new Vector3Int(x, y, z);
#else
            Size = new Vector3Int(x, z, y);
#endif
        }
    }
}