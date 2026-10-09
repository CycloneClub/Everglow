using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class HyperockSpearProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.IsAnNPCAttachedExplosive[Projectile.type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1500;
		Projectile.aiStyle = -1;
		Projectile.ArmorPenetration = 5;
		Projectile.penetrate = -1;
		Projectile.melee = true;
	}

	internal bool Shot = false;
	internal int Power = -15;
	public Vector2 oldpos = Vector2.Zero;
	public bool CollideOnTile = false;
	public bool CollideOnNPC = false;
	public NPC NPCStickTo = null;
	public Vector2 PostoNPC;
	public float rotationtoNPC;

	private Player Owner => Main.player[Projectile.owner];

	private Vector2 OwnerMouseWorld => Owner.MouseWorld();

	public override void AI()
	{
		if (Projectile.timeLeft <= 3 && Power > 40)
		{
			Projectile.tileCollide = false;
			Vector2 oldCenter = Projectile.Center;
			Projectile.width = 150;
			Projectile.height = 150;
			Projectile.Center = oldCenter;
		}

		int playerDir = -1;
		if (OwnerMouseWorld.X > Owner.Center.X)
		{
			playerDir = 1;
		}
		if (Shot)
		{
			if (CollideOnNPC)
			{
				if (Power > 40)
				{
					if (Projectile.timeLeft > 25)
					{
						for (int x = 0; x < 2; x++)
						{
							Vector2 vel = Vector2.UnitX.RotatedByRandom(MathF.PI * 2) * 4;
							var Vortex = new HyperockSpear_VortexLine
							{
								Velocity = vel * 0.001f,
								Active = true,
								Visible = true,
								PositiontoProjectile = vel.RotatedBy(MathF.PI * 0.5) * Main.rand.NextFloat(20, 70),
								OnTile = false,
								MaxTime = Main.rand.Next(120, 180),
								Scale = 14,
								VFXOwner = Projectile,
								ai = new float[] { 1, 0 },
							};
							Ins.VFXManager.Add(Vortex);
							oldpos = Projectile.Center;
						}
					}
					SticktoNPC();
					AbsorbNPC();
				}
			}
			else if (CollideOnTile)
			{
				Projectile.velocity = Vector2.Zero;
				if (Power > 40)
				{
					if (Projectile.timeLeft > 25)
					{
						for (int x = 0; x < 2; x++)
						{
							Vector2 vel = Vector2.UnitX.RotatedByRandom(MathF.PI * 2) * 4;
							var Vortex = new HyperockSpear_VortexLine
							{
								Velocity = vel * 0.001f,
								Active = true,
								Visible = true,
								PositiontoProjectile = vel.RotatedBy(MathF.PI * 0.5) * Main.rand.NextFloat(20, 70),
								OnTile = false,
								MaxTime = Main.rand.Next(120, 180),
								Scale = 14,
								VFXOwner = Projectile,
								ai = new float[] { 1, 0 },
							};
							Ins.VFXManager.Add(Vortex);
							oldpos = Projectile.Center;
						}
					}
					AbsorbNPC();
				}
			}
			else
			{
				Projectile.tileCollide = true;
				Projectile.friendly = true;
				Projectile.velocity.Y += 0.163f;
				Projectile.velocity *= 0.998f;
				Projectile.rotation = (float)(MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI * 0.25);
				if (Power > 40)
				{
					var d = Dust.NewDustDirect(Projectile.Center - new Vector2(4) - Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 18, 0, 0, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.2f, 0.6f));
					d.velocity = new Vector2(0, Main.rand.NextFloat(1f, 4f)).RotatedByRandom(6.283);
				}
			}
		}
		else
		{
			if (Power < 45)
			{
				Power += 1;
			}
			else
			{
				var d = Dust.NewDustDirect(Projectile.Center - new Vector2(4) - Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 18, 0, 0, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.2f, 0.6f));
				d.velocity = new Vector2(0, Main.rand.NextFloat(1f, 4f)).RotatedByRandom(6.283);
			}
			Projectile.timeLeft = 1500;
			Projectile.velocity = (OwnerMouseWorld - Owner.MountedCenter).SafeNormalize(new Vector2(0, -1 * Owner.gravDir));
			Projectile.Center = Owner.MountedCenter + Projectile.velocity.RotatedBy(Math.PI * -0.5) * 20 * playerDir - Projectile.velocity * (Power / 3f - 50) + new Vector2(0, 6 * Owner.gravDir);
			Projectile.rotation = (float)(MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathF.PI * 0.25);
			if (Power == 36)
			{
				for (int i = 0; i < 30; i++)
				{
					Vector2 vel = Vector2.UnitX.RotatedByRandom(MathF.PI * 2) * 10;
					Vector2 aimposition = Projectile.Center;
					var Vortex = new HyperockSpear_VortexLine
					{
						Velocity = vel,
						Active = true,
						Visible = true,
						PositiontoProjectile = Vector2.Zero,
						OnTile = false,
						MaxTime = Main.rand.Next(90, 120),
						Scale = 6,
						VFXOwner = Projectile,
						ai = new float[] { 0f, Main.rand.NextFloat(-0.05f, 0.05f) },
					};
					Ins.VFXManager.Add(Vortex);
				}
			}
			Owner.heldProj = Projectile.whoAmI;
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + 0.337f * playerDir + (float)(Math.PI * 0.25 + Math.PI * 0.6 * playerDir - (Power / 80d + 0.2) * playerDir));
			Owner.direction = playerDir;
		}
		if (Owner.controlUseItem && !Shot && !CollideOnNPC && !CollideOnTile)
		{
			Shot = true;
			Projectile.velocity = (OwnerMouseWorld - Owner.MountedCenter).SafeNormalize(new Vector2(0, -1 * Owner.gravDir)) * (Power + 40) / 4f;
			Projectile.damage = (int)(Projectile.damage * (Power / 90f + 1));
			Projectile.CritChance = (int)(Projectile.CritChance * (Power / 90f + 1));
			if (Power > 30)
			{
				Power = 45;
			}
			SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
			Owner.heldProj = -1;
		}
		if (Owner.HeldItem.type != ModContent.ItemType<Items.Weapons.RockElemental.HyperockSpear>() && !Shot)
		{
			Projectile.active = false;
		}

		Lighting.AddLight(Projectile.Center + Vector2.One.RotatedBy(Projectile.rotation + MathF.PI * 0.48) * 12.5f, 0.25f, 0.05f, 0.4f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModAsset.HyperockSpearProj.Value;
		Texture2D glow = ModAsset.HyperockSpearProj_glow.Value;

		Vector2 pos = Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 30;
		if (CollideOnNPC)
		{
			pos = Projectile.Center - Main.screenPosition + Vector2.UnitY.RotatedBy(Projectile.rotation + MathF.PI * 0.25) * 30;
		}
		Main.spriteBatch.Draw(tex, pos, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(glow, pos, null, Color.White, Projectile.rotation, glow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		if (Power > 30)
		{
			float value = Power - 30;
			value /= 4f;
			value = Math.Min(value, (48 - Power) * 0.3f);
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Vector2 starPos = Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.zeroVector) * 18;
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.6f, 0.1f, 1f, 0f), 0, star.Size() / 2f, new Vector2(0.4f, 0.5f) * value, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, starPos, null, new Color(0.6f, 0.1f, 1f, 0f), MathHelper.PiOver2, star.Size() / 2f, new Vector2(0.6f, 0.9f) * value, SpriteEffects.None, 0);
		}
		return false;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (!CollideOnNPC && !CollideOnTile)
		{
			if (Power < 40)
			{
				return true;
			}
			Projectile.velocity = Vector2.Zero;
			CollideOnTile = true;
			Projectile.timeLeft = 60;
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
			return false;
		}
		return true;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 8; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.7f);
			d.velocity = new Vector2(0, Main.rand.NextFloat(7f, 16f)).RotatedByRandom(6.283);
		}
		for (int x = 0; x < 16; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.6f, 1.1f));
			d.velocity = new Vector2(0, Main.rand.NextFloat(2f, 11f)).RotatedByRandom(6.283);
		}
		GenerateSmog(4);
		if (Power > 40)
		{
			for (int x = 0; x < 8; x++)
			{
				var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<RockElemental_fragments>(), 0f, 0f, 0, default, 0.9f);
				d.velocity = new Vector2(0, Main.rand.NextFloat(7f, 26f)).RotatedByRandom(6.283);
			}
			for (int x = 0; x < 16; x++)
			{
				var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.6f, 1.5f));
				d.velocity = new Vector2(0, Main.rand.NextFloat(2f, 21f)).RotatedByRandom(6.283);
			}
			for (int g = 0; g < 20; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(MathHelper.TwoPi);
				var somg = new RockSmogDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(37, 45),
					Scale = Main.rand.NextFloat(40f, 55f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			Vector2 vF = new Vector2(0, Main.rand.NextFloat(0, 15f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore0").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 15f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore1").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 15f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore2").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 15f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore3").Type, 1f);
		}
		else
		{
			Vector2 vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore0").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore1").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore2").Type, 1f);

			vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, Projectile.Center + vF, vF, ModContent.Find<ModGore>("Everglow/HyperockSpearProj_gore3").Type, 1f);
		}
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithVolume(0.5f), Projectile.Center);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(40f, 55f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (!CollideOnNPC && !CollideOnTile)
		{
			if (Power < 40)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] = 1;
			CollideOnNPC = true;
			NPCStickTo = target;
			Projectile.timeLeft = 60;
			PostoNPC = NPCStickTo.Center - Projectile.Center;
			rotationtoNPC = NPCStickTo.rotation;
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
			SticktoNPC();
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Projectile.Center.X ? 1 : -1;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public void AbsorbNPC()
	{
		float value = (Projectile.timeLeft - 6) / 34f;
		value = MathF.Sin(value * MathHelper.Pi);
		Lighting.AddLight(Projectile.Center, new Vector3(0.7f, 0.2f, 1f) * value * 2);
		if (Projectile.timeLeft <= 20 && Projectile.timeLeft >= 8)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && npc != NPCStickTo)
				{
					Vector2 distance = Projectile.Center - npc.Center;
					if (distance.Length() < Power * 8 && !npc.dontTakeDamage && !npc.friendly)
					{
						npc.velocity += distance.SafeNormalize(Vector2.zeroVector)
						* MathF.Min(Power / 45, npc.knockBackResist * 1145.14f / ((distance.Length() + 191) * Power / 98.10f));
					}
				}
			}
		}
	}

	public void SticktoNPC()
	{
		Projectile.ai[0] = 1;
		Projectile.ai[1] = NPCStickTo.whoAmI;

		Projectile.velocity = Vector2.Zero;
		Projectile.netUpdate = true;

		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Vector2 PCcenter = Vector2.Zero;

		if (NPCStickTo.active && !NPCStickTo.dontTakeDamage)
		{
			PostoNPC = PostoNPC.RotatedBy(NPCStickTo.rotation - rotationtoNPC);
			Projectile.rotation = Projectile.rotation + NPCStickTo.rotation - rotationtoNPC;
			rotationtoNPC = NPCStickTo.rotation;
			Projectile.Center = NPCStickTo.Center - PostoNPC;
			Projectile.gfxOffY = NPCStickTo.gfxOffY;
		}
		else
		{
			if (Projectile.timeLeft > 3)
			{
				Projectile.timeLeft = 3;
			}
		}
	}

	public override bool? CanDamage()
	{
		if (CollideOnNPC)
		{
			if (Projectile.timeLeft > 3)
			{
				return false;
			}
		}
		return base.CanDamage();
	}
}
