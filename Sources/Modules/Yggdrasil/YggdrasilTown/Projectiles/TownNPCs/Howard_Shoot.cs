using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Howard_Shoot : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public NPC Target = null;

	public int Timer;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Target = null;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
		int checkDirection = 1;
		if (Projectile.velocity.Y < 0)
		{
			checkDirection = -1;
		}
		Projectile.spriteDirection = checkDirection;
		Projectile.rotation = 0;

		if (Projectile.ai[0] is >= 0 and < 200)
		{
			Owner = Main.npc[(int)Projectile.ai[0]];
		}
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
			return;
		}
		Projectile.spriteDirection = -Owner.spriteDirection;
		Projectile.Center = Owner.Center + new Vector2(-Owner.spriteDirection * 8, -10);
		Timer++;
		GetClosestTarget();
		if (Target != null && Target.active)
		{
			Projectile.rotation = (Target.Center - Projectile.Center).ToRotationSafe() - MathHelper.PiOver2;
			if (Projectile.timeLeft % 24 == 0)
			{
				float speed = 25f;
				var toTarget = Target.Center - Projectile.Center;
				float hitTime = toTarget.Length() / speed;
				var vel = (Target.Center + Target.velocity * hitTime - Projectile.Center).NormalizeSafe() * speed;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(-4, 0).RotatedBy(Projectile.rotation), vel, ProjectileID.Bullet, Projectile.damage, 0.4f, Main.myPlayer, 0);
				SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
			}
		}
		else
		{
			Projectile.active = false;
		}
	}

	public void GetClosestTarget()
	{
		float minDis = 3000;
		if (Target is not null && Target.active)
		{
			minDis = (Target.Center - Projectile.Center).Length() - 200;
		}
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active && CanHitNPCLine(npc) && npc != Owner && !npc.friendly && npc.type != NPCID.TargetDummy)
			{
				Vector2 distance = npc.Center - Projectile.Center;
				for (int i = 0; i < 8; i++)
				{
					Vector2 checkPos = npc.Center;
					switch (i)
					{
						case 0:
							checkPos = npc.TopLeft;
							break;
						case 1:
							checkPos = npc.Top;
							break;
						case 2:
							checkPos = npc.TopRight;
							break;
						case 3:
							checkPos = npc.Left;
							break;
						case 4:
							checkPos = npc.Right;
							break;
						case 5:
							checkPos = npc.BottomLeft;
							break;
						case 6:
							checkPos = npc.Bottom;
							break;
						case 7:
							checkPos = npc.BottomRight;
							break;
					}
					if ((checkPos - Projectile.Center).Length() < minDis)
					{
						minDis = (checkPos - Projectile.Center).Length();
						Target = npc;
					}
				}
			}
		}
	}

	public bool CanHitNPCLine(NPC target)
	{
		bool canHit = true;
		var toTarget = target.Center - Projectile.Center;
		float distance = toTarget.Length() / 6f;
		if (distance < 2)
		{
			return true;
		}
		var step = toTarget.NormalizeSafe() * 6f;
		var checkPos = Projectile.Center;
		for (int i = 0; i < distance; i++)
		{
			checkPos += step;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				canHit = false;
				break;
			}
		}
		return canHit;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = ModAsset.Howard_Shoot.Value;
		float gunRot = Projectile.rotation - MathHelper.PiOver2;
		var gunFrame = new Rectangle(0, 0, 48, 16);
		var armFrame = new Rectangle(0, 38, 12, 24);
		if (Projectile.spriteDirection == -1)
		{
			armFrame.X += 14;
		}
		float flameValue = Timer % 24 / 6f - 3f;
		if (flameValue <= 0)
		{
			flameValue = 0;
		}
		var drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -4 * MathF.Pow(flameValue, 0.5f)).RotatedBy(Projectile.rotation);
		SpriteEffects sE = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			sE = SpriteEffects.FlipVertically;
		}

		Main.EntitySpriteDraw(tex, drawPos + new Vector2(-4 * Projectile.spriteDirection, 26).RotatedBy(Projectile.rotation), gunFrame, lightColor, gunRot, new Vector2(31, 8), Projectile.scale, sE, 0);
		Main.EntitySpriteDraw(tex, drawPos, armFrame, lightColor, Projectile.rotation, new Vector2(6, 0), Projectile.scale, SpriteEffects.None, 0);

		Texture2D flame = Commons.ModAsset.StarSlash.Value;
		Main.EntitySpriteDraw(flame, drawPos + new Vector2(-5 * Projectile.spriteDirection, 56).RotatedBy(Projectile.rotation), new Rectangle(0, 0, 256, 128), new Color(MathF.Pow(flameValue, 0.4f), flameValue * flameValue * 0.5f, flameValue * flameValue * flameValue * 0.6f, 0), Projectile.rotation + MathHelper.Pi, new Vector2(128, 128), Projectile.scale * flameValue, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(flame, drawPos + new Vector2(-5 * Projectile.spriteDirection, 56).RotatedBy(Projectile.rotation), new Rectangle(0, 0, 256, 128), new Color(MathF.Pow(flameValue, 0.4f), flameValue * flameValue * 0.5f, flameValue * flameValue * flameValue * 0.6f, 0), Projectile.rotation + MathHelper.Pi, new Vector2(128, 128), Projectile.scale * flameValue, SpriteEffects.None, 0);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
	}
}
