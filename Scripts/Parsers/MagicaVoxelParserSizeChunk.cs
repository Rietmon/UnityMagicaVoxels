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
            Size = new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }
    }
}