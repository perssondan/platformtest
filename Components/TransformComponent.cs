using System.Numerics;

namespace uwpKarate.Components
{
    public class TransformComponent
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Velocity { get; set; }

        public Vector2 Vector => new Vector2(XPos, YPos);
    }
}