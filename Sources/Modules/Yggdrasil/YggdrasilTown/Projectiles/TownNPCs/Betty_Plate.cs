using Everglow.Yggdrasil.YggdrasilTown.Dusts.TownNPCAttack;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Betty_Plate : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public Queue<Vector2> OldPos = new Queue<Vector2>();
	public Queue<int> OldFrames = new Queue<int>();
	public Queue<float> OldRotation = new Queue<float>();

	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 18;
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
		OldFrames.Enqueue(Projectile.frame);
		OldRotation.Enqueue(Projectile.rotation);
		int maxTrail = 6;
		if (OldPos.Count > maxTrail)
		{
			OldPos.Dequeue();
		}
		if (OldFrames.Count > maxTrail)
		{
			OldFrames.Dequeue();
		}
		if (OldRotation.Count > maxTrail)
		{
			OldRotation.Dequeue();
		}
		if (Projectile.velocity.Y <= 14)
		{
			Projectile.velocity.Y += 0.4f;
		}
		Projectile.velocity *= 0.99f;
		if (Projectile.timeLeft % 3 == 0)
		{
			Projectile.frame++;
			if (Projectile.frame > 11)
			{
				Projectile.frame = 0;
			}
		}
		Projectile.rotation += Projectile.ai[0];
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.35f, 0.35f);
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 6; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Betty_Plate_Shatter_Dust>());
			d.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 3.6f).RotatedByRandom(6.283);
			d.scale = Main.rand.NextFloat(0.8f, 2f);
			if (d.frame.Y == 0)
			{
				d.scale *= 0.5f;
			}
			d.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.3f;
			d.position += d.velocity;
		}
		for (int j = 0; j < 4; j++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/Betty_Plate_Shatter" + Main.rand.Next(3)).Type;
			var gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, v0, type, Projectile.scale);
			gore.velocity *= 0.3f;
			gore.scale = Main.rand.NextFloat(0.8f, 1.2f);
			gore.velocity -= Projectile.velocity.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat() * 0.1f;
		}
		SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		base.OnKill(timeLeft);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.Kill();
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = ModAsset.Betty_Plate.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		var frame = new Rectangle(0, 24 * Projectile.frame, 24, 24);
		Main.EntitySpriteDraw(texture, drawCenter, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		var oldP = OldPos.ToArray();
		var oldF = OldFrames.ToArray();
		var oldR = OldRotation.ToArray();
		var count = Math.Min(oldP.Count(), oldF.Count());
		count = Math.Min(count, oldR.Count());
		float fade = 1f;
		if (Projectile.velocity.Length() < 8f)
		{
			fade *= MathF.Max((Projectile.velocity.Length() - 4f) / 4f, 0);
		}
		for (int i = 0; i < count; i++)
		{
			drawCenter = oldP[i] - Main.screenPosition;
			var frameOld = new Rectangle(0, 24 * oldF[i], 24, 24);
			Main.EntitySpriteDraw(texture, drawCenter, frameOld, lightColor * (i / (float)count) * 0.3f * fade, oldR[i], frameOld.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
