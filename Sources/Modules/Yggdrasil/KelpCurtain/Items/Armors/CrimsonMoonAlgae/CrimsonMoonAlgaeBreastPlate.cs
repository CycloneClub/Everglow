using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

/// <summary>
/// 红月水藻板甲 - the body piece of the 红月水藻 four-piece set. Approved artwork is
/// not in the repository, so it reuses the shared fallback texture and registers its
/// body equip slot explicitly against that same fallback (D-13/D-14). Its damage
/// clause is consumed by <see cref="KelpCurtainPlayer.OnHurt"/>.
/// </summary>
public class CrimsonMoonAlgaeBreastPlate : ModItem
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

		// Register the body equip slot explicitly against the existing shared fallback
		// texture. Do NOT add the autoload-equip attribute to this class: with no
		// CrimsonMoonAlgaeBreastPlate_Body.png beside this file, that autoload path requests
		// a missing asset and aborts mod loading (02-RESEARCH.md Pitfall 1).
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
		// Design: 受到大于等于10的伤害时治疗该伤害的15% - applied in KelpCurtainPlayer.OnHurt.
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeBreastPlate = true;
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
