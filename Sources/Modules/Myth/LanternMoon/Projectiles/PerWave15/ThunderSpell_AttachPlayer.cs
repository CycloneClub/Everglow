using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class ThunderSpell_AttachPlayer : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float Timer = 0;

	public NPC OwnerNPC;

	public Player AttachedPlayer;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 3600000;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0.75f;
	}

	public int GetAttachOrder()
	{
		int value = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == Type && proj.whoAmI < Projectile.whoAmI)
			{
				ThunderSpell_AttachPlayer tSAP = proj.ModProjectile as ThunderSpell_AttachPlayer;
				if (tSAP.AttachedPlayer == AttachedPlayer)
				{
					value++;
				}
			}
		}
		return value;
	}

	public int GetTotalCount()
	{
		int value = 0;
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.type == Type)
			{
				ThunderSpell_AttachPlayer tSAP = proj.ModProjectile as ThunderSpell_AttachPlayer;
				if (tSAP.AttachedPlayer == AttachedPlayer)
				{
					value++;
				}
			}
		}
		return value;
	}

	public override void AI()
	{
		Timer++;
		Projectile.velocity *= 0;
		if (AttachedPlayer == null)
		{
			return;
		}
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<WizardLantern>())
		{
			float total = GetTotalCount();
			if (total > 0)
			{
				Projectile.Center = AttachedPlayer.Center + new Vector2(0, MathF.Sin((float)Main.time * 0.03f) * 10 + 80).RotatedBy(GetAttachOrder() / total * MathHelper.TwoPi + (float)Main.time * 0.03f);
			}
			if (total >= 3 && GetAttachOrder() == 2)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj is not null && proj.active && proj.type == Type)
					{
						proj.Kill();
					}
				}
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), AttachedPlayer.Bottom, Vector2.zeroVector, ModContent.ProjectileType<ThunderSpell_Thunder>(), 100, 1.5f, AttachedPlayer.whoAmI);
			}
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		Projectile.spriteDirection = Projectile.direction;
		if (Projectile.spriteDirection == -1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		var drawColor = new Color(1f, 0.9f, 0.3f, 0);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, effects, 0);
		return false;
	}
}
