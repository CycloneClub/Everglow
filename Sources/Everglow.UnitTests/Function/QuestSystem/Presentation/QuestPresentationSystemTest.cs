using System.Reflection;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestPresentationSystemTest
{
	[TestMethod]
	public void PendingEvents_AreDistinctAndReentrantEventsWaitForNextUpdate()
	{
		const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;
		const BindingFlags NonPublicNested = BindingFlags.NonPublic;
		var system = new QuestPresentationSystem();
		var identity = new QuestIdentity(QuestSide.Player, "TestQuest", "instance");
		Type eventType = typeof(QuestPresentationSystem).GetNestedType("QuestEventType", NonPublicNested);
		MethodInfo queueEvent = typeof(QuestPresentationSystem).GetMethod("QueueEvent", NonPublicInstance);
		Assert.IsNotNull(eventType);
		Assert.IsNotNull(queueEvent);
		object statusUpdated = Enum.Parse(eventType, "StatusUpdated");
		int publishCount = 0;
		system.QuestStatusUpdated += publishedIdentity =>
		{
			Assert.AreEqual(identity, publishedIdentity);
			publishCount++;
			if (publishCount == 1)
			{
				queueEvent.Invoke(system, [statusUpdated, identity]);
			}
		};

		queueEvent.Invoke(system, [statusUpdated, identity]);
		queueEvent.Invoke(system, [statusUpdated, identity]);
		system.PostUpdateEverything();

		Assert.AreEqual(1, publishCount);

		system.PostUpdateEverything();

		Assert.AreEqual(2, publishCount);
	}

}
