using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Everglow.EternalResolve.Bosses.Projectiles
{
	public abstract class Rapier_Hostile : ModProjectile, IWarpProjectile
	{
		public int itemType;

		/// <summary>
		/// 常规颜色
		/// </summary>
		public Color Color = Color.White;

		/// <summary>
		/// 阴影强度
		/// </summary>
		public float CurrentColorFactor = 0f;

		/// <summary>
		/// 重影深度
		/// </summary>
		public float OldColorFactor = 0f;

		/// <summary>
		/// 重影彩色部分亮度
		/// </summary>
		public float OldLightColorValue = 0f;

		/// <summary>
		/// 重影数量
		/// </summary>
		public int MaxDarkAttackUnitCount = 0; // 小于200

		/// <summary>
		/// 重影大小缩变,小于1
		/// </summary>
		public float ScaleMultiplicative_Modifier = 0f;

		/// <summary>
		/// 刀光宽度1
		/// </summary>
		public float AttackEffectWidth = 1f;

		/// <summary>
		/// 重影深度缩变,小于1
		/// </summary>
		public float ShadeMultiplicative_Modifier = 0f;

		/// <summary>
		/// 重影彩色部分亮度缩变,小于1
		/// </summary>
		public float LightColorValueMultiplicative_Modifier = 0f;

		/// <summary>
		/// 表示刺剑攻击长度,标准长度1
		/// </summary>
		public float AttackLength = 1f;

		/// <summary>
		/// 荧光颜色,默认不会发光
		/// </summary>
		public Color GlowColor = Color.Transparent;

		/// <summary>
		/// 荧光颜色缩变,小于1
		/// </summary>
		public float FadeGlowColorValue = 0f;

		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		/// <summary>
		/// 宽度width会影响伤害判定(斜矩形)的宽度,高度会影响判定的长度
		/// </summary>
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 4;
			Projectile.timeLeft = 60;
		}

		public override void AI()
		{
			NPC npc = Main.npc[(int)Projectile.ai[0]];
			Projectile.Center = npc.Center + Projectile.velocity * 25 + new Vector2(0, 10);
			Projectile.velocity = npc.ai[3].ToRotationVector2();
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = Projectile.direction;
			Projectile.soundDelay--;
			npc.spriteDirection = Projectile.spriteDirection;
			if (Projectile.soundDelay <= 0)
			{
				// SoundStyle ss = new SoundStyle(ModAsset.StabbingSwordSound_Mod);
				SoundStyle ss = SoundID.Item1;
				SoundEngine.PlaySound(ss, Projectile.Center);
				Projectile.soundDelay = 6;
			}

			UpdateItemDraw();
			UpdateDarkAttackEffect();
			UpdateLightAttackEffect();
		}

		public virtual void HitTileSound(float scale)
		{
			// SoundStyle ss = new SoundStyle("Everglow/Commons/Weapons/StabbingSwords/StabCollide");
			SoundStyle ss = SoundID.NPCHit4;
			SoundEngine.PlaySound(ss.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)).WithVolume(0.3f), Projectile.Center);
			Projectile.soundDelay = 6;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float point = 0;
			Vector2 HitRange = Projectile.velocity.SafeNormalize(Vector2.Zero) * AttackLength * 100;
			if (Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, new Vector2(targetHitbox.Left + targetHitbox.Width / 2f, targetHitbox.Top + targetHitbox.Height / 2f), 0, 0))
			{
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + HitRange, Projectile.width, ref point))
				{
					return true;
				}
			}
			return false;
		}

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			float cutLength = 164f;
			Vector2 end = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * cutLength * Projectile.scale * AttackLength;
			while (!Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, end, 0, 0))
			{
				cutLength -= 8;
				if (cutLength < 0)
				{
					break;
				}
				end = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * cutLength * Projectile.scale * AttackLength;
			}
			Utils.PlotTileLine(Projectile.Center, end, 80f * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			return false;
		}

		public struct DrawParameters_Structure
		{
			public DrawParameters_Structure(Vector2 postion, Color color, float rotation, Vector2 size, SpriteEffects spriteEffect)
			{
				Postion = postion;
				Color = color;
				Rotation = rotation;
				Size = size;
				SpriteEffect = spriteEffect;
			}

			public Vector2 Postion;
			public Color Color;
			public float Rotation;
			public Vector2 Size;
			public SpriteEffects SpriteEffect;
		}

		public DrawParameters_Structure ItemDraw = default(DrawParameters_Structure);

		public void UpdateItemDraw()
		{
			UnifiedRandom rand = Main.rand;
			Vector2 Pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 2;
			float itemDrawRotation = 8f + MathHelper.Lerp(0f, 20f, rand.NextFloat()) + rand.NextFloat() * 6f;
			float itemAddRotation = Projectile.rotation + rand.NextFloatDirection() * MathF.PI * 2f * 0.04f;
			float itemFinalRotation = itemAddRotation + MathF.PI / 4f;
			Vector2 itemPos = Pos + itemAddRotation.ToRotationVector2() * itemDrawRotation + rand.NextVector2Circular(8f, 8f);
			SpriteEffects itemSpriteEffect = SpriteEffects.None;
			if (Projectile.rotation < -MathF.PI / 2f || Projectile.rotation > MathF.PI / 2f)
			{
				itemFinalRotation += MathF.PI / 2f;
				itemSpriteEffect = itemSpriteEffect | SpriteEffects.FlipHorizontally;
			}
			ItemDraw.Postion = itemPos;
			ItemDraw.Rotation = itemFinalRotation;
			ItemDraw.Size = new Vector2(1f);
			ItemDraw.SpriteEffect = itemSpriteEffect;
		}

		public DrawParameters_Structure[] DarkAttackEffect = new DrawParameters_Structure[200];

		public void UpdateDarkAttackEffect()
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
			bool canHit = Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, drawPos + Vector2.Normalize(Projectile.velocity) * 36f * rndRange * lerpedTwice, 0, 0);

			float volumn = rndRange;
			if (!Main.gamePaused && !canHit)
			{
				while (!canHit)
				{
					volumn *= 0.9f;
					drawPos -= Projectile.velocity * 0.2f;
					if (volumn < 0.3f)
					{
						break;
					}
				}
				HitTileSound(volumn);
			}
			Vector2 drawSize = new Vector2(volumn, AttackEffectWidth) * lerpedTwice;
			if (MaxDarkAttackUnitCount > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > 0; f--)
				{
					DarkAttackEffect[f] = DarkAttackEffect[f - 1];
					DarkAttackEffect[f].Postion = DarkAttackEffect[f - 1].Postion + Main.player[Projectile.owner].velocity;
					DarkAttackEffect[f].Color.A = (byte)(DarkAttackEffect[f - 1].Color.A * ShadeMultiplicative_Modifier);
					DarkAttackEffect[f].Size.Y = DarkAttackEffect[f - 1].Size.Y * ScaleMultiplicative_Modifier;
				}
			}
			if (Projectile.timeLeft >= MaxDarkAttackUnitCount - 1)
			{
				DarkAttackEffect[0].Color.A = (byte)(OldColorFactor * 255);
				DarkAttackEffect[0].Postion = drawPos;
				DarkAttackEffect[0].Size = drawSize;
				DarkAttackEffect[0].Rotation = drawRotation;
			}
			else
			{
				DarkAttackEffect[0].Color.A = (byte)(DarkAttackEffect[0].Color.A * ShadeMultiplicative_Modifier);
				DarkAttackEffect[0].Postion = drawPos + Main.player[Projectile.owner].velocity;
				DarkAttackEffect[0].Size.Y = drawSize.Y * ScaleMultiplicative_Modifier;
				DarkAttackEffect[0].Rotation = drawRotation;
			}
		}

		public DrawParameters_Structure LightAttackEffect = default(DrawParameters_Structure);

		public void UpdateLightAttackEffect()
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
			while (!Collision.CanHit(Projectile.Center - Projectile.velocity, 0, 0, drawPos + Vector2.Normalize(Projectile.velocity) * 36f * rndRange * lerpedTwice, 0, 0))
			{
				rndRange *= 0.9f;
				drawPos -= Projectile.velocity * 0.2f;
				if (rndRange < 0.3f)
				{
					break;
				}
			}
			Vector2 drawSize = new Vector2(rndRange, AttackEffectWidth) * lerpedTwice;
			LightAttackEffect.Postion = drawPos;
			LightAttackEffect.Size = drawSize;
			LightAttackEffect.Rotation = drawRotation;
		}

		public virtual void DrawBeforeItem()
		{
		}

		/// <summary>
		/// mulVelocity 决定了旗帜下方两个角收到弹幕速度的影响大小
		/// </summary>
		/// <param name="lightColor"></param>
		/// <param name="flagTexture"></param>
		/// <param name="mulVelocityLeft"></param>
		/// <param name="mulVelocityRight"></param>
		/// <param name="flagLeftX">旗帜左边缘相对于武器绘制位置的横向像素偏移，在旋转和翻转前使用。</param>
		/// <param name="flagTopY">旗帜上边缘相对于武器绘制位置的纵向像素偏移，在旋转前使用。</param>
		public void DrawFlags(Color lightColor, float flagLeftX, float flagTopY, Texture2D flagTexture, float mulVelocityLeft = 1f, float mulVelocityRight = 1f)
		{
			Player player = Main.player[Projectile.owner];
			float flagRightX = flagLeftX + flagTexture.Width;
			Vector2 flagTopLeft = ItemDraw.Postion + new Vector2(flagLeftX, flagTopY).RotatedBy(ItemDraw.Rotation) - Main.screenPosition;
			Vector2 flagTopRight = ItemDraw.Postion + new Vector2(flagRightX, flagTopY).RotatedBy(ItemDraw.Rotation) - Main.screenPosition;
			if (ItemDraw.SpriteEffect == SpriteEffects.FlipHorizontally)
			{
				flagTopLeft = ItemDraw.Postion + new Vector2(-flagRightX, flagTopY).RotatedBy(ItemDraw.Rotation) - Main.screenPosition;
				flagTopRight = ItemDraw.Postion + new Vector2(-flagLeftX, flagTopY).RotatedBy(ItemDraw.Rotation) - Main.screenPosition;
			}
			Vector2 flagBottomLeft = flagTopLeft + new Vector2(0, flagTexture.Height) - Projectile.velocity * mulVelocityLeft - player.velocity;
			Vector2 flagBottomRight = flagTopRight + new Vector2(0, flagTexture.Height) - Projectile.velocity * mulVelocityRight - player.velocity;
			Vector2 deltaBottom = flagBottomRight - flagBottomLeft;
			float velBottomLeft = (deltaBottom.X - flagTexture.Width) * 1f;
			float velBottomRight = -(deltaBottom.X - flagTexture.Width) * 1f;
			flagBottomLeft.X += velBottomLeft;
			flagBottomRight.X += velBottomRight;
			List<Vertex2D> bars = new List<Vertex2D>();
			bars.Add(new Vertex2D(flagTopLeft, lightColor, new Vector3(0, 0, 0)));
			bars.Add(new Vertex2D(flagTopRight, lightColor, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(flagBottomLeft, lightColor, new Vector3(0, 1, 0)));
			bars.Add(new Vertex2D(flagBottomRight, lightColor, new Vector3(1, 1, 0)));
			Main.graphics.GraphicsDevice.Textures[0] = flagTexture;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		public virtual void DrawItem(Color lightColor)
		{
			int type = itemType;
			Texture2D itemTexture = TextureAssets.Item[type].Value;
			Main.spriteBatch.Draw(itemTexture, ItemDraw.Postion - Main.screenPosition, null, lightColor, ItemDraw.Rotation, itemTexture.Size() / 2f, ItemDraw.Size, ItemDraw.SpriteEffect, 0f);
		}

		public virtual void DrawAfterItem()
		{
		}

		public virtual void DrawEffect(Color lightColor)
		{
			Texture2D Shadow = Commons.ModAsset.Star2_black.Value;
			Texture2D light = Commons.ModAsset.StabbingProjectile.Value;
			Vector2 drawOrigin = light.Size() / 2f;
			Vector2 drawShadowOrigin = Shadow.Size() / 2f;
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Main.spriteBatch.Draw(Shadow, DarkAttackEffect[f].Postion - Main.screenPosition, null, Color.White * (DarkAttackEffect[f].Color.A / 255f), DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					Color fadeLight = Color * (DarkAttackEffect[f].Color.A / 255f);
					fadeLight.A = 0;
					fadeLight = fadeLight * OldLightColorValue * MathF.Pow(LightColorValueMultiplicative_Modifier, f);
					fadeLight = new Color(lightColor.R / 255f * fadeLight.R / 255f, lightColor.G / 255f * fadeLight.G / 255f, lightColor.B / 255f * fadeLight.B / 255f, 0);
					Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, fadeLight, DarkAttackEffect[f].Rotation, drawOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					if (GlowColor != Color.Transparent)
					{
						Main.spriteBatch.Draw(light, DarkAttackEffect[f].Postion - Main.screenPosition, null, GlowColor * MathF.Pow(FadeGlowColorValue, f), DarkAttackEffect[f].Rotation, drawShadowOrigin, DarkAttackEffect[f].Size, SpriteEffects.None, 0f);
					}
				}
			}
			if (CurrentColorFactor > 0)
			{
				Main.spriteBatch.Draw(Shadow, LightAttackEffect.Postion - Main.screenPosition, null, Color.White * CurrentColorFactor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
			Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, new Color(lightColor.R / 255f * Color.R / 255f, lightColor.G / 255f * Color.G / 255f, lightColor.B / 255f * Color.B / 255f, 0), LightAttackEffect.Rotation, drawOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			if (GlowColor != Color.Transparent)
			{
				Main.spriteBatch.Draw(light, LightAttackEffect.Postion - Main.screenPosition, null, GlowColor, LightAttackEffect.Rotation, drawShadowOrigin, LightAttackEffect.Size, SpriteEffects.None, 0f);
			}
		}

		public override void PostDraw(Player player, Color lightColor)
		{
			DrawBeforeItem();
			DrawItem(lightColor);
			DrawAfterItem();
			DrawEffect(lightColor);
		}

		public void DrawWarp(VFXBatch sb)
		{
			float time = (float)(Main.time * 0.03);
			if (OldColorFactor > 0)
			{
				for (int f = MaxDarkAttackUnitCount - 1; f > -1; f--)
				{
					Vector2 center = DarkAttackEffect[f].Postion - Main.screenPosition;
					Vector2 normalX = new Vector2(0, 40).RotatedBy(LightAttackEffect.Rotation).RotatedBy(-Math.PI / 2) * LightAttackEffect.Size.X;
					Vector2 normalY = new Vector2(0, 15).RotatedBy(LightAttackEffect.Rotation) * LightAttackEffect.Size.Y;
					Vector2 start = center - normalX * 0.4f;
					Vector2 middle = center;
					Vector2 end = center + normalX;
					Color alphaColor = Color;
					alphaColor.A = 0;
					alphaColor.R = (byte)(((DarkAttackEffect[f].Rotation + 6.283 + Math.PI) % 6.283) / 6.283 * 255);
					alphaColor.G = (byte)(DarkAttackEffect[f].Color.A / 10f);
					List<Vertex2D> bars = new List<Vertex2D>
					{
						new Vertex2D(start - normalY, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0, 0)),
						new Vertex2D(start + normalY, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 1, 0)),
						new Vertex2D(middle - normalY, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0, 0.5f)),
						new Vertex2D(middle + normalY, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 1, 0.5f)),
						new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
						new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
					};
					sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
				}
			}
			if (OldColorFactor > 0)
			{
				Vector2 center = LightAttackEffect.Postion - Main.screenPosition;
				Vector2 normalX = new Vector2(0, 45).RotatedBy(LightAttackEffect.Rotation).RotatedBy(-Math.PI / 2) * LightAttackEffect.Size.X;
				Vector2 normalY = new Vector2(0, 20).RotatedBy(LightAttackEffect.Rotation) * LightAttackEffect.Size.Y;
				Vector2 start = center - normalX * 0.4f;
				Vector2 middle = center;
				Vector2 end = center + normalX;
				Color alphaColor = Color;
				alphaColor.A = 0;
				alphaColor.R = (byte)((LightAttackEffect.Rotation + 6.283 + Math.PI) % 6.283 / 6.283 * 255);
				alphaColor.G = 20;
				List<Vertex2D> bars = new List<Vertex2D>
				{
					new Vertex2D(start - normalY, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0, 0)),
					new Vertex2D(start + normalY, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 1, 0)),
					new Vertex2D(middle - normalY, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0, 0.5f)),
					new Vertex2D(middle + normalY, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 1, 0.5f)),
					new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
					new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
				};
				sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
			}
		}
	}
}
