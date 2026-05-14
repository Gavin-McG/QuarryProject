using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ManagerSystem.Editor
{
    public class ManagersWindow : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/Managers/Managers Debug Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<ManagersWindow>();
            window.titleContent = new GUIContent("Managers");
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        private void OnGUI()
        {
            DrawHeader();
            
            EditorGUILayout.Space(10);

            DrawManagersList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Managers Debugger", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to view active managers.", MessageType.Info);
            }
        }
        
        private void DrawManagersList()
        {
            EditorGUILayout.LabelField("Active Managers", EditorStyles.boldLabel);

            if (!Application.isPlaying)
                return;

            List<KeyValuePair<Type, MonoBehaviour>> managers =
                Managers.GetCurrentManagers();

            if (managers == null || managers.Count == 0)
            {
                EditorGUILayout.HelpBox("No managers currently registered.", MessageType.Warning);
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var pair in managers)
            {
                DrawManagerEntry(pair.Key, pair.Value);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawManagerEntry(Type type, MonoBehaviour instance)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                // Type name
                EditorGUILayout.LabelField(type.Name, GUILayout.Width(150));

                // Object field (clickable)
                EditorGUILayout.ObjectField(instance, type, true);

                // Ping button
                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                {
                    EditorGUIUtility.PingObject(instance);
                }

                // Select button
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = instance.gameObject;
                }
            }
        }
    }
}
