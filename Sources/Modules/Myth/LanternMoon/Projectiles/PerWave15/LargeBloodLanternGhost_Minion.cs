using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class LargeBloodLanternGhost_Minion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float Timer = 0;

	public NPC OwnerNPC;

	public int ChasePlayerTime = 240;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 800;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		ChasePlayerTime += Main.rand.Next(-60, 61);
	}

	public override void AI()
	{
		Timer++;
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<LargeBloodLanternGhost>())
		{
			if (Timer < 60)
			{
				Projectile.scale = Timer / 60f;
				Projectile.velocity *= 0;
				Projectile.rotation = Projectile.ai[0] / 5f * MathHelper.TwoPi;
			}
			else if (Timer < ChasePlayerTime)
			{
				Projectile.scale = 1f;
				Projectile.hostile = true;
				var toTarget = OwnerNPC.Center - Projectile.Center - Projectile.velocity + new Vector2(MathF.Sin(Timer * 0.03f + Projectile.whoAmI) * 120f, MathF.Sin(Timer * 0.015f + Projectile.whoAmI) * 50f);
				Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * 6 * 0.1f + Projectile.velocity * 0.9f;
			}
			else if (Timer < ChasePlayerTime + 120)
			{
				Vector2 targetPos = Vector2.zeroVector;
				int targetIndex = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
				Player player = null;
				if (targetIndex != -1)
				{
					player = Main.player[targetIndex];
				}
				if (player != null)
				{
					targetPos = player.Center;
				}
				if (targetPos != Vector2.zeroVector)
				{
					var toTarget = targetPos - Projectile.Center - Projectile.velocity;
					Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * 4 * 0.1f + Projectile.velocity * 0.9f;
				}
			}
		}
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
			Projectile.hostile = false;
		}
		if (Timer > 60)
		{
			Projectile.rotation = MathF.Sin(Timer * 0.07f + Projectile.whoAmI) * 0.04f + Projectile.rotation * 0.9f;
		}
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.3f * MathF.Sin(Timer * 0.03f + Projectile.whoAmI) + 0.3f, 0.3f * MathF.Cos(Timer * 0.03f + Projectile.whoAmI) + 0.3f) * 0.6f * fade);
		Projectile.hide = Projectile.velocity.X > 0;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D bloom = ModAsset.LargeBloodLanternGhost_Minion_Bloom.Value;
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
		}
		Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()) * fade;
		Color bloomColor = Color.Transparent;
		if (Timer < 30)
		{
			drawColor = new Color(1f, 1f, 1f, 0);
			bloomColor = new Color(1f, 1f, 1f, 0);
		}
		if (Timer >= 30 && Timer < 60)
		{
			float value = (Timer - 30) / 30f;
			drawColor = Color.Lerp(new Color(1f, 1f, 1f, 0), drawColor, value);
			bloomColor = Color.Lerp(new Color(1f, 1f, 1f, 0), bloomColor, value);
		}
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
