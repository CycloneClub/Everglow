using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;

[AutoloadEquip(EquipType.Head)]
public class EldamberWarhelm : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(4);

	public LocalizedText SetBonusText => this.GetLocalization("SetBonus").WithFormatArgs(15);

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 24;
		Item.maxStack = 1;
		Item.defense = 7;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 30);
	}

	public override void UpdateEquip(Player player)
	{
		player.GetDamage(DamageClass.Melee) += 0.04f;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<EldamberBreastplate>() && legs.type == ModContent.ItemType<EldamberBoots>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = SetBonusText.Value;
		player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
	}

	public override void AddRecipes()
	{
		// Provisional: use the existing helmet recipe until the warrior recipe is finalized.
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<YggdrasilAmber>(12);
		recipe.AddIngredient<CrystalNucleusOfJellyKing>(3);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
