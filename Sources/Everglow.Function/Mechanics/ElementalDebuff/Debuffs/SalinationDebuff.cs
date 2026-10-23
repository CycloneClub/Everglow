using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class SalinationDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(SalinationDebuff);

	public override string TypeID => ID;

	public override int ProcDamage { get; protected set; } = 40;

	public override Asset<Texture2D> Texture => ModAsset.Salination;

	public override Color Color => Color.LightBlue;

	public override void PostProc(NPC npc)
	{
		npc.AddBuff(BuffID.Frozen, 90);
	}
}
