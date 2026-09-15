using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.WorldGeneration;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class JadeLakeRedAlgae : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		// TileObjectData assignment
		// The TileID.Signs TileObjectData doesn't set StyleMultiplier to 5, so we will not be copying from it in this case
		// Using Style1x1 as a base, we will create a TileObjectData with 5 alternate placements, each anchoring to a different anchor.
		// We also adjust the Origin for the alternates to match vanilla. Style1x1 starts with a origin at 0, 1 and a AnchorBottom, these will both be adjusted in the alternates.
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 5; // Since each style has 5 placement styles, we set this to 5.
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty; // Clear out existing bottom anchor inherited from Style1x1 temporarily so that we don't have to set it to empty in each of the alternates.

		// To reduce code repetition, we'll use the same AnchorData value multiple times. This works because the tile is as tall as it is wide.
		AnchorData SolidOrSolidSideAnchor1TilesLong = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 0);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorTop = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(1);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorLeft = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(2);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorRight = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addAlternate(3);

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Origin = Point16.Zero;
		TileObjectData.newAlternate.AnchorWall = true;
		TileObjectData.addAlternate(4);

		// Finally, we restore the default AnchorBottom, the extra AnchorTypes here allow placing on tables, platforms, and other tiles.
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 1, 0);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<JadeLakeRedAlgaeDust>();

		AddMapEntry(new Color(168, 15, 45));
		HitSound = SoundID.Grass;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (closer && Main.expertMode)
		{
			foreach (var player in Main.player)
			{
				if (player.Hitbox.Intersects(new Rectangle(i * 16 - 8, j * 16 - 8, 32, 32)))
				{
					player.velocity *= 0.9f;
					player.AddBuff(ModContent.BuffType<RedAlgaeDebuff>(), 120);
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override void RandomUpdate(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (tile.LiquidAmount <= 0)
		{
			WorldGen.KillTile(i, j);
			return;
		}
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile tile = Main.tile[i, j];
		Point tilePos = new Point(i, j);
		var tileLeft = Main.tile[tilePos + new Point(-1, 0)];
		var tileRight = Main.tile[tilePos + new Point(1, 0)];
		var tileUp = Main.tile[tilePos + new Point(0, -1)];
		var tileDown = Main.tile[tilePos + new Point(0, 1)];
		if (tile.WallType == 0 && (!tileLeft.HasTile || !Main.tileSolid[tileLeft.TileType]) && (!tileRight.HasTile || !Main.tileSolid[tileRight.TileType])
			 && (!tileUp.HasTile || !Main.tileSolid[tileUp.TileType]) && (!tileDown.HasTile || !Main.tileSolid[tileDown.TileType]))
		{
			WorldGen.KillTile(i, j);
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		for (int k = 0; k < 2; k++)
		{
			Vector2 pos = new Point(i, j).ToWorldCoordinates();
			Dust d = Dust.NewDustDirect(pos - new Vector2(20, 40) + new Vector2(4), 40, 50, type);
			d.noGravity = true;
		}
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileFluentDrawManager.AddFluentPoint(this, i, j);
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		DrawAlgae(pos, pos.ToWorldCoordinates() - screenPosition, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// Draw a piece of lotus
	/// </summary>
	private void DrawAlgae(Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[tilePos];
		if (!(tile.TileType == Type && tile.HasTile))
		{
			return;
		}
		ushort type = tile.TileType;
		bool styleWall = false;
		var tileLeft = Main.tile[tilePos + new Point(-1, 0)];
		var tileRight = Main.tile[tilePos + new Point(1, 0)];
		var tileUp = Main.tile[tilePos + new Point(0, -1)];
		var tileDown = Main.tile[tilePos + new Point(0, 1)];
		if (tile.WallType > 0 && (!tileLeft.HasTile || !Main.tileSolid[tileLeft.TileType]) && (!tileRight.HasTile || !Main.tileSolid[tileRight.TileType])
			 && (!tileUp.HasTile || !Main.tileSolid[tileUp.TileType]) && (!tileDown.HasTile || !Main.tileSolid[tileDown.TileType]))
		{
			styleWall = true;
		}
		int frameY = (tilePos.X + tilePos.Y) % 2;
		int frameX = tilePos.X % 4;
		var frame = new Rectangle(0, 0, 0, 0);
		var origin = new Vector2(0, 0);
		switch (frameX)
		{
			case 0:
				frame = new Rectangle(0, 16, 30, 32);
				origin = new Vector2(19, 32);
				if (styleWall)
				{
					frame = new Rectangle(0, 106, 36, 32);
					origin = new Vector2(16, 18);
				}
				break;
			case 1:
				frame = new Rectangle(34, 0, 40, 48);
				origin = new Vector2(23, 48);
				if (styleWall)
				{
					frame = new Rectangle(38, 106, 42, 38);
					origin = new Vector2(23, 19);
				}
				break;
			case 2:
				frame = new Rectangle(76, 18, 30, 30);
				origin = new Vector2(19, 30);
				if (styleWall)
				{
					frame = new Rectangle(84, 114, 30, 26);
					origin = new Vector2(16, 16);
				}
				break;
			case 3:
				frame = new Rectangle(108, 6, 30, 42);
				origin = new Vector2(13, 42);
				if (styleWall)
				{
					frame = new Rectangle(116, 102, 38, 38);
					origin = new Vector2(22, 20);
				}
				break;
		}
		if (frameY == 1 && !styleWall)
		{
			frame.Y += 50;
			origin.X = frame.Width - origin.X;
		}

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.JadeLakeRedAlgae_Path, type, 1, paint, tileDrawing);
		tex ??= ModAsset.JadeLakeRedAlgae.Value;
		var tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.X, tilePos.Y, tile, type, 0, 0, tileLight);

		float totalRot = YggdrasilWorldGeneration.TerrianSurfaceAngle(tilePos.X, tilePos.Y) - MathHelper.PiOver2;
		var drawPos = drawCenterPos;
		if (!styleWall)
		{
			drawPos += new Vector2(0, 10).RotatedBy(totalRot);
		}
		Vector2 deltaY = new Vector2(0, -frame.Height / 16f);
		Vector2 deltaX = new Vector2(frame.Width / 2f, 0);
		Vector2 algeaPos = Vector2.zeroVector;
		List<Vertex2D> algaes = new List<Vertex2D>();
		if (!styleWall)
		{
			for (int j = 0; j <= 16; j++)
			{
				Point leftPos = (tilePos.ToWorldCoordinates() - deltaX.RotatedBy(totalRot) + algeaPos).ToTileCoordinates();
				Point rightPos = (tilePos.ToWorldCoordinates() + deltaX.RotatedBy(totalRot) + algeaPos).ToTileCoordinates();

				float windCycle = 0;
				if (tileDrawing.InAPlaceWithWind(leftPos.X, leftPos.Y, 1, 1))
				{
					windCycle = tileDrawing.GetWindCycle(leftPos.X, leftPos.Y, tileDrawing._sunflowerWindCounter);
				}
				int totalPushTime = 140;
				float pushForcePerFrame = 0.96f;
				float highestWindGridPushComplexLeft = tileDrawing.GetHighestWindGridPushComplex(leftPos.X, leftPos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
				windCycle += highestWindGridPushComplexLeft;
				float hardness = MathF.Pow(j / 16f, 0.5f);
				Vector2 movementLeft = new Vector2(windCycle * 7.2f * hardness, 0);

				windCycle = 0;
				if (tileDrawing.InAPlaceWithWind(rightPos.X, rightPos.Y, 1, 1))
				{
					windCycle = tileDrawing.GetWindCycle(rightPos.X, rightPos.Y, tileDrawing._sunflowerWindCounter);
				}
				float highestWindGridPushComplexRight = tileDrawing.GetHighestWindGridPushComplex(rightPos.X, rightPos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
				windCycle += highestWindGridPushComplexRight;
				Vector2 movementRight = new Vector2(windCycle * 7.2f * hardness, 0);
				float midRot = (movementLeft.X + movementRight.X) * 0.015f + totalRot;
				algaes.Add(drawPos + deltaX.RotatedBy(midRot) + algeaPos + movementRight, tileLight, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height - frame.Height / 16f * j) / tex.Height, 0));
				algaes.Add(drawPos - deltaX.RotatedBy(midRot) + algeaPos + movementLeft, tileLight, new Vector3(frame.X / (float)tex.Width, (frame.Y + frame.Height - frame.Height / 16f * j) / tex.Height, 0));
				algeaPos += deltaY.RotatedBy(midRot);
			}
		}
		else
		{
			float wallRot = tilePos.X + tilePos.Y;
			Vector2[] vertexPos = new Vector2[] { -origin, new Vector2(frame.Width, 0) - origin, new Vector2(0, frame.Height) - origin, new Vector2(frame.Width, frame.Height) - origin };
			for (int a = 0; a < vertexPos.Length; a++)
			{
				vertexPos[a] = vertexPos[a].RotatedBy(wallRot);
				vertexPos[a] += drawPos + Main.screenPosition;

				float windCycle = 0;
				Point vertexAPos = vertexPos[a].ToTileCoordinates();
				if (tileDrawing.InAPlaceWithWind(vertexAPos.X, vertexAPos.Y, 1, 1))
				{
					windCycle = tileDrawing.GetWindCycle(vertexAPos.X, vertexAPos.Y, tileDrawing._sunflowerWindCounter);
				}
				int totalPushTime = 140;
				float pushForcePerFrame = 0.96f;
				float highestWindGridPushComplex0 = tileDrawing.GetHighestWindGridPushComplex(vertexAPos.X, vertexAPos.Y, 1, 1, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
				windCycle += highestWindGridPushComplex0;
				vertexPos[a] += new Vector2(windCycle * 2.5f, 0);
				vertexPos[a] -= Main.screenPosition;
			}
			tileLight *= 0.75f;
			algaes.Add(vertexPos[0], tileLight, new Vector3(frame.X / (float)tex.Width, frame.Y / (float)tex.Height, 0));
			algaes.Add(vertexPos[1], tileLight, new Vector3((frame.X + frame.Width) / (float)tex.Width, frame.Y / (float)tex.Height, 0));

			algaes.Add(vertexPos[2], tileLight, new Vector3(frame.X / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, 0));
			algaes.Add(vertexPos[3], tileLight, new Vector3((frame.X + frame.Width) / (float)tex.Width, (frame.Y + frame.Height) / (float)tex.Height, 0));
		}
		if (algaes.Count > 2)
		{
			spriteBatch.GraphicsDevice.Textures[0] = tex;
			spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, algaes.ToArray(), 0, algaes.Count - 2);
		}
	}
}
