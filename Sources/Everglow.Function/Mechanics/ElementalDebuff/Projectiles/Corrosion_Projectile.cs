using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Projectiles;

public class Corrosion_Projectile : ModProjectile
{
	public const int ActiveTimerMax = 120;
	public const int ExistMax_Player = 4;
	public const int ExistMax_Server = 20;

	private bool HasNotHitTargetOrTile { get; set; } = true;

	private int TargetWhoAmI => (int)Projectile.ai[0];

	private int ActiveTimer
	{
		get => (int)Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

	public override string Texture => ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.timeLeft = 300;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		ActiveProjectile();
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		ActiveTimer = ActiveTimerMax;
	}

	public override void AI()
	{
		if (Main.npc[TargetWhoAmI] is not null && Main.npc[TargetWhoAmI].active)
		{
			Projectile.Center = Main.npc[TargetWhoAmI].Center;
		}

		if (HasNotHitTargetOrTile)
		{
			Dust.NewDust(Projectile.Center, 1, 1, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y);
		}
		else
		{
			Projectile.rotation += 0.02f;

			if (Main.rand.NextBool(15) && tentacles.Count < 8)
			{
				NewTentacle();
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
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ActiveProjectile();
		if (Projectile.owner == -1)
		{
			target.AddElementalDebuffBuildUp_World(CorrosionDebuff.ID, 100);
		}
		else
		{
			target.AddElementalDebuffBuildUp(Main.player[Projectile.owner], CorrosionDebuff.ID, 100);
		}
	}

	private void ActiveProjectile()
	{
		if (HasNotHitTargetOrTile)
		{
			SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
		}
		HasNotHitTargetOrTile = false;
		Projectile.velocity = Vector2.Zero;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (Tentacle tentacle in tentacles)
		{
			foreach (Vector2 v0 in tentacle.oldPos)
			{
				var rectangle = new Rectangle((int)(Projectile.Center + v0).X - 4, (int)(Projectile.Center + v0).Y - 4, 8, 8);
				if (targetHitbox.Intersects(rectangle))
				{
					return true;
				}
			}
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTentacles(lightColor);

		float timer = 300 - Projectile.timeLeft;
		var drawColor = new Color(0.4f, 0f, 1f, 0);
		float fade = 1f;
		if (timer < 10)
		{
			fade *= timer / 10f;
		}
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var vertices = new List<Vertex2D>();
		var vertices_dark = new List<Vertex2D>();
		float timeValue = (float)Main.time * 0.003f;
		float size = 50f * Projectile.scale;
		for (int i = 0; i <= size; i++)
		{
			float value = i / size;
			var offset = new Vector2(0, size * 2).RotatedBy(value * MathHelper.TwoPi);
			vertices.Add(Projectile.Center + offset - Main.screenPosition, drawColor * 0, new Vector3(value, timeValue, 0f));
			vertices.Add(Projectile.Center - Main.screenPosition, drawColor * fade, new Vector3(value, timeValue + 0.25f, 0f));

			vertices_dark.Add(Projectile.Center + offset - Main.screenPosition, drawColor * 0, new Vector3(value, timeValue, 0f));
			vertices_dark.Add(Projectile.Center - Main.screenPosition, Color.White * fade, new Vector3(value, timeValue + 0.25f, 0f));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Noise_hiveCyber_black.Value;
		if (vertices_dark.Count > 2)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices_dark.ToArray(), 0, vertices_dark.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices_dark.ToArray(), 0, vertices_dark.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices_dark.ToArray(), 0, vertices_dark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Noise_hiveCyber.Value;
		if (vertices.Count > 2)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
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

	private List<Tentacle> tentacles = [];

	private void DrawTentacles(Color lightColor)
	{
		List<Vertex2D> vertices = [];
		foreach (Tentacle tentacle in tentacles)
		{
			// Generate tentacle vertices
			for (int current = 0; current < tentacle.oldPos.Count; current++)
			{
				Vector2 drawPos = tentacle.oldPos[current] + Projectile.Center;

				// The vector from current position to edge, perpendicular to direction
				Vector2 widthOffset = current > 0 // If current node is not the first node, then calculate the vector between the previous node and current node
					? Vector2.Normalize(tentacle.oldPos[current] - tentacle.oldPos[current - 1]).RotatedBy(MathHelper.PiOver2) * 5f
					: Vector2.Normalize(tentacle.oldPos[current]).RotatedBy(MathHelper.PiOver2) * 5f;

				// This standard determine how many 'Old Position' the tentacle tip part contains
				float factorMeasurementStandard = 220;

				// This factor represents the distance between the current 'Old Position' and the tip of the tentacle.
				// The larger the factor is, the closer the current 'Old Position' to the tip of the tentacle.
				float factor = (current + factorMeasurementStandard - tentacle.oldPos.Count) / factorMeasurementStandard;

				float width = 2;
				if (factor > 0.8f) // Special width calculation of the tentacle tip part
				{
					width *= MathF.Sin((1 - factor) * 2.5f * MathF.PI);
				}

				// The first node
				if (current == 0)
				{
					vertices.Add(Projectile.Center + widthOffset, Color.Transparent, new Vector3(factor, 0, width));
					vertices.Add(Projectile.Center - widthOffset, Color.Transparent, new Vector3(factor, 1, width));
					vertices.Add(Projectile.Center + widthOffset, lightColor, new Vector3(factor, 0, width));
					vertices.Add(Projectile.Center - widthOffset, lightColor, new Vector3(factor, 1, width));
				}

				// Body node
				Color newLightColor = Lighting.GetColor(drawPos.ToTileCoordinates());
				vertices.Add(drawPos + widthOffset, newLightColor, new Vector3(factor, 0, width));
				vertices.Add(drawPos - widthOffset, newLightColor, new Vector3(factor, 1, width));

				// The last node
				if (current == tentacle.oldPos.Count - 1)
				{
					vertices.Add(drawPos + widthOffset, Color.Transparent, new Vector3(factor, 0, width));
					vertices.Add(drawPos - widthOffset, Color.Transparent, new Vector3(factor, 1, width));
				}
			}
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect = ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Corrosion_Projectile.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (vertices.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	private void NewTentacle()
	{
		var tentacle = default(Tentacle);
		tentacle.oldPos = new List<Vector2>();
		tentacle.jointVel = new List<Vector2>();
		tentacle.time = 0;
		tentacle.position = Vector2.zeroVector;
		tentacle.velocity = new Vector2(Main.rand.NextFloat(3, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
		tentacle.ai0 = Main.rand.NextFloat(-0.3f, 0.3f);
		tentacle.ai1 = Main.rand.NextFloat(-0.001f, 0.027f) * -MathF.Sign(tentacle.ai0);
		tentacles.Add(tentacle);
	}

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
					float value = Math.Max(0, (coilValue - 0.01f - x) / coilValue);
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
				float value = Math.Max(0, (coilValue - 0.01f - x) / coilValue);
				tentacle.oldPos[x] -= tentacle.oldPos[0] * value;
			}
		}

		// When the tentacles are about to disappear, they are retracted violently
		if (Projectile.timeLeft <= 40 || ActiveTimer <= 40)
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

		tentacle.time++;

		return tentacle;
	}
}
