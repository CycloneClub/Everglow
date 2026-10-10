using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class YggdrasilSlashBall : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 1;
		Projectile.height = 1;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 2;
	}

	public Vector2 NormalDistance;

	public override void OnSpawn(IEntitySource source)
	{
		NormalDistance = new Vector2(45, 0);
		Projectile.scale = 0;
		base.OnSpawn(source);
	}

	public void GenerateSpark()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1.0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
		var spark = new Spark_MoonBladeDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(7, 45),
			Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 7.0f)),
			Rotation = Main.rand.NextFloat(6.283f),
			noGravity = true,
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
		};
		Ins.VFXManager.Add(spark);
	}

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, 0.14f, 0.47f, 0.97f);
		Player player = Main.player[Projectile.owner];
		if (player.controlUseItem && player.CheckMana(player.HeldItem, player.HeldItem.mana, false) && player.HeldItem.type == ModContent.ItemType<FlurryingBlades>())
		{
			if (Projectile.scale < 1f)
			{
				Projectile.scale += 0.01f;
				Projectile.timeLeft = (int)(Projectile.scale * 100 + 1);
			}
			else
			{
				Projectile.timeLeft = 100;
			}
		}
		else
		{
			Projectile.scale = Projectile.timeLeft / 100f;
		}
		if (Projectile.scale > 1f)
		{
			Projectile.scale = 1;
		}
		NormalDistance = (player.Center - player.MouseWorld()).SafeNormalize(Vector2.zeroVector);

		Projectile.Center = player.Center - NormalDistance * 100;
		float timeValue = (float)Main.time * 0.02f + Projectile.whoAmI;
		for (int i = 0; i < 8; i++)
		{
			float rot = timeValue * (30 - i * 2);
			float theta = i + timeValue;
			float phi = MathF.Sin(i * timeValue * 0.06f) * 2;
			float size = (MathF.Cos(theta - timeValue * 0.43f) + 2f) / 3f;

			for (int x = 0; x <= 5; x++)
			{
				Vector2 v0 = new Vector2(0, 50f + i * 2).RotatedBy(x / 2f + rot) * Projectile.scale;
				Vector2 newVelocity = v0.RotatedBy(MathHelper.PiOver2);
				v0.X *= MathF.Cos(theta);
				newVelocity.X *= MathF.Cos(theta);
				v0 = v0.RotatedBy(phi);
				newVelocity = newVelocity.RotatedBy(phi);
				v0 *= size;
				newVelocity *= size;
				newVelocity *= 0.08f * Main.rand.NextFloat(1f);
				if (Main.rand.NextBool(2))
				{
					var spark = new Spark_MoonBladeDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = Projectile.Center + v0 - newVelocity * 3,
						MaxTime = Main.rand.Next(3, 15),
						Scale = Main.rand.NextFloat(1f, Main.rand.NextFloat(2f, 17f)) * Projectile.scale,
						Rotation = Main.rand.NextFloat(6.283f),
						noGravity = true,
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
					};
					Ins.VFXManager.Add(spark);
				}
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<WindBladeBallHit>(), 0, 0, -1, 6 * Projectile.scale, (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(-1, 1));
		SoundEngine.PlaySound(new SoundStyle(ModAsset.Knife_Mod).WithPitchOffset(Main.rand.NextFloat(0.4f, 0.8f)).WithVolume(0.3f) with { MaxInstances = 5 }, Projectile.Center);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D dark = Commons.ModAsset.Point_black.Value;
		Vector2 pos = Projectile.Center - Main.screenPosition;

		float timeValue = (float)Main.time * 0.02f + Projectile.whoAmI;
		var bars = new List<Vertex2D>();
		for (int i = 0; i < 8; i++)
		{
			float rot = timeValue * (30 - i * 2);
			float theta = i;
			float phi = MathF.Sin(i * timeValue * 0.06f) * 2;
			float size = (MathF.Cos(theta - timeValue * 0.43f) + 2f) / 3f;
			Vector2 v2 = new Vector2(0, 60f).RotatedBy(rot) * Projectile.scale;
			v2.X *= MathF.Cos(theta);
			v2 = v2.RotatedBy(phi);
			bars.Add(pos + v2 * 0.5f * size, new Color(0, 0, 0, 0), new Vector3(0.42f, 0, 0));
			bars.Add(pos + v2 * size, new Color(0, 0, 0, 0), new Vector3(0.5f, 0, 0));
			for (int x = 0; x <= 20; x++)
			{
				Vector2 v0 = new Vector2(0, 50f + i * 2).RotatedBy(x / 8f + rot) * Projectile.scale;
				v0.X *= MathF.Cos(theta);
				v0 = v0.RotatedBy(phi);
				float amount = (x - 10f) / 10f;
				amount *= amount;
				bars.Add(pos + v0 * (0.5f + amount / 2f) * size, new Color(255, 255, 255, 255), new Vector3(0.42f, x / 20f, 0));
				bars.Add(pos + v0 * size, new Color(255, 255, 255, 255), new Vector3(0.5f, x / 20f, 0));
			}
			v2 = new Vector2(0, 60f).RotatedBy(2.5 + rot) * Projectile.scale;
			v2.X *= MathF.Cos(theta);
			v2 = v2.RotatedBy(phi);
			bars.Add(pos + v2 * 0.5f * size, new Color(0, 0, 0, 0), new Vector3(0.42f, 1f, 0));
			bars.Add(pos + v2 * size, new Color(0, 0, 0, 0), new Vector3(0.5f, 1f, 0));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		float range = 60;
		bars = new List<Vertex2D>();
		bars.Add(pos + new Vector2(-range, -range) * Projectile.scale, Color.White, new Vector3(0, 0, 0));
		bars.Add(pos + new Vector2(range, -range) * Projectile.scale, Color.White, new Vector3(1, 0, 0));
		bars.Add(pos + new Vector2(-range, range) * Projectile.scale, Color.White, new Vector3(0, 1, 0));
		bars.Add(pos + new Vector2(range, range) * Projectile.scale, Color.White, new Vector3(1, 1, 0));
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = dark;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = new List<Vertex2D>();
		for (int i = 0; i < 8; i++)
		{
			float rot = timeValue * (30 - i * 2);
			float theta = i;
			float phi = MathF.Sin(i * timeValue * 0.06f) * 2;
			float size = (MathF.Cos(theta - timeValue * 0.43f) + 2f) / 3f;
			Vector2 v2 = new Vector2(0, 60f).RotatedBy(rot) * Projectile.scale;
			v2.X *= MathF.Cos(theta);
			v2 = v2.RotatedBy(phi);
			bars.Add(pos + v2 * 0.5f * size, new Color(0, 0, 0, 0), new Vector3(0.42f, 0, 0));
			bars.Add(pos + v2 * size, new Color(0, 0, 0, 0), new Vector3(0.42f, 0, 0));
			for (int x = 0; x <= 20; x++)
			{
				Vector2 v0 = new Vector2(0, 50f + i * 2).RotatedBy(x / 8f + rot) * Projectile.scale;
				v0.X *= MathF.Cos(theta);
				v0 = v0.RotatedBy(phi);
				float amount = (x - 10f) / 10f;
				amount *= amount;
				bars.Add(pos + v0 * (0.5f + amount / 2f) * size, new Color(0, 12, 150, 0), new Vector3(0.42f, x / 20f, 0));
				bars.Add(pos + v0 * size, Color.Lerp(new Color(81, 243, 255, 0), new Color(0, 120, 250, 0), amount), new Vector3(0.5f, x / 20f, 0));
			}
			v2 = new Vector2(0, 60f).RotatedBy(2.5 + rot) * Projectile.scale;
			v2.X *= MathF.Cos(theta);
			v2 = v2.RotatedBy(phi);
			bars.Add(pos + v2 * 0.5f * size, new Color(0, 0, 0, 0), new Vector3(0.42f, 1, 0));
			bars.Add(pos + v2 * size, new Color(0, 0, 0, 0), new Vector3(0.5f, 1, 0));
		}
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		// int maxLength = 280 - Projectile.timeLeft;
		// if (Projectile.timeLeft < 260)
		// {
		// 	maxLength = 20;
		// }
		// float timeValue = (float)Main.timeForVisualEffects * 0.003f + Projectile.whoAmI * 0.3f;
		// float redValue = NormalDistance.ToRotation() / MathHelper.TwoPi;
		// List<Vertex2D> bars = new List<Vertex2D>();
		// for (int x = -20; x <= maxLength; x++)
		// {
		// 	Vector2 v0 = NormalDistance.RotatedBy(x / 20f * Projectile.ai[1]) * 90f;
		// 	Vector2 pos = Projectile.Center + v0 - Main.screenPosition - NormalDistance * 40f;
		// 	bars.Add(pos, new Color(redValue, 0.02f * (Math.Abs(x)+12), 0, 0), new Vector3(0.2f + timeValue, x / 35f, 0));
		// 	bars.Add(pos - NormalDistance * 50f, new Color(redValue, 0, 0, 0), new Vector3(0 + timeValue, x / 35f, 0));
		// }
		// Texture2D t = Commons.ModAsset.Noise_melting.Value;

		// if (bars.Count > 3)
		// 	spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool b0 = projHitbox.Center.X < targetHitbox.Right + 60 * Projectile.scale;
		bool b1 = projHitbox.Center.X > targetHitbox.Left - 60 * Projectile.scale;
		bool b2 = projHitbox.Center.Y > targetHitbox.Top - 60 * Projectile.scale;
		bool b3 = projHitbox.Center.Y < targetHitbox.Bottom + 60 * Projectile.scale;
		return b0 && b1 && b2 && b3;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x <= 20; x++)
		{
			GenerateSpark();
		}
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<WindBladeBallHit>(), 0, 0, -1, 12 * Projectile.scale, Main.rand.NextFloat(6.283f));
	}
}
