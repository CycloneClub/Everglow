using Terraria.DataStructures;

namespace Everglow.Commons.MEAC;

public class TestPlayerDrawer : ModPlayer
{
	public bool HideLeg = false;
	public float HeadRotation = 0;
	public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
	{
		base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
	}
	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		Player.headRotation = HeadRotation;
		if (HideLeg)
		{
			if (Player.gravDir == 1)
			{
				Player.bodyPosition = new Vector2(0, 2); // 偷偷调个参,防止腿太短的玩家裂开
			}

			drawInfo.hidesBottomSkin = true;
		}
		base.ModifyDrawInfo(ref drawInfo);
	}
}
