using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;
using static Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.LightningMechanism.UnderwaterLightningMechanismEntity;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.LightningMechanism;

public class UnderwaterLightningMechanism : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoFail[Type] = true;
		DustType = ModContent.DustType<WaterErodedGreenBrickDust>();

		// Placement - Standard Chandelier Setup Below
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = new int[5];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleLineSkip = 10;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.DrawYOffset = 0;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty; // Clear out existing bottom anchor inherited from Style1x1 temporarily so that we don't have to set it to empty in each of the alternates.

		AnchorData SolidOrSolidSideAnchor1TilesLong = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 3, 0);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = new Point16(1, 0);
		TileObjectData.newAlternate.AnchorTop = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.newAlternate.Style = 1;
		TileObjectData.addAlternate(1);

		TileObjectData.newTile.Origin = new Point16(1, 4);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 3, 0);
		TileObjectData.addTile(Type);
		AnimationFrameHeight = 90;
		AddMapEntry(new Color(101, 107, 121));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 18)
		{
			ModTileEntity.PlaceEntityNet(i, j - 4, ModContent.TileEntityType<UnderwaterLightningMechanismEntity>());
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out _);
		}
		if (tile.TileFrameX == 72)
		{
			ModTileEntity.PlaceEntityNet(i, j + 4, ModContent.TileEntityType<UnderwaterLightningMechanismEntity>());
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out _);
		}
		base.PlaceInWorld(i, j, item);
	}

	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Point entityPos = new Point(i - tile.TileFrameX % 54 / 18 + 1, j - tile.TileFrameY / 18);
		Tile entityTile = Main.tile[entityPos];
		if (entityTile.TileFrameX == 72)
		{
			entityPos = entityPos + new Point(0, 4);
		}
		entityTile = Main.tile[entityPos];
		if (TileEntity.ByPosition.TryGetValue(new Point16(entityPos.X, entityPos.Y), out TileEntity entity) && entity is UnderwaterLightningMechanismEntity mechanismEntity)
		{
			mechanismEntity.SetState(MechanismState.Resting);
			mechanismEntity.CurrentFrame = 0;
		}
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<UnderwaterLightningMechanism_Lightning>())
			{
				UnderwaterLightningMechanism_Lightning uLLML = proj.ModProjectile as UnderwaterLightningMechanism_Lightning;
				if (uLLML is not null)
				{
					if (uLLML.Timer < 6 && (uLLML.StartPos == entityPos.ToWorldCoordinates() || uLLML.EndPos == entityPos.ToWorldCoordinates()))
					{
						return;
					}
				}
			}
		}
		if (entityTile.TileFrameX == 18)
		{
			if (!Main.dedServ)
			{
				Point point = BFSFindOtherMechanism(entityPos.X, entityPos.Y);
				Projectile proj = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_PlayerOrWires(i, j, true, Main.LocalPlayer), point.ToWorldCoordinates(), Vector2.zeroVector, ModContent.ProjectileType<UnderwaterLightningMechanism_Lightning>(), 250, 1.5f, Main.myPlayer, Main.rand.NextFloat(30f));
				UnderwaterLightningMechanism_Lightning uLLML = proj.ModProjectile as UnderwaterLightningMechanism_Lightning;
				if (uLLML is not null)
				{
					uLLML.StartPos = entityPos.ToWorldCoordinates();
					uLLML.EndPos = point.ToWorldCoordinates();
					if (TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y), out TileEntity entity2) && entity2 is UnderwaterLightningMechanismEntity mechanismEntity2)
					{
						mechanismEntity2.SetState(MechanismState.Resting);
						mechanismEntity2.CurrentFrame = 0;
					}
				}
			}
		}
		if (entityTile.TileFrameX == 72)
		{
			if (!Main.dedServ)
			{
				Point point = BFSFindOtherMechanism(entityPos.X, entityPos.Y);
				Projectile proj = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_PlayerOrWires(i, j, true, Main.LocalPlayer), point.ToWorldCoordinates(), Vector2.zeroVector, ModContent.ProjectileType<UnderwaterLightningMechanism_Lightning>(), 250, 1.5f, Main.myPlayer, Main.rand.NextFloat(30f));
				UnderwaterLightningMechanism_Lightning uLLML = proj.ModProjectile as UnderwaterLightningMechanism_Lightning;
				if (uLLML is not null)
				{
					uLLML.StartPos = entityPos.ToWorldCoordinates();
					uLLML.EndPos = point.ToWorldCoordinates();
					if (TileEntity.ByPosition.TryGetValue(new Point16(point.X, point.Y), out TileEntity entity2) && entity2 is UnderwaterLightningMechanismEntity mechanismEntity2)
					{
						mechanismEntity2.SetState(MechanismState.Resting);
						mechanismEntity2.CurrentFrame = 0;
					}
				}
			}
		}
	}

	public Point BFSFindOtherMechanism(int i, int j)
	{
		int maxContinueCount = 4096;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),
		};
		Queue<Point> queueChecked = new Queue<Point>();
		Tile tile = Main.tile[i, j];

		// 将起始点加入队列
		queueChecked.Enqueue(new Point(i, j));
		List<Point> visited = new List<Point>();
		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();
			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);
				Tile checkTile = TileUtils.SafeGetTile(point);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					!Collision.IsWorldPointSolid(point.ToWorldCoordinates()) && !visited.Contains(point))
				{
					queueChecked.Enqueue(point);
					visited.Add(point);
					int endType = -1;
					Point solve = new Point(checkX, checkY);
					if (checkTile.TileType == ModContent.TileType<UnderwaterLightningMechanism_H>())
					{
						if (checkTile.TileFrameX < 90)
						{
							solve = new Point(checkX - checkTile.TileFrameX / 18, checkY - checkTile.TileFrameY / 18 + 1);
							endType = 2;
						}
						if (checkTile.TileFrameX >= 90)
						{
							solve = new Point(checkX - (checkTile.TileFrameX - 90) / 18 + 4, checkY - checkTile.TileFrameY / 18 + 1);
							endType = 3;
						}
					}
					if (checkTile.TileType == ModContent.TileType<UnderwaterLightningMechanism>())
					{
						if (checkTile.TileFrameX < 54)
						{
							solve = new Point(checkX - checkTile.TileFrameX / 18 + 1, checkY - checkTile.TileFrameY / 18);
							endType = 0;
						}
						if (checkTile.TileFrameX >= 54)
						{
							solve = new Point(checkX - (checkTile.TileFrameX - 54) / 18 + 1, checkY - checkTile.TileFrameY / 18 + 4);
							endType = 1;
						}
					}
					if (endType >= 0 && solve != new Point(i, j))
					{
						return solve;
					}
				}
			}
			if (queueChecked.Count > maxContinueCount || visited.Count > maxContinueCount)
			{
				break;
			}
		}
		return new Point(0, 0);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		r = 0f;
		g = 0f;
		b = 0f;
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		Tile tile = Main.tile[i, j];
		Point16 entityPoint = new Point16(i - tile.TileFrameX % 54 / 18 + 1, j - tile.TileFrameY / 18);
		Tile entityTile = Main.tile[entityPoint];
		if (entityTile.TileFrameX == 72)
		{
			entityPoint += new Point16(0, 4);
		}
		if (TileEntity.ByPosition.TryGetValue(entityPoint, out TileEntity entity) && entity is UnderwaterLightningMechanismEntity mechanismEntity)
		{
			frameYOffset = mechanismEntity.GetFrame() * AnimationFrameHeight; // 获取状态帧
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return true;
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		Texture2D glowTex = ModAsset.UnderwaterLightningMechanism_glow.Value;
		Point16 entityPoint = new Point16(i - tile.TileFrameX % 54 / 18 + 1, j - tile.TileFrameY / 18);
		Tile entityTile = Main.tile[entityPoint];
		if (entityTile.TileFrameX == 72)
		{
			entityPoint += new Point16(0, 4);
		}
		int frameYOffset = 0;
		if (TileEntity.ByPosition.TryGetValue(entityPoint, out TileEntity entity) && entity is UnderwaterLightningMechanismEntity mechanismEntity)
		{
			frameYOffset = mechanismEntity.GetFrame() * AnimationFrameHeight; // 获取状态帧
		}
		spriteBatch.Draw(glowTex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(8), 1f, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		ModContent.GetInstance<UnderwaterLightningMechanismEntity>().Kill(i, j);
	}
}
