using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Accessory;

public class IchorRing : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.timeLeft = 780;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		// Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, ModContent.ProjectileType<GoldenShowerBomb>(), 0, 0, Projectile.owner, 10f, Main.rand.NextFloat(6.283f));
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = player.Center;
		if (Projectile.ai[0] <= 180)
		{
			Projectile.ai[0] = 180 * 0.06f + Projectile.ai[0] * 0.94f;
		}

		for (int x = 0; x < 5; x++)
		{
			GenerateDust();
		}

		if (Projectile.timeLeft < 120)
		{
			Energy -= 5f;
		}
		else
		{
			if (Energy < 600)
			{
				Energy += 15;
			}
		}
		if (Projectile.timeLeft < 60)
		{
			Projectile.friendly = false;
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (Projectile.timeLeft > 690)
		{
			modifiers.FinalDamage *= 5;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int x = 0; x < 2; x++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283) - Projectile.velocity * 0.2f;
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center + velocity * -2, velocity, ModContent.ProjectileType<IchorCurrent>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 3f/*If ai[0] equal to 3, another ai will be execute*/);
			p.friendly = false;
			p.CritChance = Projectile.CritChance;
		}
		target.AddBuff(BuffID.Ichor, 600);
	}

	private bool InsertWithRing(Vector2 point1, Vector2 point2, float radius, float toleranceWidth)
	{
		return Math.Abs((point1 - point2).Length() - radius) < toleranceWidth;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (InsertWithRing(targetHitbox.BottomLeft(), Projectile.Center, Projectile.ai[0], 20))
		{
			return true;
		}

		if (InsertWithRing(targetHitbox.BottomRight(), Projectile.Center, Projectile.ai[0], 20))
		{
			return true;
		}

		if (InsertWithRing(targetHitbox.TopLeft(), Projectile.Center, Projectile.ai[0], 20))
		{
			return true;
		}

		return InsertWithRing(targetHitbox.TopRight(), Projectile.Center, Projectile.ai[0], 20) ? true : false;
	}

	public float Energy = 0;

	public void DrawPowerEffect()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 bulbPos = player.Center;
		float energyValue = Energy / 600f;
		energyValue = MathF.Pow(energyValue, 0.3f);
		energyValue *= 12f / 5f;
		Color c0 = new Color(1, 0.7f, 0, 0);
		float timeValue = (float)Main.time * 0.009f;
		List<Vertex2D> bars = new List<Vertex2D>();
		float accuracy = 16;
		List<Vertex2D> bars2 = new List<Vertex2D>();
		List<Vertex2D> bars3 = new List<Vertex2D>();
		List<Vertex2D> bars4 = new List<Vertex2D>();
		for (int x = 0; x < 9; x++)
		{
			Vector2 addPos = Vector2.zeroVector;
			Vector2 addVel = new Vector2(0, 4 * energyValue).RotatedBy(x / 9f * MathHelper.TwoPi - timeValue * 0.4f);
			for (int t = 0; t <= accuracy; t++)
			{
				float factor = t / accuracy;
				Vector2 velLeft = Vector2.Normalize(addVel.RotatedBy(MathHelper.PiOver2)) * 134 * energyValue;
				if (t == 0)
				{
					bars2.Add(new Vertex2D(bulbPos + addPos, Color.White * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
					bars2.Add(new Vertex2D(bulbPos + addPos - velLeft, Color.White * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
				}
				bars2.Add(new Vertex2D(bulbPos + addPos, Color.White * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
				bars2.Add(new Vertex2D(bulbPos + addPos - velLeft, Color.White * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));

				if (t == 0)
				{
					bars.Add(new Vertex2D(bulbPos + addPos, c0 * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
					bars.Add(new Vertex2D(bulbPos + addPos - velLeft, c0 * 0, new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
				}
				bars.Add(new Vertex2D(bulbPos + addPos, c0 * (1 - factor) * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 0.5f, factor)));
				bars.Add(new Vertex2D(bulbPos + addPos - velLeft, c0 * (1 - factor) * (1 - factor), new Vector3(timeValue + factor * factor + MathF.Sin(x), 1, factor)));
				addPos += addVel;
				addVel = addVel.RotatedBy((1 - factor * factor * 1.2f) * 0.36f) * 1.08f;
			}
		}
		timeValue = (float)Main.time * 0.09f;
		for (int x = 0; x < 45; x++)
		{
			float factor = x / 44f;
			Vector2 addPos = new Vector2(90 * energyValue, 0).RotatedBy(x / 44f * MathHelper.TwoPi);
			float mulC = 1f;
			if (x < 10)
			{
				mulC = x / 10f;
			}
			if (x >= 34)
			{
				mulC = (44 - x) / 10f;
			}
			bars3.Add(new Vertex2D(bulbPos + addPos, c0 * mulC, new Vector3(timeValue + factor * 5.6f, 0, 1)));
			bars3.Add(new Vertex2D(bulbPos + addPos - Vector2.Normalize(addPos) * Math.Min(addPos.Length(), 40f), c0 * 0, new Vector3(timeValue + factor * 8f, 1, 1)));
		}
		for (int x = 0; x < 45; x++)
		{
			float factor = x / 44f;
			Vector2 addPos = new Vector2(90 * energyValue, 0).RotatedBy(x / 44f * MathHelper.TwoPi + MathHelper.Pi);
			float mulC = 1f;
			if (x < 10)
			{
				mulC = x / 10f;
			}
			if (x >= 34)
			{
				mulC = (44 - x) / 10f;
			}
			bars3.Add(new Vertex2D(bulbPos + addPos, c0 * mulC, new Vector3(timeValue + factor * 5.6f, 0, 1)));
			bars3.Add(new Vertex2D(bulbPos + addPos - Vector2.Normalize(addPos) * Math.Min(addPos.Length(), 40f), c0 * 0, new Vector3(timeValue + factor * 8f, 1, 1)));
		}

		for (int x = 0; x < 45; x++)
		{
			float factor = x / 44f;
			Vector2 addPos = new Vector2(90 * energyValue, 0).RotatedBy(x / 44f * MathHelper.TwoPi);
			float mulC = 1f;
			if (x < 10)
			{
				mulC = x / 10f;
			}
			if (x >= 34)
			{
				mulC = (44 - x) / 10f;
			}
			bars4.Add(new Vertex2D(bulbPos + addPos, Color.White * mulC, new Vector3(timeValue + factor * 5.6f, 0, 1)));
			bars4.Add(new Vertex2D(bulbPos + addPos - Vector2.Normalize(addPos) * Math.Min(addPos.Length(), 40f), Color.White * 0, new Vector3(timeValue + factor * 8f, 1, 1)));
		}
		for (int x = 0; x < 45; x++)
		{
			float factor = x / 44f;
			Vector2 addPos = new Vector2(90 * energyValue, 0).RotatedBy(x / 44f * MathHelper.TwoPi + MathHelper.Pi);
			float mulC = 1f;
			if (x < 10)
			{
				mulC = x / 10f;
			}
			if (x >= 34)
			{
				mulC = (44 - x) / 10f;
			}
			bars4.Add(new Vertex2D(bulbPos + addPos, Color.White * mulC, new Vector3(timeValue + factor * 5.6f, 0, 1)));
			bars4.Add(new Vertex2D(bulbPos + addPos - Vector2.Normalize(addPos) * Math.Min(addPos.Length(), 40f), Color.White * 0, new Vector3(timeValue + factor * 8f, 1, 1)));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_5_black.Value;
		if (bars4.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars4.ToArray(), 0, bars4.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars4.ToArray(), 0, bars4.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_5.Value;
		if (bars3.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	private void GenerateDust()
	{
		float value = Main.rand.NextFloat(Energy) / 200f;
		float energyValue = Energy / 600f;
		energyValue = MathF.Pow(energyValue, 0.3f);
		for (int x = 0; x < value; x++)
		{
			Vector2 v0 = new Vector2(0, Projectile.ai[0] * Main.rand.NextFloat(0.9f, 1.2f) * energyValue).RotatedByRandom(MathHelper.TwoPi);

			float speed = 0.08f;
			var d = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4)/*Dust?Size=8x8*/, 0, 0, DustID.Ichor, -v0.Y * speed, v0.X * speed, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
			d.noGravity = true;
			d.velocity = v0.RotatedBy(-2f) * speed;
			d.scale *= Energy / 600f;
			if (Main.rand.NextBool(2))
			{
				var blood = new IchorDrop
				{
					Velocity = v0.RotatedBy(-2f) * speed,
					Active = true,
					Visible = true,
					Position = Projectile.Center + v0,
					MaxTime = Main.rand.Next(12, 24),
					Scale = Main.rand.NextFloat(6f, 14f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawPowerEffect();
		return false;
	}
}
