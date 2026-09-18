using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class BladeOfGreenMoss_Proj : MeleeProj
{
	public override void SetDef()
	{
		maxAttackType = 4;
		maxSlashTrailLength = 20;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		;
		autoEnd = false;
		canLongLeftClick = true;
		maxClickTimer = 240;
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return ModAsset.BladeOfGreenMoss_Color_Mod;
	}

	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 2f;
	}

	public override BlendState TrailBlendState()
	{
		return BlendState.AlphaBlend;
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		glowTexture = ModAsset.BladeOfGreenMoss_glow.Value;

		base.DrawSelf(spriteBatch, lightColor, diagonal, drawScale, glowTexture);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrail(lightColor);
		DrawSelf(Main.spriteBatch, lightColor);
		if (timer < 60 && currantAttackType == 3)
		{
			float value = timer;
			value /= 4f;
			value = Math.Min(value, (60 - timer) * 0.3f) / 5;
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Vector2 starPos = Projectile.Center - Main.screenPosition + mainAxisDirection.RotatedBy(Projectile.spriteDirection * 0.175f) * 0.4f;
			Lighting.AddLight(Projectile.Center + mainAxisDirection * 0.4f, new Vector3(0.1f, 0.36f, 0.24f) * value * 10);
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.1f, 1f, 0.6f, 0f), 0, star.Size() / 2f, new Vector2(0.4f, 0.5f) * value, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.1f, 1f, 0.6f, 0f), MathHelper.PiOver2, star.Size() / 2f, new Vector2(0.6f, 0.9f) * value, SpriteEffects.None, 0);
		}
		if (timer < 60 && currantAttackType == 4)
		{
			float value = timer;
			value /= 4f;
			value = Math.Min(value, (60 - timer) * 0.3f) / 3;
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Vector2 starPos = Projectile.Center - Main.screenPosition + mainAxisDirection.RotatedBy(Projectile.spriteDirection * 0.175f) * 0.35f;
			Lighting.AddLight(Projectile.Center + mainAxisDirection * 0.35f, new Vector3(0.1f, 0.36f, 0.24f) * value * 10);
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.1f, 1f, 0.6f, 0f), 0, star.Size() / 2f, new Vector2(0.4f, 0.5f) * value, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.1f, 1f, 0.6f, 0f), MathHelper.PiOver2, star.Size() / 2f, new Vector2(0.6f, 0.9f) * value, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void Attack()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		Tplayer.HideLeg = true;
		if (Main.myPlayer == Projectile.owner && Main.mouseRight && Main.mouseRightRelease)
		{
		}

		useSlash = true;

		Vector2 vToMouse = Main.MouseWorld - player.Top;
		float AddHeadRotation = (float)Math.Atan2(vToMouse.Y, vToMouse.X) + (1 - player.direction) * 1.57f;
		if (player.gravDir == -1)
		{
			if (player.direction == -1)
			{
				if (AddHeadRotation >= 0.57f && AddHeadRotation < 2)
				{
					AddHeadRotation = 0.57f;
				}
			}
			else
			{
				if (AddHeadRotation <= -0.57f)
				{
					AddHeadRotation = -0.57f;
				}
			}
		}
		else
		{
			if (player.direction == -1)
			{
				if (AddHeadRotation >= 2 && AddHeadRotation < 5.71f)
				{
					AddHeadRotation = 5.71f;
				}
			}
			else
			{
				if (AddHeadRotation >= 0.57f)
				{
					AddHeadRotation = 0.57f;
				}
			}
		}

		if (currantAttackType <= 2)
		{
			if (timer < 30)
			{
				Projectile.ai[0] = GetAngToMouse();
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(90, targetRot, -0.75f, Projectile.ai[0]), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 8)
			{
				AttSound(new SoundStyle(
			Commons.ModAsset.TrueMeleeSwing_Mod));
			}

			if (timer > 30 && timer < 45)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.0f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.35f;
				mainAxisDirection = Vector2Elipse(100, Projectile.rotation, -0.75f, Projectile.ai[0]);
			}
			if (timer > 50)
			{
				NextAttackType();
			}
			else if (timer > 1)
			{
				float BodyRotation = (float)Math.Sin((timer - 10) / 30d * Math.PI) * 0.2f * player.direction * player.gravDir;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
				Tplayer.HeadRotation = -BodyRotation;
			}
		}
		if (currantAttackType == 3)
		{
			if (timer < 60)
			{
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(120, targetRot, 0, 0, 1000), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}

			if (timer % 10 == 0 && timer > 60)
			{
				AttSound(new SoundStyle(Commons.ModAsset.TrueMeleeSwing_Mod));
			}

			if (timer > 60 && timer < 90)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.0f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.425f;
				mainAxisDirection = Vector2Elipse(120, Projectile.rotation, 0, 0, 1000);
			}
			if (timer > 100)
			{
				NextAttackType();
			}
			else if (timer > 1)
			{
				float BodyRotation = (float)Math.Sin((timer - 10) / 30d * Math.PI) * 0.2f * player.direction * player.gravDir;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
				Tplayer.HeadRotation = -BodyRotation;
			}
			if (timer % 2 == 0 && timer >= 60 && Main.rand.NextBool(4))
			{
				Vector2 pos = player.Center + mainAxisDirection * Main.rand.NextFloat(0.15f, 0.25f);
				Vector2 v = mainAxisDirection * Main.rand.NextFloat(0.01f, 0.015f);
				int type = Main.rand.Next(567, 572);

				Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), pos, v, type, Projectile.damage / 4, Projectile.knockBack * 0.25f, Projectile.owner);
				p.timeLeft = (int)(p.timeLeft * 1.5f);
			}
		}
		if (currantAttackType == 4)
		{
			float BodyRotation;
			if (timer < 60)
			{
				ignoreTile = true;
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(120, targetRot, -1.2f), 0.1f);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();

			}

			if (timer == 65)
			{
				AttSound(new SoundStyle(Commons.ModAsset.TrueMeleePowerSwing_Mod));
			}

			if (timer > 60)
			{
				canHit = true;
				drawScaleFactor = 0.6f;
				if (timer < 75)
				{
					canHit = true;
					mainAxisDirection = Vector2Elipse(200, Projectile.rotation, -1.2f, Projectile.ai[0], 1000);
					Projectile.rotation += Projectile.spriteDirection * 0.42f;
				}

				BodyRotation = (float)Math.Sin((timer - 114.514) / 18d * Math.PI) * 0.7f * player.direction * player.gravDir;
			}
			else
			{
				Vector2 ToMouseWorld = Main.MouseWorld - player.Top;
				BodyRotation = -timer * player.direction * 0.003f * player.gravDir;
			}
			player.fullRotation = BodyRotation;
			player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
			player.legRotation = -BodyRotation;
			player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
			Tplayer.HeadRotation = -BodyRotation + AddHeadRotation;

			if (timer > 90)
			{
				End();
			}
		}
		if (canHit)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(20, 20) + mainAxisDirection * Main.rand.NextFloat(0.5f, 1.25f), 100, 100, ModContent.DustType<YggdrasilCyatheaLeafDust>(), 0, 0, 0, default, Main.rand.NextFloat(1f, 2f));
				d.velocity += player.velocity * 0.5f + Main.rand.NextVector2Unit() * 3;
				d.noGravity = true;
			}
		}
	}

	public override void DrawTrail(Color color)
	{
		base.DrawTrail(color);
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(slashTrail.ToList());//平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count - 1; x++)
		{
			SmoothTrail.Add(smoothTrail_current[x]);
		}
		if (slashTrail.Count != 0)
			SmoothTrail.Add(slashTrail.ToArray()[slashTrail.Count - 1]);

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = TrailAlpha(factor);
			if (!longHandle)
			{
				bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.15f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(Projectile.Center + trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w)));
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.2f * Projectile.scale, Color.White, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(Projectile.Center + trail[i] * Projectile.scale, Color.White, new Vector3(factor, 0, w)));
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Effect MeleeTrail = Commons.ModAsset.MeleeTrailFade.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		float k0 = timer / 80f + 0.1f;
		MeleeTrail.Parameters["FadeValue"].SetValue(MathF.Sqrt(k0 * 1.2f));
		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes[ShaderTypeName].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		Tplayer.HeadRotation = 0;
		Tplayer.HideLeg = false;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
		player.AddBuff(ModContent.BuffType<MossCover>(), 180);
		if (Main.rand.NextBool(8))
		{
			target.AddBuff(BuffID.Poisoned, 420);
		}

		base.OnHitNPC(target, hit, damageDone);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{

		if (currantAttackType == 4)
		{
			modifiers.FinalDamage *= 2f;

			target.AddBuff(BuffID.Poisoned, 420);
		}
	}
}
