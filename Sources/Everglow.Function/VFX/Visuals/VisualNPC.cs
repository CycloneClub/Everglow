using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;

namespace Everglow.Commons.VFX.Visuals;

public abstract class VisualNPC : ModNPC, IVisual
{
	public bool Active => NPC.active && Main.npc[NPC.whoAmI] == NPC;

	public virtual CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public bool Visible => !NPC.hide && VFXManager.InScreen(NPC.position, 100);

	public virtual void Draw()
	{
	}

	public override void SetDefaults()
	{
		if (!Main.gameMenu)
		{
			Ins.VFXManager.Add(this);
		}
	}

	public void Kill()
	{
	}

	public void Update()
	{
	}
}
