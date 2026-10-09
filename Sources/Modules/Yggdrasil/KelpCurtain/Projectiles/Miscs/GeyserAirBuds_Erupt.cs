using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.ElementalDebuff;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using log4net.Core;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Miscs;

public class GeyserAirBuds_Erupt : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 60;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 53; i++)
		{
			var vel = new Vector2(0, -Main.rand.NextFloat(3.6f, 26.4f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 45.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			if (Main.rand.NextBool(3))
			{
				vel.X -= pos.X * 0.05f;
			}
			var dust = new AvariceSuccessDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center + pos,
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(1f, 2f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < 55; i++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.X *= 0.3f;
			newVelocity.Y -= 8f;
			newVelocity.Y *= 3f;
			var somg = new GeyserAirBudsSmog
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-22f, 22f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(30, 60),
				Scale = Main.rand.NextFloat(50f, 65f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void AI() => base.AI();

	public override bool PreDraw(ref Color lightColor)
	{
		// float timeValue = Projectile.timeLeft / 60f;
		// var drawColor = new Color(220, 20, 239, 0);
		// float range = (1 - timeValue) * 150;
		// var drawPos = Projectile.Center - Main.screenPosition;
		// List<Vertex2D> bars = new List<Vertex2D>();
		// for (int i = 0; i <= 100; i++)
		// {
		// var ringColor = drawColor;
		// bars.Add(drawPos + new Vector2(range + timeValue * 20f, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor * 0, new Vector3(i / 100f * 4f, timeValue, 0));
		// bars.Add(drawPos + new Vector2(range, 0).RotatedBy(i / 100f * MathHelper.TwoPi), ringColor, new Vector3(i / 100f * 4f, 0.05f + timeValue, 0));
		// }
		// if (bars.Count > 0)
		// {
		// SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		// Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_16.Value;
		// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		// Main.spriteBatch.End();
		// Main.spriteBatch.Begin(sBS);
		// }
		return false;
	}
}
