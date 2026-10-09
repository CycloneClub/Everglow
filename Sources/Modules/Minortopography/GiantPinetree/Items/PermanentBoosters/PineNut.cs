using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Everglow.Minortopography.GiantPinetree.Items.PermanentBoosters;

// 永久增加3HP,最多使用5次
internal class PineNut : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.PermanentBoosters;

	public static readonly int MaxPineNutCount = 5;
	public static readonly int LifePerFruit = 3;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit, MaxPineNutCount);

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 10;
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.LifeCrystal);
		Item.width = 14;
		Item.height = 20;
		Item.rare = ItemRarityID.Green;
		Item.value = 8000;
		Item.useStyle = ItemUseStyleID.EatFood;
	}

	public override bool CanUseItem(Player player)
	{
		// This check prevents this item from being used before vanilla health upgrades are maxed out.
		return true;
	}

	public override bool? UseItem(Player player)
	{
		// Moving the PineNutCount check from CanUseItem to here allows this example fruit to still "be used" like Life Fruit can be
		// when at the max allowed, but it will just play the animation and not affect the player's max life
		if (player.GetModPlayer<PineNutPlayer>().PineNutCount >= MaxPineNutCount)
		{
			// Returning null will make the item not be consumed
			return null;
		}

		// This method handles permanently increasing the player's max health and displaying the green heal text
		player.UseHealthMaxIncreasingItem(LifePerFruit);

		// This field tracks how many of the example fruit have been consumed
		player.GetModPlayer<PineNutPlayer>().PineNutCount++;

		return true;
	}
}
public class PineNutPlayer : ModPlayer
{
	public int PineNutCount;

	public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
	{
		health = StatModifier.Default;
		health.Base = PineNutCount * PineNut.LifePerFruit;
		mana = StatModifier.Default;
	}

	public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
	{
	}

	// Called in ExampleMod.Networking.cs
	public void ReceivePlayerSync(BinaryReader reader)
	{
		PineNutCount = reader.ReadByte();
	}

	public override void CopyClientState(ModPlayer targetCopy)
	{
		var clone = (PineNutPlayer)targetCopy;
		clone.PineNutCount = PineNutCount;
	}

	public override void SendClientChanges(ModPlayer clientPlayer)
	{
		var clone = (PineNutPlayer)clientPlayer;

		if (PineNutCount != clone.PineNutCount)
		{
			SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
		}
	}
	public override void SaveData(TagCompound tag)
	{
		tag["pineNut"] = PineNutCount;
	}

	public override void LoadData(TagCompound tag)
	{
		PineNutCount = tag.GetInt("pineNut");
	}
}
