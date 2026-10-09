using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.NPCs;

public class FireflyPiranha_small : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 5;
	}

	public override void SetDefaults()
	{
		NPC.damage = 18;
		NPC.width = 48;
		NPC.height = 38;
		NPC.defense = 14;
		NPC.lifeMax = 80;
		NPC.knockBackResist = 0.4f;
		NPC.value = 200;
		NPC.aiStyle = -1;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.localAI[0] = 0;
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
	}

	private int PhysicalStrength = 700;

	public override void AI()
	{
		if (NPC.wet)
		{
			if (!NPC.HasNPCTarget)
			{
				NPC.TargetClosest();
				WanderingWithoutTarget();
			}
			NPC.knockBackResist = 0.4f;
			NPC.noGravity = true;
			if (NPC.HasPlayerTarget && PhysicalStrength > 0)
			{
				Player player = Main.player[NPC.target];
				NormalAttack(player);
				PhysicalStrength -= 1;
				if (PhysicalStrength <= 2)
				{
					PhysicalStrength = -350;
					if (NPC.Center.X > player.Center.X)
					{
						NPC.velocity.X = -4;
					}
					else
					{
						NPC.velocity.X = 4;
					}
				}
			}
			if (NPC.HasPlayerTarget && PhysicalStrength <= 0)
			{
				Player player = Main.player[NPC.target];
				Wander(player);
				PhysicalStrength++;
				if (PhysicalStrength >= 0)
				{
					PhysicalStrength = 350;
				}
			}
			NPC.rotation = NPC.velocity.ToRotation() + (1 - NPC.spriteDirection) * MathF.PI / 2f;
		}
		else
		{
			NPC.knockBackResist = 0;
			if (PhysicalStrength > 0)
			{
				PhysicalStrength -= 1;
			}
			NPC.localAI[0] += 1;
			NPC.noGravity = false;
			NPC.velocity.Y += 0.15f;
			if (NPC.localAI[0] % 4 == 0 && NPC.collideY)
			{
				PhysicalStrength -= 100;
				NPC.velocity += new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-8f, -4f));
				NPC.rotation = Main.rand.NextFloat(-0.2f, 0.2f);
			}
			if (NPC.collideX)
			{
				PhysicalStrength -= 100;
				NPC.velocity += new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-4f, -2f));
				NPC.rotation = Main.rand.NextFloat(-0.2f, 0.2f);
			}
			NPC.velocity *= MathF.Pow(0.996f, NPC.velocity.Length());
			if (PhysicalStrength > 0)
			{
				NPC.rotation = NPC.velocity.ToRotation() + (1 - NPC.spriteDirection) * MathF.PI / 2f;
			}
			else
			{
				NPC.rotation += MathF.Sin((float)Main.time * 0.03f + NPC.whoAmI) * 0.03f;
			}
		}
		if (NPC.velocity.X > 0)
		{
			NPC.spriteDirection = 1;
		}
		if (NPC.velocity.X < 0)
		{
			NPC.spriteDirection = -1;
		}
	}

	private void NormalAttack(Player target)
	{
		Vector2 toPlayer = target.Center - NPC.Center;
		if (toPlayer.Length() > 600f)
		{
			NPC.target = 255;
			return;
		}
		if (toPlayer.Length() > 200f && Collision.CanHit(NPC, target) && target.wet)
		{
			Vector2 velocityRotation = Vector2.Normalize(toPlayer) * 17f;
			float mulVelocity = 0.15f * NPC.scale;
			NPC.velocity = velocityRotation * mulVelocity + NPC.velocity * (1 - mulVelocity);
			NPC.velocity.Y *= 0.85f;
		}
		if (toPlayer.Length() > 160f && toPlayer.Length() <= 200f && Collision.CanHit(NPC, target))
		{
			if (Main.rand.NextBool(20))
			{
				Vector2 velocityRotation = Vector2.Normalize(toPlayer) * 30f;
				NPC.velocity = velocityRotation * 0.55f + NPC.velocity * 0.45f;
			}
		}
		if (MathF.Abs(NPC.velocity.X) < 6f)
		{
			NPC.velocity.X *= 1.24f;
			if (MathF.Abs(NPC.velocity.X) == 0)
			{
				NPC.velocity.X += Main.rand.NextFloat(-0.1f, 0.1f);
			}
		}
		if (MathF.Abs(NPC.velocity.X) > 8f)
		{
			NPC.velocity.X *= 0.96f;
		}
		Vector2 willBeToPlayer = target.Center - (NPC.Center + NPC.velocity * 15f);
		if (Math.Abs(willBeToPlayer.X) > 600)
		{
			if (Main.rand.NextBool(20))
			{
				NPC.velocity.X *= -1;
			}
		}
		SoundEngine.PlaySound(SoundID.SplashWeak.WithPitchOffset(Main.rand.NextFloat(-1f, -0.2f)).WithVolume(0.5f));
	}

	private void WanderingWithoutTarget()
	{
		int waterDepth = 0;
		int x = (int)(NPC.Center.X / 16f);
		if (x < 20 || x > Main.maxTilesX - 20)
		{
			NPC.active = false;
			return;
		}
		while (Main.tile[x, (int)(NPC.Center.Y / 16f) - waterDepth].LiquidAmount > 0)
		{
			int y = (int)(NPC.Center.Y / 16f) - waterDepth;
			if (y < 20 || y > Main.maxTilesY - 20)
			{
				NPC.active = false;
				return;
			}
			waterDepth++;
		}
		if (Main.rand.NextBool(200))
		{
			NPC.velocity.X *= -1;
		}
		if (waterDepth <= 5)
		{
			if (NPC.velocity.Y < 1f)
			{
				NPC.velocity.Y += 0.01f;
			}
		}
		else if (waterDepth >= 15)
		{
			if (NPC.velocity.Y > -1f)
			{
				NPC.velocity.Y -= 0.01f;
			}
		}
		if (MathF.Abs(NPC.velocity.Y) > 2f)
		{
			NPC.velocity.Y *= 0.94f;
		}
		if (MathF.Abs(NPC.velocity.X) > 3.6f)
		{
			NPC.velocity.X *= 0.96f;
		}
		if (MathF.Abs(NPC.velocity.X) < 1.8f)
		{
			NPC.velocity.X *= 1.04f;
			if (MathF.Abs(NPC.velocity.X) == 0)
			{
				NPC.velocity.X += Main.rand.NextFloat(-0.1f, 0.1f);
			}
		}
		if (NPC.collideX)
		{
			NPC.velocity.X *= -1f;
		}
	}

	private void Wander(Player target)
	{
		int waterDepth = 0;
		int x = (int)(NPC.Center.X / 16f);
		if (x < 20 || x > Main.maxTilesX - 20)
		{
			NPC.active = false;
			return;
		}
		while (Main.tile[x, (int)(NPC.Center.Y / 16f) - waterDepth].LiquidAmount > 0)
		{
			int y = (int)(NPC.Center.Y / 16f) - waterDepth;
			if (y < 20 || y > Main.maxTilesY - 20)
			{
				NPC.active = false;
				return;
			}
			waterDepth++;
		}
		Vector2 toPlayer = target.Center - NPC.Center;
		if (MathF.Abs(toPlayer.X) > 200 && Main.rand.NextBool(200))
		{
			Vector2 velocityRotation = Vector2.Normalize(toPlayer) * 5f;
			NPC.velocity.Y = velocityRotation.Y;
			if (NPC.Center.X > target.Center.X)
			{
				NPC.velocity.X = -1.6f * NPC.scale;
			}
			else
			{
				NPC.velocity.X = 1.6f * NPC.scale;
			}
			NPC.velocity.Y *= 0.4f;
		}
		if (waterDepth <= 5)
		{
			if (NPC.velocity.Y < 1f)
			{
				NPC.velocity.Y += 0.03f;
			}
		}
		else if (waterDepth >= 15)
		{
			if (NPC.velocity.Y > -1f)
			{
				NPC.velocity.Y -= 0.03f;
			}
		}
		if (MathF.Abs(NPC.velocity.Y) > 2f)
		{
			NPC.velocity.Y *= 0.94f;
		}
	}

	public override void FindFrame(int frameHeight)
	{
		frameHeight = 38;
		NPC.frameCounter += Math.Max(Math.Abs(NPC.velocity.X), 0.5f) / NPC.scale;

		if (NPC.frameCounter > 12)
		{
			if (NPC.frame.Y < 152)
			{
				NPC.frame.Y += frameHeight;
			}
			else
			{
				NPC.frame.Y = 0;
			}
			NPC.frameCounter = 0;
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.FireflyPiranha_small.Value;
		spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.frame.Width, NPC.frame.Height) * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		tex = ModAsset.FireflyPiranha_small_glow.Value;
		float colorValue = 0.6f;
		spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, new Color(colorValue, colorValue, colorValue, 0), NPC.rotation, new Vector2(NPC.frame.Width, NPC.frame.Height) * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
	}

	public override void OnKill()
	{
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
			   new Vector2(0, Main.rand.NextFloat(4)).RotatedByRandom(6.283), ModContent.Find<ModGore>("Everglow/FireflyPiranhaSmall0").Type);
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
			   new Vector2(0, Main.rand.NextFloat(4)).RotatedByRandom(6.283), ModContent.Find<ModGore>("Everglow/FireflyPiranhaSmall1").Type);
		Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283),
			   new Vector2(0, Main.rand.NextFloat(4)).RotatedByRandom(6.283), ModContent.Find<ModGore>("Everglow/FireflyPiranhaSmall2").Type);
		for (int f = 0; f < 32; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(15f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.0f));
		}
		for (int f = 0; f < 16; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(16f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.BlueGlowAppearOnlyInWater>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}

	public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		NPC.target = projectile.owner;
		for (int f = 0; f < 4; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(6f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
		for (int f = 0; f < 4; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.BlueGlowAppearOnlyInWater>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}

	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		NPC.target = player.whoAmI;
		for (int f = 0; f < 4; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(6f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.NavyBlood>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
		for (int f = 0; f < 4; f++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(9f)).RotatedByRandom(6.283);
			Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<Dusts.BlueGlowAppearOnlyInWater>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.85f, 1.75f));
		}
	}
}
