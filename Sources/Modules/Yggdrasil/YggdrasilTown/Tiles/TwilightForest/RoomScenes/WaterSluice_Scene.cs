using System.Reflection;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using MonoMod.Cil;
using Terraria.GameContent.Liquid;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.RoomScenes;

public class WaterSluice_Scene : ModTile, ISceneTile
{
	public override string Texture => ModAsset.GreenRelicBrick_Mod;

	public static List<Rectangle> JungleWaterAreas = new List<Rectangle>();

	public override void Load()
	{
		On_WaterfallManager.DrawWaterfall_int_int_int_float_Vector2_Rectangle_Color_SpriteEffects += OnHook_ModifyWaterfallStyle;
		Ins.HookManager.AddHook(typeof(LiquidRenderer).GetMethod(nameof(LiquidRenderer.DrawNormalLiquids), BindingFlags.Public | BindingFlags.Instance), ILHook_ModifyWaterStyle);
		base.Load();
	}

	private static void OnHook_ModifyWaterfallStyle(On_WaterfallManager.orig_DrawWaterfall_int_int_int_float_Vector2_Rectangle_Color_SpriteEffects orig, WaterfallManager self, int waterfallType, int x, int y, float opacity, Vector2 Position, Rectangle sourceRect, Color color, SpriteEffects effects)
	{
		if (ModContent.GetInstance<YggdrasilBiomeTileCounter>().WaterSluiceRoomCount > 0)
		{
			foreach (var box in JungleWaterAreas)
			{
				if (box.Contains(x, y))
				{
					waterfallType = 4;
				}
			}
		}
		orig(self, waterfallType, x, y, opacity, Position, sourceRect, color, effects);
	}

	private void ILHook_ModifyWaterStyle(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		if (cursor.TryGotoNext(
			MoveType.After,
			x => x.MatchLdarg2(),
			x => x.MatchLdloc3(),
			x => x.MatchLdloc(out _)))
		{
			cursor.Index++;
			cursor.EmitLdloc(3);
			cursor.EmitLdloc(4);
			cursor.EmitLdloc(8);
			cursor.EmitDelegate(CalculateWaterStyle);
			cursor.EmitStloc(8);
		}
	}

	private int CalculateWaterStyle(int i, int j, int originStyle)
	{
		if (ModContent.GetInstance<YggdrasilBiomeTileCounter>().WaterSluiceRoomCount > 0)
		{
			foreach (var box in JungleWaterAreas)
			{
				if (box.Contains(i, j))
				{
					return WaterStyleID.Jungle;
				}
			}
		}
		return originStyle;
	}

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
		scene_Close.CustomDraw += WaterSluiceOverTile;
		var liquid = new WaterSluice_Liquid_Scene { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };

		Ins.VFXManager.Add(scene_Close);
		Ins.VFXManager.Add(liquid);
	}

	public void WaterSluiceOverTile(TwilightCastle_RoomScene_OverTiles otD)
	{
		bool flipH = otD.FlipHorizontally(otD.OriginTilePos.X, otD.OriginTilePos.Y);
		Texture2D tex0 = ModAsset.WaterSluice_Scene_Close.Value;

		List<Vertex2D> bars = new List<Vertex2D>();
		SceneUtils.DrawMultiSceneTowardBottom(otD.OriginTilePos.X, otD.OriginTilePos.Y, tex0, bars, flipH);
		Ins.Batch.Draw(tex0, bars, PrimitiveType.TriangleList);
	}

	public float GetOffsetHorizontal(float x, float y, float middleX)
	{
		return -MathF.Sqrt(y) * (middleX - x);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (JungleWaterAreas is null)
		{
			JungleWaterAreas = new List<Rectangle>();
		}
		Rectangle myArea = new Rectangle(i, j, 40, 21);
		if (FlipHorizontally(i, j))
		{
			myArea = new Rectangle(i - 39, j, 40, 21);
		}

		if (!JungleWaterAreas.Contains(myArea))
		{
			JungleWaterAreas.Add(myArea);
		}

		for (int x = myArea.X; x < myArea.X + myArea.Width; x++)
		{
			for (int y = myArea.Y; y < myArea.Y + myArea.Height; y++)
			{
				var tile = TileUtils.SafeGetTile(x, y);
				if (tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 0)
				{
					Lighting.AddLight(x, y, 0.1f, 0.5f, 1f);
				}
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
