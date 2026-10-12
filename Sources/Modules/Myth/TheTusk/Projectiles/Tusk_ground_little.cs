using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

/// <summary>
/// Projectile.ai[0] is the length of tusk over gum.
/// </summary>
public class Tusk_ground_little : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.timeLeft = 120;
		Projectile.penetrate = -1;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = 0;
		Vector2 rot = TileUtils.GetTopographicGradient(Projectile.Center, 3);
		if (rot != Vector2.zeroVector)
		{
			Projectile.rotation = TileUtils.GetTopographicGradient(Projectile.Center, 3).ToRotation();
		}
		else
		{
			Projectile.active = false;
			return;
		}
		Projectile.frame = Main.rand.Next(8);
		for (int i = 0; i < 15; i++)
		{
			if (!Collision.SolidCollision(Projectile.Center, 0, 0))
			{
				Projectile.position -= Projectile.rotation.ToRotationVector2() * 4;
			}
			else if (Collision.SolidCollision(Projectile.Center + Projectile.rotation.ToRotationVector2() * 4, 0, 0))
			{
				Projectile.position += Projectile.rotation.ToRotationVector2() * 4;
			}
			else
			{
				break;
			}
		}
		Projectile.velocity *= 0;
	}

	public override void AI()
	{
		if (Projectile.timeLeft > 107)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0.1f, 0.8f);
		}
		if (Projectile.timeLeft is > 94 and <= 107)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0.04f, 0.7f);
		}
		if (Projectile.timeLeft is > 90 and <= 94)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0.16f, 0.8f);
		}
		if (Projectile.timeLeft is > 75 and <= 90)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0.03f, 0.7f);
		}
		if (Projectile.timeLeft is > 60 and <= 75)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1f, 0.7f);
			if (Projectile.timeLeft == 75)
			{
				Projectile.hostile = true;
				SpillBlood();
				Collision.HitTiles(Projectile.Center + new Vector2(20, 0).RotatedBy(Projectile.rotation) * Projectile.scale - new Vector2(8), new Vector2(0.2f, 0).RotatedBy(Projectile.rotation), 16, 16);
			}
		}
		if (Projectile.timeLeft is < 30)
		{
			Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 0f, 0.07f);
		}
	}

	public void SpillBlood(int amount = 1)
	{
		for (int g = 0; g < amount * 2; g++)
		{
			var blood = new BloodDrop
			{
				Velocity = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(3.4f, 14.1f),
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.rotation.ToRotationVector2() * 10,
				MaxTime = Main.rand.Next(54, 74),
				Scale = Main.rand.NextFloat(6f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float value = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, new Vector2(Projectile.ai[0] * 60, 0).RotatedBy(Projectile.rotation) + Projectile.Center, 10, ref value))
		{
			return true;
		}
		return false;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		Projectile.hide = true;
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Vector2 width = new Vector2(0, 10).RotatedBy(Projectile.rotation);
		Vector2 direction = new Vector2(1, 0).RotatedBy(Projectile.rotation);
		float height = 60 * Projectile.ai[0];

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);

		Effect effect = Commons.ModAsset.Shader2D.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		// Tusk
		List<Vertex2D> bars = new List<Vertex2D>();
		AddVertex(bars, Projectile.Center - width, new Vector3(Projectile.frame / 8f, Projectile.ai[0], 0));
		AddVertex(bars, Projectile.Center + width, new Vector3((Projectile.frame + 1) / 8f, Projectile.ai[0], 0));

		AddVertex(bars, Projectile.Center - width + direction * height, new Vector3(Projectile.frame / 8f, 0, 0));
		AddVertex(bars, Projectile.Center + width + direction * height, new Vector3((Projectile.frame + 1) / 8f, 0, 0));

		Texture2D tusk = ModAsset.Tusk_ground_little.Value;
		Main.graphics.graphicsDevice.Textures[0] = tusk;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect dissolve = ModAsset.BloodMembrane.Value;
		float duration = 0.3f;
		if (Projectile.timeLeft < 75)
		{
			duration = Math.Clamp(MathHelper.Lerp(1f, 0.3f, Projectile.timeLeft / 25f - 2), 0, 1f);
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(duration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.4f, 0, 0, 0.8f) * lightColor.ToVector4());
		dissolve.Parameters["uNoiseSize"].SetValue(0.7f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		// Blood Membrane
		bars = new List<Vertex2D>();
		AddVertex(bars, Projectile.Center - width, new Vector3(Projectile.frame / 8f, Projectile.ai[0], 0.6f));
		AddVertex(bars, Projectile.Center + width, new Vector3((Projectile.frame + 1) / 8f, Projectile.ai[0], 0.6f));

		AddVertex(bars, Projectile.Center - width + direction * height, new Vector3(Projectile.frame / 8f, 0, 0));
		AddVertex(bars, Projectile.Center + width + direction * height, new Vector3((Projectile.frame + 1) / 8f, 0, 0));

		Main.graphics.graphicsDevice.Textures[0] = tusk;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);

		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		// Gum
		float frameY = 0;
		if (Projectile.timeLeft is > 5 and < 72)
		{
			frameY = 0.5f;
		}
		bars = new List<Vertex2D>();
		AddVertex(bars, Projectile.Center - width - direction * 12, new Vector3(0, 0.5f + frameY, 0));
		AddVertex(bars, Projectile.Center + width - direction * 12, new Vector3(1, 0.5f + frameY, 0));

		float gumHeight = height * 2f;
		if (Projectile.timeLeft < 75)
		{
			gumHeight = MathF.Pow(height, 0.5f);
		}
		AddVertex(bars, Projectile.Center - width + direction * gumHeight, new Vector3(0, frameY, 0));
		AddVertex(bars, Projectile.Center + width + direction * gumHeight, new Vector3(1, frameY, 0));

		Texture2D gum = ModAsset.TuskGum.Value;
		Main.graphics.graphicsDevice.Textures[0] = gum;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coords)
	{
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()), coords);
	}
}
