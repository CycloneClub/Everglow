using Everglow.Commons.Utilities;
using Everglow.Commons.Mechanics.Cooldown;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ElderberryBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ElderberryBuff");
		// Description.SetDefault("你可以短距离冲刺\n“抗氧化”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		ElderberryBuffDash ElderberryBuffDash = player.GetModPlayer<ElderberryBuffDash>();
		ElderberryBuffDash.ElderberryBuff = true;
	}
}

public class ElderberryBuffDash : ModPlayer
{
	public const int DashDown = 0;
	public const int DashUp = 1;
	public const int DashRight = 2;
	public const int DashLeft = 3;

	public const int DashCooldown = 50;
	public const int DashDuration = 35;

	public const float DashVelocity = 8f;

	public int DashDir = -1;

	public bool ElderberryBuff;
	public int DashTimer = 0;

	public override void ResetEffects()
	{
		ElderberryBuff = false;

		if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
		{
			DashDir = DashDown;
		}
		else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
		{
			DashDir = DashUp;
		}
		else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
		{
			DashDir = DashRight;
		}
		else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
		{
			DashDir = DashLeft;
		}
		else
		{
			DashDir = -1;
		}
	}

	public override void PreUpdateMovement()
	{
		if (CanUseDash() && DashDir != -1 && !Player.HasCooldown<ElderberryDashCooldown>())
		{
			Vector2 newVelocity = Player.velocity;

			switch (DashDir)
			{
				case DashUp when Player.velocity.Y > -DashVelocity:
				case DashDown when Player.velocity.Y < DashVelocity:
					{
						float dashDirection = DashDir == DashDown ? 1 : -1.3f;
						newVelocity.Y = dashDirection * DashVelocity;
						break;
					}
				case DashLeft when Player.velocity.X > -DashVelocity:
				case DashRight when Player.velocity.X < DashVelocity:
					{
						float dashDirection = DashDir == DashRight ? 1 : -1;
						newVelocity.X = dashDirection * DashVelocity;
						break;
					}
				default:
					return;
			}

			Player.AddCooldown(ElderberryDashCooldown.ID, DashCooldown);
			DashTimer = DashDuration;
			Player.velocity = newVelocity;
		}

		Player.TickCooldown(ElderberryDashCooldown.ID);

		if (DashTimer > 0)
		{
			Player.eocDash = DashTimer;
			Player.armorEffectDrawShadowEOCShield = true;

			DashTimer--;
		}
	}

	private bool CanUseDash()
	{
		return ElderberryBuff
			&& Player.dashType == 0
			&& !Player.setSolar
			&& !Player.mount.Active;
	}
}

public class ElderberryDashCooldown : CooldownBase
{
	public static new string ID => nameof(ElderberryDashCooldown);

	public override string TypeID => ID;

	public override bool AutomaticTick => false;

	public override bool CanTickDown => true;

	public override bool PersistentCooldown => true;

	public override bool CooldownNoSave => true;

	public override bool IsVisible => false;

	public override Texture2D Texture => ModAsset.ElderberryBuff.Value;
}
