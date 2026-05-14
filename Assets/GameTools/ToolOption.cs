using System;
using GameTools.Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameTools
{
    [Serializable]
    public class ToolOption
    {
        private const string buttonElementName = "button";
        private const string iconElementName = "tool_icon";
        
        [SerializeField] private GameTool tool;
        [SerializeField] private VisualElement toolOption;


        public ToolOption(GameTool tool, VisualElement toolOption)
        {
            this.tool = tool;
            this.toolOption = toolOption;
            
            // Set tool icon
            Image toolIcon = toolOption.Q<Image>(iconElementName);
            toolIcon.sprite = tool.Sprite;
            
            // Set button callback
            Button toolButton = toolOption.Q<Button>(buttonElementName);
            toolButton.clicked += SelectTool;
        }

        private void SelectTool()
        {
            ToolManager.SetTool(tool);
        }
    }
}
