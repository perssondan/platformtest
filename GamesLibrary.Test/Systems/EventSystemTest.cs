using FakeItEasy;
using GamesLibrary.Systems;
using NUnit.Framework;
using System;

namespace GamesLibrary.Test.Systems
{
    public class EventSystemTest
    {
        private IEventSystem _eventSystem;

        [SetUp]
        public void Setup()
        {
            _eventSystem = new EventSystem();
        }

        [Test]
        public void SubscriberGetsNotified()
        {
            var fakeSubscriber = A.Fake<Action<object, DateTime>>();

            _eventSystem.Subscribe(this, fakeSubscriber);

            var utcNow = DateTime.UtcNow;
            _eventSystem.Send(this, utcNow);

            A.CallTo(() => fakeSubscriber.Invoke(A<object>.That.IsEqualTo(this), A<DateTime>.That.IsEqualTo(utcNow)))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void UnscribeFromAllDoesNotGetNotified()
        {
            var fakeDateTimeSubscriber = A.Fake<Action<object, DateTime>>();
            var fakeTimeSpanSubscriber = A.Fake<Action<object, TimeSpan>>();

            _eventSystem.Subscribe(this, fakeDateTimeSubscriber);
            _eventSystem.Subscribe(this, fakeTimeSpanSubscriber);

            _eventSystem.Unsubscribe(this);

            var utcNow = DateTime.UtcNow;
            _eventSystem.Send(this, utcNow);

            var answerToTheUltimateQuestionOfLifeTheUniverseAndEverything = TimeSpan.FromMinutes(42);
            _eventSystem.Send(this, answerToTheUltimateQuestionOfLifeTheUniverseAndEverything);

            A.CallTo(() => fakeDateTimeSubscriber.Invoke(A<object>.Ignored, A<DateTime>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() => fakeTimeSpanSubscriber.Invoke(A<object>.Ignored, A<TimeSpan>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void UnscribeFromOneDoesGetNotifiedFromSecond()
        {
            var fakeDateTimeSubscriber = A.Fake<Action<object, DateTime>>();
            var fakeTimeSpanSubscriber = A.Fake<Action<object, TimeSpan>>();

            _eventSystem.Subscribe(this, fakeDateTimeSubscriber);
            _eventSystem.Subscribe(this, fakeTimeSpanSubscriber);

            _eventSystem.Unsubscribe<DateTime>(this);

            var utcNow = DateTime.UtcNow;
            _eventSystem.Send(this, utcNow);

            var answerToTheUltimateQuestionOfLifeTheUniverseAndEverything = TimeSpan.FromMinutes(42);
            _eventSystem.Send(this, answerToTheUltimateQuestionOfLifeTheUniverseAndEverything);

            A.CallTo(() => fakeDateTimeSubscriber.Invoke(A<object>.Ignored, A<DateTime>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(() => fakeTimeSpanSubscriber.Invoke(A<object>.Ignored, A<TimeSpan>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void SubscribingTwiceGetsCalledTwice()
        {
            var fakeDateTimeSubscriber = A.Fake<Action<object, DateTime>>();

            _eventSystem.Subscribe(this, fakeDateTimeSubscriber);
            _eventSystem.Subscribe(this, fakeDateTimeSubscriber);

            var utcNow = DateTime.UtcNow;
            _eventSystem.Send(this, utcNow);

            A.CallTo(() => fakeDateTimeSubscriber.Invoke(A<object>.Ignored, A<DateTime>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }
    }
}
