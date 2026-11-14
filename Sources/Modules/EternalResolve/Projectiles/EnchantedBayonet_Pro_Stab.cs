using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Everglow.EternalResolve.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class EnchantedBayonet_Pro_Stab : StabbingProjectile_Stab
	{
		public override void SetCustomDefaults()
		{
			StabColor = new Color(76, 126, 255);
			StabShade = 0.2f;
			StabDistance = 0.88f;
			StabEffectWidth = 0.4f;
		}

		public override void HitTile()
		{
			SoundStyle ss = SoundID.NPCHit4;
			SoundEngine.PlaySound(ss.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
			for (int g = 0; g < 20; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new Spark_EnchantedStabDust
				{
					velocity = newVelocity,
					Active = true,
					Visible = true,
					position = StabEndPoint_WorldPos,
					maxTime = Main.rand.Next(1, 25),
					scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
					rotation = Main.rand.NextFloat(6.283f),
					noGravity = false,
					ai = new float[] { Main.rand.Next(3), Main.rand.NextFloat(-0.13f, 0.13f) },
				};
				Ins.VFXManager.Add(spark);
			}
			var hitSparkFixed = new StabbingProjectile_HitEffect()
			{
				Active = true,
				Visible = true,
				Position = StabEndPoint_WorldPos,
				MaxTime = 16,
				Scale = 0.24f,
				Rotation = Projectile.velocity.ToRotation(),
				Color = HitTileSparkColor,
			};
			Ins.VFXManager.Add(hitSparkFixed);

			Vector2 tilePos = StabEndPoint_WorldPos + new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation());
			Point tileCoord = tilePos.ToTileCoordinates();
			Tile tile = WorldGenMisc.SafeGetTile(tileCoord);
			if (TileUtils.Sets.TileFragile[tile.TileType])
			{
				WorldGenMisc.DamageTile(tileCoord, 100);
			}
		}

		public override void DrawEffect(Color lightColor)
		{
			base.DrawEffect(lightColor);
		}

		public override void AI()
		{
			if (Main.rand.NextBool(6))
			{
				Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
				if (StabEndPoint_WorldPos != Vector2.zeroVector)
				{
					end = StabEndPoint_WorldPos;
				}
				int type = ModContent.DustType<EnchantedDustMoveWithPlayer>();
				var dust = Dust.NewDustDirect(Vector2.Lerp(StabStartPoint_WorldPos, end, Main.rand.NextFloat(0.3f, 1f)) - new Vector2(4), 0, 0, type, 0, 0, 0, default, Main.rand.NextFloat(0.45f, 1.1f));
				dust.noGravity = true;
				dust.velocity = new Vector2(0, Main.rand.NextFloat(6f)).RotateRandom(6.283);
			}
			if (StabTimer == 90 && Projectile.velocity.Length() > 0.9f)
			{
				Projectile.NewProjectileDirect(
					Projectile.GetSource_FromAI(),
					Projectile.Center + Projectile.velocity * 150f,
					Projectile.velocity * 27, ModContent.ProjectileType<EnchantedBayonet_Beam>(),
					Projectile.damage, Projectile.knockBack * 0.6f, Projectile.owner, -1);
			}
			base.AI();
		}

		public override void OnSpawn(IEntitySource source)
		{
			base.OnSpawn(source);
		}
	}
}
