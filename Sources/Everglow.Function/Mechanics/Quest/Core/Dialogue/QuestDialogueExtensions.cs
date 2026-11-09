namespace Everglow.Commons.Mechanics.Quest.Core.Dialogue;

public static class QuestDialogueExtensions
{
	/// <summary>为支持对话的目标配置台词，并保留具体目标类型以供链式调用。</summary>
	public static T WithDialogue<T>(this T objective, Action<ObjectiveDialogue> configure)
		where T : class, IQuestDialogueObjective
	{
		ArgumentNullException.ThrowIfNull(objective);
		ArgumentNullException.ThrowIfNull(configure);
		var dialogue = new ObjectiveDialogue();
		configure(dialogue);
		objective.Dialogue = dialogue;
		return objective;
	}
}
