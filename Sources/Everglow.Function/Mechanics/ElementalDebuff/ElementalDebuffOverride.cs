namespace Everglow.Commons.Mechanics.ElementalDebuff;

/// <summary>
/// Override the base infomations of a specified type of <see cref="ElementalDebuffHandler"/>.
/// </summary>
/// <param name="BuildUpMax"></param>
/// <param name="DurationMax"></param>
/// <param name="DotDamage"></param>
/// <param name="ProcDamage"></param>
/// <param name="ElementalResistance"></param>
public record ElementalDebuffOverride(int? BuildUpMax = null, int? DurationMax = null, int? DotDamage = null, int? ProcDamage = null, float? ElementalResistance = null);
