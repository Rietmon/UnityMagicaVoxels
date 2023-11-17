using System.IO;
using UnityEngine;

namespace UnityMagicaVoxels.Parsers
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
                var x = reader.ReadByte();
                var z = reader.ReadByte();
                var y = reader.ReadByte();

#if MAGICA_VOXELS_Z_UP
                Voxels[i] = new MagicaVoxelParserXYZIVoxelData(
                    x, y, z, reader.ReadByte());
#else
                Voxels[i] = new MagicaVoxelParserXYZIVoxelData(
                    x, z, y, reader.ReadByte());
#endif
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