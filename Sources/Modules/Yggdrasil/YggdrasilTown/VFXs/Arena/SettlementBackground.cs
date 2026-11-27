using System;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;

[Pipeline(typeof(ArenaMessageBackgroundPipeline))]
public class SettlementBackground : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public int Timer;

	public NPC BossNPC;

	public int NPCType = -1;

	public bool ShouldKill = false;

	/// <summary>
	/// 0: Success;1: Fail;2: Tie
	/// </summary>
	public int State;

	public override void Update()
	{
		if (BossNPC == null || !BossNPC.active)
		{
			foreach (var npc in Main.npc)
			{
				if (npc != null && npc.active && npc.ModNPC is TownNPC_LiveInYggdrasil)
				{
					BossNPC = npc;
					break;
				}
			}
		}
		if (NPCType == -1)
		{
			if (BossNPC != null)
			{
				NPCType = BossNPC.type;
			}
			else
			{
				NPCType = -1;
			}
		}
		if (!YggdrasilTownCentralSystem.InArena_YggdrasilTown() || NPCType == -1)
		{
			Active = false;
			return;
		}
		var tNLIY = BossNPC.ModNPC as TownNPC_LiveInYggdrasil;
		if (tNLIY == null)
		{
			Active = false;
			return;
		}
		if (!tNLIY.StartedFight && BossNPC.active)
		{
			ShouldKill = true;
		}
		if (ShouldKill)
		{
			if (Timer > 0)
			{
				Timer -= 10;
			}
			else
			{
				Timer = 0;
				Active = false;
			}
		}
		else
		{
			if (Timer <= 120)
			{
				Timer++;
			}
			else
			{
				Timer = 120;
			}
		}
		base.Update();
	}

	public override void Draw()
	{
		float pocession = Timer / 120f;
		if (pocession > 1)
		{
			pocession = 1;
		}
		var drawColor = new Color(0f, 0.75f, 0, 0f);
		if (State == 0)
		{
			drawColor = new Color(0f, 0.25f, 0, 0f);
		}
		float timeValue = (float)(Main.timeForVisualEffects * 0.002);
		List<Vertex2D> bars = new List<Vertex2D>();
		bars.Add(new Vector2(0), drawColor, new Vector3(0, 0, pocession));
		bars.Add(new Vector2(Main.screenWidth, 0), drawColor, new Vector3(1, 0, pocession));
		bars.Add(new Vector2(0, Main.screenHeight), drawColor, new Vector3(0, 1, pocession));
		bars.Add(new Vector2(Main.screenWidth, Main.screenHeight), drawColor, new Vector3(1, 1, pocession));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}

	public bool MouseInArea(Vector2 drawPos, Vector2 size)
	{
		Vector2 mousePos = Main.MouseWorld;
		if (mousePos.X >= drawPos.X && mousePos.X <= drawPos.X + size.X)
		{
			if (mousePos.Y >= drawPos.Y && mousePos.Y <= drawPos.Y + size.Y)
			{
				return true;
			}
		}
		return false;
	}
}
