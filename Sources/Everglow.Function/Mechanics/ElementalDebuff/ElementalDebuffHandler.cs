using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff;

public abstract class ElementalDebuffHandler
{
	public const string Generic = nameof(Generic);

	public static string ID => null;

	public abstract string TypeID { get; }

	public ElementalDebuffInstance Instance { get; internal set; }

	#region Behavior

	/// <summary>
	/// Elemental resistance to this type of element debuff.
	/// </summary>
	public virtual float ElementalResistance { get; protected set; } = 0f;

	/// <summary>
	/// The element debuff build-up limitation. Once the build-up reaches this value, the element debuff will proc on its next update.
	/// </summary>
	public virtual int BuildUpMax { get; protected set; } = 1000;

	/// <summary>
	/// The duration of element debuff proc state.
	/// </summary>
	public virtual int Duration { get; protected set; } = 600;

	/// <summary>
	/// Negative life-regeneration units during proc; two units correspond to one HP per second at 60 updates per second.
	/// </summary>
	public virtual int DotDamage { get; protected set; } = 0;

	/// <summary>
	/// Damage on element debuff proc.
	/// </summary>
	public virtual int ProcDamage { get; protected set; } = 60;

	public ElementalDebuffHandler SetOverride(ElementalDebuffOverride overrideInfo)
	{
		if (overrideInfo == null)
		{
			return this;
		}

		if (overrideInfo.BuildUpMax.HasValue)
		{
			BuildUpMax = overrideInfo.BuildUpMax.Value;
		}
		if (overrideInfo.DurationMax.HasValue)
		{
			Duration = overrideInfo.DurationMax.Value;
		}
		if (overrideInfo.DotDamage.HasValue)
		{
			DotDamage = overrideInfo.DotDamage.Value;
		}
		if (overrideInfo.ProcDamage.HasValue)
		{
			ProcDamage = overrideInfo.ProcDamage.Value;
		}
		if (overrideInfo.ElementalResistance.HasValue)
		{
			ElementalResistance = overrideInfo.ElementalResistance.Value;
		}

		return this;
	}

	public ElementalDebuffHandler SetGenericOverride(ElementalDebuffOverride overrideInfo)
	{
		if (overrideInfo == null)
		{
			return this;
		}

		if (overrideInfo.ElementalResistance.HasValue)
		{
			ElementalResistance = overrideInfo.ElementalResistance.Value;
		}

		return this;
	}

	/// <summary>
	/// Custom effects before proc, and avoid proc.
	/// </summary>
	/// <param name="npc"></param>
	/// <returns></returns>
	public virtual bool PreProc(NPC npc)
	{
		return true;
	}

	/// <summary>
	/// Custom effects on proc.
	/// </summary>
	/// <param name="npc"></param>
	public virtual void PostProc(NPC npc)
	{
	}

	/// <summary>
	/// Custom effects during proc
	/// </summary>
	/// <param name="npc"></param>
	public virtual void UpdateProc(NPC npc)
	{
	}

	#endregion

	#region Display

	/// <summary>
	/// The icon of element debuff
	/// </summary>
	public abstract Asset<Texture2D> Texture { get; }

	/// <summary>
	/// The background color of element debuff in-game icon
	/// </summary>
	public abstract Color Color { get; }

	#endregion
}
