using System.Collections.Generic;
using UnityMagicaVoxels.Runtimes.Files;

namespace UnityMagicaVoxels.Parsers
{
    public class MagicaVoxelParserFile
    {
        public int Version { get; set; }
        public MagicaVoxelParserChunk MainChunk { get; set; }
        public MagicaVoxelParserPackChunk PackChunk { get; set; }
        public List<MagicaVoxelParserSizeChunk> SizeChunks { get; set; }
        public List<MagicaVoxelParserXYZIChunk> XYZIChunks { get; set; }
        public MagicaVoxelParserPaletteChunk PaletteChunk { get; set; }

        public MagicaVoxelFile ToRuntimeFile() =>
            new(Version, 
                GetModels(), 
                PaletteChunk == null 
                    ? new MagicaVoxelPalette(null) 
                    : new MagicaVoxelPalette(PaletteChunk.Colors));

        private MagicaVoxelModel[] GetModels()
        {
            var result = new MagicaVoxelModel[PackChunk.Size];
            for (var i = 0; i < PackChunk.Size; i++)
            {
                var sizeChunk = SizeChunks[i];
                var xyziChunk = XYZIChunks[i];
                var voxels = new MagicaVoxelData[xyziChunk.VoxelsCount];
                for (var j = 0; j < xyziChunk.Voxels.Length; j++)
                {
                    var voxel = xyziChunk.Voxels[j];
                    voxels[j] = new MagicaVoxelData(voxel.Position, voxel.ColorIndex);
                }
                result[i] = new MagicaVoxelModel(sizeChunk.Size, voxels);
            }

            return result;
        }
    }
}