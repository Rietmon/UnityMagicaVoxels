using System.IO;

namespace UnityMagicaVoxels.Parsers
{
    public class MagicaVoxelParserPackChunk : MagicaVoxelParserChunk
    {
        public int Size { get; set; }
        
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            Size = reader.ReadInt32();
        }
    }
}