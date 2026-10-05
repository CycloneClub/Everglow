using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class BloodChurch_Scene : ModTile, ISceneTile
{
	public override string Texture => ModAsset.GreenRelicBrick_Mod;

	public bool FlipHorizontally(int i, int j)
	{
		int leftSolid = 0;
		int rightSolid = 0;
		for (int x = 1; x < 15; x++)
		{
			Tile tileLeft = TileUtils.SafeGetTile(i - x, j);
			Tile tileRight = TileUtils.SafeGetTile(i + x, j);
			if (tileLeft.HasTile && tileLeft.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				leftSolid++;
			}
			if (tileRight.HasTile && tileRight.TileType == ModContent.TileType<GreenRelicBrick>())
			{
				rightSolid++;
			}
		}
		if (rightSolid > leftSolid)
		{
			return false;
		}
		return true;
	}

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkForestSoil>()] = true;
		Main.tileMerge[Type][ModContent.TileType<StoneScaleWood>()] = true;

		Main.tileMerge[Type][ModContent.TileType<GreenRelicBrick>()] = true;
		Main.tileMerge[ModContent.TileType<GreenRelicBrick>()][Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_dust>();
		HitSound = SoundID.Dig;
		MinPick = int.MaxValue;
		AddMapEntry(new Color(35, 58, 58));
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public void AddScene(int i, int j)
	{
		var scene_Close = new TwilightCastle_RoomScene_OverTiles { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Close.CustomDraw += DrawBloodyChurchOverTile;

		var scene_Background = new TwilightCastle_RoomScene_Background { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		scene_Background.CustomDraw += DrawBloodyChurchBackground;

		var liquid_OverPlayer = new BloodChurch_Liquid_Scene { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, LiquidAreas = new List<Rectangle>() };

		Ins.VFXManager.Add(scene_Close);
		Ins.VFXManager.Add(scene_Background);
		Ins.VFXManager.Add(liquid_OverPlayer);

		// Ins.VFXManager.Add(scene_Fountain);
	}

	private readonly List<Vertex2D> fountainBars = new List<Vertex2D>();
	private readonly List<Vertex2D> fountainBarsBlack = new List<Vertex2D>();
	private readonly List<Vertex2D> fountainBarsReflect = new List<Vertex2D>();

	public void DrawBloodyChurchOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		Texture2D tex0 = ModAsset.BloodChurch_Scene_Close.Value;

		StaticSceneMesh mesh = otD.GetOrBuildMesh(tex0, otD.OriginTilePos.X, otD.OriginTilePos.Y);
		if (mesh.ColorRefreshTimer++ % 10 == 0)
		{
			mesh.RefreshColors();
		}
		Ins.Batch.Draw(tex0, mesh.Vertices, PrimitiveType.TriangleList);
	}

	public void DrawBloodyChurchBackground(TwilightCastle_RoomScene_Background bg)
	{
		bool flipH = bg.FlipH();
		Texture2D tex0 = ModAsset.BloodChurch_Scene_Background.Value;

		// Texture2D tex1 = ModAsset.BloodChurch_Scene_Far.Value;
		Texture2D tex2 = ModAsset.BloodChurch_Scene_Fountain.Value;
		Texture2D tex2_red = ModAsset.BloodChurch_Scene_Fountain_Red.Value;
		Texture2D tex3_dark = Commons.ModAsset.Noise_flame_2_pure_black.Value;
		Texture2D tex3 = Commons.ModAsset.Noise_flame_2_pure.Value;
		Texture2D tex4 = Commons.ModAsset.Noise_WaterFallWave.Value;

		StaticSceneMesh meshBg = bg.GetOrBuildMesh(tex0, bg.OriginTilePos.X, bg.OriginTilePos.Y);
		if (meshBg.ColorRefreshTimer++ % 10 == 0)
		{
			meshBg.RefreshColors();
		}
		Ins.Batch.Draw(tex0, meshBg.Vertices, PrimitiveType.TriangleList);

		// Fountain waterfall background
		int direction = 1;
		if (flipH)
		{
			direction = -1;
		}
		float coordX = 0;
		Color oldColor = new Color(1f, 0f, 0.2f, 0);
		float timeValue = -(float)Main.time / 240f;
		for (int i = 0; i < 11; i++)
		{
			AppendFountainStrip(bg, flipH, direction, 17, 11, new Vector2(i * 8 * direction, 0), i, 10, 16, 6f, 0.35f, false, timeValue, oldColor, ref coordX);
		}
		coordX = 0;
		for (int i = 0; i < 3; i++)
		{
			AppendFountainStrip(bg, flipH, direction, 19, 9, new Vector2(i * 8 * direction, 8), i, 3, 16, 1.5f, 0.35f, false, timeValue, oldColor, ref coordX);
		}
		FlushFountainGroup(tex3_dark, tex3, null);

		// Fountain. The red layer shares the cached mesh of tex2 (same geometry as the legacy code); its color factor changes every frame, so refresh colors directly instead of using the timer.
		StaticSceneMesh meshFountain = bg.GetOrBuildMesh(tex2, bg.OriginTilePos.X + 17 * direction, bg.OriginTilePos.Y + 10);
		meshFountain.RefreshColors();
		Ins.Batch.Draw(tex2, meshFountain.Vertices, PrimitiveType.TriangleList);

		Color envLight = Lighting.GetColor(bg.OriginTilePos.X + 22, bg.OriginTilePos.Y + 14);
		meshFountain.RefreshColors((envLight.R - Math.Max(envLight.G, envLight.B)) / 255f);
		Ins.Batch.Draw(tex2_red, meshFountain.Vertices, PrimitiveType.TriangleList);

		// Fountain waterfall front
		coordX = 0;
		timeValue += 0.5f;
		for (int i = 0; i < 11; i++)
		{
			AppendFountainStrip(bg, flipH, direction, 17, 11, new Vector2(i * 8 * direction, 0), i, 10, 16, 6f, 0.25f, true, timeValue, oldColor, ref coordX);
		}
		coordX = 0;
		for (int i = 0; i < 3; i++)
		{
			AppendFountainStrip(bg, flipH, direction, 19, 9, new Vector2(i * 8 * direction, 8), i, 4, 8, 1.5f, 0.35f, true, timeValue, oldColor, ref coordX);
		}
		FlushFountainGroup(tex3_dark, tex3, tex4);
	}

	/// <summary>
	/// Append one vertical waterfall strip into the reusable fountain buffers; strips are merged per texture and submitted by <see cref="FlushFountainGroup" />.
	/// </summary>
	private void AppendFountainStrip(TwilightCastle_RoomScene_Background bg, bool flipH, int direction, int originTileX, int originTileY, Vector2 baseOffset, int stripIndex, int segmentCount, float yStep, float middleX, float blackAlpha, bool withReflect, float timeValue, Color oldColor, ref float coordX)
	{
		float addCoordX = MathF.Acos((40f - stripIndex * 8) / 41f) / MathHelper.Pi - coordX;
		Vector2 drawPos = new Vector2(bg.OriginTilePos.X + originTileX * direction, bg.OriginTilePos.Y + originTileY).ToWorldCoordinates() + baseOffset;
		for (int j = 0; j < segmentCount; j++)
		{
			float addYCoord = MathF.Pow(j, 0.5f);
			float x0 = -4 + GetOffsetHorizontal(stripIndex, j, middleX) * direction;
			float x1 = 4 + GetOffsetHorizontal(stripIndex + 1 * direction, j, middleX) * direction;

			Vector2 drawPieceLeft = drawPos + new Vector2(x0, j * yStep);
			Vector2 drawPieceRight = drawPos + new Vector2(x1, j * yStep);

			Color leftColor = Lighting.GetColor(drawPieceLeft.ToTileCoordinates(), oldColor);
			leftColor.A = 0;
			Color rightColor = Lighting.GetColor(drawPieceRight.ToTileCoordinates(), oldColor);
			rightColor.A = 0;

			float coordX0 = coordX;
			float coordX1 = coordX + addCoordX;
			if (flipH)
			{
				(coordX0, coordX1) = (coordX1, coordX0);
			}

			Vector3 texCoordLeft = new Vector3(coordX0, timeValue + addYCoord * 0.08f, 0);
			Vector3 texCoordRight = new Vector3(coordX1, timeValue + addYCoord * 0.08f, 0);
			Vertex2D left = new Vertex2D(drawPieceLeft, leftColor * 0.15f, texCoordLeft);
			Vertex2D right = new Vertex2D(drawPieceRight, rightColor * 0.15f, texCoordRight);
			Vertex2D leftBlack = new Vertex2D(drawPieceLeft, Color.White * blackAlpha, texCoordLeft);
			Vertex2D rightBlack = new Vertex2D(drawPieceRight, Color.White * blackAlpha, texCoordRight);

			Vertex2D leftReflect = default;
			Vertex2D rightReflect = default;
			if (withReflect)
			{
				Color reflectLeft = Color.Lerp(leftColor, new Color(1f, 1f, 1f, 0), 0.2f);
				Color reflectRight = Color.Lerp(rightColor, new Color(1f, 1f, 1f, 0), 0.2f);
				leftReflect = new Vertex2D(drawPieceLeft, reflectLeft * 0.35f, texCoordLeft);
				rightReflect = new Vertex2D(drawPieceRight, reflectRight * 0.35f, texCoordRight);
			}

			if (j == 0)
			{
				// Degenerate connector vertices keep merged strips visually separate.
				ConnectStrip(fountainBars, left);
				ConnectStrip(fountainBarsBlack, leftBlack);
				if (withReflect)
				{
					ConnectStrip(fountainBarsReflect, leftReflect);
				}
			}

			fountainBars.Add(left);
			fountainBars.Add(right);
			fountainBarsBlack.Add(leftBlack);
			fountainBarsBlack.Add(rightBlack);
			if (withReflect)
			{
				fountainBarsReflect.Add(leftReflect);
				fountainBarsReflect.Add(rightReflect);
			}
		}
		coordX += addCoordX;
	}

	private static void ConnectStrip(List<Vertex2D> bars, Vertex2D firstVertex)
	{
		if (bars.Count > 0)
		{
			bars.Add(bars[^1]);
			bars.Add(firstVertex);
		}
	}

	private void FlushFountainGroup(Texture2D tex3_dark, Texture2D tex3, Texture2D tex4)
	{
		Ins.Batch.Draw(tex3_dark, fountainBarsBlack, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(tex3, fountainBars, PrimitiveType.TriangleStrip);
		if (tex4 != null)
		{
			Ins.Batch.Draw(tex4, fountainBarsReflect, PrimitiveType.TriangleStrip);
		}
		fountainBars.Clear();
		fountainBarsBlack.Clear();
		fountainBarsReflect.Clear();
	}

	public float GetOffsetHorizontal(float x, float y, float middleX)
	{
		return -MathF.Sqrt(y) * (middleX - x);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		bool flipH = FlipHorizontally(i, j);
		int direction = 1;
		if (flipH)
		{
			direction = -1;
		}
		if (!Main.gamePaused)
		{
			for (int k = 0; k < 4; k++)
			{
				float addX = Main.rand.NextFloat(96);
				Vector2 pos = new Vector2(i + 17 * direction, j + 11).ToWorldCoordinates() + new Vector2((addX - 8) * direction, Main.rand.NextFloat(-2, 14));
				var dust = new BloodChurch_Scene_FakeLiquid_Dust()
				{
					Position = pos,
					Velocity = new Vector2((addX - 48) * direction * 0.01f, 1),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = (j + 17) * 16,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f),
					Active = true,
					Visible = true,
				};
				Ins.VFXManager.Add(dust);
			}
			for (int k = 0; k < 1; k++)
			{
				float addX = Main.rand.NextFloat(16);
				Vector2 pos = new Vector2(i + 19 * direction, j + 9).ToWorldCoordinates() + new Vector2(addX * direction, Main.rand.NextFloat(6, 10));
				var dust = new BloodChurch_Scene_FakeLiquid_Dust()
				{
					Position = pos,
					Velocity = new Vector2((addX - 8) * direction * 0.01f, Main.rand.NextFloat(-0.5f, 0)),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = (j + 12) * 16,
					Frame = new Rectangle(Main.rand.Next(4) * 10, 10, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.6f),
					Active = true,
					Visible = true,
				};
				Ins.VFXManager.Add(dust);
			}

			for (int k = 0; k < 4; k++)
			{
				float addX = Main.rand.NextFloat(136);
				Vector2 pos = new Vector2(i + 16 * direction, j + 18).ToWorldCoordinates() + new Vector2((addX - 8) * direction, Main.rand.NextFloat(-8, 0));
				float apartCenter = MathF.Abs(68 - addX);
				if (apartCenter < 40)
				{
					pos.Y -= 8;
				}
				if (apartCenter < 20)
				{
					pos.Y -= 8;
				}
				int frameY = 10;
				if (Main.rand.NextBool(3))
				{
					frameY = 20;
				}
				var dust = new BloodChurch_Scene_FakeLiquid_Dust()
				{
					Position = pos,
					Velocity = new Vector2((addX - 68) * direction * 0.01f, Main.rand.NextFloat(-2f, 0f)),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Timer = 0,
					MaxTime = 150,
					MaxPosY = (j + 18) * 16 + 8,
					Frame = new Rectangle(Main.rand.Next(4) * 10, frameY, 10, 10),
					Scale = Main.rand.NextFloat(0.35f, 0.4f),
					Active = true,
					Visible = true,
				};
				Ins.VFXManager.Add(dust);
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public float GetRandomWave(float x)
	{
		float value = 0;
		float timeValue = (float)Main.time * 0.06f;
		for (int i = 0; i < 8; i++)
		{
			float rate = MathF.Pow(2, i);
			value += MathF.Sin(x * rate + timeValue * rate * 0.5f) / rate;
		}
		return value;
	}
}
