using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

/// <summary>
/// 红月水藻护胫 - the legs piece of the 红月水藻 four-piece set. Approved artwork is
/// not in the repository, so it reuses the shared fallback texture and registers its
/// legs equip slot explicitly against that same fallback (D-13/D-14). Its wet
/// movement clause is consumed by <see cref="KelpCurtainPlayer.UpdateEquips"/>.
/// </summary>
public class CrimsonMoonAlgaeGreaves : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		// Register the legs equip slot explicitly against the existing shared fallback
		// texture. Do NOT add the autoload-equip attribute to this class: with no
		// CrimsonMoonAlgaeGreaves_Legs.png beside this file, that autoload path requests a
		// missing asset and aborts mod loading (02-RESEARCH.md Pitfall 1).
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
		player.moveSpeed += 0.12f; // Design: 速度增加12%
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeGreaves = true; // Design: 水下额外增加24% (KelpCurtainPlayer.UpdateEquips)
	}

	public override void AddRecipes()
	{
		// The design supplies no recipe for the 红月水藻 set; mirror the accepted
		// red-algae family recipe already used by RedAlgaeMinionStaff.AddRecipes.
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
