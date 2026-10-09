namespace Everglow.Myth.TheFirefly.NPCs.Bosses;

public class SummonedButterfly : ModNPC
{
	public Vector2 targetPos;

	private int timer
	{
		get => (int)NPC.ai[1];
		set => NPC.ai[1] = value;
	}

	private NPC Owner => Main.npc[(int)NPC.ai[3]];

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 4;
	}

	public override void SetDefaults()
	{
		NPC.width = 36;
		NPC.height = 34;
		NPC.friendly = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.lifeMax = 1;
		NPC.aiStyle = -1;
		NPC.damage = 20;
		// if (Main.expertMode)
		// {
		//    NPC.damage = 40;
		// }
		// if (Main.masterMode)
		// {
		//    NPC.damage = 60;
		// }
		if (Main.getGoodWorld)
		{
			NPC.lifeMax = 3;
			NPC.defense = 9999;
			// NPC.damage = 80;
		}

		NPC.dontTakeDamageFromHostiles = true;
	}

	public override bool? CanFallThroughPlatforms()
	{
		return true;
	}

	private void CheckOwnerActive()
	{
		if (Owner.active && Owner.type == ModContent.NPCType<CorruptMoth>())
		{
		}
		else
		{
			NPC.active = false;
			NPC.life = 0;
		}
	}

	public override void AI()
	{
		if (NPC.ai[0] == -2)
		{
			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			NPC.dontTakeDamage = false;
			timer++;
			if (NPC.alpha > 0)
			{
				NPC.alpha -= 2;
			}

			NPC.ai[2] += 0.1f;
			if (Vector2.Distance(NPC.Center, Owner.Center) > 80)
			{
				MoveTo(Owner.Center + NPC.ai[2].ToRotationVector2() * 200, 20, 20);
			}

			NPC.friendly = false;
			CheckOwnerActive();
		}
		if (NPC.ai[0] == -1)
		{
			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			NPC.dontTakeDamage = true;
			timer++;
			if (NPC.alpha < 120)
			{
				NPC.alpha += 2;
			}

			if (Vector2.Distance(NPC.Center, Owner.Center) > 300)
			{
				MoveTo(Owner.Center + Main.rand.NextVector2Unit() * 100, 10, 20);
				NPC.netUpdate2 = true;
			}

			NPC.friendly = true;
			CheckOwnerActive();
		}// 在boss附近游荡
		if (NPC.ai[0] == 0)
		{
			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			NPC.TargetClosest(false);
			Player player = Main.player[NPC.target];
			if (timer < 30)
				NPC.dontTakeDamage = true;
			else
			{
				NPC.dontTakeDamage = false;
			}

			if (timer == 0)
			{
				NPC.ai[2] = Main.rand.Next(60, 200);
				NPC.frame.Y = Main.rand.Next(3) * 34;
				NPC.netUpdate2 = true;
			}
			if (++timer > NPC.ai[2] && timer < NPC.ai[2] + 350)// 追踪玩家
			{
				NPC.TargetClosest(false);
				MoveTo(player.Center, 8, 80);
			}
			if (timer > NPC.ai[2] + 600)
			{
				NPC.scale -= 0.05f;
				if (NPC.scale < 0)
				{
					NPC.HitEffect(1);
					NPC.active = false;
				}
			}
		}// 延迟后朝玩家运动
		if (NPC.ai[0] == 1)// 弓,rot:ai2
		{
			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			Vector2 trueTargetPos = Owner.Center + targetPos.RotatedBy(NPC.ai[2]);
			CheckOwnerActive();

			Player player = Main.player[Owner.target];
			if (timer == 0)
			{
				NPC.dontTakeDamage = true;
				NPC.ai[2] = -1.57f;
				NPC.velocity = Vector2.Zero;
			}
			if (++timer < 60)
			{
				NPC.alpha += 2;
				NPC.friendly = true;
				MoveTo(trueTargetPos, 10, 40);
			}
			if (timer is > 60 and < 120)
			{
				NPC.alpha -= 2;
				NPC.ai[2] = NPC.ai[2].AngleLerp(Owner.DirectionTo(player.Center).ToRotation(), 0.1f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.1f);
			}
			if (timer > 120)
			{
				NPC.friendly = false;
				NPC.ai[2] = NPC.ai[2].AngleLerp(Owner.DirectionTo(player.Center + player.velocity * 20).ToRotation(), 0.15f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.4f);
			}
		}
		if (NPC.ai[0] == 2)// 箭,rot:ai2，distance:localAI0
		{
			CheckOwnerActive();

			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			Vector2 trueTargetPos = Owner.Center + targetPos.RotatedBy(NPC.ai[2]);
			Player player = Main.player[Owner.target];
			if (timer == 0)
			{
				NPC.dontTakeDamage = true;
				NPC.ai[2] = -1.57f;
				NPC.localAI[0] = 0;
				NPC.velocity = Vector2.Zero;
			}
			if (++timer < 60)
			{
				NPC.alpha += 2;
				NPC.friendly = true;
				MoveTo(trueTargetPos, 10, 40);
			}
			if (timer is > 60 and < 120)
			{
				NPC.alpha -= 2;
				NPC.ai[2] = NPC.ai[2].AngleLerp(Owner.DirectionTo(player.Center).ToRotation(), 0.1f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.1f);
			}
			if (timer is > 120 and < 160)
			{
				NPC.friendly = false;
				Vector2 d = Owner.DirectionTo(player.Center + player.velocity * 20);
				NPC.ai[2] = NPC.ai[2].AngleLerp(d.ToRotation(), 0.2f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.4f) + d * NPC.localAI[0];
				NPC.localAI[0] = MathHelper.Lerp(NPC.localAI[0], -60, 0.05f);
			}
			if (timer == 160)
			{
				NPC.velocity = NPC.ai[2].ToRotationVector2() * 30;
			}

			if (timer == 220)
			{
				NPC.velocity *= 0.5f;
				NPC.velocity += Main.rand.NextVector2Unit() * 10;
				NPC.friendly = true;
				NPC.netUpdate2 = true;
			}
			if (timer > 240)
			{
				timer = 0;
			}
		}
		if (NPC.ai[0] == 3)// 剑
		{
			CheckOwnerActive();

			Vector2 trueTargetPos = Owner.Center + targetPos.RotatedBy(NPC.ai[2]);
			if (timer == 0)
			{
				NPC.friendly = true;
				NPC.dontTakeDamage = true;
				NPC.ai[2] = -1.57f;
				NPC.velocity = Vector2.Zero;
			}
			if (++timer < 60)
			{
				NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, 120, 0.1f);
				MoveTo(trueTargetPos, 10, 40);
				NPC.localAI[0] = Owner.spriteDirection * -1;
			}
			if (timer is > 60 and < 120)
			{
				NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, 0, 0.1f);
				NPC.ai[2] = NPC.ai[2].AngleLerp(-1.57f, 0.1f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.15f);
			}
			if (timer is > 120 and < 180)
			{
				NPC.friendly = false;
				NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, 0, 0.1f);
				NPC.ai[2] = NPC.ai[2].AngleLerp(-1.57f + NPC.localAI[0] * 1f, 0.05f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.5f);
			}
			if (timer is > 180 and < 220)
			{
				NPC.ai[2] -= NPC.localAI[0] * 0.12f;
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.5f);
			}
			if (timer == 220)
			{
				NPC.velocity = Main.rand.NextVector2Unit() * 10f;
				NPC.netUpdate2 = true;
			}
			if (timer > 250)
			{
				NPC.ai[0] = -1;
				timer = 0;
			}
		}
		if (NPC.ai[0] == 4)// 拳
		{
			CheckOwnerActive();

			NPC.spriteDirection = Math.Sign(NPC.velocity.X);
			Vector2 trueTargetPos = Owner.Center + targetPos.RotatedBy(NPC.ai[2]);
			Player player = Main.player[Owner.target];
			if (timer == 0)
			{
				NPC.dontTakeDamage = true;
				NPC.ai[2] = 0f;
				NPC.localAI[0] = 0;
				NPC.velocity = Vector2.Zero;
			}
			if (++timer < 60)
			{
				NPC.alpha += 2;
				NPC.friendly = true;
				MoveTo(trueTargetPos, 10, 40);
			}
			if (timer is > 60 and < 120)
			{
				NPC.alpha -= 2;
				NPC.ai[2] = NPC.ai[2].AngleLerp(Owner.DirectionTo(player.Center).ToRotation(), 0.1f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.1f);
			}
			if (timer is > 120 and < 160)
			{
				NPC.friendly = false;
				Vector2 d = Owner.DirectionTo(player.Center);
				NPC.ai[2] = NPC.ai[2].AngleLerp(d.ToRotation(), 0.2f);
				NPC.Center = Vector2.Lerp(NPC.Center, trueTargetPos, 0.4f) + d * NPC.localAI[0];
				NPC.localAI[0] = MathHelper.Lerp(NPC.localAI[0], -70, 0.05f);
			}
			if (timer == 160)
			{
				NPC.velocity = NPC.ai[2].ToRotationVector2() * 40;
			}

			if (timer == 200)
			{
				NPC.velocity = Main.rand.NextVector2Unit() * Main.rand.Next(2, 10);
				timer = 0;
				NPC.ai[0] = 0;
				NPC.dontTakeDamage = false;
				NPC.netUpdate2 = true;
			}
		}
	}

	private void MoveTo(Vector2 targetPos, float Speed, float n)
	{
		Vector2 targetVec = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * Speed;
		NPC.velocity = (NPC.velocity * n + targetVec) / (n + 1);
	}

	public override void FindFrame(int frameHeight)
	{
		if (NPC.frame.Y > 102)
		{
			NPC.frame.Y = 0;
		}

		if (timer % 10 == 0)
		{
			NPC.frame.Y += 34;
		}

		if (timer % 16 == 0 && NPC.alpha < 20)
		{
			int index = Dust.NewDust(NPC.position - new Vector2(8), NPC.width, NPC.height, ModContent.DustType<Dusts.BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.7f, 1.9f));
			Main.dust[index].velocity = NPC.velocity * 0.5f;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int i = 0; i < 6; i++)
		{
			int index = Dust.NewDust(NPC.position - new Vector2(8), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
			Main.dust[index].noGravity = true;
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(0.2f, 0.5f, 1f, 0) * (1 - NPC.alpha / 255f);
	}
}
