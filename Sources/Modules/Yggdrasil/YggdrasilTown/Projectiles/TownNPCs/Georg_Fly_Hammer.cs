using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Fly_Hammer : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public float Omega = 0;

	public int ExtraSpeed;

	public bool FireEnchanted;

	public float SpriteRotation => Projectile.rotation * Projectile.spriteDirection;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 100;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		if (Projectile.ai[0] is >= 0 and < 200)
		{
			Owner = Main.npc[(int)Projectile.ai[0]];
		}
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
		}
		var tNLIY = Owner.ModNPC as TownNPC_LiveInYggdrasil;
		if (tNLIY == null)
		{
			return;
		}
		var iKeeper = Owner.ModNPC as InnKeeper;
		if (iKeeper == null)
		{
			return;
		}
		ExtraSpeed = 0;
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (tNLIY.ArenaFighting)
			{
				ExtraSpeed = (int)tNLIY.AttackSpeed;
			}
		}
		FireEnchanted = iKeeper.BurningHammer;
	}

	public override bool ShouldUpdatePosition() => true;

	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
			return;
		}
		Timer++;
		if (Owner.target < 0)
		{
			Projectile.active = false;
			return;
		}
		Projectile.rotation += Omega;
		Player player = Main.player[Owner.target];
		if (Timer < 30)
		{
			Omega += 0.01333f;
			Projectile.spriteDirection = Owner.spriteDirection;
			Projectile.Center = Owner.Center + new Vector2(-Owner.spriteDirection * 10, -10) + new Vector2(0, -24).RotatedBy(SpriteRotation);
		}
		if (Timer == 30)
		{
			Vector2 toTarget = (player.Center - Projectile.Center).NormalizeSafe();
			Projectile.velocity = toTarget * (26 + ExtraSpeed * 6);
		}
		if (Timer is >= 40 and < 60)
		{
			Projectile.velocity *= 0.95f;
		}
		if (Timer is >= 60 and < 90)
		{
			Vector2 origToOwner = Owner.Center + Owner.velocity - Projectile.Center - Projectile.velocity;
			if ((Owner.Center - Projectile.Center).Length() < 60)
			{
				Projectile.active = false;
			}
			Vector2 toOwner = origToOwner.NormalizeSafe() * (30 + ExtraSpeed * 6);
			Projectile.velocity = 0.9f * Projectile.velocity + 0.1f * toOwner;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = ModAsset.Georg_Fly_Hammer.Value;
		var drawColor = lightColor;
		var frame = new Rectangle(0, 0, 42, 62);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, drawColor, SpriteRotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public void DrawTrail()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS.SortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public float TrailWidthFunction(float factor)
	{
		factor *= 6;
		factor -= 1;
		if (factor < 0)
		{
			return MathF.Pow(MathF.Cos(factor * MathHelper.PiOver2), 0.5f);
		}
		return MathF.Pow(MathF.Cos(factor / 5f * MathHelper.PiOver2), 3);
	}
}
