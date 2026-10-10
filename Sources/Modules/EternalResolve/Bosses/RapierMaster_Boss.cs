using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.EternalResolve.Bosses.Projectiles;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords;
using Terraria.Audio;
using Terraria.GameContent;

namespace Everglow.EternalResolve.Bosses;

public class RapierMaster_Boss : ModNPC
{
	// public override string Texture => "Terraria/Images/NPC_0";
	public override void SetDefaults()
	{
		NPC.boss = true;
		NPC.friendly = false;
		NPC.width = 24;
		NPC.height = 50;
		NPC.lifeMax = 2000;
		NPC.defense = 30;
		NPC.aiStyle = -1;
		NPC.npcSlots = 80;
		NPC.knockBackResist = 0f;
		NPC.value = Item.buyPrice(0, 1, 0, 0);
		NPC.lavaImmune = true;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.buffImmune[BuffID.Confused] = true;
		NPC.damage = 50;
		AIStates = new();
		aiScore = new int[100];
	}

	public List<AIState> AIStates;

	public Player Player => Main.player[NPC.target];

	public int T
	{
		set => NPC.ai[1] = value;
		get => (int)NPC.ai[1];
	}

	private Vector2 tPos;

	#region #治疗相关内容
	private int healCD = 1800;

	private void HealAI()
	{
		if (healCD > 0)
		{
			healCD--;
		}

		if (NPC.lifeMax - NPC.life > 150)
		{
			if (healCD == 0)
			{
				NPC.life += 200;
				CombatText.NewText(NPC.Hitbox, CombatText.HealLife, 150);
				healCD = 1800;
				SoundEngine.PlaySound(SoundID.Item3, NPC.Center);
			}
		}
	}
	#endregion

	#region #格挡相关内容
	private int parry = 0;
	private int noAI = 0;
	private int parryTextCD = 90;

	public void ParryEffect()
	{
		if (parryTextCD == 0)
		{
			CombatText.NewText(NPC.Hitbox, new Color(0.1f, 1f, 1f), "格挡！");
			parryTextCD = 90;

			// TODO : Localization Needed
		}
		noAI = 10;
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSpark_MetalStabDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center,
				MaxTime = Main.rand.Next(1, 25),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(10f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		modifiers.ModifyHitInfo += (ref NPC.HitInfo i) =>
		{
			if (parry > 0)// 如果能挡
			{
				if (projectile.penetrate != 1)
				{
					parry -= 2;
				}
				else
				{
					parry -= 1;
				}

				projectile.penetrate--;
				i.Damage = 1;
				i.Crit = false;
				SoundEngine.PlaySound(SoundID.NPCHit4, NPC.Center);
				ParryEffect();
			}
		};
	}

	public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		modifiers.ModifyHitInfo += (ref NPC.HitInfo i) =>
		{
			if (parry > 0)
			{
				parry -= 2;
				i.Damage = 1;
				i.Crit = false;
				SoundEngine.PlaySound(SoundID.NPCHit4, NPC.Center);
				ParryEffect();
			}
		};
	}
	#endregion

	/// <summary>
	/// 当一个ai未被使用时会增加此ai的score，每次选择ai时会根据score调整概率
	/// </summary>
	private int[] aiScore;

	public void SwitchAIP1()
	{
		T = 0;
		NPC.ai[2] = 0;
		NPC.ai[3] = 0;
		parry = 0;
		if (NPC.ai[0] == 0)
		{
			for (int i = 1; i < 3; i++)
			{
				if (Main.rand.Next(5) < aiScore[i])
				{
					NPC.ai[0] = i;
					goto SetScore;
				}
			}

			NPC.ai[0] = Main.rand.Next(1, 4);

			// NPC.ai[0] = 2;
		}
		else
		{
			// ai2:走路时间
			switch (NPC.ai[0])
			{
				case 1:
					NPC.ai[2] = Main.rand.Next(20, 60);
					break;
				case 2:
					NPC.ai[2] = Main.rand.Next(40, 100);
					break;
				case -3:
					NPC.ai[2] = Main.rand.Next(30, 80);
					break;
				default:
					NPC.ai[2] = Main.rand.Next(40, 100);
					break;
			}
			NPC.ai[0] = 0;
		}
	SetScore:
		if (NPC.ai[0] != 0)
		{
			for (int i = 1; i <= 3; i++)
			{
				aiScore[i]++;
			}
			aiScore[(int)NPC.ai[0]] = 0;
		}
	}

