namespace Everglow.Commons.Mechanics.Quest.Core;

public static class QuestHintRules
{
	public static bool HasContent(string hint) => !string.IsNullOrWhiteSpace(hint);
}
