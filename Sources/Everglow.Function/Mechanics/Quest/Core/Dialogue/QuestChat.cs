using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.Core.Dialogue;

/// <summary>NPC 只需转发指定按钮。对话定义、提交和进度仍归 Quest / Objective。</summary>
public static class QuestChat
{
	private sealed record DialogueBinding(string LocalizationKey, ObjectiveDialogue Dialogue, IQuestDialogueObjective Objective, Func<bool> IsAvailable, Func<bool> IsCompleted);

	private static readonly ObjectiveDialogue DefaultDialogue = new();

	private static NPC chatNPC;
	private static DialogueBinding activeDialogue;
	private static QuestDialogueSession currentSession;
	private static string lastChatText;

	/// <summary>在 SetChatButtons 的原有按钮配置之后调用。没有可用任务时保留原按钮。</summary>
	public static void SetChatButton(NPC npc, ref string button)
	{
		if (!CanChat(npc))
		{
			return;
		}
		Update();
		if (IsChattingWith(npc))
		{
			button = currentSession.State == QuestDialogueState.WaitingForCompletion ? QuestText.Get("Dialogue.Waiting")
				: currentSession.IsConfirmationRequired && currentSession.ConfirmButtonTextKey is { } key ? GetTextValue(key)
				: QuestText.Get(currentSession.IsConfirmationRequired ? "Dialogue.Confirm" : "Dialogue.Continue");
		}
		else if (FindDialogue(npc) is not null)
		{
			button = QuestText.Get("Dialogue.Open");
		}
	}

	/// <summary>在 OnChatButtonClicked 最前面调用；返回 true 时 NPC 不再执行该按钮的原动作。</summary>
	public static bool OnChatButtonClicked(NPC npc, bool firstButton, bool useSecondButton = true)
	{
		if (firstButton == useSecondButton)
		{
			CloseChat();
			return false;
		}
		if (!CanChat(npc))
		{
			CloseChat();
			return false;
		}
		if (currentSession is not null && chatNPC == npc)
		{
			currentSession.Continue();
			UpdateChatText();
			if (currentSession.State == QuestDialogueState.Closed)
			{
				CloseChat();
			}
			return true;
		}
		CloseChat();
		activeDialogue = FindDialogue(npc);
		if (activeDialogue is null)
		{
			return false;
		}
		chatNPC = npc;
		currentSession = new QuestDialogueSession(
			activeDialogue.Dialogue,
			() => activeDialogue.IsAvailable() && CanChat(chatNPC) && activeDialogue.Objective.TrySubmitObjective(Main.LocalPlayer),
			activeDialogue.IsCompleted, activeDialogue.IsAvailable);
		UpdateChatText();
		return true;
	}

	public static bool IsChattingWith(NPC npc) => currentSession is not null
		&& currentSession.State != QuestDialogueState.Closed && npc is not null && chatNPC == npc;

	public static void CloseChat()
	{
		currentSession?.Close();
		currentSession = null;
		chatNPC = null;
		activeDialogue = null;
		lastChatText = null;
	}

	internal static void Update()
	{
		if (currentSession is null)
		{
			return;
		}
		if (!CanChat(chatNPC) || Main.gameMenu || Main.npcShop != 0)
		{
			CloseChat();
			return;
		}
		currentSession.Update();
		UpdateChatText();
		if (currentSession.State == QuestDialogueState.Closed)
		{
			CloseChat();
		}
	}

	private static bool CanChat(NPC npc) => !Main.dedServ && npc is not null
		&& Main.LocalPlayer.TalkNPC == npc && CanSubmitObjective(Main.LocalPlayer, npc.type);

	/// <summary>任务交互仅接受本地玩家在有效距离内发起的提交。</summary>
	internal static bool CanSubmitObjective(Player player, int npcType) => Main.netMode != NetmodeID.Server
		&& player == Main.LocalPlayer && player.IsTalkingTo(npcType)
		&& Vector2.DistanceSquared(player.Center, player.TalkNPC.Center) <= 240 * 240;

	private static void UpdateChatText()
	{
		if (currentSession.State == QuestDialogueState.Closed)
		{
			return;
		}
		string text = currentSession.ChatTextKey is { } key ? GetTextValue(key)
			: currentSession.State == QuestDialogueState.Unavailable ? QuestText.Get("Dialogue.Unavailable")
			: activeDialogue.Objective.GetChat();
		if (text != lastChatText)
		{
			Main.npcChatText = text;
			lastChatText = text;
		}
	}

	private static string GetTextValue(string key) => Language.GetTextValue(activeDialogue.LocalizationKey + "." + key);

	private static DialogueBinding FindDialogue(NPC npc)
	{
		var world = WorldQuestManager.Instance;
		if (world is not null)
		{
			foreach (var quest in world.Quests.Where(q => q.State == WorldQuestState.Active).OrderBy(q => q.Name, StringComparer.Ordinal))
			{
				foreach (var objective in quest.ActiveObjectives.OrderBy(o => o.ObjectiveID))
				{
					if (objective.CanProgress && !objective.CheckCompletion() && objective is IQuestDialogueObjective interaction && interaction.NPCType == npc.type)
					{
						return new(quest.LocalizationKey, interaction.Dialogue ?? DefaultDialogue, interaction,
							() => world.Quests.Contains(quest) && quest.State == WorldQuestState.Active && objective.CanProgress && !objective.CheckCompletion() && quest.ActiveObjectives.Contains(objective),
							() => objective.Completed || objective.CheckCompletion());
					}
				}
			}
		}
		var player = PlayerQuestManager.Instance;
		if (player is not null)
		{
			foreach (var quest in player.Quests.Where(q => q.State == PlayerQuestState.Accepted).OrderBy(q => q.Name, StringComparer.Ordinal))
			{
				foreach (var objective in quest.Objectives.FindCurrentObjectives().OrderBy(o => o.ObjectiveID))
				{
					if (objective.CanProgress && !objective.CheckCompletion() && objective is IQuestDialogueObjective interaction && interaction.NPCType == npc.type)
					{
						return new(quest.LocalizationKey, interaction.Dialogue ?? DefaultDialogue, interaction,
							() => player.Quests.Contains(quest) && quest.State == PlayerQuestState.Accepted && objective.CanProgress && !objective.CheckCompletion() && quest.Objectives.FindCurrentObjectives().Contains(objective),
							() => objective.Completed || objective.CheckCompletion());
					}
				}
			}
		}
		return null;
	}
}
