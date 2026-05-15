using UnityEngine;

namespace ItemSystem
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Scriptable Objects/Items/Item Type")]
    public class ItemType : ScriptableObject
    {
        [SerializeField] public Mesh mesh;
        [SerializeField] public Material material;
    }
}
