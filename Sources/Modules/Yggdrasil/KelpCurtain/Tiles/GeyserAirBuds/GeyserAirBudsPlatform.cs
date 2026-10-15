using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.GeyserAirBuds;

public class GeyserAirBudsPlatform : ModTile, ITileFluentlyDrawn
{
	public override void SetStaticDefaults()
	{
		MinPick = 150;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileNoAttach[Type] = true;
		RegisterItemDrop(ModContent.ItemType<GeyserAirBudsItem>(), 1);

		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [16, 22];
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.AnchorBottom = new(Terraria.Enums.AnchorType.SolidTile, 2, 0);
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<GeyserBudDust_Blue>();
		AnimationFrameHeight = 96;

		AddMapEntry(new Color(110, 239, 48));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && !tile.HasTile)
		{
			Vector2 center = new Point(i, j).ToWorldCoordinates() + new Vector2(8);
			for (int k = 0; k < 60; k++)
			{
				Vector2 pos = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 100);
				if (pos.Y > 50)
				{
					pos.Y = Main.rand.NextFloat(-50, 50);
				}
				pos -= new Vector2(4);
				int type = ModContent.DustType<GeyserBudDust_Blue>();
				if (Main.rand.NextBool(8))
				{
					type = ModContent.DustType<GeyserBudDust_Red>();
				}
				Dust.NewDust(pos + center, 0, 0, type);
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];

