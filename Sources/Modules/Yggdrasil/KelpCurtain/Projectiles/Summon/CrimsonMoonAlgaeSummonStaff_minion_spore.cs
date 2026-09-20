using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class CrimsonMoonAlgaeSummonStaff_minion_spore : ModProjectile
{
	public float Timer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.DamageType = DamageClass.Summon;
	}

	public override void AI()
	{
		Timer++;
		Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Timer * 0.1f + Projectile.ai[0]) * 0.01f);
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.9f, 0.8f) * 0.5f);

		Vector2 closestTargetPos = new Vector2(-10000);
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (!npc.friendly && !npc.dontTakeDamage && npc.CanBeChasedBy(Projectile))
				{
					Vector2 toTargetCheck = npc.Center - Projectile.Center;
					if (toTargetCheck.Length() < (closestTargetPos - Projectile.Center).Length())
					{
						closestTargetPos = npc.Center;
					}
				}
			}
		}
		Vector2 toTarget = closestTargetPos - Projectile.Center - Projectile.velocity;
		float distance = toTarget.Length();
		if (distance < 600)
		{
			float value = distance / 600f;
			value = Math.Clamp(value, 0.1f, 0.95f);
			Projectile.velocity = Projectile.velocity * value + toTarget.NormalizeSafe() * 12 * (1 - value);
			Projectile.velocity = Projectile.velocity.NormalizeSafe() * 12f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		int type = ModContent.BuffType<RedAlgae_FriendlyDebuff>();
		if (!target.HasBuff(type))
		{
			target.AddBuff(type, 900);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, texture.Size() * 0.5f, 1, SpriteEffects.None);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft != 0)
		{
			for (int k = 0; k < 12; k++)
			{
				var redAlgaeDust = new RedAlgae_Spark();
				redAlgaeDust.Position = Projectile.Center;
				redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
				redAlgaeDust.Velocity = new Vector2(0, Main.rand.NextFloat(3f)).RotatedByRandom(MathHelper.TwoPi);
				redAlgaeDust.ai = new float[] { 0.99f };
				redAlgaeDust.MaxTime = 30;
				redAlgaeDust.Scale = Main.rand.NextFloat(1f, 2f);
				redAlgaeDust.Visible = true;
				redAlgaeDust.Active = true;
				Ins.VFXManager.Add(redAlgaeDust);
			}
			float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
			float randomSize = Main.rand.NextFloat(0.85f, 1.15f);
			for (int k = 0; k < 18; k++)
			{
				var redAlgaeDust = new RedAlgae_Spark();
				redAlgaeDust.Position = Projectile.Center;
				redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
				redAlgaeDust.Velocity = new Vector2(0, 3 * randomSize).RotatedBy(MathHelper.TwoPi * k / 18f + randomRot);
				redAlgaeDust.ai = new float[] { 0.9f };
				redAlgaeDust.MaxTime = 50;
				redAlgaeDust.Scale = Main.rand.NextFloat(2.7f, 5f);
				redAlgaeDust.Visible = true;
				redAlgaeDust.Active = true;
				Ins.VFXManager.Add(redAlgaeDust);
			}
		}
	}
}
