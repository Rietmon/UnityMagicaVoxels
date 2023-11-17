using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityMagicaVoxels.Runtimes.Files;

namespace UnityMagicaVoxels.Parsers
{
    public static class MagicaVoxelParser
    {
        public static MagicaVoxelFile Parse(string path)
        {
            if (!File.Exists(path))
                return null;
            
            var file = new MagicaVoxelParserFile();
            using var stream = new FileStream(path, FileMode.Open);
            using var reader = new BinaryReader(stream);

            ReadHeader(reader, file);
            ReadMainChunk(reader, file);
            ReadPackChunk(reader, file);
            ReadModelChunks(reader, file);
            ReadPaletteChunk(reader, file);
            
            return file.ToRuntimeFile();
        }

        private static void ReadHeader(BinaryReader reader, MagicaVoxelParserFile file)
        {
            var magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (magic != "VOX ")
                throw new Exception("File is not a MagicaVoxel file!");

            file.Version = reader.ReadInt32();
        }

        private static void ReadMainChunk(BinaryReader reader, MagicaVoxelParserFile file)
        {
            var mainChunk = new MagicaVoxelParserChunk();
            mainChunk.Read(reader);

            if (mainChunk.Id != "MAIN")
                throw new Exception("File don't have a MAIN chunk!");
            if (mainChunk.ChildrenSize == 0)
                throw new Exception("File don't have subchunks!");
            file.MainChunk = mainChunk;
        }
        
        private static void ReadPackChunk(BinaryReader reader, MagicaVoxelParserFile file)
        {
            var packChunk = new MagicaVoxelParserPackChunk();
            packChunk.Read(reader);
            if (packChunk.Id != "PACK")
            {
                reader.BaseStream.Position -= 16;
                packChunk = new MagicaVoxelParserPackChunk
                {
                    Size = 1
                };
            }
            
            if (packChunk.Size <= 0)
                throw new Exception("File don't have models! Size is 0 or less!");
            file.PackChunk = packChunk;
        }
        
        private static void ReadModelChunks(BinaryReader reader, MagicaVoxelParserFile file)
        {
            var sizeChunks = new List<MagicaVoxelParserSizeChunk>();
            var xyziChunks = new List<MagicaVoxelParserXYZIChunk>();
            
            for (var i = 0; i < file.PackChunk.Size; i++)
            {
                var sizeChunk = new MagicaVoxelParserSizeChunk();
                sizeChunk.Read(reader);
                sizeChunks.Add(sizeChunk);
                
                var xyziChunk = new MagicaVoxelParserXYZIChunk();
                xyziChunk.Read(reader);
                xyziChunks.Add(xyziChunk);
            }
            
            file.SizeChunks = sizeChunks;
            file.XYZIChunks = xyziChunks;
        }
        
        private static void ReadPaletteChunk(BinaryReader reader, MagicaVoxelParserFile file)
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
                return;
            
            var paletteChunk = new MagicaVoxelParserPaletteChunk();
            paletteChunk.Read(reader);
            
            if (paletteChunk.Id != "RGBA")
                throw new Exception("File don't have a RGBA chunk!");
            file.PaletteChunk = paletteChunk;
        }
    }
}