using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class KissOfCthulhu_Projectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public const float TrailLength = 0.3f;
	public const int VertexGroupCount = 200;
	public const int TimeLeftMax = 600;
	public const float ExpandTime = 30;

	public int DustCount = 0;

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.timeLeft = TimeLeftMax;
		DustCount = 0;
	}

	// public override void AI()
	// {
	// if (Main.rand.NextBool(3))
	// {
	// Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y);
	// }
	// }

	// public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	// {
	// target.AddElementalDebuffBuildUp(Main.player[Projectile.owner], ElementalDebuffType.Corrosion, 400);
	// }

	// public override void OnKill(int timeLeft)
	// {
	// for (int i = 0; i < 30; i++)
	// {
	// Dust.NewDust(Projectile.Center, 10, 10, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y);
	// }
	// SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
	// }

	public override void AI()
	{
		// Framw breaking smooth dust trail.
		for (int i = 0; i < Projectile.velocity.Length(); i++)
		{
			float duration = i / Projectile.velocity.Length();
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<KissOfCthulhu_Trail>());
			dust.position -= new Vector2(4) - duration * Projectile.velocity;
			var cus = new Vector4(DustCount, Projectile.velocity.ToRotationSafe(), 0, 0);
			float speed = 2.5f;
			if (cus.X < 90f)
			{
				speed *= cus.X / 90f;
			}
			dust.position += new Vector2(0, speed).RotatedBy(cus.Y) * MathF.Sin(cus.X * 0.035f + (float)Main.time * 0.2f) * (1 - duration);
			dust.customData = cus;
			dust.scale = 2 * MathF.Pow(0.87f, duration);
			DustCount++;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddElementalDebuffBuildUp(Main.player[Projectile.owner], CorrosionDebuff.ID, 400);
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 15; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y);
			dust.position -= new Vector2(4);
		}
		// for (int i = 0; i < 24; i++)
		// {
		// Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<KissOfCthulhu_Trail>());
		// dust.position -= new Vector2(4);
		// dust.velocity = new Vector2(0, 24).RotatedBy(i / 24f * MathHelper.TwoPi);
		// }
		SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		// var drawColor = new Color(100, 100, 100, 255);
		// float length = Projectile.timeLeft < (TimeLeftMax - ExpandTime)
		// ? 1f
		// : 1f - 0.8f * (Projectile.timeLeft - (TimeLeftMax - ExpandTime)) / ExpandTime;

		// var vertices = new List<Vertex2D>();
		// for (int i = 0; i < VertexGroupCount; i++)
		// {
		// var segmentRelativePosition = (i - VertexGroupCount / 2) * Projectile.velocity * 0.04f * length;

		// float timeFactor = -0.5f;
		// float x = segmentRelativePosition.Length() * 0.2f / length + (float)Main.time * timeFactor;
		// float segmentY = 1.0f * MathF.Sin((2 * MathF.PI / 10) * x - (2 * MathF.PI / 5) * (float)Main.time * timeFactor + 0) +
		// 0.5f * MathF.Sin((2 * MathF.PI / 20) * x - (2 * MathF.PI / 10) * (float)Main.time * timeFactor + MathF.PI / 2);

		// var position = Projectile.Center - Main.screenPosition + segmentRelativePosition
		// + new Vector2(0, segmentY * 6).RotatedBy(Projectile.velocity.ToRotation());

		// var widthFactor = MathF.Abs(i - VertexGroupCount / 2) < VertexGroupCount * (0.5f - TrailLength)
		// ? 1f
		// : 1 - (MathF.Abs(i - VertexGroupCount / 2) - VertexGroupCount * (0.5f - TrailLength)) / (VertexGroupCount * TrailLength);
		// var widthVector = new Vector2(0, 8 * widthFactor).RotatedBy(Projectile.velocity.ToRotation());
		// vertices.Add(position + widthVector, drawColor, new Vector3(i / 34f, 0, 0));
		// vertices.Add(position - widthVector, drawColor, new Vector3(i / 34f, 1, 0));
		// }

		// Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		// var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// Effect shineEffect = ModAsset.KissOfCthulhu_VFX.Value;
		// shineEffect.Parameters["uHeatMap"].SetValue(ModAsset.KissOfCthulhu_Projectile.Value);
		// shineEffect.CurrentTechnique.Passes["Worm"].Apply();

		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(sBS);
		return false;
	}
}
