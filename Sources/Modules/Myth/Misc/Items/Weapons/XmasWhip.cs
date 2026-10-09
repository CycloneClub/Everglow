namespace Everglow.Myth.Misc.Items.Weapons;

public class XmasWhip : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonWeapons;

	public override void SetDefaults()
	{

		DefaultToWhip(ModContent.ProjectileType<Projectiles.Weapon.Summon.XmasWhip>(), 348, 2f, 5.4f, 30);
		Item.rare = ItemRarityID.Purple;
		Item.damage = 308;
		Item.value = Item.sellPrice(0, 10, 0, 0);
	}
	public override bool? UseItem(Player player)
	{
		if (player.autoReuseGlove)
		{
			Item.autoReuse = true;
			return true;
		}
		Item.autoReuse = false;
		return true;
	}
	private void DefaultToWhip(int projectileId, int dmg, float kb, float shootspeed, int animationTotalTime = 30)
	{
		Player player = Main.LocalPlayer;
		Item.autoReuse = false;
		if (player.autoReuseGlove)
		{
			Item.autoReuse = true;
		}

		Item.useStyle = 1;
		Item.useAnimation = animationTotalTime;
		Item.useTime = animationTotalTime;
		Item.width = 18;
		Item.height = 18;
		Item.shoot = projectileId;
		Item.UseSound = SoundID.Item152;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Summon;
		Item.noUseGraphic = true;
		Item.damage = dmg;
		Item.knockBack = kb;
		Item.shootSpeed = shootspeed;
	}
}
