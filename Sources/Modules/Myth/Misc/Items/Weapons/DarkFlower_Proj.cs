using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Mechanics.EliminateLight;
using Everglow.Commons.VFX.CommonVFXDusts;
using ReLogic.Content;

namespace Everglow.Myth.Misc.Items.Weapons;

public class DarkFlower_Proj : ModProjectile
{
	public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Fireball;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] += 500;
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.timeLeft = 800;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.penetrate = 2;
		Projectile.scale = 1.5f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
		oldPos = new Vector2[25];
		Projectile.DamageType = DamageClass.Magic;
	}

	private Player Player => Main.player[Projectile.owner];

	private Vector2[] oldPos;

	private void Pre_Kill()
	{
		if (Projectile.timeLeft > 10)
		{
			Projectile.timeLeft = 10;
			Projectile.friendly = false;
			FogVFX fog = MEACVFX.Create<FogVFX>(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextVector2Circular(2, 2) + Projectile.velocity * 0.1f, 0);
			fog.Subtract = true;
			fog.DrawColor = new Color(0.3f, 0.6f, 0.3f, 1f);
			fog.SetTimeLeft(Main.rand.Next(50, 80));
			fog.Scale = 0.7f * Projectile.scale * Main.rand.NextFloat(1f, 2f);
			fog.Ai0 = 1;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Pre_Kill();
		for (int i = 0; i < 10; i++)
		{
			Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle, 0, 0, 0, default, 2);
			d.velocity += Main.rand.NextVector2Circular(5, 5);
			d.noGravity = true;
			d.noLight = Main.rand.NextBool();
		}
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		fallThrough = false;
		return true;
	}

	public override void OnKill(int timeLeft)
	{
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y * 0.7f;
			Projectile.velocity.X = Projectile.velocity.X * 0.9f;
		}
		else if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X * 0.8f;
		}
		if (++Projectile.ai[1] > 12)
		{
			Pre_Kill();
		}

		return false;
	}

	private NPC npcTarget = null;
	private float alpha = 1f;

	public override void AI()
	{
		if (Projectile.timeLeft < 10)
		{
			alpha -= 0.1f;
			Projectile.velocity *= 0.6f;
		}
		Projectile.rotation += 0.2f * (Projectile.velocity.X > 0 ? 1 : -1);

		if (Main.rand.NextBool(2))
		{
			Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WaterCandle, 0, 0, 0, default, 1.2f);
			d.noLight = true;
			d.velocity = Projectile.velocity * 0.5f;
		}
		if (Projectile.timeLeft % 5 == 0 && Projectile.ai[0] == 0)
		{
			float maxDis = 200;
			NPC target = null;
			foreach (NPC npc in Main.npc)
			{
				float dis = Vector2.Distance(npc.Center, Projectile.Center);
				if (dis < maxDis && npc.CanBeChasedBy(null, true))
				{
					maxDis = dis;
					target = npc;
				}
			}
			if (target != null)
			{
				Projectile.ai[0] = 1;
				npcTarget = target;
			}
		}
		if (Projectile.ai[0] == 1)
		{
			if (npcTarget != null && npcTarget.CanBeChasedBy())
			{
				Projectile.tileCollide = false;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npcTarget.Center) * 20, 0.06f);
			}
		}
		else
		{
			Projectile.velocity.Y += 0.2f;
		}
		ProjectileUtils.TrackOldValue(oldPos, Projectile.Center);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Color c = new Color(1f, 0.5f, 0.2f) * alpha;
		List<Vertex2D> bars = new List<Vertex2D>
			{
				new Vertex2D(Projectile.Center - Main.screenPosition, Color.Red, new Vector3(0, 0.5f, 1.3f)),
			};
		float counts = oldPos.Length;

		for (int i = 0; i < oldPos.Length - 1; ++i)
		{
			if (oldPos[i + 1] == Vector2.Zero)
			{
				break;
			}
			float t = Projectile.timeLeft * 0.04f;
			var normalDir = oldPos[i] - oldPos[i + 1];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)counts;
			var w = MathHelper.Lerp(1f, 0.1f, factor);
			float width = MathHelper.Lerp(20, 0, factor);
			bars.Add(new Vertex2D(oldPos[i] - Main.screenPosition + normalDir * width, c * w, new Vector3((float)Math.Sqrt(factor) + t, 1, w)));
			bars.Add(new Vertex2D(oldPos[i] - Main.screenPosition + normalDir * -width, c * w, new Vector3((float)Math.Sqrt(factor) + t, 0, w)));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Subtract, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

		if (bars.Count >= 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Terraria/Images/Extra_189", AssetRequestMode.ImmediateLoad).Value;
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Subtract, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

		DrawOnCenter(tex, Projectile.Center - Main.screenPosition, new Color(0.5f, 1f, 0.8f) * alpha, Projectile.rotation, Projectile.scale);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		EliminateLightManager.AddCircle(Projectile.Center, (12 - Projectile.ai[1]) * 10f);
		return false;
	}

	public void DrawOnCenter(Texture2D tex, Vector2 pos, Color color, float rotation, float scale)
	{
		Main.spriteBatch.Draw(tex, pos, null, color, rotation, tex.Size() / 2, scale, 0, 0);
	}
}
