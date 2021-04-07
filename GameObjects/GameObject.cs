using Microsoft.Graphics.Canvas;
using System;
using uwpKarate.Components;

namespace uwpKarate.GameObjects
{
    public class GameObject
    {
        public GameObject(GraphicsComponent graphicsComponent,
                          PhysicsComponent physicsComponent,
                          InputComponent inputComponent,
                          TransformComponent transformComponent)
        {
            GraphicsComponent = graphicsComponent;
            PhysicsComponent = physicsComponent;
            InputComponent = inputComponent;
            TransformComponent = transformComponent ?? new TransformComponent();
        }

        public void Update(World world, TimeSpan timeSpan)
        {
            InputComponent?.Update();
            PhysicsComponent?.Update(world);
        }

        public void Draw(CanvasDrawingSession canvasDrawingSession)
        {
            GraphicsComponent?.Update(canvasDrawingSession);
        }

        public GraphicsComponent GraphicsComponent { get; set; }
        public PhysicsComponent PhysicsComponent { get; set; }
        public InputComponent InputComponent { get; set; }
        public TransformComponent TransformComponent { get; }
    }
}