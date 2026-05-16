using ManagerSystem;
using Terrain;
using Terrain.Blocks;
using UnityEngine;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Change Tool", menuName = "Scriptable Objects/Tools/Change Tool")]
    public class ChangeTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private BlockType block;
        
        public override Sprite Sprite => toolSprite;
        
        private TerrainManager terrainManager;
        
        public override void Select()
        {
            base.Select();
            terrainManager = Managers.GetManager<TerrainManager>();
        }

        public override void TerrainLeftButtonPressed(TerrainPointerInfo info)
        {
            base.TerrainLeftButtonPressed(info);
            terrainManager?.SetBlock(info.BackPosition, block);
        }
    }
}