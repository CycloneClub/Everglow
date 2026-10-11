using Everglow.Commons.Templates.Enemies;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class AncientPhantom_Sheild : Fighter
{
	public float HurtValue = 0f;

	public float ShieldValue = 0f;

	public int ShieldHealthMax = 4;

	public int ShieldHealth = 4;

	public bool HasShield => ShieldHealth > 0;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 11;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 30;
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

		JumpScale = 0.5f;
		MaxSpeedX = 1.4f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Main.expertMode)
		{
			ShieldHealthMax = ShieldHealth = 6;
		}
		if (Main.masterMode)
		{
			ShieldHealthMax = ShieldHealth = 8;
		}
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

		if (ShieldValue > 0)
		{
			ShieldValue -= 2f;
		}
		else
		{
			ShieldValue = 0f;
		}
	}

	public override void AI()
	{
		base.AI();
		if (!HasShield)
		{
			MaxSpeedX = 2.6f;
			JumpScale = 1f;
		}
		Lighting.AddLight(NPC.Bottom, new Vector3(0, 0.24f, 0.48f));
	}

	public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		HitInSheild(ref modifiers);
		base.ModifyHitByItem(player, item, ref modifiers);
	}

	public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		if (ShieldHealth > 0 && Math.Sign(projectile.Center.X - NPC.Center.X) == NPC.spriteDirection)
		{
			HitInSheild(ref modifiers);
		}
		base.ModifyHitByProjectile(projectile, ref modifiers);
	}

	public void HitInSheild(ref NPC.HitModifiers modifiers)
	{
		modifiers.Defense += 40;
		modifiers.Knockback *= 0.1f;
		ShieldValue = 60f;
		ShieldHealth -= 1;
		if (ShieldHealth == 0)
		{
			for (int h = 0; h < 20; h++)
			{
				Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(12 * NPC.spriteDirection - 8, -18), 16, 36, ModContent.DustType<TwilightCrystalDust>());
				dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 3.6f).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int h = 0; h < 8; h++)
			{
				Vector2 velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 2.4f).RotatedByRandom(MathHelper.TwoPi);
				var dustVFX = new TwilightCrystalVFXDust
				{
					Velocity = velocity,
					Active = true,
					Visible = true,
					Position = NPC.Center + new Vector2(12 * NPC.spriteDirection - 8 + Main.rand.Next(16), -18 + Main.rand.Next(36)) + velocity * 6,
					MaxTime = Main.rand.Next(24, 42),
					Scale = Main.rand.NextFloat(3, 12),
					Rotation = velocity.ToRotation() - MathHelper.PiOver4,
					ai = new float[] { 0, 0, 0 },
				};
				Ins.VFXManager.Add(dustVFX);
			}
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (ShieldValue == 0)
		{
			HurtValue = 60f;
		}
		base.HitEffect(hit);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}

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

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects flip = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		float fade = (255 - NPC.alpha) / 255f;
		Texture2D body_glow = ModAsset.AncientPhantom_Sheild_glow.Value;
		spriteBatch.Draw(body_glow, NPC.Center + new Vector2(0, NPC.gfxOffY - 7) - screenPos, NPC.frame, Color.White, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);

		Texture2D sheild = ModAsset.AncientPhantom_Sheild_Shield.Value;
		if (HasShield)
		{
			spriteBatch.Draw(sheild, NPC.Center + new Vector2(12 * NPC.spriteDirection, NPC.gfxOffY) - screenPos, null, drawColor * fade, 0, sheild.Size() * 0.5f, NPC.scale, flip, 0);
		}

		Texture2D body_Shape = ModAsset.AncientPhantom_Sheild_Shape.Value;
		Texture2D sheild_Shape = ModAsset.AncientPhantom_Sheild_Shield_Shape.Value;
		Color hurtColor = Color.Lerp(Color.Blue, Color.White, HurtValue / 60f);
		hurtColor *= HurtValue / 60f;

		Color shieldColor = Color.Lerp(Color.Blue, Color.White, ShieldValue / 60f);
		shieldColor *= ShieldValue / 60f;
		spriteBatch.Draw(body_Shape, NPC.Center + new Vector2(0, NPC.gfxOffY) - screenPos, NPC.frame, hurtColor, 0, NPC.frame.Size() * 0.5f, NPC.scale, flip, 0);
		if (HasShield)
		{
			spriteBatch.Draw(sheild_Shape, NPC.Center + new Vector2(12 * NPC.spriteDirection, NPC.gfxOffY) - screenPos, null, shieldColor, 0, sheild.Size() * 0.5f, NPC.scale, flip, 0);
		}
	}

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		if (ShieldHealth > 0 && ShieldHealth < ShieldHealthMax)
		{
			DrawHealthBar();
		}
		return base.DrawHealthBar(hbPosition, ref scale, ref position);
	}

	public void DrawHealthBar()
	{
		float powerProgress = (float)ShieldHealth / ShieldHealthMax;
		var progressTexture = Commons.ModAsset.White.Value;
		var progressPosition = NPC.Center + new Vector2(12 * NPC.spriteDirection, NPC.gfxOffY + 24) - Main.screenPosition;

		var frameColor = new Color(0.05f, 0.05f, 0.08f, 0.9f) * 0.6f;
		var frameColor2 = new Color(0.15f, 0.25f, 0.38f, 0.4f) * 0.6f;
		Vector2 frameScale = new Vector2(2f, 0.4f) * 0.05f;
		Vector2 frameScale2 = new Vector2(1.8f, 0.2f) * 0.05f;

		Color lineColor = new Color(0.3f, 0.6f, 1f, 0.5f);
		Color lineColorInner = new Color(0.15f, 0.3f, 0.5f, 0.75f);

		Vector2 lineScaleOuter = new Vector2(2.2f * powerProgress + 0.2f, 0.7f) * 0.05f;
		Vector2 lineScale = new Vector2(2.2f * powerProgress, 0.5f) * 0.05f;
		Vector2 lineScale2 = new Vector2(2.2f * powerProgress - 0.2f, 0.3f) * 0.05f;
		var linePositionOffset = new Vector2(-2.2f * (1 - powerProgress) * progressTexture.Width * 0.025f, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor, 0, progressTexture.Size() / 2, frameScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition, null, frameColor2, 0, progressTexture.Size() / 2f, frameScale2, SpriteEffects.None, 0);

		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, frameColor, 0, progressTexture.Size() / 2, lineScaleOuter, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColor, 0, progressTexture.Size() / 2, lineScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(progressTexture, progressPosition + linePositionOffset, null, lineColorInner, 0, progressTexture.Size() / 2, lineScale2, SpriteEffects.None, 0);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
