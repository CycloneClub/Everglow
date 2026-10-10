using Everglow.Commons.Templates.Weapons.Yoyos;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class BloodyBoneYoyo : YoyoProjectile
{
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 60;
		ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 7;
		ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 320;
	}

	public struct Tentacle
	{
		public List<Vector2> oldPos;
		public List<Vector2> jointVel;
		public Vector2 velocity;
		public Vector2 position;
		public float time;
		public float ai0;
		public float ai1;
		public float ai2;
		public float ai3;
	}

	public List<Tentacle> tentacles;

	public bool TantacleHit = false;

	public Tentacle UpdateTentacle(Tentacle tentacle)
	{
		if (tentacle.time < 75 && Projectile.ai[0] > 0)
		{
			tentacle.oldPos.Add(tentacle.position);
			float mulRot = (75 - tentacle.time) / 75f;
			mulRot = MathF.Max(mulRot, 0);
			tentacle.jointVel.Add(tentacle.velocity.RotatedBy(MathHelper.PiOver2) * 0.1f * mulRot * MathF.Sin(tentacle.time * 0.4f));
		}
		if (tentacle.time > 75)
		{
			if (tentacle.oldPos.Count > 0)
			{
				tentacle.oldPos.RemoveAt(0);
				tentacle.jointVel.RemoveAt(0);
				float coilValue = 20;
				if (tentacle.time > 100)
				{
					coilValue = (tentacle.time - 95) * 4;
				}
				for (int x = tentacle.oldPos.Count - 1; x >= 0; x--)
				{
					float value = Math.Max(0, ((coilValue - 0.01f) - x) / coilValue);
					tentacle.oldPos[x] -= tentacle.oldPos[0] * value;
				}
			}
		}
		else if (tentacle.oldPos.Count > Main.rand.Next(150 - (int)tentacle.time) * 2f)
		{
			tentacle.oldPos.RemoveAt(0);
			tentacle.jointVel.RemoveAt(0);
			float coilValue = 20;
			if (tentacle.time > 100)
			{
				coilValue = (tentacle.time - 95) * 4;
			}
			for (int x = tentacle.oldPos.Count - 1; x >= 0; x--)
			{
				float value = Math.Max(0, ((coilValue - 0.01f) - x) / coilValue);
				tentacle.oldPos[x] -= tentacle.oldPos[0] * value;
			}
		}

		// 收球的时候剧烈收回
		if (Projectile.ai[0] <= 0)
		{
			for (int a = 0; a < 2; a++)
			{
				if (tentacle.oldPos.Count > 0)
				{
					tentacle.oldPos.RemoveAt(0);
					tentacle.jointVel.RemoveAt(0);
					for (int x = tentacle.oldPos.Count - 1; x >= 0; x--)
					{
						float value = Math.Max(0, (19.99f - x) / 20f);
						tentacle.oldPos[x] -= tentacle.oldPos[0] * value;
					}
				}
				else
				{
					break;
				}
			}
		}
		for (int x = tentacle.oldPos.Count - 1; x >= 0; x--)
		{
			if (tentacle.jointVel.Count > x)
			{
				tentacle.oldPos[x] += tentacle.jointVel[x];
				tentacle.jointVel[x] *= 0.98f;
			}
		}
		////碰撞
		// for (int x = tentacle.oldPos.Count - 1; x >= 0; x--)
		// {
		// if(tentacle.jointVel.Count > x)
		// {
		// if(Collision.SolidCollision(tentacle.oldPos[x] + tentacle.jointVel[x] + Projectile.Center, 1, 1))
		// {
		// tentacle.jointVel[x] -= Vector2.Normalize(tentacle.oldPos[x]);
		// tentacle.oldPos[x] += tentacle.jointVel[x];

		// }
		// else if(Collision.SolidCollision(tentacle.oldPos[x] + Projectile.velocity + Projectile.Center, 1, 1))
		// {
		// tentacle.jointVel[x] -= Vector2.Normalize(Projectile.velocity) * 0.2f;
		// tentacle.oldPos[x] -= Projectile.velocity;
		// }
		// else
		// {
		// tentacle.oldPos[x] += tentacle.jointVel[x];
		// tentacle.jointVel[x] *= 0.95f;
		// }
		// }
		// }
		////牵拉
		// for (int x = tentacle.oldPos.Count - 1; x >= 1; x--)
		// {
		// if (tentacle.jointVel.Count > x)
		// {
		// Vector2 v0 = tentacle.oldPos[x - 1] + tentacle.jointVel[x - 1] - tentacle.oldPos[x] - tentacle.jointVel[x];
		// if (v0.Length() > 15)
		// {
		// Vector2 dragForce = Vector2.Normalize(v0) * (v0.Length() - 15) * 0.01f;
		// for (int y = x; y < tentacle.oldPos.Count - 1; y++)
		// {
		// tentacle.oldPos[x] += dragForce * Math.Max(0, (19.99f - x) / 20f);
		// }
		// }
		// }
		// }
		if (Collision.SolidCollision(tentacle.position + tentacle.velocity + Projectile.Center, 1, 1))
		{
			if (tentacle.time < 75)
			{
				tentacle.time = 150 - tentacle.time;
			}
		}
		tentacle.position += tentacle.velocity;
		float maxRotVel = tentacle.time / 30f;
		if (maxRotVel > 1)
		{
			maxRotVel = 1;
		}
		tentacle.velocity = tentacle.velocity.RotatedBy(tentacle.ai0 * maxRotVel);

		tentacle.ai0 *= 0.98f;
		tentacle.ai0 += tentacle.ai1;
		tentacle.ai1 *= 0.98f;
		if (tentacle.time > 30)
		{
			if (tentacle.velocity.Length() > 0.1f)
			{
				tentacle.velocity *= 0.94f;
			}
		}
		if (tentacle.time > 60)
		{
			// tentacle.velocity += Vector2.Normalize(-tentacle.velocity - tentacle.position);
		}
		tentacle.time++;
		return tentacle;
	}

	public override void OnSpawn(IEntitySource source)
	{
		tentacles = new List<Tentacle>();
	}

	public override void PostAI()
	{
		if (Projectile.oldPosition != Vector2.Zero)
		{
			for (int g = 0; g < Projectile.velocity.Length() * 2.5f; g++)
			{
				Vector2 a0 = new Vector2(Projectile.width, Projectile.height) / 2f;
				Vector2 v3 = Projectile.oldPosition + a0;
				Vector2 v4 = Vector2.Normalize(Projectile.velocity) * 0.6f;
				Dust dust = Dust.NewDustDirect(v3 + v4 * g - new Vector2(4, 4), 4, 4, DustID.Blood, 0f, 0f, 100, default, Main.rand.NextFloat(1.3f, 2f) * 0.4f);
				dust.noGravity = true;
				dust.velocity = new Vector2(0, Main.rand.NextFloat(0.4f, 2.5f)).RotatedByRandom(Math.PI * 2d) * 0.5f;
			}
		}
		if (Main.rand.NextBool(15) && tentacles.Count < 8)
		{
			Tentacle tentacle = default(Tentacle);
			tentacle.oldPos = new List<Vector2>();
			tentacle.jointVel = new List<Vector2>();
			tentacle.time = 0;
			tentacle.position = Vector2.zeroVector;
			tentacle.velocity = new Vector2(Main.rand.NextFloat(3, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			tentacle.ai0 = Main.rand.NextFloat(-0.3f, 0.3f);
			tentacle.ai1 = Main.rand.NextFloat(-0.001f, 0.027f) * -MathF.Sign(tentacle.ai0);
			tentacles.Add(tentacle);
		}
		if (tentacles.Count > 0)
		{
			for (int x = tentacles.Count - 1; x >= 0; x--)
			{
				tentacles[x] = UpdateTentacle(tentacles[x]);
				if (tentacles[x].time > 150)
				{
					tentacles.RemoveAt(x);
				}
			}
		}
	}

	public override void ModifyOnHitBounce(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (TantacleHit)
		{
			TantacleHit = false;
			Projectile.velocity += Vector2.Normalize(Projectile.Center - target.Center) / (Weight + 0.01f);
		}
		else
		{
			Projectile.velocity += Vector2.Normalize(Projectile.Center - target.Center) / (Weight + 0.01f) * 100f;
		}
	}

	public override Color ModifyYoyoStringColor_PostVanillaRender(Color vanillaColor, Vector2 worldPos, float index, float stringCount)
	{
		float value = index / stringCount;
		Color color = Color.Lerp(vanillaColor, new Color(0.9f, 0.3f, 0.2f, 1f), value);
		color = Lighting.GetColor(worldPos.ToTileCoordinates(), color);
		return color;
	}

	public override void DrawYoyo_String_Attachments(Vector2 drawPos, float rotation, Color color, float length, float index, float stringUnitCount)
	{
		Texture2D tex = ModAsset.BloodyBoneYoyo_line.Value;
		color = Lighting.GetColor((drawPos + Main.screenPosition).ToTileCoordinates());
		if (index % 4 == 0)
		{
			Main.spriteBatch.Draw(tex, drawPos, new Rectangle(3, 0, 2, 6), color, rotation + MathHelper.PiOver2, Vector2.One, new Vector2(1f, 1f), SpriteEffects.None, 0f);
		}
		else if (index % 4 == 2)
		{
			Main.spriteBatch.Draw(tex, drawPos, new Rectangle(6, 0, 2, 6), color, rotation - MathHelper.PiOver2, Vector2.One, new Vector2(1f, 1f), SpriteEffects.None, 0f);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (Tentacle tentacle in tentacles)
		{
			foreach (Vector2 v0 in tentacle.oldPos)
			{
				Rectangle rectangle = new Rectangle((int)(Projectile.Center + v0).X - 4, (int)(Projectile.Center + v0).Y - 4, 8, 8);
				if (targetHitbox.Intersects(rectangle))
				{
					TantacleHit = true;
					return true;
				}
			}
		}
		TantacleHit = false;
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Player previousDrawPlayer = DrawPlayer;
		DrawPlayer = player;
		try
		{
			DrawYoyo_String();
			List<Vertex2D> bars = new List<Vertex2D>();
			foreach (Tentacle tentacle in tentacles)
			{
				for (int x = 0; x < tentacle.oldPos.Count; x++)
				{
					Vector2 drawPos = tentacle.oldPos[x] + Projectile.Center;
					Vector2 posLeft = Vector2.Normalize(tentacle.oldPos[x]).RotatedBy(MathHelper.PiOver2) * 5f;
					if (x > 0)
					{
						posLeft = Vector2.Normalize(tentacle.oldPos[x] - tentacle.oldPos[x - 1]).RotatedBy(MathHelper.PiOver2) * 5f;
					}
					float factor = (x + 120 - tentacle.oldPos.Count) / 120f;
					float width = 1f;
					if (factor > 0.8f)
					{
						width = MathF.Sin((1 - factor) * 2.5f * MathF.PI);
					}
					if (x == 0)
					{
						bars.Add(Projectile.Center + posLeft, Color.Transparent, new Vector3(factor, 0, width));
						bars.Add(Projectile.Center - posLeft, Color.Transparent, new Vector3(factor, 1, width));
						bars.Add(Projectile.Center + posLeft, lightColor, new Vector3(factor, 0, width));
						bars.Add(Projectile.Center - posLeft, lightColor, new Vector3(factor, 1, width));
					}
					Color newLightColor = Lighting.GetColor(drawPos.ToTileCoordinates());
					bars.Add(drawPos + posLeft, newLightColor, new Vector3(factor, 0, width));
					bars.Add(drawPos - posLeft, newLightColor, new Vector3(factor, 1, width));
					if (x == tentacle.oldPos.Count - 1)
					{
						bars.Add(drawPos + posLeft, Color.Transparent, new Vector3(factor, 0, width));
						bars.Add(drawPos - posLeft, Color.Transparent, new Vector3(factor, 1, width));
					}
				}
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect effect = Commons.ModAsset.Trailing.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.BloodyBoneYoyo_tentacle.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return true;
		}
		finally
		{
			DrawPlayer = previousDrawPlayer;
		}
	}
}
