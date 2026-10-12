using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class MechanicMosquito_Mosquito : ModProjectile
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

		public NPC Target;
		public Projectile Owner;
		public int HoverTimer = 0;
		public int Blood = 0;

		public override void SetDefaults()
		{
			Projectile.timeLeft = 3600000;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 18;
		}

		public override void OnSpawn(IEntitySource source)
		{
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile.type == ModContent.ProjectileType<MechanicMosquito_Pro>())
				{
					if (projectile.owner == Projectile.owner)
					{
						if (projectile.active)
						{
							Owner = projectile;
							break;
						}
					}
				}
			}
		}

		public override void AI()
		{
			if (!Owner.active || Owner == null || Owner.type != ModContent.ProjectileType<MechanicMosquito_Pro>() || Owner.owner != Projectile.owner)
			{
				Projectile.Kill();
				return;
			}
			var MechanicMosquito_Pro = Owner.ModProjectile as MechanicMosquito_Pro;
			if (MechanicMosquito_Pro == null)
			{
				Projectile.Kill();
				return;
			}
			if (MechanicMosquito_Pro.ProjTarget == -1)
			{
				Projectile.Kill();
				return;
			}
			Target = Main.npc[MechanicMosquito_Pro.ProjTarget];
			if (HoverTimer < 30)
			{
				Hover();
				HoverTimer++;
			}
			else
			{
				if (Main.rand.NextBool(Math.Max(150 - HoverTimer, 1)))
				{
					if (Target.active || Target != null)
					{
						if ((Target.Center - Projectile.Center).Length() < 100f)
						{
							HoverTimer = 0;
							Projectile.velocity = Vector2.Normalize(Target.Center - Projectile.Center) * 30f;
						}
						else
						{
							Hover();
							HoverTimer++;
						}
					}
					else
					{
						Hover();
						HoverTimer++;
					}
				}
				else
				{
					Hover();
					HoverTimer++;
				}
			}
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		}

		public void Hover()
		{
			Player player = Main.player[Projectile.owner];
			Vector2 aim = player.Center + new Vector2(MathF.Sin((float)(Main.time * 0.03f + Projectile.whoAmI * 7)) * 60 - 30 * player.direction, MathF.Sin((float)(Main.time * 0.007f + Projectile.ai[0])) * 30 - 60);
			if (Target != null && Target.active && (Target.Center - player.Center).Length() < 190f)
			{
				aim = Target.Center + new Vector2(MathF.Sin((float)(Main.time * 0.03f + Projectile.whoAmI * 7)) * 120, MathF.Sin((float)(Main.time * 0.007f + Projectile.ai[0])) * 40 - 80);
			}
			float mulAcc = 1f;
			if (HoverTimer < 10)
			{
				mulAcc = HoverTimer * 0.1f;
				Projectile.velocity *= 0.8f;
			}
			Projectile.velocity *= 0.95f;
			Projectile.velocity += (aim - Projectile.Center - Projectile.velocity).SafeNormalize(Vector2.zeroVector) * 0.4f * mulAcc;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (!Main.gamePaused)
			{
				Projectile.frameCounter++;
				if (Projectile.frameCounter >= 16)
				{
					if (Projectile.frame >= 1)
					{
						Projectile.frame = 0;
					}
					else
					{
						Projectile.frame++;
					}
				}
			}
			Main.EntitySpriteDraw(ModAsset.MechanicMosquito_Mosquito.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 18 * Projectile.frame, 24, 18), lightColor, Projectile.rotation, new Vector2(12, 9), Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
			Main.EntitySpriteDraw(ModAsset.MechanicMosquito_Mosquito_glow.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 18 * Projectile.frame, 24, 18), new Color(1f, 1f, 1f, 0), Projectile.rotation, new Vector2(12, 9), Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);

			return false;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Blood < 10)
			{
				Blood++;
			}
			if (Blood == 10)
			{
				Projectile.Kill();
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int a = 0; a < 10; a++)
			{
				var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.TheDestroyer, 0, 0, 0, default, Main.rand.NextFloat(0.8f, 2f));
				d.noGravity = true;
			}
			if (Blood > 0)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.VampireHeal, 5, 0, Projectile.owner, Projectile.owner, Blood);
			}
		}
	}
}
