using Everglow.Commons.TileHelper;
using Everglow.Commons.Vertex;
using Terraria.GameContent;

namespace Everglow.Commons.DeveloperContent.Items;

internal class SightOfTileProj : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
		Projectile.usesOwnerLight = true;
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}

	private int left = 0;
	private int up = 0;
	private int down = 0;
	private int right = 0;

	private void UpdateFourCoord()
	{
		int X1 = (int)(startPoint.X / 16f);
		int X2 = (int)(Main.MouseWorld.X / 16f);
		int Y1 = (int)(startPoint.Y / 16f);
		int Y2 = (int)(Main.MouseWorld.Y / 16f);
		if (X1 > X2)
		{
			(X1, X2) = (X2, X1);
		}
		if (Y1 > Y2)
		{
			(Y1, Y2) = (Y2, Y1);
		}
		left = X1;
		right = X2;
		up = Y1;
		down = Y2;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.itemTime = 5;
		player.itemAnimation = 5;
		Projectile.position = player.MountedCenter - new Vector2(17);
		player.heldProj = Projectile.whoAmI;
		if (Projectile.timeLeft > 6)
		{
			startPoint = Main.MouseWorld;
		}

		UpdateFourCoord();
		if (Main.mouseLeft)
		{
			Projectile.timeLeft = 5;
		}

		if (Main.mouseRight)
		{
			Projectile.Kill();
		}
	}

	private Vector2 startPoint = Vector2.Zero;

	public void DrawDoubleLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2)
	{
		float Wid = 1f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(0, 0, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{

		Vector2 Vdr = (Main.MouseWorld + startPoint) * 0.5f - Projectile.Center;

		Vdr = Vdr / Vdr.Length() * 7;

		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(Vdr.Y, Vdr.X) - Math.PI / 2d));
		Texture2D t = ModAsset.SightOfTileProj.Value;
		Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects S = SpriteEffects.None;
		if (Math.Sign(Vdr.X) == -1)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		Projectile.rotation = (float)(Math.Atan2(Vdr.Y, Vdr.X) + Math.PI / 4d);
		Main.spriteBatch.Draw(t, player.MountedCenter - Main.screenPosition + Vdr * 5f, null, color, Projectile.rotation, t.Size() / 2f, Projectile.scale, S, 0f);

		Vector2 ULInt = new Vector2(left, up) * 16 + new Vector2(0);
		Vector2 DRInt = new Vector2(right, down) * 16 + new Vector2(0);
		Vector2 DLInt = new Vector2(left, down) * 16 + new Vector2(0);
		Vector2 URInt = new Vector2(right, up) * 16 + new Vector2(0);

		URInt.X += 16;
		DLInt.Y += 16;
		DRInt.X += 16;
		DRInt.Y += 16;
		ULInt += new Vector2(1, 1);
		URInt += new Vector2(-1, 1);
		DLInt += new Vector2(1, -1);
		DRInt += new Vector2(-1, -1);
		DrawDoubleLine(player.MountedCenter - Main.screenPosition + Vdr * 8f, ULInt - Main.screenPosition, new Color(40, 240, 255, 100), new Color(0, 0, 65, 30));
		DrawDoubleLine(player.MountedCenter - Main.screenPosition + Vdr * 8f, URInt - Main.screenPosition, new Color(40, 240, 255, 100), new Color(0, 0, 65, 30));
		DrawDoubleLine(player.MountedCenter - Main.screenPosition + Vdr * 8f, DLInt - Main.screenPosition, new Color(40, 240, 255, 100), new Color(0, 0, 65, 30));
		DrawDoubleLine(player.MountedCenter - Main.screenPosition + Vdr * 8f, DRInt - Main.screenPosition, new Color(40, 240, 255, 100), new Color(0, 0, 65, 30));

		DrawNinePiecesForTiles(left, right, up, down);

		Main.instance.MouseText((right - left + 1).ToString() + "x" + (down - up + 1).ToString() + "\nRight Click to Cancel", 9);
		return false;
	}

	private void DrawNinePiecesForTiles(int LeftX, int RightX, int UpY, int DownY)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Texture2D t = ModAsset.TileBlock3x3.Value;
		var baseColor = new Color(0, 30, 120, 180);
		if (LeftX == RightX)
		{
			if (UpY == DownY)
			{
				int ScPosX = (int)Main.screenPosition.X;
				int ScPosY = (int)Main.screenPosition.Y;
				var source = new Rectangle(0, 0, 8, 8);
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX, UpY * 16 - ScPosY, 8, 8), source, GetTileColor(LeftX, UpY, baseColor));
				source = new Rectangle(40, 0, 8, 8);
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX + 8, UpY * 16 - ScPosY, 8, 8), source, GetTileColor(LeftX, UpY, baseColor));
				source = new Rectangle(0, 40, 8, 8);
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX, UpY * 16 - ScPosY + 8, 8, 8), source, GetTileColor(LeftX, UpY, baseColor));
				source = new Rectangle(40, 40, 8, 8);
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX + 8, UpY * 16 - ScPosY + 8, 8, 8), source, GetTileColor(LeftX, UpY, baseColor));
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				return;
			}
			for (int y = UpY; y < DownY + 1; y++)
			{
				int ScPosX = (int)Main.screenPosition.X;
				int ScPosY = (int)Main.screenPosition.Y;
				var source = new Rectangle(0, 16, 8, 16);
				var source2 = new Rectangle(40, 16, 8, 16);
				if (y == UpY)
				{
					source.Y = 0;
					source2.Y = 0;
				}
				if (y == DownY)
				{
					source.Y = 32;
					source2.Y = 32;
				}
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX, y * 16 - ScPosY, 8, 16), source, GetTileColor(LeftX, y, baseColor));
				Main.spriteBatch.Draw(t, new Rectangle(LeftX * 16 - ScPosX + 8, y * 16 - ScPosY, 8, 16), source2, GetTileColor(LeftX, y, baseColor));
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			return;
		}
		if (UpY == DownY)
		{
			for (int x = LeftX; x < RightX + 1; x++)
			{
				int ScPosX = (int)Main.screenPosition.X;
				int ScPosY = (int)Main.screenPosition.Y;
				var source = new Rectangle(16, 0, 16, 8);
				var source2 = new Rectangle(16, 40, 16, 8);
				if (x == LeftX)
				{
					source.X = 0;
					source2.X = 0;
				}
				if (x == RightX)
				{
					source.X = 32;
					source2.X = 32;
				}
				Main.spriteBatch.Draw(t, new Rectangle(x * 16 - ScPosX, UpY * 16 - ScPosY, 16, 8), source, GetTileColor(x, UpY, baseColor));
				Main.spriteBatch.Draw(t, new Rectangle(x * 16 - ScPosX, UpY * 16 - ScPosY + 8, 16, 8), source2, GetTileColor(x, UpY, baseColor));
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			return;
		}

		for (int x = LeftX; x < RightX + 1; x++)
		{
			for (int y = UpY; y < DownY + 1; y++)
			{
				int ScPosX = (int)Main.screenPosition.X;
				int ScPosY = (int)Main.screenPosition.Y;
				var source = new Rectangle(16, 16, 16, 16);
				if (x == LeftX)
				{
					source.X = 0;
				}

				if (x == RightX)
				{
					source.X = 32;
				}

				if (y == UpY)
				{
					source.Y = 0;
				}

				if (y == DownY)
				{
					source.Y = 32;
				}
				Tile tile = Main.tile[x, y];
				if (!IsHalfBrick(x, y))
				{
					Main.spriteBatch.Draw(t, new Rectangle(x * 16 - ScPosX, y * 16 - ScPosY, 16, 16), source, GetTileColor(x, y, baseColor));
				}
				else
				{
					Main.spriteBatch.Draw(ModAsset.HalfTiles.Value, new Rectangle(x * 16 - ScPosX, y * 16 - ScPosY, 16, 16), GetTileFrame(x, y), GetTileColorCheckHalfBrick(x, y, baseColor));
					Main.spriteBatch.Draw(ModAsset.HalfTiles.Value, new Rectangle(x * 16 - ScPosX, y * 16 - ScPosY, 16, 16), GetTileFrame(x, y, true), GetTileColorCheckHalfBrick(x, y, baseColor, true));
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
	}

	private bool IsHalfBrick(int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (tile != null && tile.HasTile)
		{
			if (Main.tileSolid[tile.type])
			{
				if (tile.IsHalfBlock)
				{
					return true;
				}
				if (tile.Slope == SlopeType.SlopeDownLeft)
				{
					return true;
				}
				if (tile.Slope == SlopeType.SlopeDownRight)
				{
					return true;
				}
				if (tile.Slope == SlopeType.SlopeUpLeft)
				{
					return true;
				}
				if (tile.Slope == SlopeType.SlopeUpRight)
				{
					return true;
				}
			}
		}
		return false;
	}

	private Rectangle GetTileFrame(int i, int j, bool emptyPart = false)
	{
		var frame = new Rectangle(0, 0, 16, 16);
		Tile tile = Main.tile[i, j];
		if (tile != null && tile.HasTile)
		{
			if (tile.IsHalfBlock)
			{
				frame.X = 18;
			}
			if (tile.Slope == SlopeType.SlopeDownLeft)
			{
				frame.X = 36;
			}
			if (tile.Slope == SlopeType.SlopeDownRight)
			{
				frame.X = 54;
			}
			if (tile.Slope == SlopeType.SlopeUpLeft)
			{
				frame.X = 72;
			}
			if (tile.Slope == SlopeType.SlopeUpRight)
			{
				frame.X = 90;
			}
			if (emptyPart)
			{
				frame.Y = 18;
				if (tile.IsHalfBlock)
				{
					frame.X = 108;
				}
				if (tile.Slope == SlopeType.SlopeDownLeft)
				{
					frame.X = 90;
				}
				if (tile.Slope == SlopeType.SlopeDownRight)
				{
					frame.X = 72;
				}
				if (tile.Slope == SlopeType.SlopeUpLeft)
				{
					frame.X = 54;
				}
				if (tile.Slope == SlopeType.SlopeUpRight)
				{
					frame.X = 36;
				}
			}
			return frame;
		}
		return new Rectangle(126, 0, 0, 0);
	}

	private Color GetTileColorCheckHalfBrick(int i, int j, Color baseColor, bool emptyPart = false)
	{
		Tile tile = Main.tile[i, j];
		if (tile.HasTile && !emptyPart)
		{
			if (tile.WallType > WallID.None)
			{
				return new Color(255, 120, 0, 200);
			}
			if (!Main.tileSolid[tile.type])
			{
				return new Color(0, 20, 120, 10);
			}
			return new Color(200, 200, 0, 10);
		}
		if (tile.WallType > WallID.None)
		{
			return new Color(95, 0, 0, 200);
		}

		return baseColor;
	}

	private Color GetTileColor(int i, int j, Color baseColor)
	{
		Tile tile = Main.tile[i, j];
		if (tile.HasTile)
		{
			if (tile.WallType > WallID.None)
			{
				return new Color(255, 120, 0, 200);
			}
			if (!Main.tileSolid[tile.type])
			{
				return new Color(0, 20, 120, 10);
			}
			return new Color(200, 200, 0, 10);
		}
		if (tile.WallType > WallID.None)
		{
			return new Color(95, 0, 0, 200);
		}

		return baseColor;
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft > 0)
		{
			return;
		}

		var mapIO = new MapIO(left, up, right - left + 1, down - up + 1);
		int Count = 0;
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", Mod.Name);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		while (File.Exists(path + "\\MapTiles" + Count.ToString() + ".mapio"))
		{
			Count++;
		}
		string writePath = path + "\\MapTiles" + Count.ToString() + ".mapio";
		mapIO.Write(writePath);
		CombatText.NewText(new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 0, 0), Color.Cyan, "Success saving at: " + writePath);
	}
}
