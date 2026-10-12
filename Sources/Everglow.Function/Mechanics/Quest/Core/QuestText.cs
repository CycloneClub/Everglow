using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.Core;

public static class QuestText
{
	public static string Get(string key, params object[] args) =>
		Language.GetTextValue("Mods.Everglow.QuestSystem." + key, args);
}
