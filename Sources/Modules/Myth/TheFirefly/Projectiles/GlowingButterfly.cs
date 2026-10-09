using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Items.Accessories;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowingButterfly : ModProjectile
{
	FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 6;
	}

	public override void SetDefaults()
	{
		Projectile.width = 46;
		Projectile.height = 46;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = ItemUseStyleID.Swing;
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
				Projectile.timeLeft = 400;
			else
			{
				Projectile.timeLeft = 100;
			}
		}
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = false;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.alpha = 255;
	}

	private float omega = 0;

	private int useStyle = 0;
	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{

			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				if (useStyle == 0)
				{
					Projectile.timeLeft = Main.rand.Next(135, 185);
					Projectile.frame = Main.rand.Next(6);
					omega = Main.rand.NextFloat(-0.02f, 0.02f);
					Projectile.scale = Main.rand.NextFloat(0.4f, 0.7f);
					useStyle = ItemUseStyleID.Swing;
				}
				if (Projectile.timeLeft > 100 && Projectile.alpha >= 8)
				{
					Projectile.alpha -= 4;
				}

				if (Projectile.timeLeft <= 66)
				{
					Projectile.alpha += 4;
				}

				if (Projectile.alpha < 100)
					Projectile.friendly = true;
				else
				{
					Projectile.friendly = false;
				}
			}
			else
			{
				if (useStyle == 0)
				{
					Projectile.timeLeft = Main.rand.Next(85, 135);
					Projectile.frame = Main.rand.Next(6);
					omega = Main.rand.NextFloat(-0.02f, 0.02f);
					Projectile.scale = Main.rand.NextFloat(0.6f, 1.0f);
					useStyle = ItemUseStyleID.Swing;
				}
				if (Projectile.timeLeft > 50 && Projectile.alpha >= 8)
				{
					Projectile.alpha -= 8;
				}

				if (Projectile.timeLeft <= 33)
				{
					Projectile.alpha += 8;
				}

				if (Projectile.alpha < 50)
					Projectile.friendly = true;
				else
				{
					Projectile.friendly = false;
				}
			}
		}

		// Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
		Projectile.rotation = (float)(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + Math.PI * 0.75);
		Projectile.velocity = Projectile.velocity.RotatedBy(omega);
		omega += Math.Sign(omega) * 0.001f;
		if (Projectile.frame != 5)
			Projectile.velocity *= 1.04f;
		else
		{
			Projectile.velocity *= 0.98f;
		}
		if (Collision.SolidCollision(Projectile.Center - Projectile.velocity, 1, 1))
		{
			Projectile.tileCollide = true;
		}

		if (Projectile.timeLeft % 5 == 0)
		{
			if (Projectile.frame != 5)
				Projectile.frame++;
			else
			{
				if (Main.rand.NextFloat(0, 7) >= Projectile.velocity.Length())
				{
					Projectile.frame = 0;
				}
			}
		}
		if (Projectile.frame > 5)
		{
			Projectile.frame = 0;
		}

		Projectile.velocity.Y *= 0.96f;
		if (Projectile.timeLeft % 12 == 0)
		{
			int type = ModContent.DustType<BlueGlowAppear>();
			// if (Projectile.ai[0] == 0)
			// {
			// type = ModContent.DustType<BlueGlowAppear_dark>();
			// }
			Dust dust = Dust.NewDustDirect(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, type, 0f, 0f, 100, default, Main.rand.NextFloat(0.9f, 2.2f));
			dust.velocity = Projectile.velocity * 0.5f;
		}
		GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
		SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
		Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
		Visuals();
	}

	private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
	{
		Vector2 idlePosition = owner.Center;
		idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

		// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
		// The index is projectile.minionPos
		float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
		idlePosition.X += minionPositionOffsetX; // Go behind the player

		// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

		// Teleport to player if distance is too big
		vectorToIdlePosition = idlePosition - Projectile.Center;
		distanceToIdlePosition = vectorToIdlePosition.Length();

		if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
		{
			// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
			// and then set netUpdate to true
			Projectile.position = idlePosition;
			Projectile.velocity *= 0.1f;
			Projectile.netUpdate = true;
		}

		// If your minion is flying, you want to do this independently of any conditions
		float overlapVelocity = 0.04f;

		// Fix overlap with other minions
		for (int i = 0; i < Main.maxProjectiles; i++)
		{
			Projectile other = Main.projectile[i];

			if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
			{
				if (Projectile.position.X < other.position.X)
					Projectile.velocity.X -= overlapVelocity;
				else
				{
					Projectile.velocity.X += overlapVelocity;
				}

				if (Projectile.position.Y < other.position.Y)
					Projectile.velocity.Y -= overlapVelocity;
				else
				{
					Projectile.velocity.Y += overlapVelocity;
				}
			}
		}
	}

	private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
	{
		// Starting search distance
		distanceFromTarget = 700f;
		targetCenter = Projectile.position;
		foundTarget = false;

		// This code is required if your minion weapon has the targeting feature
		if (owner.HasMinionAttackTargetNPC)
		{
			NPC npc = Main.npc[owner.MinionAttackTargetNPC];
			float between = Vector2.Distance(npc.Center, Projectile.Center);

			// Reasonable distance away so it doesn't target across multiple screens
			if (between < 2000f)
			{
				distanceFromTarget = between;
				targetCenter = npc.Center;
				foundTarget = true;
			}
		}

		if (!foundTarget)
		{
			// This code is required either way, used for finding a target
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.CanBeChasedBy())
				{
					float between = Vector2.Distance(npc.Center, Projectile.Center);
					bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
					bool inRange = between < distanceFromTarget;
					bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
					// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
					// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
					bool closeThroughWall = between < 100f;

					if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
					{
						distanceFromTarget = between;
						targetCenter = npc.Center;
						foundTarget = true;
					}
				}
			}
		}

		// friendly needs to be set to true so the minion can deal contact damage
		// friendly needs to be set to false so it doesn't damage things like target dummies while idling
		// Both things depend on if it has a target or not, so it's just one assignment here
		// You don't need this assignment if your minion is shooting things instead of dealing contact damage
		// Projectile.friendly = foundTarget;
	}

	private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
	{
		// Default movement parameters (here for attacking)
		float speed = 8f;
		float inertia = 20f;

		if (foundTarget)
		{
			// Minion has a target: attack (here, fly towards the enemy)
			if (distanceFromTarget > 40f)
			{
				// The immediate range around the target (so it doesn't latch onto it when close)
				Vector2 direction = targetCenter - Projectile.Center;
				direction.Normalize();
				direction *= speed;

				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
			}
		}
		else
		{
			// Minion doesn't have a target: return to player and idle
			if (distanceToIdlePosition > 600f)
			{
				// Speed up the minion if it's away from the player
				speed = 9f;
				inertia = 60f;
			}
			else
			{
				// Slow down the minion if closer to the player
				speed = 4f;
				inertia = 80f;
			}

			if (distanceToIdlePosition > 20f)
			{
				// The immediate range around the player (when it passively floats about)

				// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
			}
			else if (Projectile.velocity == Vector2.Zero)
			{
				// If there is a case where it's not moving at all, give it a little "poke"
				Projectile.velocity.X = -0.15f;
				Projectile.velocity.Y = -0.05f;
			}
		}
	}

	private void Visuals()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(0, 0.05f * (255 - Projectile.alpha) / 255f, 0.12f * (255 - Projectile.alpha) / 255f));
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnKill(int timeLeft)
	{
		if (Projectile.alpha > 180)
		{
			return;
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModAsset.GlowingButterfly.Value;
		// Texture2D texDark = ModAsset.GlowingButterfly_dark.Value;
		// Texture2D texBound = ModAsset.GlowingButterfly_bound.Value;
		Color lightC = new Color(55 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, (255 - Projectile.alpha) / 2);
		// if(Projectile.ai[0] == 1)
		// {
		// lightC = Color.Transparent;
		// }
		// if (Projectile.ai[0] == 2)
		// {
		// lightC = new Color(55 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, (255 - Projectile.alpha) / 2);
		// }
		// float colorValue = (255 - Projectile.alpha) / 255f;
		Rectangle frame = new Rectangle(0, Projectile.frame * 46, 46, 46);
		// Main.spriteBatch.Draw(texDark, Projectile.Center - Main.screenPosition, frame, Color.White * colorValue, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		// Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightC, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		// Main.spriteBatch.Draw(texBound, Projectile.Center - Main.screenPosition, frame, Color.White * ((255 - Projectile.alpha) / 400f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, lightC, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
