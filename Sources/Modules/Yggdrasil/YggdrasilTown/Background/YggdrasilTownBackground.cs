using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.Common.BackgroundManager;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Background;

public class YggdrasilTownBackground : ModSystem
{
	/// <summary>
	/// 地形中心
	/// </summary>
	public static Vector2 BackgroundAnchoredCenter => YggdrasilTownBiome.BiomeCenter;

	/// <summary>
	/// The world coord of [Origin Pylon]
	/// </summary>
	public static Vector2 OriginPylonCenter => new Vector2(1395, Main.maxTilesY - 405) * 16;

	/// <summary>
	/// 初始化
	/// </summary>
	public override void OnModLoad()
	{
		if (Main.netMode != NetmodeID.Server)
		{
			Ins.HookManager.AddHook(CodeLayer.PostDrawBG, DrawBackground);
		}
	}

	// Total background alpha.
	public float BackgroundAlpha = 0f;

	/// <summary>
	/// LampWoodAnchor Y
	/// </summary>
	public float LampWoodCenterY = 0;

	/// <summary>
	/// Background alpha of Yggdrasil Town
	/// </summary>
	public float BackgroundAlphaYggdrasilTown = 0f;

	/// <summary>
	/// Background alpha of midnight bayou
	/// </summary>
	public float BackgroundAlphaMidnightBayou = 0f;

	/// <summary>
	/// Background alpha of lampwood
	/// </summary>
	public float BackgroundAlphaLampWood = 0f;

	/// <summary>
	/// Background alpha of lampwood
	/// </summary>
	public float BackgroundAlphaTwilight = 0f;

	/// <summary>
	/// Background alpha of cage
	/// </summary>
	public float BackgroundAlphaCage = 0f;

	/// <summary>
	/// Background alpha of JellyBall hotbed
	/// </summary>
	public float BackgroundAlphaJellyBallHotbed = 0f;

	public static Vector2 Stratum1Center => new Vector2(Main.maxTilesX / 2f * 16, (Main.maxTilesY - 1000) * 16);

	/// <summary>
	/// 判定是否开启背景
	/// </summary>
	/// <returns></returns>
	public static bool BackgroundStratum1Active()
	{
		if (Main.screenPosition.Y > (Stratum1Center.Y - 18000))
		{
			if (SubworldSystem.IsActive<YggdrasilWorld>())
			{
				return true;
			}
		}
		if (YggdrasilTownCentralSystem.InYggdrasilTown(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f))
		{
			return true;
		}
		return false;
	}

	public override void PostUpdateEverything()// 开启地下背景
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

	/// <summary>
	/// 绘制天穹城镇背景
	/// </summary>
	/// <param name="baseColor"></param>
	private void DrawYggdrasilTownBackground(Color baseColor)
	{
		var texSky = ModAsset.YggdrasilTownBackgroundSky.Value;

		// 旧背景
		//BackgroundManager.DrawBG_RestrictY(texSky, float.PositiveInfinity, Stratum1Center, baseColor, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 16000));

		//DrawYggdrasilTown_Town(baseColor);
		DrawMidnightBayou(baseColor);
		DrawLampWood(baseColor);
		DrawTwilightForsetAndRelic(baseColor);
		DrawCageOfChallengers(baseColor);
		DrawJellyBallHotbed(baseColor);
	}

	//public void DrawYggdrasilTown_Town(Color baseColor)
	//{
	//	if (YggdrasilTownCentralSystem.InYggdrasilTown(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f))
	//	{
	//		if (BackgroundAlphaYggdrasilTown < 1f)
	//		{
	//			BackgroundAlphaYggdrasilTown += 0.02f;
	//		}
	//		else
	//		{
	//			BackgroundAlphaYggdrasilTown = 1f;
	//		}
	//	}
	//	else
	//	{
	//		if (BackgroundAlphaYggdrasilTown > 0f)
	//		{
	//			BackgroundAlphaYggdrasilTown -= 0.02f;
	//		}
	//		else
	//		{
	//			BackgroundAlphaYggdrasilTown = 0;
	//		}
	//	}

