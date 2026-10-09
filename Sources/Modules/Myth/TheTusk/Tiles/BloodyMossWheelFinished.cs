using Everglow.Myth.TheTusk.WorldGeneration;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheTusk.Tiles;

public class BloodyMossWheelFinished : ModTile
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
		DustType = 4;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(0, 0, 0, 0), modTranslation);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public int TpTime = 0;
	public static int[] PlayerTpTime = new int[255];
	private int col = 0;

	public override void PostDraw(int i, int j, SpriteBatch sb)
	{
		TileI = i;
		TileJ = j;
		DrawAll(sb);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		if ((player.Center - new Vector2(i * 16, j * 16 - 72)).Length() < 80)
		{
			if (!Main.gamePaused)
			{
				TpTime += 3;
			}

			col = 100;
		}
		else
		{
			if (col > 0)
			{
				col -= 5;
			}
			else
			{
				col = 0;
				TpTime = 0;
			}
		}
		if (TpTime >= 120)
		{
			if (SubworldSystem.IsActive<TuskWorld>())
			{
				SubworldSystem.Exit();
			}
			else
			{
				if (!SubworldSystem.Enter<TuskWorld>())
				{
					Main.NewText("Fail!");
				}
			}
			TpTime = 0;
		}
		PlayerTpTime[player.whoAmI] = TpTime;
		base.NearbyEffects(i, j, closer);
	}

	public override bool RightClick(int i, int j)
	{
		return base.RightClick(i, j);
	}

	public void DrawAll(SpriteBatch sb)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D Tdoor = Commons.ModAsset.Noise_flame_2.Value;
		Texture2D Tdoor2 = ModAsset.CosmicVort.Value;
		Texture2D Tdoor3 = ModAsset.CosmicPerlin.Value;

		sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 300f, new Vector2(56), 65f / 45f, SpriteEffects.None, 0f);
		sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(100, 100, 100, 0), -(float)Main.time / 200f, new Vector2(56), 65f / 45f, SpriteEffects.None, 0f);
		sb.Draw(Tdoor, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 150f, new Vector2(56), 65f / 50f, SpriteEffects.None, 0f);
		sb.Draw(Tdoor2, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 300f, new Vector2(56), 65 / 45f, SpriteEffects.None, 0f);
		sb.Draw(Tdoor3, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), -(float)Main.time / 200f, new Vector2(56), 65f / 45f, SpriteEffects.None, 0f);
		sb.Draw(Tdoor3, new Vector2(TileI * 16 + 8, TileJ * 16 - 68) - Main.screenPosition + zero, null, new Color(255, 255, 255, 0), (float)Main.time / 150f, new Vector2(56), 65 / 45f, SpriteEffects.None, 0f);

		Texture2D scene = ModAsset.TuskMiddle_Square.Value;
		Matrix matrix = sb.transformMatrix;

		sb.End();
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrix);

		Effect dissolve = ModAsset.Dissolve_WithCenter.Value;
		var projection = Matrix.CreateOrthographicOffCenter(-Main.offScreenRange, Main.screenWidth + Main.offScreenRange, Main.screenHeight + Main.offScreenRange, -Main.offScreenRange, 0, 1);
		float dissolveDuration = 1 - 0.2f;

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

	public static float TileI = 0;
	public static float TileJ = 0;
}
