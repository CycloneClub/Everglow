using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using static Everglow.SpellAndSkull.Common.SpellAndSkullUtils;

namespace Everglow.SpellAndSkull.Projectiles.MagnetSphere;

public class MagnetSphereArray : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ItemID.MagnetSphere && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (timer < 30)
			{
				timer++;
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;

		ringPos = ringPos * 0.9f + new Vector2(-12 * player.direction, -24 * player.gravDir) * 0.1f;
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawMagicArray();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}

	internal int timer = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 center = player.Center + ringPos - Main.screenPosition;
		float timeValue = (float)(Main.timeForVisualEffects * 0.01);
		float width = 10f;
		width *= timer / 30f;
		Color baseColor = new Color(0, 255, 215, 0);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t < 3; t++)
		{
			for (int i = 0; i < 2; i++)
			{
				Vector2 vertexPos = new Vector2(0, -70).RotatedBy(timeValue + t * MathHelper.TwoPi / 3d);
				Vector2 vertexVel = (vertexPos * 0.1f).RotatedBy((i - 0.5f) * 2);
				for (int j = 0; j < 50; j++)
				{
					Vector2 oldPos = vertexPos;
					if (j == 0)
					{
						Vector2 normalizedVel = Vector2.Normalize(vertexVel.RotatedBy(-MathHelper.PiOver2)) * width;
						bars.Add(new Vertex2D(center + vertexPos - normalizedVel, Color.Transparent, new Vector3(j / 25f + timeValue, 0, 0)));
						bars.Add(new Vertex2D(center + vertexPos + normalizedVel, Color.Transparent, new Vector3(j / 25f + timeValue, 1, 0)));
					}
					vertexPos += vertexVel;
					vertexVel = Vector2.Normalize(-vertexPos - vertexVel) * 2.5f + vertexVel * 0.96f;
					Color drawColor = baseColor;
					if (vertexPos.Length() < 3.5f)
					{
						vertexPos = Vector2.zeroVector;
						drawColor = Color.Transparent;
					}
					Vector2 normalized = Vector2.Normalize(vertexPos - oldPos).RotatedBy(MathHelper.PiOver2) * width;
					bars.Add(new Vertex2D(center + vertexPos - normalized, drawColor, new Vector3(j / 25f + timeValue, 0, 0)));
					bars.Add(new Vertex2D(center + vertexPos + normalized, drawColor, new Vector3(j / 25f + timeValue, 1, 0)));
				}
			}
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.FogTraceLight.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int x = 0; x <= 30; x++)
		{
			float rad = 70 + MathF.Sin(timeValue) * 10;
			Vector2 radius = new Vector2(0, -1).RotatedBy(x / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(center + radius * rad, baseColor, new Vector3(x / 30f, 0 + timeValue, 0)));
			bars.Add(new Vertex2D(center + radius * (rad - 15 * timer / 30f), Color.Transparent, new Vector3(x / 30f, 0.1f + timeValue, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_phantom.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int x = 0; x <= 30; x++)
		{
			float rad = 70 + MathF.Sin(timeValue) * 10;
			Vector2 radius = new Vector2(0, -1).RotatedBy(x / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(center + radius * rad, baseColor, new Vector3(x / 30f, 0 + timeValue, 0)));
			bars.Add(new Vertex2D(center + radius * (rad - 15 * timer / 30f), Color.Transparent, new Vector3(x / 30f, 0.1f + timeValue, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_phantom.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int x = 0; x < 3; x++)
		{
			Vector2 radius = new Vector2(0, -1).RotatedBy((x + 0.5) / 3d * MathHelper.TwoPi + timeValue);
			Vector2 cut = new Vector2(0, -1).RotatedBy((x + 0.5) / 3d * MathHelper.TwoPi + MathHelper.PiOver2 + timeValue);
			bars.Add(new Vertex2D(center + radius * 40, baseColor, new Vector3(x / 3f, 0 + timeValue * 0.2f, 0)));
			bars.Add(new Vertex2D(center + radius * 80 + cut * width * 0.4f, baseColor, new Vector3(x / 3f + 0.4f, 0.4f + timeValue * 0.2f, 0)));
			bars.Add(new Vertex2D(center + radius * 80 - cut * width * 0.4f, baseColor, new Vector3(x / 3f - 0.4f, 0.4f + timeValue * 0.2f, 0)));

			bars.Add(new Vertex2D(center + radius * 80 + cut * width * 0.4f, baseColor, new Vector3(x / 3f + 0.4f, 0.4f + timeValue * 0.2f, 0)));
			bars.Add(new Vertex2D(center + radius * 80 - cut * width * 0.4f, baseColor, new Vector3(x / 3f - 0.4f, 0.4f + timeValue * 0.2f, 0)));
			bars.Add(new Vertex2D(center + radius * 100, baseColor, new Vector3(x / 3f, 0.8f + timeValue * 0.2f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_crack_dense.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		Player player = Main.player[Projectile.owner];
		DrawTexCircle(spriteBatch, timer * 1.2f, 52, new Color(64, 7, 255, 0), player.Center + ringPos - Main.screenPosition, Commons.ModAsset.Trail_5.Value, Main.timeForVisualEffects / 17);
	}
}
