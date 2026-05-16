using Terrain;
using Terrain.Blocks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "GameTool", menuName = "Scriptable Objects/Tools/Select Tool")]
    public class SelectTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        
        public override Sprite Sprite => toolSprite;
    }
}