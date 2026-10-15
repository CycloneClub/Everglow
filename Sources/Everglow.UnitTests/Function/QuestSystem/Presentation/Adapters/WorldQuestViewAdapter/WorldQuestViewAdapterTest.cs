using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public partial class WorldQuestViewAdapterTest
{
	private sealed class StubQuest : WorldQuestBase
	{
		public string NameValue { get; set; } = "world-definition";

		public string DisplayNameValue { get; set; } = "World Quest";

		public string DescriptionValue { get; set; } = string.Empty;

		public string HintValue { get; set; } = string.Empty;

		public QuestSourceBase SourceValue { get; set; } = QuestSourceBase.Default;

		public QuestType TypeValue { get; set; } = QuestType.None;

		public bool VisibleValue { get; set; } = true;

		public float ProgressValue { get; set; }

		public int TimeLimitValue { get; set; }

		public override string Name => NameValue;

		public override string DisplayName => DisplayNameValue;

		public override string Description => DescriptionValue;

		public override string Hint => HintValue;

		public override QuestSourceBase Source => SourceValue;

		public override QuestType Type => TypeValue;

		public override bool Visible => VisibleValue;

		public override float Progress => ProgressValue;

		public override int TimeLimit => TimeLimitValue;

		public void SetState(WorldQuestState state) => State = state;

		public void SetTime(int time) => Time = time;

		public void SetRewards(params Item[] items) => RewardItems = [.. items];

		public void AddReward(Item item)
		{
			RewardItems ??= [];
			RewardItems.Add(item);
		}
	}

	private sealed class StubObjective : WorldObjectiveBase
	{
		public string DescriptionValue
		{
			set => WithDescription(value);
		}

		public string ObjectiveTextValue { get; set; } = string.Empty;

		public bool Ready { get; set; }

		public float ProgressValue { get; set; }

		public QuestIconBase Icon { get; set; }

		public int CheckCompletionCalls { get; private set; }

		public int UpdateCalls { get; private set; }

		public int CompleteCalls { get; private set; }

		public int ActivateCalls { get; private set; }

		public int DeactivateCalls { get; private set; }

		public int ResetCalls { get; private set; }

		public int PersistenceCalls { get; private set; }

		public int NetworkCalls { get; private set; }

		public override float Progress => ProgressValue;

		public override bool CheckCompletion()
		{
			CheckCompletionCalls++;
			return Ready;
		}

		public override string GetObjectiveText() => ObjectiveTextValue;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
			if (Icon is not null)
			{
				iconGroup.Add(Icon);
			}
		}

		public override void Update() => UpdateCalls++;

		public override void Complete()
		{
			CompleteCalls++;
			base.Complete();
		}

		public override void Activate(WorldQuestBase sourceQuest) => ActivateCalls++;

		public override void Deactivate() => DeactivateCalls++;

		public override void ResetProgress()
		{
			ResetCalls++;
			base.ResetProgress();
		}

		public override void LoadData(TagCompound tag) => PersistenceCalls++;

		public override void SaveData(TagCompound tag) => PersistenceCalls++;

		public override void NetSend(BinaryWriter writer) => NetworkCalls++;

		public override void NetReceive(BinaryReader reader) => NetworkCalls++;

		public override void SendDelta(BinaryWriter writer) => NetworkCalls++;

		public override void ReceiveDelta(BinaryReader reader) => NetworkCalls++;

		public override void SendMain(BinaryWriter writer) => NetworkCalls++;

		public override void ReceiveMain(BinaryReader reader) => NetworkCalls++;
	}

	private sealed class StubSource : QuestSourceBase
	{
		public StubSource(string name)
		{
			Name = name;
		}

		public override Texture2D Texture => null;

		public override string Name { get; }
	}

	private sealed class StubIcon : QuestIconBase
	{
		public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale)
		{
		}
	}

	[TestMethod]
	public void Create_UsesQuestNameForBothWorldIdentityPartsInsteadOfWhoAmI()
	{
		var quest = new StubQuest { NameValue = "stable-world-definition" };
		SetWhoAmI(quest, 731);

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(731, quest.WhoAmI);
		Assert.AreEqual(QuestSide.World, view.Identity.Side);
		Assert.AreEqual("stable-world-definition", view.Identity.DefinitionId);
		Assert.AreEqual("stable-world-definition", view.Identity.InstanceId);
		Assert.AreNotEqual(quest.WhoAmI.ToString(), view.Identity.DefinitionId);
		Assert.AreNotEqual(quest.WhoAmI.ToString(), view.Identity.InstanceId);
	}

	[TestMethod]
	[DataRow(WorldQuestState.Locked, QuestViewState.Locked)]
	[DataRow(WorldQuestState.Active, QuestViewState.Active)]
	[DataRow(WorldQuestState.Completed, QuestViewState.Completed)]
	[DataRow(WorldQuestState.Failed, QuestViewState.Failed)]
	public void Create_MapsEveryWorldState(WorldQuestState state, QuestViewState expected)
	{
		var quest = new StubQuest();
		quest.SetState(state);

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(expected, view.State);
	}

	[TestMethod]
	public void Create_MapsConfigurableWorldMetadata()
	{
		const string description = "[TextDrawer,Text='world body',Color='1,2,3,255']";
		const string hint = "[TextDrawer,Text='world hint',Color='4,5,6,255']";
		var source = new StubSource("world-source");
		var quest = new StubQuest
		{
			DisplayNameValue = "Mapped world quest",
			DescriptionValue = description,
			SourceValue = source,
			TypeValue = QuestType.Legend,
			VisibleValue = false,
		};

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreSame(source, view.Source);
		Assert.IsNull(view.SubSource);
		Assert.AreEqual(QuestType.Legend, view.Type);
		Assert.AreEqual("Mapped world quest", view.DisplayName);
		Assert.AreEqual(description, view.Description);
		Assert.AreEqual(string.Empty, view.Hint);
		Assert.IsFalse(view.Visible);

		quest.HintValue = hint;
		QuestView hintedView = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, hintedView.Hint);
	}

	[TestMethod]
	public void Create_ClampsQuestProgressIncludingNaNAndInfinity()
	{
		(float Domain, float Expected)[] cases =
		[
			(-0.25f, 0f),
			(0.4f, 0.4f),
			(1.25f, 1f),
			(float.NaN, 0f),
			(float.PositiveInfinity, 1f),
			(float.NegativeInfinity, 0f),
		];

		foreach ((float domain, float expected) in cases)
		{
			var quest = new StubQuest { ProgressValue = domain };

			QuestView view = WorldQuestViewAdapter.Create(quest);

			Assert.AreEqual(expected, view.Progress);
		}
	}

	[TestMethod]
	public void Create_NormalizesWorldTimeLimitsAndRemainingTime()
	{
		var expired = new StubQuest { TimeLimitValue = 100 };
		expired.SetTime(120);
		var zeroLimit = new StubQuest { TimeLimitValue = 0 };
		zeroLimit.SetTime(25);
		var negativeLimit = new StubQuest { TimeLimitValue = -10 };
		negativeLimit.SetTime(30);

		QuestView expiredView = WorldQuestViewAdapter.Create(expired);
		QuestView zeroLimitView = WorldQuestViewAdapter.Create(zeroLimit);
		QuestView negativeLimitView = WorldQuestViewAdapter.Create(negativeLimit);

		Assert.AreEqual(120, expiredView.ElapsedTime);
		Assert.AreEqual(100, expiredView.TimeLimit);
		Assert.AreEqual(0, expiredView.RemainingTime);
		Assert.AreEqual(25, zeroLimitView.ElapsedTime);
		Assert.IsNull(zeroLimitView.TimeLimit);
		Assert.IsNull(zeroLimitView.RemainingTime);
		Assert.AreEqual(30, negativeLimitView.ElapsedTime);
		Assert.IsNull(negativeLimitView.TimeLimit);
		Assert.IsNull(negativeLimitView.RemainingTime);
	}

	[TestMethod]
	[DataRow("Follow the trail")]
	[DataRow(QuestHintText.Masked)]
	public void Create_NonWhitespaceHintPreservesCompleteView(string hint)
	{
		var reward = new Item { type = 1, stack = 2 };
		var objective = new StubObjective
		{
			ProgressValue = 0.8f,
			ObjectiveTextValue = "secret objective",
		};
		var quest = new StubQuest
		{
			HintValue = hint,
			DescriptionValue = "secret description",
			VisibleValue = false,
			ProgressValue = 0.75f,
			TimeLimitValue = 120,
		};
		quest.SetState(WorldQuestState.Active);
		quest.SetTime(45);
		quest.Objectives.Add(objective);
		quest.SetRewards(reward);

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, view.Hint);
		Assert.IsFalse(view.Visible);
		Assert.AreEqual(QuestViewState.Active, view.State);
		Assert.AreEqual("secret description", view.Description);
		Assert.HasCount(1, view.ObjectiveNodes);
		Assert.AreEqual("secret objective", ((LeafObjectiveNodeView)view.ObjectiveNodes[0]).Objective.ObjectiveText);
		Assert.HasCount(1, view.Rewards);
		Assert.AreEqual(0.75f, view.Progress);
		Assert.AreEqual(45, view.ElapsedTime);
		Assert.AreEqual(120, view.TimeLimit);
		Assert.AreEqual(75, view.RemainingTime);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow(" ")]
	[DataRow("\t")]
	public void Create_BlankHintExportsDetailsAndLeavesWorldObjectiveDescriptionsEmpty(string hint)
	{
		var reward = new Item { type = 2, stack = 3 };
		var objective = new StubObjective { ProgressValue = 0.35f };
		var quest = new StubQuest
		{
			HintValue = hint,
			ProgressValue = 0.5f,
			TimeLimitValue = 120,
		};
		quest.SetState(WorldQuestState.Active);
		quest.SetTime(45);
		quest.Objectives.Add(objective);
		quest.SetRewards(reward);

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, view.Hint);
		Assert.AreEqual(0.5f, view.Progress);
		Assert.AreEqual(45, view.ElapsedTime);
		Assert.AreEqual(120, view.TimeLimit);
		Assert.AreEqual(75, view.RemainingTime);
		Assert.HasCount(1, view.ObjectiveNodes);
		Assert.HasCount(1, view.Rewards);
		var objectiveView = ((LeafObjectiveNodeView)view.ObjectiveNodes[0]).Objective;
		Assert.AreEqual(string.Empty, objectiveView.Description);
	}

	private static void SetWhoAmI(WorldQuestBase quest, int value)
	{
		var property = typeof(WorldQuestBase).GetProperty(nameof(WorldQuestBase.WhoAmI));
		Assert.IsNotNull(property);
		var setter = property.GetSetMethod(nonPublic: true);
		Assert.IsNotNull(setter);
		setter.Invoke(quest, [value]);
	}
}
