using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class CorruptOrbStonePost : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 9;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.Origin = new Point16(0, 8);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(87, 84, 96));
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.CorruptOrbStonePost_glow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(1f, 1f, 1f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			float energyValue = 1f;
			Vector2 powerCenter = new Vector2(i, j) * 16;
			Color c0 = new Color(0.4f, 0.3f, 0.7f, 0);
			float timeValue = (float)Main.time * 0.002f;
			List<Vertex2D> bars = new List<Vertex2D>();
			float accuracy = 16;
			List<Vertex2D> bars2 = new List<Vertex2D>();
			for (int x = 0; x < 9; x++)
			{
				Vector2 addPos = new Vector2(22, 16);
				Vector2 addVel = new Vector2(0, 4 * energyValue).RotatedBy(x / 9f * MathHelper.TwoPi - timeValue * 0.4f);
				for (int t = 0; t <= accuracy; t++)
				{
					float factor = t / accuracy;
					Vector2 velLeft = Vector2.Normalize(addVel.RotatedBy(MathHelper.PiOver2)) * 134 * energyValue;
					if (t == 0)
					{
						bars2.Add(new Vertex2D(powerCenter + addPos, Color.White * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
						bars2.Add(new Vertex2D(powerCenter + addPos - velLeft, Color.White * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
					}
					bars2.Add(new Vertex2D(powerCenter + addPos, Color.White * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
					bars2.Add(new Vertex2D(powerCenter + addPos - velLeft, Color.White * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));

					if (t == 0)
					{
						bars.Add(new Vertex2D(powerCenter + addPos, c0 * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
						bars.Add(new Vertex2D(powerCenter + addPos - velLeft, c0 * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
					}
					bars.Add(new Vertex2D(powerCenter + addPos, c0 * (1 - factor) * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
					bars.Add(new Vertex2D(powerCenter + addPos - velLeft, c0 * (1 - factor) * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
					addPos += addVel;
					addVel = addVel.RotatedBy((1 - factor * factor * 1.2f) * 0.36f) * 1.08f;
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
			Effect effect = Commons.ModAsset.Trailing.Value;
			var projection = Matrix.CreateOrthographicOffCenter(-Main.offScreenRange, Main.screenWidth + Main.offScreenRange, Main.screenHeight + Main.offScreenRange, -Main.offScreenRange, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.EffectMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			if (bars2.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			}

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

		}
		base.PostDraw(i, j, spriteBatch);
	}
	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (closer && !NPC.downedBoss2)
		{
			var tile = Main.tile[i, j];
			if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
			{
				Vector2 positionWorld = new Vector2(i, j) * 16f + new Vector2(6);
				Player player = Main.LocalPlayer;
				if ((positionWorld - player.MountedCenter).Length() < 450)
				{
					if (!Main.gamePaused)
					{
						foreach (Projectile projectile in Main.projectile)
						{
							if (projectile != null && projectile.active)
							{
								if (projectile.type == ModContent.ProjectileType<PylonStonePostProj_corrupt>())
								{
									if (projectile.ai[1] == i && projectile.ai[2] == j)
									{
										return;
									}
								}
							}
						}
						Vector2 toPlayer = Vector2.Normalize(player.MountedCenter - positionWorld) * 1f;
						Projectile.NewProjectile(player.GetSource_FromAI(), positionWorld, toPlayer, ModContent.ProjectileType<PylonStonePostProj_corrupt>(), 50, 5, player.whoAmI, 0, i, j);
					}
				}
			}
		}
		base.NearbyEffects(i, j, closer);
	}
	public override bool CanExplode(int i, int j)
	{
		return false;
	}
	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}
}
