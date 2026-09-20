using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

public class CrimsonMoonAlgaeBreastPlate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Body, this, nameof(CrimsonMoonAlgaeBreastPlate));
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.defense = 16;
		Item.value = 72000;
		Item.rare = ItemRarityID.Orange;

		Item.bodySlot = EquipLoader.GetEquipSlot(Mod, nameof(CrimsonMoonAlgaeBreastPlate), EquipType.Body);
	}

	public override void UpdateEquip(Player player)
	{
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeBreastPlate = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
