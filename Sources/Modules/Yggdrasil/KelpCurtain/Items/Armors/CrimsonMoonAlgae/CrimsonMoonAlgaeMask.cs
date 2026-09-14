using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

/// <summary>
/// 红月水藻面具 - the summoner helm of the 红月水藻 four-piece set. Approved artwork
/// is not in the repository, so it reuses the shared fallback texture and registers
/// its head equip slot explicitly against that same fallback (D-13/D-14).
/// </summary>
public class CrimsonMoonAlgaeMask : ModItem
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

		// Register the head equip slot explicitly against the existing shared fallback
		// texture. Do NOT add the autoload-equip attribute to this class: with no
		// CrimsonMoonAlgaeMask_Head.png beside this file, that autoload path requests a
		// missing asset and aborts mod loading (02-RESEARCH.md Pitfall 1).
		EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Head, this, nameof(CrimsonMoonAlgaeMask));
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.defense = 11;
		Item.value = 54000;
		Item.rare = ItemRarityID.Orange;

		Item.headSlot = EquipLoader.GetEquipSlot(Mod, nameof(CrimsonMoonAlgaeMask), EquipType.Head);
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<SummonDamageClass>() += 0.18f; // Design: 召唤伤害+18%
		player.maxMinions += 3; // Design: 召唤上限+3
	}

	/// <summary>
	/// Either 红月水藻 head piece (this mask or the headdress) completes the set with
	/// the breastplate and greaves.
	/// </summary>
	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return (head.type == ModContent.ItemType<CrimsonMoonAlgaeHeaddress>() || head.type == ModContent.ItemType<CrimsonMoonAlgaeMask>())
			&& body.type == ModContent.ItemType<CrimsonMoonAlgaeBreastPlate>()
			&& legs.type == ModContent.ItemType<CrimsonMoonAlgaeGreaves>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeSetBuff = true;
		player.statManaMax2 += 50; // Design: 法力上限+50
		player.buffImmune[ModContent.BuffType<RedAlgaeDebuff>()] = true; // Design: 免疫和红藻相关的所有伤害
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
