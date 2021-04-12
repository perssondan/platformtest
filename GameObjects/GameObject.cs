using Microsoft.Graphics.Canvas;
using System;
using uwpKarate.Components;

namespace uwpKarate.GameObjects
{
    public class GameObject
    {
        public GameObject()
        {
            TransformComponent = new TransformComponent();
        }

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
            InputComponent?.Update(timeSpan);
            PhysicsComponent?.Update(world, timeSpan);
            TransformComponent?.Update(world, timeSpan);
        }

        public GraphicsComponent GraphicsComponent { get; set; }
        public PhysicsComponent PhysicsComponent { get; set; }
        public InputComponent InputComponent { get; set; }
        public TransformComponent TransformComponent { get; }
    }
}