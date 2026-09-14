using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;

public class VampireMat_Tentacle_FromBackground : ModProjectile
{
	public NPC ParentVampireMat = null;

	public int Timer = 0;

	public int Duration = 0;

	public override string Texture => ModAsset.VampireMat_Tentacle_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.timeLeft = 330;
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
		Timer++;
		if (Timer < 10 || Timer > 90)
		{
			Duration++;
		}
		else
		{
			int playerIndex = Player.FindClosest(Projectile.Center, 1, 1);
			if (playerIndex >= 0)
			{
				Player target = Main.player[playerIndex];
				Projectile.rotation = (target.Center - Projectile.Center).ToRotationSafe();
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Duration < 12 || Duration > 170)
		{
			return false;
		}
		float tentacle_dis = MathF.Sin(Duration / 180f * MathHelper.Pi) * 900;
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
		float tentacle_dis = MathF.Sin(Duration / 180f * MathHelper.Pi) * 900;
		List<Vector2> tentacleCurve = new List<Vector2>();
		for (int i = 0; i <= tentacle_dis; i += 10)
		{
			float wave = 0;
			if (Duration > 60)
			{
				float decay = i / 600f;
				if (decay > 1)
				{
					decay = 1;
				}
				wave = MathF.Sin((Duration - 60) / 120f * MathHelper.Pi) * MathF.Sin(i / 30f + Projectile.whoAmI + (float)Main.time * 0.06f) * 30 * decay;
			}
			tentacleCurve.Add(Projectile.Center + new Vector2(tentacle_dis - i, wave).RotatedBy(Projectile.rotation) - Main.screenPosition);
		}
		List<Vertex2D> bars = DrawCurveStrip_EnvironmentLight_ForTentacles(tentacleCurve, 110, 1, (float)(texture.Width - tentacle_dis) / texture.Width);
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

	private List<Vertex2D> DrawCurveStrip_EnvironmentLight_ForTentacles(List<Vector2> curve, float width, float coord_x_min, float coord_x_max, float coord_y_min = 0, float coord_y_max = 1, bool curveHasScreenPos = false)
	{
		if (curve.Count < 2)
		{
			return [];
		}
		Vector2 lightSamplingOffset = Vector2.zeroVector;
		if (!curveHasScreenPos)
		{
			lightSamplingOffset = Main.screenPosition;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < curve.Count; i++)
		{
			Vector2 pos = curve[i];
			Vector2 dir;
			if (i == 0)
			{
				dir = curve[i + 1] - curve[i];
			}
			else
			{
				dir = curve[i] - curve[i - 1];
			}
			dir = dir.NormalizeSafe();
			Vector2 normal = new Vector2(dir.Y, -dir.X) * width / 2f;
			float value = i / (float)(curve.Count - 1);
			float coordX = float.Lerp(coord_x_min, coord_x_max, value);
			int rev_i = curve.Count - i;
			if (rev_i < 5)
			{
				SpriteBatchUtils.AddVertexWithEnv_Light(bars, pos + normal + lightSamplingOffset, new Vector3(coordX, coord_y_min, 0), true, rev_i / 5f);
				SpriteBatchUtils.AddVertexWithEnv_Light(bars, pos - normal + lightSamplingOffset, new Vector3(coordX, coord_y_max, 0), true, rev_i / 5f);
			}
			else
			{
				SpriteBatchUtils.AddVertexWithEnv_Light(bars, pos + normal + lightSamplingOffset, new Vector3(coordX, coord_y_min, 0));
				SpriteBatchUtils.AddVertexWithEnv_Light(bars, pos - normal + lightSamplingOffset, new Vector3(coordX, coord_y_max, 0));
			}
		}
		return bars;
	}
}
