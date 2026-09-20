using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class KelpMoss_large_tile : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<LampWood_Dust>();
		HitSound = SoundID.Grass;

		AddMapEntry(new Color(29, 63, 46));
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
	public void AddScene(int i, int j)
	{
		KelpMoss_large_fore kelp = new KelpMoss_large_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<KelpMoss_large_tile>() };
		kelp.startRotation = GetNeighborTileRotation(new Vector2(i * 16, j * 16));
		for (int x = 0; x < Main.rand.Next(4, 125); x++)
		{
			kelp.wigglerRotations.Add(kelp.startRotation * MathF.Pow(0.7f, x));
		}
		kelp.scale = Main.rand.NextFloat(45f, 80f);
		kelp.Position -= new Vector2(0, 12).RotatedBy(kelp.startRotation) - new Vector2(8);
		Ins.VFXManager.Add(kelp);
	}
	private static float GetNeighborTileRotation(Vector2 worldCoord)
	{
		Vector2 totalVector = Vector2.Zero;//合向量
		int tileCount = 0;
		for (int a = 0; a < 12; a++)
		{
			Vector2 v0 = new Vector2(10, 0).RotatedBy(a / 6d * Math.PI);
			if (Collision.SolidCollision(worldCoord + v0, 1, 1))
			{
				totalVector -= v0;
				tileCount++;
			}
			else
			{
				totalVector += v0;
			}
		}
		for (int a = 0; a < 24; a++)
		{
			Vector2 v0 = new Vector2(20, 0).RotatedBy(a / 12d * Math.PI);
			if (Collision.SolidCollision(worldCoord + v0, 1, 1))
			{
				totalVector -= v0 * 0.5f;
				tileCount++;
			}
			else
			{
				totalVector += v0 * 0.5f;
			}
		}
		if (totalVector == Vector2.Zero || tileCount > 30)
			return 0;

		return MathF.Asin(Vector3.Cross(new Vector3(0, 1, 0), new Vector3(Vector2.Normalize(totalVector), 0)).Z);
	}
}
