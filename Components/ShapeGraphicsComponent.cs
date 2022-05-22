using System.Numerics;
using uwpPlatformer.GameObjects;
using Windows.UI;

namespace uwpPlatformer.Components
{
    public class ShapeGraphicsComponent : ComponentBase, IComponent
    {
        public ShapeGraphicsComponent(GameObject gameObject, ShapeType shapeType = ShapeType.Circle)
            : this(gameObject, shapeType, Colors.White, new Vector2(3f, 3f))
        {
        }

        public ShapeGraphicsComponent(GameObject gameObject, ShapeType shapeType, Color color, Vector2 size)
            : base(gameObject)
        {
            ShapeType = shapeType;
            Color = color;
            Size = size;
        }

        public ShapeType ShapeType { get; set; }
        public Color Color { get; set; }
        public Vector2 Size { get; set; }

        public bool IsVisible { get; set; } = true;
    }

    public enum ShapeType
    {
        None = 0,
        Rectangle = 1,
        Ellipse = 2,
        Square = 4,
        Circle = 8,
    }
}
