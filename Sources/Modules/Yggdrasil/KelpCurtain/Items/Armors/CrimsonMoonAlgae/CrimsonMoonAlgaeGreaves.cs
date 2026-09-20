using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

public class CrimsonMoonAlgaeGreaves : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Legs, this, nameof(CrimsonMoonAlgaeGreaves));
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.defense = 4;
		Item.value = 48000;
		Item.rare = ItemRarityID.Orange;

		Item.legSlot = EquipLoader.GetEquipSlot(Mod, nameof(CrimsonMoonAlgaeGreaves), EquipType.Legs);
	}

	public override void UpdateEquip(Player player)
	{
		player.moveSpeed += 0.12f;
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeGreaves = true;
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
