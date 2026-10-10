using Everglow.Myth.Misc.FixCoins;
using Terraria.Audio;
using Terraria.ObjectData;

namespace Everglow.Myth.TheTusk.Tiles;

public class BloodyMossWheel : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.addTile(Type);
		DustType = DustID.TintableDust;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(96, 93, 91), modTranslation);
	}

	public override bool CanExplode(int i, int j)
	{
		return CanK;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return CanK;
	}

	private float[] rot = new float[24];
	private float[] aimRot = new float[24];
	private float rot2 = 0;
	private int[] cooling = new int[24];
	private int[] dir = new int[24];
	public static int Symbol = 1;
	private int step = 0;
	private int stepChange = 0;
	public static int Killing = 0;
	public static float TileI = 0;
	public static float TileJ = 0;
	public static bool CanK = false;

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D BaseCo1 = ModAsset.ForgeWave.Value;
		Texture2D Sp1a = ModAsset.StonePanBottomLine.Value;
		Texture2D Sp1b = ModAsset.StonePanBottomRedLight.Value;
		Texture2D Sp1c = ModAsset.StonePanBottomFace.Value;
		Texture2D Sp2 = ModAsset.StonePanPin.Value;
		Texture2D SpL1 = ModAsset.StonePanL1.Value;
		Texture2D SpL2 = ModAsset.StonePanL2.Value;
		Texture2D SpL3 = ModAsset.StonePanL3.Value;
		Texture2D SpL4 = ModAsset.StonePanL4.Value;
		Texture2D SpIC = ModAsset.StonePanStrick.Value;
		var origin = new Vector2(56);

		Color color = Lighting.GetColor(i, j);
		Vector2 v = new Vector2(0, 100).RotatedBy(Main.time / 60f);
		spriteBatch.Draw(BaseCo1, new Vector2(i * 16 - 16 + 6, j * 16 - 68 - 16) + origin - Main.screenPosition + zero, new Rectangle(256 + (int)v.X, 256 + (int)v.Y, 32, 32), new Color(255, 0, 0, 255), 0f, origin, 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(BaseCo1, new Vector2(i * 16 - 16 + 6, j * 16 - 68 - 16) + origin - Main.screenPosition + zero, new Rectangle(256 - (int)v.X, 256 - (int)v.Y, 32, 32), new Color(255, 125, 0, 0), 0f, origin, 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(Sp1a, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);
		if (stepChange > 0)
		{
			if (stepChange > 10)
			{
				spriteBatch.Draw(Sp1b, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), 0f, origin, (30 - stepChange) / 20f, SpriteEffects.None, 0f);
			}
			else
			{
				spriteBatch.Draw(Sp1b, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(stepChange * 25, stepChange * 25, stepChange * 25, 0), 0f, origin, 0.95f, SpriteEffects.None, 0f);
			}
			stepChange--;
		}
		else
		{
			stepChange = 0;
		}
		if (step == 0)
		{
			rot2 = 0;
		}

		if (step == 1)
		{
			if (stepChange > 0)
			{
				rot2 = (float)(Math.PI / 2d) * (30 - stepChange) / 30f;
			}
			else
			{
				rot2 = (float)(Math.PI / 2d);
			}
		}
		if (step == 2)
		{
			if (stepChange > 0)
			{
				rot2 = (float)(Math.PI / 2d) * (30 - stepChange) / 30f + (float)(Math.PI / 2d);
			}
			else
			{
				rot2 = (float)Math.PI;
			}
		}
		spriteBatch.Draw(Sp1c, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, color, 0f, origin, 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(Sp2, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, color, rot2, origin, 1f, SpriteEffects.None, 0f);
		Symbol = step + 1;

		// 0↑
		// 1→
		// 2↓
		// 3←
		if (dir[0] == 6 && dir[1] == 1 && dir[2] == 2 && dir[3] == 0)
		{
			if (step == 0)
			{
				step += 1;
				stepChange = 30;
			}
		}
		if (dir[0] == 7 && dir[1] == 0 && dir[2] == 4 && dir[3] == 0)
		{
			if (step == 1)
			{
				step += 1;
				stepChange = 30;
			}
		}
		if (dir[0] == 3 && dir[1] == 2 && dir[2] == 3 && dir[3] == 5)
		{
			if (step == 2)
			{
				SoundEngine.PlaySound(SoundID.Chat, new Vector2(i * 16 + 6, j * 16 - 64));
				step += 1;
				CanK = true;
				Killing = 60;
			}
		}
		Vector2 vZoom = Main.GameViewMatrix.Zoom;

		Vector2 ScreenCenter = Main.screenTarget.Size() / 2f + Main.screenPosition; // 世界位置屏幕中心
		Vector2 CorrectedMouseScreenCenter = (Main.MouseScreen - Main.screenTarget.Size() / 2f) / vZoom.X; // 鼠标的中心指向位
		Vector2 CorrectedMouseWorld = CorrectedMouseScreenCenter + ScreenCenter; // 鼠标世界坐标校正

		spriteBatch.Draw(ModContent.Request<Texture2D>("Everglow/Myth/TheTusk/Tiles/TileEffects/Symbol" + Symbol.ToString()).Value, new Vector2(i * 16 + 6, j * 16 - 68) + zero - Main.screenPosition, null, new Color(150 + (int)v.X, 150 + (int)v.X, 150 + (int)v.X, 0), 0f, origin, 1f, SpriteEffects.None, 0f);
		if ((CorrectedMouseWorld - (new Vector2(i * 16 + 6, j * 16 - 64) + new Vector2(0, -50))).Length() < 12)
		{
			spriteBatch.Draw(SpL1, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), 0, origin, 1f, SpriteEffects.None, 0f);
			if (Main.mouseRight && !Main.gamePaused)
			{
				if (cooling[0] <= 0)
				{
					dir[0]++;
					if (dir[0] >= 8)
					{
						dir[0] = 0;
					}

					aimRot[0] = (float)Math.PI / 4f * dir[0];
					cooling[0] = 4;
				}
			}
		}
		if ((CorrectedMouseWorld - (new Vector2(i * 16 + 6, j * 16 - 68) + new Vector2(50, 0))).Length() < 12)
		{
			spriteBatch.Draw(SpL2, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), 0, origin, 1f, SpriteEffects.None, 0f);
			if (Main.mouseRight && !Main.gamePaused)
			{
				if (cooling[1] <= 0)
				{
					dir[1]++;
					if (dir[1] >= 8)
					{
						dir[1] = 0;
					}

					aimRot[1] = (float)Math.PI / 4f * dir[1];
					cooling[1] = 4;
				}
			}
		}
		if ((CorrectedMouseWorld - (new Vector2(i * 16 + 6, j * 16 - 68) + new Vector2(0, 50))).Length() < 12)
		{
			spriteBatch.Draw(SpL3, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), 0, origin, 1f, SpriteEffects.None, 0f);
			if (Main.mouseRight && !Main.gamePaused)
			{
				if (cooling[2] <= 0)
				{
					dir[2]++;
					if (dir[2] >= 8)
					{
						dir[2] = 0;
					}

					aimRot[2] = (float)Math.PI / 4f * dir[2];
					cooling[2] = 4;
				}
			}
		}
		if ((CorrectedMouseWorld - (new Vector2(i * 16 + 6, j * 16 - 68) + new Vector2(-50, 0))).Length() < 12)
		{
			spriteBatch.Draw(SpL4, new Vector2(i * 16 + 6, j * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), 0, origin, 1f, SpriteEffects.None, 0f);
			if (Main.mouseRight && !Main.gamePaused)
			{
				if (cooling[3] <= 0)
				{
					dir[3]++;
					if (dir[3] >= 8)
					{
						dir[3] = 0;
					}

					aimRot[3] = (float)Math.PI / 4f * dir[3];
					cooling[3] = 4;
				}
			}
		}
		spriteBatch.Draw(SpIC, new Vector2(i * 16 + 6, j * 16 - 68) + zero + new Vector2(0, -46) - Main.screenPosition, null, color, rot[0], new Vector2(12), 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(SpIC, new Vector2(i * 16 + 6, j * 16 - 68) + zero + new Vector2(46, 0) - Main.screenPosition, null, color, rot[1], new Vector2(12), 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(SpIC, new Vector2(i * 16 + 6, j * 16 - 68 - 4) + zero + new Vector2(0, 50) - Main.screenPosition, null, color, rot[2], new Vector2(12), 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(SpIC, new Vector2(i * 16 + 6, j * 16 - 68) + zero + new Vector2(-46, 0) - Main.screenPosition, null, color, rot[3], new Vector2(12), 1f, SpriteEffects.None, 0f);
		for (int l = 0; l < 4; l++)
		{
			if (cooling[l] > 0)
			{
				rot[l] += (float)Math.PI / 20f;
				cooling[l]--;
			}
			else
			{
				rot[l] = aimRot[l];
			}
		}

		if (Killing < -5)
		{
			CanK = false;
			TuskModPlayer mplayer = Main.player[Main.myPlayer].GetModPlayer<TuskModPlayer>();
			mplayer.Shake = 6;
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, new Vector2(i * 16 + 0, j * 16 - 72));
			Vector2 vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			Vector2 vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak1").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak1").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak2").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak3").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak4").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak5").Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
			vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
			Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak6").Type, 1f);
			for (int h = 0; h < 7; h++)
			{
				vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
				vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
				Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak7").Type, 1f);
			}
			for (int h = 0; h < 10; h++)
			{
				vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
				vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
				Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak8").Type, 1f);
			}
			for (int h = 0; h < 10; h++)
			{
				vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
				vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
				Gore.NewGore(null, new Vector2(i * 16 + 0, j * 16 - 72) + vF2, vF, ModContent.Find<ModGore>("Everglow/StonePanBreak9").Type, 1f);
			}
			for (int z = 0; z < 120; z++)
			{
				vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28);
				vF2 = new Vector2(0, Main.rand.NextFloat(0, 45f)).RotatedByRandom(6.28);
				Dust.NewDust(new Vector2(i * 16 + 0, j * 16 - 72) + vF2, 0, 0, DustID.TintableDust, vF.X, vF.Y, 0, default, Main.rand.NextFloat(0.8f, 2.1f));
			}
			int[] Ty = { ModContent.ItemType<FixCoinDamage3>(), ModContent.ItemType<FixCoinCrit3>(), ModContent.ItemType<FixCoinDefense3>(), ModContent.ItemType<FixCoinSpeed3>(), ModContent.ItemType<FixCoinMelee3>() };
			Item.NewItem(null, new Vector2(i * 16 + 0, j * 16 - 72), Ty[Main.rand.Next(Ty.Length)]);
			Item.NewItem(null, new Vector2(i * 16 + 0, j * 16 - 72), ItemID.GoldCoin, 5);
			Main.tile[i, j].TileType = (ushort)ModContent.TileType<BloodyMossWheelFinished>();
			((Tile)Main.tile[i, j]).HasTile = true;
		}
		TileI = i;
		TileJ = j;
		DrawAll(spriteBatch);
	}

	public static void DrawAll(SpriteBatch sb)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D Tdoor = ModAsset.CosmicFlame.Value;
		Texture2D Tdoor2 = ModAsset.CosmicVort.Value;
		Texture2D Tdoor3 = ModAsset.CosmicPerlin.Value;
		if (CanK)
		{
			if (!Main.gamePaused)
			{
				Killing--;
			}
			sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 300f, new Vector2(56), (60 - Killing) / 45f, SpriteEffects.None, 0f);
			sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(100, 100, 100, 0), -(float)Main.time / 200f, new Vector2(56), (60 - Killing) / 45f, SpriteEffects.None, 0f);
			sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 150f, new Vector2(56), (60 - Killing) / 50f, SpriteEffects.None, 0f);
			sb.Draw(Tdoor2, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 300f, new Vector2(56), (60 - Killing) / 45f, SpriteEffects.None, 0f);
			sb.Draw(Tdoor3, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), -(float)Main.time / 200f, new Vector2(56), (60 - Killing) / 45f, SpriteEffects.None, 0f);
			sb.Draw(Tdoor3, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 150f, new Vector2(56), (60 - Killing) / 45f, SpriteEffects.None, 0f);

			Texture2D scene = ModAsset.TuskMiddle_Square.Value;
			Matrix matrix = sb.transformMatrix;

			sb.End();
			sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);

			Effect dissolve = ModAsset.Dissolve_WithCenter.Value;
			var projection = Matrix.CreateOrthographicOffCenter(-Main.offScreenRange, Main.screenWidth + Main.offScreenRange, Main.screenHeight + Main.offScreenRange, -Main.offScreenRange, 0, 1);
			float dissolveDuration = (60 - Killing) / 60f - 0.2f;

			dissolve.Parameters["uTransform"].SetValue(matrix * projection);
			dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			dissolve.Parameters["duration"].SetValue(dissolveDuration);
			dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.8f, 0, 0.1f, 1f));
			dissolve.Parameters["uNoiseSize"].SetValue(3f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(0, (float)Main.timeForVisualEffects * 0.0003f));
			dissolve.CurrentTechnique.Passes[0].Apply();

			sb.Draw(scene, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition, null, Color.White * 0.8f, 0, scene.Size() * 0.5f, 0.25f, SpriteEffects.None, 0f);

			sb.End();
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);
		}
	}
}
