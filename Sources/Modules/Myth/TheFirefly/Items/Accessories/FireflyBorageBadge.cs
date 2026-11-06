using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Myth.TheFirefly.Items.Materials;
using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Accessories;

public class FireflyBorageBadge : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 10, 20, 10, 12, 8, 10);

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 22;
		Item.value = 3868;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// +5% multiplicative magic damage
		player.GetDamage(DamageClass.Magic) *= 1.05f;
		// +10% movement speed
		player.moveSpeed += 0.1f;
		if (CorruptMoth.CorruptMothNPC != null && CorruptMoth.CorruptMothNPC.active)
		{
			player.GetModPlayer<StatBonusPlayer>().FireflyBorageBadgeAgainstMoth = true;
			// +12% multiplicative damage
			player.GetDamage(DamageClass.Generic) *= 1.12f;
			// +8% melee and whip attack speed (whips inherit melee speed).
			player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
			// -10% melee critical strike chance (MeleeNoSpeed inherits melee crit).
			player.GetCritChance(DamageClass.Melee) -= 10;
		}
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(ModIns.Mod, "UnfinishedItem", Language.GetTextValue("Mods.Everglow.ExtraTooltip.Misc.UnfinishedItem")));
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			// .AddIngredient(ModContent.ItemType<Everglow.PlantAndFarm.Items.Materials.WindMoveSeed>(), 8) // 15
			.AddIngredient(ModContent.ItemType<BlackStarShrub>(), 24)
			.AddIngredient(ModContent.ItemType<GlowingPetal>(), 6)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
