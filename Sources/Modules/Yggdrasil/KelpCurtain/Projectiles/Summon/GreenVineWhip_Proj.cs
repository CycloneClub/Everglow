using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class GreenVineWhip_Proj : WhipProjectile
{
	public override void SetDef()
	{
		WhipLength = 420;
		DustType = ModContent.DustType<YggdrasilCyatheaLeafDust>();
	}

	public override void GenerateDusts() => base.GenerateDusts();

	public override void DrawWhip(float foreStep = 0) => base.DrawWhip(foreStep);

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
		player.AddBuff(ModContent.BuffType<MossCover>(), 300);
		if (Main.rand.NextBool(4))
		{
			target.AddBuff(BuffID.Poisoned, 300);
		}
		base.OnHitNPC(target, hit, damageDone);
	}
}
