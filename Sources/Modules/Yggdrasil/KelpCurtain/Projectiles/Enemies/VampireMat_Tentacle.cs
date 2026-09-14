using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;
using Microsoft.CodeAnalysis;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Tentacle : ModProjectile
{
	public NPC ParentVampireMat = null;

	public int Timer;

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.hide = true;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (ParentVampireMat is null)
		{
			var npc = NPCUtils.FindNearest(Projectile.Center, ModContent.NPCType<VampireMat>());
			if (npc is not null)
			{
				ParentVampireMat = npc;
			}
			else
			{
				Projectile.active = false;
				return;
			}
		}
	}

	public override void AI()
	{
		if (ParentVampireMat is null || !ParentVampireMat.active)
		{
			Projectile.active = false;
			return;
		}
		Projectile.Center = ParentVampireMat.Center;
		Timer++;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float tentacle_dis = MathF.Sin(Timer / 180f * MathHelper.Pi) * 900;
		return CollisionUtils.Intersect(targetHitbox.Top(), targetHitbox.Bottom(), targetHitbox.Width, Projectile.Center, Projectile.Center + new Vector2(tentacle_dis, 0).RotatedBy(Projectile.rotation), 110);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		VampireMat.OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (ParentVampireMat is null || !ParentVampireMat.active || ParentVampireMat.type != ModContent.NPCType<VampireMat>())
		{
			return false;
		}
		VampireMat vampireMat = ParentVampireMat.ModNPC as VampireMat;
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		float tentacle_dis = MathF.Sin(Timer / 180f * MathHelper.Pi) * 900;
		List<Vector2> tentacleCurve = new List<Vector2>();
		for (int i = 0; i <= tentacle_dis; i += 10)
		{
			float wave = 0;
			if (Timer > 60)
			{
				float decay = i / 600f;
				if (decay > 1)
				{
					decay = 1;
				}
				wave = MathF.Sin((Timer - 60) / 120f * MathHelper.Pi) * MathF.Sin(i / 30f + Projectile.whoAmI + (float)Main.time * 0.06f) * 30 * decay;
			}
			tentacleCurve.Add(Projectile.Center + new Vector2(tentacle_dis - i, wave).RotatedBy(Projectile.rotation) - Main.screenPosition);
		}
		List<Vertex2D> bars = SpriteBatchUtils.DrawCurveStrip_EnvironmentLight(tentacleCurve, 110, 1, (float)(texture.Width - tentacle_dis) / texture.Width);
		if (bars.Count > 2)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);

			if (vampireMat is not null && vampireMat.HitTimer > 0)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Effect effect = ModAsset.VampireMat_HitEffect.Value;
				effect.Parameters["hitTimer"].SetValue(vampireMat.HitTimer);
				effect.CurrentTechnique.Passes[0].Apply();

				Main.graphics.GraphicsDevice.Textures[0] = texture;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}
		return false;
	}
}
