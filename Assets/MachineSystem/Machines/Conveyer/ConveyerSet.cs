using System;
using Terrain;
using UnityEngine;

namespace MachineSystem.Machines.Conveyer
{
    [CreateAssetMenu(fileName = "ConveyerSet", menuName = "Scriptable Objects/Machines/Conveyer Set")]
    public class ConveyerSet : ScriptableObject
    {
        [SerializeField] private ConveyerType straightConveyerType;
        [SerializeField] private ConveyerType rightTurnConveyerType;
        [SerializeField] private ConveyerType leftTurnConveyerType;

        public (ConveyerType, Rotation) GetConveyer(Direction inputDirection, Direction outputDirection)
        {
            Rotation angle = RotationUtility.GetAngle(inputDirection, outputDirection);

            return angle switch
            {
                // Straight Conveyer in input and output are the same
                Rotation.Degrees0 => (straightConveyerType, RotationUtility.GetRotation(inputDirection)),
                Rotation.Degrees90 => (leftTurnConveyerType, RotationUtility.GetRotation(inputDirection)),
                Rotation.Degrees180 => throw new ArgumentException("Conveyer Belt cannot turn 180 degrees"),
                Rotation.Degrees270 => (rightTurnConveyerType, RotationUtility.GetRotation(inputDirection)),
                _ => throw new ArgumentException()
            };
        }
    }
}
