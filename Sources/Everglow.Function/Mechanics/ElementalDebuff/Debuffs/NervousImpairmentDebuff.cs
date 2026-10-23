using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class NervousImpairmentDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(NervousImpairmentDebuff);

	public override string TypeID => ID;

	public override Asset<Texture2D> Texture => ModAsset.NervousImpairment;

	public override Color Color => Color.LightSeaGreen;

	public override void PostProc(NPC npc)
	{
		npc.AddBuff(BuffID.Confused, 180);
	}
}
