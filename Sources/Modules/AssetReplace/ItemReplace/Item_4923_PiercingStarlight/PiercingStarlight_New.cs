using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.TileHelper;
using Terraria.Utilities;

namespace Everglow.AssetReplace.ItemReplace.Item_4923_PiercingStarlight
{
	public class PiercingStarlight_New : StabbingProjectile
	{
		public override void SetCustomDefaults()
		{
			MaxDarkAttackUnitCount = 4;
			CurrentColorFactor = 0f;
			ShadeMultiplicative_Modifier = 0f;
			ScaleMultiplicative_Modifier = 1;
			LightColorValueMultiplicative_Modifier = 0f;
			FadeGlowColorValue = 0.4f;
			OldLightColorValue = 1f;
			OldColorFactor = 0.9f;
			AttackLength = 1.85f;
			AttackEffectWidth = 0.8f;
		}

		public override void CustomBehavior()
		{
			float timeValue = (float)(Main.time / 120f);
			GlowColor = Main.hslToRgb(timeValue % 1.0f, 1, 0.5f);
			HitTileSparkColor = GlowColor * 1.2f;
			HitTileSparkColor.A = 0;
			GlowColor = Color.Lerp(GlowColor, new Color(147, 242, 255), 0.2f);
			GlowColor.A = 0;
			AttackColor = Color.Transparent;
		}

		public override void UpdateDarkAttackEffect()
		{
			UnifiedRandom rand = Main.rand;
			Vector2 Pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 2;
			float rndFloat = rand.NextFloat();
			float lerpedFloat = Utils.GetLerpValue(0f, 0.3f, rndFloat, clamped: true) * Utils.GetLerpValue(1f, 0.5f, rndFloat, clamped: true);
			float lerpedTwice = MathHelper.Lerp(0.6f, 1f, lerpedFloat);

			float rndRange = rand.NextFloat(AttackLength * 0.5f, AttackLength * 1.21f) * 2f;
			float rndDirction = rand.NextFloatDirection();
			float drawRotation = Projectile.rotation + rndDirction * (MathF.PI * 2f) * 0.03f;
			float additiveDrawPos = AttackLength * 15f + MathHelper.Lerp(0f, 50f, rndFloat) + rndRange * 16f;
			Vector2 drawPos = Pos + drawRotation.ToRotationVector2() * additiveDrawPos + rand.NextVector2Circular(20f, 20f);

			Vector2 unitVel = new Vector2(1, 0).RotatedBy(drawRotation);
			for (float k = -rndRange * 0.6f; k < rndRange; k += 0.02f)
			{
				if (SolidTileButNotSolidTop(drawPos + unitVel * 36f * k * lerpedTwice))
				{
					HitTileEffect(drawPos + unitVel * 36f * k * lerpedTwice, drawRotation, (rndRange - k) * 0.25f);
					rndRange = k;
					HitTileSound(rndRange);
					break;
				}
			}
			Vector2 drawSize = new Vector2(rndRange, AttackEffectWidth) * lerpedTwice;
			for (int f = MaxDarkAttackUnitCount - 1; f >= 0; f--)
			{
				DarkAttackEffect[f].DarkShadow = DarkAttackEffect[f].DarkShadow * MathF.Pow(ShadeMultiplicative_Modifier, 1f / NormalExtraUpdates);
				DarkAttackEffect[f].Size.Y *= MathF.Pow(ScaleMultiplicative_Modifier, 1f / NormalExtraUpdates);
			}

			if (UpdateTimer % NormalExtraUpdates == NormalExtraUpdates / 2)
			{
				if (MaxDarkAttackUnitCount > 0)
				{
					for (int f = MaxDarkAttackUnitCount - 1; f > 0; f--)
					{
						DarkAttackEffect[f] = DarkAttackEffect[f - 1];
						DarkAttackEffect[f].Postion = DarkAttackEffect[f - 1].Postion + Main.player[Projectile.owner].velocity;
						DarkAttackEffect[f].DarkShadow = DarkAttackEffect[f - 1].DarkShadow * MathF.Pow(ShadeMultiplicative_Modifier, 1f / NormalExtraUpdates);
						DarkAttackEffect[f].Size.Y = DarkAttackEffect[f - 1].Size.Y * MathF.Pow(ScaleMultiplicative_Modifier, 1f / NormalExtraUpdates);

						float timeValue = (float)(Main.time / 120f) - f / 8f;
						Color oldGlowColor = Main.hslToRgb(timeValue % 1.0f, 1, 0.5f);
						oldGlowColor = Color.Lerp(oldGlowColor, new Color(147, 242, 255), 0.2f);
						oldGlowColor.A = 0;
						DarkAttackEffect[f].Color = oldGlowColor;
						Lighting.AddLight(DarkAttackEffect[f].Postion, oldGlowColor.ToVector3() * 0.25f * DarkAttackEffect[f].Size.X);
					}
				}
				if (Projectile.timeLeft >= (MaxDarkAttackUnitCount - 1) * (NormalExtraUpdates + 1))
				{
					DarkAttackEffect[0].DarkShadow = OldColorFactor;
					DarkAttackEffect[0].Postion = drawPos;
					DarkAttackEffect[0].Size = drawSize;
					DarkAttackEffect[0].Rotation = drawRotation;
					DarkAttackEffect[0].Color = GlowColor;
				}
				else
				{
					DarkAttackEffect[0].DarkShadow = DarkAttackEffect[0].DarkShadow * MathF.Pow(ShadeMultiplicative_Modifier, 1f / NormalExtraUpdates);
					DarkAttackEffect[0].Postion = drawPos + Main.player[Projectile.owner].velocity;
					DarkAttackEffect[0].Size.Y = drawSize.Y * MathF.Pow(ScaleMultiplicative_Modifier, 1f / NormalExtraUpdates);
					DarkAttackEffect[0].Rotation = drawRotation;
					DarkAttackEffect[0].Color = GlowColor;
				}
			}
		}

		public override void DrawDarkAttackEffect(Color lightColor)
		{
			Texture2D shadow = ModAsset.Star2_Black.Value;
			Texture2D light = ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = shadow.Size() / 2f;
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					DrawParameters_Structure darkDraw = DarkAttackEffect[f];
					Main.spriteBatch.Draw(shadow, darkDraw.Postion - Main.screenPosition, null, Color.White * darkDraw.DarkShadow, darkDraw.Rotation, drawShadowOrigin, darkDraw.Size, SpriteEffects.None, 0f);
					Color fadeLight = AttackColor * darkDraw.DarkShadow;
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, darkDraw.Postion - Main.screenPosition, null, fadeLight, darkDraw.Rotation, drawOrigin, darkDraw.Size, SpriteEffects.None, 0f);
					if (GlowColor != Color.Transparent)
					{
						Main.spriteBatch.Draw(light, darkDraw.Postion - Main.screenPosition, null, darkDraw.Color * MathF.Pow(FadeGlowColorValue, f), darkDraw.Rotation, drawShadowOrigin, darkDraw.Size, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override void HitTileEffect(Vector2 hitPosition, float rotation, float power)
		{
			base.HitTileEffect(hitPosition, rotation, power * 0.5f);
		}
	}
}
