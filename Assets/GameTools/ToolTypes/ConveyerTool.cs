using MachineSystem;
using MachineSystem.Machines.Conveyer;
using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Conveyer Tool", menuName = "Scriptable Objects/Tools/Conveyer Tool")]
    public class ConveyerTool : GameTool
    {
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private ConveyerSet conveyerSet;
        [SerializeField] private PointerEventData.InputButton button = PointerEventData.InputButton.Left;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;
        private TerrainDragTracker dragTracker;

        public override void Select()
        {
            base.Select();
            
            machineManager = Managers.GetManager<MachineManager>();
            dragTracker = new TerrainDragTracker();
            
            TerrainManager.onPointerDown.AddListener(TerrainDown);
            TerrainManager.onPointerUp.AddListener(TerrainUp);
            TerrainManager.onPointerEnter.AddListener(TerrainEnter);
            TerrainManager.onPointerExit.AddListener(TerrainExit);
        }

        public override void Deselect()
        {
            base.Deselect();
            
            machineManager = null;
            dragTracker = null;
            
            TerrainManager.onPointerDown.RemoveListener(TerrainDown);
            TerrainManager.onPointerUp.RemoveListener(TerrainUp);
            TerrainManager.onPointerEnter.RemoveListener(TerrainEnter);
            TerrainManager.onPointerExit.RemoveListener(TerrainExit);
        }
        
        private void PlaceBelts()
        {
            var dragInfo = dragTracker.GetDragInfo();
            Vector3Int startPosition = dragInfo.start.FrontPosition;
            Vector3Int endPosition = dragInfo.end.FrontPosition;
        
            if (startPosition.x == endPosition.x)
            {
                // Single belt line in Z axis
                Direction direction = startPosition.z <= endPosition.z ? Direction.Forward : Direction.Back;
                PlaceBeltLine(startPosition, endPosition, direction);
            }
            else if (startPosition.z == endPosition.z)
            {
                // Single belt line in X axis
                Direction direction = startPosition.x <= endPosition.x ? Direction.Right : Direction.Left;
                PlaceBeltLine(startPosition, endPosition, direction);
            }
            else
            {
                Vector3Int cornerPosition = new Vector3Int(endPosition.x, startPosition.y, startPosition.z);
        
                // Place X axis belt
                Direction directionX = startPosition.x <= endPosition.x ? Direction.Right : Direction.Left;
                Vector3Int beltEndPositionX = DirectionUtility.MovePosition(cornerPosition, DirectionUtility.Opposite(directionX));
                PlaceBeltLine(startPosition, beltEndPositionX, directionX);
                
                // Place Z axis belt
                Direction directionZ = startPosition.z <= endPosition.z ? Direction.Forward : Direction.Back;
                Vector3Int beltStartPositionZ = DirectionUtility.MovePosition(cornerPosition, directionZ);
                PlaceBeltLine(beltStartPositionZ, endPosition, directionZ);
                
                // Place Corner conveyer
                (ConveyerType type, Rotation rotation) = conveyerSet.GetConveyer(directionX, directionZ);
                machineManager.PlaceMachine(cornerPosition, type, rotation);
            }
        }
        
        private void PlaceBeltLine(Vector3Int startPosition, Vector3Int endPosition, Direction direction)
        {
            (ConveyerType type, Rotation rotation) = conveyerSet.GetConveyer(direction, direction);
        
            if (direction == Direction.Right || direction == Direction.Left)
            {
                int startX = Mathf.Min(startPosition.x, endPosition.x);
                int endX = Mathf.Max(startPosition.x, endPosition.x);
                for (int x = startX; x <= endX; x++)
                {
                    Vector3Int position = new Vector3Int(x, startPosition.y, startPosition.z);
                    machineManager.PlaceMachine(position, type, rotation);
                }
            }
            else if (direction == Direction.Forward || direction == Direction.Back)
            {
                int startZ = Mathf.Min(startPosition.z, endPosition.z);
                int endZ = Mathf.Max(startPosition.z, endPosition.z);
                for (int z = startZ; z <= endZ; z++)
                {
                    Vector3Int position = new Vector3Int(startPosition.x, startPosition.y, z);
                    machineManager.PlaceMachine(position, type, rotation);
                }
            }
        }
        
        private void TerrainDown(PointerEventData eventData)
        {
            if (eventData.button != button) return;
            dragTracker.OnPointerDown(eventData);
        }

        private void TerrainUp(PointerEventData eventData)
        {
            if (eventData.button != button) return;

            PlaceBelts();
            
            dragTracker.OnPointerUp(eventData);
        }

        private void TerrainEnter(PointerEventData eventData)
        {
            dragTracker.OnPointerEnter(eventData);
        }

        private void TerrainExit(PointerEventData eventData)
        {
            dragTracker.OnPointerExit(eventData);
        }
    }
}
