namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.IRProbe;

public class IRVision_GlobalItem : GlobalItem
{
	public List<int> IR_Visualize_Item = new List<int>() { ItemID.Sunglasses, ItemID.AviatorSunglasses };

	public override bool InstancePerEntity => true;

	public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
	{
		// if(IR_Visualize_Item == new List<int>())
		// {
		// IR_Visualize_Item.AddRange(new List<int>() { ItemID.Sunglasses, ItemID.AviatorSunglasses });
		// }
		base.Update(item, ref gravity, ref maxFallSpeed);
	}

	public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	{
		if (IR_Visualize_Item.Contains(item.type))
		{
			player.GetModPlayer<VisionPlayer>().IR_Vision = true;
		}
		base.UpdateAccessory(item, player, hideVisual);
	}

	public override void UpdateEquip(Item item, Player player)
	{
		if (IR_Visualize_Item.Contains(item.type))
		{
			player.GetModPlayer<VisionPlayer>().IR_Vision = true;
		}
		base.UpdateEquip(item, player);
	}

	public override void UpdateInventory(Item item, Player player)
	{
		if (IR_Visualize_Item.Contains(item.type))
		{
			player.GetModPlayer<VisionPlayer>().IR_Vision = true;
		}
		base.UpdateInventory(item, player);
	}
}

public class VisionPlayer : ModPlayer
{
	public bool IR_Vision = false;

	public override void ResetEffects()
	{
		IR_Vision = false;
		base.ResetEffects();
	}
}
