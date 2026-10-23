using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class BurnDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(BurnDebuff);

	public override string TypeID => ID;

	public override Asset<Texture2D> Texture => ModAsset.Burn;

	public override Color Color => new Color(0.7f, 0.2f, 0, 0);

	public override void PostProc(NPC npc)
	{
		npc.AddBuff(BuffID.OnFire3, 360);
	}
}
