using System.Reflection;
using System.Text.Json;
using Terraria.Localization;
using Terraria.ID;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class TextDefinitionTest
{
	private LanguageManager originalLanguage = null!;

	[TestInitialize]
	public void InitializeLanguage()
	{
		originalLanguage = LanguageManager.Instance;
		LoadQuestLanguage("zh-Hans");
	}

	[TestCleanup]
	public void RestoreLanguage() => LanguageManager.Instance = originalLanguage;

	private static void LoadQuestLanguage(string culture)
	{
		LanguageManager.Instance = (LanguageManager)Activator.CreateInstance(typeof(LanguageManager), true)!;
		var texts = (Dictionary<string, LocalizedText>)typeof(LanguageManager)
			.GetField("_localizedTexts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(LanguageManager.Instance)!;
		var root = new DirectoryInfo(AppContext.BaseDirectory);
		while (root is not null && !File.Exists(Path.Combine(root.FullName, "Everglow.sln")))
		{
			root = root.Parent;
		}
		Assert.IsNotNull(root);
		using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(
			root.FullName, "Sources", "Everglow", "Localization", culture, "Mods.Everglow.QuestSystem.hjson")));
		AddTexts(document.RootElement, "Mods.Everglow.QuestSystem", texts);
	}

	private static void AddTexts(JsonElement element, string prefix, Dictionary<string, LocalizedText> texts)
	{
		foreach (var property in element.EnumerateObject())
		{
			string key = prefix + "." + property.Name;
			if (property.Value.ValueKind == JsonValueKind.Object)
			{
				AddTexts(property.Value, key, texts);
			}
			else
			{
				texts[key] = (LocalizedText)Activator.CreateInstance(
					typeof(LocalizedText), BindingFlags.Instance | BindingFlags.NonPublic, null,
					[key, property.Value.GetString()!], null)!;
			}
		}
	}

	[TestMethod]
	[DataRow(null, "全部")]
	[DataRow(QuestViewState.Active, "进行中")]
	[DataRow(QuestViewState.Completed, "完成")]
	public void GetQuestStateText_ReturnsPresentationLabel(QuestViewState? state, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetQuestStateText(state));
	}

	[TestMethod]
	[DataRow(null, "全部")]
	[DataRow(QuestType.MainStory, "主线任务")]
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
			$"[TimerIconDrawer,QuestName='TestQuest'] 剩余时间：[TimerStringDrawer,QuestName='TestQuest']\n\n描述：\n{description}\n",
			TextDefinition.GetQuestDetailText(quest));
	}

	[TestMethod]
	public void GetQuestObjectivesText_ShowsOnlyCurrentDescriptionAndDefaultText()
	{
		var quest = new QuestView
		{
			State = QuestViewState.Active,
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Past", State = ObjectiveViewState.Completed }),
				new BranchObjectiveNodeView(
				[
					new ObjectiveBranchView(
						ObjectiveBranchState.Selected,
						[
							new ObjectiveView { Description = "Current context", ObjectiveText = "Current action", State = ObjectiveViewState.Active },
							new ObjectiveView { ObjectiveText = "Future", State = ObjectiveViewState.Pending },
						]),
					new ObjectiveBranchView(
						ObjectiveBranchState.Skipped,
						[
							new ObjectiveView { ObjectiveText = "Skipped", State = ObjectiveViewState.Skipped },
						]),
				]),
			],
		};

		Assert.AreEqual("目标：\nCurrent context\nCurrent action\n", TextDefinition.GetQuestObjectivesText(quest));
	}

	[TestMethod]
	[DataRow(QuestSide.Player, QuestViewState.Available)]
	[DataRow(QuestSide.World, QuestViewState.Locked)]
	public void GetQuestObjectiveLines_PreviewsPendingCurrentNodeRegardlessOfHideMode(QuestSide side, QuestViewState state)
	{
		foreach (QuestHideMode mode in Enum.GetValues<QuestHideMode>())
		{
			var first = new ObjectiveView { Description = "Context", ObjectiveText = "First", State = ObjectiveViewState.Pending };
			var second = new ObjectiveView { ObjectiveText = "Second", State = ObjectiveViewState.Pending };
			var quest = new QuestView
			{
				Identity = new QuestIdentity(side, "quest", "quest"),
				State = state,
				HideMode = mode,
				ObjectiveNodes =
				[
					new ParallelObjectiveNodeView([first, second]),
					new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Future", State = ObjectiveViewState.Pending }),
				],
			};

			Assert.AreEqual("目标：\nContext\nFirst\nSecond\n", TextDefinition.GetQuestObjectivesText(quest));
			Assert.AreEqual(ObjectiveViewState.Pending, first.State);
			Assert.AreEqual(ObjectiveViewState.Pending, second.State);
		}
	}

	[TestMethod]
	[DataRow(ObjectiveBranchState.Candidate)]
	[DataRow(ObjectiveBranchState.Selected)]
	public void GetQuestObjectiveLines_PreviewsOnlyNextPendingObjectiveInEachBranch(ObjectiveBranchState branchState)
	{
		var quest = new QuestView
		{
			State = QuestViewState.Active,
			ObjectiveNodes =
			[
				new BranchObjectiveNodeView(
				[
					new ObjectiveBranchView(
						branchState,
						[
							new ObjectiveView { ObjectiveText = "First", State = ObjectiveViewState.Pending },
							new ObjectiveView { ObjectiveText = "Future", State = ObjectiveViewState.Pending },
						]),
					new ObjectiveBranchView(
						ObjectiveBranchState.Skipped,
						[
							new ObjectiveView { ObjectiveText = "Excluded", State = ObjectiveViewState.Skipped },
						]),
				]),
			],
		};

		Assert.AreEqual("目标：\nFirst\n", TextDefinition.GetQuestObjectivesText(quest));
	}

	[TestMethod]
	[DataRow(QuestViewState.Locked, "First")]
	[DataRow(QuestViewState.Available, "First")]
	[DataRow(QuestViewState.Active, "Current")]
	[DataRow(QuestViewState.Failed, "Current")]
	[DataRow(QuestViewState.Completed, "Last")]
	public void GetQuestObjectivesText_PreservesTheStageForEachQuestState(QuestViewState state, string expected)
	{
		var quest = new QuestView
		{
			State = state,
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "First", State = ObjectiveViewState.Completed }),
				new LeafObjectiveNodeView(new ObjectiveView
				{
					ObjectiveText = "Current",
					State = state == QuestViewState.Completed ? ObjectiveViewState.Completed : ObjectiveViewState.Pending,
				}),
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Last", State = ObjectiveViewState.Completed }),
			],
		};

		Assert.AreEqual("目标：\n" + expected + "\n", TextDefinition.GetQuestObjectivesText(quest));
	}

	[TestMethod]
	public void GetQuestObjectivesText_CompletedBranchShowsSelectedBranchFinalObjective()
	{
		var quest = new QuestView
		{
			State = QuestViewState.Completed,
			ObjectiveNodes =
			[
				new BranchObjectiveNodeView(
				[
					new ObjectiveBranchView(
						ObjectiveBranchState.Selected,
						[
							new ObjectiveView { ObjectiveText = "Earlier", State = ObjectiveViewState.Completed },
							new ObjectiveView { ObjectiveText = "Final", State = ObjectiveViewState.Completed },
						]),
					new ObjectiveBranchView(
						ObjectiveBranchState.Skipped,
						[
							new ObjectiveView { ObjectiveText = "Excluded", State = ObjectiveViewState.Skipped },
						]),
				]),
			],
		};

		Assert.AreEqual("目标：\nFinal\n", TextDefinition.GetQuestObjectivesText(quest));
	}

	[TestMethod]
	public void GetQuestObjectiveLines_DoesNotKeepTimedOutAlternativeFromCompletedNode()
	{
		var quest = new QuestView
		{
			State = QuestViewState.Active,
			ObjectiveNodes =
			[
				new AnyOfObjectiveNodeView(
				[
					new ObjectiveView { ObjectiveText = "Old timeout", State = ObjectiveViewState.TimedOut },
					new ObjectiveView { ObjectiveText = "Done", State = ObjectiveViewState.Completed },
				]),
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Current", State = ObjectiveViewState.Active }),
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Future timeout", State = ObjectiveViewState.TimedOut }),
			],
		};

		Assert.AreEqual("目标：\nCurrent\n", TextDefinition.GetQuestObjectivesText(quest));
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
			State = ObjectiveViewState.Active,
			Timer = secondTimer,
		};
		var quest = new QuestView
		{
			State = QuestViewState.Active,
			ObjectiveNodes =
			[
				new ParallelObjectiveNodeView([firstObjective, secondObjective]),
			],
		};

		IReadOnlyList<ObjectiveLineView> lines = TextDefinition.GetQuestObjectiveLines(quest);

		Assert.AreEqual(2, lines.Count);
		Assert.AreSame(firstObjective, lines[0].Objective);
		Assert.AreSame(firstTimer, lines[0].Timer);
		Assert.StartsWith("First", lines[0].Text);
		Assert.AreSame(secondObjective, lines[1].Objective);
		Assert.AreSame(secondTimer, lines[1].Timer);
		Assert.StartsWith("Second", lines[1].Text);
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
			State = QuestViewState.Active,
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
						State = ObjectiveViewState.Active,
						Timer = new TimerView { TimeLimit = 120, ElapsedTime = 0 },
					},
				]),
			],
		};

		IReadOnlyList<ObjectiveLineView> lines = TextDefinition.GetQuestObjectiveLines(quest);

		Assert.AreEqual("First", lines[0].Text);
		Assert.AreEqual("Second", lines[1].Text);
	}

	[TestMethod]
	public void GetQuestObjectivesText_MarksTimedOutObjective()
	{
		var quest = new QuestView
		{
			State = QuestViewState.Active,
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
			State = QuestViewState.Active,
			ObjectiveNodes =
			[
				new LeafObjectiveNodeView(new ObjectiveView { ObjectiveText = "Untimed", State = ObjectiveViewState.Active }),
			],
		};

		Assert.AreEqual("目标：\nUntimed\n", TextDefinition.GetQuestObjectivesText(quest));
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
	[DataRow(null, "不限时")]
	[DataRow(3720, "1分2秒")]
	public void GetRemainingTimeText_FormatsTicks(int? remainingTime, string expected)
	{
		Assert.AreEqual(expected, TextDefinition.GetRemainingTimeText(remainingTime));
	}

	[TestMethod]
	[DataRow(-60, "0秒")]
	[DataRow(0, "0秒")]
	[DataRow(1020, "17秒")]
	[DataRow(7500, "2分5秒")]
	[DataRow(216300, "1时0分5秒")]
	[DataRow(443040, "2时3分4秒")]
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
	[DataRow(QuestNotificationType.ObjectiveCompleted, "Node", "[World Quest]任务当前目标已完成", 250, 250, 150)]
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

	[TestMethod]
	public void SwitchingLanguageUpdatesLabelsAndTimerWithoutRecreatingViews()
	{
		var quest = new QuestView { DisplayName = "Test Quest" };
		var notification = new QuestNotification(
			new QuestIdentity(QuestSide.World, "Test", "Test"), QuestNotificationType.Completed);
		Assert.AreEqual("[Test Quest]任务已完成", TextDefinition.GetQuestNotificationText(quest, notification));
		LoadQuestLanguage("en-US");
		Assert.AreEqual("Main Story", TextDefinition.GetQuestTypeText(QuestType.MainStory));
		Assert.AreEqual("In Progress", TextDefinition.GetQuestStateText(QuestViewState.Active));
		Assert.AreEqual("Quest completed: [Test Quest]", TextDefinition.GetQuestNotificationText(quest, notification));
		Assert.AreEqual("1m 2s", TextDefinition.GetRemainingTimeText(3720));
		Assert.AreEqual("2m5s", TextDefinition.GetObjectiveTimerText(7500));
	}

	[TestMethod]
	public void DefaultObjectiveTextKeepsItemMarkupAndUsesCurrentLanguage()
	{
		var objective = new WorldConsumeItemObjective(ItemID.WoodenArrow, 2);
		string item = ItemDrawer.Create(ItemID.WoodenArrow);
		Assert.AreEqual($"消耗{item}2个 (0/2)", objective.GetObjectiveText());
		LoadQuestLanguage("en-US");
		Assert.AreEqual($"Consume {item} ×2 (0/2)", objective.GetObjectiveText());
	}

	[TestMethod]
	public void DefaultDialogueFollowsLanguageButAuthoredDialogueIsPreserved()
	{
		var defaults = new GiveItemObjective([ItemID.WoodenArrow], 2, NPCID.Guide);
		var authored = new GiveItemObjective([ItemID.WoodenArrow], 2, NPCID.Guide, "Custom request", "Custom thanks");
		Assert.AreEqual("谢谢你！", defaults.EndText);
		LoadQuestLanguage("en-US");
		Assert.AreEqual("Please bring me some items.", defaults.StartText);
		Assert.AreEqual("Thank you!", defaults.EndText);
		Assert.AreEqual("Custom request", authored.StartText);
		Assert.AreEqual("Custom thanks", authored.EndText);
	}
}
