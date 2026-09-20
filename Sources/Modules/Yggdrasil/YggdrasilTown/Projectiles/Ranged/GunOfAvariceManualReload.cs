using Everglow.Yggdrasil.Common.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class GunOfAvariceManualReload : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	private bool HasNotPlayedSound { get; set; } = true;

	private Player Owner => Main.player[Projectile.owner];

	public override void SetDefaults()
	{
		Projectile.width = 62;
		Projectile.height = 32;
		Projectile.timeLeft = GunOfAvarice.ManualReloadDuration;
		Projectile.penetrate = -1;
		Projectile.hide = true;
		Projectile.scale = 0.75f;
	}

	public override void AI()
	{
		if (Projectile.timeLeft <= 15 && HasNotPlayedSound)
		{
			HasNotPlayedSound = false;
			SoundEngine.PlaySound(new SoundStyle(ModAsset.GunReload2_Mod));
			VFX(Projectile.ai[1]);
			for (int i = 0; i < 14; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<BulletShell_yggdrasil>());
				dust.velocity = new Vector2(0, -Main.rand.NextFloat(5, 8)).RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
			}
		}
		float timeValue = 1 - Projectile.timeLeft / 30f;
		float deltaRot = 1.8f;
		if (timeValue < 0.2f)
		{
			deltaRot = 1.8f - MathF.Pow(timeValue / 0.2f, 0.5f) * 1.4f;
		}
		if (timeValue >= 0.2f)
		{
			deltaRot = 0.6f + MathF.Pow((timeValue - 0.2f) / 0.8f, 0.5f) * 1.2f;
		}
		Projectile.rotation = -MathHelper.PiOver2 + Owner.direction * deltaRot;
		var offset = new Vector2(-15f * Owner.direction, 0);
		if (timeValue is > 0.1f and < 0.3f)
		{
			offset = new Vector2((0.5f - Math.Abs(timeValue - 0.2f) / 0.1f) * -15f, 0).RotatedBy(Projectile.rotation) + new Vector2(-15f * Owner.direction, 0);
		}
		Projectile.spriteDirection = Owner.direction;
		Projectile.Center = Owner.Center + offset + new Vector2(24 * Owner.direction, -8);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D gun = ModAsset.GunOfAvarice.Value;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.spriteBatch.Draw(gun, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, gun.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
		return false;
	}

	public void VFX(float level)
	{
		for (int i = 0; i < level * 3 + 10; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(3.6f, 6.4f)).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 22.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var dust = new AvariceSuccessDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(1f, 2f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < level * 2 + 8; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(1.6f, 6.4f)).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
			Vector2 pos = new Vector2(0, Main.rand.NextFloat(0f, 50.4f)).RotatedByRandom(MathHelper.TwoPi);
			pos.Y *= 0.1f;
			var cube = new AvariceSuccessCube
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Owner.Center + pos,
				maxTime = Main.rand.Next(60, 120),
				scale = Main.rand.NextFloat(10f, 20f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 20.93f), 0.06f },
			};
			Ins.VFXManager.Add(cube);
		}
	}
}
