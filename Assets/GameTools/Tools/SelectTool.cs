using ClickManager;
using ItemSystem;
using MachineSystem;
using ManagerSystem;
using Terrain;
using UnityEngine;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Select Tool", menuName = "Scriptable Objects/Tools/Select Tool")]
    public class SelectTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private GameObject selectionPrefab;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;
        private TerrainManager terrainManager;

        private GameObject selectionObject;

        public override void Select()
        {
            base.Select();
            machineManager = Managers.GetManager<MachineManager>();
            terrainManager = Managers.GetManager<TerrainManager>();
            
            selectionObject = Instantiate(selectionPrefab);
            selectionObject.SetActive(false);
        }

        public override void Deselect()
        {
            base.Deselect();
            Destroy(selectionObject);
        }

        public override void Update()
        {
            base.Update();
            IClickReceiver currentFocus = ClickRaycast.CurrentHover;

            if (currentFocus is ISelectable selectable)
            {
                Bounds bounds = selectable.GetSelectionRect(ClickRaycast.CurrentHit);
                bounds.min -= Vector3.one * 0.01f;
                bounds.max += Vector3.one * 0.01f;
                
                Vector3 center = bounds.center;
                Vector3 scale = bounds.size;
                selectionObject.SetActive(true);
                selectionObject.transform.position = center;
                selectionObject.transform.localScale = scale;
            }
            else
            {
                selectionObject.SetActive(false);
            }
        }

        public override void TerrainLeftButtonDragged(TerrainPointerInfo startInfo, TerrainPointerInfo endInfo)
        {
            base.TerrainLeftButtonDragged(startInfo, endInfo);

            // Ignore drags unless on same block
            if (startInfo.BackPosition != endInfo.BackPosition) return;

            // Check if block clicked is a machine
            var machine = machineManager.GetMachine(startInfo.FrontPosition);
            if (machine != null)
            {
                Debug.Log("Clicked Machine: " + machine.name);
                return;
            }

            // Click normal block
            var blockType = terrainManager.GetBlock(startInfo.position);
            if (blockType != null)
            {
                Debug.Log("Clicked Block: " + blockType.blockName);
                return;
            }
        }

        public override void ItemLeftButtonClicked(ItemInstance item)
        {
            base.ItemLeftButtonClicked(item);
            
            Debug.Log("Clicked Item: " + item.item);
        }
    }
}