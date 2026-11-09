namespace Everglow.Commons.Mechanics.Quest.Core.Dialogue;

/// <summary>文本键相对于所属 Quest.LocalizationKey；定义不保存会话或任务进度。</summary>
public sealed class ObjectiveDialogue
{
	public IReadOnlyList<string> TextKeys { get; private set; } = Array.Empty<string>();

	public IReadOnlyList<string> CompletionTextKeys { get; private set; } = Array.Empty<string>();

	public string ConfirmButtonTextKey { get; private set; }

	public string UnavailableTextKey { get; private set; }

	public ObjectiveDialogue WithText(params string[] keys)
	{
		TextKeys = CopyTextKeys(keys);
		return this;
	}

	public ObjectiveDialogue WithConfirmButtonText(string key)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);
		ConfirmButtonTextKey = key;
		return this;
	}

	public ObjectiveDialogue WithCompletionText(params string[] keys)
	{
		CompletionTextKeys = CopyTextKeys(keys);
		return this;
	}

	public ObjectiveDialogue WithUnavailableText(string key)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);
		UnavailableTextKey = key;
		return this;
	}

	private static IReadOnlyList<string> CopyTextKeys(string[] keys)
	{
		ArgumentNullException.ThrowIfNull(keys);
		foreach (string key in keys)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(key);
		}
		return Array.AsReadOnly((string[])keys.Clone());
	}
}

public interface IQuestDialogueObjective
{
	ObjectiveDialogue Dialogue { get; set; }

	int NPCType { get; }

	/// <summary>原版 NPC 聊天框使用的纯文本，不包含任务面板的 StringDrawer 标记。</summary>
	string GetChat();

	/// <summary>返回是否接受提交。联机提交后仍需等待目标进度同步。</summary>
	bool TrySubmitObjective(Player player);
}
