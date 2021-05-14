using System.Numerics;
using uwpKarate.GameObjects;
using Windows.UI;

namespace uwpKarate.Components
{
    public class ShapeGraphicsComponent : GameObjectComponent, IComponent
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
            ShapeGraphicsComponentManager.Instance.AddComponent(this);
        }

        public ShapeType ShapeType { get; set; }
        public Color Color { get; set; }
        public Vector2 Size { get; set; }

        protected override void OnDispose()
        {
            ShapeGraphicsComponentManager.Instance.RemoveComponent(this);
        }
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
