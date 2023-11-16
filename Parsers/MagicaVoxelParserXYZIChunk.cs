using System.IO;
using UnityEngine;

namespace MagicaVoxels.Parsers
{
    public class MagicaVoxelParserXYZIChunk : MagicaVoxelParserChunk
    {
        public int VoxelsCount { get; set; }
        public MagicaVoxelParserXYZIVoxelData[] Voxels { get; set; }
        
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            VoxelsCount = reader.ReadInt32();
            Voxels = new MagicaVoxelParserXYZIVoxelData[VoxelsCount];
            for (var i = 0; i < VoxelsCount; i++)
            {
                Voxels[i] = new MagicaVoxelParserXYZIVoxelData(
                    reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            }
        }
    }

    public class MagicaVoxelParserXYZIVoxelData
    {
        public Vector3Int Position { get; set; }
        public byte ColorIndex { get; set; }

        public MagicaVoxelParserXYZIVoxelData(byte x, byte y, byte z, byte colorIndex)
        {
            Position = new Vector3Int(x, y, z);
            ColorIndex = colorIndex;
        }
    }
}