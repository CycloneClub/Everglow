using Terraria.DataStructures;
using Terraria.ObjectData;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.CorruptWormHive.Dusts;
using Everglow.Yggdrasil.CorruptWormHive.VFXs;

namespace Everglow.Yggdrasil.CorruptWormHive.Tiles;

public class BloodLightCrystal : ModTile
{
	public static Vector4 EDGE_COLOR = new Vector4(1, 40f / 255f, 7f / 255f, 1);
	public static Vector2 EDGE_THRESHOLD = new Vector2(0.01f, 0.15f);
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BloodLightCrystalEntity>().Hook_AfterPlacement, -1, 0, false);
		TileObjectData.addTile(Type);
	}
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		MinPick = 375;

		AddMapEntry(new Color(107, 34, 21, 205));
	}
	public override bool CanExplode(int i, int j)
	{
		return false;
	}
	public override bool CreateDust(int i, int j, ref int type)
	{
		return false;
	}
	public override void NearbyEffects(int i, int j, bool closer)
	{
		Color c0 = Lighting.GetColor(i, j);

		if (c0.R > 160 && c0.G + c0.B < 80)
		{
			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing)
				&& existing is BloodLightCrystalEntity existingAsT)
			{
				existingAsT.startDissolve();
				if (Main.rand.NextBool(10))
					SummonCrystal(i, j);
			}

			// WorldGen.KillTile(i, j,false,false,true);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing)
			   && existing is BloodLightCrystalEntity existingAsT)
		{
			float dissolveProgress = existingAsT.getDissolveProgress();

			if (dissolveProgress > 0)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

				Vector2 worldPos = new Vector2(i, j) * 16 + new Vector2(Main.offScreenRange);
				Texture2D tex = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Texture2D noiseTex = ModAsset.DissolveNoise.Value;
				Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
				Effect dissolveEffect = ModAsset.NoiseDissolve.Value;
				dissolveEffect.Parameters["worldPos"].SetValue(worldPos);
				dissolveEffect.Parameters["textureWidth"].SetValue(400);
				dissolveEffect.Parameters["progress"].SetValue(dissolveProgress);
				dissolveEffect.Parameters["edgeColor"].SetValue(EDGE_COLOR);
				dissolveEffect.Parameters["edgeThreshold"].SetValue(EDGE_THRESHOLD);
				dissolveEffect.CurrentTechnique.Passes["NoiseDissolve"].Apply();

				Tile tile = Main.tile[i, j];

				spriteBatch.Draw(tex, worldPos - Main.screenPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j));
				spriteBatch.End();
				spriteBatch.Begin();

				return false;
			}
		}
		return base.PreDraw(i, j, spriteBatch);
	}
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		// 待定
	}

	public static void SummonDust(int i, int j)
	{
		Dust d = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<BloodSpark>());
		d.velocity = new Vector2(Main.rand.NextFloat(1.5f, 4f), 0).RotatedByRandom(6.283);

		d.color.R = (byte)Main.rand.Next(15, 255);
		d.alpha = Main.rand.Next(10);
		d.noGravity = true;
	}

	public static void SummonCrystal(int i, int j)
	{
		var bc = new BrokenCrystal
		{
			timeLeft = 70,
			size = Main.rand.NextFloat(0.65f, 1f),
			Velocity = new Vector2(Main.rand.NextFloat(2.5f, 7.5f), 0).RotatedByRandom(6.283),
			Active = true,
			Visible = true,
			Position = new Vector2(i * 16 + 8, j * 16 + 8),
		};
		Ins.VFXManager.Add(bc);
	}
}
