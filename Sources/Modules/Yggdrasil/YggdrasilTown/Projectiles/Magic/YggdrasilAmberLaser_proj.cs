using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class YggdrasilAmberLaser_proj : HandholdProjectile
{
	public override void SetDef()
	{
		DepartLength = 60;
		TextureRotation = 5 / 18f * MathHelper.Pi;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 5;
		Projectile.ArmorPenetration = 45;
		base.SetDef();
	}

	public Vector2 EndPoint = Vector2.Zero;
	public bool Crystalized = false;

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.Shimmer1.WithPitchOffset(1), Projectile.Center);
		Player player = Main.player[Projectile.owner];
		Vector2 mouseToPlayer = Main.MouseWorld - player.Center;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
		EndPoint = Vector2.Zero;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		HeldProjectileAI();
		Player player = Main.player[Projectile.owner];
		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		float duration = player.itemTime / (float)timeMax;
		duration *= 1.5f;
		duration -= 0.5f;
		if (duration < 0)
		{
			duration = 0;
		}
		duration = MathF.Sin(duration * MathHelper.Pi);
		if (duration < 0.3f)
		{
			return;
		}
		if (EndPoint != Vector2.Zero)
		{
			if (!Crystalized)
			{
				Crystalized = true;
				var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), EndPoint, Vector2.zeroVector, ModContent.ProjectileType<YggdrasilAmberLaser_crystal>(), Projectile.damage / 2, 0, Projectile.owner);
				var yALc = projectile.ModProjectile as YggdrasilAmberLaser_crystal;
				if (yALc != null)
				{
					yALc.LaserOwner = Projectile;
				}
			}
			for (int g = 0; g < 10; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 12f)).RotatedByRandom(MathHelper.TwoPi);
				var somg = new AmberFlameDust
				{
					velocity = newVelocity,
					Active = true,
					Visible = true,
					position = EndPoint,
					maxTime = Main.rand.Next(47, 85),
					scale = Main.rand.NextFloat(2.20f, 12.35f),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Player player = Main.player[Projectile.owner];
		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		float duration = player.itemTime / (float)timeMax;
		duration *= 1.5f;
		duration -= 0.5f;
		if (duration < 0)
		{
			duration = 0;
		}
		duration = MathF.Sin(duration * MathHelper.Pi);
		if (duration < 0.3f)
		{
			return false;
		}
		bool k = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), new Vector2(targetHitbox.Width, targetHitbox.Height), Projectile.Center, EndPoint);
		if (k)
		{
			for (int g = 0; g < 10; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
				var somg = new AmberFlameDust
				{
					velocity = newVelocity,
					Active = true,
					Visible = true,
					position = targetHitbox.Center(),
					maxTime = Main.rand.Next(37, 145),
					scale = Main.rand.NextFloat(1.20f, 12.35f),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
		}
		return k;
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<YggdrasilAmberLaser>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);

		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		if (Projectile.localNPCHitCooldown > timeMax - 1)
		{
			Projectile.localNPCHitCooldown = timeMax - 1;
		}
		if (player.itemTime == 1)
		{
			Projectile.Kill();
		}
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		DepartLength = 60;

		Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
		Projectile.timeLeft = timeMax;
		if (Projectile.Center.X < ArmRootPos.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		if (EndPoint != Vector2.zeroVector)
		{
			int duplicateTimes = 1;
			for (int i = 0; i < duplicateTimes; i++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0.20f, 2.35f)).RotatedByRandom(MathHelper.TwoPi);
				var somg = new LightFruitParticleDust
				{
					velocity = newVelocity,
					Active = true,
					Visible = true,
					position = EndPoint,
					maxTime = Main.rand.Next(37, 145),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.60f, 7.35f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			if (player.itemTime > 6)
			{
				for (int i = 0; i < 2; i++)
				{
					var d = Dust.NewDustDirect(EndPoint - new Vector2(4), 0, 0, ModContent.DustType<YggdrasilAmber_crack>());
					d.velocity = new Vector2(0, Main.rand.NextFloat(0.20f, 8.35f)).RotatedByRandom(MathHelper.TwoPi);
					d.scale *= 4f;
				}
			}
		}
	}

	public float MaxStep = 0;

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		var texMain_glow = ModAsset.YggdrasilAmberLaser_glow_proj.Value;
		float duration = player.itemTime / (float)player.itemTimeMax;
		duration *= 1.5f;
		duration -= 0.5f;
		if (duration < 0)
		{
			duration = 0;
		}
		duration = MathF.Sin(duration * MathHelper.Pi);

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + DrawOffset - new Vector2(54, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, lightColor, rot, texMain.Size() / 2f, 1f, se, 0);
		var powerColor = new Color(duration, duration * duration, duration * duration, 0);
		Main.spriteBatch.Draw(texMain_glow, Projectile.Center - Main.screenPosition + DrawOffset - new Vector2(54, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, powerColor, rot, texMain_glow.Size() / 2f, 1f, se, 0);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Vector2 mouseToPlayer = new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75);
		float timeValue = (float)Main.time * 0.06f;

		int step = -6;
		while (!Collision.SolidCollision(Projectile.Center + mouseToPlayer * step * 8, 0, 0))
		{
			step++;
			if (step > 200)
			{
				break;
			}
			Vector2 checkPoint = Projectile.Center + mouseToPlayer * step * 8;
			if (!Main.gamePaused && Main.rand.NextBool(30))
			{
				Vector2 newVelocity = mouseToPlayer.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 4f * duration;
				var somg = new AmberFlameDust
				{
					velocity = newVelocity,
					Active = true,
					Visible = true,
					position = checkPoint,
					maxTime = Main.rand.Next(37, 55) * duration,
					scale = Main.rand.NextFloat(1.20f, 4.35f) * duration,
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			MaxStep = step;
			EndPoint = Projectile.Center + MaxStep * mouseToPlayer * 8;
		}
		var bars = new List<Vertex2D>();
		step = -6;
		while (!Collision.SolidCollision(Projectile.Center + mouseToPlayer * step * 8, 0, 0))
		{
			step++;
			if (step > 200)
			{
				break;
			}
			Vector2 checkPoint = Projectile.Center + mouseToPlayer * step * 8;
			Vector2 toMouseLeft = mouseToPlayer.RotatedBy(MathHelper.PiOver2);
			float width = 25f;
			float duration2 = Math.Max(duration, 0);
			var drawColor = new Color(duration * 0.7f, duration2 * duration2 * 0.52f, 0, 0) * 0.1f;
			float mulWidth = 1f;
			if (step + 4 <= 10)
			{
				mulWidth = MathF.Pow((step + 4) / 10f, 0.3f);
			}
			bars.Add(checkPoint + toMouseLeft * width, drawColor, new Vector3(step * 0.03f - timeValue, 0, mulWidth));
			bars.Add(checkPoint - toMouseLeft * width, drawColor, new Vector3(step * 0.03f - timeValue, 1, mulWidth));
		}
		for (int k = 0; k < 24; k++)
		{
			if (bars.Count <= k * 2)
			{
				break;
			}
			Vertex2D v0 = bars[^(k * 2 + 1)];
			Vertex2D v1 = bars[^(k * 2 + 2)];
			float value = 24 - k;
			value /= 4f;
			value += 1f;
			v0 = new Vertex2D(v0.position, v0.color * value, v0.texCoord);
			v1 = new Vertex2D(v1.position, v1.color * value, v1.texCoord);
			bars[^(k * 2 + 1)] = v0;
			bars[^(k * 2 + 2)] = v1;
		}
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		var drawC = new Color(duration * 0.7f, duration * duration * 0.52f, duration * duration * 0.06f, 0);

		Vector2 starCenter = EndPoint - Main.screenPosition;

		Main.spriteBatch.Draw(star, starCenter, null, drawC, MathHelper.PiOver2 + timeValue, star.Size() / 2f, 1.9f * duration, se, 0);
		Main.spriteBatch.Draw(star, starCenter, null, drawC, 0 + timeValue, star.Size() / 2f, 1.9f * duration, se, 0);
		drawC = new Color(duration * 0.4f, duration * duration * 0.32f, duration * duration * 0.12f, 0);
		Main.spriteBatch.Draw(star, starCenter, null, drawC * 0.6f, MathHelper.PiOver4 + timeValue, star.Size() / 2f, 1.3f * duration, se, 0);
		Main.spriteBatch.Draw(star, starCenter, null, drawC * 0.6f, -MathHelper.PiOver4 + timeValue, star.Size() / 2f, 1.3f * duration, se, 0);
		return false;
	}
}
