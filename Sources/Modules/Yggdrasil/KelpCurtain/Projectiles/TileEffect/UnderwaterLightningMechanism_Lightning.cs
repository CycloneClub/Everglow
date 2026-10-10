using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;

public class UnderwaterLightningMechanism_Lightning : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public Vector2 StartPos;

	public Vector2 EndPos;

	public List<Vector2> LightningTrail = new List<Vector2>();

	public bool Initialized = false;

	public int Timer = 0;

	public List<Point> ContinueAirTilesArea = new List<Point>();

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.extraUpdates = 1;
	}

	public override void AI()
	{
		Timer++;
		if (!Initialized)
		{
			if (Collision.IsWorldPointSolid(StartPos) || Collision.IsWorldPointSolid(EndPos))
			{
				Projectile.Kill();
				return;
			}
			BFSContinueEmpty();
			Initialized = true;
		}
		else if (LightningTrail.Count > 0)
		{
			List<Vector2> newLightningTrail = new List<Vector2>();
			newLightningTrail.Add(LightningTrail[0]);
			for (int i = 1; i < LightningTrail.Count; i++)
			{
				var oldTrail = LightningTrail[i - 1];
				var distance = LightningTrail[i] - oldTrail;
				if (distance.Length() > 16)
				{
					int addJointCount = (int)(distance.Length() / 16f);
					for (int c = 0; c < addJointCount; c++)
					{
						float lerpVec = (c + 1) / (addJointCount + 1f);
						newLightningTrail.Add(oldTrail * (1 - lerpVec) + LightningTrail[i] * lerpVec);
					}
				}
				newLightningTrail.Add(LightningTrail[i]);
			}
			LightningTrail = new List<Vector2>();
			LightningTrail.AddRange(newLightningTrail);
			for (int i = 1; i < LightningTrail.Count - 1; i++)
			{
				Vector2 dirWidth = (LightningTrail[i] - LightningTrail[i - 1]).NormalizeSafe().RotatedBy(MathHelper.PiOver2);
				Vector2 randomMove = dirWidth * GetRandomValueMove(i / (float)LightningTrail.Count * 10) * 0.55f;
				if (i < 5)
				{
					randomMove *= i / 5f;
				}
				if (i > LightningTrail.Count - 7)
				{
					randomMove *= (LightningTrail.Count - i - 1) / 5f;
				}
				int safeCount = 0;
				while (Collision.IsWorldPointSolid(LightningTrail[i] + randomMove))
				{
					randomMove = randomMove.RotatedBy(MathHelper.PiOver2);
					safeCount++;
					if (safeCount >= 4)
					{
						randomMove *= 0;
						break;
					}
				}
				LightningTrail[i] += randomMove;
				Vector2 rejectiveForce = Vector2.zeroVector;
				Vector2 toBefore = LightningTrail[i] - LightningTrail[i - 1];
				Vector2 toNext = LightningTrail[i] - LightningTrail[i + 1];
				if (toBefore.Length() < 10)
				{
					rejectiveForce += toBefore.NormalizeSafe() / (toBefore.Length() + 2) * 3;
				}
				if (toNext.Length() < 10)
				{
					rejectiveForce += toNext.NormalizeSafe() / (toNext.Length() + 2) * 3;
				}
				LightningTrail[i] += rejectiveForce;
			}
		}
	}

	public void BFSContinueEmpty()
	{
		int maxContinueCount = 4096;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),
		};
		Queue<Point> queueCheckedStart = new Queue<Point>();
		Queue<Point> queueCheckedEnd = new Queue<Point>();

		// 将起始点加入队列
		queueCheckedStart.Enqueue(StartPos.ToTileCoordinates());
		queueCheckedEnd.Enqueue(EndPos.ToTileCoordinates());
		List<Point> visitedStart = new List<Point>();
		List<Point> visitedEnd = new List<Point>();

		List<Point> visitedStartDir = new List<Point>();
		List<Point> visitedEndDir = new List<Point>();

		Point intersectPoint = default(Point);
		while (queueCheckedStart.Count > 0)
		{
			var tilePosStart = queueCheckedStart.Dequeue();
			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePosStart.X + dx;
				int checkY = tilePosStart.Y + dy;
				Point point = new Point(checkX, checkY);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					!Collision.IsWorldPointSolid(point.ToWorldCoordinates()) && !visitedStart.Contains(point))
				{
					queueCheckedStart.Enqueue(point);
					visitedStart.Add(point);
					visitedStartDir.Add(new Point(-dx, -dy));
				}
			}
			if (queueCheckedStart.Count > maxContinueCount || visitedStart.Count > maxContinueCount)
			{
				break;
			}
			var tilePosEnd = queueCheckedEnd.Dequeue();
			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePosEnd.X + dx;
				int checkY = tilePosEnd.Y + dy;
				Point point = new Point(checkX, checkY);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					!Collision.IsWorldPointSolid(point.ToWorldCoordinates()) && !visitedEnd.Contains(point))
				{
					queueCheckedEnd.Enqueue(point);
					visitedEnd.Add(point);
					visitedEndDir.Add(new Point(-dx, -dy));
				}
			}
			if (queueCheckedEnd.Count > maxContinueCount || visitedEnd.Count > maxContinueCount)
			{
				break;
			}
			if (visitedStart.Intersect(visitedEnd).ToList().Count > 0)
			{
				intersectPoint = visitedStart.Intersect(visitedEnd).ToArray()[0];
				break;
			}
		}
		ContinueAirTilesArea.AddRange(visitedEnd);
		ContinueAirTilesArea.AddRange(visitedStart);
		List<Point> trailToStart = new List<Point>();
		int intersectIndex = visitedStart.IndexOf(intersectPoint);
		if (intersectIndex > -1)
		{
			Point checkPoint = intersectPoint;
			for (int i = 0; i < 999; i++)
			{
				int checkIndex = visitedStart.IndexOf(checkPoint);
				trailToStart.Add(checkPoint);
				if (checkIndex <= -1)
				{
					break;
				}
				checkPoint += visitedStartDir[checkIndex];
				if (checkPoint == StartPos.ToTileCoordinates())
				{
					trailToStart.Add(checkPoint);
					break;
				}
			}
		}

		trailToStart.Reverse();
		List<Point> trailToEnd = new List<Point>();
		intersectIndex = visitedEnd.IndexOf(intersectPoint);
		if (intersectIndex > -1)
		{
			Point checkPoint = intersectPoint;
			for (int i = 0; i < 999; i++)
			{
				int checkIndex = visitedEnd.IndexOf(checkPoint);
				if (checkIndex <= -1)
				{
					break;
				}
				checkPoint += visitedEndDir[checkIndex];
				trailToEnd.Add(checkPoint);
				if (checkPoint == EndPos.ToTileCoordinates())
				{
					break;
				}
			}
		}

		List<Point> lightningTrail = [.. trailToStart, .. trailToEnd];

		// The shortest way to connect StartPos and EndPos.
		LightningTrail = new List<Vector2>();
		for (int i = 0; i < lightningTrail.Count; i++)
		{
			if (i == 0 || i == lightningTrail.Count - 1)
			{
				LightningTrail.Add(lightningTrail[i].ToWorldCoordinates());
			}
			else
			{
				Vector2 back = lightningTrail[i].ToWorldCoordinates() - lightningTrail[i - 1].ToWorldCoordinates();
				Vector2 front = lightningTrail[i + 1].ToWorldCoordinates() - lightningTrail[i].ToWorldCoordinates();
				if ((front - back).Length() > 1)
				{
					LightningTrail.Add(lightningTrail[i].ToWorldCoordinates());
				}
			}
		}
		List<Vector2> LightningTrailOblique = new List<Vector2>();
		int lastJoint = 0;
		if (LightningTrail.Count > 0)
		{
			for (int i = 0; i < LightningTrail.Count; i++)
			{
				if (i == 0)
				{
					LightningTrailOblique.Add(LightningTrail[i]);
				}
				if (i > 0 && i <= LightningTrail.Count - 1)
				{
					if (!LineInSafeArea(LightningTrail[lastJoint], LightningTrail[i]))
					{
						LightningTrailOblique.Add(LightningTrail[i - 1]);
						lastJoint = i - 1;
					}
				}
				if (i == LightningTrail.Count - 1)
				{
					LightningTrailOblique.Add(LightningTrail[i]);
				}
			}
		}
		LightningTrail = GraphicsUtils.CatmullRom(LightningTrailOblique.ToList());
		for (int i = 0; i < LightningTrail.Count; i++)
		{
			LightningTrail[i] += GetMarginalForce(LightningTrail[i]);
		}
	}

	public Vector2 GetMarginalForce(Vector2 pos)
	{
		Vector2 marginalRejectionalForce = Vector2.zeroVector;
		float forceSize = 0.3f;
		for (int i = 0; i < 8; i++)
		{
			Vector2 v0 = new Vector2(0, -16).RotatedBy(i / 8f * MathHelper.TwoPi);
			if (ContinueAirTilesArea.Contains((v0 + pos).ToTileCoordinates()))
			{
				marginalRejectionalForce += v0 * forceSize;
			}
			else
			{
				marginalRejectionalForce -= v0 * forceSize;
			}
		}
		forceSize = 0.01f;
		for (int i = 0; i < 16; i++)
		{
			Vector2 v0 = new Vector2(0, -48).RotatedBy(i / 16f * MathHelper.TwoPi);
			if (ContinueAirTilesArea.Contains((v0 + pos).ToTileCoordinates()))
			{
				marginalRejectionalForce += v0 * forceSize;
			}
			else
			{
				marginalRejectionalForce -= v0 * forceSize;
			}
		}
		return marginalRejectionalForce;
	}

	public bool LineInSafeArea(Vector2 pos0, Vector2 pos1)
	{
		Vector2 route = pos1 - pos0;
		Vector2 step = route.NormalizeSafe() * 16f;
		int times = (int)(route.Length() / 16f);
		Vector2 checkPoint = pos0;
		for (int k = 0; k < times; k++)
		{
			if (!ContinueAirTilesArea.Contains(checkPoint.ToTileCoordinates()))
			{
				return false;
			}
			checkPoint += step;
		}
		if (!ContinueAirTilesArea.Contains(pos1.ToTileCoordinates()))
		{
			return false;
		}
		return true;
	}

	public float GetRandomValueMove(float index)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			value += MathF.Sin((i + index) * MathF.Pow(2, i) + (float)Main.time / 256f + Projectile.ai[0]) * MathF.Pow(2, -i);
		}
		return value;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (LightningTrail.Count <= 1)
		{
			return false;
		}
		for (int i = 0; i < LightningTrail.Count; i++)
		{
			Vector2 sizeHalf = new Vector2(16);
			Vector2 checkTopLeft = LightningTrail[i] - sizeHalf;
			Rectangle check = new Rectangle((int)checkTopLeft.X, (int)checkTopLeft.Y, 32, 32);
			if (check.Intersects(targetHitbox))
			{
				return true;
			}
		}
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (LightningTrail.Count <= 1)
		{
			return false;
		}
		Texture2D tex = Commons.ModAsset.Trail_10.Value;
		float value = Timer / 60f;
		GradientColor gradientColor = new GradientColor();
		gradientColor.colorList.Add((new Color(1f, 1f, 1f, 0), 0));
		gradientColor.colorList.Add((new Color(140, 232, 255, 0), 0.2f));
		gradientColor.colorList.Add((new Color(252, 170, 255, 0), 0.4f));
		gradientColor.colorList.Add((new Color(255, 166, 114, 0), 0.6f));
		gradientColor.colorList.Add((new Color(255, 84, 81, 0), 0.8f));
		gradientColor.colorList.Add((new Color(0, 0, 0, 0), 1));
		Color drawColor = gradientColor.GetColor(value);
		float width = 10f;
		if (Timer < 10)
		{
			width += (10 - Timer) * 6;
		}
		if (Projectile.timeLeft < 60)
		{
			width *= Projectile.timeLeft / 60f;
		}
		for (int j = 0; j < 3; j++)
		{
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int i = 0; i < LightningTrail.Count; i++)
			{
				float mulWidth = 1f;
				if (i <= 5)
				{
					mulWidth += (5 - i) / 5f;
				}
				if (i >= LightningTrail.Count - 6)
				{
					mulWidth += (i + 5 - LightningTrail.Count) / 5f;
				}
				Vector2 dir = new Vector2(0, -width * mulWidth).RotatedBy(j / 3f * MathHelper.TwoPi);
				bars.Add(LightningTrail[i] + dir - Main.screenPosition, drawColor, new Vector3(i / 15f, 0, 0));
				bars.Add(LightningTrail[i] - dir - Main.screenPosition, drawColor, new Vector3(i / 15f, 1, 0));
				Lighting.AddLight(LightningTrail[i], drawColor.ToVector3() * MathF.Pow(width, 0.5f) / 10f);
			}
			if (bars.Count > 3)
			{
				Main.graphics.graphicsDevice.Textures[0] = tex;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D bloom = Commons.ModAsset.LightPoint2.Value;
		value = 1 - value;
		Vector2 scaleH = new Vector2(value, 0.5f + value * 0.5f) * MathF.Pow(width, 0.5f) / 10f;
		Main.EntitySpriteDraw(bloom, LightningTrail[0] - Main.screenPosition, null, drawColor, 0, bloom.Size() * 0.5f, MathF.Pow(width, 0.2f) * 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningTrail[0] - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningTrail[0] - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);

		Main.EntitySpriteDraw(bloom, LightningTrail[^1] - Main.screenPosition, null, drawColor, 0, bloom.Size() * 0.5f, MathF.Pow(width, 0.2f) * 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningTrail[^1] - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningTrail[^1] - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		return false;
	}
}
