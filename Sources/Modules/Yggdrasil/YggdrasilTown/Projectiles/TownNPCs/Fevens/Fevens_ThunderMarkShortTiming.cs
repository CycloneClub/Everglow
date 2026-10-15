using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_ThunderMarkShortTiming : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => ModAsset.Fevens_ThunderMark_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 110;
		Projectile.tileCollide = false;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (Main.rand.NextBool())
		{
			Vector2 vel = new Vector2(0, -Main.rand.NextFloat(5.6f, 8.4f)).RotatedByRandom(0.1f);
			var dust = new Fevens_LightingBoltDust
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(30, 50),
				Scale = Main.rand.NextFloat(3f, 5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float mulColor2 = 1f;
		if (Projectile.timeLeft > 120)
		{
			mulColor2 = (150 - Projectile.timeLeft) / 30f;
		}
		var drawColor = new Color(1f, 0f, 0f, 0f);
		float timeValue = (float)Main.time * 0.03f;
		var powerCircleLarge = new List<Vertex2D>();
		var powerCircleLarge_Dark = new List<Vertex2D>();
		for (int i = 0; i <= 50; i++)
		{
			Vector2 radius = new Vector2(Projectile.timeLeft + 60).RotatedBy(i / 50f * MathHelper.TwoPi) * 0.5f;
			Vector2 radius2 = new Vector2(Projectile.timeLeft - 30).RotatedBy(i / 50f * MathHelper.TwoPi) * 0.5f;
			powerCircleLarge.Add(Projectile.Center + radius - Main.screenPosition, drawColor * mulColor2 * 0.5f, new Vector3(i / 25f - timeValue * 0.25f + Projectile.whoAmI, 0, 0));
			powerCircleLarge.Add(Projectile.Center + radius2 - Main.screenPosition, drawColor * mulColor2 * 0.7f, new Vector3(i / 25f - timeValue * 0.25f + Projectile.whoAmI, 1, 0));

			powerCircleLarge_Dark.Add(Projectile.Center + radius - Main.screenPosition, Color.White * mulColor2 * 0.5f, new Vector3(i / 25f - timeValue * 0.25f + Projectile.whoAmI, 0, 0));
			powerCircleLarge_Dark.Add(Projectile.Center + radius2 - Main.screenPosition, Color.White * mulColor2 * 0.7f, new Vector3(i / 25f - timeValue * 0.25f + Projectile.whoAmI, 1, 0));
		}
		if (powerCircleLarge.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerCircleLarge_Dark.ToArray(), 0, powerCircleLarge_Dark.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerCircleLarge.ToArray(), 0, powerCircleLarge.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f, 0f, 0.1f, 0f), 0, light.Size() / 2f, new Vector2(0.7f + MathF.Sin(timeValue * 6 + Projectile.whoAmI) * 0.6f, 1f) * 1.05f, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_LightingBolt>(), Projectile.damage, 2);
		base.OnKill(timeLeft);
	}
}
