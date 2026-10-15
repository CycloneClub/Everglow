using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.Misc.Projectiles.Accessory;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class IchorClub : ClubProj
{
	private float vfxTimer = 0;
	private int flyClubCooling = 0;

	public override void SetCustomDefaults()
	{
		Beta = 0.005f;
		MaxOmega = 0.45f;
		vfxTimer = 0;
		WarpValue = 0.03f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int x = 0; x < 2; x++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283) - Projectile.velocity * 0.2f;
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center + velocity * -2, velocity, ModContent.ProjectileType<IchorCurrent>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 3f/*If ai[0] equal to 3, another ai will be execute*/);
			p.friendly = false;
			p.CritChance = Projectile.CritChance;
		}
		target.AddBuff(BuffID.Ichor, (int)(818 * Omega));
	}

	public override void AI()
	{
		base.AI();

		if (Omega > 0.1f)
		{
			for (float d = 0.1f; d < Omega; d += 0.1f)
			{
			}
			if (flyClubCooling > 0)
			{
				flyClubCooling--;
			}

			if (flyClubCooling <= 0 && Omega > 0.2f)
			{
				flyClubCooling = 44;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IchorClub_fly>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack * 0.4f, Projectile.owner);
			}
		}
		vfxTimer += Omega * 4;
		if (vfxTimer >= 1)
		{
			GenerateVFX((int)(vfxTimer * 3));
			vfxTimer = 0;
		}
	}

	public override void PostDraw(Color lightColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (Projectile.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Texture2D texture = ModAsset.IchorClub_glow.Value;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
		for (int i = 0; i < 5; i++)
		{
			float fade = Omega * 2f + 0.2f;
			fade *= (5 - i) / 5f;
			var color2 = new Color(fade, fade, fade, 0);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation - i * 0.75f * Omega, texture.Size() / 2f, Projectile.scale, effects, 0f);
		}
	}

	public override void PostPreDraw()
	{
		float fade = Omega * 2f + 0.2f;
		var color2 = new Color(fade, Math.Min(fade * 0.4f, 0.6f), 0, 0);
		var bars = CreateTrailVertices(paramA: 0.5f, paramB: 0.5f, useSpecialAplha: true, trailColor: color2);
		if (bars == null)
		{
			return;
		}

		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, model);

		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void GenerateVFX(int frequency)
	{
		Player player = Main.player[Projectile.owner];
		float mulVelocity = Main.rand.NextFloat(0.75f, 1.5f);

		for (int g = 0; g < frequency * 2; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(10f)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(1f, 6f);
			Vector2 startPos = new Vector2(Main.rand.NextFloat(0.7f, 1f) * HitLength * 1.9f, 0).RotatedBy(Projectile.rotation - Omega * Main.rand.NextFloat(1f) + MathHelper.PiOver4 * Projectile.spriteDirection);
			if (Main.rand.NextBool(2))
			{
				startPos *= -1;
			}
			var blood = new IchorDrop
			{
				Velocity = afterVelocity * mulVelocity / mulScale + startPos.RotatedBy(MathHelper.PiOver2) * Omega * 0.4f * HitLength / 32f + player.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + startPos,
				MaxTime = Main.rand.Next(6, 12),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(3f)).RotatedByRandom(MathHelper.TwoPi);
			Vector2 startPos = new Vector2(MathF.Sqrt(Main.rand.NextFloat(0f, 1f)) * HitLength * 1.9f, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection);
			if (Main.rand.NextBool(2))
			{
				startPos *= -1;
			}
			var blood = new IchorSplash
			{
				Velocity = afterVelocity * mulVelocity + startPos.RotatedBy(MathHelper.PiOver2) * Omega * 0.4f * HitLength / 32f + player.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + startPos,
				MaxTime = Main.rand.Next(6, 12),
				Scale = Main.rand.NextFloat(12f, 24f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(blood);
		}
	}
}
