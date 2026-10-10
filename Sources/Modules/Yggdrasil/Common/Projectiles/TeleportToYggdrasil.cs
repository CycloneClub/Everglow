using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.Common.Projectiles;

public class TeleportToYggdrasil : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MiscsProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public Vector2 HitPosition;
	public Vector2 TouchedPlayerPosition;
	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 210;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.scale = 1;
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		HitPosition = Projectile.position;
		TouchedPlayerPosition = Projectile.Center + new Vector2(Math.Sign((Projectile.Center - Main.player[Projectile.owner].Center).X) * -18, 0);
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 60, 40f, 120, 0.95f, 0.95f, 210);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.Center = TouchedPlayerPosition;
		Projectile.position = HitPosition;
		Timer++;
		Main.ColorOfSurfaceBackgroundsModified *= 0.5f;
		Main.ColorOfTheSkies = Color.Red;
		Lighting.GlobalBrightness *= 0.5f;
		if (!NetUtils.IsServer)
		{
			Main.ApplyColorOfTheSkiesToTiles();
		}
		TeleportYggdrasilLightSystem.EffectProjWhoAmI = Projectile.whoAmI;
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 1.6f, 1f) * Timer * 0.05f);

		if (Projectile.timeLeft <= 1)
		{
			Projectile.active = false;
			Ins.VFXManager.Clear();
			if (SubworldSystem.IsActive<YggdrasilWorld>())
			{
				SubworldSystem.Exit();
			}
			else
			{
				if (!SubworldSystem.Enter<YggdrasilWorld>())
				{
					Main.NewText("Fail!");
				}
			}
		}
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect = ModAsset.TeleportToYggdrasilVortexEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		int precise = 150;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uTimer"].SetValue(Timer * 0.018f);
		effect.Parameters["uColor0"].SetValue(new Vector4(1f, 1f, 1f, 1f));
		effect.Parameters["uColor1"].SetValue(new Vector4(1f, 1f, 1f, 1f));
		effect.CurrentTechnique.Passes[0].Apply();
		Vector2 drawCenter = Projectile.Center;
		float timeValue = (float)Main.time * 0.004f;
		timeValue += MathF.Pow(Timer / 210f, 5) * 20f;

		// dark net
		float deltaRot = 0.05f;
		List<Vertex2D> nets = new List<Vertex2D>();
		for (int i = 0; i <= precise; i++)
		{
			Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
			float vertexWidth = 600;

			float fade = Math.Clamp((210 - Projectile.timeLeft) / 100f, 0, 1);
			Color drawColor = new Color(1f, 1f, 1f, 1f);
			nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
			nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, Timer * deltaRot));
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_crack_dense_black.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

		nets = new List<Vertex2D>();
		for (int i = 0; i <= precise; i++)
		{
			Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
			float vertexWidth = 500;

			float fade = Math.Clamp((210 - Projectile.timeLeft) / 100f, 0, 1);
			Color drawColor = new Color(1f, 1f, 1f, 1f);
			nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
			nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, Timer * deltaRot));
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_Sand.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uTimer"].SetValue(Timer * 0.018f);
		effect.Parameters["uColor0"].SetValue(new Vector4(0.1f, 0.7f, 1f, 0));
		effect.Parameters["uColor1"].SetValue(new Vector4(1f, 1f, 0.3f, 0));
		effect.CurrentTechnique.Passes[0].Apply();

		nets = new List<Vertex2D>();
		for (int i = 0; i <= precise; i++)
		{
			Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
			float vertexWidth = 400;

			float fade = Math.Clamp((210 - Projectile.timeLeft) / 100f, 0, 1);
			Color drawColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(i / 37.5f * MathHelper.TwoPi + timeValue * 3) + 1) * 0.5f);
			nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
			nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, Timer * deltaRot));
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

		if (Timer < 30)
		{
			nets = new List<Vertex2D>();
			for (int i = 0; i <= precise; i++)
			{
				Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
				float vertexWidth = Timer * 10 + 600;

				float fade = Math.Clamp((30 - Timer) / 30f, 0, 1);
				Color drawColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(i / 37.5f * MathHelper.TwoPi + timeValue * 3) + 1) * 0.5f);
				nets.Add(drawCenter + normalWidth * vertexWidth, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
				nets.Add(drawCenter + normalWidth * (vertexWidth + 600), Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, Timer * deltaRot));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

			nets = new List<Vertex2D>();
			for (int i = 0; i <= precise; i++)
			{
				Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
				float vertexWidth = Timer * 10 + 600;

				float fade = Math.Clamp((30 - Timer) / 30f, 0, 1);
				Color drawColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(i / 37.5f * MathHelper.TwoPi + timeValue * 3) + 1) * 0.5f);
				nets.Add(drawCenter + normalWidth * (vertexWidth - 600), Color.Transparent, new Vector3(new Vector2(0.5f), 0));
				nets.Add(drawCenter + normalWidth * vertexWidth, drawColor * fade, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, Timer * deltaRot));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		effect = ModAsset.TeleportToYggdrasilFlowEffect.Value;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();

		// dark flow
		for (int r = 0; r < 15; r++)
		{
			List<Vertex2D> flows = new List<Vertex2D>();
			for (int i = 0; i < 150; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 14).RotatedBy(GetFlowEffectRotation(i, r));
				Vector2 nextJoint = new Vector2(0, (i + 1) * 14).RotatedBy(GetFlowEffectRotation(i + 1, r));
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 120 + MathF.Sin(r) * 30;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 65f, 0.5f) * MathF.PI);
				float fade = Math.Clamp((130 - Projectile.timeLeft + (i + 75 - r * 6) * 0.5f) * 0.1f, 0, 1);
				if (Timer < 160)
				{
					fade *= Timer / 160f;
				}
				Color drawColor = new Color(1f, 1f, 1f, 1);
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_4_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}

		// light flow
		for (int r = 0; r < 15; r++)
		{
			List<Vertex2D> flows = new List<Vertex2D>();
			for (int i = 0; i < 150; i++)
			{
				Vector2 thisJoint = new Vector2(0, i * 14).RotatedBy(GetFlowEffectRotation(i, r));
				Vector2 nextJoint = new Vector2(0, (i + 1) * 14).RotatedBy(GetFlowEffectRotation(i + 1, r));
				Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
				float vertexWidth = 120 + MathF.Sin(r) * 30;
				normalWidth *= vertexWidth;
				float drawWidth = MathF.Sin(Math.Min(i / 65f, 0.5f) * MathF.PI);
				float fade = Math.Clamp((130 - Projectile.timeLeft + (i + 55 - r * 6) * 0.5f) * 0.1f, 0, 1);
				if (i > 30)
				{
					fade *= MathF.Max((100 - i) / 70f, 0);
				}
				if (Timer < 160)
				{
					fade *= MathF.Pow(Timer / 160f, 0.5f);
				}
				if (Timer > 60 && Timer < 120)
				{
					fade *= (float)Utils.Lerp(1f, (Projectile.timeLeft - i) / (float)(Projectile.timeLeft + 1), (Timer - 60) / 60f);
				}
				else if (Timer >= 120)
				{
					fade *= (Projectile.timeLeft - i) / (float)(Projectile.timeLeft + 1);
				}
				Color drawColor = new Color(1f, 1f, 0.3f, 0);
				if (r % 2 == 1)
				{
					drawColor = new Color(0.1f, 0.7f, 1f, 0);
				}
				flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 0, drawWidth));
				flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 1, drawWidth));
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_4.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
		}
		if (Timer > 160)
		{
			for (int r = 0; r < 15; r++)
			{
				List<Vertex2D> flows = new List<Vertex2D>();
				for (int i = 0; i < 150; i++)
				{
					Vector2 thisJoint = new Vector2(0, i * 14).RotatedBy(GetFlowEffectRotation(i, r));
					Vector2 nextJoint = new Vector2(0, (i + 1) * 14).RotatedBy(GetFlowEffectRotation(i + 1, r));
					Vector2 normalWidth = Vector2.Normalize(nextJoint - thisJoint).RotatedBy(MathHelper.PiOver2);
					float vertexWidth = 120 + MathF.Sin(r) * 30;
					normalWidth *= vertexWidth;
					float drawWidth = MathF.Sin(Math.Min(i / 65f, 0.5f) * MathF.PI);
					float fade = Math.Clamp((130 - Projectile.timeLeft + (i + 55 - r * 6) * 0.5f) * 0.1f, 0, 1);
					if (i > 30)
					{
						fade *= MathF.Max((100 - i) / 70f, 0);
					}
					if (Timer < 160)
					{
						fade *= MathF.Pow(Timer / 160f, 0.5f);
					}
					if (Timer >= 120)
					{
						fade *= (Projectile.timeLeft - i) / (float)(Projectile.timeLeft + 1);
					}
					fade *= (Timer - 160) / 30f;
					Color drawColor = new Color(1f, 1f, 0.3f, 0);
					if (r % 2 == 1)
					{
						drawColor = new Color(0.1f, 0.7f, 1f, 0);
					}
					flows.Add(drawCenter + thisJoint - normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 0, drawWidth));
					flows.Add(drawCenter + thisJoint + normalWidth, drawColor * fade, new Vector3(i * 0.03f + timeValue + r * 0.2f, 1, drawWidth));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_4.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, flows.ToArray(), 0, flows.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Color starColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(timeValue * 24) + 1) * 0.5f);
		float starscale = (210 - Projectile.timeLeft) / 100f;
		Main.spriteBatch.Draw(star, drawCenter - Main.screenPosition, null, starColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(Math.Clamp(starscale, 0, 1), 1f + starscale * 3), SpriteEffects.None, 0);
		starColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(timeValue * 32) + 1) * 0.5f);
		Main.spriteBatch.Draw(star, drawCenter - Main.screenPosition, null, starColor, 0, star.Size() * 0.5f, new Vector2(Math.Clamp(starscale, 0, 1), 1f + starscale * 0.6f), SpriteEffects.None, 0);
		starColor = Color.Lerp(new Color(0.1f, 0.7f, 1f, 0), new Color(1f, 1f, 0.3f, 0), (MathF.Sin(timeValue * 37 + 4.5f) + 1) * 0.5f);
		Main.spriteBatch.Draw(star, drawCenter - Main.screenPosition, null, starColor, MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(Math.Clamp(starscale, 0, 1), 1f + starscale * 0.6f) * 0.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, drawCenter - Main.screenPosition, null, starColor, -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(Math.Clamp(starscale, 0, 1), 1f + starscale * 0.6f) * 0.5f, SpriteEffects.None, 0);

		if (Projectile.timeLeft < 20)
		{
			Texture2D bloom = Commons.ModAsset.LightPoint.Value;
			Main.spriteBatch.Draw(bloom, drawCenter - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * ((20 - Projectile.timeLeft) * 0.1f), -MathHelper.PiOver4, bloom.Size() * 0.5f, 1000f, SpriteEffects.None, 0);
		}
		return false;
	}

	private float GetFlowEffectRotation(int i, int r)
	{
		float timeValue = (float)Main.time * 0.004f;
		timeValue += MathF.Pow(Timer / 210f, 5) * 20f;
		timeValue *= 0.5f;
		float addRot = 0;
		if (i < 30)
		{
			addRot = (MathF.Cos(i / 30f * MathHelper.Pi) + 1) * Timer * 0.0007f;
		}
		return r / 5f * MathHelper.TwoPi + MathF.Sin(i * 0.03f + r) * 1.4f + MathF.Sin(i * 0.03f + r + timeValue) * 0.2f + addRot;
	}

	private static void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radious / 2; h += 1)
		{
			float colorR = (h / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);

			color = new Color(colorR, color.G / 255f, 0, 0);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.2f, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (Timer < 30)
		{
			float value = Timer / 30f;
			value = MathF.Sqrt(value);
			float colorV = 3.9f * (1 - value);
			Texture2D t = Commons.ModAsset.Noise_rgbPerlin.Value;
			float width = 600;

			DrawWarpTexCircle_VFXBatch(spriteBatch, value * 1600, width, new Color(colorV, colorV * 0.07f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
		}
	}
}

public class TeleportYggdrasilLightSystem : ModSystem
{
	public static int EffectProjWhoAmI = -1;

	public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
	{
		if (EffectProjWhoAmI >= 0)
		{
			if (Main.projectile[EffectProjWhoAmI] != null && Main.projectile[EffectProjWhoAmI].type == ModContent.ProjectileType<TeleportToYggdrasil>() && Main.projectile[EffectProjWhoAmI].active && Main.projectile[EffectProjWhoAmI].timeLeft > 0)
			{
				float colorValue = Main.projectile[EffectProjWhoAmI].timeLeft / 210f;
				tileColor *= colorValue;
				backgroundColor *= colorValue;
			}
			else
			{
				EffectProjWhoAmI = -1;
			}
		}
	}
}
