using Terrain;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Place Tool")]
    public class PlaceTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private BlockType block;
        
        public override Sprite Sprite => toolSprite;
        
        private TerrainManager terrainManager;

        public override void Select()
        {
            terrainManager = GameObject.Find("TerrainManager")?.GetComponent<TerrainManager>();
        }
        
        // Left click - Place single block

        public override void PressLeft(TerrainHoverInfo info)
        {
            terrainManager?.SetBlock(info.FrontPosition, block);
        }
    }
}