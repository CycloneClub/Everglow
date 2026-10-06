using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Hook : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public float HookRot = 0;

	public float HookPhase = 0;

	public float ArmPhase = 0;

	public float StandardRange = 360;

	public float RotatedValue = 0f;

	public int ExtraSpeed = 0;

	public bool LightingboltEnchanted;

	public Vector2 HookSurfacePos = Vector2.zeroVector;

	public Vector2 HookSurfacePos_Phase0 = Vector2.zeroVector;

	public Vector2 HookPos = Vector2.zeroVector;

	public Vector2 HookVel = Vector2.zeroVector;

	public Queue<Vector2> OldHookPos = new Queue<Vector2>();

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 100;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;

		Projectile.friendly = true;

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
		LightingboltEnchanted = iKeeper.ThunderHook;

		Projectile.extraUpdates = ExtraSpeed - 1;
		Projectile.velocity = Projectile.velocity.NormalizeSafe();
		Projectile.direction = 1;
		if (Projectile.velocity.X < 0)
		{
			Projectile.direction = -1;
		}
		Projectile.spriteDirection = Projectile.direction;

		HookSurfacePos = Vector2.zeroVector;
		ArmPhase = 0;
		HookPhase = 0;
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
			return;
		}
		Projectile.Center = Owner.Center + new Vector2(-Owner.spriteDirection * 10, -10);
		Timer++;
		if (Timer < 6)
		{
			Projectile.rotation = Projectile.rotation * 0.6f + (Projectile.velocity.ToRotationSafe() - MathHelper.PiOver2) * 0.4f;
			RotatedValue = 0;
		}
		if (Timer == 6)
		{
			HookSurfacePos = Projectile.velocity * 36;
			HookVel = Projectile.velocity * 12;
			Projectile.ai[1] = 0;
		}
		if (Timer >= 6 && Timer < 40)
		{
			HookSurfacePos += HookVel;
			HookVel.Y += 0.5f;
			if (HookVel != Vector2.zeroVector)
			{
				HookRot = HookVel.ToRotation() + MathHelper.PiOver2;
			}
			if (Collision.SolidCollision(HookSurfacePos + HookVel - new Vector2(12), 24, 24))
			{
				HookVel *= 0;
			}
			if (Timer < 30)
			{
				Projectile.rotation = Projectile.velocity.ToRotationSafe() - MathHelper.PiOver2;
				RotatedValue = 0;
			}
			HookSurfacePos_Phase0 = HookSurfacePos;
		}
		if (Timer >= 30 && Timer < 45)
		{
			if (Projectile.ai[1] < 1f)
			{
				Projectile.ai[1] += 0.08f;
			}
			Projectile.rotation += Projectile.ai[1] * Projectile.spriteDirection;
			ArmPhase += Projectile.ai[1] * Projectile.spriteDirection;
			RotatedValue += 1 / 15f;
		}
		if (Timer >= 40 && Timer < 60)
		{
			float valueLerp = 0f;
			if (valueLerp < 0.6f)
			{
				valueLerp += 0.1f;
			}

			// Main.NewText(HookPhase);
			HookSurfacePos_Phase0 = HookSurfacePos_Phase0 * 0.8f + HookSurfacePos_Phase0.NormalizeSafe() * StandardRange * 0.2f;
			float oldPhase = HookPhase;
			HookPhase = HookPhase * (1 - valueLerp) + ArmPhase * valueLerp;
			if (Timer == 59)
			{
				Projectile.ai[1] = HookPhase - oldPhase;
			}
			HookSurfacePos = HookSurfacePos_Phase0.RotatedBy(HookPhase);
			HookVel = HookSurfacePos.NormalizeSafe().RotatedBy(-MathHelper.PiOver2) * 0.5f * Projectile.spriteDirection;
			if (HookVel != Vector2.zeroVector)
			{
				HookRot = HookVel.ToRotation() + MathHelper.PiOver2;
			}
		}
		if (Timer >= 60 && Timer <= 80)
		{
			if (Timer == 60)
			{
				// HookPhase = ArmPhase;
			}
			StandardRange -= 18;
			HookSurfacePos_Phase0 = HookSurfacePos_Phase0 * 0.8f + HookSurfacePos_Phase0.NormalizeSafe() * StandardRange * 0.2f;
			HookPhase += Projectile.ai[1];
			Projectile.ai[1] *= 0.9f;
			HookSurfacePos = HookSurfacePos_Phase0.RotatedBy(HookPhase);
			HookVel = HookSurfacePos.NormalizeSafe().RotatedBy(-MathHelper.PiOver4) * 0.5f * Projectile.spriteDirection;
			if (HookVel != Vector2.zeroVector)
			{
				HookRot = HookVel.ToRotation() + MathHelper.PiOver2;
			}
		}
		if (Timer == 80)
		{
			Projectile.rotation += MathHelper.TwoPi * 100;
			Projectile.rotation %= MathHelper.TwoPi;
		}
		if (Timer > 80)
		{
			Projectile.rotation *= 0.8f;
		}
		HookPos = HookSurfacePos;
		HookPos.Y *= (float)Utils.Lerp(1f, 0.45f, RotatedValue);
		HookPos = HookPos.RotatedBy(VelocityRot() * RotatedValue);
		HookRot += VelocityRot() * RotatedValue;

		OldHookPos.Enqueue(HookPos);
		if (OldHookPos.Count > 20)
		{
			OldHookPos.Dequeue();
		}
		if (LightingboltEnchanted)
		{
			if (Timer == 40)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<Georg_Hook_Thunder_Release>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0.6f);
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Timer >= 80)
		{
			return false;
		}
		if (Timer >= 40 && Timer <= 80)
		{
			List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(OldHookPos); // 平滑
			var smoothTrail = new List<Vector2>();
			for (int x = 0; x < smoothTrail_current.Count - 1; x++)
			{
				smoothTrail.Add(smoothTrail_current[x]);
			}
			if (OldHookPos.Count != 0)
			{
				smoothTrail.Add(OldHookPos.ToArray()[OldHookPos.Count - 1]);
			}
			for (int i = smoothTrail.Count - 1; i > 0; --i)
			{
				float point = 0;
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, smoothTrail[i] + Projectile.Center, 40, ref point))
				{
					return true;
				}
			}
		}
		Vector2 hitPos = Projectile.Center + HookPos;
		return Rectangle.Intersect(new Rectangle((int)hitPos.X - 24, (int)hitPos.Y - 24, 48, 48), targetHitbox) != Rectangle.emptyRectangle;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		var vecToCenter = Projectile.Center - target.Center;
		float mulValue = vecToCenter.Length() / HookPos.Length();
		if (Timer < 40 || Timer > 80)
		{
			modifiers.Knockback *= 0.1f;
			modifiers.FinalDamage *= 0.1f;
		}
		else
		{
			modifiers.FinalDamage *= mulValue * 1.3f + 0.4f;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	public float VelocityRot()
	{
		float value = Projectile.velocity.ToRotationSafe();
		if (value > MathHelper.PiOver2)
		{
			value -= MathHelper.Pi;
		}
		if (value < -MathHelper.PiOver2)
		{
			value += MathHelper.Pi;
		}
		return value;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModAsset.Georg_Hook.Value;

		var hookFrame = new Rectangle(56, 30, 26, 44);
		float hookScale = 0f;
		if (Timer >= 6 && Timer <= 9)
		{
			hookScale = (Timer - 6) / 3f;
		}
		if (Timer >= 9)
		{
			hookScale = 1;
		}

		if (Timer < 80)
		{
			Color lightC = Lighting.GetColor((Projectile.Center + HookPos).ToTileCoordinates());
			Main.spriteBatch.Draw(tex, Projectile.Center + HookPos - Main.screenPosition, hookFrame, lightC, HookRot, hookFrame.Size() * 0.5f, Projectile.scale * hookScale, SpriteEffects.None, 0);
		}

		if (Timer < 80)
		{
			var checkPos = HookSurfacePos;
			var antiVel = -HookVel.NormalizeSafe() * 16;
			var middlePoints = new List<Vector2>();
			if (Timer >= 30 && Timer < 45)
			{
				int midCount = Timer - 30;
				for (int i = 0; i < midCount; i++)
				{
					middlePoints.Add(Projectile.velocity.RotatedBy(ArmPhase * (i / (float)midCount)) * (midCount - i) * HookSurfacePos_Phase0.Length() * 0.9f / 15f);
				}
			}
			if (Timer >= 36)
			{
				int removeCount = Timer - 40 + 4;

				// removeCount = (int)(MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 6 + 6);
				for (int i = 0; i < removeCount; i++)
				{
					if (middlePoints.Count > 0)
					{
						middlePoints.RemoveAt(0);
					}
				}
			}
			for (int t = 0; t < middlePoints.Count; t++)
			{
				for (int i = 0; i < 300; i++)
				{
					var jointFrame = new Rectangle(0, 30, 14, 24);
					if (i % 2 == 0)
					{
						jointFrame = new Rectangle(18, 30, 6, 24);
					}
					checkPos += antiVel;
					Vector2 toTarget = middlePoints[t] - checkPos - antiVel;
					antiVel = antiVel * 0.8f + toTarget.NormalizeSafe() * 16f * 0.2f;
					Vector2 correctionCheckPos = checkPos;
					correctionCheckPos.Y *= (float)Utils.Lerp(1f, 0.45f, RotatedValue);
					correctionCheckPos = correctionCheckPos.RotatedBy(VelocityRot() * RotatedValue);
					Vector2 correctionVel = antiVel;
					correctionVel.Y *= (float)Utils.Lerp(1f, 0.45f, RotatedValue);
					correctionVel = correctionVel.RotatedBy(VelocityRot() * RotatedValue);
					Color lightC = Lighting.GetColor((Projectile.Center + correctionCheckPos).ToTileCoordinates());
					Main.spriteBatch.Draw(tex, Projectile.Center + correctionCheckPos - Main.screenPosition, jointFrame, lightC, correctionVel.ToRotation() + MathHelper.PiOver2, jointFrame.Size() * 0.5f, Projectile.scale * hookScale, SpriteEffects.None, 0);
					if ((middlePoints[t] - checkPos).Length() < 16)
					{
						break;
					}
				}
			}
			var terminalPos = Projectile.velocity.RotatedBy(ArmPhase) * 12 * StandardRange / 360f;
			for (int i = 0; i < 300; i++)
			{
				var jointFrame = new Rectangle(0, 30, 14, 24);
				if (i % 2 == 0)
				{
					jointFrame = new Rectangle(18, 30, 6, 24);
				}
				checkPos += antiVel;
				Vector2 toTarget = terminalPos - checkPos - antiVel;
				antiVel = antiVel * 0.8f + toTarget.NormalizeSafe() * 16f * 0.2f;
				Vector2 correctionCheckPos = checkPos;
				correctionCheckPos.Y *= (float)Utils.Lerp(1f, 0.45f, RotatedValue);
				correctionCheckPos = correctionCheckPos.RotatedBy(VelocityRot() * RotatedValue);
				Vector2 correctionVel = antiVel;
				correctionVel.Y *= (float)Utils.Lerp(1f, 0.45f, RotatedValue);
				correctionVel = correctionVel.RotatedBy(VelocityRot() * RotatedValue);
				Color lightC = Lighting.GetColor((Projectile.Center + correctionCheckPos).ToTileCoordinates());
				Main.spriteBatch.Draw(tex, Projectile.Center + correctionCheckPos - Main.screenPosition, jointFrame, lightC, correctionVel.ToRotation() + MathHelper.PiOver2, jointFrame.Size() * 0.5f, Projectile.scale * hookScale, SpriteEffects.None, 0);
				if ((terminalPos - checkPos).Length() < 16)
				{
					break;
				}
			}
		}

		var armFrame = new Rectangle(0, 0, 12, 28);
		if (Projectile.spriteDirection == -1)
		{
			armFrame = new Rectangle(70, 0, 12, 28);
		}
		if (Timer >= 6 && Timer < 9)
		{
			armFrame = new Rectangle(14, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(56, 0, 12, 28);
			}
		}
		if (Timer >= 9 && Timer < 79)
		{
			armFrame = new Rectangle(28, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(42, 0, 12, 28);
			}
		}
		if (Timer >= 79 && Timer < 83)
		{
			armFrame = new Rectangle(14, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(56, 0, 12, 28);
			}
		}
		if (Timer >= 86)
		{
			armFrame = new Rectangle(0, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(70, 0, 12, 28);
			}
		}
		Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(4, -4) - Main.screenPosition, armFrame, lightColor, Projectile.rotation, new Vector2(6, 4), Projectile.scale, SpriteEffects.None, 0);
		DrawTrail();
		return false;
	}

	public void DrawTrail()
	{
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(OldHookPos); // 平滑
		var smoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count - 1; x++)
		{
			smoothTrail.Add(smoothTrail_current[x]);
		}
		if (OldHookPos.Count != 0)
		{
			smoothTrail.Add(OldHookPos.ToArray()[OldHookPos.Count - 1]);
		}
		float trailWidth = 60;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		var bars3 = new List<Vertex2D>();

		var bars4 = new List<Vertex2D>();
		var bars5 = new List<Vertex2D>();
		var bars6 = new List<Vertex2D>();
		for (int i = smoothTrail.Count - 1; i > 0; --i)
		{
			float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
			if (mulFac > 1f)
			{
				mulFac = 1f;
			}
			float factor = i / (float)smoothTrail.Count * mulFac;
			float width = TrailWidthFunction(1 - factor);

			float timeValue = (float)Main.time * 0.06f;

			Vector2 drawPos = smoothTrail[i] + Projectile.Center;
			Color drawC = Color.White * 0.8f;
			if (i == 1)
			{
				drawC *= 0;
			}
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
			bars.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
			bars2.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 0, width)));
			bars2.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
			bars3.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
			bars3.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));

			drawC = new Color(1f, 1f, 1f, 0) * 0.4f;
			Color lightC = Lighting.GetColor((drawPos / 16f).ToPoint());
			drawC.R = (byte)(lightC.R * drawC.R / 255f);
			drawC.G = (byte)(lightC.G * drawC.G / 255f);
			drawC.B = (byte)(lightC.B * drawC.B / 255f);
			if (LightingboltEnchanted)
			{
				drawC = Color.Lerp(new Color(0.05f, 0.1f, 0.6f, 0), new Color(0.7f, 0.9f, 1f, 0), factor);
				Lighting.AddLight(drawPos, drawC.ToVector3() * width * 0.5f);
				if (!Main.gamePaused && i > 1)
				{
					if (Main.rand.Next(1000) < (smoothTrail[i] - smoothTrail[i - 1]).Length())
					{
						float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(8f, 18f));
						Vector2 afterVelocity = (smoothTrail[i] - smoothTrail[i - 1]).NormalizeSafe() * 15;
						var electric = new ElectricCurrent
						{
							Velocity = afterVelocity,
							Active = true,
							Visible = true,
							Position = smoothTrail[i] + Projectile.Center - afterVelocity * 3,
							MaxTime = 10,
							Scale = size,
							ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), size, 0 },
						};
						Ins.VFXManager.Add(electric);
					}
				}
			}
			if (i == 1)
			{
				drawC *= 0;
			}
			bars4.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
			bars4.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
			bars5.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 0, width)));
			bars5.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
			bars6.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * trailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
			bars6.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS.SortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}

		if (bars3.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
		if (LightingboltEnchanted)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_17.Value;
		}
		if (bars4.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars4.ToArray(), 0, bars4.Count - 2);
		}

		if (bars5.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars5.ToArray(), 0, bars5.Count - 2);
		}

		if (bars6.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars6.ToArray(), 0, bars6.Count - 2);
		}
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
