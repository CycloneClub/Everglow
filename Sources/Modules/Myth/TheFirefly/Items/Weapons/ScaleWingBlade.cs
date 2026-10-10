using Everglow.Myth.TheFirefly.Items.Accessories;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class ScaleWingBlade : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	private FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();

	// MothEye mothEye = ModContent.GetInstance<MothEye>();
	public Player owner;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 1;
		Item.height = 1;
		Item.useAnimation = 5;
		Item.useTime = 5;
		Item.shootSpeed = 5f;
		Item.knockBack = 2.5f;
		Item.damage = 30;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.value = 2400;
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse != 2)
				{
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ScaleWingBladeProj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
				}
				else// 右键
				{
					var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ScaleWingBladeProj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					(proj.ModProjectile as MeleeProj).currantAttackType = 100;
					(proj.ModProjectile as MeleeProj).isRightClick = true;
					proj.netUpdate = true;
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	// public override void UpdateInventory(Player player) //KEEP FOR REFERENCE
	// {
	//    owner = player;
	//    //owner.GetModPlayer<MothEyePlayer>().MothEyeEquipped = true;
	//    if (owner != null && owner.TryGetModPlayer(out MothEyePlayer mothEyePlayer) && fireflyBiome.IsBiomeActive(Main.LocalPlayer))
	//    {
	//        owner.statDefense = 999; //used as an example
	//    }
	//    base.UpdateInventory(player);
	// }
	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		// Main.NewText(owner != null); // DEBUGGING PURPOSES
		// Console.WriteLine(owner != null); // DEBUGGING PURPOSES
		// Main.NewText(LocalOwner != null); // DEBUGGING PURPOSES
		// Console.WriteLine(LocalOwner != null); // DEBUGGING PURPOSES
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				tooltips.AddRange(new TooltipLine[]
				{
					new(ModIns.Mod, "MothEyeBonusText", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MothEyeBonusText")),
					new(ModIns.Mod, "MothEyeBladeBonus", Language.GetTextValue("Mods.Everglow.ExtraTooltip.FireflyItems.MEyeBonusTextMothBlade")),
				});
			}
		}
	}
}
