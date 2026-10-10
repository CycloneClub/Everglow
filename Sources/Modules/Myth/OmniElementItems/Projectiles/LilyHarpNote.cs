using Everglow.Myth.Common;
using Everglow.Myth.OmniElementItems.Dusts;

namespace Everglow.Myth.OmniElementItems.Projectiles;

public class LilyHarpNote : ModProjectile
{
	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
		Projectile.extraUpdates = 1;
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.hostile = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{

		if (Projectile.timeLeft < 220)
		{
			/*if(Projectile.ai[1] != -1)
                {
                    Projectile.friendly = true;
                    if(Main.npc[(int)Projectile.ai[1]].active)
                    {
                        NPC target = Main.npc[(int)Projectile.ai[1]];
                        if (!target.active && Projectile.timeLeft > 10)
                        {
                            Projectile.timeLeft = 10;
                        }
                        else//追踪目标
                        {
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 15, 0.05f);
                        }
                    }
                    else
                    {
                        Projectile.ai[1] = -1;
                    }
                }
                else
                {
                    CheckEmemies();
                }*/
			Projectile.friendly = true;
			if (Main.mouseLeft)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * 15, 0.05f);
			}
		}
		else
		{
			Projectile.velocity *= 0.93f;
		}
	}

	internal void CheckEmemies()
	{
		int AimWhoAmI = -1;
		for (int j = 0; j < 200; j++)
		{
			if (Main.npc[j].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[j].Center, 1, 1))
			{
				Vector2 v1 = Main.npc[j].Center;
				Vector2 v2 = Main.MouseWorld;

				if (AimWhoAmI == -1)
				{
					AimWhoAmI = j;
				}
				else if ((v1 - v2).Length() < (Main.npc[j].Center - v2).Length())
				{
					AimWhoAmI = j;
				}
			}
		}
		Projectile.ai[1] = AimWhoAmI;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D t = ModAsset.LilyHarpNote.Value;
		SpriteEffects se = SpriteEffects.None;
		if (player.gravDir == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 120), Projectile.rotation, t.Size() / 2f, Projectile.scale, se, 0);
		return false;
	}


	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LilyHarpNoteKill>(), 0, 0);
		for (int i = 0; i < 18; i++)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<GreenParticle>(), 0f, 0f, 0, default, Main.rand.NextFloat(1.7f, 3.1f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(2f, 4f)).RotatedByRandom(6.283);
			Main.dust[index].alpha = 230;
			Main.dust[index].rotation = Main.rand.NextFloat(0, 6.283f);
		}
		for (int i = 0; i < 6; i++)
		{
			int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LilyLeaf>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, Main.rand.NextFloat(1.7f, 3.1f));
			Main.dust[index2].velocity = new Vector2(0, Main.rand.NextFloat(2f, 4f)).RotatedByRandom(6.283);
		}
		base.OnKill(timeLeft);
	}
}
