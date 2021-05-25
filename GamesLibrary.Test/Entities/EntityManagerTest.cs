using FakeItEasy;
using GamesLibrary.Components;
using GamesLibrary.Entities;
using GamesLibrary.Models;
using GamesLibrary.Systems;
using NUnit.Framework;

namespace GamesLibrary.Test.Entities
{
    public class EntityManagerTest
    {
        private IEntityManager _componentManager;
        private IEventSystem _eventSystem;

        [SetUp]
        public void Setup()
        {
            _eventSystem = A.Fake<IEventSystem>();
            _componentManager = new EntityManager(_eventSystem);
        }

        [Test]
        public void AddEntity()
        {
            var entity = _componentManager.CreateEntity();

            A.CallTo(() => _eventSystem.Send(A<object>._, A<EntityAdded>._))
                .MustHaveHappenedOnceExactly();

            Assert.That(_componentManager.GetEntity(entity.Id), Is.EqualTo(entity));
        }

        [Test]
        public void RemoveEntity()
        {
            var entity = _componentManager.CreateEntity();

            _componentManager.Remove(entity);

            A.CallTo(() => _eventSystem.Send(A<object>._, A<EntityRemoved>._))
                .MustHaveHappenedOnceExactly();

            Assert.That(_componentManager.GetEntity(entity.Id), Is.Not.EqualTo(entity));
        }

        [Test]
        public void AddComponent()
        {
            var entity = _componentManager.CreateEntity();

            var component = A.Fake<IComponent>();
            A.CallTo(() => component.Entity).Returns(entity);

            // TODO: Possibly simplify add component, since component should contain the id of the entity
            // TODO: Need to resolve if we force the entity id of the component to match the entity
            // TODO: Protection against adding the same component twice to two different entities?
            _componentManager.AddComponent(entity, component);

            A.CallTo(() => _eventSystem.Send(A<object>._, A<ComponentAdded>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddSameComponentTypeTwiceTriggerEventOnce()
        {
            var entity = _componentManager.CreateEntity();

            var componentOne = A.Fake<IComponent>();
            A.CallTo(() => componentOne.Entity).Returns(entity);

            var componentTwo = A.Fake<IComponent>();
            A.CallTo(() => componentTwo.Entity).Returns(entity);

            // TODO: Should we replace the component if it's the same type?
            _componentManager.AddComponent(entity, componentOne);
            _componentManager.AddComponent(entity, componentTwo);

            A.CallTo(() => _eventSystem.Send(A<object>._, A<ComponentAdded>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RemoveComponent()
        {
            var entity = _componentManager.CreateEntity();

            var component = A.Fake<IComponent>();
            A.CallTo(() => component.Entity).Returns(entity);

            _componentManager.AddComponent(entity, component);

            _componentManager.RemoveComponent(entity, component);

            A.CallTo(() => _eventSystem.Send(A<object>._, A<ComponentRemoved>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void EntitesAddedWithTagIsReturned()
        {
            var tag = "42";
            var firstEntity = _componentManager.CreateEntity(tag);
            var secondEntity = _componentManager.CreateEntity(tag);

            var taggedEntities = _componentManager.GetEntities(tag);

            Assert.That(taggedEntities, Contains.Item(firstEntity).And.Contains(secondEntity));

        }
    }
}
