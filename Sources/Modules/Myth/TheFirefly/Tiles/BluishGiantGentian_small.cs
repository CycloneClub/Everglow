using Everglow.Commons.TileHelper;
using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Gores;
using Everglow.Myth.TheFirefly.Items;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class BluishGiantGentian_small : ModTile, ITileFluentlyDrawn
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileCut[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 60;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<BluishGiantGentian_dust>();
		AddMapEntry(new Color(6, 90, 255));
		HitSound = SoundID.Grass;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
		{
			return;
		}

		var thisTile = Main.tile[i, j];
		int x0 = i;
		int y0 = j - thisTile.TileFrameY / 18;
		int times = 1;
		for (int y = 0; y < 2; y++)
		{
			var tile = Main.tile[x0, y0 + y];
			if (tile.TileFrameY == y * 18)
			{
				if (tile.TileType == Type)
				{
					times++;
					tile.HasTile = false;
				}
			}
		}
		SoundEngine.PlaySound(HitSound, new Vector2(i * 16, j * 16));
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		short num = (short)Main.rand.Next(0, 6);
		Main.tile[i, j].TileFrameX = (short)(num * 48);
		Main.tile[i, j + 1].TileFrameX = (short)(num * 48);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 18)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		if (tile.TileFrameY == 0)
		{
			Lighting.AddLight(new Vector2(i, j) * 16 + new Vector2(8), new Vector3(0f, 0.1f, 0.4f));
		}
		return false;
	}

	private static double _oldWindCounter = 0;
	private static double _ooldWindCounter = 0;

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;
		DrawBlossomPiece(ModAsset.BluishGiantGentian_small_Path, pos, drawCenterPos, spriteBatch, tileDrawing);
	}

	/// <summary>
	/// 绘制花花的一个小Piece
	/// </summary>
	private void DrawBlossomPiece(string texPath, Point tilePos, Vector2 drawCenterPos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[tilePos];
		ushort type = tile.TileType;
		var frame = new Rectangle(tile.TileFrameX, 0, 48, 48);

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[tilePos].TileColor;
		int textureStyle = tile.TileFrameX;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(texPath, type, textureStyle, paint, tileDrawing);
		tex ??= ModContent.Request<Texture2D>($"Everglow/{texPath}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		var topLeft = tilePos - new Point(1, 2);
		var size = new Point(3, 3);

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(topLeft.X, topLeft.Y, size.X, size.Y))
		{
			windCycle = tileDrawing.GetWindCycle(tilePos.X, tilePos.Y, tileDrawing._sunflowerWindCounter);
		}

		int totalPushTime = 120;
		float pushForcePerFrame = 2.96f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, size.X, size.Y, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * 0.14f;

		// 角速度计算，源码乱抄与瞎写代码的结合，用于抖花粉
		float factor = tilePos.X + tilePos.Y / 100;
		float radiusNow = (float)Math.Cos(tileDrawing._sunflowerWindCounter * Math.PI * 2.0 + (double)factor);
		float radiusPrev = (float)Math.Cos(_ooldWindCounter * Math.PI * 2.0 + (double)factor);
		float angularVelocity = highestWindGridPushComplex + radiusNow - radiusPrev;

		// 颜色
		Color tileLight = Lighting.GetColor(tilePos);

		// 支持发光涂料
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(tilePos.X, tilePos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(tilePos.Y, tilePos.X, tile, type, 0, 0, tileLight);

		var origin = new Vector2(24, 48);
		var tileSpriteEffect = SpriteEffects.None;
		if (tilePos.X % 2 == 1)
		{
			tileSpriteEffect = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(0, 4), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
		frame.Y += 48;
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(0, 4), frame, new Color(0f, 0.2f, 0.7f, 0f), rotation, origin, 1f, tileSpriteEffect, 0f);

		// 抖落蓝色荧光花粉
		if (Math.Abs(angularVelocity) > 0.04 && Main.rand.NextBool(Math.Clamp((int)(100 - Math.Abs(angularVelocity) * 40f), 60, 900)))
		{
			var dustSpawnOffset = new Vector2(-12, -40);
			var dustSpawnPos = drawCenterPos + dustSpawnOffset.RotatedBy(rotation) + Main.screenPosition;
			var dustVelocity = new Vector2(-1f, 0f).RotatedBy(angularVelocity) * 0.7f;
			if (angularVelocity > 0)
			{
				dustVelocity = -dustVelocity;
			}

			var d = Dust.NewDustDirect(dustSpawnPos, 24, 30, ModContent.DustType<BlueParticleDark>(), Alpha: 150);
			var d2 = Dust.NewDustDirect(dustSpawnPos, 24, 30, ModContent.DustType<BlueParticle>(), Alpha: 150);
			d.scale = Main.rand.NextFloat() * 0.2f + 0.2f;
			d2.scale = Main.rand.NextFloat() * 0.2f + 0.3f;
			d.velocity = dustVelocity;
			d2.velocity = dustVelocity;
		}

		_ooldWindCounter = _oldWindCounter;
		_oldWindCounter = tileDrawing._sunflowerWindCounter;
	}
}
