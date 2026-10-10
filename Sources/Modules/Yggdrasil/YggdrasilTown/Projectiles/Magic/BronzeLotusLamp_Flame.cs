using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BronzeLotusLamp_Flame : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 20;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = 1;
		Projectile.ignoreWater = true;
		Projectile.friendly = true;
	}

	public override void AI()
	{
		for (int i = 0; i < 2; i++)
		{
			float size = Main.rand.NextFloat(0.1f, 0.96f);
			var lotusFlame = new CyanLotusFlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 1f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(24, 36),
				Scale = 14f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = new float[] { Main.rand.NextFloat(-0.8f, 0.8f) },
			};
			Ins.VFXManager.Add(lotusFlame);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// target.GetGlobalNPC<EverglowGlobalNPC>().ElementDebuffs[ElementDebuffType.Necrosis].AddBuildUp(50, 0.2f);

		// Delete explain to enable value buff.
		// bool hasValueBuff = false;
		// for (int i = 0; i < ValueBuffSystem.AllBuffs.Count; i++)
		// {
		// var buff = ValueBuffSystem.AllBuffs[i];
		// if (buff.Target == target.whoAmI && buff.Type == 0)
		// {
		// hasValueBuff = true;
		// if(!buff.BreakOut)
		// {
		// ValueBuffSystem.AddValue(i, 12);
		// }
		// break;
		// }
		// }
		// if (!hasValueBuff)
		// {
		// ValueBuffSystem.NPCValueBuff lotusFlame = default(ValueBuffSystem.NPCValueBuff);
		// lotusFlame.Active = true;
		// lotusFlame.Type = 0;
		// lotusFlame.Target = target.whoAmI;
		// lotusFlame.Value = 12;
		// lotusFlame.ValueMax = 300;
		// ValueBuffSystem.AllBuffs.Add(lotusFlame);
		// }
	}
}
