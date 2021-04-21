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
            OnBeforeUpdate(world, timeSpan);

            InputComponent?.Update(world, timeSpan);
            PhysicsComponent?.Update(world, timeSpan);
            // Must be last
            TransformComponent?.Update(world, timeSpan);

            OnAfterUpdate(world, timeSpan);
        }

        public virtual void OnBeforeUpdate(World world, TimeSpan timeSpan)
        {
        }

        public virtual void OnAfterUpdate(World world, TimeSpan timeSpan)
        {
        }

        public GraphicsComponent GraphicsComponent { get; set; }
        public PhysicsComponent PhysicsComponent { get; set; }
        public InputComponent InputComponent { get; set; }
        public TransformComponent TransformComponent { get; }
    }
}