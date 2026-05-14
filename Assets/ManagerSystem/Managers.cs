using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ManagerSystem
{
    public static class Managers
    {
        private static readonly Dictionary<Type, MonoBehaviour> managers = new();
        private static readonly Dictionary<ManagerSO, GameObject> managerObjects = new();

        /// <summary>
        /// Gets a list of all ManagerObjects within Resources
        /// </summary>
        private static ManagerSO[] GetAllManagers()
        {
            return Resources.LoadAll<ManagerSO>("");
        }

        /// <summary>
        /// Create the prefab and register managers of a ManagerSO
        /// </summary>
        public static void SpawnManager(ManagerSO managerSO)
        {
            // Create and mark object as DoNotDestroy
            var managerObject = Object.Instantiate(managerSO.prefab);
            managerObjects.Add(managerSO, managerObject);
            Object.DontDestroyOnLoad(managerObject);
        }

        /// <summary>
        /// Destroy the GameObject and unregister managers of a ManagerSO
        /// </summary>
        public static void DestroyManager(ManagerSO managerSO)
        {
            if (!managerObjects.Remove(managerSO, out var managerObject)) return;
            
            // Destroy manager object
            Object.Destroy(managerObject);
        }

        public static void ResetManager(ManagerSO managerSO)
        {
            DestroyManager(managerSO);
            SpawnManager(managerSO);
        }

        /// <summary>
        /// Method used to spawn all manager objects immediately, regardless of scene
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SpawnInitializeManagers()
        {
            managers.Clear();

            var managerSOs = GetAllManagers();
            foreach (var managerSO in managerSOs)
            {
                //Only instantiate those marked for initialize
                if (!managerSO.spawnOnInitialize) continue;

                SpawnManager(managerSO);
            }
        }
        
        //---------------------------------------GetManager--------------------------------------------
        
        /// <summary>
        /// Get the MonoBehavior registered as a manager for a type
        /// </summary>
        public static T GetManager<T>() where T : MonoBehaviour
        {
            MonoBehaviour manager = GetManager(typeof(T));
            return manager as T;
        }

        /// <summary>
        /// Get the MonoBehavior registered as a manager for a type
        /// </summary>
        public static MonoBehaviour GetManager(Type type)
        {
            return managers.GetValueOrDefault(type);
        }

        /// <summary>
        /// Returns a list of all registered managers within the scene
        /// </summary>
        public static List<KeyValuePair<Type, MonoBehaviour>> GetCurrentManagers()
        {
            return managers.ToList();
        }
        
        //--------------------------------------RegisterManager--------------------------------------------

        /// <summary>
        /// Register a MonoBehavior as a manager type
        /// </summary>
        public static void RegisterManager<T>(T manager) where T : MonoBehaviour
        {
            if (!manager) return;
            managers.Add(typeof(T), manager);
        }

        /// <summary>
        /// Register a MonoBehavior as a manager type
        /// </summary>
        public static void RegisterManager(MonoBehaviour manager)
        {
            if (!manager) return;
            Type type = manager.GetType();
            managers.Add(type, manager);
        }

        public static void RegisterManager(Type type, MonoBehaviour manager)
        {
            if (!manager) return;
            managers.Add(type, manager);
        }
        
        //---------------------------------------UnregisterType--------------------------------------------

        /// <summary>
        /// Unregister a type of manager
        /// </summary>
        public static void UnregisterType<T>() where T : MonoBehaviour
        {
            UnregisterType(typeof(T));
        }

        /// <summary>
        /// Unregister a type of manager
        /// </summary>
        public static void UnregisterType(Type type)
        {
            managers.Remove(type);
        }
        
        //-------------------------------------UnregisterManager--------------------------------------------

        /// <summary>
        /// Unregister a MonoBehavior as a Manager
        /// </summary>
        public static void UnregisterManager<T>(T manager) where T : MonoBehaviour
        {
            UnregisterManager(typeof(T), manager);
        }

        /// <summary>
        /// Unregister a MonoBehavior as a Manager
        /// </summary>
        public static void UnregisterManager(MonoBehaviour manager)
        {
            if (!manager) return;
            Type type = manager.GetType();
            UnregisterManager(type, manager);
        }

        /// <summary>
        /// Unregister a MonoBehavior as a Manager
        /// </summary>
        public static void UnregisterManager(Type type, MonoBehaviour manager)
        {
            if (managers.TryGetValue(type, out var currentManager) && currentManager == manager)
            {
                UnregisterType(type);
            }
            else
            {
                // Log warning if the manager is not already registered
                Debug.LogWarning($"Managers: Attempted to Unregister manager of type {type.Name} while not registered.");
            }
        }
    }
}
