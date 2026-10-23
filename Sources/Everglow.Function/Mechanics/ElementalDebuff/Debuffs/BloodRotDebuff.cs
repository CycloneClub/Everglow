using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class BloodRotDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(BloodRotDebuff);

	public override string TypeID => ID;

	public override int ProcDamage { get; protected set; } = 80;

	public override Asset<Texture2D> Texture => ModAsset.BloodRot;

	public override Color Color => Color.DarkRed;

	public override void PostProc(NPC npc)
	{
		npc.AddBuff(BuffID.Bleeding, 360);
	}
}
