using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ManagerSystem
{
    /// <summary>
    /// Component which registers managers based on the objects's lifetime
    /// </summary>
    public class RegisterManagers : MonoBehaviour
    {
        [SerializeField] public List<MonoBehaviour> managers;

        private void OnEnable()
        {
            foreach (var manager in managers)
            {
                Managers.RegisterManager(manager);
            }
        }

        private void OnDisable()
        {
            foreach (var manager in managers)
            {
                Managers.UnregisterManager(manager);
            }
        }
    }
}
