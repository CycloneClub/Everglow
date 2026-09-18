using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class VitalizedRocksStone : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.width = 32;
		Projectile.height = 32;
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
			Projectile.ai[0] += 3f;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.ai[1] = Main.rand.NextFloat(-0.3f, 0.3f);
		}
		else
		{
			Projectile.rotation += Projectile.ai[1];
		}
		float value = (Projectile.timeLeft - 540) / 30f;
		var d = Dust.NewDustDirect(Projectile.position - Projectile.velocity * 2, Projectile.width, Projectile.height, ModContent.DustType<RockElemental_Energy_normal>());
		d.velocity = Projectile.velocity * 0.5f;
		d.scale = Main.rand.NextFloat(0.75f, 1f) * Math.Min(value, 1);
		d.noGravity = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		var Portal = new RockPortal
		{
			velocity = Vector2.Zero,
			Active = true,
			Visible = true,
			position = Projectile.Center,
			maxTime = Main.rand.Next(12, 30),
			scale = Main.rand.NextFloat(30, 54),
			maxScale = 160,
			rotation = Projectile.rotation,
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
		for (int x = 0; x < 8; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(7f, 16f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 16; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.6f, 1.1f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(2f, 11f)).RotatedByRandom(6.283);
		}
		GenerateSmog(12);
		ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 1, 30, 120);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<VitalizedRocksProj_Explosion>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner, 1f);
		switch (Main.rand.Next(2))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_hit_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.vitalizedrocks_hit_2_Mod), Projectile.Center);
				break;
		}
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 12f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(37, 75),
				scale = Main.rand.NextFloat(40f, 55f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Vector2 pos = Projectile.position - Main.screenPosition;
		Main.spriteBatch.Draw(tex, new Rectangle((int)pos.X + 18, (int)pos.Y + 18, (int)Projectile.ai[0], 36),
							   new Rectangle((int)(36 - Projectile.ai[0]), 0, (int)Projectile.ai[0], 36), lightColor, Projectile.rotation, new Vector2(18, 18), 0, 0);
		return false;
	}
}
