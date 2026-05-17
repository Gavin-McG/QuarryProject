using System;
using GameTools.Tools;
using ItemSystem;
using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools
{
    public class ToolManager : MonoBehaviour
    {
        [SerializeField] private PhysicsRaycaster raycaster;
        
        public static GameTool CurrentTool { get; private set; } = null;
        
        private TerrainManager terrainManager;
        

        private void Update()
        {
            CurrentTool?.Update();
        }

        public void SetTool(GameTool tool)
        {
            CurrentTool?.Deselect();
            CurrentTool = tool;
            raycaster.eventMask = CurrentTool.interactionLayerMask;
            tool.Select();
        }
    }
}
