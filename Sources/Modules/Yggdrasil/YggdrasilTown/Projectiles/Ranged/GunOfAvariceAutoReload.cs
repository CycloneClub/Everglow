using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.Common.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class GunOfAvariceAutoReload : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	private bool ReloadStatus => Projectile.ai[0] == 0;

	private int Level => (int)Projectile.ai[1];

	private bool HasNotPlayedSound { get; set; } = true;

	private Player Owner => Main.player[Projectile.owner];

	public override void SetDefaults()
	{
		Projectile.width = 62;
		Projectile.height = 32;
		Projectile.timeLeft = GunOfAvarice.AutoReloadDuration;
		Projectile.penetrate = -1;
		Projectile.hide = true;
		Projectile.scale = 0.75f;
	}

	public override void AI()
	{
		if (Projectile.timeLeft <= 15 && HasNotPlayedSound)
		{
			HasNotPlayedSound = false;
			SoundEngine.PlaySound(new SoundStyle(ModAsset.GunReload2_Mod));

			if (ReloadStatus)
			{
				SuccessVFX(Level);
			}
			else
			{
				FailureVFX(Level);
				Owner.Hurt(PlayerDeathReason.ByCustomReason($"{Owner.name} died in explosion!"), Projectile.damage, 0, false, false, 0);
			}
			for (int i = 0; i < 14; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<BulletShell_yggdrasil>());
				dust.velocity = new Vector2(0, -Main.rand.NextFloat(5, 8)).RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
			}
		}
		float timeValue = 1 - Projectile.timeLeft / 30f;
		float deltaRot = 1.8f;
		var offset = new Vector2(-15f * Owner.direction, 0);
		if (Projectile.timeLeft <= 30)
		{
			if (timeValue < 0.2f)
			{
				deltaRot = 1.8f - MathF.Pow(timeValue / 0.2f, 0.5f) * 1.4f;
			}
			if (timeValue >= 0.2f)
			{
				deltaRot = 0.6f + MathF.Pow((timeValue - 0.2f) / 0.8f, 0.5f) * 1.2f;
			}
			if (timeValue is > 0.1f and < 0.3f)
			{
				offset = new Vector2((0.5f - Math.Abs(timeValue - 0.2f) / 0.1f) * -15f, 0).RotatedBy(Projectile.rotation) + new Vector2(-15f * Owner.direction, 0);
			}
		}
		Projectile.rotation = -MathHelper.PiOver2 + Owner.direction * deltaRot;
		Projectile.spriteDirection = Owner.direction;
		Projectile.Center = Owner.Center + offset + new Vector2(24 * Owner.direction, -8);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D gun = ModAsset.GunOfAvarice.Value;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.spriteBatch.Draw(gun, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, gun.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
		if (Projectile.timeLeft > 30 && Projectile.timeLeft < 60)
		{
			var bloomColor = new Color(0.5f, 0f, 0f, 0f);
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			if (Projectile.ai[0] == 0)
			{
				bloomColor = new Color(0f, 0.6f, 0.8f, 0f);
			}
			float size = (60 - Projectile.timeLeft) / 30f;
			size = MathF.Pow(size, 0.4f);
			size = MathF.Sin(size * MathHelper.Pi);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, bloomColor, 0, star.Size() * 0.5f, size, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, bloomColor, MathHelper.PiOver2, star.Size() * 0.5f, size, SpriteEffects.None, 0);

			float timer = 90 - Projectile.timeLeft;
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Effect effect = ModAsset.TeleportToYggdrasilVortexEffect.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			int precise = 150;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["uTimer"].SetValue(timer * 0.018f);
			effect.Parameters["uColor0"].SetValue(new Vector4(1f, 1f, 1f, 1f));
			effect.Parameters["uColor1"].SetValue(new Vector4(1f, 1f, 1f, 1f));
			effect.CurrentTechnique.Passes[0].Apply();
			Vector2 drawCenter = Projectile.Center;
			float timeValue = (float)Main.time * 0.004f;
			timeValue += MathF.Pow(timer / 210f, 5) * 20f;

			// dark net
			float deltaRot = 0.05f;
			var nets = new List<Vertex2D>();
			for (int i = 0; i <= precise; i++)
			{
				Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
				float vertexWidth = 60;

				float fade = Math.Clamp((210 - Projectile.timeLeft) / 100f, 0, 1);
				var drawColor = new Color(1f, 1f, 1f, 1f);
				nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
				nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, timer * deltaRot));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_crack_dense_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["uTimer"].SetValue(timer * 0.018f);
			effect.Parameters["uColor0"].SetValue(bloomColor.ToVector4());
			effect.Parameters["uColor1"].SetValue(bloomColor.ToVector4() * 0.8f);
			effect.CurrentTechnique.Passes[0].Apply();

			nets = new List<Vertex2D>();
			for (int i = 0; i <= precise; i++)
			{
				Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
				float vertexWidth = 40;

				float fade = Math.Clamp((210 - Projectile.timeLeft) / 100f, 0, 1);
				var drawColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(i / 37.5f * MathHelper.TwoPi + timeValue * 3) + 1) * 0.5f);
				nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
				nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, timer * deltaRot));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		return false;
	}

	public void FailureVFX(int level)
	{
		for (int i = 0; i < level * 12 + 40; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 8.4f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = new AvariceFailureDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(1f, 2f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < level * 4 + 14; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 13.4f)).RotatedByRandom(MathHelper.TwoPi);
			var cube = new AvariceFailureCube
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(10f, 50f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0.06f },
			};
			Ins.VFXManager.Add(cube);
		}
		float waveRot = Main.rand.NextFloat(6.283f);
		for (int i = 0; i < 2; i++)
		{
			var wave = new AvariceFailureWave
			{
				velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				position = Owner.Center,
				maxTime = Main.rand.Next(30, 40),
				scale = 1 + i * 0.6f,
				rotation = waveRot + i * 2f,
				ai = new float[] { 0.04f * MathF.Sqrt(level) },
			};
			Ins.VFXManager.Add(wave);
		}
	}

	public void SuccessVFX(int level)
	{
		for (int i = 0; i < level * 3 + 10; i++)
		{
			var vel = new Vector2(0, -Main.rand.NextFloat(3.6f, 6.4f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 22.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var dust = new AvariceSuccessDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(1f, 2f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < level * 2 + 8; i++)
		{
			var vel = new Vector2(0, -Main.rand.NextFloat(1.6f, 6.4f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 50.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var cube = new AvariceSuccessCube
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(10f, 20f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 20.93f), 0.06f },
			};
			Ins.VFXManager.Add(cube);
		}
	}
}
