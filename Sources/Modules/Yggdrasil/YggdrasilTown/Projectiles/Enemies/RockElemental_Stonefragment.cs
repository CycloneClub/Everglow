using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class RockElemental_Stonefragment : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = 0;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 600;
	}

	public override void AI()
	{
		if (Projectile.velocity.Y <= 12)
		{
			Projectile.velocity.Y += 0.2f;
		}
		if (Projectile.timeLeft == 597)
		{
			Projectile.tileCollide = true;
		}
		Projectile.rotation += Projectile.ai[0];
		if (Projectile.timeLeft > 540)
		{
			float value = (Projectile.timeLeft - 540) / 30f;
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RockElemental_Energy_normal>());
			d.velocity = Projectile.velocity * 0.5f;
			d.scale = Main.rand.NextFloat(0.75f, 1f) * Math.Min(value, 1);
			d.noGravity = true;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.scale = Main.rand.NextFloat(0.75f, 1f);
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.15f, 0.15f);
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 3; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(0.1f, 1.6f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 5; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 0.6f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(0.7f, 4.1f)).RotatedByRandom(6.283);
		}
		GenerateSmog(3);
		SoundEngine.PlaySound(SoundID.NPCHit2.WithVolume(0.3f), Projectile.Center);
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
				Scale = Main.rand.NextFloat(10f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D texture = ModAsset.RockElemental_Stonefragment.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		if (Projectile.timeLeft > 540)
		{
			float value = (Projectile.timeLeft - 540) / 60f;
			for (int i = 0; i < 7; i++)
			{
				Main.EntitySpriteDraw(texture, drawCenter, null, new Color(0.7f, 0.4f, 1f, 0f) * value * 0.4f, Projectile.rotation + i, texture.Size() * 0.5f, Projectile.scale * (1.25f + value), SpriteEffects.None, 0);
			}
		}
		Main.EntitySpriteDraw(texture, drawCenter, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
