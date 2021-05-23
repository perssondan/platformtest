using GamesLibrary.Entities;
using NUnit.Framework;
using System.Collections.Generic;

namespace GamesLibrary.Test.Entities
{
    public class EntityTest
    {
        [Test]
        public void IdIsNotChanged()
        {
            var counter = 10;
            var originEntity = new Entity();
            var originId = originEntity.Id;
            while (counter-- > 0)
            {
                var entity = new Entity();
                _ = entity.Id;
            }

            Assert.That(originId, Is.EqualTo(originEntity.Id));
        }

        [Test]
        public void CanBeFoundInAList()
        {
            var originEntity = new Entity();

            var entities = new List<Entity>();
            entities.Add(originEntity);

            var searchedEntity = entities.Find(entity => entity == originEntity);

            Assert.That(searchedEntity, Is.EqualTo(originEntity));
        }

        [Test]
        public void AddWithSameId()
        {
            var firstEntity = new Entity(42);
            var nextEntity = new Entity(42);

            var entities = new List<Entity>();
            entities.Add(firstEntity);
            entities.Add(nextEntity);

            Assert.That(entities, Contains.Item(firstEntity));
            Assert.That(entities, Contains.Item(nextEntity));
            Assert.That(entities.Count, Is.EqualTo(2));
        }

        [Test]
        public void CreateMethodInitializesWithId()
        {
            var firstEntity = Entity.CreateEntity();
            var secondEntity = Entity.CreateEntity();

            Assert.That(firstEntity, Is.Not.EqualTo(secondEntity));
        }

        [Test]
        public void EntitiesInDictionary()
        {
            var originValue = "42";
            var entities = new Dictionary<Entity, string>();

            var entity = Entity.CreateEntity();
            entities.Add(entity, originValue);

            var duplicateEntity = new Entity(entity.Id);

            var valueFromEntity = entities[duplicateEntity];
            var valueFromId = entities[duplicateEntity.Id];

            Assert.That(originValue, Is.EqualTo(valueFromEntity));
            Assert.That(originValue, Is.EqualTo(valueFromId));
        }

        [TestCase(42, 42)]
        [TestCase(0, 0)]
        [TestCase(-42, -42)]
        public void ImplicitConstructorSetsId(int fromId, int expectedId)
        {
            Entity implicitEntity = fromId;
            var expectedEntity = new Entity(expectedId);

            Assert.That(expectedEntity, Is.EqualTo(implicitEntity));
            Assert.That(expectedEntity.Id, Is.EqualTo(implicitEntity.Id));
        }

        [TestCase(42, 42, true)]
        [TestCase(42, 4, false)]
        public void EqualsGivesExpectedResult(int first, int second, bool equals)
        {
            var firstEntity = new Entity(first);
            var secondEntity = new Entity(second);
            Assert.That(firstEntity.Equals(secondEntity) == equals);
            Assert.That(Entity.Equals(firstEntity, secondEntity) == equals);
            Assert.That(firstEntity == secondEntity, Is.EqualTo(equals));
            Assert.That(firstEntity != secondEntity, Is.EqualTo(!equals));
        }

        [TestCase(42, "Id=42")]
        [TestCase(4, "Id=4")]
        public void ToStringReturnsExpectedId(int id, string expectedString)
        {
            var entity = new Entity(id);

            Assert.That(expectedString, Is.EqualTo(entity.ToString()));
            Assert.That(expectedString, Is.EqualTo(entity.ToString(string.Empty, null)));
        }

        [Test]
        public void IsNotEqualToAnyType()
        {
            Assert.That(Entity.Equals(Entity.CreateEntity(), new object()) == false);
        }
    }
}
