using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class NehemothBullet : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public const int BuffDuration = 240;
	public const float DebuffTriggerChance = 0.66f;

	public bool HasNotShot { get; private set; } = true;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = ProjAIStyleID.Arrow;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 600;
		Projectile.alpha = 255;
		Projectile.light = 0.5f;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 1;
		Projectile.scale = 0.6f;

		AIType = ProjectileID.Bullet;
		Projectile.velocity = Projectile.velocity.NormalizeSafe() * 24f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextFloat() < DebuffTriggerChance)
		{
			target.AddBuff(ModContent.BuffType<Plague>(), BuffDuration);
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Main.rand.NextFloat() < DebuffTriggerChance)
		{
			target.AddBuff(ModContent.BuffType<Plague>(), BuffDuration);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;

		var drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
		for (int k = 0; k < Projectile.oldPos.Length; k++)
		{
			Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
			Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, SpriteEffects.None);
		}

		return true;
	}

	public override void OnKill(int timeLeft)
	{
		Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
	}
}
