using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class CeremonialBladeProj : ModProjectile, IWarpProjectile_warpStyle2
{
	// We define some constants that determine the swing range of the sword
	// Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
	// You could change the values or even replace them entirely, but they are tweaked with looks in mind
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	// We define some constants that determine the swing range of the sword
	// Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
	// You could change the values or even replace them entirely, but they are tweaked with looks in mind
	private const float SWINGRANGE = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
	private const float FIRSTHALFSWING = 0.45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
	private const float SPINRANGE = 3.5f * (float)Math.PI; // The angle a spin attack covers (630 degrees)
	private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
	private const float UNWIND = 0.4f; // When should the sword start disappearing
	private const float SPINTIME = 2.5f; // How much longer a spin is than a swing

	private enum AttackType // Which attack is being performed
	{
		/// <summary>
		/// Swings are normal sword swings that can be slightly aimed
		/// Swings goes through the full cycle of animations
		/// </summary>
		Swing,

		/// <summary>
		/// Spins are swings that go full circle
		/// They are slower and deal more knockback
		/// </summary>
		Spin,
	}

	private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
	{
		Prepare,
		Execute,
		Unwind,
	}

	// These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
	private AttackType CurrentAttack
	{
		get => (AttackType)Projectile.ai[0];
		set => Projectile.ai[0] = (float)value;
	}

	private AttackStage CurrentStage
	{
		get => (AttackStage)Projectile.localAI[0];
		set
		{
			Projectile.localAI[0] = (float)value;
			Timer = 0; // reset the timer when the projectile switches states
		}
	}

	// Variables to keep track of during runtime
	private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)

	private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage

	private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle

	private ref float Size => ref Projectile.localAI[2]; // Size of sword

	// We define timing functions for each stage, taking into account melee attack speed
	// Note that you can change this to suit the need of your projectile
	private float PrepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

	private float ExecTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

	private float HideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

	public override string Texture => ModAsset.CeremonialBlade_Mod; // Use texture of item as projectile texture

	private Player Owner => Main.player[Projectile.owner];

	public Queue<Vector2> OldRotScales = new Queue<Vector2>();

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 46; // Hitbox width of projectile
		Projectile.height = 48; // Hitbox height of projectile
		Projectile.friendly = true; // Projectile hits enemies
		Projectile.timeLeft = 10000; // Time it takes for projectile to expire
		Projectile.penetrate = -1; // Projectile pierces infinitely
		Projectile.tileCollide = false; // Projectile does not collide with tiles
		Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
		Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
		Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
		Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
		float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

		if (CurrentAttack == AttackType.Spin)
		{
			InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection); // For the spin, starting angle is designated based on direction of hit
		}
		else
		{
			if (Projectile.spriteDirection == 1)
			{
				// However, we limit the rangle of possible directions so it does not look too ridiculous
				targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
			}
			else
			{
				if (targetAngle < 0)
				{
					targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
				}

				targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
			}

			InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
		}
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		// Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually.
		writer.Write((sbyte)Projectile.spriteDirection);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		Projectile.spriteDirection = reader.ReadSByte();
	}

	public override void AI()
	{
		// Extend use animation until projectile is killed
		Owner.itemAnimation = 2;
		Owner.itemTime = 2;

		// Kill the projectile if the player dies or gets crowd controlled
		if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
		{
			Projectile.Kill();
			return;
		}

		// AI depends on stage and attack
		// Note that these stages are to facilitate the scaling effect at the beginning and end
		// If this is not desirable for you, feel free to simplify
		switch (CurrentStage)
		{
			case AttackStage.Prepare:
				PrepareStrike();
				break;
			case AttackStage.Execute:
				ExecuteStrike();
				break;
			default:
				UnwindStrike();
				break;
		}

		SetSwordPosition();
		Timer++;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		// Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
		Vector2 origin;
		float rotationOffset;
		SpriteEffects effects;

		if (Projectile.spriteDirection > 0)
		{
			origin = new Vector2(0, Projectile.height);
			rotationOffset = MathHelper.ToRadians(45f);
			effects = SpriteEffects.None;
		}
		else
		{
			origin = new Vector2(Projectile.width, Projectile.height);
			rotationOffset = MathHelper.ToRadians(135f);
			effects = SpriteEffects.FlipHorizontally;
		}

		var drawPos = Projectile.Center - Main.screenPosition;
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Main.spriteBatch.Draw(texture, drawPos, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

		// Draw a special visual effect.
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var bladeEffect_Body_Dark = new List<Vertex2D>();
		var bladeEffect_Body = new List<Vertex2D>();
		var bladeEffect_Edge = new List<Vertex2D>();
		var oldPara = OldRotScales.ToArray();
		var normal = new Vector2(24 * Projectile.spriteDirection, -29) * 2;
		float phase = 0.5f;
		for (int i = 0; i < oldPara.Length; i++)
		{
			var edge = normal.RotatedBy(oldPara[i].X + rotationOffset) * oldPara[i].Y;
			var drawColor = lightColor;
			var darkColor = Color.White;
			drawColor.A = 0;
			float coordY = i / 30f - Projectile.timeLeft / 90f + phase;
			if (i == oldPara.Length - 1)
			{
				drawColor *= 0;
				darkColor *= 0;
			}
			if (i < oldPara.Length - 4)
			{
				float fade = (i - (oldPara.Length - 8)) / 4f;
				fade = Math.Max(fade, 0);
				drawColor *= fade;
				darkColor *= fade;
			}
			if (i < oldPara.Length - 8)
			{
				drawColor *= 0;
				darkColor *= 0;
			}
			float colorValue = MathF.Cos(Projectile.localAI[1] + MathF.PI);
			drawColor *= 1 - Projectile.Opacity;
			darkColor *= 1 - Projectile.Opacity;
			drawColor *= colorValue;
			darkColor *= colorValue;
			bladeEffect_Edge.Add(drawPos + edge, drawColor, new Vector3(0.5f, coordY, 0));
			bladeEffect_Edge.Add(drawPos + edge * 0.3f, drawColor * 0.1f, new Vector3(Math.Clamp(0.5f - Projectile.localAI[1] * 0.1f, 0, 0.5f), coordY, 0));

			bladeEffect_Body_Dark.Add(drawPos + edge, darkColor, new Vector3(2f, coordY, 0));
			bladeEffect_Body_Dark.Add(drawPos + edge * 0.3f, darkColor * 0f, new Vector3(0, coordY, 0));

			bladeEffect_Body.Add(drawPos + edge, drawColor * 0.6f * colorValue, new Vector3(2f, coordY, 0));
			bladeEffect_Body.Add(drawPos + edge * 0.3f, drawColor * 0.0f, new Vector3(0, coordY, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_2_black.Value;
		if (bladeEffect_Body_Dark.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bladeEffect_Body_Dark.ToArray(), 0, bladeEffect_Body_Dark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_2.Value;
		if (bladeEffect_Body.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bladeEffect_Body.ToArray(), 0, bladeEffect_Body.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
		if (bladeEffect_Edge.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bladeEffect_Edge.ToArray(), 0, bladeEffect_Edge.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Since we are doing a custom draw, prevent it from normally drawing
		return false;
	}

	public void DrawWarp(VFXBatch batch)
	{
		var bladeEffect_Warp = new List<Vertex2D>();
		var drawPos = Projectile.Center - Main.screenPosition;
		var oldPara = OldRotScales.ToArray();
		var normal = new Vector2(24 * Projectile.spriteDirection, -29) * 2;
		float phase = 0.5f;
		float rotationOffset;
		if (Projectile.spriteDirection > 0)
		{
			rotationOffset = MathHelper.ToRadians(45f);
		}
		else
		{
			rotationOffset = MathHelper.ToRadians(135f);
		}
		for (int i = 0; i < oldPara.Length; i++)
		{
			var edge = normal.RotatedBy(oldPara[i].X + rotationOffset) * oldPara[i].Y;
			var dir = edge.NormalizeSafe() * 0.5f + new Vector2(0.5f);
			float coordY = i / 30f - Projectile.timeLeft / 90f + phase;
			float strength = 1f;
			if (i == oldPara.Length - 1)
			{
				strength *= 0;
			}
			if (i < oldPara.Length - 4)
			{
				float fade = (i - (oldPara.Length - 8)) / 4f;
				fade = Math.Max(fade, 0);
				strength *= fade;
			}
			if (i < oldPara.Length - 8)
			{
				strength *= 0;
			}
			float colorValue = MathF.Cos(Projectile.localAI[1] + MathF.PI);
			strength *= (1 - Projectile.Opacity) * colorValue * 0.2f;
			var colorWarp = new Color(dir.X, dir.Y, strength, 1);
			var colorWarpFade = new Color(dir.X, dir.Y, strength * 0.1f, 1);
			bladeEffect_Warp.Add(drawPos + edge, colorWarp, new Vector3(0.5f, coordY, 1));
			bladeEffect_Warp.Add(drawPos + edge * 0.3f, colorWarpFade, new Vector3(0, coordY, 1));
		}
		if (bladeEffect_Warp.Count > 3)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			batch.Draw(Commons.ModAsset.Noise_flame_2.Value, bladeEffect_Warp, PrimitiveType.TriangleStrip);
		}
	}

	// Find the start and end of the sword and use a line collider to check for collision with enemies
	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 start = Owner.MountedCenter;
		Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
		float collisionPoint = 0f;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
	}

	// Do a similar collision check for tiles
	public override void CutTiles()
	{
		Vector2 start = Owner.MountedCenter;
		Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
		Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
	}

	// We make it so that the projectile can only do damage in its release and unwind phases
	public override bool? CanDamage()
	{
		if (CurrentStage == AttackStage.Prepare)
		{
			return false;
		}

		return base.CanDamage();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// Make knockback go away from player
		modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

		// If the NPC is hit by the spin attack, increase knockback slightly
		if (CurrentAttack == AttackType.Spin)
		{
			modifiers.Knockback += 1;
		}
	}

	// Function to easily set projectile and arm position
	public void SetSwordPosition()
	{
		float newRot = InitialAngle + Projectile.spriteDirection * Progress;
		float omega = MathF.Abs(Projectile.rotation - newRot);
		Projectile.Opacity = 1 - 2.4f * omega;
		Projectile.rotation = newRot; // Set projectile rotation

		// Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
		Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

		armPosition.Y += Owner.gfxOffY;
		Projectile.Center = armPosition; // Set projectile to arm position
		Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

		// Record old rotations and scales.
		OldRotScales.Enqueue(new Vector2(Projectile.rotation, Projectile.scale));
		if (OldRotScales.Count > 90)
		{
			OldRotScales.Dequeue();
		}

		Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
	}

	// Function facilitating the taking out of the sword
	private void PrepareStrike()
	{
		Progress = WINDUP * SWINGRANGE * (1f - Timer / PrepTime); // Calculates rotation from initial angle
		Size = MathHelper.SmoothStep(0, 1, Timer / PrepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

		if (Timer >= PrepTime)
		{
			CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
		}
	}

	// Function facilitating the first half of the swing
	private void ExecuteStrike()
	{
		if (CurrentAttack == AttackType.Swing)
		{
			Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / ExecTime);

			if (Timer >= ExecTime)
			{
				CurrentStage = AttackStage.Unwind;
			}
		}
		else
		{
			Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / (ExecTime * SPINTIME));

			if (Timer == (int)(ExecTime * SPINTIME * 3.5f / 4))
			{
				SoundEngine.PlaySound(SoundID.Item1); // Play sword sound again
				Projectile.ResetLocalNPCHitImmunity(); // Reset the local npc hit immunity for second half of spin
			}

			if (Timer >= ExecTime * SPINTIME)
			{
				CurrentStage = AttackStage.Unwind;
			}
		}
	}

	// Function facilitating the latter half of the swing where the sword disappears
	private void UnwindStrike()
	{
		if (CurrentAttack == AttackType.Swing)
		{
			Progress = MathHelper.SmoothStep(0, SWINGRANGE, 1f - UNWIND + UNWIND * Timer / HideTime);
			Size = 1f - MathHelper.SmoothStep(0, 1, Timer / HideTime); // Make sword slowly decrease in size as we end the swing to make a smooth hiding animation

			if (Timer >= HideTime)
			{
				Projectile.Kill();
			}
		}
		else
		{
			Progress = MathHelper.SmoothStep(0, SPINRANGE, 1f - UNWIND / 2 + UNWIND / 2 * Timer / (HideTime * SPINTIME / 2));
			Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (HideTime * SPINTIME / 2));

			if (Timer >= HideTime * SPINTIME / 2)
			{
				Projectile.Kill();
			}
		}
	}
}
