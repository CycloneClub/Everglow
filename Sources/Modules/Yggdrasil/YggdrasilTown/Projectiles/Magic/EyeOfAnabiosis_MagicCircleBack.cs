using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class EyeOfAnabiosis_MagicCircleBack : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	private const int MagicCircleHeight = 80;

	public override string Texture => ModAsset.EyeOfAnabiosis_Mod;

	private Player Owner => Main.player[Projectile.owner];

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 2;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
	}

	public override void AI()
	{
		Projectile.Center = Owner.MountedCenter;
		Projectile.velocity *= 0;

		UpdateLifetime();
		UpdateTextList();
	}

	public void UpdateLifetime()
	{
		if (Owner == null
			|| !Owner.active
			|| Owner.dead
			|| Owner.CCed
			|| Owner.noItems)
		{
			Projectile.Kill();
			return;
		}

		if (Owner.HeldItem.type == ModContent.ItemType<EyeOfAnabiosis>())
		{
			Projectile.timeLeft = 60;
		}
	}

	private List<EyeOfAnabiosis_MagicCircleText> TextList { get; set; } = [];

	private void UpdateTextList()
	{
		if (Main.rand.NextBool(8))
		{
			TextList.Add(new EyeOfAnabiosis_MagicCircleText());
		}

		foreach (EyeOfAnabiosis_MagicCircleText text in TextList)
		{
			text.Update();
		}

		TextList.RemoveAll(text => text.RelativePosition.Y < -MagicCircleHeight);
	}



	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var magicCirPosition = player.gravDir == 1 ? player.Bottom : player.Top;
		magicCirPosition = magicCirPosition - Main.screenPosition + new Vector2(0, 2 * player.gravDir);
		var magicCirRotation = player.gravDir == 1 ? 0 : MathF.PI;

		// Text of magic circle
		var textTexture = Commons.ModAsset.AlienWriting.Value;
		foreach (var text in TextList)
		{
			var textColor = text.Color * (1 + text.RelativePosition.Y / MagicCircleHeight) * 0.3f;
			Main.spriteBatch.Draw(textTexture, magicCirPosition + text.RelativePosition * player.gravDir, text.SourceRectangle, textColor, magicCirRotation, text.Origin, text.Scale, SpriteEffects.None, 0);
		}

		return false;
	}
}
