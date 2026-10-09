namespace Everglow.Myth.Misc.FixCoins;

public abstract class FixCoinItem : ModItem
{
	/// <summary>
	/// 品阶,白色1绿色2蓝色3紫色4黄色5别的自己定
	/// </summary>
	/// <returns></returns>
	public virtual int Level()
	{
		return 1;
	}
	/// <summary>
	/// 额外属性设置
	/// </summary>
	public virtual void SSD()
	{
	}
	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.value = 1000 * 2 ^ Level();
		Item.maxStack = 999;
		Item.shootSpeed = 16;

		Item.noMelee = true;
		Item.consumable = true;
		Item.noUseGraphic = true;

		Item.UseSound = SoundID.Item1;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.rare = ItemRarityID.White;

		SSD();
	}
	public override bool CanUseItem(Player player)
	{
		foreach (Item item in player.inventory)
		{
			if (item.accessory)
			{
				return true;
			}
		}
		// TODO:你的背包里没有饰品
		// string tex3 = "There's no accessory in your inventory";
		// if (Language.ActiveCulture.Name == "zh-Hans")
		// {
		//    tex3 = "你的背包中没有饰品";
		// }
		// CombatText.NewText(new Rectangle((int)player.Center.X - 10, (int)player.Center.Y - 10, 20, 20), Color.White, tex3);
		return false;
	}
}
