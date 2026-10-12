using Everglow.Myth.Misc.Items.Weapons.Clubs;
using SteelSeries.GameSense;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class Titanium_Fragment : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = 6;
		Projectile.usesIDStaticNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
	}

	public Vector2 OribTrack = Vector2.Zero;
	public int AITimer = 0;
	public float TimeValue = 0;
	public float MulRange = 1f;

	public override void OnSpawn(IEntitySource source)
	{
		AITimer = Main.rand.Next(-40, 40);
		Projectile.timeLeft = 15000;
		Projectile.frame = Main.rand.Next(6);
		Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
		Projectile.ai[1] = Main.rand.NextFloat(-0.4f, 0.4f);
		Projectile.scale = Main.rand.NextFloat(0.9f, 1.7f);
		MulRange = 1f;
		base.OnSpawn(source);
	}

	public void GenerateSpark(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(7f, 27f)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
			var spark = new FireSpark_TitaniumDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(5, 15),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4.1f, 57.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		AITimer++;
		if (Projectile.timeLeft < 70)
		{
			Projectile.alpha += 4;
		}
		if (Projectile.timeLeft < 150)
		{
			bool canFall = true;
			Projectile.velocity *= 0.96f;
			if (TileUtils.PlatformCollision(Projectile.Center + new Vector2(Projectile.velocity.X, 0)))
			{
				SoundEngine.PlaySound(SoundID.NPCHit4.WithVolume(Main.rand.NextFloat(0.001f, 0.03f) * Projectile.velocity.Length()).WithPitchOffset(Main.rand.NextFloat(-0.5f, 0.5f)), Projectile.Center);
				Projectile.velocity.X *= Main.rand.NextFloat(-0.9f, -0.6f);
				canFall = false;
			}
			if (TileUtils.PlatformCollision(Projectile.Center + new Vector2(0, Projectile.velocity.Y)))
			{
				SoundEngine.PlaySound(SoundID.NPCHit4.WithVolume(Main.rand.NextFloat(0.001f, 0.03f) * Projectile.velocity.Length()).WithPitchOffset(Main.rand.NextFloat(-0.5f, 0.5f)), Projectile.Center);
				Projectile.velocity.Y *= Main.rand.NextFloat(-0.9f, -0.6f);
				canFall = false;
			}
			if (canFall)
			{
				Projectile.velocity += new Vector2(0, 0.6f);
			}
			return;
		}
		else
		{
			if (player.HeldItem.type != ModContent.ItemType<TitaniumClub_Item>())
			{
				if (Main.rand.NextBool(4))
				{
					Projectile.timeLeft = 149;
					Projectile.velocity *= 0.2f;
				}
			}
		}
		TimeValue = (float)(Main.time / Projectile.ai[0] * 5f) + Projectile.whoAmI * 4.3742f;
		float timeValue2 = TimeValue % MathHelper.TwoPi;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<TitaniumClub_smash>()] > 0)
		{
			MulRange = (float)Utils.Lerp(MulRange, 0, 0.02f);
		}
		else
		{
			MulRange = (float)Utils.Lerp(MulRange, 1, 0.02f);
		}
		OribTrack = new Vector2(Projectile.ai[0] * MulRange, 0).RotatedBy(TimeValue);
		OribTrack.Y *= 0.2f;
		if (timeValue2 > MathHelper.Pi)
		{
			Projectile.hide = true;
		}
		else
		{
			Projectile.hide = false;
		}
		if (AITimer < 60)
		{
			Projectile.velocity *= 0.96f;
			Projectile.rotation += Projectile.ai[1] * Projectile.velocity.Length() * 0.25f;
		}
		if (AITimer >= 60 && AITimer < 80)
		{
			Projectile.velocity *= 0.75f;
			Projectile.rotation += Projectile.ai[1] * Projectile.velocity.Length() * 0.25f;
		}
		if (AITimer >= 80)
		{
			Vector2 toAim = player.Center + OribTrack - Projectile.Center - Projectile.velocity;
			Vector2 toAim2 = player.Center - player.velocity + OribTrack - Projectile.Center;
			if (toAim2.Length() < 25)
			{
				Projectile.Center = player.Center + OribTrack;
				return;
			}
			if (toAim2.Length() > 800)
			{
				if (Projectile.timeLeft > 150)
				{
					Projectile.timeLeft = 149;
				}
				return;
			}
			Projectile.rotation += Projectile.ai[1];
			Projectile.velocity *= 0.95f;
			Vector2 acc = toAim * 0.07f;
			Projectile.velocity += acc;
		}
		if (AITimer == 100 + (int)(Math.Sin(Projectile.whoAmI) * 25))
		{
			SoundEngine.PlaySound(SoundID.NPCHit4.WithVolume(Main.rand.NextFloat(0.02f, 0.08f)).WithPitchOffset(Main.rand.NextFloat(-0.5f, 0.5f)), Projectile.Center);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		if (Projectile.hide)
		{
			behindNPCs.Add(index);
		}
		else
		{
			overPlayers.Add(index);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float mulColor = (255 - Projectile.alpha) / 255f;
		float timeValue2 = (TimeValue + Projectile.rotation) % MathHelper.TwoPi;
		Texture2D texture_b = ModAsset.Titanium_Fragment_dark.Value;
		Texture2D texture = ModAsset.Titanium_Fragment.Value;
		Texture2D texture_l0 = ModAsset.Titanium_Fragment_highlight0.Value;
		Texture2D texture_l1 = ModAsset.Titanium_Fragment_highlight1.Value;
		Texture2D texture_l2 = ModAsset.Titanium_Fragment_highlight2.Value;

		Rectangle frame = new Rectangle(0, 20 * Projectile.frame, 20, 20);
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Vector2 origin = new Vector2(10);
		Main.EntitySpriteDraw(texture_b, drawPos, frame, lightColor * mulColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(texture, drawPos, frame, lightColor * mulColor * (0.5f + MathF.Sin(TimeValue) * 0.5f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

		Color light2 = lightColor;
		light2.A = 0;

		float value0 = MathF.Pow(0.5f - Math.Abs(timeValue2 - 1), 1.7f) * 5.4f * (0.9f + MathF.Sin(TimeValue) * 0.3f);
		float value1 = MathF.Pow(0.5f - Math.Abs(timeValue2 - 3), 1.7f) * 5.4f * (0.9f + MathF.Sin(TimeValue) * 0.3f);
		float value2 = MathF.Pow(0.5f - Math.Abs(timeValue2 - 5), 1.7f) * 5.4f * (0.9f + MathF.Sin(TimeValue) * 0.3f);
		Main.EntitySpriteDraw(texture_l0, drawPos, frame, light2 * value0 * mulColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(texture_l1, drawPos, frame, light2 * value1 * mulColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(texture_l2, drawPos, frame, light2 * value2 * mulColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextBool(4))
		{
			Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.Titanium);
		}
		GenerateSpark(3);
		base.OnHitNPC(target, hit, damageDone);
	}
}
