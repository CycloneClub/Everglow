using Everglow.SpellAndSkull.Items;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class MossySpell : SpellTomeItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 13;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 4;
		Item.width = 34;
		Item.height = 46;
		Item.useTime = 16;
		Item.useAnimation = 16;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 2.5f;
		Item.value = Item.sellPrice(0, 0, 20, 0);
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item42;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<MossySpell_proj>();
		Item.shootSpeed = 12f;
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<MossySpellBook>());
		DecorativeProjectileTypes.Add(ModContent.ProjectileType<MossySpellArray>());
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<SpellAndSkull.GlobalItems.MagicBookPlayer>().MagicBookLevel > 0)
		{
			return false;
		}
		for (int t = 0; t < 15; t++)
		{
			Vector2 phi = new Vector2(0, Main.rand.Next(12, 18)).RotatedBy(t / 15f * MathHelper.TwoPi);
			var dustVFX4 = new MossBlossomDustSide
			{
				velocity = phi.RotatedBy(0.7f) * 0.2f,
				Active = true,
				Visible = true,
				position = Main.MouseWorld + phi * 0.4f,
				maxTime = Main.rand.Next(15, 40),
				scale = Main.rand.NextFloat(12, 16),
				rotation = phi.RotatedBy(0.7f).ToRotationSafe() - MathHelper.PiOver4 * 3,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX4);
		}
		for (int t = 0; t < 15; t++)
		{
			Vector2 phi = new Vector2(0, Main.rand.Next(0, 18)).RotatedBy(t / 15f * MathHelper.TwoPi);
			var dustVFX4 = new MossBlossomDustFace
			{
				velocity = phi.RotatedBy(0.7f) * 0.2f,
				Active = true,
				Visible = true,
				position = Main.MouseWorld + phi * 0.4f,
				maxTime = Main.rand.Next(15, 40),
				scale = Main.rand.NextFloat(12, 16),
				rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX4);
		}
		Projectile.NewProjectileDirect(source, Main.MouseWorld, new Vector2(Main.rand.NextFloat(-0.13f, 0.13f), 2f), type, damage, knockback, player.whoAmI);
		return false;
	}
}
