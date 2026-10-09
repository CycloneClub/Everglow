using Everglow.SpellAndSkull.Buffs;
using Everglow.SpellAndSkull.Projectiles;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

internal class WaterBoltBook : MagicBookProjectile
{
	public override void SetDef()
	{
		DustType = DustID.WaterCandle;
		ItemType = ItemID.WaterBolt;
		effectColor = new Color(30, 60, 225, 100);

		TexCoordTop = new Vector2(6, 0);
		TexCoordLeft = new Vector2(0, 24);
		TexCoordDown = new Vector2(22, 24);
		TexCoordRight = new Vector2(28, 0);
	}

	public override void OnSpawn(IEntitySource source)
	{
		FrontTexture = ModAsset.WaterBolt_A.Value;
		PaperTexture = ModAsset.WaterBolt_C.Value;
		BackTexture = ModAsset.WaterBolt_B.Value;
		GlowTexture = ModAsset.WaterBolt_E.Value;
		base.OnSpawn(source);
	}

	public override void SpecialAI()
	{
		Player player = Main.player[Projectile.owner];
		int damage = player.HeldItem.damage;
		if (player.itemTime == 2)
		{
			Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * player.HeldItem.shootSpeed;
			int T = ProjectileID.WaterBolt;
			if (player.HasBuff(ModContent.BuffType<WaterBoltII>()))
			{
				damage = (int)(damage * 1.85);
				T = ModContent.ProjectileType<NewWaterBolt>();
			}
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity * 6, velocity, T, damage, player.HeldItem.knockBack, player.whoAmI);
			p.penetrate = 2;
			p.CritChance = player.GetWeaponCrit(player.HeldItem);
		}
	}
}
