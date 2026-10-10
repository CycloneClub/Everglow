using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class VitalizedRocksStone_small : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.width = 12;
		Projectile.height = 12;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 1023;
	}

	public override void AI()
	{
		if (Projectile.ai[0] < 36)
		{
			Projectile.ai[0] += 12f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.ai[1] = Main.rand.NextFloat(-0.3f, 0.3f);
		}
		else
		{
			Projectile.rotation += Projectile.ai[1];
		}
		if (Main.rand.NextBool(4))
		{
			float value = (Projectile.timeLeft - 540) / 30f;
			var d = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 1, Projectile.width, Projectile.height, ModContent.DustType<RockElemental_Energy_normal>());
			d.velocity = Projectile.velocity * 0.5f;
			d.scale = Main.rand.NextFloat(0.45f, 1f) * Math.Min(value, 1);
			d.noGravity = true;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		var Portal = new RockPortal
		{
			Velocity = Vector2.Zero,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(12, 30),
			Scale = Main.rand.NextFloat(10, 24),
			MaxScale = 80,
			Rotation = Projectile.rotation,
			ai = new float[] { 1 },
		};
		Ins.VFXManager.Add(Portal);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.BrokenArmor, 180);
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 4; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.5f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(3f, 8f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 8; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.3f, 0.8f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283);
		}
		GenerateSmog(4);
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithVolume(0.5f), Projectile.Center);
		ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 1, 30, 120);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(20f, 35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Vector2 pos = Projectile.position - Main.screenPosition;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, 1f, 0, 0);
		return false;
	}
}