		int startX = i - tile.TileFrameX / 18;
		int startY = j - tile.TileFrameY / 18;
		Rectangle tileRect = new Rectangle(
			startX * 16,
			startY * 16,
			2 * 16,
			32);
		if (tile.TileType != Type)
		{
			return false;
		}
		foreach (Player player in Main.player)
		{
			if (!player.active || player.dead)
			{
				continue;
			}
			bool isStandingOnPlatform = IsPlayerStandingOnPlatform(player, tileRect);
			if (isStandingOnPlatform)
			{
				ReplaceWithGeyserAirBuds(startX + 1, startY + 1, triggeredByPlayer: true);
				Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_TileBreak(startX, startY), new Point(startX, startY).ToWorldCoordinates() + new Vector2(8), Vector2.zeroVector, ModContent.ProjectileType<GeyserAirBuds_Erupt>(), 0, 0, player.whoAmI);
				break;
			}
		}
		if (tile.TileFrameY == 18 && tile.TileFrameX == 0)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return false;
	}

	public void FluentDraw(Vector2 screenPosition, Point pos, SpriteBatch spriteBatch, TileDrawing tileDrawing)
	{
		var tile = Main.tile[pos];
		var drawCenterPos = pos.ToWorldCoordinates(autoAddY: 16) - screenPosition;

		ushort type = tile.TileType;
		if (type != Type)
		{
			return;
		}

		// 回声涂料
		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		int paint = Main.tile[pos].TileColor;
		Texture2D tex = PaintedTextureSystem.TryGetPaintedTexture(ModAsset.GeyserAirBuds_Untrimmed_Path, type, 0, paint, tileDrawing);
		tex ??= ModAsset.GeyserAirBuds_Untrimmed.Value;

		float windCycle = 0;
		if (tileDrawing.InAPlaceWithWind(pos.X, pos.Y - 1, 2, 2))
		{
			windCycle = tileDrawing.GetWindCycle(pos.X, pos.Y - 1, tileDrawing._sunflowerWindCounter);
		}
		float swayStrength = 0.05f;
		int totalPushTime = 80;
		float pushForcePerFrame = 1.26f;
		float highestWindGridPushComplex = tileDrawing.GetHighestWindGridPushComplex(pos.X, pos.Y - 1, 2, 2, totalPushTime, pushForcePerFrame, 3, swapLoopDir: true);
		windCycle += highestWindGridPushComplex;
		float rotation = windCycle * swayStrength;

		// 颜色
		Color tileLight = Lighting.GetColor(pos);
		tileDrawing.DrawAnimatedTile_AdjustForVisionChangers(pos.X, pos.Y, tile, type, 0, 0, ref tileLight, tileDrawing._rand.NextBool(4));
		tileLight = tileDrawing.DrawTiles_GetLightOverride(pos.Y, pos.X, tile, type, 0, 0, tileLight);

		var origin = new Vector2(48, 96);
		var tileSpriteEffect = SpriteEffects.None;

		int frameYOffset = 0;
		Rectangle frame = new Rectangle(0, frameYOffset, 96, AnimationFrameHeight);
		spriteBatch.Draw(tex, drawCenterPos + new Vector2(8, 6), frame, tileLight, rotation, origin, 1f, tileSpriteEffect, 0f);
		Lighting.AddLight(pos.ToWorldCoordinates() + new Vector2(8, 6), 0.21f, 0.45f, 0.32f);
		Lighting.AddLight(pos.ToWorldCoordinates() + new Vector2(-8, 6), 0.21f, 0.45f, 0.32f);
		if (!Main.gamePaused && Main.rand.NextBool(21))
		{
			Vector2 dustPos = pos.ToWorldCoordinates() + new Vector2(8, -20) - new Vector2(4);
			Dust d = Dust.NewDustDirect(dustPos, 0, 0, ModContent.DustType<GeyserBudPollen>());
			d.customData = dustPos.X;
			d.position.X += Main.rand.NextFloat(-10, 10);
			d.velocity.Y = -1.5f;
		}
	}

	/*public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		Item item = Main.LocalPlayer.HeldItem;
		Vector2 position = new Vector2(i * 16 + 8, j * 16 + 8);

		if (item.pick == 0 && item.axe == 0)
		{
			fail = noItem = true;

			CreatePlantEffect(position, 8);

			Tile tile = Main.tile[i, j];
			int baseX = i - (tile.TileFrameX / 18 - 1);
			int baseY = j - (tile.TileFrameY / 18 - 1);

		    ReplaceWithGeyserAirBuds(baseX, baseY, false);
		}
		 else if ((item.pick > 0 && item.pick < 350) || (item.axe > 0 && item.axe < 80))
		{
			Vector2 velocity = new Vector2(
				Main.rand.NextFloat(-1.5f, 1.5f));

			Dust.NewDustPerfect(
				Position,
				DustID.GrassBlades,
				Velocity,
				150, new Color(100, 200, 100), Main.rand.NextFloat(0.8f, 1.2f));
			fail = true;
			return;
		}
}*/

	private bool IsPlayerStandingOnPlatform(Player player, Rectangle platformBounds)// 是否站在平台上
	{
		if (player.Bottom.Y < platformBounds.Top)
		{
			return false;
		}

		float playerBottom = player.Bottom.Y;
		float platformTop = platformBounds.Top;

		if (Math.Abs(playerBottom - platformTop) > 2f)
		{
			return false;
		}

		float playerLeft = player.Left.X;
		float playerRight = player.Right.X;
		float platformLeft = platformBounds.Left;
		float platformRight = platformBounds.Right;

		// 我感觉4格写起来不灵活所以调成2x2了
		return playerRight > platformLeft && playerLeft < platformRight;
	}

	public static void ReplaceWithGeyserAirBuds(int i, int j, bool triggeredByPlayer) // 替换动画块
	{
		Point16 origin = new(1, 1);
		int baseX = i - origin.X;
		int baseY = j - origin.Y;
		Tile origTile = Main.tile[baseX, baseY];
		int newTileType = ModContent.TileType<GeyserAirBuds>();
		for (int x = 0; x < 2; x++)
		{
			for (int y = 0; y < 2; y++)
			{
				Tile t = Main.tile[baseX + x, baseY + y];
				t.TileType = (ushort)newTileType;
				t.TileFrameX = (short)(x * 18);
				t.TileFrameY = (short)(y * 18);
			}
		}

		if (triggeredByPlayer)
		{
			Player player = Main.LocalPlayer;
			player.velocity.Y = -22f;
			player.fallStart = (int)(player.position.Y / 16f);
		}

		ModTileEntity.PlaceEntityNet(baseX + origin.X, baseY + origin.Y, ModContent.TileEntityType<GeyserAirBudsEntity>());

		if (Main.netMode != NetmodeID.SinglePlayer)
		{
			NetMessage.SendTileSquare(-1, baseX + 1, baseY + 1, 3, 2);
		}
	}

	private void CreatePlantEffect(Vector2 position, int count) // 自定义dust效果
	{
		for (int i = 0; i < count; i++)
		{
			Vector2 velocity = new Vector2(
				Main.rand.NextFloat(-1.5f, 1.5f),
				Main.rand.NextFloat(-3f, -1f));

			Dust.NewDustPerfect(
				position,
				DustID.GrassBlades,
				velocity,
				150, new Color(100, 200, 100), Main.rand.NextFloat(0.8f, 1.2f));
		}

		for (int i = 0; i < 2; i++)
		{
			Dust.NewDustPerfect(
				position,
				DustID.GrassBlades,
				new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0f)),
				100, new Color(255, 255, 200, 150), Main.rand.NextFloat(0.7f, 1f));
		}
	}

	public override bool CreateDust(int i, int j, ref int type)
	{
		return false;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<GeyserAirBudsItem>());
	}
}
