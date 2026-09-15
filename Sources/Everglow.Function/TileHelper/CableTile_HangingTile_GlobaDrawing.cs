using System.Reflection;
using Everglow.Commons.Utilities;
using Everglow.Commons.VFX;
using MonoMod.Cil;
using Terraria.GameContent.Drawing;

namespace Everglow.Commons.TileHelper;

public class CableTile_HangingTile_GlobaDrawing : GlobalTile
{
	public override void Load()
	{
		Ins.HookManager.AddHook(typeof(TileDrawing).GetMethod(nameof(TileDrawing.Draw), BindingFlags.Public | BindingFlags.Instance), ILHook_TileDrawing_DrawCableTile_HangingTile);
	}

	private void ILHook_TileDrawing_DrawCableTile_HangingTile(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		if (cursor.TryGotoNext(
			MoveType.After,
			x => x.MatchStfld(typeof(TileDrawing).GetField("_martianGlow", BindingFlags.NonPublic | BindingFlags.Instance)),
			x => x.MatchLdarg(0), // 匹配 ldarg.0
			x => x.MatchLdfld(typeof(TileDrawing).GetField("_currentTileDrawInfo", BindingFlags.NonPublic | BindingFlags.Instance)))) // 匹配 ldfld
		{
			cursor.Index += 2;
			cursor.EmitDelegate(ILHook_SearchCableTile_HangingTilesAndCallPreDraw);
		}
	}

	private void ILHook_SearchCableTile_HangingTilesAndCallPreDraw()
	{
		Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
		Vector2 vector = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			vector = Vector2.Zero;
		}
		GetScreenDrawArea(unscaledPosition, vector + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY);
		foreach (var hangingTile in TileLoader.tiles.OfType<HangingTile>())
		{
			foreach (var tilePos in hangingTile.RopesOfAllThisTileInTheWorld.Keys)
			{
				var worldPos = tilePos.ToWorldCoordinates();
				if (VFXManager.InScreen(worldPos, hangingTile.DrawOffScreenRange) && (tilePos.X < firstTileX - 2 || tilePos.X >= lastTileX + 2 || tilePos.Y < firstTileY || tilePos.Y >= lastTileY + 4))
				{
					int type = Main.tile[tilePos].type;
					TileLoader.PreDraw(tilePos.X, tilePos.Y, type, Main.spriteBatch);
				}
			}
		}
		foreach (var cableTile in TileLoader.tiles.OfType<CableTile>())
		{
			foreach (var tilePos in cableTile.RopesOfAllThisTileInTheWorld.Keys)
			{
				var worldPos = tilePos.ToWorldCoordinates();
				if (VFXManager.InScreen(worldPos, cableTile.DrawOffScreenRange) && (tilePos.X < firstTileX - 2 || tilePos.X >= lastTileX + 2 || tilePos.Y < firstTileY || tilePos.Y >= lastTileY + 4))
				{
					int type = Main.tile[tilePos].type;
					TileLoader.PreDraw(tilePos.X, tilePos.Y, type, Main.spriteBatch);
				}
			}
		}

		foreach (var offestScreenTile in TileLoader.tiles.OfType<ITileOffsetOverScreenDrawn>())
		{
			for (int x = ITileOffsetOverScreenDrawn.SpecialTilePositon.Count - 1; x >= 0; x--)
			{
				var tilePos = ITileOffsetOverScreenDrawn.SpecialTilePositon[x];
				var worldPos = tilePos.ToWorldCoordinates();
				if (VFXManager.InScreen(worldPos, offestScreenTile.TileOffsetScreenRange()) && (tilePos.X < firstTileX - 2 || tilePos.X >= lastTileX + 2 || tilePos.Y < firstTileY || tilePos.Y >= lastTileY + 4))
				{
					var tile = Main.tile[tilePos];
					if (tile.HasTile && TileLoader.GetTile(tile.type) is ITileOffsetOverScreenDrawn)
					{
						int type = tile.type;
						TileLoader.PreDraw(tilePos.X, tilePos.Y, type, Main.spriteBatch);
					}
					else
					{
						ITileOffsetOverScreenDrawn.SpecialTilePositon.Remove(tilePos);
					}
				}
			}
		}
	}

	private void GetScreenDrawArea(Vector2 screenPosition, Vector2 offSet, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY)
	{
		firstTileX = (int)((screenPosition.X - offSet.X) / 16f - 1f);
		lastTileX = (int)((screenPosition.X + Main.screenWidth + offSet.X) / 16f) + 2;
		firstTileY = (int)((screenPosition.Y - offSet.Y) / 16f - 1f);
		lastTileY = (int)((screenPosition.Y + Main.screenHeight + offSet.Y) / 16f) + 5;
		if (firstTileX < 4)
		{
			firstTileX = 4;
		}

		if (lastTileX > Main.maxTilesX - 4)
		{
			lastTileX = Main.maxTilesX - 4;
		}

		if (firstTileY < 4)
		{
			firstTileY = 4;
		}

		if (lastTileY > Main.maxTilesY - 4)
		{
			lastTileY = Main.maxTilesY - 4;
		}
	}

	public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		Point checkPos = (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f).ToTileCoordinates();
		if (checkPos.X == i && checkPos.Y == j)
		{
			foreach (var hangingTile in TileLoader.tiles.OfType<HangingTile>())
			{
				foreach (var tilePos in hangingTile.RopesOfAllThisTileInTheWorld.Keys)
				{
					var worldPos = tilePos.ToWorldCoordinates();
					if (VFXManager.InScreen(worldPos, hangingTile.DrawOffScreenRange))
					{
						TileFluentDrawManager.AddFluentPoint(hangingTile, tilePos.X, tilePos.Y);
					}
				}
			}
		}
		return base.PreDraw(i, j, type, spriteBatch);
	}
}
