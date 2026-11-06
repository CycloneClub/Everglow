using Everglow.Myth.Common;
using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Furnitures;

public class GlowWoodMusicBox : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		ItemID.Sets.CanGetPrefixes[Type] = false;
		ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
		MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(ModIns.Mod, ModAsset.MothBiome_Path), ModContent.ItemType<GlowWoodMusicBox>(), ModContent.TileType<Tiles.Furnitures.GlowWoodMusicBox>());
	}

	public override void SetDefaults()
	{
		Item.DefaultToMusicBox(ModContent.TileType<Tiles.Furnitures.GlowWoodMusicBox>(), 0);
		Item.width = 28;
		Item.height = 30;
	}
}
