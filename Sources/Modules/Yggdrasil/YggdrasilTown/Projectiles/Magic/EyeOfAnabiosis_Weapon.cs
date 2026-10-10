using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class EyeOfAnabiosis_Weapon : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public const int MaxChargeTime = 720;
	private const int MaxTargetCount = 3;
	private const float ProjectileRandomRotation = 0.5f;
	public Vector2 HangingItemPos;

	public bool CanDisplay { get; set; } = true;

	public int ChargeTimer { get; set; }

	public bool HasCreateMatrix { get; set; } = false;

	public bool HasCreateMagicCircle { get; set; } = false;

	public override string Texture => ModAsset.EyeOfAnabiosis_Mod;

	private Player Owner => Main.player[Projectile.owner];

	private Item HeldItem => Main.mouseItem.IsAir ? Owner.HeldItem : Main.mouseItem;

	private Vector2 OwnerMouseWorld => Owner.MouseWorld();

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 72;
		Projectile.height = 66;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 2;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;

		ChargeTimer = 0;
	}

	public override void AI()
	{
		if (Main.time % Main.projFrames[Type] == 0)
		{
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
		}

		KillHoldout();
		ManageHoldout();
		HoldoutAI();
		UpdateRotationSuspension();
		UpdateVFX();
	}

	private void UpdateVFX()
	{
		if (!CanDisplay)
		{
			return;
		}
		if (ChargeTimer++ < MaxChargeTime)
		{
			return;
		}
		float size = Main.rand.NextFloat(0.3f, 0.5f);
		var acytaeaFlame = new AnabiosisFlameDust_OverPlayer
		{
			Velocity = new Vector2(0, -Main.rand.NextFloat(1, 1.2f)) + Owner.velocity * 0.85f,
			Active = true,
			Visible = true,
			Position = HangingItemPos,
			MaxTime = Main.rand.Next(24, 36),
			Scale = 25f * size,
			Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			Frame = Main.rand.Next(3),
			ai = new float[] { Projectile.Center.X, Main.rand.NextFloat(-0.8f, 0.8f) },
		};
		Ins.VFXManager.Add(acytaeaFlame);
	}

	private void KillHoldout()
	{
		if (Owner == null
			|| !Owner.active
			|| Owner.dead
			|| Owner.CCed
			|| Owner.noItems)
		{
			Projectile.Kill();
			return;
		}

		if (HeldItem.type == ModContent.ItemType<EyeOfAnabiosis>())
		{
			Projectile.timeLeft = 60;
			CanDisplay = true;
		}
		else
		{
			CanDisplay = false;
		}
	}

	private void ManageHoldout()
	{
		if (CanDisplay)
		{
			Owner.heldProj = Projectile.whoAmI;
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathF.PI + Owner.direction * MathF.PI * 2 / 3);
		}

		Projectile.Center = Owner.MountedCenter;
		Projectile.velocity *= 0;
	}

	public float Suspension_Rotation;
	public float Suspension_Omega;

	private void HoldoutAI()
	{
		if (!CanDisplay)
		{
			return;
		}

		if (ChargeTimer++ < MaxChargeTime)
		{
			return;
		}

		if (!HasCreateMatrix)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Owner.Center, Vector2.zeroVector, ModContent.ProjectileType<EyeOfAnabiosis_Matrix>(), 0, 0, Projectile.owner);
			HasCreateMatrix = true;
		}
		if (!HasCreateMagicCircle)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Owner.Center, Vector2.zeroVector, ModContent.ProjectileType<EyeOfAnabiosis_MagicCircleFront>(), 0, 0, Projectile.owner);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Owner.Center, Vector2.zeroVector, ModContent.ProjectileType<EyeOfAnabiosis_MagicCircleBack>(), 0, 0, Projectile.owner);
			HasCreateMagicCircle = true;
		}

		if (!Owner.controlUseItem)
		{
			return;
		}

		if (Owner.itemTime == 0)
		{
			bool manaCostPaid = Owner.CheckMana(HeldItem, pay: true);
			if (manaCostPaid)
			{
				Vector2 projVelocity = Vector2.Normalize(OwnerMouseWorld - HangingItemPos) * HeldItem.shootSpeed;

				for (int i = 0; i < MaxTargetCount; i++)
				{
					var proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(HeldItem), HangingItemPos, projVelocity.RotatedBy(Main.rand.NextFloat(-ProjectileRandomRotation, ProjectileRandomRotation)), ModContent.ProjectileType<EyeOfAnabiosis_Projectile>(), HeldItem.damage, HeldItem.knockBack, Projectile.owner);
					proj.frame = Main.rand.Next(4);
				}

				SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, Projectile.Center);
				Owner.ItemCheck_ApplyManaRegenDelay(HeldItem);
				Owner.itemTime = HeldItem.useTime;
			}
		}

		Owner.direction = (OwnerMouseWorld - Owner.MountedCenter).X < 0 ? -1 : 1;
	}

	private void UpdateRotationSuspension()
	{
		if (MathF.Abs(Suspension_Rotation) < 1.5f)
		{
			Suspension_Omega += 0.01f * (MathF.Sin((float)Main.timeForVisualEffects * 0.05f) + 0.75f) * Main.windSpeedCurrent + Owner.velocity.X * 0.006f;
			Suspension_Omega -= Suspension_Rotation * 0.04f;
			Suspension_Omega *= 0.99f;
			Suspension_Rotation += Suspension_Omega;
		}
		else
		{
			Suspension_Omega -= Suspension_Rotation * 0.04f;
			Suspension_Omega *= 0.99f;
			Suspension_Rotation += Suspension_Omega;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (!CanDisplay)
		{
			return false;
		}

		float chargeProgress = (float)ChargeTimer / MaxChargeTime > 1f ? 1f : (float)ChargeTimer / MaxChargeTime;
		lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));

		var body_texture = ModAsset.EyeOfAnabiosis_Body.Value;
		var body_position_offset = new Vector2(Owner.direction * (body_texture.Width / 2 - 12), Owner.gravDir * (-body_texture.Height / 2 + 16)) * Projectile.scale;
		var body_position = Projectile.Center - Main.screenPosition + body_position_offset;
		var body_rotation = Owner.direction == 1 ? 0f : MathF.PI;
		var body_effects = (Owner.direction == 1 && Owner.gravDir == 1) || (Owner.gravDir == -1 && Owner.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipVertically;
		var body_origin = body_texture.Size() / 2;

		Main.spriteBatch.Draw(body_texture, body_position, null, lightColor, body_rotation, body_origin, Projectile.scale, body_effects, 0);

		var head_texture = ModAsset.EyeOfAnabiosis_Head.Value;
		var head_position_offset = new Vector2(Owner.direction * 23, Owner.gravDir * -10) * Projectile.scale;
		var head_position = body_position + head_position_offset;
		var head_rotation = Owner.gravDir == 1 ? 0 : MathF.PI;
		{
			head_rotation += Suspension_Rotation; // 0.1f * (MathF.Sin((float)Main.timeForVisualEffects * 0.05f) + 0.75f) * Main.windSpeedCurrent + Owner.velocity.X * 0.1f;
		}
		var head_effects = (Owner.direction == 1 && Owner.gravDir == 1) || (Owner.gravDir == -1 && Owner.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		var head_rope_texture = ModAsset.EyeOfAnabiosis_Rope.Value;
		var head_rope_origin = new Vector2(head_rope_texture.Width / 2, 0);
		var head_ball_texture = ModAsset.EyeOfAnabiosis_Head_Ball.Value;
		var head_ball_origin = head_rope_origin + new Vector2((head_ball_texture.Width - head_rope_texture.Width) / 2, -head_rope_texture.Height + 2);
		var head_ballbg_texture = ModAsset.EyeOfAnabiosis_Head_BallBG.Value;
		var head_ballbg_origin = head_ball_origin + new Vector2((head_ballbg_texture.Width - head_ball_texture.Width) / 2, (head_ballbg_texture.Height - head_ball_texture.Height) / 2);

		var head_container_texture = ModAsset.EyeOfAnabiosis_Head_Container.Value;
		var head_container_position_offset = (-head_ballbg_origin + new Vector2(head_ball_texture.Width + (head_container_texture.Width - head_ball_texture.Width) / 2) / 2).RotatedBy(head_rotation) * Projectile.scale;
		var head_container_position = head_position + head_container_position_offset;

		// shoot proj from here.
		HangingItemPos = head_container_position + Main.screenPosition;
		var head_container_origin = new Vector2(head_container_texture.Width / 2, 0);
		var head_container_rotation = 0.3f * head_rotation;

		var fireProgress = 0.2f + 0.8f * chargeProgress;
		var head_fire_texture = ModAsset.EyeOfAnabiosis_Projectile.Value;
		var head_fire_origin = new Vector2(head_fire_texture.Width / 8, head_fire_texture.Height * (1.5f - 1 / fireProgress) / 2);
		var frame = head_fire_texture.Frame(horizontalFrames: Main.projFrames[Type], frameX: Projectile.frame);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// Draw charge progress UI
		if (Main.myPlayer == Projectile.owner && chargeProgress < 1)
		{
			var progressTexture = Commons.ModAsset.White.Value;
			var progressPosition = Owner.Center - Main.screenPosition + Owner.gravDir * new Vector2(0, 36);

			var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.9f);
			var frameColor2 = new Color(0.15f, 0.25f, 0.38f, 0.4f);
			Vector2 frameScale = new Vector2(2f, 0.4f) * 0.05f;
			Vector2 frameScale2 = new Vector2(1.8f, 0.2f) * 0.05f;

			var lineColor = Color.Lerp(new Color(0f, 0.3f, 0.7f, 0.8f), new Color(0.7f, 1f, 0.8f, 0.8f), chargeProgress);
			var lineColorInner = Color.Lerp(new Color(0f, 0f, 0.6f, 0.9f), new Color(0.3f, 0.8f, 0.8f, 0.9f), chargeProgress);
			Vector2 lineScaleOuter = new Vector2(2.2f * chargeProgress + 0.2f, 0.7f) * 0.05f;
			Vector2 lineScale = new Vector2(2.2f * chargeProgress, 0.5f) * 0.05f;
			Vector2 lineScale2 = new Vector2(2.2f * chargeProgress - 0.2f, 0.3f) * 0.05f;
			var linePositionOffset = new Vector2(-2.2f * (1 - chargeProgress) * progressTexture.Width * 0.025f, 0);

			Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor, 0, progressTexture.Size() / 2, frameScale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor2, 0, progressTexture.Size() / 2f, frameScale2, SpriteEffects.None, 0);

			Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, frameColor, 0, progressTexture.Size() / 2, lineScaleOuter, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColor, 0, progressTexture.Size() / 2, lineScale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColorInner, 0, progressTexture.Size() / 2, lineScale2, SpriteEffects.None, 0);
		}

		Main.spriteBatch.Draw(head_ball_texture, head_position, null, lightColor, head_rotation, head_ball_origin, Projectile.scale, head_effects, 0);
		Main.spriteBatch.Draw(head_rope_texture, head_position, null, lightColor, head_rotation, head_rope_origin, Projectile.scale, head_effects, 0);

		Effect shineEffect = ModAsset.EyeOfAnabiosis_Shine.Value;
		shineEffect.Parameters["uImageSize"].SetValue(head_texture.Size());
		shineEffect.Parameters["uChargeProgress"].SetValue(chargeProgress);
		shineEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);
		shineEffect.CurrentTechnique.Passes["Ball_Pixel"].Apply();
		Main.spriteBatch.Draw(head_ballbg_texture, head_position, null, lightColor, head_rotation, head_ballbg_origin, Projectile.scale, head_effects, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Main.spriteBatch.Draw(head_fire_texture, head_container_position, frame, lightColor, head_container_rotation, head_fire_origin, Projectile.scale * fireProgress, head_effects, 0);
		Main.spriteBatch.Draw(head_container_texture, head_container_position, null, lightColor, head_container_rotation, head_container_origin, Projectile.scale, head_effects, 0);

		var lightPosition = head_container_position + Main.screenPosition - head_ball_texture.Size() * Projectile.scale / 4;
		Lighting.AddLight(lightPosition, new Vector3(0.05f, 0.3f, 0.9f) * chargeProgress);
		return false;
	}
}
