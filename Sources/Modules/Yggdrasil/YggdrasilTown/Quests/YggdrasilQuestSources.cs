using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public static class YggdrasilQuestSources
{
	public static readonly QuestSourceBase Guard = new GuardSource();
	public static readonly QuestSourceBase Warden = new WardenSource();
	public static readonly QuestSourceBase Restauranteur = new RestauranteurSource();
	public static readonly QuestSourceBase CanteenMaid = new CanteenMaidSource();
	public static readonly QuestSourceBase TeahouseLady = new TeahouseLadySource();
	public static readonly QuestSourceBase InnKeeper = new InnKeeperSource();

	private sealed class GuardSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Guard_Of_YggdrasilTown_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<Guard_Of_YggdrasilTown>());
	}

	private sealed class WardenSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Howard_Warden_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<Howard_Warden>());
	}

	private sealed class RestauranteurSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.Restauranteur_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<Restauranteur>());
	}

	private sealed class CanteenMaidSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.CanteenMaid_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<CanteenMaid>());
	}

	private sealed class TeahouseLadySource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.TeahouseLady_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<TeahouseLady>());
	}

	private sealed class InnKeeperSource : QuestSourceBase
	{
		public override Texture2D Texture => ModContent.Request<Texture2D>(ModAsset.InnKeeper_Head_Mod).Value;

		public override string Name => Lang.GetNPCNameValue(ModContent.NPCType<InnKeeper>());
	}
}
