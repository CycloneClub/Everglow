using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;

namespace Everglow.EternalResolve.Projectiles
{
	public class RottenGoldBayonet_Pro_Stab : StabbingProjectile_Stab
	{
		public override void SetCustomDefaults()
		{
			StabColor = new Color(105, 105, 255);
			StabShade = 0.5f;
			StabEffectWidth = 0.4f;
			spAttCounts = 2;
			HitTileSparkColor = new Color(105, 105, 255, 150);
		}

		public int spAttCounts = 2; // 一次攻击最多触发两次特殊

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (spAttCounts > 0)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), target.Center, Vector2.zeroVector, ModContent.ProjectileType<RottenGoldBayonet_Mark>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 2.97f, Projectile.owner, 1, target.whoAmI);
			}

			spAttCounts--;
			base.OnHitNPC(target, hit, damageDone);
		}

		public override void DrawEffect(Color lightColor)
		{
			base.DrawEffect(lightColor);
		}

		public override void AI()
		{
			base.AI();
			if (Main.rand.NextBool(6))
			{
				Vector2 end = Projectile.Center + Projectile.velocity * 80 * StabDistance;
				if (StabEndPoint_WorldPos != Vector2.zeroVector)
				{
					end = StabEndPoint_WorldPos;
				}
				var dust = Dust.NewDustDirect(Vector2.Lerp(StabStartPoint_WorldPos, end, Main.rand.NextFloat(0.3f, 1f)) - new Vector2(4), 0, 0, ModContent.DustType<CorruptShine>(), 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.2f));
				dust.velocity = new Vector2(0, Main.rand.NextFloat(2f)).RotateRandom(6.283);
			}
		}
	}
}
