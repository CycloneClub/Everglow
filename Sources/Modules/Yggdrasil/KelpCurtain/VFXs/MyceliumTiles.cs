using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(MyceliumTilesPipeline))]
public class MyceliumTiles : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public Vector2 Position;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public int MyceliumAmount = 0;
	public bool Wither = false;
	public Point RootPos;
	public int KillTimer = 40;
	public Projectile LockProjectile;
	public WoodlandWraithStaff_FungiBall FungiBall;

	public List<Point> ContinueTiles = new List<Point>();

	public List<Projectile> SporeZones = new List<Projectile>();

	public override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (MyceliumAmount < 256)
		{
			MyceliumAmount += 10;
			Rotation = -YggdrasilWorldGeneration.TerrianSurfaceAngle(RootPos.X, RootPos.Y, 8) + MathHelper.PiOver2;
			if (MyceliumAmount >= 256)
			{
				MyceliumAmount = 256;
			}
			ContinueTiles = new List<Point>();
			BFSContinueTile(RootPos);
		}
		if (FungiBall == null)
		{
			Active = false;
		}
		if (!Wither)
		{
			Timer = 0;
			if (FungiBall.State != WoodlandWraithStaff_FungiBall.States.Mycelume || !FungiBall.Projectile.active)
			{
				Wither = true;
				Timer = MaxTime - KillTimer;
			}
		}
		SporeZones = new List<Projectile>();
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<WoodlandWraithStaff_SporeZone>() && proj.owner == LockProjectile.owner)
			{
				SporeZones.Add(proj);
			}
		}
	}

	public override void Draw()
	{
		if (ContinueTiles.Count > 0)
		{
			foreach (var point in ContinueTiles)
			{
				DrawTilePiece(point);
			}
		}
	}

	public float ZoneSporeFade(Vector2 checkPos)
	{
		float maxFade = 0;
		foreach (var proj in SporeZones)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<WoodlandWraithStaff_SporeZone>() && proj.owner == LockProjectile.owner)
			{
				WoodlandWraithStaff_SporeZone wWSSZ = proj.ModProjectile as WoodlandWraithStaff_SporeZone;
				if (Vector2.Distance(proj.Center, checkPos) < wWSSZ.Range)
				{
					if (Math.Min(proj.timeLeft / 60f, 1f) > maxFade)
					{
						maxFade = Math.Min(proj.timeLeft / 60f, 1f);
					}
				}
			}
		}
		return maxFade;
	}

	public void DrawTilePiece(Point pos)
	{
		List<Vertex2DMycelium> bars = new List<Vertex2DMycelium>();
		var tile = TileUtils.SafeGetTile(pos);
		var texTile = TextureAssets.Tile[tile.TileType].Value;
		var addPos = RootPos - pos;
		float zValue = MathF.Pow(MyceliumAmount / 256f, 6) * 19.9f;
		float size = 1 / 24f;
		var pos0 = pos.ToWorldCoordinates() + new Vector2(-8, -8);
		var pos1 = pos.ToWorldCoordinates() + new Vector2(8, -8);
		var pos2 = pos.ToWorldCoordinates() + new Vector2(-8, 8);
		var pos3 = pos.ToWorldCoordinates() + new Vector2(8, 8);
		var color0 = Lighting.GetColor(pos0.ToTileCoordinates());
		var color1 = Lighting.GetColor(pos1.ToTileCoordinates());
		var color2 = Lighting.GetColor(pos2.ToTileCoordinates());
		var color3 = Lighting.GetColor(pos3.ToTileCoordinates());
		color3.A = color2.A = color1.A = color0.A = 0;
		color0.R += 30;
		color1.R += 30;
		color2.R += 30;
		color3.R += 30;
		color0 *= 0.6f;
		color1 *= 0.6f;
		color2 *= 0.6f;
		color3 *= 0.6f;
		Color powerfulColor = new Color(0.1f, 0.0f, 0.4f, 0);
		color0 = Color.Lerp(color0, powerfulColor, ZoneSporeFade(pos0) * 0.3f);
		color1 = Color.Lerp(color1, powerfulColor, ZoneSporeFade(pos1) * 0.3f);
		color2 = Color.Lerp(color2, powerfulColor, ZoneSporeFade(pos2) * 0.3f);
		color3 = Color.Lerp(color3, powerfulColor, ZoneSporeFade(pos3) * 0.3f);
		float coord2Z = 0;
		if (Timer > MaxTime - KillTimer)
		{
			coord2Z = (Timer - MaxTime + KillTimer) / KillTimer;
		}
		bars.Add(new Vertex2DMycelium(pos0, color0, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(0.5f, 0.5f)) * size, Rotation), zValue), new Vector3(tile.TileFrameX / (float)texTile.Width, tile.TileFrameY / (float)texTile.Height, coord2Z)));
		bars.Add(new Vertex2DMycelium(pos1, color1, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(-0.5f, 0.5f)) * size, Rotation), zValue), new Vector3((tile.TileFrameX + 16) / (float)texTile.Width, tile.TileFrameY / (float)texTile.Height, coord2Z)));
		bars.Add(new Vertex2DMycelium(pos2, color2, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(0.5f, -0.5f)) * size, Rotation), zValue), new Vector3(tile.TileFrameX / (float)texTile.Width, (tile.TileFrameY + 16) / (float)texTile.Height, coord2Z)));

		bars.Add(new Vertex2DMycelium(pos2, color2, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(0.5f, -0.5f)) * size, Rotation), zValue), new Vector3(tile.TileFrameX / (float)texTile.Width, (tile.TileFrameY + 16) / (float)texTile.Height, coord2Z)));
		bars.Add(new Vertex2DMycelium(pos1, color1, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(-0.5f, 0.5f)) * size, Rotation), zValue), new Vector3((tile.TileFrameX + 16) / (float)texTile.Width, tile.TileFrameY / (float)texTile.Height, coord2Z)));
		bars.Add(new Vertex2DMycelium(pos3, color3, new Vector3(GetRotVec((addPos.ToVector2() + new Vector2(-0.5f, -0.5f)) * size, Rotation), zValue), new Vector3((tile.TileFrameX + 16) / (float)texTile.Width, (tile.TileFrameY + 16) / (float)texTile.Height, coord2Z)));

		if (bars.Count >= 3)
		{
			Ins.Batch.Draw(texTile, bars, PrimitiveType.TriangleList);
		}
	}

	public Vector2 GetRotVec(Vector2 oldVec, float rot)
	{
		return oldVec.RotatedBy(rot);
	}

	public void BFSContinueTile(Point checkPoint)
	{
		ContinueTiles = new List<Point>();
		int maxContinueCount = MyceliumAmount;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),
		};
		Queue<Point> queueChecked = new Queue<Point>();

		// 将起始点加入队列
		queueChecked.Enqueue(checkPoint);
		List<Point> visited = new List<Point>();

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);
				Tile tile = TileUtils.SafeGetTile(checkX, checkY);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					tile.HasTile && !visited.Contains(point))
				{
					bool canAdd = true;
					foreach (var proj in Main.projectile)
					{
						if (proj != null && proj.active && proj != LockProjectile && proj.type == LockProjectile.type)
						{
							WoodlandWraithStaff_FungiBall wWSFB = proj.ModProjectile as WoodlandWraithStaff_FungiBall;
							if (wWSFB != null)
							{
								if (wWSFB.ContinueTiles.Contains(point))
								{
									canAdd = false;
									break;
								}
							}
						}
					}
					if (canAdd)
					{
						queueChecked.Enqueue(point);
						visited.Add(point);
					}
				}
			}
			if (queueChecked.Count > maxContinueCount || visited.Count > maxContinueCount)
			{
				break;
			}
		}
		ContinueTiles = visited;
	}
}
