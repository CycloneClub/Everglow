using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class AncientPhantom_Bow : Fighter
{
	public float HurtValue = 0f;

	public bool CanShoot = false;

	public int ShootTimer = 60;

	public float AimAngle = 0f;

	public float SafeShootDistance => 430 + MathF.Sin(NPC.whoAmI) * 160;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
	}

	public override void SetDefaults()
	{
		NPC.width = 16;
		NPC.height = 40;
		NPC.lifeMax = 110;
		NPC.damage = 25;
		NPC.defense = 20;
		NPC.friendly = false;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.Item53;
		NPC.DeathSound = SoundID.Shatter;
		NPC.alpha = 100;

		JumpScale = 1f;
		MaxSpeedX = 2.8f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void FindFrame(int frameHeight)
	{
		if (NPC.velocity.Y == 0f)
		{
			NPC.spriteDirection = NPC.direction;
		}
		else if (NPC.velocity.Y < 0f)
		{
			NPC.frameCounter = 0;
		}

		NPC.frameCounter += 1;
		int frameNumber = (int)(NPC.frameCounter / 3);
		if (frameNumber >= 9)
		{
			NPC.frameCounter = 0;
			frameNumber = 0;
		}
		if (!NPC.collideY)
		{
			frameNumber = 11;
		}
		if (CanShoot)
		{
			frameNumber = 10;
		}
		NPC.frame.Y = frameNumber * frameHeight;
		if (HurtValue > 0)
		{
			HurtValue -= 2f;
		}
		else
		{
			HurtValue = 0f;
		}
	}

	public override void AI()
	{
		if (!CanShoot)
		{
			base.AI();
		}
		else
		{
			if (NPC.target < 0)
			{
				CanShoot = false;
				return;
			}
			NPC.FaceTarget();
			NPC.velocity.X = 0f;
			Vector2 shootPos = NPC.Center + new Vector2(NPC.direction * 12, -9) + new Vector2(-10, 0).RotatedBy(AimAngle);
			Vector2 shootVel = FindBestShootAngle(shootPos, Main.player[NPC.target].MountedCenter);
			AimAngle = shootVel.ToRotation() + MathHelper.Pi;
			if (ShootTimer > 0)
			{
				ShootTimer--;
			}
			else
			{
				ShootTimer = 60;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), shootPos, shootVel, ModContent.ProjectileType<AncientPhantom_Bow_Arrow>(), 20, 0f, Main.myPlayer);

				float dis_X = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
				if (dis_X <= SafeShootDistance || dis_X >= SafeShootDistance + 160)
				{
					CanShoot = false;
					ShootTimer = 60;
				}
			}
		}
		Lighting.AddLight(NPC.Bottom, new Vector3(0, 0.24f, 0.48f));
	}

	public Vector2 FindBestShootAngle(Vector2 shootPos, Vector2 targetPos)
	{
		var arrowPos = shootPos;
		float predictY = -300;
		List<Vector2> oldPredict = new List<Vector2>();
		float initial_MinDis = CalculateMinDis(shootPos, targetPos, 100);
		oldPredict.Add(new Vector2(100, initial_MinDis));
		for (int h = 0; h < 200; h++)
		{
			float minDis = CalculateMinDis(shootPos, targetPos, predictY);
			oldPredict.Add(new Vector2(predictY, minDis));
			oldPredict.Sort((a, b) => a.Y.CompareTo(b.Y));
			if (minDis < 7)
			{
				Vector2 shootVel = targetPos + new Vector2(0, predictY) - shootPos;
				shootVel.Normalize();
				return shootVel * 13;
			}
			predictY = Math.Abs(oldPredict[1].X - oldPredict[0].X) < 4 && oldPredict[0].Y > 7
				? oldPredict[0].X + Math.Sign(oldPredict[0].X - oldPredict[1].X) * 10
				: (oldPredict[0].X + oldPredict[1].X) * 0.5f;
		}
		return (targetPos + new Vector2(0, oldPredict[0].X) - shootPos).NormalizeSafe() * 13f;
	}

	public float CalculateMinDis(Vector2 shootPos, Vector2 targetPos, float predictY)
	{
		Vector2 shootVel = targetPos + new Vector2(0, predictY) - shootPos;
		shootVel.Normalize();
		shootVel *= 13f;
		float minDis = 1000;
		float prevDis = 1000;
		var arrowPos = shootPos;
		for (int k = 0; k < 120; k++)
		{
			shootVel.Y += 0.1f;
			if (shootVel.Length() > 20f)
			{
				shootVel = Vector2.Normalize(shootVel) * 20f;
			}
			arrowPos += shootVel;
			float dis = (arrowPos - targetPos).Length();
			if (dis < minDis)
			{
				minDis = dis;
			}
			prevDis = dis;
		}
		return minDis;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		HurtValue = 60f;
		base.HitEffect(hit);
	}

	public override void Fighters_FindTarget()
	{
		Player player = Main.player[NPC.target];
		if (NPC.ai[3] < AggroThreshold)
		{
			NPC.TargetClosest(true);
			if (NPC.target < 0)
			{
				NPC.velocity.X *= 0;
				return;
			}
			if (NPC.directionY > 0 && player.Center.Y <= NPC.Bottom.Y)
			{
				NPC.directionY = -1;
			}
		}
		else if (NPC.ai[2] <= 0f)
		{
			if (TendToDespawn)
			{
				NPC.EncourageDespawn(10);
			}
			if (NPC.velocity.X == 0f)
			{
				if (NPC.velocity.Y == 0f)
				{
					NPC.ai[0] += 1f;
					if (NPC.ai[0] >= 2f)
					{
						NPC.direction *= -1;
						NPC.spriteDirection = NPC.direction;
						NPC.ai[0] = 0f;
					}
				}
			}
			else
			{
				NPC.ai[0] = 0f;
			}
			if (NPC.direction == 0)
			{
				NPC.direction = 1;
			}
		}

		float dis_X = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
		float dis_X_next = Math.Abs(NPC.Center.X + NPC.direction * 5 - Main.player[NPC.target].Center.X);
		if (dis_X <= SafeShootDistance && dis_X_next < dis_X)
		{
			NPC.direction *= -1;
		}
	}

	public override void Fighters_Accelerate()
	{
		if (NPC.velocity.X < -MaxSpeedX || NPC.velocity.X > MaxSpeedX)
		{
			if (NPC.velocity.Y == 0f)
			{
				NPC.velocity *= 0.8f;
			}
		}
		else if (NPC.velocity.X < MaxSpeedX && NPC.direction == 1)
		{
			NPC.velocity.X += 0.07f;
			if (NPC.velocity.X > MaxSpeedX)
			{
				NPC.velocity.X = MaxSpeedX;
			}
		}
		else if (NPC.velocity.X > -MaxSpeedX && NPC.direction == -1)
		{
			NPC.velocity.X -= 0.07f;
			if (NPC.velocity.X < -MaxSpeedX)
			{
				NPC.velocity.X = -MaxSpeedX;
			}
		}
		float dis_X = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
		if (dis_X > SafeShootDistance && dis_X < SafeShootDistance + 160 && NPC.velocity.Y == 0)
		{
			CanShoot = true;
		}
	}

	public override void Fighters_Jump(int pushTileX, int pushTileY, bool canJump) => base.Fighters_Jump(pushTileX, pushTileY, canJump);

	public override void OnKill()
	{
		for (int h = 0; h < 40; h++)
		{
			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<TwilightCrystalDust>());
			dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
		}
		for (int h = 0; h < 16; h++)
		{
			Vector2 velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + velocity * 6,
				MaxTime = Main.rand.Next(24, 42),
				Scale = Main.rand.NextFloat(3, 12),
				Rotation = velocity.ToRotation() - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects flip = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		float fade = (255 - NPC.alpha) / 255f;
		Texture2D body_glow = ModAsset.AncientPhantom_Bow_glow.Value;
		spriteBatch.Draw(body_glow, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, Color.White, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);

		Texture2D bow = ModAsset.AncientPhantom_Bow_Bow.Value;
		Texture2D bow_glow = ModAsset.AncientPhantom_Bow_Bow_glow.Value;
		float bowRot = -MathHelper.PiOver2 - 0.7f * NPC.spriteDirection;
		Vector2 bowOffset = new Vector2(8 * NPC.spriteDirection, NPC.gfxOffY);
		if (CanShoot)
		{
			bowRot = AimAngle;
			bowOffset = new Vector2(12 * NPC.spriteDirection, NPC.gfxOffY - 11);
		}

		Rectangle bowFrame = new Rectangle(0, 62 * (int)((60 - ShootTimer) / 60f * 4), bow.Width, 62);
		spriteBatch.Draw(bow, NPC.Center + bowOffset - screenPos, bowFrame, drawColor * fade, bowRot, bowFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
		if (CanShoot)
		{
			spriteBatch.Draw(bow_glow, NPC.Center + bowOffset - screenPos, bowFrame, new Color(1f, 1f, 1f, 0) * ((60 - ShootTimer) / 60f), bowRot, bowFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
		}

		Texture2D body_Shape = ModAsset.AncientPhantom_Bow_Shape.Value;
		Color hurtColor = Color.Lerp(Color.Blue, Color.White, HurtValue / 60f);
		hurtColor *= HurtValue / 60f;

		spriteBatch.Draw(body_Shape, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, hurtColor, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
