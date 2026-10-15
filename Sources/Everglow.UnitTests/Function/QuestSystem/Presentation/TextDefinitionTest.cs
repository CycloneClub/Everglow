using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class TextDefinitionTest
{
	[TestMethod]
	[DataRow(null, "All")]
	[DataRow(QuestViewState.Active, "Accepted")]
	[DataRow(QuestViewState.Completed, "Completed")]
	public void GetQuestStateText_ReturnsPresentationLabel(QuestViewState? state, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetQuestStateText(state));
	}

	[TestMethod]
	[DataRow(null, "All")]
	[DataRow(QuestType.MainStory, "MainStory")]
	public void GetQuestTypeText_ReturnsPresentationLabel(QuestType? type, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetQuestTypeText(type));
	}

	[TestMethod]
	public void GetQuestDetailText_FormatsTimerAndDescription()
	{
		const string description = "[TextDrawer,Text='Description',Color='1,2,3,255']";
		var quest = new QuestView
		{
			Identity = new QuestIdentity(QuestSide.Player, "TestQuest", "TestQuest"),
			Description = description,
			TimeLimit = 60,
		};

		Assert.AreEqual(
			$"[TimerIconDrawer,QuestName='TestQuest'] 剩余时间:[TimerStringDrawer,QuestName='TestQuest']\n\n描述：\n{description}\n",
			TextDefinition.GetQuestDetailText(quest));
	}

	[TestMethod]
	public void GetQuestObjectivesText_FormatsCompletedAndBranchObjectives()
	{
		var quest = new QuestView
		{
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView
				{
					Description = "must not render",
					ObjectiveText = "First",
					State = ObjectiveViewState.Completed,
				}),
				new BranchObjectiveNodeView(
				[
					new ObjectiveBranchView(
						ObjectiveBranchState.Candidate,
						[new ObjectiveView
						{
							Description = "must not render",
							ObjectiveText = "Second",
						}]),
				]),
			],
		};

		string text = TextDefinition.GetQuestObjectivesText(quest);

		Assert.AreEqual(
			"目标：\n1.1 [TextDrawer,Text='(已完成)',Color='100,100,100,255'] First\n2.1 [TextDrawer,Text='(Branch 1)',Color='100,180,120,255'] Second\n",
			text);
		Assert.DoesNotContain("must not render", text);
	}

	[TestMethod]
	public void GetQuestObjectiveLines_PreservesEachObjectivesTimer()
	{
		var firstTimer = new TimerView { TimeLimit = 600, ElapsedTime = 120 };
		var secondTimer = new TimerView { TimeLimit = 300, ElapsedTime = 60 };
		var firstObjective = new ObjectiveView
		{
			ObjectiveText = "First",
			State = ObjectiveViewState.Active,
			Timer = firstTimer,
		};
		var secondObjective = new ObjectiveView
		{
			ObjectiveText = "Second",
			State = ObjectiveViewState.Pending,
			Timer = secondTimer,
		};
		var quest = new QuestView
		{
			ObjectiveNodes =
			[
				new ParallelObjectiveNodeView([firstObjective, secondObjective]),
			],
		};

		IReadOnlyList<ObjectiveLineView> lines = TextDefinition.GetQuestObjectiveLines(quest);

		Assert.AreEqual(2, lines.Count);
		Assert.AreSame(firstObjective, lines[0].Objective);
		Assert.AreSame(firstTimer, lines[0].Timer);
		Assert.StartsWith("1.1 First", lines[0].Text);
		Assert.AreSame(secondObjective, lines[1].Objective);
		Assert.AreSame(secondTimer, lines[1].Timer);
		Assert.StartsWith("1.2 Second", lines[1].Text);
	}

	[TestMethod]
	[DataRow(ObjectiveViewState.Pending, true)]
	[DataRow(ObjectiveViewState.Active, true)]
	[DataRow(ObjectiveViewState.TimedOut, true)]
	[DataRow(ObjectiveViewState.Completed, false)]
	[DataRow(ObjectiveViewState.Skipped, false)]
	public void ObjectiveLineView_TimerVisibilityFollowsObjectiveState(ObjectiveViewState state, bool expectedVisible)
	{
		var timer = new TimerView { TimeLimit = 60 };
		var objective = new ObjectiveView { State = state, Timer = timer };
		var line = new ObjectiveLineView(objective, "Objective");

		if (expectedVisible)
		{
			Assert.AreSame(timer, line.Timer);
		}
		else
		{
			Assert.IsNull(line.Timer);
		}
	}

	[TestMethod]
	public void GetQuestObjectiveLines_OmitDuplicateRemainingTimeText()
	{
		var quest = new QuestView
		{
			ObjectiveNodes =
			[
				new ParallelObjectiveNodeView(
				[
					new ObjectiveView
					{
						ObjectiveText = "First",
						State = ObjectiveViewState.Active,
						Timer = new TimerView { TimeLimit = 7200, ElapsedTime = 3480 },
					},
					new ObjectiveView
					{
						ObjectiveText = "Second",
						State = ObjectiveViewState.Pending,
						Timer = new TimerView { TimeLimit = 120, ElapsedTime = 0 },
					},
				]),
			],
		};

		IReadOnlyList<ObjectiveLineView> lines = TextDefinition.GetQuestObjectiveLines(quest);

		Assert.AreEqual("1.1 First", lines[0].Text);
		Assert.AreEqual("1.2 Second", lines[1].Text);
	}

	[TestMethod]
	public void GetQuestObjectivesText_MarksTimedOutObjective()
	{
		var quest = new QuestView
		{
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView
				{
					ObjectiveText = "Rescue NPC",
					State = ObjectiveViewState.TimedOut,
					Timer = new TimerView { TimeLimit = 60, ElapsedTime = 60 },
				}),
			],
		};

		string text = TextDefinition.GetQuestObjectivesText(quest);

		Assert.Contains("[TextDrawer,Text='(已超时)',Color='210,90,70,255'] Rescue NPC", text);
		Assert.DoesNotContain("剩余", text);
	}

	[TestMethod]
	public void GetQuestObjectivesText_LeavesUntimedObjectiveUnchanged()
	{
		var quest = new QuestView
		{
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Untimed" }),
			],
		};

		Assert.AreEqual("目标：\n1.1 Untimed\n", TextDefinition.GetQuestObjectivesText(quest));
	}

	[TestMethod]
	public void GetQuestActionText_NullEntrySubstitutesColorArgument()
	{
		Assert.AreEqual(
			"[TextDrawer,Text='',Color='45,38,33']",
			TextDefinition.GetQuestActionText(null, "45,38,33"));
	}

	[TestMethod]
	public void GetQuestActionText_UsesAvailableSubmitAction()
	{
		var identity = new QuestIdentity(QuestSide.Player, "TestQuest", "TestQuest");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Active },
			[new QuestAction(identity, QuestActionType.Submit)]);

		Assert.AreEqual(
			"[TextDrawer,Text='提交',Color='45,38,33']",
			TextDefinition.GetQuestActionText(entry, "45,38,33"));
	}

	[TestMethod]
	[DataRow(QuestActionType.Retry, QuestViewState.Failed, "重试")]
	[DataRow(QuestActionType.ClaimReward, QuestViewState.Completed, "领取奖励")]
	public void GetQuestActionText_UsesExportedActionBeforeState(
		QuestActionType actionType,
		QuestViewState state,
		string expectedText)
	{
		var identity = new QuestIdentity(QuestSide.World, "TestQuest", "TestQuest");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity, State = state },
			[new QuestAction(identity, actionType)]);

		Assert.AreEqual(
			$"[TextDrawer,Text='{expectedText}',Color='45,38,33']",
			TextDefinition.GetQuestActionText(entry, "45,38,33"));
	}

	[TestMethod]
	public void GetQuestActionText_WorldActiveWithoutActionUsesInProgressStateLabel()
	{
		var identity = new QuestIdentity(QuestSide.World, "TestQuest", "TestQuest");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Active },
			[]);

		Assert.AreEqual(
			"[TextDrawer,Text='进行中',Color='45,38,33']",
			TextDefinition.GetQuestActionText(entry, "45,38,33"));
	}

	[TestMethod]
	public void GetQuestActionText_CompletedWithoutActionUsesPassiveStateLabel()
	{
		var identity = new QuestIdentity(QuestSide.World, "TestQuest", "TestQuest");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Completed },
			[]);

		Assert.AreEqual(
			"[TextDrawer,Text='完成',Color='45,38,33']",
			TextDefinition.GetQuestActionText(entry, "45,38,33"));
	}

	[TestMethod]
	public void GetQuestActionText_UsesLockedStateLabel()
	{
		var identity = new QuestIdentity(QuestSide.World, "TestQuest", "TestQuest");
		var entry = new QuestPresentationEntry(
			new QuestView { Identity = identity, State = QuestViewState.Locked },
			[]);

		Assert.AreEqual(
			"[TextDrawer,Text='锁定',Color='45,38,33']",
			TextDefinition.GetQuestActionText(entry, "45,38,33"));
	}

	[TestMethod]
	[DataRow(null, "Indefinitely")]
	[DataRow(3720, "1Min 2s")]
	public void GetRemainingTimeText_FormatsTicks(int? remainingTime, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetRemainingTimeText(remainingTime));
	}

	[TestMethod]
	[DataRow(-60, "0s")]
	[DataRow(0, "0s")]
	[DataRow(1020, "17s")]
	[DataRow(7500, "2m5s")]
	[DataRow(216300, "1h0m5s")]
	[DataRow(443040, "2h3m4s")]
	public void GetObjectiveTimerText_FormatsCompactTimeUnits(int remainingTime, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetObjectiveTimerText(remainingTime));
	}

	[TestMethod]
	public void GetObjectiveTimerTooltip_ReturnsRetryText()
	{
		Assert.AreEqual("重试", TextDefinition.GetObjectiveTimerTooltip());
	}

	[TestMethod]
	[DataRow(QuestNotificationType.Unlocked, null, "[World Quest]任务已解锁", 150, 150, 250)]
	[DataRow(QuestNotificationType.Restored, null, "[World Quest]任务已恢复", 150, 150, 250)]
	[DataRow(QuestNotificationType.Failed, null, "[World Quest]任务已失败", 250, 150, 150)]
	[DataRow(QuestNotificationType.Completed, null, "[World Quest]任务已完成", 150, 250, 150)]
	[DataRow(QuestNotificationType.Restarted, null, "[World Quest]任务已重启", 150, 250, 150)]
	[DataRow(QuestNotificationType.ObjectiveCompleted, "Node", "[World Quest]任务当前节点[Node]中目标已完成", 250, 250, 150)]
	public void QuestNotificationDefinitions_ReturnPresentationValues(
		QuestNotificationType type,
		string detail,
		string expectedText,
		int red,
		int green,
		int blue)
	{
		var quest = new QuestView { DisplayName = "World Quest" };
		var notification = new QuestNotification(
			new QuestIdentity(QuestSide.World, "TestQuest", "TestQuest"),
			type,
			detail);

		Assert.AreEqual(expectedText, TextDefinition.GetQuestNotificationText(quest, notification));
		Assert.AreEqual(new Color(red, green, blue), ColorDefinition.GetQuestNotificationColor(type));
	}
}
