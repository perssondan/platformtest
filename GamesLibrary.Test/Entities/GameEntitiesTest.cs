using FakeItEasy;
using GamesLibrary.Components;
using GamesLibrary.Entities;
using GamesLibrary.Models;
using GamesLibrary.Systems;
using NUnit.Framework;

namespace GamesLibrary.Test.Entities
{
    public class GameEntitiesTest
    {
        private IEntities _entities;
        private IEventSystem _eventSystem;

        [SetUp]
        public void Setup()
        {
            _eventSystem = A.Fake<IEventSystem>();
            _entities = new GameEntities(_eventSystem);
        }

        [Test]
        public void AddEntity()
        {
            var entity = Entity.CreateEntity();
            _entities.Add(entity);

            A.CallTo(() => _eventSystem.Send<EntityAdded>(A<object>._, A<EntityAdded>._))
                .MustHaveHappenedOnceExactly();

            Assert.That(_entities.GetEntity(entity.Id), Is.EqualTo(entity));
        }

        [Test]
        public void RemoveEntity()
        {
            var entity = Entity.CreateEntity();
            _entities.Add(entity);

            _entities.Remove(entity);

            A.CallTo(() => _eventSystem.Send<EntityRemoved>(A<object>._, A<EntityRemoved>._))
                .MustHaveHappenedOnceExactly();

            Assert.That(_entities.GetEntity(entity.Id), Is.Not.EqualTo(entity));
        }

        [Test]
        public void AddComponent()
        {
            var entity = Entity.CreateEntity();
            _entities.Add(entity);

            var component = A.Fake<IComponent>();
            A.CallTo(() => component.Entity).Returns(entity);

            // TODO: Possibly simplify add component, since component should contain the id of the entity
            // TODO: Need to resolve if we force the entity id of the component to match the entity
            // TODO: Protection against adding the same component twice to two different entities?
            _entities.AddComponent(entity, component);

            A.CallTo(() => _eventSystem.Send<ComponentAdded>(A<object>._, A<ComponentAdded>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddSameComponentTypeTwiceTriggerEventOnce()
        {
            var entity = Entity.CreateEntity();
            _entities.Add(entity);

            var componentOne = A.Fake<IComponent>();
            A.CallTo(() => componentOne.Entity).Returns(entity);

            var componentTwo = A.Fake<IComponent>();
            A.CallTo(() => componentTwo.Entity).Returns(entity);

            // TODO: Should we replace the component if it's the same type?
            _entities.AddComponent(entity, componentOne);
            _entities.AddComponent(entity, componentTwo);

            A.CallTo(() => _eventSystem.Send<ComponentAdded>(A<object>._, A<ComponentAdded>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RemoveComponent()
        {
            var entity = Entity.CreateEntity();
            _entities.Add(entity);

            var component = A.Fake<IComponent>();
            A.CallTo(() => component.Entity).Returns(entity);

            _entities.AddComponent(entity, component);

            _entities.RemoveComponent(entity, component);

            A.CallTo(() => _eventSystem.Send<ComponentRemoved>(A<object>._, A<ComponentRemoved>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
