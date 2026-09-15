using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class RedAlgaeMagicSpellBook_proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public float Range = 0;

	public float MaxRange = 320;

	public bool Released = false;

	public List<Point> SurfaceTiles = new List<Point>();

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 36000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.hide = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 120;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 toMouse = Main.MouseWorld - Projectile.Center;
		if (!Released)
		{
			Vector2 toPlayer = player.Center + toMouse.NormalizeSafe() * 30f - Projectile.Center;
			if (toPlayer.Length() > 50f)
			{
				Projectile.velocity = toPlayer.NormalizeSafe() * 10f;
			}
			else
			{
				Projectile.velocity = toPlayer / 5f;
			}
			if (player.controlUseItem)
			{
				float maxFlucRange = MaxRange + 20 * MathF.Sin((float)Main.time * 0.08f);
				if (Range < maxFlucRange)
				{
					Range = Range * 0.99f + maxFlucRange * 0.01f;
				}
				else
				{
					Projectile.velocity = toMouse.NormalizeSafe() * 12f;
					Released = true;
				}
			}
			if ((Projectile.Center - player.Center).Length() > 1000 || player.HeldItem.type != ModContent.ItemType<RedAlgaeMagicSpellBook>())
			{
				Released = true;
			}
		}
		else
		{
			Projectile.velocity *= 0.995f;
			Range -= 0.45f;
			if (Range <= 0)
			{
				Projectile.Kill();
			}
		}

		int rangeInt = (int)Range / 16 + 1;
		Point centerPoint = Projectile.Center.ToTileCoordinates();
		SurfaceTiles = new List<Point>();
		for (int x = -rangeInt; x <= rangeInt; x++)
		{
			for (int y = -rangeInt; y <= rangeInt; y++)
			{
				if (new Vector2(x, y).Length() < rangeInt)
				{
					Vector2 checkPos = centerPoint.ToWorldCoordinates() + new Vector2(x, y) * 16;
					if (Collision.IsWorldPointSolid(checkPos))
					{
						for (int r = 0; r < 4; r++)
						{
							if (!Collision.IsWorldPointSolid(checkPos + new Vector2(0, 16).RotatedBy(r * MathHelper.PiOver2)))
							{
								SurfaceTiles.Add(new Point(x, y) + centerPoint);
								break;
							}
						}
					}
				}
			}
		}
		if (Main.rand.NextBool(2))
		{
			Vector2 pos = new Vector2(0, Range * 1.05f).RotatedByRandom(MathHelper.TwoPi);
			var redAlgaeDust = new RedAlgaeDust();
			redAlgaeDust.Position = Projectile.Center + pos;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = pos.NormalizeSafe().RotatedBy(-MathHelper.PiOver2) * 2f;
			redAlgaeDust.ai = new float[] { -0.05f };
			redAlgaeDust.MaxScale = Main.rand.NextFloat(0.47f, 0.6f);
			redAlgaeDust.MaxTime = 70;
			redAlgaeDust.Frame = Main.rand.Next(4);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}

		for (int i = 0; i <= Range; i += 40)
		{
			var redAlgaeDust = new RedAlgae_Small_Dust_SpinAroundEntity();
			redAlgaeDust.ParentEntity = Projectile;
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = Vector2.zeroVector;
			redAlgaeDust.ai = new float[] { Main.rand.NextFloat(Range / 8f, Range / 2f), Main.rand.NextFloat(MathHelper.TwoPi) };
			redAlgaeDust.MaxTime = 30;
			redAlgaeDust.MaxScale = Main.rand.NextFloat(2f);
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		if (Main.rand.NextBool(2))
		{
			var redAlgaeDust = new RedAlgae_Spark_SpinAroundEntity();
			redAlgaeDust.ParentEntity = Projectile;
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = Vector2.zeroVector;
			redAlgaeDust.ai = new float[] { Main.rand.NextFloat(Range * 0.07f, Range * 0.27f), Main.rand.NextFloat(MathHelper.TwoPi) };
			redAlgaeDust.MaxTime = 30;
			redAlgaeDust.MaxScale = Main.rand.NextFloat(3f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (var pos in SurfaceTiles)
		{
			for (int r = 0; r < 4; r++)
			{
				Vector2 checkPos = pos.ToWorldCoordinates() + new Vector2(16, 0).RotatedBy(r * MathHelper.PiOver2);
				if (!Collision.IsWorldPointSolid(checkPos))
				{
					checkPos += new Vector2(12, 0).RotatedBy(r * MathHelper.PiOver2) - new Vector2(12);
					Rectangle rectangle = new Rectangle((int)checkPos.X, (int)checkPos.Y, 24, 24);
					if (rectangle.Intersects(targetHitbox))
					{
						return true;
					}
				}
			}
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (!target.HasBuff(type))
		{
			target.AddBuff(type, 900);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float fade = 1f;
		if (Released)
		{
			if (Range < 50)
			{
				fade = Range / 50f;
			}
		}
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Texture2D spot_dark = Commons.ModAsset.LightPoint2_black.Value;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(spot_dark, drawPos, null, Color.White, 0, spot_dark.Size() * 0.5f, 1f * fade, SpriteEffects.None);
		Main.EntitySpriteDraw(spot, drawPos, null, new Color(1f, 0.9f, 0.8f, 0), 0, spot.Size() * 0.5f, 1f * fade, SpriteEffects.None);
		float flucTime = MathF.Sin(Main.GlobalTimeWrappedHourly * 2.5f) * 0.5f + 1f;
		Main.EntitySpriteDraw(star, drawPos, null, new Color(1f, 0.9f, 0.8f, 0) * 0.25f, 0, star.Size() * 0.5f, new Vector2(1f, 0.5f) * fade, SpriteEffects.None);
		Main.EntitySpriteDraw(star, drawPos, null, new Color(1f, 0.9f, 0.8f, 0) * 0.25f, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(1f, 1f * flucTime) * fade, SpriteEffects.None);
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.9f, 0.8f));

		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_dark = new List<Vertex2D>();
		float rate = 6f;
		float timeValue = Main.GlobalTimeWrappedHourly * 0.5f;
		for (int h = 0; h <= 60; h++)
		{
			float value = h / 60f;
			float coordX = value * rate + timeValue;
			Vector2 outerRing = new Vector2(Range + 70, 0).RotatedBy(value * MathHelper.TwoPi);
			Vector2 innerRing = new Vector2(Range, 0).RotatedBy(value * MathHelper.TwoPi);
			if (h > 0)
			{
				Lighting.AddLight(drawPos + Main.screenPosition + (innerRing + outerRing) * 0.5f + Projectile.velocity * 3, new Vector3(0.4f, 0.05f, 0.3f) * 2);
			}
			bars_dark.Add(drawPos + outerRing, Color.White * fade, new Vector3(coordX, 0, 0));
			bars_dark.Add(drawPos + innerRing, Color.White * fade, new Vector3(coordX, 1, 0));

			bars.Add(drawPos + outerRing, new Color(1f, 0.2f, 0.4f, 0) * fade, new Vector3(coordX, 0, 0));
			bars.Add(drawPos + innerRing, new Color(0.7f, 0.1f, 0.3f, 0) * fade, new Vector3(coordX, 1, 0));
		}

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;

		if (bars_dark.Count > 0 && bars.Count > 0)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_16_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_16.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}

		List<Vertex2D> algae = new List<Vertex2D>();
		foreach (var pos in SurfaceTiles)
		{
			for (int r = 0; r < 4; r++)
			{
				if (!Collision.IsWorldPointSolid(pos.ToWorldCoordinates() + new Vector2(16, 0).RotatedBy(r * MathHelper.PiOver2)))
				{
					AddDrawPiece(algae, pos, r * MathHelper.PiOver2);
				}
			}
		}

		if (algae.Count > 0)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			Effect algaeEffect = ModAsset.RedAlgaeMagicSpellBook_proj_algaeEffect.Value;
			algaeEffect.Parameters["uTransform"].SetValue(model * projection);
			algaeEffect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.RedAlgae_EffectTexture.Value;
			Main.graphics.GraphicsDevice.Textures[1] = ModAsset.RedAlgaeMagicSpellBook_proj_heatmap.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, algae.ToArray(), 0, algae.Count / 3);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void AddDrawPiece(List<Vertex2D> drawList, Point tilePos, float rotation)
	{
		Vector2 platformOffset = Vector2.zeroVector;
		var tile = TileUtils.SafeGetTile(tilePos);
		float frameCount = 8;
		int style = TileUtils.GetFixedRandomNumber(tilePos.X, tilePos.Y + (int)(rotation * 12), (int)frameCount);
		if (MathF.Abs(rotation - MathHelper.PiOver2) < 0.01f)
		{
			if (tile.HasTile && TileID.Sets.Platforms[tile.TileType])
			{
				platformOffset = new Vector2(0, -8);
			}
		}
		if (MathF.Abs(rotation - MathHelper.PiOver2 * 3) < 0.01f)
		{
			if (tile.HasTile && tile.IsHalfBlock)
			{
				platformOffset = new Vector2(0, 8);
			}
		}
		if (!tile.HasTile)
		{
			style = 1;
		}
		Vector2 drawPos = tilePos.ToWorldCoordinates() + new Vector2(8, 0).RotatedBy(rotation) + platformOffset;
		Vector2 pos0 = drawPos + new Vector2(0, -24).RotatedBy(rotation);
		Vector2 pos1 = drawPos + new Vector2(54, -24).RotatedBy(rotation);
		Vector2 pos2 = drawPos + new Vector2(54, 24).RotatedBy(rotation);
		Vector2 pos3 = drawPos + new Vector2(0, 24).RotatedBy(rotation);
		drawList.Add(pos0, Lighting.GetColor(pos0.ToTileCoordinates()), new Vector3(style / frameCount, 1, GetAlgaeValueDistance(pos0)));
		drawList.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(style / frameCount, 0, GetAlgaeValueDistance(pos1)));
		drawList.Add(pos3, Lighting.GetColor(pos3.ToTileCoordinates()), new Vector3((style + 1f) / frameCount, 1, GetAlgaeValueDistance(pos3)));

		drawList.Add(pos3, Lighting.GetColor(pos3.ToTileCoordinates()), new Vector3((style + 1f) / frameCount, 1, GetAlgaeValueDistance(pos3)));
		drawList.Add(pos1, Lighting.GetColor(pos1.ToTileCoordinates()), new Vector3(style / frameCount, 0, GetAlgaeValueDistance(pos1)));
		drawList.Add(pos2, Lighting.GetColor(pos2.ToTileCoordinates()), new Vector3((style + 1f) / frameCount, 0, GetAlgaeValueDistance(pos2)));

		Vector2 center = (pos0 + pos3) * 0.5f;
		Lighting.AddLight(center, new Vector3(0.7f, 0.1f, 0.6f) * GetAlgaeValueDistance(center));
		if (!Main.gamePaused && Main.rand.NextBool(240))
		{
			Vector2 pos = new Vector2(0, 20).RotatedByRandom(MathHelper.TwoPi);
			var redAlgaeDust = new RedAlgae_Small_Dust();
			redAlgaeDust.Position = drawPos;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(1, 0).RotatedBy(rotation + Main.rand.NextFloat(-1f, 1f));
			redAlgaeDust.ai = new float[] { 0.99f };
			redAlgaeDust.MaxTime = 240;
			redAlgaeDust.Scale = 0.5f;
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
	}

	public float GetAlgaeValueDistance(Vector2 pos)
	{
		return Math.Min(1, ((Range * 1.02f - (pos - Projectile.Center).Length()) / MaxRange) * 5);
	}

	public override void OnKill(int timeLeft)
	{
		for (int k = 0; k < 20; k++)
		{
			var redAlgaeDust = new RedAlgae_Small_Dust();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(12f)).RotatedByRandom(MathHelper.TwoPi);
			redAlgaeDust.ai = new float[] { 0.99f };
			redAlgaeDust.MaxTime = 60;
			redAlgaeDust.Scale = Main.rand.NextFloat(1f, 4f);
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		for (int k = 0; k < 20; k++)
		{
			var redAlgaeDust = new RedAlgae_Spark();
			redAlgaeDust.Position = Projectile.Center;
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(12f)).RotatedByRandom(MathHelper.TwoPi);
			redAlgaeDust.ai = new float[] { 0.99f };
			redAlgaeDust.MaxTime = 60;
			redAlgaeDust.Scale = Main.rand.NextFloat(1f, 5f);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		base.OnKill(timeLeft);
	}
}
