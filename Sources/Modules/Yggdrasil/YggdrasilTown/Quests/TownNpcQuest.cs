using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// 只复用显示与世界前置；剧情推进直接组合现有目标。
public abstract class TownNpcQuest : WorldQuestBase
{
	public static readonly QuestSourceBase Anna = new AnnaSource();
	public static readonly QuestSourceBase Howard = new HowardSource();
	public static readonly QuestSourceBase Rolle = new RolleSource();
	public static readonly QuestSourceBase Betty = new BettySource();
	public static readonly QuestSourceBase Schorl = new SchorlSource();
	public static readonly QuestSourceBase Georg = new GeorgSource();

	public static string Text(string key, params object[] args) =>
		Language.GetTextValue("Mods.Everglow.TownQuests." + key, args);

	public abstract int GiverNpcType { get; }

	public override string LocalizationKey => "Mods.Everglow.TownQuests." + Name;

	public override string DisplayName => Text(Name + ".Name");

	public override string Description => Text(Name + ".Description");

	public override string Hint => Text(Name + ".Hint");

	public override QuestType Type => QuestType.SideStory;

	public override bool CanUnlock() => CanOffer(WorldQuestManager.Instance);

	public abstract bool CanOffer(WorldQuestManager manager);

	private sealed class AnnaSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Guard_of_YggdrasilTown_Head_Mod).Value;

		public override string Name => Text("Anna");
	}

	private sealed class HowardSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Howard_Warden_Head_Mod).Value;

		public override string Name => Text("Howard");
	}

	private sealed class RolleSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Restauranteur_Head_Mod).Value;

		public override string Name => Text("Rolle");
	}

	private sealed class BettySource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.CanteenMaid_Head_Mod).Value;

		public override string Name => Text("Betty");
	}

	private sealed class SchorlSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.TeahouseLady_Head_Mod).Value;

		public override string Name => Text("Schorl");
	}

	private sealed class GeorgSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.InnKeeper_Head_Mod).Value;

		public override string Name => Text("Georg");
	}
}
