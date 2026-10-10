using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class WoodlandWraithStaff_SporeZone : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public const float RangeMax = 300;

	public float Range = 0;

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.friendly = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.timeLeft = 900;
	}

	public override void AI()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.owner == Projectile.owner && proj.type == Type && proj != Projectile)
			{
				if (proj.timeLeft < Projectile.timeLeft && proj.timeLeft > 60)
				{
					proj.timeLeft = 60;
				}
			}
		}
		if (Range < RangeMax)
		{
			Range = MathHelper.Lerp(Range, RangeMax + 5, 0.1f);
		}
		else
		{
			Range = RangeMax;
		}
		for (int t = 0; t < Range / 150; t++)
		{
			Vector2 posAdd = new Vector2(0, Range).RotatedByRandom(MathHelper.TwoPi);
			if (Main.rand.NextBool(3))
			{
				Vector2 vel = posAdd.NormalizeSafe() * 0.3f;
				if (Main.rand.NextBool())
				{
					vel *= -1;
				}
				var dustVFX4 = new SporeRingDust
				{
					Velocity = vel,
					Active = true,
					Visible = true,
					Position = posAdd + Projectile.Center,
					MaxTime = 80,
					Scale = Main.rand.NextFloat(16, 24),
					Rotation = vel.ToRotation() - MathHelper.PiOver4 * 3,
					ai = new float[] { 0, 0, 0 },
				};
				Ins.VFXManager.Add(dustVFX4);
			}

			Dust dust = Dust.NewDustDirect(Projectile.Center + posAdd - new Vector2(4), 0, 0, ModContent.DustType<WoodlandWraithStaff_Spore2>());
			dust.velocity = posAdd.NormalizeSafe() * 0.3f;
			if (Main.rand.NextBool())
			{
				dust.velocity *= -1;
			}
			dust.scale = 0.6f;
		}
		Lighting.AddLight(Projectile.Center, new Vector3(220, 220, 239) / 300f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float timeValue = (float)(Main.time * 0.0002f);
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> barsCenter = new List<Vertex2D>();
		var drawColor = new Color(220, 220, 239, 0);

		for (int i = 0; i <= 100; i++)
		{
			var colorTile = Lighting.GetColor((drawPos + new Vector2(Range + 20, 0).RotatedBy(i / 100f * MathHelper.TwoPi) + Main.screenPosition).ToTileCoordinates());
			var ringColor = Color.Lerp(drawColor, colorTile, 0.95f);
			if (Projectile.timeLeft < 60)
			{
				ringColor *= Projectile.timeLeft / 60f;
			}
			ringColor.A = 0;
			bars.Add(drawPos + new Vector2(Range + 20, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor * 0, new Vector3(i / 100f * 4f, timeValue, 0));
			bars.Add(drawPos + new Vector2(Range, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor, new Vector3(i / 100f * 4f, 0.05f + timeValue, 0));
		}
		for (int i = 0; i <= 100; i++)
		{
			var colorTile = Lighting.GetColor((drawPos + new Vector2(Range, 0).RotatedBy(i / 100f * MathHelper.TwoPi) + Main.screenPosition).ToTileCoordinates());
			var ringColor = Color.Lerp(drawColor, colorTile, 0.95f);
			if (Projectile.timeLeft < 60)
			{
				ringColor *= Projectile.timeLeft / 60f;
			}
			ringColor.A = 0;
			bars.Add(drawPos + new Vector2(Range, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor, new Vector3(i / 100f * 4f, 0.05f + timeValue, 0));
			bars.Add(drawPos + new Vector2(Range - 20, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor * 0, new Vector3(i / 100f * 4f, timeValue, 0));
		}

		for (int i = 0; i <= 100; i++)
		{
			var colorTile = Lighting.GetColor((drawPos + new Vector2(Range + 10, 0).RotatedBy(i / 100f * MathHelper.TwoPi) + Main.screenPosition).ToTileCoordinates());
			var ringColor = Color.Lerp(drawColor, colorTile, 0.8f);
			if (Projectile.timeLeft < 60)
			{
				ringColor *= Projectile.timeLeft / 60f;
			}
			ringColor.A = 0;
			barsCenter.Add(drawPos + new Vector2(Range + 10, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor, new Vector3(i / 100f * 4f, 0, 0));
			barsCenter.Add(drawPos + new Vector2(Range - 10, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor, new Vector3(i / 100f * 4f, 1, 0));
		}
		if (bars.Count > 0)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_forceField_medium.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_17.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsCenter.ToArray(), 0, barsCenter.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		Texture2D bloom = Commons.ModAsset.SwirlPoint.Value;
		var bloomColor = drawColor * 0.6f;
		bloomColor.A = 0;
		if (Projectile.timeLeft < 60)
		{
			bloomColor *= Projectile.timeLeft / 60f;
		}
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, bloomColor, timeValue * 90 + Projectile.whoAmI, bloom.Size() * 0.5f, Range / 600f + 0.5f, SpriteEffects.None);
		return false;
	}
}
