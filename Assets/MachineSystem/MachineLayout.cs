using System;
using System.Collections.Generic;
using Terrain;
using UnityEngine;

namespace MachineSystem
{
    [Serializable]
    public struct LayoutBlock
    {
        [SerializeField] public BlockType blockType;
        [SerializeField] public Vector3Int position;
        [SerializeField] public Rotation rotation;
    }
    
    public abstract class MachineLayout
    {
        public abstract IEnumerable<LayoutBlock> GetLayout();
    }

    [Serializable]
    public class SingleBlockLayout : MachineLayout
    {
        [SerializeField] public BlockType blockType;
        [SerializeField] public Rotation Rotation;

        public override IEnumerable<LayoutBlock> GetLayout()
        {
            yield return new LayoutBlock()
            {
                blockType = blockType,
                position = Vector3Int.zero,
                rotation = Rotation,
            };
        }
    }

    [Serializable]
    public class MultiBlockLayout : MachineLayout
    {
        [SerializeField] List<LayoutBlock> blocks;

        public override IEnumerable<LayoutBlock> GetLayout()
        {
            return new List<LayoutBlock>(blocks);
        }
    }
}