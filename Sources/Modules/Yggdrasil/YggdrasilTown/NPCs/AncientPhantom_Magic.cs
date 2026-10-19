using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.ProjNPC;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class AncientPhantom_Magic : Fighter
{
	public float HurtValue = 0f;

	public bool CanShoot = false;

	public int ShootTimer = 90;

	public int MaxShootTime = 90;

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
		MaxSpeedX = 2.1f;
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
			if (ShootTimer > 0)
			{
				ShootTimer--;
				if (ShootTimer > 48)
				{
					for (int h = 0; h < 2; h++)
					{
						var dustVFX = new TwilightCrystalVFXDust_Charge
						{
							Owner = NPC,
							Velocity = new Vector2(0, 0),
							Active = true,
							Visible = true,
							Position = NPC.Center + new Vector2(48, 0).RotatedByRandom(MathHelper.TwoPi),
							MaxTime = 120,
							Scale = 0,
							Rotation = NPC.velocity.ToRotation() - MathHelper.PiOver4,
							ai = new float[] { 0, 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
				}
			}
			else
			{
				ShootTimer = MaxShootTime;
				Vector2 shootPos = NPC.Center + new Vector2(NPC.direction * 9, 0);
				Vector2 shootVel = Main.player[NPC.target].MountedCenter - shootPos;
				shootVel.Normalize();
				shootVel *= 8f;
				NPC orb = NPC.NewNPCDirect(NPC.GetSource_FromAI(), shootPos, ModContent.NPCType<AncientPhantom_Magic_Orb>());
				orb.velocity = shootVel;

				float dis_X = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
				if (dis_X <= SafeShootDistance || dis_X >= SafeShootDistance + 160)
				{
					CanShoot = false;
					ShootTimer = MaxShootTime;
				}

				for (int i = 0; i < 30; i++)
				{
					Vector2 vel = new Vector2(10 + (i % 2) * 2, 0).RotatedBy(MathHelper.TwoPi / 30f * i) * 0.5f;
					vel.Y *= 0.3f;
					vel = vel.RotatedBy(shootVel.ToRotationSafe() + MathHelper.PiOver2);
					var dustVFX = new TwilightCrystalVFXDust
					{
						Velocity = vel,
						Active = true,
						Visible = true,
						Position = shootPos + shootVel * 3,
						MaxTime = Main.rand.Next(24, 35),
						Scale = Main.rand.NextFloat(3, 4),
						Rotation = vel.ToRotationSafe() - MathHelper.PiOver4,
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFX);
				}

				for (int i = 0; i < 20; i++)
				{
					Vector2 vel = new Vector2(10 + (i % 2) * 2, 0).RotatedBy(MathHelper.TwoPi / 20f * i) * 0.25f;
					vel.Y *= 0.3f;
					vel = vel.RotatedBy(shootVel.ToRotationSafe() + MathHelper.PiOver2);
					var dustVFX = new TwilightCrystalVFXDust
					{
						Velocity = vel,
						Active = true,
						Visible = true,
						Position = shootPos + shootVel * 6,
						MaxTime = Main.rand.Next(37, 55),
						Scale = Main.rand.NextFloat(3, 4),
						Rotation = vel.ToRotationSafe() - MathHelper.PiOver4,
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFX);
				}
			}
		}
		Lighting.AddLight(NPC.Bottom, new Vector3(0, 0.24f, 0.48f));
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

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		TwilightForsetAndRelic twlightBiome = ModContent.GetInstance<TwilightForsetAndRelic>();
		return !twlightBiome.IsBiomeActive(Main.LocalPlayer) ? 0f : 3f;
	}

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
		Texture2D body_glow = ModAsset.AncientPhantom_Magic_glow.Value;
		spriteBatch.Draw(body_glow, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, Color.White, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);

		if (CanShoot)
		{
			Vector2 shootPos = NPC.Center + new Vector2(NPC.direction * 9, 0);
			Texture2D orb = ModAsset.AncientPhantom_Magic_Orb.Value;
			float timer = ShootTimer / (float)MaxShootTime;
			spriteBatch.Draw(orb, shootPos + new Vector2(0, NPC.gfxOffY - 7) - screenPos, null, Color.White, (float)Main.time * 0.1f + NPC.whoAmI, orb.Size() * 0.5f, 1 - timer, flip, 0);
			if (ShootTimer % 6 == 0 && ShootTimer < 48f)
			{
				Texture2D star = Commons.ModAsset.StarSlash.Value;
				float size = Main.rand.NextFloat(0.2f, 0.5f);
				for (int t = 0; t < 3; t++)
				{
					Main.EntitySpriteDraw(star, shootPos + new Vector2(0, NPC.gfxOffY - 7) + new Vector2(10, 0).RotatedBy((float)Main.time * 0.1f + NPC.whoAmI) - screenPos, null, new Color(1f, 1f, 1f, 0) * size, t * MathHelper.TwoPi / 3f, star.Size() * 0.5f, size, SpriteEffects.None, 0);
				}
			}
		}
		Texture2D body_Shape = ModAsset.AncientPhantom_Magic_Shape.Value;
		Color hurtColor = Color.Lerp(Color.Blue, Color.White, HurtValue / 60f);
		hurtColor *= HurtValue / 60f;

		spriteBatch.Draw(body_Shape, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, hurtColor, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
