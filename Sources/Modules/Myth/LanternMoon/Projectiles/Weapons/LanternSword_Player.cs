using Everglow.Myth.LanternMoon.Items.Weapons;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternSword_Player : ModPlayer
{
	public float LerpValue = 0;

	public bool Active = false;

	public Vector2 OldPos;

	public override void ModifyScreenPosition()
	{
		if (Active)
		{
			if (Player.HeldItem.type == ModContent.ItemType<LanternSword>())
			{
				if (LerpValue > 0)
				{
					LerpValue -= 0.1f;
				}
				else
				{
					LerpValue = 0;
				}
				float wavedLerpValue = (1 + MathF.Cos(LerpValue * MathHelper.Pi)) * 0.5f;
				Main.screenPosition = LerpValue.Lerp(Main.screenPosition, OldPos);
			}
			else
			{
				Active = false;
				LerpValue = 0;
			}
		}
		base.ModifyScreenPosition();
	}
}
