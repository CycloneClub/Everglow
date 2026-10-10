using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class ScarpasScissors : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 52;
		Item.height = 52;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 17;
		Item.knockBack = 3.5f;
		Item.crit = 8;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = Item.useAnimation = 16;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 3);

		Item.shoot = ModContent.ProjectileType<ScarpasScissorsProj>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse != 2)
		{
			// Left click: swing attack
			SoundEngine.PlaySound(SoundID.Item1, position);
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
		}
		else
		{
			// Right click: cut attack
			if (player.direction <= 0)
			{
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScarpasScissorsRight>(), damage, knockback, player.whoAmI);
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScarpasScissorsLeft>(), damage, knockback, player.whoAmI);
			}
			else
			{
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScarpasScissorsLeft>(), damage, knockback, player.whoAmI);
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScarpasScissorsRight>(), damage, knockback, player.whoAmI);
			}
		}
		return false;
	}

	public override bool MeleePrefix() => true;

	public override bool AltFunctionUse(Player player) => true;

	public abstract class ScarpasScissorsProjBase : ModProjectile
	{
		public override string Texture => ModAsset.ScarpasScissors_Mod;

		public Player Owner => Main.player[Projectile.owner];

		public sealed override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// + 200% Critical Damage
			modifiers.CritDamage += 2f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (!target.active)
			{
				// TODO: Replace type with mission item
				// Drop special mission items when npc is killed
				Owner.QuickSpawnItem(target.GetSource_Loot(), ItemID.DirtBlock, Main.rand.Next(10, 20));
			}
		}
	}
}
