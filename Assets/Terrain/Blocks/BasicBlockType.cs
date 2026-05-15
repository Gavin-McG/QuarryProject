using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Basic Block", menuName = "Scriptable Objects/Blocks/Basic Block")]
    public class BasicBlockType : BlockType
    {
        [SerializeField] public string blockName;
        [SerializeField] private Sprite sprite;
        
        public override Sprite GetSprite(Direction direction) => sprite;
    }
}
