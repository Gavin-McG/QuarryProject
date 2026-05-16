using System;
using GameTools.Tools;
using ItemSystem;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace GameTools
{
    public class ToolManager : MonoBehaviour
    {
        public static GameTool CurrentTool { get; private set; } = null;
        
        private TerrainManager terrainManager;

        private void OnEnable()
        {
            TerrainManager.TerrainLeftButtonPressed.AddListener(TerrainLeftButtonPressed);
            TerrainManager.TerrainLeftButtonReleased.AddListener(TerrainLeftButtonReleased);
            TerrainManager.TerrainLeftButtonDragged.AddListener(TerrainLeftButtonDragged);
            TerrainManager.TerrainRightButtonPressed.AddListener(TerrainRightButtonPressed);
            TerrainManager.TerrainRightButtonReleased.AddListener(TerrainRightButtonReleased);
            TerrainManager.TerrainRightButtonDragged.AddListener(TerrainRightButtonDragged);
            
            ItemInstance.ItemLeftButtonClicked.AddListener(ItemLeftButtonClicked);
            ItemInstance.ItemRightButtonClicked.AddListener(ItemRightButtonClicked);
        }

        private void OnDisable()
        {
            TerrainManager.TerrainLeftButtonPressed.RemoveListener(TerrainLeftButtonPressed);
            TerrainManager.TerrainLeftButtonReleased.RemoveListener(TerrainLeftButtonReleased);
            TerrainManager.TerrainLeftButtonDragged.RemoveListener(TerrainLeftButtonDragged);
            TerrainManager.TerrainRightButtonPressed.RemoveListener(TerrainRightButtonPressed);
            TerrainManager.TerrainRightButtonReleased.RemoveListener(TerrainRightButtonReleased);
            TerrainManager.TerrainRightButtonDragged.RemoveListener(TerrainRightButtonDragged);
            
            ItemInstance.ItemLeftButtonClicked.RemoveListener(ItemLeftButtonClicked);
            ItemInstance.ItemRightButtonClicked.RemoveListener(ItemRightButtonClicked);
        }

        public static void SetTool(GameTool tool)
        {
            CurrentTool?.Deselect();
            tool.Select();
            CurrentTool = tool;
        }

        private void TerrainLeftButtonPressed(TerrainPointerInfo info)
        {
            CurrentTool?.TerrainLeftButtonPressed(info);
        }

        private void TerrainLeftButtonReleased(TerrainPointerInfo info)
        {
            CurrentTool?.TerrainLeftButtonReleased(info);
        }
        
        private void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            CurrentTool?.TerrainLeftButtonDragged(startInfo, endInfo);
        }

        private void TerrainRightButtonPressed(TerrainPointerInfo info)
        {
            CurrentTool?.TerrainLeftButtonPressed(info);
        }

        private void TerrainRightButtonReleased(TerrainPointerInfo info)
        {
            CurrentTool?.TerrainRightButtonReleased(info);
        }
        
        private void TerrainRightButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            CurrentTool?.TerrainRightButtonDragged(startInfo, endInfo);
        }

        private void ItemLeftButtonClicked(ItemInstance item)
        {
            CurrentTool?.ItemLeftButtonClicked(item);
        }

        private void ItemRightButtonClicked(ItemInstance item)
        {
            CurrentTool?.ItemRightButtonClicked(item);
        }
    }
}
