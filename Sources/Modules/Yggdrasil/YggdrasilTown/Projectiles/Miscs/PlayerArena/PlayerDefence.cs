using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Miscs.PlayerArena;

public class PlayerDefence : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 60;
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Projectile.owner < 0)
		{
			Projectile.active = false;
		}
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		Projectile.Center = owner.Center;
		base.AI();
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = ModAsset.PlayerDefenceEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_perlin.Value);
		effect.Parameters["uFade"].SetValue(Commons.ModAsset.Noise_turtleCrack.Value);
		effect.Parameters["uSize"].SetValue(2f);
		effect.Parameters["uLight"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 0.3f));
		effect.CurrentTechnique.Passes[0].Apply();

		Texture2D tex = ModAsset.PlayerDefence.Value;
		var drawColor = Color.Lerp(new Color(1f, 0, 1f, 1f), new Color(0.5f, 0, 0.5f, 0.5f), (Projectile.timeLeft - 50) / 10f);
		if (Projectile.timeLeft < 50f)
		{
			drawColor = new Color(0.5f, 0, 0.5f, 0.5f);
		}
		if (Projectile.timeLeft < 10)
		{
			drawColor.G = (byte)((1 - Projectile.timeLeft / 10f) * 255f);
		}
		Main.spriteBatch.Draw(tex, Projectile.Center, null, drawColor, 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		if (Projectile.timeLeft >= 56f)
		{
			for (int i = 0; i < Projectile.timeLeft - 56; i++)
			{
				Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f), 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
		}
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		var bars = new List<Vertex2D>();
		var colorTopRight = new Color(1, 0, 0.8f, 1);
		var colorBottomRight = new Color(1, 1, 0.8f, 1);
		var colorTopLeft = new Color(0, 0, 0.8f, 1);
		var colorBottomLeft = new Color(0, 1, 0.8f, 1);

		bars.Add(drawPos + new Vector2(0, -50), colorTopRight, new Vector3(0.5f, 0, 1));
		bars.Add(drawPos + new Vector2(50, 0), colorTopRight, new Vector3(1f, 0.5f, 1));
		bars.Add(drawPos + new Vector2(0, 0), new Color(1, 0, 0f, 1), new Vector3(0.5f, 0.5f, 1));

		bars.Add(drawPos + new Vector2(0, 50), colorBottomRight, new Vector3(0.5f, 1f, 1));
		bars.Add(drawPos + new Vector2(50, 0), colorBottomRight, new Vector3(1f, 0.5f, 1));
		bars.Add(drawPos + new Vector2(0, 0), new Color(1, 1, 0f, 1), new Vector3(0.5f, 0.5f, 1));

		bars.Add(drawPos + new Vector2(0, -50), colorTopLeft, new Vector3(0.5f, 0, 1));
		bars.Add(drawPos + new Vector2(-50, 0), colorTopLeft, new Vector3(0f, 0.5f, 1));
		bars.Add(drawPos + new Vector2(0, 0), new Color(0, 0, 0f, 1), new Vector3(0.5f, 0.5f, 1));

		bars.Add(drawPos + new Vector2(0, 50), colorBottomLeft, new Vector3(0.5f, 1f, 1));
		bars.Add(drawPos + new Vector2(-50, 0), colorBottomLeft, new Vector3(0f, 0.5f, 1));
		bars.Add(drawPos + new Vector2(0, 0), new Color(0, 1, 0f, 1), new Vector3(0.5f, 0.5f, 1));
		if (bars.Count > 3)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(ModAsset.PlayerDefence.Value, bars, PrimitiveType.TriangleList);
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft > 0)
		{
			for (int s = 0; s < 40; s++)
			{
				float addPosY = Main.rand.NextFloat(-50, 50);
				float addPosX = 50 - MathF.Abs(addPosY);
				if (Main.rand.NextBool())
				{
					addPosX *= -1;
				}
				var addPos = new Vector2(addPosX, addPosY);
				addPos *= MathF.Sqrt(Main.rand.NextFloat());
				Vector2 newVelocity = addPos * 0.15f;
				SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
				var dust = new PlayerDefenseShards
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + addPos,
					MaxTime = Main.rand.Next(6, 42),
					Scale = 0,
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(4.0f, 14.5f), Main.rand.NextFloat(-0.03f, 0.03f) },
				};
				Ins.VFXManager.Add(dust);
			}
		}
		base.OnKill(timeLeft);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}
}
