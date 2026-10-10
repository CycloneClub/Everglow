using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Slash_Target : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 3;

		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public override void AI()
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		float dissolveDuration = Projectile.timeLeft / 60f - 0.2f;
		if (Projectile.timeLeft > 60)
		{
			dissolveDuration = 1.2f;
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(lightColor.ToVector4());
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.2f, 0f, 0.9f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(60f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		var drawColor = new Color(0.3f, 0.05f, 0.9f, 0);
		Vector2 drawPos = Projectile.Center;
		float timeLeftValue = (120 - Projectile.timeLeft) / 120f;
		float addWidth = MathF.Pow(timeLeftValue, 2.5f) * 10;

		for (int i = 0; i < 2; i++)
		{
			float rotation = Math.Clamp(timeLeftValue * 3, 0, 1) * MathHelper.PiOver4;
			if (i == 1)
			{
				rotation *= -1;
			}
			var bars = new List<Vertex2D>();
			bars.Add(drawPos + new Vector2(-44, 0).RotatedBy(rotation), drawColor, new Vector3(0, 0.2f, 0));
			bars.Add(drawPos + new Vector2(-44, 0).RotatedBy(rotation), drawColor, new Vector3(0, 0.5f, 0));

			bars.Add(drawPos + new Vector2(0, -10 - addWidth).RotatedBy(rotation), drawColor, new Vector3(0.5f, 0.2f, 0));
			bars.Add(drawPos + new Vector2(0, -addWidth).RotatedBy(rotation), drawColor, new Vector3(0.5f, 0.5f, 0));

			bars.Add(drawPos + new Vector2(44, 0).RotatedBy(rotation), drawColor, new Vector3(1, 0.2f, 0));
			bars.Add(drawPos + new Vector2(44, 0).RotatedBy(rotation), drawColor, new Vector3(1, 0.5f, 0));

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			bars = new List<Vertex2D>();
			bars.Add(drawPos + new Vector2(-44, 0).RotatedBy(rotation), drawColor, new Vector3(0, 0.2f, 0));
			bars.Add(drawPos + new Vector2(-44, 0).RotatedBy(rotation), drawColor, new Vector3(0, 0.5f, 0));

			bars.Add(drawPos + new Vector2(0, 10 + addWidth).RotatedBy(rotation), drawColor, new Vector3(0.5f, 0.2f, 0));
			bars.Add(drawPos + new Vector2(0, addWidth).RotatedBy(rotation), drawColor, new Vector3(0.5f, 0.5f, 0));

			bars.Add(drawPos + new Vector2(44, 0).RotatedBy(rotation), drawColor, new Vector3(1, 0.2f, 0));
			bars.Add(drawPos + new Vector2(44, 0).RotatedBy(rotation), drawColor, new Vector3(1, 0.5f, 0));

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_7.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
