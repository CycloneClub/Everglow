using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class MeatLantern_Proj : MeleeProj
{
	public override void SetDef()
	{
		maxAttackType = 3;
		maxSlashTrailLength = 20;
		longHandle = true;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		;
		autoEnd = false;
		canLongLeftClick = true;
	}

	public override string TrailColorTex()
	{
		return ModAsset.MeatLantern_Color_Mod;
	}

	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 2f;
	}
	public override BlendState TrailBlendState()
	{
		return BlendState.Additive;
	}
	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = new Vector4(), Vector2 drawScale = new Vector2(), Texture2D glowTexture = null)
	{
		if (diagonal == new Vector4())
		{
			diagonal = new Vector4(0, 1, 1, 0);
		}
		if (drawScale == new Vector2())
		{
			drawScale = new Vector2(0, 1);
			if (longHandle)
			{
				drawScale = new Vector2(-0.6f, 1);
			}
			drawScale *= drawScaleFactor;
		}
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}
	public void DrawVertexByTwoLine(Texture2D texture, Color drawColor, Vector2 textureCoordStart, Vector2 textureCoordEnd, Vector2 positionStart, Vector2 positionEnd)
	{
		Vector2 coordVector = textureCoordEnd - textureCoordStart;
		coordVector.X *= texture.Width;
		coordVector.Y *= texture.Height;
		float theta = MathF.Atan2(coordVector.Y, coordVector.X);
		Vector2 drawVector = positionEnd - positionStart;

		Vector2 mainVectorI = drawVector.RotatedBy(theta * -Projectile.spriteDirection) * MathF.Cos(theta);
		Vector2 mainVectorJ = drawVector.RotatedBy((theta - MathHelper.PiOver2) * -Projectile.spriteDirection) * MathF.Sin(theta);

		if (currantAttackType == 1 || currantAttackType == 3)
		{
			mainVectorI = drawVector.RotatedBy(theta * +Projectile.spriteDirection) * MathF.Cos(theta);
			mainVectorJ = drawVector.RotatedBy((theta - MathHelper.PiOver2) * +Projectile.spriteDirection) * MathF.Sin(theta);
		}
		List<Vertex2D> vertex2Ds = new List<Vertex2D>
		{
			new Vertex2D(positionStart, drawColor, new Vector3(textureCoordStart, 0)),
			new Vertex2D(positionStart + mainVectorI, drawColor, new Vector3(textureCoordEnd.X, textureCoordStart.Y, 0)),

			new Vertex2D(positionStart + mainVectorJ, drawColor, new Vector3(textureCoordStart.X, textureCoordEnd.Y, 0)),
			new Vertex2D(positionEnd, drawColor, new Vector3(textureCoordEnd, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
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
					AddHeadRotation = 0.57f;
			}
			else
			{
				if (AddHeadRotation <= -0.57f)
					AddHeadRotation = -0.57f;
			}
		}
		else
		{
			if (player.direction == -1)
			{
				if (AddHeadRotation >= 2 && AddHeadRotation < 5.71f)
					AddHeadRotation = 5.71f;
			}
			else
			{
				if (AddHeadRotation >= 0.57f)
					AddHeadRotation = 0.57f;
			}
		}

		if (currantAttackType == 0)
		{
			LockPlayerDir(Player);
			float rot = player.direction;
			if (timer < 20)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(60, targetRot, -0.75f, rot), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 8)
			{
				AttSound(new SoundStyle(
			Commons.ModAsset.TrueMeleeSwing_Mod));
			}

			if (timer > 20 && timer < 35)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.36f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation -= Projectile.spriteDirection * 0.25f;
				mainAxisDirection = Vector2Elipse(75, Projectile.rotation, -0.75f, rot);
			}
			if (timer > 40)
				NextAttackType();
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
		if (currantAttackType == 1)
		{
			LockPlayerDir(Player);
			float rot = -player.direction;
			if (timer < 20)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(60, targetRot, -0.75f, rot), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 8)
			{
				AttSound(new SoundStyle(
			Commons.ModAsset.TrueMeleeSwing_Mod));
			}

			if (timer > 20 && timer < 35)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.36f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.25f;
				mainAxisDirection = Vector2Elipse(75, Projectile.rotation, -0.75f, rot);
			}
			if (timer > 40)
				NextAttackType();
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
		if (currantAttackType == 2)
		{
			LockPlayerDir(Player);
			if (timer < 20)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 + Player.direction * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(60, targetRot, -1.2f), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 8)
			{
				AttSound(new SoundStyle(
			Commons.ModAsset.TrueMeleeSwing_Mod));
			}
			if (timer % 10 == 8 && timer > 30)
				SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
			if (timer > 20 && timer < 75)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.36f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation -= Projectile.spriteDirection * 0.4f;
				mainAxisDirection = Vector2Elipse(90, Projectile.rotation, -1.2f, 0, 1000);
			}
			if (timer > 80)
			{
				if (player.statLife <= player.statLifeMax2 * 0.5f)
				{
					NextAttackType();
				}
				else
				{
					End();
				}
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
			LockPlayerDir(Player);
			float rot = -player.direction;
			if (timer < 20)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 2.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(80, targetRot, 0, rot), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == 8)
			{
				AttSound(new SoundStyle(
			Commons.ModAsset.TrueMeleeSwing_Mod));
			}

			if (timer > 20 && timer < 30)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.36f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.2f;
				mainAxisDirection = Vector2Elipse(80, Projectile.rotation, 0, rot);
			}
			if (timer > 60 && timer < 80)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.36f, 0.36f, 0.24f);
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.15f;
				mainAxisDirection = Vector2Elipse(80, Projectile.rotation, 0, rot);
			}
			if (timer > 80)
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
		if (canHit)
		{
			Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(20, 20) + mainAxisDirection * Main.rand.NextFloat(0.3f, 1f), 40, 40, ModContent.DustType<MeatLanternDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1.5f));
			d.velocity += player.velocity * 0.3f + Main.rand.NextVector2Unit() * 2;
			d.noGravity = true;
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
			return;
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

		if (currantAttackType == 3)
		{
			player.HealLife(15);
		}
		else
		{
			player.HealLife(1);
		}
		for (int i = 0; i < 60; i++)
		{
			Vector2 v = Vector2.One.RotatedByRandom(MathF.PI * 2) * 2;
			Dust d = Dust.NewDustDirect(target.Center + v * 5, 40, 40, ModContent.DustType<MeatLanternDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1.5f));
			d.velocity = target.velocity * 0.3f + v;
			d.noGravity = true;
		}

	}
}

