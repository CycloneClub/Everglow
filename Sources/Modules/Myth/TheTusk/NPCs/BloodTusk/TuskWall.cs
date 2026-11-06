using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.DataStructures;
using Everglow.Myth.TheTusk.Projectiles;

namespace Everglow.Myth.TheTusk.NPCs.BloodTusk;

public class TuskWall : BoxEntity
{
	public bool Flip;
	public int Timer = 3;
	public float BottomY = -1;
	public NPC Tusk;
	public bool StartShake = false;
	public bool EndShake = false;

	public override void SetDefaults()
	{
		Size = new Vector2(194, 16);
		Timer = 3;
	}

	public override void AI()
	{
		if (BottomY == -1)
		{
			BottomY = Position.Y + Size.Y;
			for (int t = 0; t < 300; t++)
			{
				if (!Terraria.Collision.SolidCollision(new Vector2(Position.X - 5, BottomY + 5), 10, 10))
				{
					BottomY += 4;
				}
				else
				{
					break;
				}
			}
			BottomY += 15;
		}
		Size = new(Size.X, Math.Clamp(Timer * 3, 16, 840));
		Position = new(Position.X, BottomY - Size.Y);
		if (Tusk != null && Tusk.active)
		{
			BloodTusk bloodTusk = Tusk.ModNPC as BloodTusk;
			if (!Tusk.active)
			{
				Kill();
			}
			if (Flip)
			{
				bloodTusk.TuskWallClampRange.X = Position.X + Size.X + 100;
			}
			else
			{
				bloodTusk.TuskWallClampRange.Y = Position.X - 100;
			}
			Timer++;
			if (!StartShake)
			{
				Point point = (Position + new Vector2(Size.X * 0.5f, Size.Y)).ToTileCoordinates();
				Projectile.NewProjectile(WorldGen.GetProjectileSource_TileBreak(point.X, point.Y), Position + new Vector2(Size.X * 0.5f, Size.Y), Vector2.zeroVector, ModContent.ProjectileType<TuskWall_Wave>(), 0, 0, Owner: Main.maxPlayers);
				ShakerManager.AddShaker(Position + new Vector2(Size.X * 0.5f, Size.Y), new Vector2(0, 1), 6, 30, 120, 0.999f, 0.999f, 280);
				StartShake = true;
			}
			if (Timer == 280)
			{
				ShakerManager.AddShaker(Position + new Vector2(Size.X * 0.5f, Size.Y), new Vector2(0, 1), 120, 30, 120, 0.9f, 0.9f, 120);
			}
		}
		else
		{
			if (Timer > 0)
			{
				if (Timer > 280)
				{
					Timer = 280;
				}
				Timer--;
				if (!EndShake)
				{
					Point point = (Position + new Vector2(Size.X * 0.5f, Size.Y)).ToTileCoordinates();
					Projectile.NewProjectile(WorldGen.GetProjectileSource_TileBreak(point.X, point.Y), Position + new Vector2(Size.X * 0.5f, Size.Y), Vector2.zeroVector, ModContent.ProjectileType<TuskWall_Wave>(), 0, 0, Owner: Main.maxPlayers);
					ShakerManager.AddShaker(Position + new Vector2(Size.X * 0.5f, Size.Y), new Vector2(0, 1), 60, 30, 120, 0.9f, 0.9f, 120);
					ShakerManager.AddShaker(Position + new Vector2(Size.X * 0.5f, Size.Y), new Vector2(0, 1), 6, 30, 120, 0.999f, 0.999f, Timer);
					EndShake = true;
				}
			}
			else
			{
				Timer = 0;
				Kill();
			}
		}
		base.AI();
	}

	public override Color MapColor => new Color(153, 113, 90);

	public override void Draw()
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect effect = Commons.ModAsset.Shader2D.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Texture2D tuskWall = ModAsset.TuskWall.Value;

		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				Vector2 drawPos = new Vector2(i * 53, j * 44);
				Vector2 pos0 = Position + drawPos;
				Vector2 pos1 = Position + drawPos + new Vector2(53, 0);

				Vector2 pos2 = Position + drawPos + new Vector2(0, 44);
				Vector2 pos3 = Position + drawPos + new Vector2(53, 44);
				bool shouldBreak = false;
				if (drawPos.Y + 44 > Size.Y)
				{
					pos2 = Position + new Vector2(drawPos.X, Size.Y);
					pos3 = Position + new Vector2(drawPos.X + 53, Size.Y);
					shouldBreak = true;
				}
				AddVertex(bars, pos0);
				AddVertex(bars, pos1);
				AddVertex(bars, pos2);

				AddVertex(bars, pos2);
				AddVertex(bars, pos1);
				AddVertex(bars, pos3);
				if (shouldBreak)
				{
					break;
				}
			}
		}
		Main.graphics.graphicsDevice.Textures[0] = tuskWall;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos)
	{
		Texture2D tuskWall = ModAsset.TuskWall.Value;
		Vector3 coord = new Vector3((pos - Position) / tuskWall.Size(), 0);
		if (!Flip)
		{
			coord.X = 1 - coord.X;
			pos += new Vector2(0, 0);
		}
		pos -= new Vector2(2 * 53, 0);
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()), coord);
	}
}
