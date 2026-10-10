using Everglow.Commons.Coroutines;
using Everglow.Minortopography.GiantPinetree.Items;
using Terraria.DataStructures;

namespace Everglow.Minortopography.GiantPinetree.Projectiles;

public class PineSprite : ModProjectile
{
	private CoroutineManager _coroutineManager = new CoroutineManager();

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 3600000;
		Projectile.penetrate = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 2;
	}

	public override void OnSpawn(IEntitySource source)
	{
		_coroutineManager.StartCoroutine(new Coroutine(ChasePlayer()));
	}

	private int targetWhoAmI = -1;

	public override void AI()
	{
		_coroutineManager.Update();
		Player player = Main.player[Projectile.owner];
		float waringDistance = 800;
		targetWhoAmI = -1;
		foreach (NPC target in Main.npc)
		{
			if (target.active && !target.dontTakeDamage && !target.friendly && target.CanBeChasedBy() && target.life > 0)
			{
				if ((target.Center - player.Center).Length() < waringDistance)
				{
					waringDistance = (target.Center - player.Center).Length();
					targetWhoAmI = target.whoAmI;
				}
			}
		}
		if (targetWhoAmI != -1)
		{
			_coroutineManager.StartCoroutine(new Coroutine(ChaseEnemy(Main.npc[targetWhoAmI])));
		}
		else
		{
			if ((player.Center - Projectile.Center).Length() > 100)
			{
				_coroutineManager.StartCoroutine(new Coroutine(ChasePlayer()));
			}
			else
			{
				Projectile.velocity *= 0.94f;
			}
		}

		bool hasAcc = false;
		foreach (Item item in player.armor)
		{
			if (item.type == ModContent.ItemType<SnowPineLockBox>())
			{
				hasAcc = true;
				break;
			}
		}
		if (!hasAcc)
		{
			Projectile.Kill();
		}
		Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(4)/*Half size of a dust texture*/ + new Vector2(0, -8), 0, 0, DustID.IceTorch);
		d.noGravity = true;
		d.scale = Main.rand.NextFloat(0.85f, 1.15f);
	}

	private IEnumerator<ICoroutineInstruction> ChasePlayer()
	{
		Player player = Main.player[Projectile.owner];
		while ((player.Center - Projectile.Center).Length() > 100)
		{
			if (targetWhoAmI != -1)
			{
				yield break;
			}
			Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
			float velocityLength = 7f + (player.Center - Projectile.Center).Length() / 40f;
			if ((player.Center - Projectile.Center).Length() >= 1200)
			{
				velocityLength = 37f;
			}
			Vector2 finalVelocity = Utils.SafeNormalize(toPlayer, Vector2.zeroVector) * velocityLength;
			Projectile.velocity = Projectile.velocity * 0.95f + finalVelocity * 0.05f;
			yield return new SkipThisFrame();
		}
	}

	private IEnumerator<ICoroutineInstruction> ChaseEnemy(NPC npc)
	{
		while (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.CanBeChasedBy() && npc.life > 0)
		{
			Player player = Main.player[Projectile.owner];
			foreach (NPC target in Main.npc)
			{
				if (target != npc)
				{
					if (target.active && !target.dontTakeDamage && !target.friendly && target.CanBeChasedBy() && target.life > 0)
					{
						if ((target.Center - player.Center).Length() < (npc.Center - player.Center).Length())
						{
							targetWhoAmI = target.whoAmI;
							npc = target;
							break;
						}
					}
				}
			}
			player.MinionAttackTargetNPC = npc.whoAmI;
			Vector2 toTarget = npc.Center - Projectile.Center - Projectile.velocity;
			float velocityLength = 15f + (npc.Center - Projectile.Center).Length() / 60f;
			if ((npc.Center - Projectile.Center).Length() >= 1200)
			{
				velocityLength = 35f;
			}
			Vector2 finalVelocity = Utils.SafeNormalize(toTarget, Vector2.zeroVector) * velocityLength;
			Projectile.velocity = Projectile.velocity * 0.95f + finalVelocity * 0.05f;
			yield return new SkipThisFrame();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D mainTex = ModAsset.PineSprite.Value;
		Texture2D glowTex = ModAsset.PineSprite_glow.Value;
		Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, mainTex.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f), Projectile.rotation, glowTex.Size() / 2f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
