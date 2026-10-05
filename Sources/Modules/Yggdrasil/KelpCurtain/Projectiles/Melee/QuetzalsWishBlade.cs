using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Terraria.Audio;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class QuetzalsWishBlade : ModProjectile
{
	private enum AiSlot
	{
		Stage,
		Frame,
		Aim,
	}

	private enum AttackStage
	{
		FirstThrust,
		SecondThrust,
		DownwardSlash,
		RisingSlash,
		DoubleSpin,
		Charge,
		Thrown,
		Explosion,
		GroundSlash,
	}

	private AttackStage Stage
	{
		get => (AttackStage)(int)Projectile.ai[(int)AiSlot.Stage];
		set => Projectile.ai[(int)AiSlot.Stage] = (float)value;
	}

	private int Frame
	{
		get => (int)Projectile.ai[(int)AiSlot.Frame];
		set => Projectile.ai[(int)AiSlot.Frame] = value;
	}

	private float Aim
	{
		get => Projectile.ai[(int)AiSlot.Aim];
		set => Projectile.ai[(int)AiSlot.Aim] = value;
	}

	private bool previousRight;
	private bool explosionVisualsShown;
	private float bladeReach = 112f;

	public bool IsHeld => Stage <= AttackStage.Charge;

	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => $"Terraria/Images/Item_{ItemID.TerraBlade}";

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 240;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override bool ShouldUpdatePosition() => Stage is AttackStage.Thrown or AttackStage.GroundSlash;

	public override bool? CanDamage() => Stage == AttackStage.Charge ? false : null;

	public override void AI()
	{
		if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
		{
			Projectile.Kill();
			return;
		}
		var player = Main.player[Projectile.owner];
		if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<QuetzalsWish>())
		{
			Projectile.Kill();
			return;
		}

		Frame++;
		if (Stage == AttackStage.Explosion)
		{
			ResizeExplosion();
			ShowExplosion();
			Projectile.timeLeft = 2;
			if (Frame >= 3)
			{
				Projectile.Kill();
			}
			return;
		}
		if (Stage == AttackStage.Thrown)
		{
			Projectile.tileCollide = true;
			Projectile.timeLeft = 2;
			Projectile.rotation += 0.35f;
			Projectile.velocity.Y += 0.1f;
			AddDust();
			if (Frame >= 240)
			{
				Projectile.Kill();
			}
			return;
		}
		if (Stage == AttackStage.GroundSlash)
		{
			Projectile.tileCollide = true;
			Projectile.timeLeft = 2;
			Projectile.rotation = -MathHelper.PiOver2;
			AddDust();
			if (Frame >= 18)
			{
				Projectile.Kill();
			}
			return;
		}

		Projectile.timeLeft = 2;
		player.heldProj = Projectile.whoAmI;
		player.itemTime = 2;
		player.itemAnimation = 2;
		int direction = MathF.Cos(Aim) >= 0f ? 1 : -1;
		player.ChangeDir(direction);
		bool owner = Projectile.owner == Main.myPlayer;
		bool right = owner && Main.mouseRight;
		if (owner && right && !previousRight && QuetzalsWishCombatRules.CanBranch((int)Stage))
		{
			Stage = AttackStage.DoubleSpin;
			Frame = 0;
			Array.Clear(Projectile.localNPCImmunity);
			Projectile.netUpdate = true;
			player.GetModPlayer<QuetzalsWishPlayer>().ResetCombo();
		}
		previousRight = right;

		if (Stage == AttackStage.Charge)
		{
			if (owner)
			{
				Aim = (Main.MouseWorld - player.MountedCenter).SafeNormalize(new Vector2(direction, 0f)).ToRotation();
				if (Frame % 6 == 0)
				{
					Projectile.netUpdate = true;
				}
				var result = QuetzalsWishCombatRules.ResolveCharge(Frame, right, true);
				if (result == QuetzalsWishCombatRules.ChargeResult.Cancelled)
				{
					Projectile.Kill();
					return;
				}
				if (result == QuetzalsWishCombatRules.ChargeResult.Throw)
				{
					Stage = AttackStage.Thrown;
					Frame = 0;
					Projectile.Center = player.MountedCenter;
					Projectile.velocity = Aim.ToRotationVector2() * 16f;
					Projectile.timeLeft = 240;
					Projectile.tileCollide = true;
					Projectile.netUpdate = true;
					return;
				}
			}
			Projectile.rotation = Aim - direction * MathHelper.PiOver2;
			Projectile.Center = player.MountedCenter + Projectile.rotation.ToRotationVector2() * 40f;
			AddDust();
			return;
		}

		float progress = Frame / (float)QuetzalsWishCombatRules.AttackFrames;
		bladeReach = 112f;
		switch (Stage)
		{
			case AttackStage.FirstThrust:
			case AttackStage.SecondThrust:
				if (Frame < 12)
				{
					Projectile.rotation = Aim;
					bladeReach = MathHelper.Lerp(40f, 128f, Frame / 12f);
				}
				else
				{
					Projectile.rotation = Aim + direction * MathHelper.Lerp(-1.5f, 1.5f, (Frame - 12) / 12f);
					if (Frame == 12)
					{
						Array.Clear(Projectile.localNPCImmunity);
						MovePlayer(player, -direction * 5f, owner);
					}
				}
				break;
			case AttackStage.DownwardSlash:
				Projectile.rotation = Aim + direction * MathHelper.Lerp(-1.8f, 1.2f, progress);
				break;
			case AttackStage.RisingSlash:
				Projectile.rotation = Aim + direction * MathHelper.Lerp(1.2f, -1.8f, progress);
				if (Frame == 12 && owner)
				{
					int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Bottom + new Vector2(direction * 24f, -48f), new Vector2(direction * 7f, 0f), Type, Projectile.damage, Projectile.knockBack, Projectile.owner, (int)AttackStage.GroundSlash, 0f, Aim);
					if (index >= 0 && index < Main.maxProjectiles)
					{
						Main.projectile[index].timeLeft = 18;
						Main.projectile[index].netUpdate = true;
					}
				}
				break;
			case AttackStage.DoubleSpin:
				Projectile.rotation = Aim + direction * progress * MathHelper.TwoPi * 2f;
				if (Frame < 6)
				{
					MovePlayer(player, direction * 8f, owner);
				}
				if (Frame == 12)
				{
					Array.Clear(Projectile.localNPCImmunity);
				}
				break;
		}
		Projectile.Center = player.MountedCenter + Projectile.rotation.ToRotationVector2() * (bladeReach * 0.5f);
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
		AddDust();
		if (Frame >= QuetzalsWishCombatRules.AttackFrames)
		{
			Projectile.Kill();
		}
	}

	private static void MovePlayer(Player player, float speed, bool owner)
	{
		if (owner)
		{
			player.velocity.X = Collision.TileCollision(player.position, new Vector2(speed, player.velocity.Y), player.width, player.height).X;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Stage == AttackStage.Explosion)
		{
			var nearest = new Vector2(MathHelper.Clamp(Projectile.Center.X, targetHitbox.Left, targetHitbox.Right), MathHelper.Clamp(Projectile.Center.Y, targetHitbox.Top, targetHitbox.Bottom));
			return Vector2.DistanceSquared(nearest, Projectile.Center) <= QuetzalsWishCombatRules.ExplosionRadius * QuetzalsWishCombatRules.ExplosionRadius;
		}
		if (Stage == AttackStage.Thrown)
		{
			return null;
		}
		float collisionPoint = 0f;
		Vector2 start = Stage == AttackStage.GroundSlash ? Projectile.Center + new Vector2(0f, 44f) : Main.player[Projectile.owner].MountedCenter;
		Vector2 end = Stage == AttackStage.GroundSlash ? Projectile.Center - new Vector2(0f, 52f) : start + Projectile.rotation.ToRotationVector2() * bladeReach;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24f, ref collisionPoint);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<QuetzalsWishWound>(), QuetzalsWishCombatRules.WoundFrames);
		Explode();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Stage != AttackStage.Thrown)
		{
			return true;
		}
		Explode();
		return false;
	}

	private void Explode()
	{
		if (!QuetzalsWishCombatRules.ShouldExplode((int)Stage, true))
		{
			return;
		}
		Stage = AttackStage.Explosion;
		Frame = 0;
		Projectile.velocity = Vector2.Zero;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 3;
		Projectile.netUpdate = true;
		ResizeExplosion();
		ShowExplosion();
	}

	private void ShowExplosion()
	{
		if (!Main.dedServ && !explosionVisualsShown)
		{
			explosionVisualsShown = true;
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			for (int i = 0; i < 32; i++)
			{
				Vector2 velocity = (MathHelper.TwoPi * i / 32f).ToRotationVector2() * 5f;
				Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, velocity, 0, default, 1.5f).noGravity = true;
			}
		}
	}

	private void ResizeExplosion()
	{
		Vector2 center = Projectile.Center;
		Projectile.width = Projectile.height = QuetzalsWishCombatRules.ExplosionRadius * 2;
		Projectile.Center = center;
		Projectile.tileCollide = false;
	}

	private void AddDust()
	{
		if (!Main.dedServ && Frame % 3 == 0)
		{
			Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, Vector2.Zero).noGravity = true;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (Main.dedServ || Stage == AttackStage.Explosion)
		{
			return false;
		}
		var texture = TextureAssets.Item[ItemID.TerraBlade].Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, texture.Size() * 0.5f, 1.5f, SpriteEffects.None);
		return false;
	}
}
