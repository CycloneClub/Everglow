using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class FrostDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(FrostDebuff);

	public override string TypeID => ID;

	public override int ProcDamage { get; protected set; } = 40;

	public override Asset<Texture2D> Texture => ModAsset.Frost;

	public override Color Color => Color.LightSkyBlue;

	public override void PostProc(NPC npc)
	{
		npc.AddBuff(BuffID.Frozen, 90);
	}
}
