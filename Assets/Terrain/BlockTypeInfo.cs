
namespace Terrain
{
    public struct BlockFaceInfo
    {
        public float uMin, uMax, vMin, vMax;
    }

    public struct BlockTypeInfo
    {
        public BlockFaceInfo upFace;
        public BlockFaceInfo downFace;
        public BlockFaceInfo leftFace;
        public BlockFaceInfo rightFace;
        public BlockFaceInfo forwardFace;
        public BlockFaceInfo backFace;
        public bool fullBlock;
        public bool customMesh;
        public int meshIndex;

        public BlockFaceInfo GetFace(Direction direction) => direction switch
        {
            Direction.Up => upFace,
            Direction.Down => downFace,
            Direction.Left => leftFace,
            Direction.Right => rightFace,
            Direction.Forward => forwardFace,
            Direction.Back => backFace,
            _ => upFace,
        };
            
        public BlockFaceInfo GetFace(Direction direction, Rotation rotation)
        {
            Direction rotatedDirection = RotationUtility.Rotate(direction, rotation);
            return rotatedDirection switch
            {
                Direction.Up => upFace,
                Direction.Down => downFace,
                Direction.Left => leftFace,
                Direction.Right => rightFace,
                Direction.Forward => forwardFace,
                Direction.Back => backFace,
                _ => upFace,
            };
        }
    }
}
