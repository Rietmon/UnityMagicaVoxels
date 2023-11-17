using System.IO;

namespace UnityMagicaVoxels.Parsers
{
    public class MagicaVoxelParserPaletteChunk : MagicaVoxelParserChunk
    {
        public uint[] Colors { get; set; }
        
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            Colors = new uint[255];
            for (var i = 0; i < 255; i++)
                Colors[i] = reader.ReadUInt32();
        }
    }
}