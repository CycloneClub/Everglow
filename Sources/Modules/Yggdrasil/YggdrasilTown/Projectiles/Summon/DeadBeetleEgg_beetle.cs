using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class DeadBeetleEgg_beetle : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	private Player Owner => Main.player[Projectile.owner];

	public int EnemyTarget;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 300;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		Projectile.minionSlots = 1;
		Projectile.minion = true;
	}

	public int ManaValue = 0;

	public bool ManaAttack = false;

	public int TeleportTimer = 0;

	/// <summary>
	/// 0 : walk;1 : fly
	/// </summary>
	public int State = 0;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = 0;
		Projectile.ai[1] = 0;
		EnemyTarget = -1;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		CheckKill();
		if (Projectile.velocity.X > 1)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X < -1)
		{
			Projectile.spriteDirection = 1;
		}
		FindFrame();
		EnemyTarget = FindEnemy();
		if (EnemyTarget != -1)
		{
			if (ManaAttack)
			{
				AttackWithMagic();
			}
			else
			{
				AttackNoMagic();
			}
		}
		else
		{
			ApproachMyOwner();
		}
		if (TeleportTimer > 0)
		{
			TeleportTimer--;
		}
		else
		{
			TeleportTimer = 0;
		}
	}

	/// <summary>
	/// When there are no enemies, try go back to player.
	/// </summary>
	public void ApproachMyOwner()
	{
		Projectile.ai[0] = 0;
		Projectile.ai[2] = 0;
		Projectile.friendly = false;
		Vector2 targetPos = FindTargetPosWhenNoEnemies();
		bool PlayerStand = Collision.SolidCollision(Owner.BottomLeft, Owner.width, 4);
		Vector2 toTarget = targetPos - Projectile.velocity - Projectile.Center;
		if (toTarget.Length() > 2400)
		{
			TeleportTimer = 20;
			Projectile.Center = targetPos;
			Projectile.velocity *= 0;
			for (int i = 0; i < 20; i++)
			{
				var spark = new Spark_MoonBladeDust
				{
					Velocity = new Vector2(0, -6).RotatedBy(i / 20f * MathHelper.TwoPi),
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(20, 36),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(5f, 10.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					noGravity = true,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
				};
				Ins.VFXManager.Add(spark);
			}
			for (int i = 0; i < 20; i++)
			{
				var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.Wraith);
				dust.velocity = new Vector2(0, -6).RotatedBy(i / 20f * MathHelper.TwoPi);
				dust.scale = 1.5f;
				dust.noGravity = true;
			}
		}
		if (PlayerStand)
		{
			if (Main.rand.NextBool(16))
			{
				State = 0;
			}
		}
		else
		{
			if (Main.rand.NextBool(16))
			{
				State = 1;
			}
		}
		if (State == 0)
		{
			if (toTarget.Length() > 8)
			{
				toTarget = Vector2.Normalize(toTarget);
				if (Collision.SolidCollision(Projectile.Bottom, 2, 8))
				{
					Projectile.velocity.X += toTarget.X * 0.5f;
				}
				else
				{
					Projectile.velocity.X += toTarget.X * 0.1f;
				}
				if (Projectile.velocity.Length() > 8f)
				{
					Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 8;
				}
				Projectile.velocity.Y += 0.3f;
			}
			else
			{
				Projectile.velocity *= 0f;
			}
		}
		if (State == 1)
		{
			if (toTarget.Length() > 8)
			{
				toTarget = Vector2.Normalize(toTarget);
				Projectile.velocity += toTarget * 0.5f;
				Projectile.velocity *= 0.95f;
			}
			else
			{
				Projectile.velocity *= 0f;
			}
		}
	}

	public int FindEnemy()
	{
		if (Owner.MinionAttackTargetNPC != -1)
		{
			return Owner.MinionAttackTargetNPC;
		}
		float minDetectionRange = 1200;
		int targeWhoAmI = -1;
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc.life > 0 && !npc.friendly && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter && npc.CanBeChasedBy() && !npc.dontTakeDamage)
			{
				if ((npc.Center - Owner.Center).Length() < 1200)
				{
					if (Collision.CanHit(npc.Center - Vector2.One, 2, 2, Projectile.Center - Vector2.One, 2, 2))
					{
						float distance = (npc.Center - Projectile.Center).Length();
						if (npc.boss)
						{
							distance -= 600;
						}
						if (distance < minDetectionRange)
						{
							targeWhoAmI = npc.whoAmI;
							minDetectionRange = distance;
						}
					}
				}
			}
		}
		return targeWhoAmI;
	}

	/// <summary>
	/// ai[0] for rush-timer.
	/// </summary>
	public void AttackNoMagic()
	{
		Projectile.ai[2] = 0;
		State = 1;
		NPC target = Main.npc[EnemyTarget];
		if (target == null || !target.active || target.life <= 0 || target.friendly)
		{
			EnemyTarget = -1;
			return;
		}
		Vector2 relativePos = new Vector2(0, -125).RotatedBy(Projectile.ai[1]);
		Vector2 targetPos = target.Center + relativePos;
		Vector2 toTarget = targetPos - Projectile.velocity - Projectile.Center;
		if (Projectile.ai[0] <= 0)
		{
			Projectile.rotation *= 0.8f;
			if (toTarget.Length() > 80)
			{
				toTarget = Vector2.Normalize(toTarget);
				Projectile.velocity += toTarget * 0.5f;
				Projectile.velocity *= 0.95f;
			}
			else
			{
				Projectile.ai[0] = 40f;
			}
		}
		else
		{
			if (Projectile.ai[0] > 20f)
			{
				toTarget = Vector2.Normalize(toTarget);
				Projectile.velocity += toTarget * 0.5f;
				Projectile.velocity *= 0.95f;
				Projectile.rotation = Projectile.rotation * 0.9f + Projectile.ai[1] * 0.1f;
			}
			if (Projectile.ai[0] == 20)
			{
				Projectile.friendly = true;
				Projectile.velocity = -relativePos * 0.3f;
				var trace = new BeetleDashTraceDust
				{
					Velocity = Projectile.velocity * 0.1f,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = 28,
					Scale = 80,
					Rotation = Projectile.velocity.ToRotation(),
					ProjectileOwner = Projectile,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 1 },
				};
				Ins.VFXManager.Add(trace);
				var trace2 = new BeetleDashTrace_frontDust
				{
					Velocity = Projectile.velocity * 0.1f,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = 28,
					Scale = 80,
					Rotation = Projectile.velocity.ToRotation(),
					projectileOwner = Projectile,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 1 },
				};
				for (int i = 0; i < 10; i++)
				{
					var dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.Cloud);
					dust.velocity = new Vector2(0, -2).RotatedBy(i / 10f * MathHelper.TwoPi);
					dust.scale = 0.5f;
				}
				Ins.VFXManager.Add(trace2);
			}
			if (Projectile.ai[0] < 20)
			{
				Projectile.velocity *= 0.9f;
			}
			if (Projectile.ai[0] == 1)
			{
				Projectile.friendly = false;
				Projectile.ai[1] = Main.rand.NextFloat(-1.3f, 1.3f);
			}

			Projectile.ai[0]--;
		}
	}

	/// <summary>
	/// When mana attack ();
	/// </summary>
	public void AttackWithMagic()
	{
		if (ManaValue <= 7)
		{
			ManaValue = 0;
			ManaAttack = false;
			return;
		}
		State = 1;
		NPC target = Main.npc[EnemyTarget];
		if (target == null || !target.active || target.life <= 0 || target.friendly)
		{
			EnemyTarget = -1;
			return;
		}
		Projectile.ai[1] = MathF.Sin((float)Main.time * 0.01f + Projectile.whoAmI);
		float distance = 260 + 175 * MathF.Sin((float)Main.time * 0.04f + Projectile.whoAmI);
		Vector2 relativePos = new Vector2(0, -distance).RotatedBy(Projectile.ai[1]);
		Vector2 targetPos = target.Center + relativePos;
		Vector2 toTarget = targetPos - Projectile.velocity - Projectile.Center;
		Projectile.rotation *= 0.8f;
		toTarget = Vector2.Normalize(toTarget);
		Projectile.velocity += toTarget * 1.5f;
		Projectile.velocity *= 0.9f;
		if (Projectile.velocity.Length() > 1f)
		{
			// Dust effect
			Vector2 v0 = Projectile.velocity;
			if (v0.Length() > 6)
			{
				v0 = Vector2.Normalize(v0) * 6;
			}
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0.1f, 4f)).RotatedByRandom(MathHelper.TwoPi) + v0;
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-24f, 24f), 0).RotatedByRandom(6.283) - Projectile.velocity * 6,
				MaxTime = Main.rand.Next(20, 36),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(5f, 10.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
		Projectile.ai[2]++;
		if (Projectile.ai[2] >= 120)
		{
			Vector2 vel = Vector2.Normalize(-relativePos) * 8f;
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<DeadBeetleEgg_beetle_proj>(), (int)(Projectile.damage * 2.8), Projectile.knockBack, Projectile.owner, EnemyTarget);
			Projectile.ai[2] = 0;
			ManaValue -= 2;
		}
	}

	public Vector2 FindTargetPosWhenNoEnemies()
	{
		// when state is walk, line up by whoAmI.
		if (State == 0)
		{
			// A serrated function.
			float deltaX = MathF.Acos(MathF.Cos(GetOrderFromOwner() * 0.1f));
			Vector2 targetPos = Owner.Center + new Vector2((-48 - deltaX * 240) * Owner.direction, 24);
			if (!Collision.SolidCollision(targetPos - new Vector2(15), 30, 32))
			{
				int count = 0;
				while (!Collision.SolidCollision(targetPos - new Vector2(15), 30, 32))
				{
					count++;
					targetPos.Y += 16;
					if (count > 40)
					{
						break;
					}
				}
			}
			if (Collision.SolidCollision(targetPos - new Vector2(30), 30, 30))
			{
				int count = 0;
				while (Collision.SolidCollision(targetPos - new Vector2(30), 30, 30))
				{
					count++;
					targetPos.Y -= 16;
					if (count > 40)
					{
						break;
					}
				}
			}
			return targetPos;
		}

		// when state is fly, swirl behide player.
		if (State == 1)
		{
			float timeValue = (float)Main.time * 0.09f;
			Vector2 targetPos = Owner.Center + new Vector2(-120, -72) + new Vector2(180 * MathF.Sin(timeValue + Projectile.whoAmI), 90 * MathF.Sin(timeValue * 0.33f + Projectile.whoAmI));
			return targetPos;
		}
		return Owner.Center;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (State == 0)
		{
			Vector2 targetPos = FindTargetPosWhenNoEnemies();
			Vector2 toTarget = targetPos - Projectile.velocity - Projectile.Center;
			if (toTarget.Length() > 60)
			{
				Projectile.velocity.Y -= 3f;
			}
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		ManaValue++;
		if (ManaValue >= 24)
		{
			ManaValue = 24;
			ManaAttack = true;
		}
	}

	/// <summary>
	/// Get a unique order by Projectile.whoAmI.
	/// </summary>
	/// <returns></returns>
	public int GetOrderFromOwner()
	{
		int count = 0;
		for (int i = 0; i < Main.projectile.Length; i++)
		{
			Projectile projectile = Main.projectile[i];
			if (projectile.active && projectile.owner == Owner.whoAmI)
			{
				if (projectile == Projectile)
				{
					return count;
				}
				if (projectile.type == Type)
				{
					count++;
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// Walking or flying will behaved different.
	/// </summary>
	public void FindFrame()
	{
		if (State == 0)
		{
			Projectile.frameCounter += (int)Projectile.velocity.Length() * 10;
			if (Projectile.frameCounter > 24)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 3)
			{
				Projectile.frame = 0;
			}
			Point myPos = Projectile.Center.ToTileCoordinates();
			float rot = YggdrasilWorldGeneration.TerrianSurfaceAngle(myPos.X, myPos.Y, 4);
			if (rot != -1)
			{
				Projectile.rotation = rot - MathHelper.PiOver2;
			}
			else
			{
				Projectile.rotation = 0;
			}
		}
		if (State == 1)
		{
			Projectile.frameCounter += 1;
			if (Projectile.frameCounter > 1)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 4)
			{
				Projectile.frame = 0;
			}
			if (Projectile.ai[0] <= 0)
			{
				Projectile.rotation *= 0.8f;
			}
		}
	}

	private void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<DeadBeetleEggBuff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<DeadBeetleEggBuff>()))
		{
			Projectile.timeLeft = 2;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D texture = ModAsset.DeadBeetleEgg_beetle.Value;
		Texture2D textureglow = ModAsset.DeadBeetleEgg_beetle_glow.Value;
		var frame = new Rectangle(0, Projectile.frame * 30, 60, 30);
		if (State == 1)
		{
			texture = ModAsset.DeadBeetleEgg_beetle_fly.Value;
			textureglow = ModAsset.DeadBeetleEgg_beetle_fly_glow.Value;
			frame = new Rectangle(0, (Projectile.frame + Projectile.whoAmI) % 4 * 60, 60, 60);
		}
		Texture2D point = Commons.ModAsset.LightPoint2.Value;
		Texture2D point2 = Commons.ModAsset.LightPoint2_black.Value;
		for (int i = 0; i < 24; i++)
		{
			Vector2 offest = new Vector2(0, -24).RotatedBy((i - 12) * 0.2f);
			offest.X *= Projectile.spriteDirection;
			float scale = 0.2f + MathF.Sin((float)Main.time * 0.03f + Projectile.whoAmI + i) * 0.06f;
			if (ManaValue > i)
			{
				scale *= 1.2f;
			}
			if (ManaValue == i)
			{
				scale *= 2f;
			}
			Main.spriteBatch.Draw(point2, Projectile.Center + offest - Main.screenPosition, null, Color.White * 0.75f, Projectile.rotation, point2.Size() * 0.5f, scale, SpriteEffects.None, 0);
			if (ManaValue > i)
			{
				Main.spriteBatch.Draw(point, Projectile.Center + offest - Main.screenPosition, null, new Color(0.1f, 0.7f, 1f, 0f), Projectile.rotation, point.Size() * 0.5f, scale, SpriteEffects.None, 0);
			}
		}
		if (ManaAttack)
		{
			Texture2D texture_light = ModAsset.DeadBeetleEgg_beetle_light.Value;
			if (State == 1)
			{
				texture_light = ModAsset.DeadBeetleEgg_beetle_fly_light.Value;
			}
			Main.spriteBatch.Draw(texture_light, Projectile.Center - Main.screenPosition, frame, new Color(0.1f, 0.7f, 1f, 0f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.05f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			Main.spriteBatch.Draw(texture_light, Projectile.Center - Main.screenPosition, frame, new Color(0.1f, 0.7f, 1f, 0f) * 0.5f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * 1.2f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		Main.spriteBatch.Draw(textureglow, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0f), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

		// Concentrating power
		if (ManaAttack)
		{
			if (Projectile.ai[2] >= 30)
			{
				float timer = Projectile.ai[2] - 30f;
				SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Effect effect = ModAsset.TeleportToYggdrasilVortexEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				int precise = 150;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uTimer"].SetValue(timer * 0.018f);
				effect.Parameters["uColor0"].SetValue(new Vector4(1f, 1f, 1f, 1f));
				effect.Parameters["uColor1"].SetValue(new Vector4(1f, 1f, 1f, 1f));
				effect.CurrentTechnique.Passes[0].Apply();
				Vector2 drawCenter = Projectile.Center;
				float timeValue = (float)Main.time * 0.004f;
				timeValue += MathF.Pow(timer / 210f, 5) * 20f;

				// dark net
				float deltaRot = 0.25f;
				var nets = new List<Vertex2D>();
				for (int i = 0; i <= precise; i++)
				{
					Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
					float vertexWidth = 100 * (90 - timer) / 90f;

					float fade = Math.Clamp(timer / 90f, 0, 1);
					var drawColor = new Color(1f, 1f, 1f, 1f);
					nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
					nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, timer * deltaRot));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_crack_dense_black.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uTimer"].SetValue(timer * 0.02f);
				effect.Parameters["uColor0"].SetValue(new Vector4(0.1f, 0.5f, 1f, 0));
				effect.Parameters["uColor1"].SetValue(new Vector4(0f, 0f, 0.6f, 0));
				effect.CurrentTechnique.Passes[0].Apply();

				nets = new List<Vertex2D>();
				for (int i = 0; i <= precise; i++)
				{
					Vector2 normalWidth = new Vector2(0, -1).RotatedBy(i / (float)precise * MathHelper.TwoPi);
					float vertexWidth = 100 * (90 - timer) / 90f;

					float fade = Math.Clamp(timer / 90f, 0, 1);
					Color drawColor = Color.Lerp(new Color(0.1f, 0.5f, 1f, 0), new Color(0f, 0.7f, 0.6f, 0), (MathF.Sin(i / 37.5f * MathHelper.TwoPi + timeValue * 3) + 1) * 0.5f) * 2;
					nets.Add(drawCenter, drawColor * fade, new Vector3(new Vector2(0.5f), 0));
					nets.Add(drawCenter + normalWidth * vertexWidth, Color.Transparent, new Vector3(new Vector2(0.5f) + normalWidth * 0.35f, timer * deltaRot));
				}
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, nets.ToArray(), 0, nets.Count - 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
			if (Projectile.ai[2] >= 60)
			{
				Texture2D star = Commons.ModAsset.StarSlash.Value;
				float sinValue = MathF.Sin((Projectile.ai[2] - 60) / 60f * MathHelper.Pi);
				Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.5f, 1f, 0), 0, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.5f), SpriteEffects.None, 0);
				Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.5f, 1f, 0), MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 1f), SpriteEffects.None, 0);
				float lerpValue = MathF.Sin((Projectile.ai[2] - 60) / 40f * MathHelper.Pi) * 1.5f;
				Texture2D proj = ModAsset.SquamousAirProj.Value;
				Main.spriteBatch.Draw(proj, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), 0, proj.Size() * 0.5f, lerpValue, SpriteEffects.None, 0);
			}
		}
		if (TeleportTimer > 0)
		{
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Texture2D starBlack = Commons.ModAsset.StarSlash_black.Value;
			float sinValue = MathF.Sin(TeleportTimer / 20f * MathHelper.Pi);
			Main.spriteBatch.Draw(starBlack, Projectile.Center - Main.screenPosition, null, Color.White, 0, starBlack.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.85f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(starBlack, Projectile.Center - Main.screenPosition, null, Color.White, MathHelper.PiOver2, starBlack.Size() * 0.5f, new Vector2(0.5f * sinValue, 1f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(starBlack, Projectile.Center - Main.screenPosition, null, Color.White, -MathHelper.PiOver4, starBlack.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.55f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(starBlack, Projectile.Center - Main.screenPosition, null, Color.White, MathHelper.PiOver4, starBlack.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.55f), SpriteEffects.None, 0);

			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.2f, 0.14f, 0), 0, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.85f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.2f, 0.14f, 0), MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 1f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.2f, 0.14f, 0), -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.55f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.2f, 0.14f, 0), MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(0.5f * sinValue, 0.55f), SpriteEffects.None, 0);
		}
		return false;
	}
}
