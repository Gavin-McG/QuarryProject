using System;
using System.Collections.Generic;
using GameTools.Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools
{
    [RequireComponent(typeof(UIDocument))]
    public class ToolBar : MonoBehaviour
    {
        private const string toolContainerElement = "tools_container";

        [SerializeField] private VisualTreeAsset toolOptionElement;
        [SerializeField] private List<GameTool> tools;
        
        private readonly List<ToolOption> toolOptions = new List<ToolOption>();
        
        private void OnEnable()
        {
            // Get tool container element
            UIDocument uidoc = GetComponent<UIDocument>();
            VisualElement root = uidoc.rootVisualElement;
            VisualElement toolContainer = root.Q<VisualElement>(toolContainerElement);
            
            // Add tools to container
            foreach (GameTool tool in tools)
            {
                // Create new visual Element for tool
                VisualElement toolButton = toolOptionElement.CloneTree();
                toolContainer.Add(toolButton);
                
                // Create ToolOption for tool element
                ToolOption newOption = new ToolOption(tool, toolButton);
                toolOptions.Add(newOption);
            }
        }

        private void OnDisable()
        {
            // Remove tools
            foreach (ToolOption option in toolOptions)
            {
                option.RemoveOption();
            }
            toolOptions.Clear();
        }
    }
}
