using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class Glow : ModProjectile
{
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
		Main.projFrames[Type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 4;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.ownerHitCheck = true;
		Projectile.ownerHitCheckDistance = 140f;
		Projectile.usesOwnerMeleeHitCD = true;
		Projectile.stopsDealingDamageAfterPenetrateHits = true;

		Projectile.aiStyle = -1;

		Projectile.noEnchantmentVisuals = true;
	}

	public override void AI()
	{
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
		float distance = Main.rand.NextFloat(10f, 80.0f);
		var spark = new RayDustDust
		{
			Velocity = Vector2.Normalize(new Vector2(player.direction, 1).RotatedBy(Projectile.rotation * player.direction + MathHelper.PiOver2)) * distance / 12f * player.direction,
			Active = true,
			Visible = true,
			Position = Projectile.Center + Vector2.Normalize(new Vector2(player.direction, 1).RotatedBy(Projectile.rotation * player.direction)) * distance,
			MaxTime = Main.rand.Next(137, 245),
			Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.4f, 2.0f)) * MathF.Sqrt(distance),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { 0, 0 },
		};
		Ins.VFXManager.Add(spark);

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

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float coneLength = 94f * Projectile.scale;
		float collisionRotation = MathHelper.Pi * 2f / 25f * Projectile.ai[0];
		float maximumAngle = MathHelper.PiOver4;
		float coneRotation = Projectile.rotation + collisionRotation;

		if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength, coneRotation, maximumAngle))
		{
			return true;
		}

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
		Vector2 starting = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		Vector2 ending = (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * 60f * Projectile.scale;
		float width = 60f * Projectile.scale;
		Utils.PlotTileLine(Projectile.Center + starting, Projectile.Center + ending, width, DelegateMethods.CutTiles);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Spawn_CustomColorExcalibur(new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, new Color(0.6f, 0.3f, 0, 0.5f), new Color(0.82f, 0.22f, 0, 1f));

		hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur,
			new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
			Projectile.owner);

		info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 position = Projectile.Center - Main.screenPosition;
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Rectangle sourceRectangle = texture.Frame(1, 4); // The sourceRectangle says which frame to use.
		Vector2 origin = sourceRectangle.Size() / 2f;
		float scale = Projectile.scale * 0.5f;
		SpriteEffects spriteEffects = (!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None; // Flip the sprite based on the direction it is facing.
		float percentageOfLife = Projectile.localAI[0] / Projectile.ai[1]; // The current time over the max time.
		float lerpTime = Utils.Remap(percentageOfLife, 0f, 0.6f, 0f, 1f) * Utils.Remap(percentageOfLife, 0.6f, 1f, 1f, 0f);
		float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
		lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);

		Color backDarkColor = new Color(145, 30, 0, 50); // Original Excalibur color: Color(180, 160, 60)
		Color middleMediumColor = new Color(255, 120, 0, 150); // Original Excalibur color: Color(255, 255, 80)
		Color frontLightColor = new Color(255, 240, 100); // Original Excalibur color: Color(255, 240, 150)

		Color whiteTimesLerpTime = Color.White * lerpTime * 0.5f;
		whiteTimesLerpTime.A = (byte)(whiteTimesLerpTime.A * (1f - lightingColor));
		Color faintLightingColor = whiteTimesLerpTime * 0.5f;
		faintLightingColor.G = faintLightingColor.G;
		faintLightingColor.B = (byte)(faintLightingColor.R * (0.25f + lightingColor * 0.75f));

		Texture2D sword = ModAsset.Weapons_Glow.Value;
		Main.EntitySpriteDraw(sword, position + new Vector2(40, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver4 * player.direction), null, lightColor, Projectile.rotation + MathHelper.PiOver2 * player.direction, sword.Size() * 0.5f, 1f, spriteEffects, 0f);

		// Back part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, backDarkColor * lerpTime, Projectile.rotation + Projectile.ai[0] * MathHelper.PiOver4 * -1f * (1f - percentageOfLife), origin, scale, spriteEffects, 0f);

		// Very faint part affected by the light color
		Main.EntitySpriteDraw(texture, position, sourceRectangle, faintLightingColor * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);

		// Middle part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, middleMediumColor * lerpTime * 0.3f, Projectile.rotation, origin, scale, spriteEffects, 0f);

		// Front part
		Main.EntitySpriteDraw(texture, position, sourceRectangle, frontLightColor * lerpTime * 0.5f, Projectile.rotation, origin, scale * 0.975f, spriteEffects, 0f);

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
			Vector2 drawPosition = position + edgeRotation.ToRotationVector2() * (texture.Width * 0.5f - 6f) * scale;
			DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPosition, new Color(180, 255, 255, 0) * lerpTime * (i / 9f), middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(percentageOfLife, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
		}

		// This draws a large star sparkle at the front of the projectile.
		Vector2 drawPosition2 = position + (Projectile.rotation + Utils.Remap(percentageOfLife, 0f, 1f, 0f, MathHelper.PiOver4) * Projectile.ai[0]).ToRotationVector2() * (texture.Width * 0.5f - 4f) * scale;
		DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPosition2, new Color(180, 255, 255, 0) * lerpTime * 0.5f, middleMediumColor, percentageOfLife, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(percentageOfLife, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale);

		// Uncomment this line for a visual representation of the projectile's size.
		// Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, position, sourceRectangle, Color.Orange * 0.75f, 0f, origin, scale, spriteEffects);
		return false;
	}

	// Copied from Main.DrawPrettyStarSparkle() which is private
	private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPosition, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
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
		Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, bigColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, bigColor, 0f + rotation, origin, scaleUpDown, dir);

		// Dim, small part
		Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, smallColor, MathHelper.PiOver2 + rotation, origin, scaleLeftRight * 0.6f, dir);
		Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, smallColor, 0f + rotation, origin, scaleUpDown * 0.6f, dir);
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
		}
	}
}
