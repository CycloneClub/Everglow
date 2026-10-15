using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public partial class PlayerQuestViewAdapterTest
{
	private sealed class StubQuest : PlayerQuestBase
	{
		public string NameValue { get; set; } = "definition-id";

		public string DisplayNameValue { get; set; } = "Display Name";

		public string DescriptionValue { get; set; } = "Quest description";

		public string HintValue { get; set; } = string.Empty;

		public QuestSourceBase? SourceValue { get; set; } = QuestSourceBase.Default;

		public QuestSourceBase? SubSourceValue { get; set; }

		public QuestType TypeValue { get; set; } = QuestType.None;

		public float ProgressValue { get; set; }

		public int TimeLimitValue { get; set; } = -1;

		public override string Name => NameValue;

		public override string DisplayName => DisplayNameValue;

		public override string Description => DescriptionValue;

		public override string Hint => HintValue;

		public override QuestSourceBase Source => SourceValue!;

		public override QuestSourceBase SubSource => SubSourceValue!;

		public override QuestType Type => TypeValue;

		public override float Progress => ProgressValue;

		public override int TimeLimit => TimeLimitValue;
	}

	private sealed class StubObjective : PlayerObjectiveBase
	{
		public StubObjective(params string[] lines)
		{
			ObjectiveTextValue = string.Join('\n', lines);
		}

		public string ObjectiveTextValue { get; set; } = string.Empty;

		public bool Ready { get; set; }

		public float ProgressValue { get; set; }

		public QuestIconBase? Icon { get; set; }

		public override float Progress => ProgressValue;

		public override bool CheckCompletion() => Ready;

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
			if (Icon is not null)
			{
				iconGroup.Add(Icon);
			}
		}

		public override string GetObjectiveText() => ObjectiveTextValue;
	}

	private sealed class StubSource : QuestSourceBase
	{
		public StubSource(string name)
		{
			Name = name;
		}

		public override Texture2D Texture => null!;

		public override string Name { get; }
	}

	private sealed class StubIcon : QuestIconBase
	{
		public override void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color, float baseScale)
		{
		}
	}

	[TestMethod]
	public void Create_MapsIdentityMetadataSourcesAndVisibility()
	{
		const string description = "[TextDrawer,Text='quest body',Color='1,2,3,255']";
		const string hint = "[TextDrawer,Text='quest hint',Color='4,5,6,255']";
		var source = new StubSource("source");
		var subSource = new StubSource("sub-source");
		var quest = new StubQuest
		{
			NameValue = "quest-definition",
			DisplayNameValue = "Quest title",
			DescriptionValue = description,
			SourceValue = source,
			SubSourceValue = subSource,
			TypeValue = QuestType.Legend,
			IsVisible = false,
			State = PlayerQuestState.Available,
		};

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.AreEqual(QuestSide.Player, view.Identity.Side);
		Assert.AreEqual(quest.Name, view.Identity.DefinitionId);
		Assert.AreEqual(quest.InstanceId, view.Identity.InstanceId);
		Assert.IsTrue(Guid.TryParseExact(view.Identity.InstanceId, "N", out _));
		Assert.AreSame(source, view.Source);
		Assert.AreSame(subSource, view.SubSource);
		Assert.AreEqual(QuestType.Legend, view.Type);
		Assert.AreEqual("Quest title", view.DisplayName);
		Assert.AreEqual(description, view.Description);
		Assert.AreEqual(string.Empty, view.Hint);
		Assert.IsFalse(view.Visible);

		quest.HintValue = hint;
		QuestView hintedView = PlayerQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, hintedView.Hint);

		quest.SourceValue = null;
		QuestView defaultSourceView = PlayerQuestViewAdapter.Create(quest);

		Assert.AreSame(QuestSourceBase.Default, defaultSourceView.Source);
		Assert.AreSame(subSource, defaultSourceView.SubSource);
	}

	[TestMethod]
	[DataRow(PlayerQuestState.Available, QuestViewState.Available)]
	[DataRow(PlayerQuestState.Accepted, QuestViewState.Active)]
	[DataRow(PlayerQuestState.Completed, QuestViewState.Completed)]
	[DataRow(PlayerQuestState.Failed, QuestViewState.Failed)]
	public void Create_MapsEveryPlayerState(PlayerQuestState state, QuestViewState expected)
	{
		var quest = new StubQuest { State = state };

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.AreEqual(expected, view.State);
	}

	[TestMethod]
	public void Create_ClampsQuestProgressIncludingNaN()
	{
		var belowRange = new StubQuest { ProgressValue = -0.25f };
		var aboveRange = new StubQuest { ProgressValue = 1.25f };
		var notANumber = new StubQuest { ProgressValue = float.NaN };
		var positiveInfinity = new StubQuest { ProgressValue = float.PositiveInfinity };
		var negativeInfinity = new StubQuest { ProgressValue = float.NegativeInfinity };

		Assert.AreEqual(0f, PlayerQuestViewAdapter.Create(belowRange).Progress);
		Assert.AreEqual(1f, PlayerQuestViewAdapter.Create(aboveRange).Progress);
		Assert.AreEqual(0f, PlayerQuestViewAdapter.Create(notANumber).Progress);
		Assert.AreEqual(1f, PlayerQuestViewAdapter.Create(positiveInfinity).Progress);
		Assert.AreEqual(0f, PlayerQuestViewAdapter.Create(negativeInfinity).Progress);
	}

	[TestMethod]
	public void Create_NormalizesTimeLimitsAndRemainingTime()
	{
		var expired = new StubQuest
		{
			Time = 120,
			TimeLimitValue = 100,
		};
		var zeroLimit = new StubQuest
		{
			Time = 25,
			TimeLimitValue = 0,
		};
		var negativeLimit = new StubQuest
		{
			Time = 30,
			TimeLimitValue = -1,
		};

		QuestView expiredView = PlayerQuestViewAdapter.Create(expired);
		QuestView zeroLimitView = PlayerQuestViewAdapter.Create(zeroLimit);
		QuestView negativeLimitView = PlayerQuestViewAdapter.Create(negativeLimit);

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
		var visibleIcon = new StubIcon();
		var objective = new StubObjective("secret objective")
		{
			Icon = visibleIcon,
			ProgressValue = 0.8f,
		};
		var quest = new StubQuest
		{
			HintValue = hint,
			DescriptionValue = "secret description",
			ProgressValue = 0.75f,
			Time = 45,
			TimeLimitValue = 120,
			IsVisible = false,
			State = PlayerQuestState.Accepted,
		};
		quest.Objectives.Add(objective);
		quest.RewardItems.Add(new Item());

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, view.Hint);
		Assert.IsFalse(view.Visible);
		Assert.AreEqual("secret description", view.Description);
		Assert.HasCount(1, view.ObjectiveNodes);
		Assert.AreEqual("secret objective", ((LeafObjectiveNodeView)view.ObjectiveNodes[0]).Objective.ObjectiveText);
		Assert.HasCount(1, view.Rewards);
		Assert.AreEqual(0.75f, view.Progress);
		Assert.AreEqual(45, view.ElapsedTime);
		Assert.AreEqual(120, view.TimeLimit);
		Assert.AreEqual(75, view.RemainingTime);
		Assert.HasCount(2, view.Icons);
		Assert.IsInstanceOfType<QuestSourceIcon>(view.Icons[0]);
		Assert.AreSame(visibleIcon, view.Icons[1]);
	}

	[TestMethod]
	[DataRow(" ")]
	[DataRow("\t")]
	public void Create_WhitespaceHintDoesNotHideDetails(string hint)
	{
		var quest = new StubQuest
		{
			HintValue = hint,
			DescriptionValue = "visible description",
			ProgressValue = 0.75f,
		};

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.AreEqual(hint, view.Hint);
		Assert.AreEqual("visible description", view.Description);
		Assert.AreEqual(0.75f, view.Progress);
	}

}
