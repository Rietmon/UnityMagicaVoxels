using System.IO;
using System.Text;

namespace UnityMagicaVoxels.Parsers
{
    public class MagicaVoxelParserChunk
    {
        public string Id { get; set; }
        public int ContentSize { get; set; }
        public int ChildrenSize { get; set; }

        public virtual void Read(BinaryReader reader)
        {
            Id = Encoding.ASCII.GetString(reader.ReadBytes(4));
            ContentSize = reader.ReadInt32();
            ChildrenSize = reader.ReadInt32();
        }
    }
}