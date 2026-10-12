#if DEBUG
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

namespace Everglow.Example.Quest;

public class ExampleQuestPlayer : ModPlayer
{
	private bool registerSamples;

	public override void OnEnterWorld()
	{
		registerSamples = true;
	}

	public override void PostUpdate()
	{
		if (!registerSamples)
		{
			return;
		}
		registerSamples = false;

		// All OnEnterWorld hooks have finished restoring the player's quest data.
		if (!Main.dedServ && Player.whoAmI == Main.myPlayer && PlayerQuestManager.Instance.Quests.Count == 0)
		{
			PlayerQuestManager.Instance.AddQuest(new KillNPCQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new ParallelQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new QuestObjectivesTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new CancellableKillNPCQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new OpenPanelQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new BranchingQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest1(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest2(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest3(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest4(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest5(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new NoneQuest6(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new QuestTimerTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new QuestIconTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new GiveItemQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new ExploreQuestTest(), PlayerQuestState.Available);
			PlayerQuestManager.Instance.AddQuest(new CancellableKillNPCQuestTest(), PlayerQuestState.Available);
		}
	}
}
#endif
