using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC.VFX;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternSword_Proj : MeleeProj_3D
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public NPC NextTarget = null;

	public int NextTargetAvailableTimer = 0;

	public int AttackStyle = 0;

	public override void OnSpawn(IEntitySource source)
	{
		EnableSphereCoordDraw = false;
		SlashColor = new Color(0.85f, 0.02f, 0.06f, 0);
	}

	public override void SetCustomDefaults()
	{
		Projectile.width = 82;
		Projectile.height = 82;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 5;
		WeaponLength = 84;
	}

	public override void AI()
	{
		base.AI();
		if (NextTargetAvailableTimer > 0)
		{
			NextTargetAvailableTimer--;
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			if (NextTarget is not null && NextTarget.active && NextTargetAvailableTimer > 0)
			{
				Vector2 des = TeleportDestination(NextTarget);
				if (!Collision.SolidCollision(des - new Vector2(8, 16), 16, 16))
				{
					Vector2 slashVel = Vector2.zeroVector;
					if (NextTarget.velocity.X > 0)
					{
						slashVel = new Vector2(12f, 0);
					}
					else
					{
						slashVel = new Vector2(-12f, 0);
					}
					Owner.immune = true;
					Owner.immuneTime = 30;
					var lanternPlayer = Owner.GetModPlayer<LanternSword_player>();
					if (lanternPlayer is not null)
					{
						lanternPlayer.Active = true;
						lanternPlayer.OldPos = Main.screenPosition;
						lanternPlayer.LerpValue = 1f;
					}
					var trail = new LanternSwordTeleportEffect
					{
						Active = true,
						Visible = true,
						MaxTime = 30,
						Position_End = des,
						Position_Start = Owner.MountedCenter,
					};
					Ins.VFXManager.Add(trail);
					Owner.Center = des;
					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_teleport_1_Mod).WithPitchOffset(Main.rand.NextFloat(-0.3f, 0.3f)), Owner.Center);
							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_teleport_2_Mod).WithPitchOffset(Main.rand.NextFloat(-0.3f, 0.3f)), Owner.Center);
							break;
					}

					for (int i = 0; i < 24; i++)
					{
						var dust = new LanternSwordDust
						{
							Active = true,
							Visible = true,
							Position = Owner.Center + new Vector2(Main.rand.NextFloat(-45, 45), 0),
							Velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-6, 0)),
							Frame = Main.rand.Next(4),
							Scale = Main.rand.NextFloat(2f, 5f),
							MaxTime = Main.rand.Next(45, 75),
							RotateSpeed = Main.rand.NextFloat(-0.15f, 0.15f),
							VelocityRotateSpeed = Main.rand.NextFloat(-0.05f, 0.05f),
							Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						};
						Ins.VFXManager.Add(dust);
					}
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Owner.Center, slashVel, ModContent.ProjectileType<LanternSword_Slash>(), Projectile.damage, 2f, Projectile.owner);
					switch (Main.rand.Next(2))
					{
						case 0:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_dimensionslash_1_b_Mod), Owner.Center);
							break;
						case 1:
							SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_dimensionslash_2_b_Mod), Owner.Center);
							break;
					}
					NextTarget = null;
				}
			}
		}
	}

	public override void NewAttack()
	{
		CurrentAttackType++;
		if (Main.MouseWorld.X > Owner.MountedCenter.X)
		{
			Owner.direction = 1;
		}
		else
		{
			Owner.direction = -1;
		}

		Vector2 mouseDir = Main.MouseWorld - Owner.MountedCenter;
		mouseDir = mouseDir.SafeNormalize(Vector2.zeroVector);
		AttackTimer = 0;
		float meleeSpeed = Owner.meleeSpeed;
		Attack_RotativeSwing(mouseDir, meleeSpeed);

		switch (AttackStyle)
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_attack_1_Mod).WithPitchOffset(Main.rand.NextFloat(-0.3f, 0.3f)), Owner.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.redoiledpapersword_attack_2_Mod).WithPitchOffset(Main.rand.NextFloat(-0.3f, 0.3f)), Owner.Center);
				break;
		}
		AttackStyle++;
		if (AttackStyle >= 2)
		{
			AttackStyle = 0;
		}
		float itemUseTime = Owner.HeldItem.useTime;
		float maxTime = itemUseTime / 0.4f;
		AddSlashEffect(maxTime, itemUseTime, (int)(maxTime + 1));
	}

	public Vector2 TeleportDestination(NPC target)
	{
		if (target.velocity.X > 0)
		{
			return target.Left + new Vector2(-20, 0);
		}
		else
		{
			return target.Right + new Vector2(20, 0);
		}
	}

	public override void HitNPCVFX(float hitRotation, Vector2 hitPos)
	{
		LanternSwordHitStar lSHS = new LanternSwordHitStar();
		lSHS.Active = true;
		lSHS.Visible = true;
		lSHS.Position = hitPos;
		lSHS.Rotation = hitRotation;
		lSHS.Scale = 1f;
		lSHS.MaxTime = 12;
		Ins.VFXManager.Add(lSHS);

		for (int k = 0; k < 8; k++)
		{
			LanternSwordHitSpark lSHSp = new LanternSwordHitSpark();
			lSHSp.Active = true;
			lSHSp.Visible = true;
			lSHSp.Position = hitPos;
			lSHSp.Scale = Main.rand.NextFloat(0.1f, 0.3f);
			lSHSp.MaxTime = Main.rand.NextFloat(30, 60);
			lSHSp.Velocity = new Vector2(Main.rand.NextFloat(6, 48), 0).RotatedBy(hitRotation - MathHelper.PiOver2 + Main.rand.NextFloat(-0.5f, 0.5f));
			lSHSp.ai = [Main.rand.NextFloat(MathHelper.TwoPi)];
			Ins.VFXManager.Add(lSHSp);
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 3)
		{
			Color drawColor = new Color(1f, 0.7f, 0.1f, 0);
			drawColor *= factor * extraValue0;
			if (!SelfLuminous)
			{
				Color lightC = Lighting.GetColor(worldPos.ToTileCoordinates());
				drawColor.R = (byte)(lightC.R * drawColor.R / 255f);
				drawColor.G = (byte)(lightC.G * drawColor.G / 255f);
				drawColor.B = (byte)(lightC.B * drawColor.B / 255f);
			}
			drawColor *= 1.7f;
			float rot = (worldPos - Projectile.Center).ToRotation() - (float)Main.time * 0.01f;
			float factorHighlight = factor * 3;
			float mulValue = MathF.Max(MathF.Pow(Math.Max(0, 0.5f + 0.5f * MathF.Cos(rot * 2)), 16) * 0.4f * ReflectionSharpValue * MathF.Pow(extraValue0, 2), factorHighlight);
			drawColor *= mulValue;
			if (mulValue < 1f)
			{
				drawColor.G = drawColor.B = 0;
			}
			drawColor.A = 0;
			return drawColor;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.Defense *= 2f;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
		if (target.life <= 0)
		{
			float maxDis = 800;
			foreach (var npc in Main.npc)
			{
				if (npc is not null && npc.active && npc != target && !npc.boss)
				{
					if (npc.type == target.type)
					{
						Vector2 toNPC = Owner.Center - npc.Center;
						if (toNPC.Length() < maxDis)
						{
							maxDis = toNPC.Length();
							NextTarget = npc;
							NextTargetAvailableTimer = 180;
						}
					}
				}
			}
		}
	}

	public float MarkRotation = 0;

	public override void PostDraw(Color lightColor)
	{
		if (NextTarget is not null && NextTarget.active && NextTargetAvailableTimer > 0)
		{
			Vector2 des = TeleportDestination(NextTarget);
			if (!Collision.SolidCollision(des - new Vector2(8, 16), 16, 16))
			{
				Texture2D mark = ModAsset.LanternSword_NextTargetMark.Value;
				Texture2D mark_black = ModAsset.LanternSword_NextTargetMark_black.Value;
				float timeValue = (float)Main.timeForVisualEffects % 30;
				float fade = 1f;
				if (NextTargetAvailableTimer < 60)
				{
					fade *= NextTargetAvailableTimer / 60f;
				}
				float rot = 0;
				Color drawColor = new Color(1f, 0f, 0, 0.5f);
				if (timeValue < 10)
				{
					rot = -timeValue * 0.4f;
				}
				if (timeValue is >= 10 and < 20)
				{
					rot = timeValue * 0.01f + 0.1f;
				}
				MarkRotation = MarkRotation * 0.5f + rot * 0.5f;
				SpriteEffects spd = SpriteEffects.None;
				Vector2 origin = Vector2.zeroVector;
				int rotDir = 1;
				Vector2 moveVec = mark.Size() * 0.5f;
				if (NextTarget.velocity.X < 0)
				{
					spd = SpriteEffects.FlipHorizontally;
					origin = new Vector2(mark.Width, 0);
					rotDir = -1;
					moveVec.X *= -1;
				}
				Vector2 drawPos = NextTarget.Center - Main.screenPosition - moveVec;
				Main.EntitySpriteDraw(mark_black, drawPos, null, Color.White, MarkRotation * rotDir, origin, 0.75f, spd, 0);
				if (timeValue is >= 13 && timeValue < 18)
				{
					drawColor = Color.White;
					float moveValue = (timeValue - 12) / 6f * MathHelper.Pi;
					moveValue = MathF.Sin(moveValue);
					Main.EntitySpriteDraw(mark, drawPos + new Vector2(16 * moveValue, 0), null, drawColor * 0.3f, MarkRotation * rotDir, origin, 0.75f, spd, 0);
					Main.EntitySpriteDraw(mark, drawPos + new Vector2(-16f * moveValue, 0), null, drawColor * 0.3f, MarkRotation * rotDir, origin, 0.75f, spd, 0);
				}
				Main.EntitySpriteDraw(mark, drawPos, null, drawColor, MarkRotation * rotDir, origin, 0.75f, spd, 0);

				drawPos = NextTarget.Center;
				drawColor.A = 0;
				List<Vertex2D> bars = new List<Vertex2D>();
				List<Vertex2D> bars_b = new List<Vertex2D>();
				for (int i = 0; i <= 80; i++)
				{
					float range = NextTargetAvailableTimer;
					Vector2 radius = new Vector2(0, range).RotatedBy(i / 80f * MathHelper.TwoPi);
					Vector2 radius_out = new Vector2(0, range + (180 - range) * 0.5f).RotatedBy(i / 80f * MathHelper.TwoPi);
					float xCoord = i / 80f + NextTargetAvailableTimer / 80f;

					bars_b.Add(drawPos + radius, Color.White, new Vector3(xCoord, 0.6f, fade));
					bars_b.Add(drawPos + radius_out, Color.White, new Vector3(xCoord, 0.4f, fade));

					bars.Add(drawPos + radius, drawColor, new Vector3(xCoord, 0.6f, fade));
					bars.Add(drawPos + radius_out, drawColor, new Vector3(xCoord, 0.4f, fade));
				}
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
				SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Effect effect0 = ModAsset.WizardLantern_Thunder_Matrix_Shader.Value;
				effect0.Parameters["uTransform"].SetValue(model * projection);
				effect0.Parameters["size1"].SetValue(Vector2.One);
				effect0.CurrentTechnique.Passes[0].Apply();


				if (bars_b.Count > 0)
				{
					Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
					Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star_black.Value;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_b.ToArray(), 0, bars_b.Count - 2);
				}

				if (bars.Count > 0)
				{
					Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
					Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				}

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}
		base.PostDraw(lightColor);
	}

	public override void AddDust(Vector3 oldAxisTip, Vector3 oldAxisTail, Vector3 rotationAxis, float rotationSpeed, float trailFade)
	{
		float maxCount = Math.Abs(rotationSpeed) * 100;
		float rotSpeed = rotationSpeed / maxCount;
		for (int i = 0; i < maxCount; i++)
		{
			if (Main.rand.NextBool(10))
			{
				float randValue = MathF.Sqrt(Main.rand.NextFloat());
				var melee_dust = new MeleeProj_3D_Dust()
				{
					Active = true,
					Visible = true,
					Position_Space = oldAxisTip * randValue + oldAxisTail * (1 - randValue),
					MaxTime = Main.rand.NextFloat(30, 60),
					Scale = Main.rand.NextFloat(0.1f, 0.8f) * (randValue + 1f) * trailFade * 0.37f,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					RotSpeed = rotationSpeed * 0.05f * Main.rand.NextFloat(0.8f, 1.2f),
					RotAxis = rotationAxis,
					ParentProj = this,
					ai = new float[] { Main.rand.NextFloat(6) },
				};
				melee_dust.RegisterBehavior(CustomDustBehavior);
				melee_dust.RegisterDraw(CustomDustDraw);
				Ins.VFXManager.Add(melee_dust);
			}

			// Enchantment Effects
			EnchantmentDustEffect(oldAxisTip, oldAxisTail, rotationAxis, rotationSpeed, trailFade, Owner.meleeEnchant);
			if (Owner.magmaStone)
			{
				EnchantmentDustEffect(oldAxisTip, oldAxisTail, rotationAxis, rotationSpeed, trailFade, 3);
			}
			RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTip);
			RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTail);
		}
	}

	public override void CustomDustDraw(MeleeProj_3D_Dust dust)
	{
		Vector3 wldPos3D = dust.Position_Space + new Vector3(0, 0, CenterZ);
		Vector2 wldPos = Project(wldPos3D, ProjectionMatrix()) + Projectile.Center;
		Texture2D tex = ModAsset.LanternSwordDust.Value;
		var dustColor = SlashColor;
		if (!SelfLuminous)
		{
			Color lightC = Lighting.GetColor(wldPos.ToTileCoordinates());
			dustColor.R = (byte)(lightC.R * dustColor.R / 255f);
			dustColor.G = (byte)(lightC.G * dustColor.G / 255f);
			dustColor.B = (byte)(lightC.B * dustColor.B / 255f);
		}
		dustColor.A = 255;
		float colorVariation = MathF.Sin((dust.MaxTime - dust.Timer) * 1f) + 1;
		colorVariation *= 0.5f;
		colorVariation = MathF.Pow(colorVariation, 1);
		dustColor *= ReflectionSharpValue * 5f * colorVariation;
		Rectangle frame = new Rectangle((int)dust.ai[0] * 7, 0, 7, 7);
		Ins.Batch.Draw(tex, wldPos, frame, dustColor, dust.Rotation, frame.Size() * 0.5f, dust.Scale, SpriteEffects.None);
	}
}
