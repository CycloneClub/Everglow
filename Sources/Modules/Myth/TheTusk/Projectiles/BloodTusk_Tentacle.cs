using Everglow.Commons.DataStructures;
using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class BloodTusk_Tentacle : ModProjectile
{
	public List<Vector2> TentaclePoints = new List<Vector2>();
	public NPC BloodTuskOwner;
	public Vector2 LinePostions = Vector2.zeroVector;
	public int RemovedCount = 0;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.timeLeft = 3000;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 80;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6400;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = 0;
		int index = -1;
		if (BloodTuskOwner is null)
		{
			index = NPC.FindFirstNPC(ModContent.NPCType<BloodTusk>());
		}
		if (index is >= 0 and < 200)
		{
			BloodTuskOwner = Main.npc[index];
		}
		else
		{
			Projectile.active = false;
			return;
		}
		if (BloodTuskOwner is null)
		{
			Projectile.active = false;
			return;
		}
		Projectile.ai[0] = 0.07f;
		Projectile.Center = BloodTuskOwner.Bottom + new Vector2(0, -62);
		LinePostions = Vector2.zeroVector;
		RemovedCount = 0;
	}

	public override void AI()
	{
		Projectile.Center = BloodTuskOwner.Bottom + new Vector2(0, -62);
		if (Projectile.timeLeft > 1000)
		{
			if (Projectile.timeLeft % 15 == 0)
			{
				TentaclePoints.Add(LinePostions);
			}
			if (Projectile.timeLeft < 1600)
			{
				if (Projectile.extraUpdates > 6 && Projectile.timeLeft % 6 == 0)
				{
					Projectile.extraUpdates--;
				}
				float rotatedValue = (1600 - Projectile.timeLeft) / 600f;
				rotatedValue = MathF.Pow(rotatedValue, 4);
				Projectile.velocity = Projectile.velocity.RotatedBy(-Projectile.ai[0] * rotatedValue * Projectile.ai[1]);
			}
			else
			{
				Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.timeLeft * 0.01f) * 0.002f * Projectile.ai[1]);
			}
		}
		else if (Projectile.timeLeft > 901)
		{
			Projectile.extraUpdates = 0;
			float x = (1000 - Projectile.timeLeft) * 0.02f;
			float omega = (x * x - 1.4f * x) * 0.01f;
			if (omega > 0)
			{
				omega *= 2200 * omega;
			}

			Projectile.rotation += omega * Projectile.ai[1];
		}
		else if (Projectile.timeLeft > 850)
		{
			float omega = (Projectile.timeLeft - 850) / 2000f;
			Projectile.rotation -= omega * Projectile.ai[1];
		}
		if (Projectile.timeLeft < 890)
		{
			if (TentaclePoints.Count > 1)
			{
				TentaclePoints.RemoveAt(TentaclePoints.Count - 1);
				RemovedCount++;
			}
			else
			{
				Projectile.Kill();
				return;
			}
		}
		LinePostions += Projectile.velocity;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float value = 0;
		for (int i = 0; i < TentaclePoints.Count - 1; i++)
		{
			int endCount = Math.Clamp((Projectile.timeLeft - 1000) / 3, 50, 900);
			float scale = 1f;
			if (TentaclePoints.Count - i < endCount)
			{
				scale *= (TentaclePoints.Count - i) / (float)endCount;
			}
			if (TentaclePoints[i].Length() < 60)
			{
				scale *= TentaclePoints[i].Length() / 60f;
			}
			Vector2 length = Utils.SafeNormalize(TentaclePoints[i + 1] - TentaclePoints[i], Vector2.One).RotatedBy(MathHelper.PiOver2) * 20;
			Vector2 wave = length * GetWave(i);
			length = Utils.SafeNormalize(TentaclePoints[i + 1] + length * GetWave(i + 1) - TentaclePoints[i] - length * GetWave(i), Vector2.One).RotatedBy(MathHelper.PiOver2 * Projectile.ai[1]) * 100 * scale;
			Vector2 pos0 = GetPaintPos(TentaclePoints[i] + length * 0.1f + wave);
			Vector2 pos1 = GetPaintPos(TentaclePoints[i] + length + wave);
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), pos0, pos1, 10, ref value))
			{
				return true;
			}
		}
		return false;
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (TentaclePoints.Count <= 1)
		{
			return false;
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null);
		Effect dissolve = ModAsset.BloodMembrane.Value;
		float duration = 1f;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(duration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.4f, 0, 0, 0.8f) * lightColor.ToVector4());
		dissolve.Parameters["uNoiseSize"].SetValue(0.7f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		Texture2D rope = ModAsset.BloodTusk_Tentacle.Value;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> barsTusk = new List<Vertex2D>();
		List<Vertex2D> barsTusk2 = new List<Vertex2D>();
		for (int i = 0; i < TentaclePoints.Count - 1; i++)
		{
			AddVertex(bars, i);
			if ((i + RemovedCount) % 10 == 0)
			{
				AddVertexTusk(barsTusk, i);
			}
			if ((i + RemovedCount) % 2 == 1)
			{
				AddVertexTusk2(barsTusk, i);
			}
		}
		Main.graphics.graphicsDevice.Textures[0] = ModAsset.Tusk_ground.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, barsTusk.ToArray(), 0, barsTusk.Count / 3);

		Main.graphics.graphicsDevice.Textures[0] = ModAsset.Tusk_ground.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, barsTusk2.ToArray(), 0, barsTusk2.Count / 3);

		Main.graphics.graphicsDevice.Textures[0] = rope;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public float GetWave(float value)
	{
		float limitToNPC = 1;
		if (value < 30)
		{
			limitToNPC = value / 30f;
			limitToNPC = MathF.Pow(limitToNPC, 0.4f);
		}
		value = TentaclePoints.Count - value;
		value -= 30;
		if (value <= 0)
		{
			return 0;
		}
		value = value / (TentaclePoints.Count - 30f);
		value = MathF.Pow(value, 1.7f);
		float sinValue = MathF.Sin(value * 2.3f + Projectile.timeLeft * 0.005f) * Projectile.ai[1];
		value *= sinValue * MathF.Log(TentaclePoints.Count) * 0.85f * limitToNPC;
		return value;
	}

	public Vector2 GetPaintPos(Vector2 pos)
	{
		return pos.RotatedBy(Projectile.rotation) + Projectile.Center;
	}

	public void AddVertexTusk(List<Vertex2D> bars, int index)
	{
		int endCount = Math.Clamp((Projectile.timeLeft - 1000) / 3, 50, 900);
		int frame = (int)MathF.Floor(MathF.Sin(index + RemovedCount) * 21);
		float scale = MathF.Sin((index + RemovedCount) * 17) * 0.15f + 0.6f;
		if (TentaclePoints.Count - index < endCount)
		{
			scale *= (TentaclePoints.Count - index) / (float)endCount;
		}
		if (TentaclePoints[index].Length() < 60)
		{
			scale *= TentaclePoints[index].Length() / 60f;
		}
		Vector2 length = Utils.SafeNormalize(TentaclePoints[index + 1] - TentaclePoints[index], Vector2.One).RotatedBy(MathHelper.PiOver2) * 20;
		Vector2 wave = length * GetWave(index);
		length = Utils.SafeNormalize(TentaclePoints[index + 1] + length * GetWave(index + 1) - TentaclePoints[index] - length * GetWave(index), Vector2.One).RotatedBy(MathHelper.PiOver2 * Projectile.ai[1]) * 150 * scale;
		Vector2 width = Utils.SafeNormalize(length, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2 * Projectile.ai[1]) * 12 * scale;
		Vector2 pos0 = GetPaintPos(TentaclePoints[index] + length * 0.1f + wave - width);
		Vector2 pos1 = GetPaintPos(TentaclePoints[index] + length * 0.1f + wave + width);
		Vector2 pos2 = GetPaintPos(TentaclePoints[index] + length + wave - width);
		Vector2 pos3 = GetPaintPos(TentaclePoints[index] + length + wave + width);
		bars.Add(pos0, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(0 + frame / 6f, 1, 0));
		bars.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 1, 0));
		bars.Add(pos2, Lighting.GetColor(pos2.ToTileCoordinates()), new Vector3(0 + frame / 6f, 0, 0));

		bars.Add(pos2, Lighting.GetColor(pos2.ToTileCoordinates()), new Vector3(0 + frame / 6f, 0, 0));
		bars.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 1, 0));
		bars.Add(pos3, Lighting.GetColor(pos3.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 0, 0));
	}

	public void AddVertexTusk2(List<Vertex2D> bars, int index)
	{
		int endCount = Math.Clamp((Projectile.timeLeft - 1000) / 10, 10, 900);
		int frame = (int)MathF.Floor(MathF.Sin(index + RemovedCount) * 21);
		float scale = MathF.Sin((index + RemovedCount) * 17) * 0.25f + 0.9f;
		if (TentaclePoints.Count - index < endCount)
		{
			scale *= (TentaclePoints.Count - index) / (float)endCount;
		}
		if (TentaclePoints[index].Length() < 60)
		{
			scale *= TentaclePoints[index].Length() / 60f;
		}
		Vector2 length = Utils.SafeNormalize(TentaclePoints[index + 1] - TentaclePoints[index], Vector2.One).RotatedBy(MathHelper.PiOver2) * 20;
		Vector2 wave = length * GetWave(index);
		length = Utils.SafeNormalize(TentaclePoints[index + 1] + length * GetWave(index + 1) - TentaclePoints[index] - length * GetWave(index), Vector2.One).RotatedBy(MathHelper.PiOver2 * Projectile.ai[1]) * 35 * scale;
		Vector2 width = Utils.SafeNormalize(length, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2 * Projectile.ai[1]) * 5 * scale;
		Vector2 pos0 = GetPaintPos(TentaclePoints[index] + length * 0.3f + wave - width);
		Vector2 pos1 = GetPaintPos(TentaclePoints[index] + length * 0.3f + wave + width);
		Vector2 pos2 = GetPaintPos(TentaclePoints[index] + length * 1.5f + wave - width);
		Vector2 pos3 = GetPaintPos(TentaclePoints[index] + length * 1.5f + wave + width);
		bars.Add(pos0, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(0 + frame / 6f, 1, 0));
		bars.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 1, 0));
		bars.Add(pos2, Lighting.GetColor(pos2.ToTileCoordinates()), new Vector3(0 + frame / 6f, 0, 0));

		bars.Add(pos2, Lighting.GetColor(pos2.ToTileCoordinates()), new Vector3(0 + frame / 6f, 0, 0));
		bars.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 1, 0));
		bars.Add(pos3, Lighting.GetColor(pos3.ToTileCoordinates()), new Vector3(1 / 6f + frame / 6f, 0, 0));
	}

	public void AddVertex(List<Vertex2D> bars, int index)
	{
		int endCount = 30;
		float coordsX;
		float coordsY = 0;
		if (TentaclePoints.Count < endCount)
		{
			coordsX = (endCount - TentaclePoints.Count + index) / (endCount - 1f);
			coordsY = 0.5f;
		}
		else
		{
			int endAt = Math.Max(TentaclePoints.Count - endCount, 0);
			if (index < TentaclePoints.Count - endCount)
			{
				coordsX = (TentaclePoints.Count - endCount - index) / 40f;
			}
			else if (index > TentaclePoints.Count - endCount)
			{
				coordsX = (index - endAt) / (endCount - 1f);
				coordsY = 0.5f;
			}
			else
			{
				Vector2 widthCritical = Utils.SafeNormalize(TentaclePoints[endAt + 1] - TentaclePoints[endAt], Vector2.One).RotatedBy(MathHelper.PiOver2) * 20 * Projectile.ai[1];
				Vector2 pos0 = GetPaintPos(TentaclePoints[endAt] + widthCritical);
				Vector2 pos1 = GetPaintPos(TentaclePoints[endAt]);
				bars.Add(pos0, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(0, 0, 0));
				bars.Add(pos1, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(0, 0.5f, 0));
				bars.Add(pos0, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(0, 0.5f, 0));
				bars.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(0, 1, 0));
				return;
			}
		}
		Vector2 width = Utils.SafeNormalize(TentaclePoints[index + 1] - TentaclePoints[index], Vector2.One).RotatedBy(MathHelper.PiOver2) * 20;
		Vector2 wave = width * GetWave(index);
		width = Utils.SafeNormalize(TentaclePoints[index + 1] + width * GetWave(index + 1) - TentaclePoints[index] - width * GetWave(index), Vector2.One).RotatedBy(MathHelper.PiOver2) * 20 * Projectile.ai[1];
		bars.Add(GetPaintPos(TentaclePoints[index] + width + wave), Lighting.GetColor(GetPaintPos(TentaclePoints[index] + width).ToTileCoordinates()), new Vector3(coordsX, coordsY, 0));
		bars.Add(GetPaintPos(TentaclePoints[index] + wave), Lighting.GetColor(GetPaintPos(TentaclePoints[index]).ToTileCoordinates()), new Vector3(coordsX, coordsY + 0.5f, 0));
	}
}
