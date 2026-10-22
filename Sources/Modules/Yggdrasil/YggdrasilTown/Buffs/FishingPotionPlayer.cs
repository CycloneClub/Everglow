namespace Everglow.Yggdrasil.YggdrasilTown.Buffs;

public class FishingPotionPlayer : ModPlayer
{
	public bool JumpPotionActive { get; set; }

	public bool ScalePotionActive { get; set; }

	public override void ResetEffects()
	{
		JumpPotionActive = false;
		ScalePotionActive = false;
	}

	public override void PostUpdateRunSpeeds()
	{
		if (!JumpPotionActive || Player.mount.Active || Player.gravity <= 0f || Player.jumpSpeed <= Player.gravity)
		{
			return;
		}

		// 原版此时已为当前玩家重算 jumpSpeed/jumpHeight，随后才执行 JumpMovement。
		// 跳跃由按住跳跃的匀速段和松开后的减速段组成，不能把速度直接乘 1.15。
		// H ≈ t*v + v²/(2*g)；求增加 15% 高度所需的速度，保持跳跃持续帧数不变。
		float gravity = Player.gravity;
		float speed = Player.jumpSpeed - gravity;
		float hold = gravity * (Player.jumpHeight + 0.5f);
		Player.jumpSpeed = MathF.Sqrt(hold * hold + 1.15f * speed * (speed + 2f * hold)) - hold + gravity;
	}

	public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
	{
		if (ScalePotionActive)
		{
			modifiers.FinalDamage *= 0.94f;
		}
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		// 原版以 ByOther(0) 标记普通摔落，ByOther(5) 标记石化摔落。
		if (JumpPotionActive && modifiers.DamageSource.SourceOtherIndex is 0 or 5)
		{
			modifiers.FinalDamage *= 0.7f;
		}
	}
}
