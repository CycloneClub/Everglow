using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Attack_Proj_Ball : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 600;
		Projectile.penetrate = -1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
	}

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.1f, 0.2f) * Projectile.scale * 5f);
		var playerWhoAmI = Player.FindClosest(Projectile.Center, 0, 0);
		if (playerWhoAmI < 0)
		{
			return;
		}
		Player player = Main.player[playerWhoAmI];
		Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
		toPlayer = toPlayer.NormalizeSafe() * 6;
		Projectile.velocity = toPlayer * 0.05f + Projectile.velocity * 0.95f;
		Projectile.velocity = Projectile.velocity.NormalizeSafe() * 6f;
		if (Projectile.timeLeft % 6 == 0)
		{
			Projectile.frame++;
			if (Projectile.frame >= 10)
			{
				Projectile.frame = 0;
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		if (Main.rand.NextBool())
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Ball_Small_Group>(), 36, 2.5f, Main.myPlayer);
		}
		else
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Ball_Small_Group_1>(), 36, 2.5f, Main.myPlayer);
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		VampireMat.OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Reverse, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		float drawScale = Projectile.scale * (MathF.Sin((float)Main.time * 0.15f + Projectile.whoAmI) * 0.25f + 1);
		Rectangle frame = new Rectangle(0, 90 * Projectile.frame, 90, 90);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 1), 0, frame.Size() * 0.5f, drawScale, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
