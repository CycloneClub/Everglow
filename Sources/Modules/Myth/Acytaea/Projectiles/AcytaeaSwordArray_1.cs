using Everglow.Myth.Acytaea.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSwordArray_1 : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 3600;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
	}

	public int Timer = 0;
	public float AddRot = 0;
	public float Omega = 0;
	public float Range = 0;
	public NPC Owner = new NPC();
	public List<bool> subProjActive;

	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			Owner = Main.npc[index];
		}
		else
		{
			Projectile.Kill();
		}
		subProjActive = new List<bool>();
		for (int x = 0; x < Projectile.ai[1]; x++)
		{
			subProjActive.Add(true);
		}
		Projectile.frame = 0;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.Kill();
			return;
		}
		Timer++;
		if (Timer < 100)
		{
			Projectile.rotation = 0;
			Vector2 center = Owner.Center + new Vector2(0, -455);
			foreach (var proj in Main.projectile)
			{
				if (proj != null && proj.active)
				{
					if (proj.type == ModContent.ProjectileType<AcytaeaMagicArraySword>())
					{
						AcytaeaMagicArraySword aMAS = proj.ModProjectile as AcytaeaMagicArraySword;
						center = aMAS.EndPos;
						break;
					}
				}
			}
			Projectile.Center = center;
			Range = Timer * Projectile.ai[2];
		}
		else if (Timer > 100 && Timer < 130)
		{
			if (Timer % 14 == 0)
			{
				Projectile.frame++;
			}
			Projectile.rotation += (Timer - 100) / 360f * Projectile.spriteDirection;
			Omega += 0.002f * Projectile.spriteDirection;
		}
		else
		{
			Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
			Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
			Vector2 normalizeToPlayer = Utils.SafeNormalize(toPlayer, Vector2.zeroVector) * (1620f / Range + Projectile.whoAmI % 5);
			Projectile.velocity = Projectile.velocity * 0.95f + normalizeToPlayer * 0.05f;
			GenerateVFX();
		}
		AddRot += Omega;
		if (Timer > 100)
		{
			for (int k = 0; k < subProjActive.Count; k++)
			{
				Vector2 deltaVector = new Vector2(0, Range).RotatedBy(k / (float)subProjActive.Count * MathHelper.TwoPi + AddRot);
				Vector2 test = Projectile.Center + deltaVector;
				if (Collision.SolidCollision(test - new Vector2(40), 80, 80))
				{
					subProjActive[k] = false;
					AmmoHit(k);
				}
			}
		}

		foreach (bool active in subProjActive)
		{
			if (active)
			{
				return;
			}
		}
		Projectile.Kill();
	}

	private void GenerateVFX()
	{
		for (int k = 0; k < subProjActive.Count; k++)
		{
			if (subProjActive[k])
			{
				if (Main.rand.NextBool(7))
				{
					Vector2 deltaVector = new Vector2(0, Range).RotatedBy(k / (float)subProjActive.Count * MathHelper.TwoPi + AddRot);
					int times = 1;
					for (int x = 0; x < times; x++)
					{
						Vector2 newVec = deltaVector;
						Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2);
						var positionVFX = Projectile.Center + deltaVector + mainVecLeft * Main.rand.NextFloat(-30f, 30f);

						var acytaeaFlame = new AcytaeaFlameDust
						{
							velocity = -mainVecLeft * Main.rand.NextFloat(6f, 12f) * Projectile.spriteDirection,
							Active = true,
							Visible = true,
							position = positionVFX,
							maxTime = Main.rand.Next(14, 16),
							ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
						};
						Ins.VFXManager.Add(acytaeaFlame);
					}
					for (int x = 0; x < times * 2; x++)
					{
						Vector2 newVec = deltaVector;
						Vector2 mainVecLeft = Vector2.Normalize(newVec).RotatedBy(-MathHelper.PiOver2);
						var positionVFX = Projectile.Center + deltaVector + mainVecLeft * Main.rand.NextFloat(-30f, 30f);

						var acytaeaFlame = new AcytaeaSparkDust
						{
							velocity = -mainVecLeft * Main.rand.NextFloat(6f, 12f) * Projectile.spriteDirection,
							Active = true,
							Visible = true,
							position = positionVFX,
							maxTime = Main.rand.Next(14, 36),
							ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(8f, 10f) },
						};
						Ins.VFXManager.Add(acytaeaFlame);
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int k = 0; k < subProjActive.Count; k++)
		{
			Vector2 deltaVector = new Vector2(0, Range).RotatedBy(k / (float)subProjActive.Count * MathHelper.TwoPi + AddRot);
			Rectangle newProjectileHitBox = projHitbox;
			newProjectileHitBox.X += (int)deltaVector.X;
			newProjectileHitBox.Y += (int)deltaVector.Y;
			if (newProjectileHitBox.Intersects(targetHitbox) && subProjActive[k])
			{
				if (Timer > 100)
				{
					subProjActive[k] = false;
					AmmoHit(k);
				}
				return true;
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
	}

	public void AmmoHit(int whoAmI)
	{
		Vector2 deltaVector = new Vector2(0, Range).RotatedBy(whoAmI / (float)subProjActive.Count * MathHelper.TwoPi + AddRot);
		Vector2 test = Projectile.Center + deltaVector;
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb.WithPitchOffset(-1), Projectile.Center);
		Player player = Main.player[Projectile.owner];
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithVolume(0.3f).WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), test);
		for (int x = 0; x < 5; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(6.238f);
			var positionVFX = test + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D tex = ModAsset.AcytaeaFlySword_red.Value;
		Rectangle projFrame = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int k = 0; k < subProjActive.Count; k++)
		{
			if (subProjActive[k])
			{
				Vector2 deltaVector = new Vector2(0, Range).RotatedBy(k / (float)subProjActive.Count * MathHelper.TwoPi + AddRot);
				float rot = MathF.Atan2(deltaVector.Y, deltaVector.X) + MathHelper.PiOver4 + Projectile.rotation;
				Vector2 drawCenter = Projectile.Center - Main.screenPosition + deltaVector;
				Vector2 normal20 = new Vector2(40, -40).RotatedBy(rot);
				Main.spriteBatch.Draw(tex, drawCenter, projFrame, new Color(255, 0, 215, 155), rot, new Vector2(40), Projectile.scale, SpriteEffects.None, 0);
				Color red = Color.Red * (0.03f * Math.Abs(Omega * Range));
				red.A = 0;
				bars.Add(drawCenter + normal20, Color.Transparent, new Vector3(1, 0, 0));
				bars.Add(drawCenter - normal20, Color.Transparent, new Vector3(1, 1, 0));
				for (int i = 0; i < 6; i++)
				{
					float value = 1 - i / 6f;
					Vector2 newDelta = deltaVector.RotatedBy(-i * Omega);
					Vector2 newDrawCenter = Projectile.Center - Main.screenPosition + newDelta;
					bars.Add(newDrawCenter + normal20.RotatedBy(-i * Omega), red * value, new Vector3(value, 0, 0));
					bars.Add(newDrawCenter - normal20.RotatedBy(-i * Omega), red * value, new Vector3(value, 1, 0));
				}
				Vector2 endDelta = deltaVector.RotatedBy(-6 * Omega);
				Vector2 endDrawCenter = Projectile.Center - Main.screenPosition + endDelta;
				bars.Add(endDrawCenter + normal20.RotatedBy(-6 * Omega), Color.Transparent, new Vector3(0, 0, 0));
				bars.Add(endDrawCenter - normal20.RotatedBy(-6 * Omega), Color.Transparent, new Vector3(0, 1, 0));
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
}
