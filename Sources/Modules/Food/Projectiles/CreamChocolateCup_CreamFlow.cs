using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Food.Dusts;
using Humanizer.Bytes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Food.Projectiles;

internal class CreamChocolateCup_CreamFlow : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Joints = new List<Vector2>();
		JointVelocity = new List<Vector2>();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void AI()
	{
		Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
		for (int i = 0; i < Joints.Count; i++)
		{
			Joints[i] += JointVelocity[i];
			Vector2 check0 = Joints[i] + new Vector2(JointVelocity[i].X, 0) + Projectile.Center;
			if (TileUtils.PlatformCollision(check0))
			{
				JointVelocity[i] = new Vector2(JointVelocity[i].X * -1, JointVelocity[i].Y) * -0.4f;
			}
			Vector2 check1 = Joints[i] + new Vector2(0, JointVelocity[i].Y) + Projectile.Center;
			if (TileUtils.PlatformCollision(check1))
			{
				bool b = JointVelocity[i].Y < 0;
				JointVelocity[i] = new Vector2(JointVelocity[i].X, JointVelocity[i].Y * -1) * -0.4f;
				if (b)
				{
					JointVelocity[i] += new Vector2(0, -0.15f);
				}
			}
			else
			{
				JointVelocity[i] += new Vector2(0, 0.15f);
			}
			if (Joints.Count > 1)
			{
				if (i == 0)
				{
					float dragForce = (Joints[i + 1] - Joints[i]).Length() - 30;
					dragForce = MathF.Max(0, dragForce);
					JointVelocity[i] += dragForce * Utils.SafeNormalize(Joints[i + 1] - Joints[i], Vector2.zeroVector) * 0.01f;
				}
				else if (i == Joints.Count - 1)
				{
					float dragForce = (Joints[i - 1] - Joints[i]).Length() - 30;
					dragForce = MathF.Max(0, dragForce);
					JointVelocity[i] += dragForce * Utils.SafeNormalize(Joints[i - 1] - Joints[i], Vector2.zeroVector) * 0.01f;
				}
				else
				{
					float dragForce = (Joints[i + 1] - Joints[i]).Length() - 30;
					dragForce = MathF.Max(0, dragForce);
					JointVelocity[i] += dragForce * Utils.SafeNormalize(Joints[i + 1] - Joints[i], Vector2.zeroVector) * 0.005f;
					dragForce = (Joints[i - 1] - Joints[i]).Length() - 30;
					dragForce = MathF.Max(0, dragForce);
					JointVelocity[i] += dragForce * Utils.SafeNormalize(Joints[i - 1] - Joints[i], Vector2.zeroVector) * 0.005f;
				}
			}

			JointVelocity[i] *= 0.98f;
		}
		if (Joints.Count > 10)
		{
			Joints.RemoveAt(0);
			JointVelocity.RemoveAt(0);
		}

		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(Joints.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (Joints.Count != 0)
		{
			SmoothTrail.Add(Joints.ToArray()[Joints.Count - 1]);
		}

		if (Projectile.timeLeft < 140 && Projectile.timeLeft > 40)
		{
			for (int x = 0; x < SmoothTrail.Count - 1; x++)
			{
				if (Main.rand.NextBool(2))
				{
					int indexOfVelocity = (int)(x / (float)SmoothTrail.Count * JointVelocity.Count);
					indexOfVelocity = Math.Clamp(indexOfVelocity, 0, JointVelocity.Count - 1);
					int correctionTimeLeft = Projectile.timeLeft - indexOfVelocity;
					if (correctionTimeLeft < 120 && correctionTimeLeft > 60)
					{
						Vector2 v1 = SmoothTrail[x] + Projectile.Center;
						Dust d = Dust.NewDustDirect(v1 - new Vector2(4) - new Vector2(15f), 30, 30, ModContent.DustType<CreamDust>(), 0, 0, 0);
						d.velocity = JointVelocity[indexOfVelocity];
						d.rotation = Main.rand.NextFloat(6.283f);
						d.scale = Main.rand.NextFloat(0.8f, 1.6f);
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft < 120)
		{
			return false;
		}
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(Joints.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (Joints.Count != 0)
		{
			SmoothTrail.Add(Joints.ToArray()[Joints.Count - 1]);
		}

		foreach (Vector2 v0 in SmoothTrail)
		{
			Vector2 v1 = v0 + Projectile.Center;
			Rectangle r0 = new Rectangle((int)(v1.X - 24), (int)(v1.Y - 24), 48, 48);
			if (Rectangle.Intersect(targetHitbox, r0) != Rectangle.emptyRectangle)
			{
				return true;
			}
		}
		return false;
	}

	public List<Vector2> Joints = new List<Vector2>();
	public List<Vector2> JointVelocity = new List<Vector2>();

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (Joints.Count > 1)
		{
			List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(Joints.ToList()); // 平滑
			var SmoothTrail = new List<Vector2>();
			for (int x = 0; x < SmoothTrailX.Count - 1; x++)
			{
				SmoothTrail.Add(SmoothTrailX[x]);
			}
			if (Joints.Count != 0)
			{
				SmoothTrail.Add(Joints.ToArray()[Joints.Count - 1]);
			}

			Vector2 jointVelocity0 = Utils.SafeNormalize(SmoothTrail[1] - SmoothTrail[0], Vector2.zeroVector);
			Vector2 jointVelocity0Left = jointVelocity0.RotatedBy(MathHelper.PiOver2) * 10;
			List<Vertex2D> bars = new List<Vertex2D>();
			Color drawColor = Lighting.GetColor((Projectile.Center + SmoothTrail[0] - jointVelocity0 * 24).ToTileCoordinates());
			byte colorA = 0;

			if (Projectile.timeLeft < 120f)
			{
				colorA = (byte)((1 - Projectile.timeLeft / 120f) * 255);
			}
			drawColor.A = colorA;
			bars.Add(Projectile.Center + SmoothTrail[0] - jointVelocity0 * 24 + jointVelocity0Left, drawColor, new Vector3(1, 0.22f, 1));
			bars.Add(Projectile.Center + SmoothTrail[0] - jointVelocity0 * 24 - jointVelocity0Left, drawColor, new Vector3(1, 0.78f, 1));

			for (int i = 1; i < SmoothTrail.Count - 1; i++)
			{
				int indexOfVelocity = (int)(i / (float)SmoothTrail.Count * JointVelocity.Count);
				indexOfVelocity = Math.Clamp(indexOfVelocity, 0, JointVelocity.Count - 1);
				int correctionTimeLeft = Projectile.timeLeft - indexOfVelocity;
				if (correctionTimeLeft < 120f)
				{
					colorA = (byte)((1 - correctionTimeLeft / 120f) * 255);
					if (correctionTimeLeft < 0)
					{
						colorA = 255;
					}
				}

				Vector2 jointVelocityLeft = Utils.SafeNormalize(SmoothTrail[i] - SmoothTrail[i - 1], Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * 10f;
				Vector2 basePos = Projectile.Center + SmoothTrail[i];

				drawColor = Lighting.GetColor((basePos + jointVelocityLeft).ToTileCoordinates());
				drawColor.A = colorA;

				bars.Add(basePos + jointVelocityLeft, drawColor, new Vector3(0.33f + 0.33f * (i % 2), 0.22f, 1));
				bars.Add(basePos - jointVelocityLeft, drawColor, new Vector3(0.33f + 0.33f * (i % 2), 0.78f, 1));
			}

			if (SmoothTrail.Count > 2)
			{
				jointVelocity0 = Utils.SafeNormalize(SmoothTrail[SmoothTrail.Count - 1] - SmoothTrail[SmoothTrail.Count - 2], Vector2.zeroVector);
				jointVelocity0Left = jointVelocity0.RotatedBy(MathHelper.PiOver2) * 10;

				drawColor = Lighting.GetColor((Projectile.Center + SmoothTrail[SmoothTrail.Count - 1] + jointVelocity0 * 4).ToTileCoordinates());
				drawColor.A = colorA;

				bars.Add(Projectile.Center + SmoothTrail[SmoothTrail.Count - 1] + jointVelocity0 * 4 + jointVelocity0Left, drawColor, new Vector3(0, 0.22f, 1));
				bars.Add(Projectile.Center + SmoothTrail[SmoothTrail.Count - 1] + jointVelocity0 * 4 - jointVelocity0Left, drawColor, new Vector3(0, 0.78f, 1));
			}
			if (bars.Count > 0)
			{
				Effect effect = ModAsset.CreamChocolateCup_CreamShader.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uNoise"].SetValue(ModAsset.CreamChocolateCup_CreamFlow_dissolve.Value);
				effect.CurrentTechnique.Passes["Test"].Apply();
				Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
				Main.graphics.graphicsDevice.Textures[0] = ModAsset.CreamChocolateCup_CreamFlow.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
		return false;
	}
}