	//	if (BackgroundAlphaYggdrasilTown > 0)
	//	{
	//		var townClose = ModAsset.Town_Close.Value;
	//		var townMiddle = ModAsset.Town_Middle.Value;
	//		var townFar = ModAsset.Town_Far.Value;
	//		var townSky = ModAsset.Town_Sky.Value;
	//		Vector2 correction = OriginPylonCenter;
	//		if (!YggdrasilTownCentralSystem.InCanteen_YggdrasilTown() && !YggdrasilTownCentralSystem.InUnion_YggdrasilTown() && !YggdrasilTownCentralSystem.InPlayerRoom_YggdrasilTown() && !YggdrasilTownCentralSystem.InArena_YggdrasilTown())
	//		{
	//			BackgroundManager.DrawBG_RestrictY(townSky, float.PositiveInfinity, correction, baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townFar, 15f, correction, baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townMiddle, 6f, correction + new Vector2(0, 200), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townClose, 3f, correction + new Vector2(0, 200), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//		}
	//		else
	//		{
	//			BackgroundManager.DrawBG_RestrictY(townSky, float.PositiveInfinity, correction + new Vector2(0, 2000), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townFar, 15f, correction + new Vector2(0, 2000), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townMiddle, 6f, correction + new Vector2(0, 3800), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			BackgroundManager.DrawBG_RestrictY(townClose, 3f, correction + new Vector2(0, 3800), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
	//			{
	//				var arenaClose = ModAsset.ArenaBackground.Value;
	//				BackgroundManager.DrawBG_RestrictY(arenaClose, 1.5f, correction + new Vector2(3150, 3754), baseColor * BackgroundAlphaYggdrasilTown, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
	//			}
	//		}
	//	}
	//}

