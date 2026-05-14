using UnityEngine;

namespace ManagerSystem
{
    [CreateAssetMenu(fileName = "ManagerObject", menuName = "Scriptable Objects/Manager Object", order = 1)]
    public class ManagerSO : ScriptableObject
    {
        [Tooltip("The Prefab to create for this manager")]
        [SerializeField] public GameObject prefab;
        
        [Tooltip("Should this manager be created on program start")]
        [SerializeField] public bool spawnOnInitialize = true;
    }
}