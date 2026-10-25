using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Valiant;

[AutoloadEquip(EquipType.Head)]
public class ValiantHelmet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Armor;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 400, 2, 400);

	public LocalizedText SetBonusText => this.GetLocalization("SetBonus").WithFormatArgs(5, 400, 2, 400);

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 26;
		Item.value = 2500;
		Item.rare = ItemRarityID.Green;
		Item.defense = 3;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs)
	{
		return body.type == ModContent.ItemType<ValiantBreastplate>() && legs.type == ModContent.ItemType<ValiantLeggings>();
	}

	public override void UpdateArmorSet(Player player)
	{
		player.setBonus = SetBonusText.Value;
		player.aggro -= 400;
		player.statDefense += 2;
		player.endurance += 4f;
	}

	public override void UpdateEquip(Player player)
	{
		ValiantHelmetPlayer ValiantHelmetPlayer = player.GetModPlayer<ValiantHelmetPlayer>();
		ValiantHelmetPlayer.hasValiantHelmet = true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();

		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}

public class ValiantHelmetPlayer : ModPlayer
{
	public bool hasValiantHelmet;

	public override void ResetEffects()
	{
		hasValiantHelmet = false;
	}

	public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
	{
		if (hasValiantHelmet &&
			(Player.GetModPlayer<MEACPlayer>().isUsingMeleeProj ||
			(Player.heldProj > 0 && Main.projectile[Player.heldProj].DamageType == DamageClass.Melee)))
		{
			modifiers.SourceDamage *= 1.05f;
		}
	}

	public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
	{
		if (hasValiantHelmet)
		{
			modifiers.SourceDamage *= 1.05f;
		}
	}
}
