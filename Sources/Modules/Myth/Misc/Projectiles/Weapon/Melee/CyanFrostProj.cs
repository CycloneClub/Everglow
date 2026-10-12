using Everglow.Myth.Misc.Dusts;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

// This is a copy of the Excalibur's projectile
public class CyanFrostProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	// We could use a vanilla texture if we want instead of supplying our own.
	// public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Excalibur;

	public override void SetStaticDefaults()
	{
		// If a Jellyfish is zapping and we attack it with this projectile, it will deal damage to us.
		// This set has the projectiles for the Night's Edge, Excalibur, Terra Blade (close range), and The Horseman's Blade (close range).
		// This set does not have the True Night's Edge, True Excalibur, or the long range Terra Beam projectiles.
		ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
		Main.projFrames[Type] = 4; // This projectile has 4 frames.
	}

	public override void SetDefaults()
	{
		// The width and height don't really matter here because we have custom collision.
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 4; // The projectile can hit 3 enemies.
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.ownerHitCheck = true; // A line of sight check so the projectile can't deal damage through tiles.
		Projectile.ownerHitCheckDistance = 400f; // The maximum range that the projectile can hit a target. 300 pixels is 18.75 tiles.
		Projectile.usesOwnerMeleeHitCD = true; // This will make the projectile apply the standard number of immunity frames as normal melee attacks.
											   // Normally, projectiles die after they have hit all the enemies they can.
											   // But, for this case, we want the projectile to continue to live so we can have the visuals of the swing.
		Projectile.stopsDealingDamageAfterPenetrateHits = true;

		// We will be using custom AI for this projectile. The original Excalibur uses aiStyle 190.
		Projectile.aiStyle = -1;
		// Projectile.aiStyle = ProjAIStyleID.NightsEdge; // 190
		// AIType = ProjectileID.Excalibur;

		// If you are using custom AI, add this line. Otherwise, visuals from Flasks will spawn at the center of the projectile instead of around the arc.
		// We will spawn the visuals around the arc ourselves in the AI().
		Projectile.noEnchantmentVisuals = true;
	}

	public override void AI()
	{
		// In our item, we spawn the projectile with the direction, max time, and scale
		// Projectile.ai[0] == direction
		// Projectile.ai[1] == max time
		// Projectile.ai[2] == scale
		// Projectile.localAI[0] == current time

		// Terra Blade makes an extra sound when spawning.
		// if (Projectile.localAI[0] == 0f) {
		// 	SoundEngine.PlaySound(SoundID.Item60 with { Volume = 0.65f }, Projectile.position);
		// }

		Projectile.localAI[0]++; // Current time that the projectile has been alive.
		Player player = Main.player[Projectile.owner];
		float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
		float direction = Projectile.ai[0];
		float velocityRotation = Projectile.velocity.ToRotation();
		float adjustedRotation = MathHelper.Pi * direction * percentageOfLife + velocityRotation + direction * MathHelper.Pi + player.fullRotation;
		Projectile.rotation = adjustedRotation; // Set the rotation to our to the new rotation we calculated.

		float scaleMulti = 1.2f; // Excalibur, Terra Blade, and The Horseman's Blade is 0.6f; True Excalibur is 1f; default is 0.2f
		float scaleAdder = 1.2f; // Excalibur, Terra Blade, and The Horseman's Blade is 1f; True Excalibur is 1.2f; default is 1f

		Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
		Projectile.scale = scaleAdder + percentageOfLife * scaleMulti;

		// The other sword projectiles that use AI Style 190 have different effects.
		// This example only includes the Excalibur.
		// Look at AI_190_NightsEdge() in Projectile.cs for the others.

		// Here we spawn some dust inside the arc of the swing.
		float dustRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
		Vector2 dustPosition = Projectile.Center + dustRotation.ToRotationVector2() * 84f * Projectile.scale;
		Vector2 dustVelocity = (dustRotation + Projectile.ai[0] * MathHelper.PiOver2).ToRotationVector2();
		if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
		{
			// Original Excalibur color: Color.Gold, Color.White
			Color dustColor = Color.Lerp(Color.SkyBlue, Color.White, Main.rand.NextFloat() * 0.3f);
			Dust coloredDust = Dust.NewDustPerfect(Projectile.Center + dustRotation.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale), DustID.FireworksRGB, dustVelocity * 1f, 100, dustColor, 0.4f);
			coloredDust.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
			coloredDust.noGravity = true;
		}

		if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
		{
			// This dust spawns on the swinging arc, or just on the perimeter of the projectile arc.
			// Original Excalibur color: Color.White
			Dust coloredDust = Dust.NewDustPerfect(dustPosition, DustID.Frost, dustVelocity, 100, Color.SkyBlue * Projectile.Opacity, 1.2f * Projectile.Opacity);
			coloredDust.noGravity = true;
		}

		Dust diamond = Dust.NewDustPerfect(Projectile.Center + new Vector2(70, 0).RotatedBy(Projectile.rotation), ModContent.DustType<IceScale>(), Vector2.zeroVector, 100, Color.SkyBlue * Projectile.Opacity, 1.2f * Projectile.Opacity);
		diamond.noGravity = true;
		diamond.rotation = Projectile.rotation + MathHelper.PiOver2;

		Dust diamond2 = Dust.NewDustPerfect(Projectile.Center + new Vector2(100, 0).RotatedBy(Projectile.rotation), ModContent.DustType<IceScale>(), Vector2.zeroVector, 100, Color.SkyBlue * Projectile.Opacity, 1.8f * Projectile.Opacity);
		diamond2.noGravity = true;
		diamond2.rotation = Projectile.rotation + MathHelper.PiOver2;
		if (Projectile.timeLeft % 2 == 1)
		{
			Dust diamond3 = Dust.NewDustPerfect(Projectile.Center + new Vector2(170, 0).RotatedBy(Projectile.rotation), ModContent.DustType<IceScale>(), Vector2.zeroVector, 100, Color.SkyBlue * Projectile.Opacity, 3.8f * Projectile.Opacity);
			diamond3.noGravity = true;
			diamond3.rotation = Projectile.rotation + MathHelper.PiOver2;
		}
		Dust diamond4 = Dust.NewDustPerfect(Projectile.Center + new Vector2(190, 0).RotatedBy(Projectile.rotation), ModContent.DustType<IceScale2>(), Vector2.zeroVector, 100, Color.SkyBlue * Projectile.Opacity, 1.8f * Projectile.Opacity);
		diamond4.noGravity = true;
		diamond4.rotation = Projectile.rotation + MathHelper.PiOver2;

		Dust diamond5 = Dust.NewDustPerfect(Projectile.Center + new Vector2(130 + 30 * MathF.Sin(Projectile.timeLeft), 0).RotatedBy(Projectile.rotation), ModContent.DustType<IceScale3>(), Vector2.zeroVector, 100, Color.SkyBlue * Projectile.Opacity, 2.6f * Projectile.Opacity);
		diamond5.noGravity = true;
		diamond5.rotation = Projectile.rotation + MathHelper.PiOver2;

		Projectile.scale *= Projectile.ai[2]; // Set the scale of the projectile to the scale of the item.

		// If the projectile is as old as the max animation time, kill the projectile.
		if (Projectile.localAI[0] >= Projectile.ai[1])
		{
			Projectile.Kill();
		}

		// This for loop spawns the visuals when using Flasks (weapon imbues)
		for (float i = -MathHelper.PiOver4; i <= MathHelper.PiOver4; i += MathHelper.PiOver2)
		{
			Rectangle rectangle = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + i).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
			Projectile.EmitEnchantmentVisualsAt(rectangle.TopLeft(), rectangle.Width, rectangle.Height);
		}
	}

	// Here is where we have our custom collision.
	// This collision will only run if the projectile is within range of target with the range being Projectile.ownerHitCheckDistance
	// Or if the projectile hasn't already hit all of the targets it can with Projectile.penetrate
	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		// This is how large the circumference is, aka how big the range is. Vanilla uses 94f to match it to the size of the texture.
		float coneLength = 94f * Projectile.scale;
		// This number affects how much the start and end of the collision will be rotated.
		// Bigger Pi numbers will rotate the collision counter clockwise.
		// Smaller Pi numbers will rotate the collision clockwise.
		// (Projectile.ai[0] is the direction)
		float collisionRotation = MathHelper.Pi * 2f / 25f * Projectile.ai[0];
		float maximumAngle = MathHelper.PiOver4; // The maximumAngle is used to limit the rotation to create a dead zone.
		float coneRotation = Projectile.rotation + collisionRotation;

		// First, we check to see if our first cone intersects the target.
		if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}

		// The first cone isn't the entire swinging arc, though, so we need to check a second cone for the back of the arc.
		float backOfTheSwing = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
		if (backOfTheSwing > 0f)
		{
			float coneRotation2 = coneRotation - MathHelper.PiOver4 * Projectile.ai[0] * backOfTheSwing;

			if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation2, maximumAngle))
			{
				return true;
			}
		}

		return false;
	}

	public override void CutTiles()
	{
		// Here we calculate where the projectile can destroy grass, pots, Queen Bee Larva, etc.
		Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		float width = 60f * Projectile.scale;
		Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
		// new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
		// Projectile.owner);
		Spawn_CustomColorExcalibur(new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, new Color(0f, 0.5f, 0.6f, 0.5f), new Color(0f, 0.82f, 0.82f, 1f));

		// Set the target's hit direction to away from the player so the knockback is in the correct direction.
		hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);

		target.AddBuff(BuffID.Chilled, 100);
		if (Main.rand.NextBool(10))
		{
			target.AddBuff(BuffID.Frostburn, 100);
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
			new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
			Projectile.owner);

		info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
	}

	// Taken from Main.DrawProj_Excalibur()
	// Look at the source code for the other sword types.
	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 position = Projectile.Center - Main.screenPosition;
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Rectangle sourceRectangle = texture.Frame(1, 4); // The sourceRectangle says which frame to use.
		Vector2 origin = sourceRectangle.Size() / 2f;
		float scale = Projectile.scale * 1.1f;
		SpriteEffects spriteEffects = (!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None; // Flip the sprite based on the direction it is facing.
		float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
		float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
		float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
		lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

		Color backDarkColor = new Color(60, 160, 180); // Original Excalibur color: Color(180, 160, 60)
		Color middleMediumColor = new Color(80, 255, 255); // Original Excalibur color: Color(255, 255, 80)
		Color frontLightColor = new Color(150, 240, 255); // Original Excalibur color: Color(255, 240, 150)

		Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
		whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
		Color faintLightingColor = whiteTimesLerpTime * lightingColor * 0.5f;
		faintLightingColor.G = (byte)(faintLightingColor.G * lightingColor);
		faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

		Texture2D sword = ModAsset.CyanFrost.Value;
		Main.EntitySpriteDraw(sword, position + new Vector2(70, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver4 * player.direction), new Rectangle(0, 0, 104, 104), lightColor, Projectile.rotation + MathHelper.PiOver2 * player.direction, sword.Size() * 0.5f, 1f, spriteEffects, 0f);
		// Back part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lightingColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);
		// Very faint part affected by the light color
		Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
		// Middle part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lightingColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);
		// Front part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lightingColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);
		// Thin top line (final frame)
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.6f * lerpTime, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
		// Thin middle line (final frame)
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.5f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
		// Thin bottom line (final frame)
		Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, 0, 3), Color.White * 0.4f * lerpTime, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

		// This draws some sparkles around the circumference of the swing.
		for (float i = 0f; i < 8f; i += 1f)
		{
			float edgeRotation = Projectile.rotation + Projectile.ai[0] * i * (MathHelper.Pi * -2f) * 0.025f + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0];
			Vector2 drawpos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 6f) * scale;
			DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Color(180, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
		}

		// This draws a large star sparkle at the front of the projectile.
		Vector2 drawpos2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * ((float)texture.Width * 0.5f - 4f) * scale;
		DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos2, new Color(180, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

		// Uncomment this line for a visual representation of the projectile's size.
		// Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, position, sourceRectangle, Color.Orange * 0.75f, 0f, origin, scale, spriteEffects);

		return false;
	}

	// Copied from Main.DrawPrettyStarSparkle() which is private
	private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
	{
		Texture2D sparkleTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
		Color bigColor = shineColor * opacity * 0.5f;
		bigColor.A = 0;
		Vector2 origin = sparkleTexture.Size() / 2f;
		Color smallColor = drawColor * 0.5f;
		float lerpValue = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
		Vector2 scaleLeftRight = new Vector2(fatness.X * 0.5f, scale.X) * lerpValue;
		Vector2 scaleUpDown = new Vector2(fatness.Y * 0.5f, scale.Y) * lerpValue;
		bigColor *= lerpValue;
		smallColor *= lerpValue;
		// Bright, large part
		Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawpos, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);
		// Dim, small part
		Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawpos, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
	}

	// Copied from Terraria.GameContent.Drawing.ParticleOrchestra.Spawn_Excalibur which is private
	private static PrettySparkleParticle GetNewPrettySparkleParticle() => new PrettySparkleParticle();

	private static ParticlePool<PrettySparkleParticle> _poolPrettySparkle = new ParticlePool<PrettySparkleParticle>(200, GetNewPrettySparkleParticle);

	/// <summary>
	/// A custom version of Spawn_Excalibur from Terraria.GameContent.Drawing.ParticleOrchestra
	/// </summary>
	/// <param name="settings">The Particle Orchestra Settings</param>
	/// <param name="colorTint1">The First Color Tint of the Sparkle</param>
	/// <param name="colorTint2">The Second Color Tint of the Sparkle. Leave empty to default to colorTint1</param>
	internal static void Spawn_CustomColorExcalibur(ParticleOrchestraSettings settings, Color colorTint1, Color colorTint2 = default)
	{
		if (colorTint2 == default)
		{
			colorTint2 = colorTint1;
		}

		float num = 30f;
		float num2 = 0f;
		for (float num3 = 0f; num3 < 4f; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 2f * num3 + num2).ToRotationVector2() * 2f; // Was 4f;
			prettySparkleParticle.ColorTint = colorTint1; // Originally new Color(0.9f, 0.85f, 0.4f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2((num3 % 2f == 0f) ? 2f : 4f, 0.5f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.Velocity = -vector * 0.2f;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num3 % 2f == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 1.5f;
			}

			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}

		for (float num4 = 0f; num4 < 4f; num4 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 2f * num4 + num2).ToRotationVector2() * 2f; // Was 4f;
			prettySparkleParticle2.ColorTint = colorTint2; // Originally new Color(1f, 1f, 0.2f, 1f)
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2((num4 % 2f == 0f) ? 2f : 4f, 0.5f) * 0.7f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.Velocity = vector2 * 0.2f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			if (num4 % 2f == 1f)
			{
				prettySparkleParticle2.Scale *= 1.5f;
				prettySparkleParticle2.Velocity *= 1.5f;
			}

			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			for (int i = 0; i < 1; i++)
			{
				Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, DustID.Frost, vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust.noGravity = true;
				dust.scale = 0.8f;
				Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, DustID.Frost, -vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust2.noGravity = true;
				dust2.scale = 0.8f;
			}
		}
	}
	// Do we need this:
	// internal static void Spawn_CustomColorExcaliburWithRequests(bool clientOnly, ParticleOrchestraType type, ParticleOrchestraSettings settings, int? overrideInvokingPlayerIndex = null)
	// {
	// if (clientOnly)
	// ParticleOrchestrator.SpawnParticlesDirect(type, settings);
	// else
	// NetManager.Instance.SendToServerAndSelf(NetParticlesModule.Serialize(type, settings));
	// Spawn_CustomColorExcalibur(settings, new Color(0f, 0.56f, 0.6f, 0.5f));
	// }
}
