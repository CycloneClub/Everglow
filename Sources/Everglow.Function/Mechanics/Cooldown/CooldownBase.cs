using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Cooldown;

public abstract class CooldownBase
{
	public const string LocalizationCategory = "Mods.Everglow.Cooldowns";
	public const string LocalizationDisplayNameKey = "DisplayName";
	public const string LocalizationDescriptionKey = "Description";

	public static string ID => null;

	public abstract string TypeID { get; }

	public CooldownInstance Instance { get; set; }

	#region Ingame Behavior

	/// <summary>
	/// Advance in PostUpdateMiscEffects. Disable when an existing gameplay hook
	/// calls PlayerUtils.TickCooldown to preserve its update order and pause conditions.
	/// </summary>
	public virtual bool AutomaticTick => true;

	public virtual bool CanTickDown => true;

	/// <summary>
	/// Allow the cooldown to persist after the player dies.
	/// <para/>Same to <see cref="Main.persistentBuff"/>.
	/// </summary>
	public virtual bool PersistentCooldown => false;

	/// <summary>
	/// Set this buff to not be saved when exiting a world.
	/// <para/>Same to <see cref="Main.buffNoSave"/>.
	/// </summary>
	public virtual bool CooldownNoSave => false;

	/// <summary>
	/// This method runs every frame while the cooldown instance is active.
	/// </summary>
	public virtual void Update()
	{
	}

	/// <summary>
	/// This method runs when the cooldown instance ends naturally.<br/>
	/// It is not called if the cooldown instance is deleted because the player died.
	/// </summary>
	public virtual void OnCompleted()
	{
	}
	#endregion

	#region Display

	public virtual string DisplayName => Language.GetTextValue($"{LocalizationCategory}.{TypeID}.{LocalizationDisplayNameKey}");

	public virtual string Discription => Language.GetTextValue($"{LocalizationCategory}.{TypeID}.{LocalizationDescriptionKey}");

	public virtual bool IsVisible => true;

	public abstract Texture2D Texture { get; }

	public virtual bool EnableCutShader => false;

	public virtual Vector2 TextureScale => Vector2.One;

	public virtual Vector2 TextureOffset => Vector2.Zero;

	public virtual Color StartColor => Color.Red;

	public virtual Color EndColor => Color.Green;

	#endregion
}
