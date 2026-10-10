using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskSpice_WithGravity : ModProjectile
{
	public Vector2 StartPos = default;

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 400;
		Projectile.penetrate = -1;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		StartPos = Projectile.Center;
		Projectile.frame = Main.rand.Next(5);
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		Projectile.ai[0] = Projectile.rotation;
		Projectile.ai[1] = Projectile.rotation;
		Projectile.ai[2] = Projectile.rotation;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.5f;
		Projectile.velocity *= 0.99f;
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], Projectile.rotation, 0.5f);
		Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[1], Projectile.ai[0], 0.5f);
		Projectile.ai[2] = (float)Utils.Lerp(Projectile.ai[2], Projectile.ai[1], 0.5f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D texture = ModAsset.TuskSpice_WithGravity.Value;
		for (int i = 0; i < Projectile.oldPos.Length; i++)
		{
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(Projectile.frame * 18, 0, 18, 72), lightColor * ((3 - i) / 5f), Projectile.ai[i], new Vector2(9, 36), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(Projectile.frame * 18, 0, 18, 72), lightColor, Projectile.rotation, new Vector2(9, 36), Projectile.scale, SpriteEffects.None, 0);
		if (VFXManager.InScreen(Projectile.Center, 200))
		{
			for (int j = 0; j < 55; j++)
			{
				if (Collision.SolidCollision(Projectile.Center + new Vector2(0, j * 16), 0, 0))
				{
					Vector2 shadowPos = Projectile.Center + new Vector2(0, j * 16);
					Point shadowPoint = shadowPos.ToTileCoordinates();
					Tile tile = Main.tile[shadowPoint];
					if (tile.HasTile)
					{
						float colorValue = (55 - j + Projectile.velocity.Y * 0.4f) / 40f;
						if (colorValue < 0)
						{
							break;
						}
						colorValue = Math.Clamp(MathF.Pow(colorValue, 1.6f), 0, 1);
						Texture2D shadow = ModAsset.TuskSpice_WithGravity_shadow.Value;
						if (Math.Abs(StartPos.X - Projectile.Center.X) < 150)
						{
							colorValue *= Math.Abs(StartPos.X - Projectile.Center.X) / 150f;
						}
						Main.EntitySpriteDraw(shadow, new Vector2(shadowPos.X, shadowPoint.Y * 16 + 24) - Main.screenPosition, new Rectangle(Projectile.frame * 18, 0, 18, 72), Color.White * colorValue, Projectile.rotation, new Vector2(9, 36), Projectile.scale, SpriteEffects.None, 0);
						break;
					}
				}
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = -5; i < 5; i++)
		{
			Vector2 pos = Projectile.Center + new Vector2(i * 6, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2) - new Vector2(0, 4);
			Dust dust = Dust.NewDustDirect(pos, 0, 0, ModContent.DustType<Dusts.TuskBreak_small>());
			dust.velocity = Projectile.velocity;
		}
	}
}
