using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.GameContent.Personalities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class ExploreObjective : PlayerObjectiveBase
{
	public ExploreObjective()
	{
	}

	public ExploreObjective(IShoppingBiome biome, float moveRequirement = 0)
	{
		if (moveRequirement < 0)
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
			distanceMoved += Main.LocalPlayer.velocity.Length() * PlayerQuestManager.UpdateInterval;
		}
	}

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
	}

	public override string GetObjectiveText()
	{
		var biomeName = Biome is ModBiome modBiome
			? modBiome.DisplayName.ToString()
			: Terraria.GameContent.ShopHelper.BiomeNameByKey(Biome.NameKey);

		if (MoveRequirement > 0)
		{
			return QuestText.Get("Objectives.ExploreDistance", biomeName, MoveRequirement, Math.Round(distanceMoved));
		}

		return QuestText.Get("Objectives.Explore", biomeName);
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		distanceMoved = 0;
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
