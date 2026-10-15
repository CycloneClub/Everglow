namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public class ElectroProbe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public int OwnHelperCooling = 0;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;
		Item.useTime = 6;
		Item.useAnimation = 6;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.White;
		Item.value = 0;
	}

	public override void HoldItem(Player player)
	{
		if (OwnHelperCooling == 0)
		{
			var electroprobeHelper = new ElectroProbe_HitWireHelper
			{
				Active = true,
				Visible = true,
				MyOwner = player,
				MyProbe = Item,
			};
			Ins.VFXManager.Add(electroprobeHelper);
			OwnHelperCooling++;
		}
		Main.instance.MouseText("[i:" + Item.type + "]");
	}

	public override void UpdateInventory(Player player)
	{
		if (player.HeldItem != Item)
		{
			OwnHelperCooling--;
			if (OwnHelperCooling <= 0)
			{
				OwnHelperCooling = 0;
			}
		}
		base.UpdateInventory(player);
	}

	public override bool CanUseItem(Player player)
	{
		if ((Main.MouseWorld - player.Center).Length() <= 300)
		{
			Point point = Main.MouseWorld.ToTileCoordinates();
			int i = point.X;
			int j = point.Y;

			Wiring.HitWireSingle(i, j);
			Wiring.TripWire(i, j, 1, 1);
		}
		return base.CanUseItem(player);
	}
}
