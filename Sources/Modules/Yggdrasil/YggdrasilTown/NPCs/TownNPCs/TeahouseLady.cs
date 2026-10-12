using Everglow.Commons.Coroutines;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]

// Schorl, The Teahouse Lady
public class TeahouseLady : TownNPC_LiveInYggdrasil, INPCQuestIconFrameProvider
{
	public bool Attacking0 = false;
	public int Attack0Target = -1;
	public int Attack1Target = -1;

	public override string HeadTexture => ModAsset.TeahouseLady_Head_Mod;

	// The sheet also contains sitting/attack frames and separate arms; the icon loops only walking frames.
	public Rectangle GetQuestIconFrame(int animationFrame) => new(0, animationFrame % 10 * 56, 48, 56);

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();
		StandFrame = new Rectangle(0, 0, 48, 56);
		SitFrame = new Rectangle(0, 560, 48, 56);
		NPC.frame = StandFrame;
		FrameHeight = 56;
	}

	public override void CheckInSuitableArea()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return;
		}
		AnchorForBehaviorPos = YggdrasilTownCentralSystem.TownTopLeftWorldCoord.ToTileCoordinates() + new Point(405, 157);
		bool safe = false;
		var homePoint = AnchorForBehaviorPos;
		NPC.homeless = false;
		NPC.homeTileX = homePoint.X;
		NPC.homeTileY = homePoint.Y;

		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X > AnchorForBehaviorPos.X - 100 && npcTilePos.X < AnchorForBehaviorPos.X + 100)
		{
			if (npcTilePos.Y > AnchorForBehaviorPos.Y - 20 && npcTilePos.Y < AnchorForBehaviorPos.Y + 20)
			{
				safe = true;
			}
		}
		if (!safe)
		{
			TeleportHome();
		}
	}

	public override void WalkFrame()
	{
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 9 * FrameHeight)
		{
			NPC.frame.Y = 0;
		}
	}

	public bool CanHitNPCLine(NPC target)
	{
		bool canHit = true;
		var toTarget = target.Center - NPC.Center;
		float distance = toTarget.Length() / 6f;
		if (distance < 2)
		{
			return true;
		}
		var step = toTarget.NormalizeSafe() * 6f;
		var checkPos = NPC.Center;
		for (int i = 0; i < distance; i++)
		{
			checkPos += step;
			if (Collision.SolidCollision(checkPos - new Vector2(8), 16, 16))
			{
				canHit = false;
				break;
			}
		}
		return canHit;
	}

	public override void TryAttack()
	{
		float index = Main.rand.NextFloat(100);
		if (ThreatenTarget is >= 0 and < 200)
		{
			NPC npc = Main.npc[ThreatenTarget];
			if (npc != null && npc.active)
			{
				// Total damage calculate.
				if (npc.life > 300)
				{
					index = 100;
				}
			}
		}
		if (index < 75)
		{
			if (CanAttack0())
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack0()));
			}
			else if (CanAttack1())
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack1()));
			}
		}
		else
		{
			if (CanAttack1())
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack1()));
			}
			else if (CanAttack0())
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack0()));
			}
		}
	}

	public bool CanAttack0()
	{
		float minDis = 1000;
		Attack0Target = -1;
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter && npc != NPC)
			{
				Vector2 distance = npc.Center - NPC.Center;
				if (distance.Length() < minDis && CanHitNPCLine(npc))
				{
					minDis = distance.Length();
					if (npc.Center.X > NPC.Center.X)
					{
						NPC.direction = 1;
					}
					else
					{
						NPC.direction = -1;
					}
					NPC.spriteDirection = NPC.direction;
					Attack0Target = npc.whoAmI;
				}
			}
		}
		if (Attack0Target != -1)
		{
			return true;
		}
		return false;
	}

	public bool CanAttack1()
	{
		float minDis = 450;
		Attack1Target = -1;
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter && npc != NPC)
			{
				Vector2 distance = npc.Center - NPC.Center;
				if (distance.Length() < minDis)
				{
					minDis = distance.Length();
					if (npc.Center.X > NPC.Center.X)
					{
						NPC.direction = 1;
					}
					else
					{
						NPC.direction = -1;
					}
					NPC.spriteDirection = NPC.direction;
					Attack1Target = npc.whoAmI;
				}
			}
		}
		if (Attack1Target != -1)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Laser; None-Inheritable
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> Attack0()
	{
		if (Attack0Target == -1)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[Attack0Target];
		if (target == null || !CanHitNPCLine(target))
		{
			EndAIPiece();
			yield break;
		}
		NPC.spriteDirection = NPC.direction;
		Attacking0 = true;
		for (int t = 0; t < 180; t++)
		{
			NPC.frame = new Rectangle(0, 672, 48, FrameHeight);
			Vector2 toTarget = target.Center - NPC.Center;
			if (toTarget.X > 0)
			{
				NPC.direction = 1;
			}
			else
			{
				NPC.direction = -1;
			}
			if (t == 0)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).NormalizeSafe() * 6, ModContent.ProjectileType<Schorl_Laser>(), 150, 1.2f, Main.myPlayer, target.whoAmI);
			}
			NPC.spriteDirection = NPC.direction;
			NPC.velocity *= 0;
			if (target == null || !target.active)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		Attacking0 = false;
		NPC.frame = new Rectangle(0, 0, 48, FrameHeight);
		yield return new WaitForFrames(16);
		EndAIPiece();
	}

	/// <summary>
	/// Explosion; None-Inheritable
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> Attack1()
	{
		if (Attack1Target == -1)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[Attack1Target];
		if (target == null)
		{
			EndAIPiece();
			yield break;
		}
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<Schorl_Mark>(), 0, 0, Main.myPlayer, target.whoAmI);
		for (int t = 0; t < 141; t++)
		{
			Vector2 toTarget = target.Center - NPC.Center;
			if (toTarget.X > 0)
			{
				NPC.direction = 1;
			}
			else
			{
				NPC.direction = -1;
			}
			if (t >= 20 && t % 20 == 0)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<Schorl_Explosion>(), 30, 0.15f, Main.myPlayer, target.whoAmI);
			}
			NPC.spriteDirection = NPC.direction;
			NPC.velocity *= 0;
			if (target == null || !target.active)
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.direction *= -1;
		NPC.spriteDirection = NPC.direction;
		NPC.frame = new Rectangle(0, 0, 48, FrameHeight);
		EndAIPiece();
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texMain = ModAsset.TeahouseLady.Value;
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height) * 0.5f;
		Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

		if (Attacking0)
		{
			Rectangle armFrame = new Rectangle(0, 730, 8, 18);
			if (NPC.spriteDirection == 1)
			{
				armFrame = new Rectangle(10, 730, 8, 18);
			}
			if (Attack0Target is >= 0 and < 200)
			{
				NPC target = Main.npc[Attack0Target];

				// float rotationAttack = (Main.MouseWorld - NPC.Center).ToRotationSafe() - MathHelper.PiOver2;
				float rotationAttack = (target.Center - NPC.Center).ToRotationSafe() - MathHelper.PiOver2;
				Main.spriteBatch.Draw(texMain, drawPos + new Vector2(0, -2), armFrame, drawColor, rotationAttack, new Vector2(4, 4), NPC.scale, SpriteEffects.None, 0);

				// Texture2D mark = ModAsset.Schorl_Mark.Value;
				// Main.EntitySpriteDraw(mark, target.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), target.rotation, mark.Size() * 0.5f, target.scale, SpriteEffects.None, 0);
			}
		}

		// Point checkPoint = (NPC.Bottom + new Vector2(8 * NPC.direction, 8)).ToTileCoordinates() + new Point(NPC.direction, -1);
		// Tile tile = Main.tile[checkPoint];
		// Texture2D block = Commons.ModAsset.TileBlock.Value;
		// Main.spriteBatch.Draw(block, checkPoint.ToWorldCoordinates() - Main.screenPosition, null, new Color(1f, 0f, 0f, 0.5f), 0, block.Size() * 0.5f, 1, SpriteEffects.None, 0);
		return false;
	}

	public override void FindFrame(int frameHeight)
	{
		base.FindFrame(frameHeight);
	}
}
