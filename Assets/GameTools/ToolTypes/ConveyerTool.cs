using MachineSystem;
using MachineSystem.Machines.Conveyer;
using ManagerSystem;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameTools.Tools
{
    [CreateAssetMenu(fileName = "Conveyer Tool", menuName = "Scriptable Objects/Tools/Conveyer Tool")]
    public class ConveyerTool : GameTool
    {
        private enum AxisMode { XAxis, ZAxis }
        
        [SerializeField] private Sprite toolSprite;
        [SerializeField] private ConveyerSet conveyerSet;
        [SerializeField] private PointerEventData.InputButton button = PointerEventData.InputButton.Left;
        [SerializeField] private InputActionReference swapAxisAction;
        [SerializeField] private GameObject beltPrefab;
        [SerializeField] private GameObject cornerPrefab;
        
        public override Sprite Sprite => toolSprite;
        
        private MachineManager machineManager;
        private TerrainDragTracker dragTracker;
        private AxisMode axisMode;
        private GameObject beltOutline1;
        private GameObject beltOutline2;
        private GameObject cornerOutline;

        public override void Select()
        {
            base.Select();
            
            machineManager = Managers.GetManager<MachineManager>();
            dragTracker = new TerrainDragTracker();
            
            swapAxisAction.action.Enable();
            
            beltOutline1 = Instantiate(beltPrefab);
            beltOutline2 = Instantiate(beltPrefab);
            cornerOutline = Instantiate(cornerPrefab);
            beltOutline1.SetActive(false);
            beltOutline2.SetActive(false);
            cornerOutline.SetActive(false);
            
            TerrainManager.onPointerDown.AddListener(TerrainDown);
            TerrainManager.onPointerUp.AddListener(TerrainUp);
            TerrainManager.onPointerEnter.AddListener(TerrainEnter);
            TerrainManager.onPointerExit.AddListener(TerrainExit);

            swapAxisAction.action.performed += SwapAxisMode;
        }

        public override void Deselect()
        {
            base.Deselect();
            
            machineManager = null;
            dragTracker = null;
            
            swapAxisAction.action.Disable();
            
            Destroy(beltOutline1);
            Destroy(beltOutline2);
            Destroy(cornerOutline);
            
            TerrainManager.onPointerDown.RemoveListener(TerrainDown);
            TerrainManager.onPointerUp.RemoveListener(TerrainUp);
            TerrainManager.onPointerEnter.RemoveListener(TerrainEnter);
            TerrainManager.onPointerExit.RemoveListener(TerrainExit);
            
            swapAxisAction.action.performed -= SwapAxisMode;
        }

        public override void Update()
        {
            beltOutline1.SetActive(false);
            beltOutline2.SetActive(false);
            cornerOutline.SetActive(false);
            if (!dragTracker.IsDragging()) return;

            var dragInfo = dragTracker.GetDragInfo();

            Vector3Int startPosition = dragInfo.start.FrontPosition;
            Vector3Int endPosition = dragInfo.end.FrontPosition;

            // Straight line along Z
            if (startPosition.x == endPosition.x)
            {
                beltOutline1.SetActive(true);

                int minZ = Mathf.Min(startPosition.z, endPosition.z);
                int maxZ = Mathf.Max(startPosition.z, endPosition.z);

                Vector3 min = new Vector3(startPosition.x, startPosition.y, minZ);
                Vector3 max = new Vector3(startPosition.x, startPosition.y, maxZ);

                beltOutline1.transform.position =
                    (min + max + Vector3.one) * 0.5f;

                beltOutline1.transform.localScale =
                    new Vector3(1.02f, 1.02f, (maxZ - minZ + 1) + 0.02f);

                beltOutline1.transform.rotation = Quaternion.identity;

                return;
            }

            // Straight line along X
            if (startPosition.z == endPosition.z)
            {
                beltOutline1.SetActive(true);

                int minX = Mathf.Min(startPosition.x, endPosition.x);
                int maxX = Mathf.Max(startPosition.x, endPosition.x);

                Vector3 min = new Vector3(minX, startPosition.y, startPosition.z);
                Vector3 max = new Vector3(maxX, startPosition.y, startPosition.z);

                beltOutline1.transform.position =
                    (min + max + Vector3.one) * 0.5f;

                beltOutline1.transform.localScale =
                    new Vector3((maxX - minX + 1) + 0.02f, 1.02f, 1.02f);

                beltOutline1.transform.rotation = Quaternion.identity;

                return;
            }

            // L shaped belt placement
            Vector3Int cornerPosition = axisMode switch
            {
                AxisMode.XAxis => new Vector3Int(endPosition.x, startPosition.y, startPosition.z),
                _ => new Vector3Int(startPosition.x, startPosition.y, endPosition.z),
            };
            
            // X BELT SEGMENT
            
            Direction beltDirectionX = startPosition.x <= endPosition.x ? Direction.Right : Direction.Left;
            {
                // Get direction for corner offset
                Direction CornerOffsetDirection = axisMode switch
                {
                    AxisMode.XAxis => DirectionUtility.Opposite(beltDirectionX),
                    _ => beltDirectionX
                };

                // Get endpoints for X axis belt
                Vector3Int cornerEnd = DirectionUtility.MovePosition(cornerPosition, CornerOffsetDirection);
                Vector3Int dragEnd = axisMode switch
                {
                    AxisMode.XAxis => startPosition,
                    _ => endPosition
                };

                // Place preview
                int minX = Mathf.Min(cornerEnd.x, dragEnd.x);
                int maxX = Mathf.Max(cornerEnd.x, dragEnd.x);
                
                Vector3 min = new Vector3(minX, cornerEnd.y, cornerEnd.z);
                Vector3 max = new Vector3(maxX, cornerEnd.y, cornerEnd.z);
                
                beltOutline1.transform.position = (min + max + Vector3.one) * 0.5f;
                beltOutline1.transform.localScale = new Vector3((maxX - minX + 1) + 0.02f, 1.02f, 1.02f);
                beltOutline1.transform.rotation = Quaternion.identity;
                beltOutline1.SetActive(true);
            }

            // Z BELT SEGMENT
            
            Direction beltDirectionZ = startPosition.z <= endPosition.z ? Direction.Forward : Direction.Back;
            {
                // Get direction for corner offset
                Direction CornerOffsetDirection = axisMode switch
                {
                    AxisMode.ZAxis => DirectionUtility.Opposite(beltDirectionZ),
                    _ => beltDirectionZ
                };

                // Get endpoints for X axis belt
                Vector3Int cornerEnd = DirectionUtility.MovePosition(cornerPosition, CornerOffsetDirection);
                Vector3Int dragEnd = axisMode switch
                {
                    AxisMode.ZAxis => startPosition,
                    _ => endPosition
                };

                // Place preview
                int minZ = Mathf.Min(cornerEnd.z, dragEnd.z);
                int maxZ = Mathf.Max(cornerEnd.z, dragEnd.z);
                
                Vector3 min = new Vector3(cornerEnd.x, cornerEnd.y, minZ);
                Vector3 max = new Vector3(cornerEnd.x, cornerEnd.y, maxZ);
                
                beltOutline2.transform.position = (min + max + Vector3.one) * 0.5f;
                beltOutline2.transform.localScale = new Vector3(1.02f, 1.02f, (maxZ - minZ + 1) + 0.02f);
                beltOutline2.transform.rotation = Quaternion.identity;
                beltOutline2.SetActive(true);
            }

            // CORNER
            
            // Match corner conveyor rotation
            (_, Rotation rotation) = conveyerSet.GetConveyer(beltDirectionX, beltDirectionZ);

            cornerOutline.transform.position = cornerPosition + Vector3.one * 0.5f;
            cornerOutline.transform.localScale = Vector3.one * 1.02f;
            cornerOutline.transform.rotation = RotationUtility.GetQuaternion(rotation);
            cornerOutline.SetActive(true);
        }

        private void PlaceBelts()
        {
            var dragInfo = dragTracker.GetDragInfo();
            Vector3Int startPosition = dragInfo.start.FrontPosition;
            Vector3Int endPosition = dragInfo.end.FrontPosition;
        
            // Single belt line in Z axis
            if (startPosition.x == endPosition.x)
            {
                Direction direction = startPosition.z <= endPosition.z ? Direction.Forward : Direction.Back;
                PlaceBeltLine(startPosition, endPosition, direction);
                return;
            }
            
            // Single belt line in X axis
            if (startPosition.z == endPosition.z)
            {
                Direction direction = startPosition.x <= endPosition.x ? Direction.Right : Direction.Left;
                PlaceBeltLine(startPosition, endPosition, direction);
                return;
            }
            
            // L shaped belt placement
            Vector3Int cornerPosition = axisMode switch
            {
                AxisMode.XAxis => new Vector3Int(endPosition.x, startPosition.y, startPosition.z),
                _ => new Vector3Int(startPosition.x, startPosition.y, endPosition.z),
            };
            
            // Place X axis belt
            Direction beltDirectionX = startPosition.x <= endPosition.x ? Direction.Right : Direction.Left;
            {
                // Get direction for corner offset
                Direction CornerOffsetDirection = axisMode switch
                {
                    AxisMode.XAxis => DirectionUtility.Opposite(beltDirectionX),
                    _ => beltDirectionX
                };

                // Get endpoints for X axis belt
                Vector3Int cornerEnd = DirectionUtility.MovePosition(cornerPosition, CornerOffsetDirection);
                Vector3Int dragEnd = axisMode switch
                {
                    AxisMode.XAxis => startPosition,
                    _ => endPosition
                };
                
                // Place belts between Endpoints
                PlaceBeltLine(cornerEnd, dragEnd, beltDirectionX);
            }
            
            // Place Z axis belt
            Direction beltDirectionZ = startPosition.z <= endPosition.z ? Direction.Forward : Direction.Back;
            {
                // Get direction for corner offset
                Direction CornerOffsetDirection = axisMode switch
                {
                    AxisMode.ZAxis => DirectionUtility.Opposite(beltDirectionZ),
                    _ => beltDirectionZ
                };

                // Get endpoints for X axis belt
                Vector3Int cornerEnd = DirectionUtility.MovePosition(cornerPosition, CornerOffsetDirection);
                Vector3Int dragEnd = axisMode switch
                {
                    AxisMode.ZAxis => startPosition,
                    _ => endPosition
                };

                // Place belts between Endpoints
                PlaceBeltLine(cornerEnd, dragEnd, beltDirectionZ);
            }
            
            // Place Corner conveyer
            (ConveyerType type, Rotation rotation) = axisMode switch
            {
                AxisMode.XAxis => conveyerSet.GetConveyer(beltDirectionX, beltDirectionZ),
                _ => conveyerSet.GetConveyer(beltDirectionZ, beltDirectionX),
            };
            machineManager.PlaceMachine(type, cornerPosition, rotation);
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
                    machineManager.PlaceMachine(type, position, rotation);
                }
            }
            else if (direction == Direction.Forward || direction == Direction.Back)
            {
                int startZ = Mathf.Min(startPosition.z, endPosition.z);
                int endZ = Mathf.Max(startPosition.z, endPosition.z);
                for (int z = startZ; z <= endZ; z++)
                {
                    Vector3Int position = new Vector3Int(startPosition.x, startPosition.y, z);
                    machineManager.PlaceMachine(type, position, rotation);
                }
            }
        }
        
        private void TerrainDown(PointerEventData eventData)
        {
            if (eventData.button != button) return;
            dragTracker.OnPointerDown(eventData);
            axisMode = AxisMode.XAxis;
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

        private void SwapAxisMode(InputAction.CallbackContext context)
        {
            axisMode = axisMode switch
            {
                AxisMode.XAxis => AxisMode.ZAxis,
                _ => AxisMode.XAxis
            };
        }
    }
}
