using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.Common.BackgroundManager;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class YggdrasilTownBackground : ModSystem
{
	public static Vector2 BackgroundAnchoredCenter => YggdrasilTownBiome.BiomeCenter;

	public static Vector2 OriginPylonCenter => new Vector2(1395, Main.maxTilesY - 405) * 16;

	public static Vector2 Stratum1Center => new Vector2(Main.maxTilesX / 2f * 16, (Main.maxTilesY - 1000) * 16);

	public float BackgroundAlpha;

	public float BackgroundAlphaMidnightBayou;

	public float BackgroundAlphaJellyBallHotbed;

	//public override void OnModLoad()
	//{
	//	if (Main.netMode != NetmodeID.Server)
	//	{
	//		Ins.HookManager.AddHook(CodeLayer.PostDrawBG, DrawBackground);
	//	}
	//}

	public static bool BackgroundStratum1Active()
	{
		return Main.screenPosition.Y > Stratum1Center.Y - 18000 && SubworldSystem.IsActive<YggdrasilWorld>()
			? true
			: YggdrasilTownCentralSystem.InYggdrasilTown(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f);
	}

	public override void PostUpdateEverything()
	{
		const float increase = 0.02f;
		if (BackgroundStratum1Active() && Main.BackgroundEnabled)
		{
			YggdrasilEnvironmentLightManager.LightingScene = YggdrasilScene.YggdrasilTown;
			if (BackgroundAlpha < 1)
			{
				BackgroundAlpha += increase;
			}
			else
			{
				BackgroundAlpha = 1;
				Ins.HookManager.Disable(TerrariaFunction.DrawBackground);
			}
		}
		else
		{
			if (BackgroundAlpha > 0)
			{
				BackgroundAlpha -= increase;
			}
			else
			{
				BackgroundAlpha = 0;
			}
			Ins.HookManager.Enable(TerrariaFunction.DrawBackground);
		}
	}

	private void DrawYggdrasilTownBackground(Color baseColor)
	{
		DrawJellyBallHotbed(baseColor);
	}

	private void DrawJellyBallHotbed(Color baseColor)
	{
		if (ModContent.GetInstance<JellyBallHotbedBiome>().IsBiomeActive(Main.LocalPlayer))
		{
			if (BackgroundAlphaJellyBallHotbed < 1f)
			{
				BackgroundAlphaJellyBallHotbed += 0.02f;
			}
			else
			{
				BackgroundAlphaJellyBallHotbed = 1f;
			}
		}
		else
		{
			if (BackgroundAlphaJellyBallHotbed > 0f)
			{
				BackgroundAlphaJellyBallHotbed -= 0.02f;
			}
			else
			{
				BackgroundAlphaJellyBallHotbed = 0;
			}
		}

		if (BackgroundAlphaJellyBallHotbed > 0)
		{
			var jellyClose = ModAsset.JellyBallHotbed_Close.Value;
			var jellyFar = ModAsset.JellyBallHotbed_Far.Value;
			int upBound = Main.maxTilesY - 500;
			int bottomBound = Main.maxTilesY - 40;
			int leftBound = Main.maxTilesX - 350;
			int rightBound = Main.maxTilesX;
			Vector2 anchorPos = new Vector2((leftBound + rightBound) / 2f, (upBound + bottomBound) / 2f - 120f) * 16f;

			BackgroundManager.DrawBG_RestrictY(jellyFar, 20f, anchorPos, baseColor * BackgroundAlphaJellyBallHotbed, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), true, true);
			BackgroundManager.DrawBG_RestrictY(jellyClose, 6f, anchorPos, baseColor * BackgroundAlphaJellyBallHotbed, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
		}
	}

	//private void DrawBackground()
	//{
	//	SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
	//	SpriteBatchState sBS2 = sBS;
	//	sBS2.BlendState = BlendState.NonPremultiplied;
	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(sBS2);
	//	if (BackgroundAlpha <= 0)
	//	{
	//		Main.spriteBatch.End();
	//		Main.spriteBatch.Begin(sBS);
	//		return;
	//	}

	//	DrawYggdrasilTownBackground(Color.White * BackgroundAlpha);
	//	Main.spriteBatch.End();
	//	Main.spriteBatch.Begin(sBS);
	//}
}
