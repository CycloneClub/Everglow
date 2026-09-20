using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.SubSpace;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class MarbleGate_BackgroundTile : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void Update()
	{
		base.Update();
		if (Main.MouseWorld.X > Position.X && Main.MouseWorld.X < Position.X + 112)
		{
			if (Main.MouseWorld.Y > Position.Y && Main.MouseWorld.Y < Position.Y + 128)
			{
				Main.instance.MouseText("Rightclick to Enter Union.");
				if (Main.mouseRight && Main.mouseRightRelease && !Main.mapFullscreen)
				{
					int i = Main.MouseWorld.ToTileCoordinates().X;
					int j = Main.MouseWorld.ToTileCoordinates().Y;
					for (int x = -8; x < 9; x++)
					{
						for (int y = -8; y < 9; y++)
						{
							Tile tile = TileUtils.SafeGetTile(i + x, j + y);
							if (tile.TileType == ModContent.TileType<MarbleGate>())
							{
								i += x;
								j += y;
								x = 100;
								break;
							}
						}
					}
					Point point = new Point(i, j);
					RoomManager.EnterNextLevelRoom(point + new Point(3, 6), new Point(60, 192), BuildUnionGen);
				}
			}
		}
	}

	public void BuildUnionGen()
	{
		var mapIO = new MapIO(30, 110);

		mapIO.Read(ModIns.Mod.GetFileStream(ModAsset.HallOfUnion237x110_Path));

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
		for (int x = 20; x < 22; x++)
		{
			for (int y = 20; y < 23; y++)
			{
				Tile tile = TileUtils.SafeGetTile(x, y);
				tile.WallType = 1;
				ushort typeChange = (ushort)ModContent.TileType<UnionCommandBlock>();
				if (y == 22)
				{
					typeChange = 0;
				}
				else
				{
					tile.TileFrameX = (short)((x - 20) * 18);
					tile.TileFrameY = (short)((y - 20) * 18);
				}
				tile.TileType = typeChange;
				tile.HasTile = true;
			}
		}
	}

	public override void OnSpawn()
	{
		Texture = ModAsset.MarbleGate.Value;
	}

	public override void Draw()
	{
		Color lightColor0 = Lighting.GetColor((int)Position.X / 16, (int)Position.Y / 16);
		Color lightColor1 = Lighting.GetColor((int)(Position.X + Texture.Width) / 16, (int)Position.Y / 16);
		Color lightColor2 = Lighting.GetColor((int)Position.X / 16, (int)(Position.Y + Texture.Height) / 16);
		Color lightColor3 = Lighting.GetColor((int)(Position.X + Texture.Width) / 16, (int)(Position.Y + Texture.Height) / 16);

		Vector2 subBackgroundScale = Texture.Size() / ModAsset.MarbleGate_Background.Value.Size();

		// Vector2 offsetMouse = new Vector2((Main.MouseScreen.X / 150) % 1f, (Main.MouseScreen.Y / 150) % 1f);
		Vector2 viewOffset = (Position - Main.LocalPlayer.position) * 0.0004f + new Vector2(10000) + new Vector2(0.44f);

		// Main.NewText(offsetMouse);
		viewOffset.X %= 1;
		viewOffset.Y %= 1;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position, Color.White, new Vector3(viewOffset, 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, 0), Color.White, new Vector3(viewOffset + new Vector2(subBackgroundScale.X, 0), 0)),

			new Vertex2D(Position + new Vector2(0, Texture.Height), Color.White, new Vector3(viewOffset + new Vector2(0, subBackgroundScale.Y), 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, Texture.Height), Color.White, new Vector3(viewOffset + subBackgroundScale, 0)),
		};
		Ins.Batch.Draw(ModAsset.MarbleGate_Background.Value, bars, PrimitiveType.TriangleStrip);
		bars = new List<Vertex2D>()
		{
			new Vertex2D(Position, lightColor0, new Vector3(0, 0, 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, 0), lightColor1, new Vector3(1, 0, 0)),

			new Vertex2D(Position + new Vector2(0, Texture.Height), lightColor2, new Vector3(0, 1, 0)),
			new Vertex2D(Position + new Vector2(Texture.Width, Texture.Height), lightColor3, new Vector3(1, 1, 0)),
		};
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
