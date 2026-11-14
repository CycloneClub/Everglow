using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.Core;

public static class QuestText
{
	public static string Get(string key, params object[] args) =>
		Language.GetTextValue("Mods.Everglow.QuestSystem." + key, args);

	/// <summary>
	/// Vanilla chat tag for an item icon. Stacks above 1 use <c>[i/sStack:Type]</c>.
	/// NPC chat parses this; the quest panel does not.
	/// </summary>
	public static string ItemTag(int type, int stack = 1)
	{
		if (type <= 0)
		{
			return string.Empty;
		}

		return stack > 1 ? $"[i/s{stack}:{type}]" : $"[i:{type}]";
	}
}
