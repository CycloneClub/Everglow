using Terraria.Localization;

namespace Everglow.Commons.Templates;

public abstract class BannerItemBase<TBannerTile> : ModItem
	where TBannerTile : ModBannerTile
{
	public abstract int BannerTileStyle { get; }

	public abstract int BannerNPCType { get; }

	public virtual int BannerKillRequirement => ItemID.Sets.DefaultKillsForBannerNeeded;

	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.Placeables;

	public virtual LocalizedText NPCName => NPCLoader.GetNPC(BannerNPCType).DisplayName;

	public override LocalizedText DisplayName => Language
		.GetOrRegister("Mods.Everglow.Common.BannerNameFormat", () => "{0} Banner")
		.WithFormatArgs(NPCName);

	public override void SetStaticDefaults()
	{
		ItemID.Sets.KillsToBanner[Type] = BannerKillRequirement;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<TBannerTile>(), BannerTileStyle);
		Item.width = 10;
		Item.height = 24;
		Item.value = Item.sellPrice(silver: 2);
		Item.rare = ItemRarityID.Blue;
	}
}
