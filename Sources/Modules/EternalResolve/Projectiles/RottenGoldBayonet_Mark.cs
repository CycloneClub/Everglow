using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;

namespace Everglow.EternalResolve.Projectiles
{
	public class RottenGoldBayonet_Mark : ModProjectile
	{
		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.aiStyle = -1;
		}

		public override void AI()
		{
			Projectile.velocity *= 0;
			if (Main.npc[(int)Projectile.ai[1]].active)// 锁定目标
			{
				Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
			}
			if (Projectile.ai[0] == 0)
			{
				if (Projectile.timeLeft > 60)
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 deltaPos = new Vector2(0, Main.rand.NextFloat(30f, 100f)).RotatedByRandom(6.283);
						var dust = Dust.NewDustDirect(Projectile.Center + deltaPos, 0, 0, ModContent.DustType<RottenSmog>());
						dust.color.R = (byte)(Projectile.whoAmI % 256);
						dust.color.G = (byte)((Projectile.whoAmI - dust.color.R) / 256f);
						dust.rotation += Main.rand.NextFloat(6.283f);
						dust.velocity = Vector2.Normalize(deltaPos).RotatedBy(MathF.PI / 2f) * 10;
						dust.scale = 0.1f;
					}
				}
				else if (Projectile.timeLeft == 60)
				{
					Vector2 addPos = new Vector2(0, 120f).RotatedByRandom(6.283);
					for (int i = 0; i < 3; i++)
					{
						Vector2 newAddPos = addPos.RotatedBy(Math.PI / 3f * i * 2);
						Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + newAddPos, -newAddPos * 0.2f, ModContent.ProjectileType<RottenGoldBayonet_Slash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
					}
				}
			}
			else if (Projectile.ai[0] == 1)// 重击的标记
			{
				if (Projectile.timeLeft > 80)
				{
					for (int i = 0; i < 2; i++)
					{
						Vector2 deltaPos = new Vector2(0, Main.rand.NextFloat(30f, 100f)).RotatedByRandom(6.283);
						var dust = Dust.NewDustDirect(Projectile.Center + deltaPos, 0, 0, ModContent.DustType<RottenSmog>());
						dust.color.R = (byte)(Projectile.whoAmI % 256);
						dust.color.G = (byte)((Projectile.whoAmI - dust.color.R) / 256f);
						dust.rotation += Main.rand.NextFloat(6.283f);
						dust.velocity = Vector2.Normalize(deltaPos).RotatedBy(MathF.PI / 2f) * 10;
						dust.scale = 0.1f;
					}
				}

				if (Projectile.timeLeft > 100 && Projectile.timeLeft < 120 && Projectile.timeLeft % 5 == 0)
				{
					Vector2 newAddPos = new Vector2(0, 100f).RotatedByRandom(6.283);
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + newAddPos, -newAddPos * 0.2f, ModContent.ProjectileType<RottenGoldBayonet_Slash>(), Projectile.damage, Projectile.knockBack, Projectile.owner).extraUpdates++;
				}
			}
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			return false;
		}
	}
}
