using Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;
using Everglow.Yggdrasil.Netcode;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using static Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury.ArmOfGiantTreeAttackState;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class ArmOfGiantTreeHeld : ModProjectile
{
	private enum AiSlot
	{
		Phase,
		Charge,
		Timer,
	}

	private AttackPhase SyncedPhase { get => (AttackPhase)Projectile.ai[(int)AiSlot.Phase]; set => Projectile.ai[(int)AiSlot.Phase] = (float)value; }

	private int SyncedCharge { get => (int)Projectile.ai[(int)AiSlot.Charge]; set => Projectile.ai[(int)AiSlot.Charge] = value; }

	private int SyncedTimer { get => (int)Projectile.ai[(int)AiSlot.Timer]; set => Projectile.ai[(int)AiSlot.Timer] = value; }

	private readonly ArmOfGiantTreeAttackState attack = new();
	private int selectedSlot;
	private int baseDamage;
	private float aimAngle;
	private bool releaseSent;

	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => ModContent.GetInstance<ArmOfGiantTree>().Texture;

	public override void SetDefaults()
	{
		Projectile.width = Projectile.height = 24;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 2;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.netImportant = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		selectedSlot = Main.player[Projectile.owner].selectedItem;
		baseDamage = Projectile.damage;
		aimAngle = Projectile.velocity.ToRotation();
		attack.Begin(SyncedPhase == AttackPhase.NormalSwing);
		WriteState();
	}

	public override bool ShouldUpdatePosition() => false;

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write((short)selectedSlot);
		writer.Write(aimAngle);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		int slot = reader.ReadInt16();
		float angle = reader.ReadSingle();
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			return;
		}
		selectedSlot = slot;
		if (float.IsFinite(angle))
		{
			aimAngle = angle;
		}
		attack.Restore(SyncedPhase, SyncedCharge, SyncedTimer);
	}

	public void Release(float angle)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || !float.IsFinite(angle) || !attack.Release())
		{
			return;
		}
		aimAngle = angle;
		WriteState();
		Projectile.netUpdate = true;
	}

	private void WriteState()
	{
		SyncedPhase = attack.Phase;
		SyncedCharge = attack.ChargeFrames;
		SyncedTimer = attack.Timer;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		bool valid = player.active && !player.dead && !player.noItems && !player.CCed
			&& player.selectedItem == selectedSlot && player.HeldItem.type == ModContent.ItemType<ArmOfGiantTree>();
		if (!valid || attack.Phase == AttackPhase.Idle)
		{
			Projectile.Kill();
			return;
		}

		if (Projectile.owner == Main.myPlayer)
		{
			player.GetModPlayer<KelpCurtainPlayer>().ArmOfGiantTreeRequestWait = 0;
			if (attack.Phase == AttackPhase.Charging)
			{
				aimAngle = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX * player.direction).ToRotation();
				if (!player.controlUseItem && !releaseSent)
				{
					releaseSent = true;
					ArmOfGiantTreeChargePacket.Request(player, true, false, aimAngle, Projectile.identity);
				}
			}
		}

		player.ChangeDir(MathF.Cos(aimAngle) >= 0 ? 1 : -1);
		player.heldProj = Projectile.whoAmI;
		player.itemTime = player.itemAnimation = 2;
		float progress;
		float sweep;
		switch (attack.Phase)
		{
			case AttackPhase.Charging:
				sweep = -1.7f - 0.15f * attack.ChargeFrames / ChargeDuration;
				break;
			case AttackPhase.Smashing:
				progress = MathHelper.Clamp(attack.Timer / (float)ImpactFrame, 0, 1);
				sweep = MathHelper.Lerp(-1.85f, 1.2f, progress * progress);
				break;
			case AttackPhase.Recovering:
				sweep = MathHelper.Lerp(1.2f, -0.5f, attack.Timer / (float)RecoveryDuration);
				break;
			default:
				sweep = MathHelper.Lerp(-1.5f, 1.3f, attack.Timer / (float)NormalDuration);
				break;
		}
		Projectile.rotation = aimAngle + sweep * player.direction;
		Projectile.Center = player.MountedCenter + Projectile.rotation.ToRotationVector2() * 44;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
		if (attack.ConsumeShockwave())
		{
			if (!Main.dedServ)
			{
				SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
				for (int i = 0; i < 32; i++)
				{
					Vector2 direction = (MathHelper.TwoPi * i / 32).ToRotationVector2();
					Dust.NewDustPerfect(player.Center + direction * 200, DustID.Stone, direction * 2);
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				foreach (NPC npc in Main.npc)
				{
					if (!npc.CanBeChasedBy(Projectile) || Vector2.Distance(npc.Center, player.Center) > 200 + npc.width * 0.5f)
					{
						continue;
					}
					npc.PlayerInteraction(player.whoAmI);
					npc.SimpleStrikeNPC(baseDamage, npc.Center.X >= player.Center.X ? 1 : -1, false, Projectile.knockBack, DamageClass.Melee);
				}
			}
		}
		AttackPhase previous = attack.Phase;
		attack.Tick(true);
		WriteState();
		if (Main.netMode != NetmodeID.MultiplayerClient && (previous != attack.Phase || (attack.Phase == AttackPhase.Charging && attack.ChargeFrames % 15 == 0)))
		{
			Projectile.netUpdate = true;
		}
		Projectile.timeLeft = 2;
	}

	public override bool? CanDamage() => attack.Phase == AttackPhase.NormalSwing
		|| (attack.Phase == AttackPhase.Smashing && attack.Timer >= 10 && attack.Timer <= ImpactFrame + 5) ? null : false;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 start = Main.player[Projectile.owner].MountedCenter;
		Vector2 end = start + Projectile.rotation.ToRotationVector2() * 84;
		float point = 0;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 24, ref point);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= attack.DamageMultiplier;

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Vector2 anchor = Main.player[Projectile.owner].MountedCenter;
		Main.EntitySpriteDraw(texture, anchor - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4,
			new Vector2(0, texture.Height), Projectile.scale, SpriteEffects.None);
		return false;
	}
}
