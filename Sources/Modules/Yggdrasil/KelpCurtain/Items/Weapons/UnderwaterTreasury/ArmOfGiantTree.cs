using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;
using Everglow.Yggdrasil.Netcode;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

public class ArmOfGiantTree : ModItem
{
	public const int MaxChargeFrames = ArmOfGiantTreeAttackState.ChargeDuration;

	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 62;
		Item.damage = 55;
		Item.DamageType = DamageClass.Melee;
		Item.knockBack = 7.25f;
		Item.useTime = Item.useAnimation = 2;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.channel = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<ArmOfGiantTreeHeld>();
		Item.shootSpeed = 1;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 2);
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player) => player.GetModPlayer<KelpCurtainPlayer>().ArmOfGiantTreeRequestWait == 0
		&& player.ownedProjectileCounts[ModContent.ProjectileType<ArmOfGiantTreeHeld>()] == 0;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			float angle = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX * player.direction).ToRotation();
			player.GetModPlayer<KelpCurtainPlayer>().ArmOfGiantTreeRequestWait = 60;
			ArmOfGiantTreeChargePacket.Request(player, false, player.altFunctionUse == 2, angle);
		}
		return false;
	}
}
