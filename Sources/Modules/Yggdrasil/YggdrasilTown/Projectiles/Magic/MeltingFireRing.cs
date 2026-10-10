using Everglow.Commons.VFX.CommonDusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class MeltingFireRing : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => "Terraria/Images/Projectile_0";

	private float dissolve = 0;

	public override void SetDefaults()
	{
		Projectile.width = 1;
		Projectile.height = 1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 600;
		Projectile.tileCollide = false;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.6f, 0.1f) * dissolve);
		foreach (NPC npc in Main.ActiveNPCs)// 点燃
		{
			if (Vector2.Distance(npc.Center, Projectile.Center) < 200 && !npc.friendly)
			{
				npc.AddBuff(BuffID.OnFire, 5);
			}
		}
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			Projectile.Kill();
		}

		Projectile.Center = player.Center;
		if (player.HeldItem.type != ModContent.ItemType<SmeltingStaff>())
		{
			Projectile.ai[0] = 1;
		}

		if (Projectile.ai[0] == 1)// 消失
		{
			dissolve = MathHelper.Lerp(dissolve, 0f, 0.15f);
			if (dissolve < 0.05)
			{
				Projectile.Kill();
			}
		}
		else
		{
			Projectile.timeLeft++;
			Projectile.velocity = Vector2.Zero;

			dissolve = MathHelper.Lerp(dissolve, 0.85f, 0.1f);
		}
		if (dissolve > 0.5f)
		{
			if (Main.rand.NextBool(3))
			{
				Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200) * dissolve;
				var dust = new LightDust();
				dust.scale = Main.rand.NextFloat(0.6f, 0.9f) * 0.8f;
				dust.position = pos - Projectile.Center;
				dust.Owner = player;
				dust.color = new Color(1f, 0.6f, 0.3f, 0f);
				dust.timeleft = dust.maxTimeleft = Main.rand.Next(75, 100);
				if (Main.rand.NextBool())
				{
					dust.velocity = Vector2.Normalize(dust.position.RotatedBy(1.57f)) * Main.rand.NextFloat(4f, 10f);
					dust.aiStyle = LightDust.AIStyle.Rotation;
				}
				else
				{
					dust.velocity = Vector2.Normalize(dust.position.RotatedBy(-1.57f)) * Main.rand.NextFloat(4f, 10f);
					dust.aiStyle = LightDust.AIStyle.Rotation2;
				}
				Ins.VFXManager.Add(dust);
			}
			/*
			if (Main.rand.NextBool(1))
			{
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(200, 200);
                LightDust dust = new LightDust();
                dust.scale = Main.rand.NextFloat(0.5f, 1.5f);
                dust.position = pos - Projectile.Center;
                dust.Owner = Projectile;
                dust.color = new Color(1f, 0.5f, 0.3f, 0f);
                dust.velocity = -Vector2.UnitY * Main.rand.NextFloat(1f, 2.5f);
                dust.timeleft = dust.maxTimeleft = Main.rand.Next(30, 40);
                Ins.VFXManager.Add(dust);
            }*/
		}
		base.AI();
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Effect shader = Commons.ModAsset.Dissolve0.Value;
		Texture2D tex1 = Commons.ModAsset.Trail_5.Value;
		Texture2D noise = Commons.ModAsset.Noise_cell.Value;

		var drawColor = new Color(1f, 0.4f, 0.2f, 1f);

		// drawColor = new Color(0.5f, 1f, 0.2f, 0f);
		List<Vertex2D> vertices1 = new();
		List<Vertex2D> vertices2 = new();

		float radius = dissolve * 20 + 200;
		float t = (float)Main.time * 0.015f;

		int verticeCounts = 60;
		for (int i = 0; i <= verticeCounts; i++)
		{
			float offset1 = (float)Math.Sin(i * MathHelper.TwoPi / verticeCounts * 7f + t * 3) * 3;
			vertices1.Add(Projectile.Center - Main.screenPosition + (i * MathHelper.TwoPi / verticeCounts).ToRotationVector2() * (radius + offset1), drawColor, new Vector3(4 * i / 120f + t, 0, 1));
			vertices1.Add(Projectile.Center - Main.screenPosition + (i * MathHelper.TwoPi / verticeCounts).ToRotationVector2() * (radius - 35 + offset1), drawColor, new Vector3(4 * i / 120f + t, 1, 1));

			offset1 = (float)Math.Cos(i * MathHelper.TwoPi / verticeCounts * 5f + t * 5f) * 6;
			vertices2.Add(Projectile.Center - Main.screenPosition + (i * MathHelper.TwoPi / verticeCounts + 3.1f).ToRotationVector2() * (radius + offset1), drawColor * 1.5f, new Vector3(-6 * i / 120f + t, 0, 1));
			vertices2.Add(Projectile.Center - Main.screenPosition + (i * MathHelper.TwoPi / verticeCounts + 3.14f).ToRotationVector2() * (radius - 40 + offset1), drawColor * 1.5f, new Vector3(-6 * i / 120f + t, 1, 1));
		}

		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, /* CustomBlendStates.SoftAdditive*/BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.graphics.graphicsDevice.Textures[0] = tex1;
		Main.graphics.graphicsDevice.Textures[1] = noise;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

		shader.Parameters["_DissolveFactor"].SetValue(1 - dissolve);
		shader.CurrentTechnique.Passes[0].Apply();

		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices1.ToArray(), 0, vertices1.Count - 2);
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices1.ToArray(), 0, vertices1.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}
}
