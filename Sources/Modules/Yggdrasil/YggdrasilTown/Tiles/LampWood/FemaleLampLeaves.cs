using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class FemaleLampLeaves : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<LampWood_Dust>();
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(71, 71, 145));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}

	public void AddScene(int i, int j)
	{
		float mainRot = 0.4f + Main.rand.NextFloat(-0.3f, 0.3f) + YggdrasilWorldGeneration.TerrianSurfaceNormal(i, j).ToRotation() - MathHelper.PiOver2;
		for (int x = 1; x < 3; x++)
		{
			FemaleLampLeaves_leaf_fore leaf = new FemaleLampLeaves_leaf_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			leaf.rotation = -(x + 0.3f) * 0.7f - Main.rand.NextFloat(.3f) + mainRot;
			leaf.startRotation = leaf.rotation;
			leaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			leaf.Style = x;
			leaf.Flip_H = true;
			Ins.VFXManager.Add(leaf);
		}
		for (int x = 1; x < 3; x++)
		{
			FemaleLampLeaves_leaf_fore leaf = new FemaleLampLeaves_leaf_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			leaf.rotation = (x + 0.3f) * 0.7f + Main.rand.NextFloat(.3f) + mainRot;
			leaf.startRotation = leaf.rotation;
			leaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			leaf.Style = x;
			leaf.Flip_H = false;
			Ins.VFXManager.Add(leaf);
		}
		for (int x = -1; x < 2; x++)
		{
			if (x == 0)
			{
				continue;
			}
			FemaleLampLeaves_leaf_fore leaf = new FemaleLampLeaves_leaf_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			leaf.rotation = (x + 0.3f) * 0.2f + Main.rand.NextFloat(.3f) + mainRot;
			leaf.startRotation = leaf.rotation;
			leaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			leaf.Style = Main.rand.Next(6);
			leaf.Flip_H = i > 0;
			Ins.VFXManager.Add(leaf);
		}
		if (Main.rand.NextBool(3))
		{
			FemaleLampLeaves_leaf_fore topLeaf = new FemaleLampLeaves_leaf_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			topLeaf.rotation = mainRot + MathHelper.Pi;
			topLeaf.startRotation = topLeaf.rotation;
			topLeaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			topLeaf.Style = Main.rand.Next(6) + 6;
			topLeaf.Flip_H = Main.rand.NextBool(2);
			Ins.VFXManager.Add(topLeaf);
		}
		else
		{
			FemaleLampLeaves_leaf_fore topLeaf = new FemaleLampLeaves_leaf_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			topLeaf.rotation = mainRot + MathHelper.Pi;
			topLeaf.startRotation = topLeaf.rotation;
			topLeaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			topLeaf.Style = Main.rand.Next(12, 14);
			topLeaf.Flip_H = Main.rand.NextBool(2);
			Ins.VFXManager.Add(topLeaf);
		}
		for (int x = 0; x < 3; x++)
		{
			FemaleLampLeaves_leaf leaf = new FemaleLampLeaves_leaf { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			leaf.rotation = x * 0.6f + Main.rand.NextFloat(.3f) + mainRot;
			leaf.startRotation = leaf.rotation;
			leaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			leaf.Style = x;
			leaf.Flip_H = false;
			Ins.VFXManager.Add(leaf);
		}
		for (int x = 0; x < 3; x++)
		{
			FemaleLampLeaves_leaf leaf = new FemaleLampLeaves_leaf { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			leaf.rotation = -x * 0.6f + Main.rand.NextFloat(.3f) + mainRot;
			leaf.startRotation = leaf.rotation;
			leaf.scale = Main.rand.NextFloat(0.75f, 1.25f);
			leaf.Style = x;
			leaf.Flip_H = true;
			Ins.VFXManager.Add(leaf);
		}
	}
}
