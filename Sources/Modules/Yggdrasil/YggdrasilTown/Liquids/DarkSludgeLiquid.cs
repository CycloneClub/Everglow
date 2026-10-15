using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.Structs;
using Terraria.DataStructures;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Light;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Liquids;

public class DarkSludgeLiquid : ModLiquid
{
	public static bool CanDive = false;

	public override void SetStaticDefaults()
	{
		PlayerMovementMultiplier = 0.2f;
		StopWatchMPHMultiplier = 0.2f;
		NPCMovementMultiplierDefault = 0.2f;
		SlopeOpacity = 1f;
		WaterRippleMultiplier = 1f;
		SplashDustType = ModContent.DustType<Dusts.SplashDust_DarkSludge>();
		SplashSound = SoundID.SplashWeak;
		AddMapEntry(new Color(31, 26, 45));
	}

	public override int LiquidMerge(int i, int j, int otherLiquid)
	{
		return ModContent.TileType<DarkSludgeLiquid_SolidBlock>();
	}

	public override LightMaskMode LiquidLightMaskMode(int i, int j)
	{
		return LightMaskMode.None;
	}

	public override int ChooseWaterfallStyle(int i, int j)
	{
		return ModContent.GetInstance<DarkSludgeWaterfall>().Slot;
	}

	public override void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
	{
		drawData.liquidAlphaMultiplier = 1f;
	}

	public override bool OnPlayerSplash(Player player, bool isEnter)
	{
		return true;
	}

	public override bool PlayerLiquidMovement(Player player, bool fallThrough, bool ignorePlats)
	{
		if (!CanDive)
		{
			Vector2 topPos = player.position / 16f;
			topPos.Y -= 1f;
			Tile tile = Main.tile[topPos.ToPoint()];
			if (tile.LiquidType == LiquidLoader.LiquidType<DarkSludgeLiquid>() && tile.LiquidAmount > 100)
			{
				player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromKey("KilledBySludge")), 1000f, 0);
			}
		}
		return true;
	}

	public override bool OnNPCSplash(NPC npc, bool isEnter)
	{
		return true;
	}

	public override bool OnProjectileSplash(Projectile proj, bool isEnter)
	{
		return true;
	}

	public override bool OnItemSplash(Item item, bool isEnter)
	{
		return true;
	}

	public override void PostDraw(int i, int j, LiquidRenderer.LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
	{
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		var destRect = new Rectangle((int)(i * 16 - Main.screenPosition.X + zero.X), (int)(j * 16 - Main.screenPosition.Y + zero.Y), 16, 16);
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, destRect,liquidDrawCache.SourceRectangle, Color.Black);
		base.PostDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw);
	}
}
