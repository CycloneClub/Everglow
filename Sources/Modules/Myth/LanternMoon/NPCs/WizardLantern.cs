using Everglow.Commons.DataStructures;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.NPCs;

public class WizardLantern : LanternMoonNPC
{
	public int Timer;

	public int SkillTime;

	public Vector2 SkillReleasePosition;

	public override void SetDefaults()
	{
		NPC.damage = 88;
		NPC.lifeMax = 1605;
		NPC.npcSlots = 2.5f;
		NPC.width = 50;
		NPC.height = 40;
		NPC.defense = 8;
		NPC.value = 0;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.7f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 50f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		SkillTime = Main.rand.Next(450, 550);
	}

	public override void AI()
	{
		UpdateCloakRope();
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			return;
		}
		Timer++;
		NPC.TargetClosest();
		if (NPC.target == -1)
		{
			NPC.velocity *= 0;
			return;
		}
		Player player = Main.player[NPC.target];
		NPC.rotation = Math.Clamp(NPC.velocity.X / 10f, -0.7f, 0.7f);
		if (Timer == 1)
		{
			int signRandom = Main.rand.NextBool() ? 1 : -1;
			SkillReleasePosition = player.Center + new Vector2(0, Main.rand.NextFloat(-420, -330)).RotatedBy(Main.rand.NextFloat(0.8f, 1.6f) * signRandom);
		}
		if (Timer < 150)
		{
			Vector2 toTarget = SkillReleasePosition - NPC.Center - NPC.velocity;
			if (toTarget.Length() > 80)
			{
				toTarget = toTarget.NormalizeSafe() * 4;
			}
			else
			{
				toTarget = toTarget / 20f;
			}
			NPC.velocity = toTarget * 0.1f + NPC.velocity * 0.9f;
		}
		if (Timer > 150 && Timer < 160)
		{
			NPC.velocity *= 0.9f;
		}
		if (Timer == 160)
		{
			NPC.velocity *= 0;
			switch (Main.rand.Next(3))
			{
				case 0:
					Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<WizardLantern_Matrix_Thunder>(), 0, 0f, -1);
					WizardLantern_Matrix_Thunder wLMT = p0.ModProjectile as WizardLantern_Matrix_Thunder;
					if (wLMT is not null)
					{
						wLMT.OwnerNPC = NPC;
					}
					break;
				case 1:
					Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<WizardLantern_Matrix_Curse>(), 0, 0f, -1);
					WizardLantern_Matrix_Curse wLMC = p1.ModProjectile as WizardLantern_Matrix_Curse;
					if (wLMC is not null)
					{
						wLMC.OwnerNPC = NPC;
					}
					break;
				case 2:
					Projectile p2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<WizardLantern_Matrix_Witching>(), 0, 0f, -1);
					WizardLantern_Matrix_Witching wLMW = p2.ModProjectile as WizardLantern_Matrix_Witching;
					if (wLMW is not null)
					{
						wLMW.OwnerNPC = NPC;
					}
					break;
			}
		}
		if (Timer > 300)
		{
			float timeValue = Timer + NPC.whoAmI;
			timeValue *= 0.03f;
			NPC.velocity = new Vector2(MathF.Sin(timeValue), MathF.Cos(timeValue * 2));
		}
		if (Timer > 500)
		{
			Timer = 0;
		}
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc != NPC)
			{
				if (npc.type == ModContent.NPCType<NPCs.LanternGhostKing.LanternGhostKing>())
				{
					Vector2 v0 = NPC.Center - npc.Center;
					if (v0.Length() < 400)
					{
						NPC.velocity += Vector2.Normalize(v0) * 2.5f;
						break;
					}
				}
			}
		}
	}

	public override void OnKill()
	{
		for (int k = 0; k < 16; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 16f)).RotatedByRandom(MathHelper.TwoPi);
			var thunderSpark = new ThunderSpellDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Collided = false,
				MaxTime = Main.rand.Next(45, 80),
				Scale = Main.rand.NextFloat(1.5f, 6.2f),
			};
			Ins.VFXManager.Add(thunderSpark);
		}
		for (int k = 0; k < 12; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 9f)).RotatedByRandom(MathHelper.TwoPi);
			var CurseSpark = new CurseSpellDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Collided = false,
				MaxTime = Main.rand.Next(45, 80),
				Scale = Main.rand.NextFloat(0.5f, 1.2f),
			};
			Ins.VFXManager.Add(CurseSpark);
		}
		for (int k = 0; k < 8; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 9f)).RotatedByRandom(MathHelper.TwoPi);
			var CurseSpark = new CurseSpellDust_Blue_Normal
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Collided = false,
				MaxTime = Main.rand.Next(25, 30),
				Scale = 0,
			};
			Ins.VFXManager.Add(CurseSpark);
		}

		for (int k = 0; k < 16; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var CurseSpark = new CurseSpellSmoke_Normal
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				MaxTime = Main.rand.Next(60, 125),
				Scale = Main.rand.NextFloat(20f, 40f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { Main.rand.NextFloat(0f, 1f), 0 },
			};
			Ins.VFXManager.Add(CurseSpark);
		}
		for (int g = 0; g < 15; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.WizardLanternGore_0_Mod;
			if (texturePath is not null)
			{
				texturePath = texturePath.Remove(texturePath.Length - 1, 1);
				texturePath += g;
			}
			var gore = new NormalGore
			{
				Velocity = vel,
				Position = NPC.Center + vel,
				Texture = ModContent.Request<Texture2D>(texturePath).Value,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Main.rand.NextFloat(0.8f, 1.2f),
				MaxTime = Main.rand.Next(300, 340),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(gore);
		}
		for (int k = 0; k < 16; k++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var WitchingSpark = new WitchingSpellDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) + newVelocity * 3,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.4f, 0.4f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.02f, 0.05f) * (k % 2 - 0.5f) * 2,
				MaxTime = Main.rand.Next(45, 80),
				Scale = Main.rand.NextFloat(1.5f, 2f),
			};
			Ins.VFXManager.Add(WitchingSpark);
		}
	}

	public Rope LanternCloak = null;
	public static MassSpringContainer WizardLanternMassSpringSystem = new MassSpringContainer();
	public static PBDSolver WizardLanternPBDSolver = new PBDSolver(8);

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = ModAsset.WizardLantern_Body.Value;
		float mulColor = (255 - NPC.alpha) / 255f;
		SpriteEffects effects = SpriteEffects.None;
		NPC.spriteDirection = NPC.direction;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		Vector2 bodyOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.7f);
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor * mulColor, NPC.rotation, bodyOrigin, NPC.scale, effects, 0);
		DrawCloak(spriteBatch);
		Lighting.AddLight(NPC.Center, new Vector3(1f, 0.6f, 0.4f) * mulColor);
		return false;
	}

	public void UpdateCloakRope()
	{
		if (LanternCloak == null)
		{
			LanternCloak = Rope.Create_Fixed_StartPos(Main.MouseWorld, 8, 2f, 0.4f, 4f);
			WizardLanternMassSpringSystem.AddMassSpringMesh(LanternCloak);
		}
		LanternCloak.Masses[0].Position = NPC.Center + new Vector2(0, 4).RotatedBy(NPC.rotation);
		for (int i = 1; i < LanternCloak.Masses.Length; i++)
		{
			float damping = 0.3f;
			float toEnd = LanternCloak.Masses.Length - i - 1;
			if (toEnd < 3)
			{
				damping += (1 - toEnd / 3f) * 0.1f;
			}
			float toStart = i - 1;
			if (toStart < 3)
			{
				damping *= toStart / 3f;
			}
			LanternCloak.ApplyForceSpecial(i, -LanternCloak.Masses[i].Velocity * damping);
			LanternCloak.ApplyForceSpecial(i, new Vector2(0, 1.6f * LanternCloak.Masses[i].Value));
		}
		bool shouldUpdateRope = true;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active && npc.type == Type && npc.whoAmI < NPC.whoAmI)
			{
				shouldUpdateRope = false;
				break;
			}
		}
		if (shouldUpdateRope)
		{
			WizardLanternPBDSolver.Step(WizardLanternMassSpringSystem, 1f);
		}
	}

	public void DrawCloak(SpriteBatch spriteBatch)
	{
		if (LanternCloak == null || LanternCloak.Masses.Length <= 0)
		{
			return;
		}
		Texture2D tex = ModAsset.WizardLantern_Cloak.Value;
		float effectWidth = 48f;
		float alphaFade = (255 - NPC.alpha) / 255f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < LanternCloak.Masses.Length; i++)
		{
			float value = i;
			value /= LanternCloak.Masses.Length - 1;
			Vector2 drawPos = LanternCloak.Masses[i].Position - Main.screenPosition;
			Vector2 directionVec = new Vector2(0, 1);
			if (i >= 1)
			{
				directionVec = LanternCloak.Masses[i].Position - LanternCloak.Masses[i - 1].Position;
			}
			else if (LanternCloak.Masses.Length > 1)
			{
				directionVec = LanternCloak.Masses[i + 1].Position - LanternCloak.Masses[i].Position + new Vector2(0, 3);
			}
			if (directionVec.Length() > 200)
			{
				return;
			}
			directionVec = directionVec.NormalizeSafe();
			Vector2 directionLeft = directionVec.RotatedBy(MathHelper.PiOver2) * effectWidth / 2f;
			bars.Add(drawPos + directionLeft, Lighting.GetColor(LanternCloak.Masses[i].Position.ToTileCoordinates()) * alphaFade, new Vector3(0, value, 0));
			bars.Add(drawPos - directionLeft, Lighting.GetColor(LanternCloak.Masses[i].Position.ToTileCoordinates()) * alphaFade, new Vector3(1, value, 0));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}
}
