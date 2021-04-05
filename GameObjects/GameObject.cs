using Microsoft.Graphics.Canvas;
using uwpKarate.Components;

namespace uwpKarate.GameObjects
{
    public class GameObject
    {
        public GameObject(GraphicsComponent graphicsComponent,
                          PhysicsComponent physicsComponent,
                          InputComponent inputComponent)
        {
            GraphicsComponent = graphicsComponent;
            PhysicsComponent = physicsComponent;
            InputComponent = inputComponent;
        }

        public void Update(World world, CanvasDrawingSession canvasDrawingSession)
        {
            InputComponent?.Update(this);
            PhysicsComponent?.Update(this, world);
            GraphicsComponent?.Update(this, canvasDrawingSession);
        }

        public int XPos { get; set; }
        public int YPos { get; set; }
        public int Velocity { get; set; }
        public GraphicsComponent GraphicsComponent { get; set; }
        public PhysicsComponent PhysicsComponent { get; set; }
        public InputComponent InputComponent { get; set; }
    }
}