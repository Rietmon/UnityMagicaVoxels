using System;
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityMagicaVoxels.Runtimes.Types;
using Random = UnityEngine.Random;

namespace UnityMagicaVoxels.Runtimes.Files
{
    [Serializable]
    public class MagicaVoxelFile
    {
        [field: SerializeField] public int Version { get; private set; }
        
        [field: SerializeField] public MagicaVoxelModel[] Models { get; private set; }
        
        [field: SerializeField] public MagicaVoxelPalette Palette { get; private set; }

        public MagicaVoxelFile(int version, MagicaVoxelModel[] models, MagicaVoxelPalette palette)
        {
            Version = version;
            Models = models;
            Palette = palette;
        }

        public NativeArray<VoxelGroupData> CreateMultipleGroupData()
        {
            var groupData = new NativeArray<VoxelGroupData>(Models.Length, Allocator.Persistent);
            for (var i = 0; i < Models.Length; i++)
                groupData[i] = CreateGroupData(i);

            return groupData;
        }

        public VoxelGroupData CreateGroupData(int index)
        {
            var model = Models[index];
            var size = new int3(model.Size.x, model.Size.y, model.Size.z);
            var builder = new StringBuilder();
            for (var i = 0; i < model.Voxels.Length; i++)
            {
                var color = (Color)Palette.GetColor(model.Voxels[i].ColorIndex);
                if (color != Color.white)
                    builder.Append($"{color} ");
            }
            Debug.Log(builder.ToString());
            var dataWithPosition = new NativeArray<VoxelDataWithPosition>(model.Voxels.Length, Allocator.Temp);
            for (var j = 0; j < model.Voxels.Length; j++)
            {
                var voxel = model.Voxels[j];
                var color = (Color)Palette.GetColor(voxel.ColorIndex);
                dataWithPosition[j] = new VoxelDataWithPosition(
                    new int3(voxel.Position.x, voxel.Position.y, voxel.Position.z),
                    new float4(color.r, color.g, color.b, color.a));
            }

            var data = SortVoxels(size, dataWithPosition);
            return new VoxelGroupData(size, data);
        }

        [BurstCompile]
        private static NativeArray<VoxelData> SortVoxels(int3 size, NativeArray<VoxelDataWithPosition> source)
        {
            var result = new NativeArray<VoxelData>(size.x * size.y * size.z, Allocator.Persistent);
            for (var i = 0; i < source.Length; i++)
            {
                var voxel = source[i];
                var index = voxel.Position.x * size.y * size.z + voxel.Position.y * size.z + voxel.Position.z;
                result[index] = new VoxelData(voxel.Color);
            }

            return result;
        }
    }
}