	public void DrawMidnightBayou(Color baseColor)
	{
		if (ModContent.GetInstance<MidnightBayouBiome>().IsBiomeActive(Main.LocalPlayer))
		{
			if (BackgroundAlphaMidnightBayou < 1f)
			{
				BackgroundAlphaMidnightBayou += 0.02f;
			}
			else
			{
				BackgroundAlphaMidnightBayou = 1f;
			}
		}
		else
		{
			if (BackgroundAlphaMidnightBayou > 0f)
			{
				BackgroundAlphaMidnightBayou -= 0.02f;
			}
			else
			{
				BackgroundAlphaMidnightBayou = 0;
			}
		}

		if (BackgroundAlphaMidnightBayou > 0)
		{
			var bayouClose = ModAsset.MidnightBayou_Close.Value;
			var bayouMiddle0 = ModAsset.MidnightBayou_Middle_0.Value;
			var bayouMiddle1 = ModAsset.MidnightBayou_Middle_1.Value;
			var bayouMiddle2 = ModAsset.MidnightBayou_Middle_2.Value;
			var bayouSky = ModAsset.MidnightBayou_Sky.Value;
			Vector2 correction = OriginPylonCenter;

			BackgroundManager.DrawBG_RestrictY(bayouSky, float.PositiveInfinity, correction, baseColor * BackgroundAlphaMidnightBayou, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
			BackgroundManager.DrawBG_RestrictY(bayouMiddle2, 80f, correction, baseColor * BackgroundAlphaMidnightBayou, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
			BackgroundManager.DrawBG_RestrictY(bayouMiddle1, 20f, correction, baseColor * BackgroundAlphaMidnightBayou, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
			BackgroundManager.DrawBG_RestrictY(bayouMiddle0, 10f, correction, baseColor * BackgroundAlphaMidnightBayou, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
			BackgroundManager.DrawBG_RestrictY(bayouClose, 6f, correction + new Vector2(0, 2950), baseColor * BackgroundAlphaMidnightBayou, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
		}
	}

	public void DrawJellyBallHotbed(Color baseColor)
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
			Vector2 anchorPos = new Vector2((leftBound + rightBound) / 2f, (upBound + bottomBound) / 2f - 120f);
			anchorPos *= 16f;

			BackgroundManager.DrawBG_RestrictY(jellyFar, 20f, anchorPos, baseColor * BackgroundAlphaJellyBallHotbed, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), true, true);
			BackgroundManager.DrawBG_RestrictY(jellyClose, 6f, anchorPos, baseColor * BackgroundAlphaJellyBallHotbed, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 18000), false, true);
		}
	}

	public void DrawCageOfChallengers(Color baseColor)
	{
		if (SubworldSystem.Current != null)
		{
			YggdrasilWorld yWorld = SubworldSystem.Current as YggdrasilWorld;
			if (yWorld != null)
			{
				// 获取挑战者石牢的位置
				if (yWorld.StoneCageOfChallengesCenter == Vector2.zeroVector)
				{
					for (int x = 50; x < Main.maxTilesX - 50; x++)
					{
						for (int y = 50; y < Main.maxTilesY - 50; y++)
						{
							Tile tile = TileUtils.SafeGetTile(x, y);
							if (tile.TileType == ModContent.TileType<SquamousShellSeal>())
							{
								if (tile.TileFrameX == 180 && tile.TileFrameY == 162)
								{
									yWorld.StoneCageOfChallengesCenter = new Vector2(x, y - 40) * 16;
									x = Main.maxTilesX - 50;
									break;
								}
							}
						}
					}
					if (yWorld.StoneCageOfChallengesCenter == Vector2.zeroVector)
					{
						yWorld.StoneCageOfChallengesCenter = new Vector2(Main.maxTilesX / 2, Main.maxTilesY - 1900) * 16;
					}
				}
				Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
				Vector2 backgroundCenter = yWorld.StoneCageOfChallengesCenter + new Vector2(0, -600);

				// 如果在挑战者石牢附近
				if (Math.Abs(yWorld.StoneCageOfChallengesCenter.X - screenCenter.X) < 240 * 16)
				{
					if (Math.Abs(yWorld.StoneCageOfChallengesCenter.Y - screenCenter.Y) < 60 * 16)
					{
						if (BackgroundAlphaCage < 1f)
						{
							BackgroundAlphaCage += 0.02f;
						}
						else
						{
							BackgroundAlphaCage = 1f;
						}
					}
					else
					{
						if (BackgroundAlphaCage > 0f)
						{
							BackgroundAlphaCage -= 0.02f;
						}
						else
						{
							LampWoodCenterY = Main.screenPosition.Y;
							BackgroundAlphaCage = 0;
						}
					}
				}
				else
				{
					if (BackgroundAlphaCage > 0f)
					{
						BackgroundAlphaCage -= 0.02f;
					}
					else
					{
						LampWoodCenterY = Main.screenPosition.Y;
						BackgroundAlphaCage = 0;
					}
				}
				if (BackgroundAlphaCage > 0)
				{
					var stoneClose2 = ModAsset.StoneCageOfChallengesClose2.Value;
					var stoneClose = ModAsset.StoneCageOfChallengesClose.Value;
					var stoneMiddle = ModAsset.StoneCageOfChallengesMiddle.Value;
					var stoneFar = ModAsset.StoneCageOfChallengesFar.Value;
					var stoneSky = ModAsset.StoneCageOfChallengesSky.Value;

					BackgroundManager.DrawBG_RestrictY(stoneSky, float.PositiveInfinity, backgroundCenter + new Vector2(0, -1800), baseColor * BackgroundAlphaCage, (int)(Stratum1Center.Y - 25600), (int)(Stratum1Center.Y + 8000), true, true);
					BackgroundManager.DrawBG_RestrictY(stoneFar, 30, backgroundCenter + new Vector2(0, -9000), baseColor * BackgroundAlphaCage, (int)(Stratum1Center.Y - 25600), (int)(Stratum1Center.Y + 8000), true, true);
					BackgroundManager.DrawBG_RestrictY(stoneMiddle, 20, backgroundCenter + new Vector2(0, -4800), baseColor * BackgroundAlphaCage, (int)(Stratum1Center.Y - 25600), (int)(Stratum1Center.Y + 8000), true, true);
					BackgroundManager.DrawBG_RestrictY(stoneClose, 12, backgroundCenter + new Vector2(0, -1800), baseColor * BackgroundAlphaCage, (int)(Stratum1Center.Y - 25600), (int)(Stratum1Center.Y + 8000), true, true);
					BackgroundManager.DrawBG_RestrictY(stoneClose2, 3, backgroundCenter + new Vector2(0, 100), baseColor * BackgroundAlphaCage, (int)(Stratum1Center.Y - 25600), (int)(Stratum1Center.Y + 8000), true, true);
				}
			}
		}
	}

	public void DrawLampWood(Color baseColor)
	{
		if (ModContent.GetInstance<LampWoodForest>().IsBiomeActive(Main.LocalPlayer))
		{
			if (Math.Abs(LampWoodCenterY - Main.screenPosition.Y) < Main.screenHeight)
			{
				if (BackgroundAlphaLampWood < 1f)
				{
					BackgroundAlphaLampWood += 0.02f;
				}
				else
				{
					BackgroundAlphaLampWood = 1f;
				}
			}
			else
			{
				if (BackgroundAlphaLampWood > 0f)
				{
					BackgroundAlphaLampWood -= 0.02f;
				}
				else
				{
					LampWoodCenterY = Main.screenPosition.Y;
					BackgroundAlphaLampWood = 0;
				}
			}
		}
		else
		{
			if (BackgroundAlphaLampWood > 0f)
			{
				BackgroundAlphaLampWood -= 0.02f;
			}
			else
			{
				BackgroundAlphaLampWood = 0;
			}
		}

		if (BackgroundAlphaLampWood > 0)
		{
			var lampClose = ModAsset.LampWoodClose.Value;
			var lampMiddle = ModAsset.LampWoodMiddle.Value;
			var lampFar = ModAsset.LampWoodFar.Value;
			var lampSky = ModAsset.LampWoodSky.Value;
			Vector2 correction = new Vector2(0, LampWoodCenterY - 4000);

			BackgroundManager.DrawBG_RestrictY(lampSky, float.PositiveInfinity, correction, baseColor * BackgroundAlphaLampWood, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), false, true);
			BackgroundManager.DrawBG_RestrictY(lampFar, 20f, correction, baseColor * BackgroundAlphaLampWood, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), false, true);
			BackgroundManager.DrawBG_RestrictY(lampMiddle, 10f, correction, baseColor * BackgroundAlphaLampWood, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), false, true);
			BackgroundManager.DrawBG_RestrictY(lampClose, 6f, correction + new Vector2(0, 6000), baseColor * BackgroundAlphaLampWood, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), false, true);
		}
	}

	public void DrawTwilightForsetAndRelic(Color baseColor)
	{
		if (ModContent.GetInstance<TwilightForsetAndRelic>().IsBiomeActive(Main.LocalPlayer))
		{
			if (BackgroundAlphaTwilight < 1f)
			{
				BackgroundAlphaTwilight += 0.02f;
			}
			else
			{
				BackgroundAlphaTwilight = 1f;
			}
		}
		else
		{
			if (BackgroundAlphaTwilight > 0f)
			{
				BackgroundAlphaTwilight -= 0.02f;
			}
			else
			{
				BackgroundAlphaTwilight = 0;
			}
		}

		if (BackgroundAlphaTwilight > 0)
		{
			var twilightClose = ModAsset.TwilightClose.Value;
			var twilightMiddleClose = ModAsset.TwilightMiddleClose.Value;
			var twilightMiddle = ModAsset.TwilightMiddle.Value;
			var twilightMiddleFar = ModAsset.TwilightMiddleFar.Value;
			var twilightFar = ModAsset.TwilightFar.Value;
			var twilightSky = ModAsset.TwilightSky.Value;
			BackgroundManager.DrawBG_RestrictY(twilightSky, float.PositiveInfinity, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
			BackgroundManager.DrawBG_RestrictY(twilightFar, 40, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
			BackgroundManager.DrawBG_RestrictY(twilightMiddleFar, 30, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
			BackgroundManager.DrawBG_RestrictY(twilightMiddle, 16, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
			BackgroundManager.DrawBG_RestrictY(twilightMiddleClose, 10, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
			BackgroundManager.DrawBG_RestrictY(twilightClose, 5, Stratum1Center, baseColor * BackgroundAlphaTwilight, (int)(Stratum1Center.Y - 20600), (int)(Stratum1Center.Y + 8000), true, true);
		}
	}

	/// <summary>
	/// 当然是绘制主体啦
	/// </summary>
	private void DrawBackground()
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		SpriteBatchState sBS2 = sBS;
		sBS2.BlendState = BlendState.NonPremultiplied;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS2);
		if (BackgroundAlpha <= 0)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
			return;
		}

		Color baseColor = Color.White * BackgroundAlpha;
		DrawYggdrasilTownBackground(baseColor);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
