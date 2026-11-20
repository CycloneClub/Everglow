using Terraria.Localization;

namespace Everglow.Commons.Templates;

public abstract class BannerItemBase : ModItem
{
	public override string Texture => ModAsset.BannerItemBase_Mod;

	public virtual int BannerTileType => ModContent.TileType<BannerTile>();

	public virtual int BannerTileStyle => 0;

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
		Item.DefaultToPlaceableTile(BannerTileType, BannerTileStyle);
		Item.width = 10;
		Item.height = 24;
		Item.value = Item.sellPrice(silver: 2);
		Item.rare = ItemRarityID.Blue;
	}
}
