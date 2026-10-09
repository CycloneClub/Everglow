using Terraria;
using Terraria.DataStructures;
using static Terraria.NPC.NPCNameFakeLanguageCategoryPassthrough;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class ToothBow_BloodArrow : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.timeLeft = 2400;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.alpha = 255;
		Projectile.hide = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 4;
		Projectile.aiStyle = -1;
	}

	public int stickNPC = -1;
	public float relativeAngle = 0;
	public Vector2 relativePos = Vector2.zeroVector;
	public float hitTargetAngle = 0;
	public bool HasHitTile = false;
	public int HitCount = 0;
	public float Power = 0;

	public override void OnSpawn(IEntitySource source)
	{
		HasHitTile = false;
	}

	public override void AI()
	{
		Power *= 0.75f;
		if (!Collision.SolidCollision(Projectile.Center, 0, 0))
		{
			Projectile.velocity.Y += 0.2f;
		}
		else
		{
			Projectile.velocity *= Vector2.zeroVector;
		}

		if (stickNPC != -1)
		{
			StickToTarget();
		}
		else
		{
			if (!HasHitTile)
			{
				Projectile.rotation = Projectile.velocity.ToRotation();
			}
		}
	}

	public void StickToTarget()
	{
		Projectile.velocity *= 0;
		if (stickNPC != -1)
		{
			NPC stick = Main.npc[stickNPC];
			if (stick != null && stick.active && !stick.dontTakeDamage)
			{
				Projectile.rotation = stick.rotation + relativeAngle;
				Projectile.Center = stick.Center + relativePos.RotatedBy(stick.rotation + relativeAngle - hitTargetAngle);
			}
			else
			{
				stickNPC = -1;
			}
		}
	}

	public bool Collide(Vector2 positon)
	{
		foreach (NPC npc in Main.npc)
		{
			if (npc.active && !npc.dontTakeDamage)
			{
				if (new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1).Intersects(npc.Hitbox))
				{
					relativeAngle = Projectile.rotation - npc.rotation;
					hitTargetAngle = Projectile.rotation;
					relativePos = Projectile.Center - npc.Center;
					stickNPC = npc.whoAmI;
					return true;
				}
			}
		}
		return Collision.SolidCollision(positon, 0, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (Power > 1f)
		{
			Texture2D texGlow = ModAsset.ToothBow_BloodArrow_glow.Value;
			float value = Power / 40f;
			Color c0 = new Color(value, value, value, 0);
			Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		Texture2D tex = ModAsset.ToothBow_BloodArrow.Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity *= 0;
		HasHitTile = true;
		return true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.ai[0] != 3)
		{
			HitCount++;
			Power = 40f;
			Projectile.friendly = false;
			relativeAngle = Projectile.rotation - target.rotation;
			hitTargetAngle = Projectile.rotation;
			relativePos = Projectile.Center - target.Center;
			stickNPC = target.whoAmI;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj != null && proj.active)
				{
					if (proj.type == Projectile.type && proj.ai[0] == 3)
					{
						ToothBow_BloodArrow tBBA = proj.ModProjectile as ToothBow_BloodArrow;
						if (tBBA != null)
						{
							proj.friendly = true;
							tBBA.HitCount++;
							tBBA.Power += 40;
							if (tBBA.HitCount > 6)
							{
								proj.Kill();
							}
						}
					}
				}
			}
			Projectile.ai[0] = 3;
		}
		else
		{
			Projectile.friendly = false;
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 7; i++)
		{
			Gore gore = Gore.NewGoreDirect(null, Projectile.Center + new Vector2(i * 20 - 70, 0).RotatedBy(Projectile.rotation), Projectile.velocity, ModContent.Find<ModGore>("Everglow/ToothBow_BloodArrow_gore" + i).Type, 1f);
			gore.position = Projectile.Center + new Vector2(i * 20 - 70, 0).RotatedBy(Projectile.rotation) - new Vector2(gore.Width, gore.Height) / 2f;
			gore.velocity = Projectile.velocity;
			gore.rotation = Projectile.rotation;
		}
		base.OnKill(timeLeft);
	}
}
