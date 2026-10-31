using Everglow.SpellAndSkull.Projectiles;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles.DreamWeaver;

internal class DreamWeaverBook : MagicBookProjectile
{
	public override void SetDef()
	{
		DustType = DustID.WaterCandle;
		ItemType = ModContent.ItemType<TheFirefly.Items.Weapons.DreamWeaver>();

		TexCoordTop = new Vector2(9, 0);
		TexCoordLeft = new Vector2(0, 34);
		TexCoordDown = new Vector2(21, 46);
		TexCoordRight = new Vector2(34, 9);
		effectColor = new Color(0, 0, 55, 175);
	}

	public override void OnSpawn(IEntitySource source)
	{
		GlowTexture = ModAsset.DreamWeaverFrontGlow.Value;
		BackTexture = ModAsset.DreamWeaverBack.Value;
		BackGlowTexture = ModAsset.DreamWeaverBackGlow.Value;
		PaperTexture = ModAsset.DreamWeaverPaper.Value;
		FrontTexture = ModAsset.DreamWeaverFront.Value;
		base.OnSpawn(source);
	}

	public int UseCount = 0;

	public override void SpecialAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.itemTime <= 0 || player.HeldItem.type != ModContent.ItemType<TheFirefly.Items.Weapons.DreamWeaver>())
		{
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		if (player.itemTime == 2 && player.HeldItem.type == ItemType)
		{
			Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * player.HeldItem.shootSpeed * 3.3f;
			UseCount++;
			if (UseCount >= 6)
			{
				UseCount = 0;
			}
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity * -2 + new Vector2(0, -10), velocity, ModContent.ProjectileType<DreamWeaverII>(), player.HeldItem.damage * 3 / 2, player.HeldItem.knockBack, player.whoAmI);
			p.CritChance = player.GetWeaponCrit(player.HeldItem);
		}
	}
}
