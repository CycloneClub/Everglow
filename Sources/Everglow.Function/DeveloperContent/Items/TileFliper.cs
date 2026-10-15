namespace Everglow.Commons.DeveloperContent.Items;

public class TileFliper : ModItem
{
	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.Tools;

	public bool EnableFlipSystem = false;

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Magic;
		Item.damage = 0;
		Item.width = 34;
		Item.height = 34;
		Item.useTime = 2;
		Item.useAnimation = 2;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item44;
		Item.noUseGraphic = true;
	}

	public override void HoldItem(Player player)
	{
		// if(Main.mouseLeft && Main.mouseLeftRelease && !EnableFlipSystem)
		// {
		// EnableFlipSystem = true;
		// }
		if (!EnableFlipSystem)
		{
			EnableFlipSystem = true;
			TileFliperSystemVFX tileFliperSystemVFX = new TileFliperSystemVFX()
			{
				Owner = player,
				Active = true,
				Visible = true,
			};
			Ins.VFXManager.Add(tileFliperSystemVFX);
		}
		base.HoldItem(player);
	}
}
