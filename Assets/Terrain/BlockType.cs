using UnityEngine;

namespace Terrain
{
    [CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
    public class BlockType : ScriptableObject
    {
        [SerializeField] public string blockName;
        [SerializeField] public Sprite sprite;
        [SerializeField] public Color color = Color.white;

        public int Index { get; set; }
        public Rect UVs { get; set; }
    }
}
