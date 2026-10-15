using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Terraria;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_DoesNotTriggerObjectiveBehaviorPersistenceNetworkOrRewardClaims()
	{
		var reward = new Item { type = 1, stack = 1 };
		var objective = new StubObjective { ProgressValue = 0.6f };
		var quest = new StubQuest
		{
			ProgressValue = 0.6f,
			TimeLimitValue = 300,
		};
		quest.SetState(WorldQuestState.Active);
		quest.SetTime(90);
		quest.Objectives.Add(objective);
		quest.SetRewards(reward);
		SetWhoAmI(quest, 19);

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(WorldQuestState.Active, quest.State);
		Assert.AreEqual(90, quest.Time);
		Assert.AreEqual(19, quest.WhoAmI);
		Assert.IsEmpty(quest.RewardClaimedPlayers);
		Assert.IsFalse(objective.Completed);
		Assert.AreEqual(0, objective.CheckCompletionCalls);
		Assert.AreEqual(0, objective.UpdateCalls);
		Assert.AreEqual(0, objective.CompleteCalls);
		Assert.AreEqual(0, objective.ActivateCalls);
		Assert.AreEqual(0, objective.DeactivateCalls);
		Assert.AreEqual(0, objective.ResetCalls);
		Assert.AreEqual(0, objective.PersistenceCalls);
		Assert.AreEqual(0, objective.NetworkCalls);
		Assert.HasCount(1, view.Rewards);
		Assert.AreSame(reward, view.Rewards[0].Item);
	}
}
