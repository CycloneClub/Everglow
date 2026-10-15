using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.GameContent.Creative;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Armors.CrimsonMoonAlgae;

public class CrimsonMoonAlgaeMask : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override string Texture => $"Terraria/Images/Item_{ItemID.ChlorophyteMask}";

	public override void Load()
	{
		if (Main.dedServ)
		{
			return;
		}

		EquipLoader.AddEquipTexture(Mod, $"Terraria/Images/Armor_Head_{ArmorIDs.Head.ChlorophyteMask}", EquipType.Head, this, nameof(CrimsonMoonAlgaeMask));
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
		player.GetDamage<SummonDamageClass>() += 0.18f;
		player.maxMinions += 3;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return (head.type == ModContent.ItemType<CrimsonMoonAlgaeHeaddress>() || head.type == ModContent.ItemType<CrimsonMoonAlgaeMask>())
			&& body.type == ModContent.ItemType<CrimsonMoonAlgaeBreastPlate>()
			&& legs.type == ModContent.ItemType<CrimsonMoonAlgaeGreaves>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.GetModPlayer<KelpCurtainPlayer>().CrimsonMoonAlgaeSetBuff = true;
		player.statManaMax2 += 50;
		player.buffImmune[ModContent.BuffType<RedAlgaeDebuff>()] = true;
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
