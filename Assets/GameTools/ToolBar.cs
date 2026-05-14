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
        
        private List<ToolOption> toolOptions = new List<ToolOption>();
        
        private void Start()
        {
            UIDocument uidoc = GetComponent<UIDocument>();
            VisualElement root = uidoc.rootVisualElement;

            VisualElement toolContainer = root.Q<VisualElement>(toolContainerElement);
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
    }
}
