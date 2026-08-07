using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Terraria.GameContent.Personalities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;

public class ExploreObjective : PlayerObjectiveBase
{
	public ExploreObjective()
	{
	}

	public ExploreObjective(IShoppingBiome biome, float moveRequirement = 0)
	{
		if (MoveRequirement < 0)
		{
			throw new ArgumentOutOfRangeException("Move requirement should not less than 0.");
		}

		Biome = biome;
		MoveRequirement = moveRequirement;
		distanceMoved = 0;
	}

	private float distanceMoved;

	public float MoveRequirement { get; init; }

	public IShoppingBiome Biome { get; init; }

	public override bool CheckCompletion() => MoveRequirement > 0
		? distanceMoved >= MoveRequirement
		: Biome.IsInBiome(Main.LocalPlayer);

	public override void Update()
	{
		base.Update();

		if (Biome.IsInBiome(Main.LocalPlayer))
		{
			distanceMoved += Main.LocalPlayer.velocity.Length();
		}
	}

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
	}

	public override void GetObjectivesText(List<string> lines)
	{
		var biomeName = Biome is ModBiome modBiome
			? modBiome.DisplayName.ToString()
			: Biome.NameKey;

		if (MoveRequirement > 0)
		{
			lines.Add($"在{biomeName}中走过{MoveRequirement}米. ({Math.Round(distanceMoved)}/{MoveRequirement})");
		}
		else
		{
			lines.Add("探索" + biomeName);
		}
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);

		tag.TryGet(nameof(distanceMoved), out distanceMoved);
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);

		tag.Add(nameof(distanceMoved), distanceMoved);
	}
}
