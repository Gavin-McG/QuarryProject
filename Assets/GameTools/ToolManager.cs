using System;
using GameTools.Tools;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace GameTools
{
    public class ToolManager : MonoBehaviour
    {
        public static GameTool CurrentTool { get; private set; }
        
        private TerrainManager terrainManager;

        private void OnEnable()
        {
            TerrainManager.TerrainPressedLeft.AddListener(TerrainPressedLeft);
            TerrainManager.TerrainReleasedLeft.AddListener(TerrainReleasedLeft);
            TerrainManager.TerrainPressedRight.AddListener(TerrainPressedRight);
            TerrainManager.TerrainReleasedRight.AddListener(TerrainReleasedRight);
        }

        private void OnDisable()
        {
            TerrainManager.TerrainPressedLeft.RemoveListener(TerrainPressedLeft);
            TerrainManager.TerrainReleasedLeft.RemoveListener(TerrainReleasedLeft);
            TerrainManager.TerrainPressedRight.RemoveListener(TerrainPressedRight);
            TerrainManager.TerrainReleasedRight.RemoveListener(TerrainReleasedRight);
        }

        public static void SetTool(GameTool tool)
        {
            CurrentTool?.Deselect();
            tool.Select();
            CurrentTool = tool;
        }

        private void TerrainPressedLeft(TerrainHoverInfo info)
        {
            CurrentTool?.PressLeft(info);
        }

        private void TerrainReleasedLeft(TerrainHoverInfo info)
        {
            CurrentTool?.ReleaseLeft(info);
        }

        private void TerrainPressedRight(TerrainHoverInfo info)
        {
            CurrentTool?.PressRight(info);
        }

        private void TerrainReleasedRight(TerrainHoverInfo info)
        {
            CurrentTool?.ReleaseRight(info);
        }
    }
}
