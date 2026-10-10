using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Wing_Fly : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => ModAsset.Fevens_Wing_Mod;

	public Vector2 TargetPos;

	public int Target = -1;

	public int MaxTime = 180;

	public override void OnSpawn(IEntitySource source)
	{
		TargetPos = Vector2.zeroVector;
		Projectile.timeLeft = MaxTime;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void AI()
	{
		if (Projectile.timeLeft > MaxTime - 60)
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3;
			if (Projectile.spriteDirection == -1)
			{
				Projectile.rotation -= MathHelper.PiOver2;
			}
			Projectile.velocity *= 0.95f;
		}

		if (Projectile.timeLeft == MaxTime - 60)
		{
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<NPCs.TownNPCs.Fevens>())
					{
						TargetPos = npc.Center + new Vector2(-200 - Projectile.ai[0] * 100, -500 + Projectile.ai[0] * 100);
						if (Projectile.ai[0] >= 4)
						{
							TargetPos = npc.Center + new Vector2(200 + (Projectile.ai[0] - 4) * 100, -500 + (Projectile.ai[0] - 4) * 100);
						}
					}
				}
			}
		}
		if (Projectile.timeLeft == MaxTime - 61)
		{
			Vector2 vel = (TargetPos - Projectile.Center) * 0.1f;
			Projectile.velocity = vel;
		}

		if (Projectile.timeLeft < MaxTime - 60 && Projectile.timeLeft > MaxTime - 70)
		{
			float targetRot = MathHelper.PiOver2;
			if (Projectile.spriteDirection == 1)
			{
				targetRot = MathHelper.PiOver2 * 3;
			}
			Projectile.rotation = Projectile.rotation * 0.05f + targetRot * 0.95f;
		}
		if (Projectile.timeLeft == MaxTime - 69)
		{
			Projectile.Center = TargetPos;
		}

		if (Projectile.timeLeft == MaxTime - 70)
		{
			float targetRot = MathHelper.PiOver2;
			if (Projectile.spriteDirection == 1)
			{
				targetRot = MathHelper.PiOver2 * 3;
			}
			Projectile.rotation = Projectile.rotation * 0.05f + targetRot * 0.95f;
			Projectile.velocity *= 0;
			Projectile.Center = TargetPos;
			float speed = 120;
			var vel = new Vector2(speed, speed);
			if (Projectile.ai[0] >= 4)
			{
				vel = new Vector2(-speed, speed);
			}
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<Fevens_Wing_Slash>(), 54, 2, default, 2);
			Projectile.hide = true;
		}

		if (Projectile.timeLeft == MaxTime - 75)
		{
			Projectile.hide = false;
			float speed = 700;
			var vel = new Vector2(speed, speed);
			if (Projectile.ai[0] >= 4)
			{
				vel = new Vector2(-speed, speed);
			}
			Projectile.Center += vel;
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<NPCs.TownNPCs.Fevens>())
					{
						Target = npc.target;
						if (npc.target != -1)
						{
							TargetPos = Main.player[Target].Center;
						}
						else
						{
							TargetPos = npc.Center;
						}
					}
				}
			}
			TargetPos += new Vector2(-200, 400).RotatedBy(Projectile.ai[0] / 7f + Main.time);
			vel = (TargetPos - Projectile.Center) * 0.1f;
			Projectile.velocity = vel;
		}
		if (Projectile.timeLeft == MaxTime - 85)
		{
			Projectile.Center = TargetPos;
			Projectile.velocity *= 0;
			if (Target >= 0)
			{
				TargetPos = Main.player[Target].Center;
			}
			if (Projectile.ai[0] == 0)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), TargetPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash_Target>(), 0, 2, default, 2);
			}
		}

		if (Projectile.timeLeft < MaxTime - 85 && Projectile.timeLeft >= MaxTime - 100)
		{
			Vector2 toTarget = TargetPos - Projectile.Center;
			float targetRot = toTarget.ToRotationSafe() + MathHelper.PiOver4;
			if (Projectile.spriteDirection == 1)
			{
				targetRot += MathHelper.PiOver2;
			}
			Projectile.rotation = Projectile.rotation * 0.05f + targetRot * 0.95f;
			if (Projectile.timeLeft == MaxTime - 100)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(toTarget) * 210, ModContent.ProjectileType<Fevens_Wing_Slash>(), 54, 2, default, 2);
				Projectile.hide = true;
			}
		}

		if (Projectile.timeLeft == MaxTime - 115)
		{
			Projectile.velocity *= 0;
			if (Target >= 0)
			{
				TargetPos = Main.player[Target].Center;
				Projectile.Center = TargetPos + new Vector2(400, 0).RotatedBy(Projectile.ai[0] / 8f * MathHelper.TwoPi);
				if (Projectile.ai[0] == 0)
				{
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), TargetPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash_Target>(), 0, 2, default, 2);
				}
			}
			Projectile.hide = false;
		}

		if (Projectile.timeLeft < MaxTime - 115 && Projectile.timeLeft >= MaxTime - 130)
		{
			Vector2 toTarget = TargetPos - Projectile.Center;
			toTarget = toTarget.RotatedBy(0.46f);
			float targetRot = toTarget.ToRotationSafe() + MathHelper.PiOver4;
			if (Projectile.spriteDirection == 1)
			{
				targetRot += MathHelper.PiOver2;
			}
			Projectile.rotation = Projectile.rotation * 0.05f + targetRot * 0.95f;
			if (Projectile.timeLeft == MaxTime - 130)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(toTarget) * 240, ModContent.ProjectileType<Fevens_Wing_Slash>(), 54, 2, default, 2);
				Projectile.hide = true;
			}
		}

		if (Projectile.timeLeft == MaxTime - 135)
		{
			Projectile.velocity *= 0;
			if (Target >= 0)
			{
				foreach (var npc in Main.npc)
				{
					if (npc != null && npc.active)
					{
						if (npc.type == ModContent.NPCType<NPCs.TownNPCs.Fevens>())
						{
							Target = npc.target;
							TargetPos = npc.Center;
							if (Projectile.ai[0] == 0)
							{
								Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), TargetPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash_Target>(), 0, 2, default, 2);
							}
						}
					}
				}
				Projectile.Center = TargetPos + new Vector2(400, 0).RotatedBy(Projectile.ai[0] / 8f * MathHelper.TwoPi);
			}
			Projectile.hide = false;
		}

		if (Projectile.timeLeft < MaxTime - 135 && Projectile.timeLeft >= MaxTime - 150)
		{
			Vector2 toTarget = TargetPos - Projectile.Center;
			float targetRot = toTarget.ToRotationSafe() + MathHelper.PiOver4;
			if (Projectile.spriteDirection == 1)
			{
				targetRot += MathHelper.PiOver2;
			}
			Projectile.rotation = Projectile.rotation * 0.05f + targetRot * 0.95f;
			if (Projectile.timeLeft == MaxTime - 150)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Normalize(toTarget) * 240, ModContent.ProjectileType<Fevens_Wing_Slash>(), 54, 2, default, 2);
				Projectile.hide = true;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (Projectile.hide)
		{
			return false;
		}
		Texture2D wing = ModAsset.Fevens_Wing.Value;
		Texture2D wingFlame = ModAsset.Fevens_Wing_Flame.Value;
		int frameIndex = (int)(Projectile.timeLeft * 0.25f + Projectile.whoAmI) % 7;
		var frame = new Rectangle(0, 40 * frameIndex, 40, 40);
		Main.EntitySpriteDraw(wing, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, 1, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		Main.EntitySpriteDraw(wingFlame, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), Projectile.rotation, frame.Size() * 0.5f, 1, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		return false;
	}
}
