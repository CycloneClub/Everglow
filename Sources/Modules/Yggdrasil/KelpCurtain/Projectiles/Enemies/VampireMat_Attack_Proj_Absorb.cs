using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Absorb : ModProjectile
{
	public int TargetPlayerIndex => (int)Projectile.ai[0];

	public int ParentNPCIndex => (int)Projectile.ai[1];

	public int Timer = 0;

	public float Fade = 1f;

	public float rotationValue = 0;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 600;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1024;
		base.SetDefaults();
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Timer++;
		Fade = 1f;
		if (Timer < 60f)
		{
			Fade *= Timer / 60f;
		}
		if (Projectile.timeLeft < 60f)
		{
			Fade *= Projectile.timeLeft / 60f;
		}
		rotationValue += 0.004f * Fade;
		Projectile.velocity *= 0;
		int maxDistance = 800;
		foreach (var player in Main.player)
		{
			if (player is not null && player.active && !player.dead)
			{
				Vector2 toCenter = Projectile.Center - player.Center;
				if (toCenter.Length() < maxDistance)
				{
					float force = (maxDistance - toCenter.Length()) * 0.01f * Fade;
					Vector2 move = toCenter.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * force;
					player.position += move;
				}
			}
		}
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<SpongeOxygenBubble>())
				{
					Vector2 toCenter = Projectile.Center - proj.Center;
					if (toCenter.Length() < maxDistance)
					{
						float force = (maxDistance - toCenter.Length()) * 0.01f * Fade;
						Vector2 move = toCenter.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * force;
						proj.position += move;
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		VampireMat.OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Reverse, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		List<Vertex2D> bars = [];
		List<Vertex2D> bars_dark = [];
		float width = 150f;
		SpriteBatchUtils.AddVerticesForCircleRing(bars, Projectile.Center, 960, width, new Color(0.6f, 0.05f, 0.11f, 0), rotationValue, 10 + rotationValue, 1 - Fade);

		if (bars.Count > 2)
		{
			Effect effect = ModAsset.VampireMat_Attack_Proj_Absorb_Fade.Value;
			effect.Parameters["uTransform"].SetValue(
				Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
				Main.GameViewMatrix.TransformationMatrix *
				Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_16.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		float timeValue = rotationValue;
		bars = [];
		bars_dark = [];
		int count = 200;
		for (int k = 0; k <= count; k++)
		{
			float value = k / (float)count * 3;
			Vector2 pos_out = Projectile.Center - Main.screenPosition + new Vector2(0, -960).RotatedBy(MathHelper.TwoPi * value);
			Vector2 pos_in = Projectile.Center - Main.screenPosition + new Vector2(0, -480).RotatedBy(MathHelper.TwoPi * value + 1f);
			bars.Add(pos_out, Color.Transparent, new Vector3(value + timeValue, 0, 0));
			bars.Add(pos_in, new Color(0.3f, 0, 0.06f, 0) * Fade, new Vector3(value + timeValue, 1, 0));
			bars_dark.Add(pos_out, Color.Transparent, new Vector3(value + timeValue, 0, 0));
			bars_dark.Add(pos_in, Color.White * Fade * 2, new Vector3(value + timeValue, 1, 0));
		}

		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		bars = [];
		bars_dark = [];
		for (int k = 0; k <= count; k++)
		{
			float value = k / (float)count * 3;
			Vector2 pos_out = Projectile.Center - Main.screenPosition + new Vector2(0, -480).RotatedBy(MathHelper.TwoPi * value + 1f);
			Vector2 pos_in = Projectile.Center - Main.screenPosition + new Vector2(0, -20).RotatedBy(MathHelper.TwoPi * value + 2f);
			bars.Add(pos_out, new Color(0.3f, 0, 0.06f, 0) * Fade, new Vector3(value + timeValue * 2, 0, 0));
			bars.Add(pos_in, Color.Transparent, new Vector3(value + timeValue * 3, 1, 0));
			bars_dark.Add(pos_out, Color.Transparent, new Vector3(value + timeValue * 2, 0, 0));
			bars_dark.Add(pos_in, Color.White * Fade * 2, new Vector3(value + timeValue * 3, 1, 0));
		}

		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
