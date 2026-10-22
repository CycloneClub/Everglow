using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

public enum CrowdDensity
{
	Sparse,
	Standard,
	Busy,
}

/// <summary>Local presentation preferences, shared by all characters on this client.</summary>
public class CrowdConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[DefaultValue(true)]
	public bool Enabled = true;

	[DefaultValue(CrowdDensity.Standard)]
	[DrawTicks]
	public CrowdDensity Density = CrowdDensity.Standard;
}
