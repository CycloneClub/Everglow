using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class LargeBloodLanternGhost_Tentacles : ModProjectile
{
	public struct Tentacle()
	{
		public Vector2 Position;
		public List<Vector2> Joints;
		public Vector2 Velocity;
		public int Timer;
		public int TimeMax;
		public bool Active;
	}

	public List<Tentacle> TentacleEntities = new List<Tentacle>();

	public float Timer = 0;

	public int StretchTime = 30;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 500;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		TentacleEntities = new List<Tentacle>();
	}

	public void UpdateEntity()
	{
		int prepareTimer = 120;
		if (Timer < prepareTimer)
		{
			if (Projectile.timeLeft % 20 == 0)
			{
				float rot = Projectile.timeLeft * 0.8f;
				for (int x = 0; x < 3; x++)
				{
					var entity = new Tentacle();
					entity.Position = Vector2.zeroVector;
					entity.Velocity = new Vector2(0, Main.rand.NextFloat(4.5f, 6f)).RotatedBy(rot + x / 3f * MathHelper.TwoPi) * (Timer + 120) / 120f;
					entity.Joints = new List<Vector2>();
					entity.TimeMax = 180;
					entity.Timer = 0;
					entity.Active = true;
					TentacleEntities.Add(entity);
				}
			}
		}
		for (int i = 0; i < TentacleEntities.Count; i++)
		{
			Tentacle entity = TentacleEntities[i];
			UpdateTentacle(ref entity);
			TentacleEntities[i] = entity;
		}
	}

	public void UpdateTentacle(ref Tentacle entity)
	{
		if (entity.Active)
		{
			if (entity.Timer <= StretchTime)
			{
				entity.Joints.Add(entity.Position);
				entity.Position += entity.Velocity;
				if (entity.Timer >= StretchTime - 5)
				{
					entity.Velocity *= 0.9f;
				}
			}
			else
			{
				if (entity.Velocity.Length() != 1)
				{
					entity.Velocity = entity.Velocity.NormalizeSafe();
				}
				float timeValue = (entity.Timer - StretchTime * 1.5f) * 0.06f - 0.8f;
				if (entity.Joints.Count > 0)
				{
					for (int c = entity.Joints.Count - 1; c >= 0; c--)
					{
						entity.Joints[c] -= entity.Velocity * timeValue;
					}
					Vector2 toCenterDis = entity.Joints[0];
					Vector2 toCenterDisNext = entity.Joints[0] - entity.Velocity * timeValue;
					if (toCenterDisNext.Length() > toCenterDis.Length() && timeValue > 0)
					{
						entity.Joints.RemoveAt(0);
					}
				}
			}
			if (entity.Joints.Count > 0)
			{
				Lighting.AddLight(entity.Joints[^1], new Vector3(1f, 0.3f * MathF.Sin(Timer * 0.03f) + 0.3f, 0.3f * MathF.Cos(Timer * 0.03f) + 0.3f) * 0.8f);
			}
			entity.Timer++;
			if (entity.TimeMax - entity.Timer <= 0 || (entity.Joints.Count <= 1 && entity.Timer > StretchTime))
			{
				entity.Active = false;
			}
		}
	}

	public override void AI()
	{
		Timer++;
		UpdateEntity();
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<LargeBloodLanternGhost>())
		{
			Projectile.Center = OwnerNPC.Center;
		}
		else
		{
			foreach (var tentacle in TentacleEntities)
			{
				for (int g = 0; g < 3; g++)
				{
					if (tentacle.Joints.Count > 8)
					{
						Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 3f, 0).RotatedByRandom(MathHelper.TwoPi);
						string texturePath = ModAsset.LargeBloodLanternGhostTentacle_Gore_0_Mod;
						if (texturePath is not null)
						{
							texturePath = texturePath.Remove(texturePath.Length - 1, 1);
							texturePath += g;
						}
						Vector2 pos = tentacle.Joints[^1] * g / 3f + Projectile.Center;
						var gore = new NormalGore
						{
							Velocity = vel,
							Position = pos,
							Texture = ModContent.Request<Texture2D>(texturePath).Value,
							RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
							Scale = Main.rand.NextFloat(0.8f, 1.2f),
							MaxTime = Main.rand.Next(300, 340),
							Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						};
						Ins.VFXManager.Add(gore);
					}
				}
			}
			Projectile.Kill();
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var ent in TentacleEntities)
		{
			if (ent.Active)
			{
				for (int i = 0; i < ent.Joints.Count; i++)
				{
					var pos = ent.Joints[i] + Projectile.Center;
					var collisionBox = new Rectangle(-6 + (int)pos.X, -6 + (int)pos.Y, 12, 12);
					if (collisionBox.Intersects(targetHitbox))
					{
						return true;
					}
				}
			}
		}
		return false;
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		if (TentacleEntities.Count > 0)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			for (int i = 0; i < TentacleEntities.Count; i++)
			{
				var ent = TentacleEntities[i];
				var bars = new List<Vertex2D>();
				if (ent.Active)
				{
					List<Vector2> joint2 = ent.Joints;
					if (!Main.gamePaused)
					{
						float timeValue = (StretchTime * 1.5f - ent.Timer) * 0.06f;
						var dir = ent.Velocity.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
						for (int j = 1; j < joint2.Count; j++)
						{
							float value = timeValue * MathF.Sin(j * 0.75f) * (5f / (j + 5f));
							if (ent.Timer < StretchTime * 1.5)
							{
								value *= MathF.Sin((float)Main.time * 0.12f);
							}
							joint2[j] += dir * value;
						}
					}
					for (int j = 1; j < joint2.Count; j++)
					{
						var drawPos = joint2[j] + Projectile.Center;
						var direction = joint2[j - 1] - joint2[j];
						direction = direction.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 8;
						Color drawColor = Lighting.GetColor(drawPos.ToTileCoordinates());
						drawPos -= Main.screenPosition;
						float xCoord = j - joint2.Count + 1;
						xCoord /= StretchTime;
						xCoord = 1 - xCoord;
						bars.Add(drawPos + direction, drawColor, new Vector3(xCoord, 0, 0));
						bars.Add(drawPos - direction, drawColor, new Vector3(xCoord, 1, 0));
					}
				}
				if (bars.Count > 0)
				{
					Main.graphics.GraphicsDevice.Textures[0] = tex;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		return false;
	}
}
