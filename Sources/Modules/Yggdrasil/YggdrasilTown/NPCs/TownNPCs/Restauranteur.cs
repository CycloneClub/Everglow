using Everglow.Commons.Coroutines;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class Restauranteur : TownNPC_LiveInYggdrasil
{
	public override string HeadTexture => ModAsset.Restauranteur_Head_Mod;

	public bool Attacking = false;

	public bool CanDespawn = false;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 15;
	}

	public override void SetDefaults()
	{
		StandFrame = new Rectangle(0, 0, 32, 56);
		SitFrame = new Rectangle(0, 784, 32, 56);
		NPC.frame = StandFrame;
		FrameHeight = 56;

		base.SetDefaults();
	}

	public override void CheckInSuitableArea()
	{
		if (!YggdrasilTownCentralSystem.InCanteen_YggdrasilTown())
		{
			CanDespawn = true;
			return;
		}
		CanDespawn = false;
		AnchorForBehaviorPos = new Point(220, 150);
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

	public override bool NeedSaving() => CanDespawn;

	public override bool CheckActive() => CanDespawn;

	public override void TryAttack()
	{
		if (ThreatenTarget >= 0 && ThreatenTarget < 200)
		{
			NPC npc = Main.npc[ThreatenTarget];
			Vector2 distance = npc.Center - NPC.Center;
			for (int i = 0; i < 8; i++)
			{
				Vector2 checkPos = npc.Center;
				switch (i)
				{
					case 0:
						checkPos = npc.TopLeft;
						break;
					case 1:
						checkPos = npc.Top;
						break;
					case 2:
						checkPos = npc.TopRight;
						break;
					case 3:
						checkPos = npc.Left;
						break;
					case 4:
						checkPos = npc.Right;
						break;
					case 5:
						checkPos = npc.BottomLeft;
						break;
					case 6:
						checkPos = npc.Bottom;
						break;
					case 7:
						checkPos = npc.BottomRight;
						break;
				}
				if ((checkPos - NPC.Center).Length() < distance.Length())
				{
					distance = checkPos - NPC.Center;
				}
			}
			if (Main.rand.NextBool())
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack()));
				return;
			}
			else
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack2()));
				return;
			}
		}
	}

	public IEnumerator<ICoroutineInstruction> Attack()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;
		var vel = (target.Center - NPC.Center).NormalizeSafe();
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<Rolle_Thrust>(), 20, 4, Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers, NPC.whoAmI).npcProj = true;
		for (int t = 0; t < 54; t++)
		{
			Attacking = true;
			NPC.velocity.X *= 0;
			yield return new SkipThisFrame();
		}
		Attacking = false;
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> Attack2()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-NPC.spriteDirection, 0), ModContent.ProjectileType<Rolle_Swing>(), 20, 4, Main.netMode == NetmodeID.SinglePlayer ? Main.myPlayer : Main.maxPlayers, NPC.whoAmI).npcProj = true;
		for (int t = 0; t < 42; t++)
		{
			NPC.velocity.X *= 0;
			Attacking = true;
			yield return new SkipThisFrame();
		}
		Attacking = false;
		EndAIPiece();
	}

	public override void WalkFrame()
	{
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 7 * FrameHeight)
		{
			NPC.frame.Y = 0;
		}
	}

	public override void CheckWalkBound()
	{
		if (!YggdrasilTownCentralSystem.InCanteen_YggdrasilTown())
		{
			return;
		}
		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X < AnchorForBehaviorPos.X - 90)
		{
			NPC.direction = 1;
		}
		if (npcTilePos.X > AnchorForBehaviorPos.X + 90)
		{
			NPC.direction = -1;
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!Attacking)
		{
			Texture2D texMain = ModAsset.Restauranteur.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height) * 0.5f;
			Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		else
		{
			Texture2D texMain = ModAsset.Restauranteur_Attack.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height) * 0.5f;
			Main.spriteBatch.Draw(texMain, drawPos, null, drawColor, NPC.rotation, texMain.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}
}
