using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManagerSystem.Editor
{
    [CustomEditor(typeof(ManagerSO))]
    public class ManagerSODrawer : UnityEditor.Editor
    {
        private const string prefabAssignedWarning = "ManagerSO must have an assigned prefab";

        private const string missingManagerProvider = "The assigned prefab does not have a ManagerProvider. If the prefab has any components which should be available to Managers.GetManager, this component must be present.";
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var prefabProperty = serializedObject.FindProperty("prefab");

            // Create warning boxes once
            var prefabWarning = new HelpBox(prefabAssignedWarning, HelpBoxMessageType.Error);
            var providerWarning = new HelpBox(missingManagerProvider, HelpBoxMessageType.Info);

            root.Add(prefabWarning);
            root.Add(providerWarning);

            // Function to refresh warnings
            void RefreshWarnings(SerializedProperty prop)
            {
                GameObject prefab = prop.objectReferenceValue as GameObject;

                // Toggle visibility instead of recreating elements
                prefabWarning.style.display = prefab == null
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

                if (prefab != null)
                {
                    var provider = prefab.GetComponent<RegisterManagers>();
                    providerWarning.style.display = provider == null
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
                }
                else
                {
                    providerWarning.style.display = DisplayStyle.None;
                }
            }

            // Initial state
            RefreshWarnings(prefabProperty);

            // React to changes
            root.TrackPropertyValue(prefabProperty, RefreshWarnings);

            // Draw inspector (use one of the safe methods)
            root.Add(new IMGUIContainer(() =>
            {
                serializedObject.Update();
                DrawDefaultInspector();
                serializedObject.ApplyModifiedProperties();
            }));

            return root;
        }
    }
}
