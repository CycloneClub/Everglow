using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Ocean.Common;
using Everglow.Ocean.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Ocean.Projectiles.Weapons;

public class RampageShark : ModProjectile
{
	public override string Texture => "Everglow/Ocean/Projectiles/Weapons/RampageShark/RampageShark_gun";
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 100000;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Ranged;
	}
	internal float Power = 0;
	internal IEntitySource shootSource = null;
	public override void OnSpawn(IEntitySource source)
	{
		shootSource = source;
	}
	private void Shoot()
	{
		Player player = Main.player[Projectile.owner];
		float chance = Power / 100f + player.GetCritChance(DamageClass.Generic) / 300f;
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		Vector2 velocity = Vector2.Normalize(toMouse) * 27;
		Item item = player.HeldItem;
		var rampage = item.ModItem as Items.Weapons.RampageShark;
		var offset = new Vector2(0, -5);
		if (rampage != null)
		{
			Vector2 random = new Vector2(0, Main.rand.NextFloat(Power * 0.2f)).RotatedByRandom(6.283);
			if (Power >= 16)
			{
				new Vector2(0, Main.rand.NextFloat(Power)).RotatedByRandom(6.283);
			}

			if (Main.rand.NextFloat(1f) > chance)
			{
				ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
				Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
				if (Power == 16)
					SoundEngine.PlaySound(new SoundStyle("Everglow/Ocean/Sounds/SharkGun0").WithVolumeScale(0.6f).WithPitchOffset(0.2f), Projectile.Center);
				else
				{
					SoundEngine.PlaySound(new SoundStyle("Everglow/Ocean/Sounds/SharkGun0").WithVolumeScale(0.4f), Projectile.Center);
				}
				Vector2 newvelocity = velocity.RotatedBy(Main.rand.NextFloat(-Power / 244f, Power / 244f));
				Projectile p = Projectile.NewProjectileDirect(shootSource, Projectile.Center + offset + velocity * 0.0f + random, newvelocity, rampage.ShootType, item.damage, item.knockBack, player.whoAmI);
				p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));

				Dust d = Dust.NewDustDirect(Projectile.Center + offset + velocity * 0.7f - new Vector2(4, 8) + random, 0, 0, ModContent.DustType<BulletShell>(), velocity.X, velocity.Y, 0, default, 1f);
				d.velocity = velocity.RotatedBy(-1.57 * player.direction - Main.rand.NextFloat(1.25f, 1.75f) * player.direction) * 0.4f * (24 + Power) / 54f;
				d.noGravity = false;
				d.scale = 1f;

				float rot = newvelocity.ToRotation();
				Projectile.NewProjectile(shootSource, Projectile.Center + offset * 1.5f + velocity * 1.3f + random, Vector2.Zero, ModContent.ProjectileType<RampageSharkHit>(), item.damage, item.knockBack, player.whoAmI, 0.06f, rot);
			}
			else
			{
				ScreenShaker Gsplayer = player.GetModPlayer<ScreenShaker>();
				Gsplayer.FlyCamPosition = new Vector2(0, 75).RotatedByRandom(6.283);
				SoundEngine.PlaySound(new SoundStyle("Everglow/Ocean/Sounds/SharkGun1").WithPitchOffset(-0.8f), Projectile.Center);
				int Times = Main.rand.Next(4, 7);

				for (int i = 0; i < Times; i++)
				{
					Vector2 newvelocity = velocity.RotatedBy(Main.rand.NextFloat(-Power / 66f, Power / 66f));
					Projectile p = Projectile.NewProjectileDirect(shootSource, Projectile.Center + offset + velocity * 0.0f + random, newvelocity, rampage.ShootType, item.damage, item.knockBack, player.whoAmI);
					p.CritChance = (int)(item.crit + player.GetCritChance(DamageClass.Generic));
					Dust d = Dust.NewDustDirect(Projectile.Center + offset + velocity * 0.7f - new Vector2(4, 8) + random, 0, 0, ModContent.DustType<BulletShell>(), velocity.X, velocity.Y, 0, default, 1f);
					d.velocity = velocity.RotatedBy(-1.57 * player.direction - Main.rand.NextFloat(1.25f, 1.75f) * player.direction) * 0.4f * (24 + Power) / 54f;
					d.noGravity = false;
					d.scale = 1f;
					float rot = newvelocity.ToRotation();
					Projectile.NewProjectile(shootSource, Projectile.Center + offset * 1.5f + velocity * 1.3f + random, Vector2.Zero, ModContent.ProjectileType<RampageSharkHit>(), item.damage, item.knockBack, player.whoAmI, 0.12f, rot);
				}
			}
			for (int x = 0; x < (Power + 16) / 2; x++)
			{
				Dust d = Dust.NewDustDirect(Projectile.Center + offset + velocity * 0.7f - new Vector2(4, 8) + random, 0, 0, ModContent.DustType<GunSpark>(), velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(0.2f, 0.6f));
				Vector2 newvelocity = velocity.RotatedBy(Main.rand.NextFloat(-Power / 66f, Power / 66f)) * Main.rand.NextFloat(0.85f, 1.65f);
				d.velocity = newvelocity;
				d.noGravity = true;
			}
		}
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		var rampage = player.HeldItem.ModItem as Items.Weapons.RampageShark;
		if (rampage != null)
			Power = rampage.CrazyValue;
		else
		{
			Projectile.Kill();
		}

		Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
		toMouse = Vector2.Normalize(toMouse);
		if (player.controlUseItem)
		{
			Projectile.rotation = (float)(Math.Atan2(toMouse.Y, toMouse.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(toMouse) * 6;
			{
			}
			;
			Projectile.velocity *= 0;
			if (Projectile.timeLeft % player.HeldItem.useTime == 0)
			{
				Shoot();
			}

			if (Projectile.timeLeft % player.HeldItem.useTime == player.HeldItem.useTime / 2 && Power == 16)
			{
				Shoot();
			}
		}
		else
		{
			Projectile.Kill();
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		Vector2 toMouse = Projectile.Center - player.MountedCenter;
		if (player.controlUseItem)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(toMouse.Y, toMouse.X) - Math.PI / 2d));
		}

		Texture2D texMain = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_gun");
		Texture2D texMainG = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_glow");
		Texture2D TexEye = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/RampageShark_redEye");
		Texture2D TexStar = OceanContent.QuickTexture("Projectiles/Weapons/RampageShark/HitStar");
		var gunTexRectangle = new Rectangle(0, 0, 72, 34);
		if (Main.timeForVisualEffects % 6 >= 3)
		{
			gunTexRectangle.Y = (int)(Main.timeForVisualEffects / 6f) % 4 * 34;
		}

		SpriteEffects se = SpriteEffects.None;
		if (Projectile.Center.X < player.Center.X)
		{
			se = SpriteEffects.FlipVertically;
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
		Vector2 random = new Vector2(0, Main.rand.NextFloat(Power * 0.2f)).RotatedByRandom(6.283);
		if (Power >= 16)
		{
			new Vector2(0, Main.rand.NextFloat(Power)).RotatedByRandom(6.283);
		}

		var offset = new Vector2(0, -5);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + offset - random, gunTexRectangle, lightColor, Projectile.rotation - (float)(Math.PI * 0.25), new Vector2(texMain.Size().X / 2f, texMain.Size().Y / 8f), 1f, se, 0);
		float glow = Power / 16f;
		Main.spriteBatch.Draw(texMainG, Projectile.Center - Main.screenPosition + offset - random, null, new Color(glow, glow * 0.2f, glow * 0.2f, 0), Projectile.rotation - (float)(Math.PI * 0.25), texMainG.Size() / 2f, 1f, se, 0);
		if (Power >= 15 && Power < 16)
		{
			if (Power < 15 + 1 / 30f)
			{
				SoundEngine.PlaySound(SoundID.Shatter);
			}

			float progress = Power - 15f;
			float powerII = MathF.Sin(progress * MathF.PI);
			Main.spriteBatch.Draw(TexEye, Projectile.Center - Main.screenPosition + offset - random, null, new Color(progress, progress, progress, progress), Projectile.rotation - (float)(Math.PI * 0.25), TexEye.Size() / 2f, 1f, se, 0);

			Vector2 StarCenter = Projectile.Center - Main.screenPosition + offset + new Vector2(-2, -10).RotatedBy(Projectile.rotation) - random * 2;
			if (player.direction == -1)
			{
				StarCenter = Projectile.Center - Main.screenPosition + offset + new Vector2(10, -2).RotatedBy(Projectile.rotation);
			}

			Main.spriteBatch.Draw(TexStar, StarCenter, null, new Color(1f, 0, 0, 0), Projectile.rotation + progress * 1f, TexStar.Size() / 2f, new Vector2(progress * 2, powerII * powerII) * 0.36f, se, 0);
			Main.spriteBatch.Draw(TexStar, StarCenter, null, new Color(1f, 0, 0, 0), Projectile.rotation + progress * 1f - MathF.PI * 0.5f, TexStar.Size() / 2f, new Vector2(progress * 2, powerII * powerII) * 0.36f, se, 0);

			Ins.Batch.Begin();
			DrawTexCircle_VFXBatch(Ins.Batch, progress * 120f, (1f - progress) * 10, new Color(1f - progress, 0, 0, 0), StarCenter, ModAsset.FogTraceLight.Value);
			Ins.Batch.End();
		}
		else if (Power >= 16)
		{
			Main.spriteBatch.Draw(TexEye, Projectile.Center - Main.screenPosition + offset - random, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation - (float)(Math.PI * 0.25), TexEye.Size() / 2f, 1f, se, 0);
		}
	}
	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radious / 2; h += 1)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.2f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}
}
