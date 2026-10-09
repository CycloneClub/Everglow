using Everglow.SpellAndSkull.Projectiles;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.BoneFeatherMagic;

internal class BoneFeatherMagicBook : MagicBookProjectile
{
	public override string Texture => "Everglow/" + ModAsset.BoneFeatherMagic_Path;
	public override void SetDef()
	{
		ProjType = ModContent.ProjectileType<BoneFeather>();
		DustType = ModContent.DustType<Dusts.BoneFeather>();
		ItemType = ModContent.ItemType<Items.Weapons.BoneFeatherMagic>();
		MulStartPosByVelocity = 2f;
		UseGlow = true;

		FrontTexture = ModAsset.BoneFeatherMagic.Value;
		PaperTexture = ModAsset.FireFeatherMagic_paper.Value;
		GlowTexture = ModAsset.BoneFeatherMagic.Value;
		GlowColor = Color.Transparent;
		effectColor = new Color(50, 50, 31, 40);
		TexCoordTop = new Vector2(10, 0);
		TexCoordLeft = new Vector2(-1, 29);
		TexCoordDown = new Vector2(28, 39);
		TexCoordRight = new Vector2(43, 9);
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f; // 书跟着玩家飞
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ItemType && player.active && !player.dead)// 检测手持物品
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (timer < 30)
			{
				timer++;
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full; // 玩家动作

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;
		SpecialAI();
		if (ProjType == -1)
		{
			return;
		}

		if (player.itemTime == player.itemTimeMax - 2 && player.HeldItem.type == ItemType)
		{
			for (int x = 0; x < 4; x++)
			{
				var p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -4) + new Vector2(0, Main.rand.NextFloat(50f)).RotateRandom(6.283) + Vector2.Normalize(vTOMouse) * 12, Vector2.Zero, ModContent.ProjectileType<BoneShoot>(), (int)(player.HeldItem.damage * MulDamage), player.HeldItem.knockBack, player.whoAmI, Main.rand.NextFloat(0.5f, 2.5f), 0, player.HeldItem.shootSpeed);
				p2.timeLeft = Main.rand.Next(200, 300);
			}
		}
	}
}
