using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.ElementalDebuff;

public sealed class ElementalDebuffInstance
{
	public ElementalDebuffInstance()
	{
	}

	public ElementalDebuffInstance(NPC npc, ElementalDebuffNet eDN)
	{
		this.NPC = npc;
		this.NetID = eDN.NetID;

		Type eDHandlerT = ElementalDebuffRegistry.NameToType[eDN.ID];
		var handler = Activator.CreateInstance(eDHandlerT) as ElementalDebuffHandler;
		handler.Instance = this;
		handler.SetOverride(ElementalDebuffOverrideRegistry.GetOverride(npc, eDN.ID))
			.SetGenericOverride(ElementalDebuffOverrideRegistry.GetOverride(npc, ElementalDebuffHandler.Generic));
		this.Handler = handler;
	}

	/// <summary>
	/// The type of element debuff
	/// </summary>
	public ushort NetID { get; }

	public NPC NPC { get; private set; }

	public ElementalDebuffHandler Handler { get; private set; }

	/// <summary>
	/// The element debuff build-up, use <see cref="AddBuildUp"/> to edit this property.
	/// </summary>
	public int BuildUp { get; internal set; } = 0;

	public float BuildUpProgress => Math.Clamp(BuildUp / (float)Handler.BuildUpMax, 0f, 1f);

	/// <summary>
	/// If the element debuff has build-up.
	/// </summary>
	public bool HasBuildUp => BuildUp > 0;

	private StatModifier elementalResistanceStatModifier = StatModifier.Default;

	/// <summary>
	/// Use this to edit npc's resistance to this type of elemetal debuff
	/// <br/>Add: <see cref="ElementalResistanceModifier"/> += 0.1f;
	/// </summary>
	public ref StatModifier ElementalResistanceModifier => ref elementalResistanceStatModifier;

	/// <summary>
	/// Current elemental resistance value.
	/// </summary>
	public float ElementalResistance => ElementalResistanceModifier.ApplyTo(Handler.ElementalResistance);

	/// <summary>
	/// Determine whether this element debuff is in proc state.
	/// <para/>This property should only be edit by <see cref="Update(NPC)"/>.
	/// </summary>
	public bool Proc { get; private set; } = false;

	public int LastInteraction { get; private set; } = -1;

	/// <summary>
	/// Determine which player procced this delement debuff.
	/// <para/> Default to -1, and has value <see cref="Entity.whoAmI"/> of <see cref="Player"/> during proc state, <c>255</c> is server.
	/// </summary>
	public int ProccedBy { get; private set; } = -1;

	/// <summary>
	/// The timer of element debuff proc state.
	/// </summary>
	public int TimeLeft { get; private set; } = 0;

	public float TimeProgress => Math.Clamp(TimeLeft / (float)Handler.Duration, 0f, 1f);

	/// <summary>
	/// Add element debuff build-up, calculate resistance and penetration.
	/// </summary>
	/// <param name="buildUp">Build-up value</param>
	/// <param name="elementPenetration">Element penetration. If penetration is less than 0, it will be count as 0.</param>
	/// <returns></returns>
	public bool AddBuildUp(int buildUp, int sourcePlayer, float elementPenetration = 0)
	{
		if (Proc || buildUp <= 0)
		{
			return false;
		}
		else
		{
			if (elementPenetration < 0)
			{
				elementPenetration = 0;
			}

			// Calculate element resistance and penetration
			float finElementResisitance = ElementalResistance - elementPenetration;
			if (finElementResisitance < 0)
			{
				finElementResisitance = 0;
			}
			else if (finElementResisitance > 1)
			{
				finElementResisitance = 1;
			}

			int addedBuildUp = (int)(buildUp * (1 - finElementResisitance));
			BuildUp += addedBuildUp;
			if (addedBuildUp > 0)
			{
				LastInteraction = sourcePlayer;
			}

			return true;
		}
	}

	/// <summary>
	/// Update the debuff and apply proc effect to npc
	/// <para/> Should only be called in <see cref="GlobalNPC.AI(NPC)"/>
	/// </summary>
	/// <param name="npc"></param>
	public void Update(NPC npc)
	{
		if (Proc)
		{
			TimeLeft--;

			Handler.UpdateProc(npc);

			if (TimeLeft <= 0)
			{
				TimeLeft = 0;
				Proc = false;
				ProccedBy = -1;
			}
		}
		else
		{
			if (BuildUp >= Handler.BuildUpMax)
			{
				OnProc(npc);
			}
		}
	}

	/// <summary>
	/// Apply proc damage to npc, and set duration to max
	/// </summary>
	/// <param name="npc"></param>
	public void OnProc(NPC npc)
	{
		if (!Handler.PreProc(npc))
		{
			return;
		}

		Proc = true;
		ProccedBy = LastInteraction;
		BuildUp = 0;
		TimeLeft = Handler.Duration;

		if (!npc.dontTakeDamage && Main.myPlayer == ProccedBy)
		{
			var modifier = npc.GetIncomingStrikeModifiers(DamageClass.Default, 0);
			var hitInfo = modifier.ToHitInfo(Handler.ProcDamage, false, 0);
			npc.StrikeNPC(hitInfo);

			if (!NetUtils.IsSingle)
			{
				NetMessage.SendStrikeNPC(npc, hitInfo);
			}
		}

		Handler.PostProc(npc);
	}

	/// <summary>
	/// Apply proc state effect dot damage to npc, which can reduce npc's life regen.
	/// <para/> Should only be called in <see cref="GlobalNPC.UpdateLifeRegen(NPC, ref int)"/>
	/// </summary>
	/// <param name="npc"></param>
	public void ApplyDot(NPC npc)
	{
		if (Proc)
		{
			if (npc.dontTakeDamage)
			{
				return;
			}
			npc.lifeRegen -= Handler.DotDamage;
			npc.SetLifeRegenExpectedLossPerSecond(Handler.DotDamage);
		}
	}

	/// <summary>
	/// Be called in <see cref="GlobalNPC.ResetEffects(NPC)"/>
	/// </summary>
	public void ResetEffects()
	{
		ElementalResistanceModifier = StatModifier.Default;
	}

	public void Reset()
	{
		BuildUp = 0;
		TimeLeft = 0;
		Proc = false;
		ProccedBy = -1;
		elementalResistanceStatModifier = StatModifier.Default;
	}

	public bool HasValue() => BuildUp != 0 || TimeLeft != 0 || Proc;

	public void NetSend(BinaryWriter writer)
	{
		writer.Write(Proc);
		writer.Write(BuildUp);
		writer.Write(TimeLeft);
	}

	public void NetReceive(BinaryReader reader)
	{
		bool flag = Proc;

		Proc = reader.ReadBoolean();
		BuildUp = reader.ReadInt32();
		TimeLeft = reader.ReadInt32();

		if (!flag && Proc)
		{
			OnProc(NPC);
		}
	}

	public static void EmptyNetReceive(BinaryReader reader)
	{
		reader.ReadBoolean();
		reader.ReadInt32();
		reader.ReadInt32();
	}
}
