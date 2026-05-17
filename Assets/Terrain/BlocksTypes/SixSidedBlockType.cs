using UnityEngine;

namespace Terrain.Blocks
{
    [CreateAssetMenu(fileName = "Six Sided Block", menuName = "Scriptable Objects/Blocks/Six Sided Block")]
    public class SixSidedBlockType : BlockType
    {
        [SerializeField] private Sprite topSprite;
        [SerializeField] private Sprite bottomSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite frontSprite;
        [SerializeField] private Sprite backSprite;

        public override Sprite GetSprite(Direction direction) => direction switch
        {
            Direction.Up => topSprite,
            Direction.Down => bottomSprite,
            Direction.Left => leftSprite,
            Direction.Right => rightSprite,
            Direction.Forward => frontSprite,
            _ => backSprite,
        };
    }
}