	public override void AI()
	{
		HealAI();
		if (parryTextCD > 0)
		{
			parryTextCD--;
		}
		else
		{
			parryTextCD = 0;
		}

		if (noAI == 0)
		{
			NPC.maxFallSpeed = 2333f;
			NPC.TargetClosest();
			UpdateAIState();
			bool dashing = AIStates.Any(i => i is DashAI);
			if (NPC.velocity.Y == 0 && !dashing)
			{
				NPC.velocity.X *= 0.95f;
			}

			Point p = NPC.Center.ToTileCoordinates();
			Tile tile = Main.tile[p.X, p.Y + 1];
			if (tile.active() && Main.tileSolid[tile.type])
			{
				NPC.position.Y -= 16f;
			}

			if (NPC.ai[0] == 0)// 走路
			{
				NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (NPC.Center.X < Player.Center.X ? 1 : -1) * Main.rand.NextFloat(2f, 5f), 0.05f);
				UpdateDirection();
				if (T == 0)
				{
					parry = (int)(NPC.ai[2] / 10);
				}
				if (++T > NPC.ai[2])
				{
					SwitchAIP1();
				}
			}
			if (NPC.ai[0] == 1)// 跳两下
			{
				NPC.ai[3] = Terraria.Utils.AngleLerp(NPC.ai[3], NPC.DirectionTo(Player.Center).ToRotation(), 0.02f);

				if (++T < 50)
				{
					NPC.velocity *= 0.9f;
				}

				if (T == 25)
				{
					NPC.ai[3] = NPC.DirectionTo(Player.Center).ToRotation();
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrutchRapier_Hostile>(), NPC.damage / 8, 0f, Main.myPlayer, NPC.whoAmI);
					}
				}
				if (T == 30 || T == 50)
				{
					UpdateDirection();
					NPC.ai[3] = NPC.DirectionTo(Player.Center).ToRotation();
					Vector2 velocity = NPC.DirectionTo(Player.Center);
					velocity.Y -= 0.2f;
					velocity.Y *= 1.2f;
					if (T == 30)
					{
						Dash(velocity * 18, 20);
					}
					if (T == 50)
					{
						Dash(velocity * 15, 20);
					}
				}
				if (T == 80 && Main.rand.NextBool(2))
				{
					T = 0;
					NPC.ai[0] = -3;
				}
				if (T > 100)
				{
					SwitchAIP1();
				}
			}
			if (NPC.ai[0] == 2)// 后撤，长冲
			{
				Vector2 velocity = NPC.DirectionTo(Player.Center);
				velocity.Y = 0;
				velocity.Normalize();
				NPC.ai[2] = velocity.X;
				if (T++ == 0)
				{
					UpdateDirection();
					float y = NPC.Center.Y - Player.Center.Y > 120 ? -Main.rand.Next(8, 15) : 0;
					Dash(-velocity * 6 + new Vector2(0, y), 20);
				}
				if (T == 25)
				{
					Dash(velocity * 15, 15);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, ModContent.ProjectileType<CrutchRapier_Stab_Hostile>(), NPC.damage / 6, 0f, Main.myPlayer, NPC.whoAmI);
					}
				}
				if (T > 25 && T < 40)
				{
					for (int i = 0; i < 3; i++)
					{
						NPC.velocity = Collision.TileCollision(NPC.position, NPC.velocity, NPC.width, NPC.height);
						NPC.position += NPC.velocity;
					}
				}
				if (T > 50)
				{
					SwitchAIP1();
				}
			}
			if (NPC.ai[0] == -3)// 戳一下
			{
				UpdateDirection();
				if (T++ == 0)
				{
					Vector2 velocity = NPC.DirectionTo(Player.Center) * 8;
					velocity.Y -= 1;
					Dash(velocity, 40);
				}
				if (T == 30)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Player.Center - Player.velocity * 5), ModContent.ProjectileType<CrutchRapier_Stab_Hostile>(), NPC.damage / 6, 0f, Main.myPlayer, NPC.whoAmI, 1f);
					}
				}
				if (T > 40)
				{
					SwitchAIP1();
				}
			}
			if (NPC.ai[0] == 3)// 标记
			{
				if (++T < 20)
				{
					NPC.velocity *= 0.9f;
				}
				if (T == 20)
				{
					Dash(new Vector2(0, -10), 60);
					Vector2 vec = new Vector2(Player.velocity.X * 40, -Main.rand.NextFloat(0f, 120f));
					tPos = Player.Center + vec;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), Player.Center + vec, Vector2.zeroVector, ModContent.ProjectileType<Rapier_Slash>(), NPC.damage / 6, 0f, Main.myPlayer, NPC.whoAmI, 1f);
					}
				}
				if (T == 80)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (tPos - NPC.Center) * 0.002f, ModContent.ProjectileType<CrutchRapier_Stab_Hostile>(), NPC.damage / 6, 0f, Main.myPlayer, NPC.whoAmI, 1f);
					}
					NPC.noGravity = true;
				}
				if (T > 120)
				{
					NPC.noGravity = false;
					SwitchAIP1();
				}
			}
			if (NPC.ai[0] == 1111)// 下砸,没做好
			{
				if (T++ == 0)
				{
					Vector2 tPos = new Vector2(Player.Center.X + Player.velocity.X * 30, NPC.Center.Y - 400);
					Jump(NPC.DirectionTo(tPos) * Vector2.Distance(NPC.Center, tPos) * 0.06f, 30);
				}
				if (T < 30)
				{
					NPC.velocity.Y += 0.2f;
				}

				if (T > 30 && T < 60)
				{
					NPC.velocity.X *= 0.9f;
					if (NPC.velocity.Y != 0 && NPC.velocity.Y < 30)
					{
						NPC.velocity.Y += 2;
					}
				}
				if (T > 60)
				{
					SwitchAIP1();
				}
			}
		}
		else
		{
			NPC.position -= NPC.velocity;
			noAI--;
			if (noAI < 0)
			{
				noAI = 0;
			}
		}
	}

	public void WalkToPlayer(float speed, float n = 0.5f)
	{
		NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (NPC.Center.X < Player.Center.X ? 1 : -1) * speed, n);
	}

	public void UpdateDirection()
	{
		NPC.direction = NPC.spriteDirection = NPC.Center.X < Player.Center.X ? 1 : -1;
	}

	public void UpdateAIState()
	{
		for (int i = 0; i < AIStates.Count; i++)
		{
			AIState aIState = AIStates[i];
			aIState.Update(NPC);
			aIState.Timer++;
			if (aIState.Timer > aIState.MaxTime)
			{
				aIState.OnRemove(NPC);
				AIStates.Remove(aIState);
			}
		}
	}

	public void AddAIState(AIState a)
	{
		AIStates.Add(a);
		a.OnActive(NPC);
	}

	public void Dash(Vector2 velocity, int maxTime)
	{
		DashAI dash = new DashAI()
		{
			Velocity = velocity,
			MaxTime = maxTime,
		};
		AddAIState(dash);
	}

	public void Jump(Vector2 velocity, int maxTime)
	{
		JumpAI ai = new JumpAI()
		{
			Velocity = velocity,
			MaxTime = maxTime,
		};
		AddAIState(ai);
	}

	public class DashAI : AIState
	{
		public Vector2 Velocity = Vector2.UnitX;

		public override void Update(NPC npc)
		{
			if (Timer < MaxTime * 0.4f)
			{
				for (int i = 0; i < 4; i++)
				{
					Dust d = Dust.NewDustDirect(npc.Center - new Vector2(15) + new Vector2(0, 10), 30, 30, DustID.Smoke, 0, 0, 0, default, 1.6f);
					d.noGravity = true;
				}
			}

			if (Timer < MaxTime * 0.3f)
			{
				npc.noGravity = true;
				npc.velocity = Velocity;
			}
			else
			{
				npc.noGravity = false;
				npc.velocity *= 0.95f;
			}
		}
	}

	public class JumpAI : AIState
	{
		public Vector2 Velocity = Vector2.UnitX;

		public override void Update(NPC npc)
		{
			if (Timer == 0)
			{
				npc.velocity.Y = Velocity.Y;
			}

			if (Timer < MaxTime * 0.6f)
			{
				npc.velocity.X = MathHelper.Lerp(npc.velocity.X, Velocity.X, 0.05f);
			}
		}
	}

	public class AIState
	{
		public int Timer = 0;
		public int MaxTime = 0;

		public virtual void OnActive(NPC npc)
		{
		}

		public virtual void Update(NPC npc)
		{
		}

		public virtual void OnRemove(NPC npc)
		{
		}
	}

	public override bool CanHitPlayer(Player target, ref int cooldownSlot)
	{
		return false;
	}

	public override bool CanHitNPC(NPC target)
	{
		return false;
	}

	public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
	{
		NPC.lifeMax = (int)(2000 * balance * bossAdjustment);
		NPC.damage = 50;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>("Everglow/EternalResolve/Bosses/RapierMaster_Boss").Value;
		Main.spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

		if (noAI > 0)
		{
			int type = ModContent.ItemType<CrutchBayonet>();
			Texture2D itemTexture = TextureAssets.Item[type].Value;
			Main.spriteBatch.Draw(itemTexture, NPC.Center + new Vector2(20, 0) * NPC.spriteDirection - Main.screenPosition, null, drawColor, 0, itemTexture.Size() / 2f, 1, NPC.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
		}
		return false;
	}
}
