using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class DarkFanFly : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 450;
		Projectile.extraUpdates = 1;
		Projectile.tileCollide = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.scale = 1.5f;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<FanHit>(), 0, 0, player.whoAmI, 15, Main.rand.NextFloat(6.283f));
		int[] array = Projectile.localNPCImmunity;
		bool flag = (!Projectile.usesLocalNPCImmunity && !Projectile.usesIDStaticNPCImmunity) || (Projectile.usesLocalNPCImmunity && array[target.whoAmI] == 0) || (Projectile.usesIDStaticNPCImmunity && Projectile.IsNPCIndexImmuneToProjectileType(Projectile.type, target.whoAmI));
		if (target.active && !target.dontTakeDamage && flag && (target.aiStyle != NPCAIStyleID.Fairy || target.ai[2] <= 1f))
		{
			if (target.active)
			{
				Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			}
		}
		target.AddBuff(ModContent.BuffType<OnMoth>(), 300);

		if (MothBuffTarget.mothStack[target.whoAmI] < 5)
		{
			MothBuffTarget.mothStack[target.whoAmI] += 1;
		}
		else
		{
			MothBuffTarget.mothStack[target.whoAmI] = 5;
		}
		target.AddBuff(ModContent.BuffType<FireflyInferno>(), 120);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 v0 = player.Center - Projectile.Center;
		Vector2 v1 = Vector2.Normalize(v0) * 120f;
		Vector2 v2 = player.Center + v1 - Projectile.Center;
		Vector2 v3 = Vector2.Normalize(v2) * 0.48f;
		Projectile.velocity += v3;
		if (Projectile.timeLeft is < 380 and > 60)
		{
			if (v0.Length() < 48)
			{
				Projectile.timeLeft = 20;
			}
		}
		Projectile.velocity *= 0.99f;
		for (int x = 58; x >= 0; x--)
		{
			oldVelocity[x + 1] = oldVelocity[x];
		}
		oldVelocity[0] = Projectile.velocity;

		for (int x = 58; x >= 0; x--)
		{
			oldScale[x + 1] = oldScale[x];
		}
		oldScale[0] = Projectile.scale;
		if (Projectile.timeLeft < 300)
		{
			Projectile.tileCollide = false;
		}

		if (Projectile.timeLeft < 60)
		{
			Projectile.timeLeft -= 4;
		}

		int frequency = 70 / (2 + player.maxMinions);
		if (Projectile.timeLeft % frequency == 0)
		{
			Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity * 0.3f, ModContent.ProjectileType<GlowingButterfly>(), Projectile.damage / 3, Projectile.knockBack, player.whoAmI, Main.rand.Next(2), 0f);
		}

		for (int g = 0; g < 1; g++)
		{
			float velRot = Projectile.rotation;
			float timeRot = Projectile.timeLeft / 10f;
			Vector2 v4 = posRot0.RotatedBy(timeRot + Main.rand.NextFloat(4f));
			Vector2 v5 = new Vector2(v4.X, v4.Y / 2f).RotatedBy(velRot);
			Vector2 v6 = new Vector2(v4.X / 2f, v4.Y).RotatedBy(velRot + Math.PI / -2);
			Vector2 newVelocity = Projectile.velocity * 0.3f + v6 / 15f;
			var smog = new MothShimmerScaleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + v5,
				Coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
				MaxTime = Main.rand.Next(20, 85),
				Scale = Main.rand.NextFloat(0.4f, 3.4f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-30f, 30f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void OnKill(int timeLeft)
	{
	}

	private Vector2 posRot0 = new Vector2(-16, -100);
	private Vector2 posRot1 = new Vector2(100, 0);
	private Vector2 posRot2 = new Vector2(-100, 100);

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	private Vector2[] oldVelocity = new Vector2[60];
	private float[] oldScale = new float[60];

	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float velRot = Projectile.rotation;
		float timeRot = Projectile.timeLeft / 10f;
		var effect = new List<Vertex2D>();
		for (int x = 0; x < 30; x++)
		{
			Vector2 v4 = posRot0.RotatedBy(timeRot + x / 6d);
			v4 = new Vector2(v4.X, v4.Y / 2f).RotatedBy(velRot);
			float value = 29 - x;
			value /= 29f;
			if (x < 5)
			{
				value = x / 5f;
			}
			float coordValue = (float)Main.time * 0.02f;
			effect.Add(new Vertex2D(Projectile.Center + v4 * Projectile.scale * 0.5f - Main.screenPosition, new Color(0, 100, 255, 0) * value, new Vector3(x / 50f + coordValue, 0, 0)));
			effect.Add(new Vertex2D(Projectile.Center - Main.screenPosition, new Color(0, 100, 255, 0) * 0.5f * value, new Vector3(x / 50f + coordValue, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_forceField_sparse.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, effect.ToArray(), 0, effect.Count - 2);

		effect = new List<Vertex2D>();
		for (int x = 0; x < 30; x++)
		{
			Vector2 v4 = posRot0.RotatedBy(timeRot + x / 6d);
			v4 = new Vector2(v4.X, v4.Y / 2f).RotatedBy(velRot);
			float value = 29 - x;
			value /= 29f;
			if (x < 5)
			{
				value = x / 5f;
			}
			float coordValue = (float)Main.time * 0.02f;
			effect.Add(new Vertex2D(Projectile.Center + v4 * Projectile.scale * 0.5f - Main.screenPosition, Color.White * value, new Vector3(x / 50f + coordValue, 0.5f, 0)));
			effect.Add(new Vertex2D(Projectile.Center - Main.screenPosition, Color.White * 0.5f * value, new Vector3(x / 50f + coordValue, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, effect.ToArray(), 0, effect.Count - 2);

		Texture2D tex = ModAsset.DarkFanFly.Value;
		Texture2D texG = ModAsset.DarkFanFlyGlow.Value;
		var vertex = new List<Vertex2D>();

		Vector2 v0 = posRot0.RotatedBy(timeRot);
		v0 = new Vector2(v0.X, v0.Y / 2f).RotatedBy(velRot);
		Vector2 v1 = posRot1.RotatedBy(timeRot);
		v1 = new Vector2(v1.X, v1.Y / 2f).RotatedBy(velRot);
		Vector2 v2 = posRot2.RotatedBy(timeRot);
		v2 = new Vector2(v2.X, v2.Y / 2f).RotatedBy(velRot);
		vertex.Add(new Vertex2D(Projectile.Center + v0 * Projectile.scale - Main.screenPosition, lightColor, new Vector3(84f / 200f, 0f / 200f, 0)));
		vertex.Add(new Vertex2D(Projectile.Center + v1 * Projectile.scale - Main.screenPosition, lightColor, new Vector3(200f / 200f, 100f / 200f, 0)));
		vertex.Add(new Vertex2D(Projectile.Center + v2 * Projectile.scale - Main.screenPosition, lightColor, new Vector3(0f / 200f, 200f / 200f, 0)));
		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex.ToArray(), 0, vertex.Count / 3);

		vertex.Add(new Vertex2D(Projectile.Center + v0 * Projectile.scale - Main.screenPosition, new Color(0, 100, 255, 0), new Vector3(84f / 200f, 0f / 200f, 0)));
		vertex.Add(new Vertex2D(Projectile.Center + v1 * Projectile.scale - Main.screenPosition, new Color(0, 100, 255, 0), new Vector3(200f / 200f, 100f / 200f, 0)));
		vertex.Add(new Vertex2D(Projectile.Center + v2 * Projectile.scale - Main.screenPosition, new Color(0, 100, 255, 0), new Vector3(0f / 200f, 200f / 200f, 0)));
		Main.graphics.GraphicsDevice.Textures[0] = texG;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex.ToArray(), 0, vertex.Count / 3);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.98f;
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		return false;
	}
}
