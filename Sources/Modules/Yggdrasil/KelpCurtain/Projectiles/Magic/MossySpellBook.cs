using Everglow.SpellAndSkull.Projectiles;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class MossySpellBook : MagicBookProjectile
{
	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDef()
	{
		Projectile.DamageType = DamageClass.Magic;

		DustType = DustID.WaterCandle;
		ItemType = ModContent.ItemType<MossySpell>();

		TexCoordTop = new Vector2(16, -2);
		TexCoordLeft = new Vector2(-3, 17);
		TexCoordDown = new Vector2(13, 33);
		TexCoordRight = new Vector2(32, 14);
		effectColor = new Color(140, 180, 60, 175);
	}

	public override void OnSpawn(IEntitySource source)
	{
		BackGlowTexture = Commons.ModAsset.Empty.Value;
		GlowTexture = Commons.ModAsset.Empty.Value;
		BackTexture = ModAsset.MossySpellBack.Value;
		PaperTexture = ModAsset.MossySpellPaper.Value;
		FrontTexture = ModAsset.MossySpellFront.Value;
		base.OnSpawn(source);
	}

	public int UseCount = 0;

	public override void SpecialAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.itemTime <= 0 || player.HeldItem.type != ItemType)
		{
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		if (player.itemTime == 2 && player.HeldItem.type == ItemType)
		{
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Main.MouseWorld, new Vector2(0, 1), ModContent.ProjectileType<MossySpell_proj>(), player.HeldItem.damage * 2, player.HeldItem.knockBack, player.whoAmI);
			p.CritChance = player.GetWeaponCrit(player.HeldItem);

			// var p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (Main.MouseWorld - Projectile.Center).NormalizeSafe() * 16, ModContent.ProjectileType<MossySpell_proj>(), player.HeldItem.damage * 2, player.HeldItem.knockBack, player.whoAmI);
			// p2.CritChance = player.GetWeaponCrit(player.HeldItem);
		}
	}
}
