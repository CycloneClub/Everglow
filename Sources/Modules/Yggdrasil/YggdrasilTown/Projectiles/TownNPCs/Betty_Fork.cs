using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Betty_Fork : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public Queue<Vector2> OldPos = new Queue<Vector2>();
	public Queue<float> OldRotation = new Queue<float>();

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void AI()
	{
		OldPos.Enqueue(Projectile.Center);
		OldRotation.Enqueue(Projectile.rotation);
		int maxTrail = 6;
		if (OldPos.Count > maxTrail)
		{
			OldPos.Dequeue();
		}
		if (OldRotation.Count > maxTrail)
		{
			OldRotation.Dequeue();
		}
		if (Projectile.velocity.Y <= 14)
		{
			Projectile.velocity.Y += 0.3f;
		}
		Projectile.velocity *= 0.99f;
		Projectile.rotation += Projectile.ai[0];
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-1.5f, 1.5f);
	}

	public override void OnKill(int timeLeft)
	{
		Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
		int type = ModContent.Find<ModGore>("Everglow/Betty_Fork_invaild").Type;
		var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, v0, type, Projectile.scale);
		gore.velocity *= 0.3f;
		gore.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.1f;
		SoundEngine.PlaySound(SoundID.NPCHit4, Projectile.Center);
		base.OnKill(timeLeft);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.Kill();
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = ModAsset.Betty_Fork.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(texture, drawCenter, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		var oldP = OldPos.ToArray();
		var oldR = OldRotation.ToArray();
		var count = Math.Min(oldP.Count(), oldR.Count());
		float fade = 1f;
		if (Projectile.velocity.Length() < 8f)
		{
			fade *= MathF.Max((Projectile.velocity.Length() - 4f) / 4f, 0);
		}
		for (int i = 0; i < count; i++)
		{
			drawCenter = oldP[i] - Main.screenPosition;
			Main.EntitySpriteDraw(texture, drawCenter, null, lightColor * (i / (float)count) * 0.3f * fade, oldR[i], texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
