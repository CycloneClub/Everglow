using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

/// <summary>
/// 红月水藻头饰 - the magic helm of the 红月水藻 four-piece set. Its approved
/// artwork is not in the repository, so it reuses the shared fallback texture for
/// the inventory icon and registers its head equip slot explicitly against that
/// same fallback (D-13/D-14).
/// </summary>
public class CrimsonMoonAlgaeHeaddress : ModItem
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
		// CrimsonMoonAlgaeHeaddress_Head.png beside this file, that autoload path requests a
		// missing asset and aborts mod loading (02-RESEARCH.md Pitfall 1).
		EquipLoader.AddEquipTexture(Mod, Commons.ModAsset.White_Mod, EquipType.Head, this, nameof(CrimsonMoonAlgaeHeaddress));
	}

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.defense = 8;
		Item.value = 54000;
		Item.rare = ItemRarityID.Orange;

		// Assign the slot from the name registered in Load(); this makes the assignment
		// deterministic regardless of whether the registration already populated it.
		Item.headSlot = EquipLoader.GetEquipSlot(Mod, nameof(CrimsonMoonAlgaeHeaddress), EquipType.Head);
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage<MagicDamageClass>() += 0.18f; // Design: 魔法伤害+18%
		player.manaCost -= 0.12f; // Design: 法力消耗-12%
	}

	// D-14 migration path: the item identity never depends on the fallback. When approved
	// art arrives, add CrimsonMoonAlgaeHeaddress.png and CrimsonMoonAlgaeHeaddress_Head.png
	// beside this .cs and repoint the Load() registration at the class's own asset path.
	// That is a texture addition plus a blocker removal, not a class rework.
}
