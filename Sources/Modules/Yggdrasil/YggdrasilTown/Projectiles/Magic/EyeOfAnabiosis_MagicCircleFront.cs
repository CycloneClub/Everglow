using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class EyeOfAnabiosis_MagicCircleFront : ModProjectile
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
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
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
			if (Owner.heldProj >= 0 && Main.projectile[Owner.heldProj].type == ModContent.ProjectileType<EyeOfAnabiosis_Weapon>())
			{
				var eOAW = Main.projectile[Owner.heldProj].ModProjectile as EyeOfAnabiosis_Weapon;
				if (eOAW.ChargeTimer >= EyeOfAnabiosis_Weapon.MaxChargeTime - 10)
				{
					Projectile.timeLeft = 60;
				}
			}
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
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect shineEffect = ModAsset.EyeOfAnabiosis_Shine.Value;
		shineEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);
		shineEffect.Parameters["uNoise"].SetValue(Commons.ModAsset.NoiseWave.Value);
		shineEffect.CurrentTechnique.Passes["MagicCircle_Pixel"].Apply();

		// Beam of magic circle
		var magicCirTexture = Commons.ModAsset.Point.Value;
		var magicCirPosition = player.gravDir == 1 ? player.Bottom : player.Top;
		magicCirPosition = magicCirPosition - Main.screenPosition + new Vector2(0, 2 * player.gravDir);
		var magicCirScale = new Vector2(0.30f, 0.30f);
		var magicCirRotation = player.gravDir == 1 ? 0 : MathF.PI;
		var magicCirColor = Color.White * 0.8f;
		Main.spriteBatch.Draw(magicCirTexture, magicCirPosition, null, magicCirColor, magicCirRotation, new Vector2(magicCirTexture.Width / 2, magicCirTexture.Height), magicCirScale, SpriteEffects.None, 0);

		// Core of magic circle
		var coreColor = new Color(0.4f, 0.8f, 2f, 0);
		var coreScale = new Vector2(0.2f, 0.32f) * 1.6f;

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Main.spriteBatch.Draw(coreTexture, magicCirPosition, null, Color.White, magicCirRotation, coreTexture.Size() / 2, coreScale / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		shineEffect.Parameters["uPow"].SetValue(24f);
		shineEffect.Parameters["uMul"].SetValue(1.3f);
		shineEffect.CurrentTechnique.Passes["MagicCircleCore_Pixel"].Apply();

		Texture2D coreTextureDark = Commons.ModAsset.Star_black.Value;
		Texture2D coreTexture = Commons.ModAsset.Textures_Star.Value;
		Texture2D bloom = Commons.ModAsset.Point.Value;

		Main.spriteBatch.Draw(coreTextureDark, magicCirPosition, null, Color.White * 0.5f, magicCirRotation, coreTextureDark.Size() / 2, coreScale * new Vector2(3f, 1f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(bloom, magicCirPosition, null, new Color(0.2f, 0.3f, 1f, 0) * 0.75f, magicCirRotation, bloom.Size() / 2, new Vector2(1.2f, 0.6f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(coreTexture, magicCirPosition, null, coreColor, magicCirRotation, coreTexture.Size() / 2, coreScale * new Vector2(3f, 1f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(bloom, magicCirPosition, null, new Color(1f, 1f, 1f, 0) * 0.76f, magicCirRotation, bloom.Size() / 2, new Vector2(0.4f, 0.2f), SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Text of magic circle
		var textTexture = Commons.ModAsset.AlienWriting.Value;
		foreach (var text in TextList)
		{
			var textColor = text.Color * (1 + text.RelativePosition.Y / MagicCircleHeight);
			Main.spriteBatch.Draw(textTexture, magicCirPosition + text.RelativePosition * player.gravDir, text.SourceRectangle, textColor, magicCirRotation, text.Origin, text.Scale, SpriteEffects.None, 0);
		}

		return false;
	}
}
