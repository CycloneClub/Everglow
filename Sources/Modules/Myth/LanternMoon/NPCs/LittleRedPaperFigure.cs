using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.LanternMoon.Buffs;
using Everglow.Myth.LanternMoon.Gores;

namespace Everglow.Myth.LanternMoon.NPCs;

public class LittleRedPaperFigure : LanternMoonNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
	}

	public enum BehaviorState
	{
		Fighter,
		Fly,
		Attach,
		Teleporting,
	}

	public int State;

	public int Timer;

	public int ClosingTimeMax = 300;

	public Vector2 SummonPos = Vector2.zeroVector;

	public int TeleportTimer = 0;

	public int TeleportCooling = 0;

	public int MaxTeleportTime = 60;

	public override void SetDefaults()
	{
		NPC.friendly = false;
		NPC.damage = 25;
		NPC.lifeMax = 150;
		NPC.npcSlots = 1f;
		NPC.width = 38;
		NPC.height = 52;
		NPC.defense = 8;
		NPC.value = 200;
		NPC.aiStyle = NPCAIStyleID.Fighter;
		NPC.knockBackResist = 1.5f;
		NPC.dontTakeDamage = false;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 2f;
	}

	public override void FindFrame(int frameHeight)
	{
		Timer++;
		NPC.frame.Width = 52;
		NPC.frame.Height = 52;
		if (State == (int)BehaviorState.Fighter)
		{
			NPC.frameCounter++;
			NPC.frame.X = 0;
			Main.npcFrameCount[NPC.type] = 8;
			if (NPC.frameCounter >= 8)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += NPC.frame.Height;
				if (NPC.frame.Y >= NPC.frame.Height * Main.npcFrameCount[NPC.type])
				{
					NPC.frame.Y = 0;
				}
			}
		}
		else if (State == (int)BehaviorState.Fly)
		{
			if (Timer % 2 == 0)
			{
				NPC.frameCounter++;
			}
			NPC.frame.X = 52;
			Main.npcFrameCount[NPC.type] = 8;
			if (NPC.frameCounter >= 8)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += NPC.frame.Height;
				if (NPC.frame.Y >= NPC.frame.Height * Main.npcFrameCount[NPC.type])
				{
					NPC.frame.Y = 0;
				}
			}
		}
		else if (State == (int)BehaviorState.Attach)
		{
			NPC.frame.X = 104;
			NPC.frame.Y = 0;
			NPC.frameCounter = 0;
		}
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.aiStyle = -1;
			NPC.noTileCollide = true;
			NPC.velocity.Y += 0.1f;
			return;
		}

		NPC.TargetClosest();
		if (NPC.target == -1)
		{
			return;
		}
		Player player = Main.player[NPC.target];
		NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
		Lighting.AddLight(NPC.Center, 0.4f, 0.05f, 0.05f);
		if (State == (int)BehaviorState.Fighter)
		{
			NPC.ai[0] = 0;
			NPC.aiStyle = NPCAIStyleID.Fighter; // 几千行代码有点难扒下来，先用现成的AIStyle代替，但看了一下把ai[0]设好能避免大部分问题
			NPC.noGravity = false;
			NPC.rotation = 0;
		}
		else if (State == (int)BehaviorState.Fly)
		{
			NPC.ai[0] = 1;
			NPC.aiStyle = NPCAIStyleID.StarCell;
			NPC.noGravity = true;
			NPC.rotation = 0;
		}
		else if (State == (int)BehaviorState.Attach)
		{
			NPC.ai[0] = 0;
			NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
			NPC.knockBackResist = 0f;
			NPC.Center = player.Center - Vector2.UnitY * 9 - (player.direction == 1 ? 2 * Vector2.UnitX : -2 * Vector2.UnitX);
			NPC.noGravity = true;
			NPC.rotation = 0;
		}
		else if (State == (int)BehaviorState.Teleporting)
		{
			TeleportTimer--;
			if (TeleportTimer < 0)
			{
				NPC.aiStyle = NPCAIStyleID.Fighter;
				State = (int)BehaviorState.Fighter;
			}
			NPC.velocity *= 0;
			if (TeleportTimer == 30)
			{
				Teleport(player);
			}
			if (Main.rand.NextBool(3))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireSparkDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = NPC.Center + new Vector2(Main.rand.NextFloat(-35f, 35f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(37, 45),
					Scale = Main.rand.NextFloat(0.1f, 12.0f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
			}
			if (Main.rand.NextBool(3))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = NPC.Center + new Vector2(Main.rand.NextFloat(-30f, 30f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(5, 15),
					Scale = Main.rand.NextFloat(0.1f, 6.0f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
			}
			NPC.rotation = 0;
		}
		Vector2 toTarget = player.Center - NPC.Center;
		if (toTarget.Length() > 1000 && TeleportCooling <= 0)
		{
			TeleportTimer = MaxTeleportTime;
			TeleportCooling = Main.rand.Next(260, 300);
			State = (int)BehaviorState.Teleporting;
		}
		if (TeleportCooling > 0)
		{
			TeleportCooling--;
		}
	}

	public void Teleport(Player player)
	{
		for (int i = 0; i < 1000; i++)
		{
			Vector2 checkPoint = player.Center + new Vector2(i + 300, 0).RotatedBy(Math.PI * (Math.Sin(i * 0.5f + NPC.whoAmI + Main.time) + 1) / 2);
			if (!Collision.SolidCollision(checkPoint - NPC.Size * 0.5f, NPC.width, NPC.height))
			{
				for (int j = 0; j < 50; j++)
				{
					checkPoint += new Vector2(0, 10);
					if (Collision.SolidCollision(checkPoint - NPC.Size * 0.5f, NPC.width, NPC.height))
					{
						if ((checkPoint - player.Center).Length() >= 300)
						{
							NPC.Center = checkPoint + new Vector2(0, 0);
							NPC.rotation = 0;
							return;
						}
					}
				}
			}
		}
	}

	public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (State == (int)BehaviorState.Fighter)
		{
			State = (int)BehaviorState.Fly;
		}
	}

	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		if (State == (int)BehaviorState.Fighter)
		{
			State = (int)BehaviorState.Fly;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit)
	{
		if (State == (int)BehaviorState.Fighter)
		{
			State = (int)BehaviorState.Fly;
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		if (State == (int)BehaviorState.Attach)
		{
			target.AddBuff(ModContent.BuffType<PaperObstructed>(), 59);
		}
		if (State != (int)BehaviorState.Attach)
		{
			State = (int)BehaviorState.Attach;
		}
	}

	public float OverTileHeight(Vector2 pos)
	{
		float value = 0f;
		int checkCount = 0;
		while (Collision.IsWorldPointSolid(pos + new Vector2(0, value)))
		{
			value -= 8;
			checkCount++;
			if (checkCount > 100)
			{
				return -9999999;
			}
		}
		if (checkCount > 0)
		{
			return value + 8;
		}
		while (!Collision.IsWorldPointSolid(pos + new Vector2(0, value)))
		{
			value += 8;
			checkCount++;
			if (checkCount > 100)
			{
				return 9999999;
			}
		}
		return value - 8;
	}

	public override void OnKill()
	{
		for (int g = 0; g < 8; g++)
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 5f, 0).RotatedByRandom(MathHelper.TwoPi);
				string texturePath = ModAsset.LittleRedPaperFigure_Gore_0_Mod;
				if (texturePath is not null)
				{
					texturePath = texturePath.Remove(texturePath.Length - 1, 1);
					texturePath += g;
				}
				var gore = new PaperGore
				{
					LightValue = 0.15f,
					Velocity = vel,
					Position = NPC.Center + vel,
					Texture = ModContent.Request<Texture2D>(texturePath).Value,
					RotateSpeed = vel.X / 8f,
					Scale = Main.rand.NextFloat(1f, 1.25f),
					MaxTime = Main.rand.Next(120, 360),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					ai = new float[] { Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-MathF.PI, MathF.PI) },
				};
				Ins.VFXManager.Add(gore);
			}
		}
		base.OnKill();
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Player target = Main.player[NPC.target];
		NPC.spriteDirection = NPC.direction;
		Texture2D texture = ModAsset.LittleRedPaperFigure.Value;
		if (State != (int)BehaviorState.Attach && !target.creativeGodMode)
		{
			if (State == (int)BehaviorState.Teleporting)
			{
				drawColor *= (float)Math.Abs(TeleportTimer - MaxTeleportTime / 2) / (MaxTeleportTime / 2);
				spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			}
			else
			{
				spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			}
		}
		if (State == (int)BehaviorState.Attach)
		{
			spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, target.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}

		return false;
	}
}
