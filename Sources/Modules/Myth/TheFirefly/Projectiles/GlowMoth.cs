using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Items.Accessories;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowMoth : ModProjectile
{
	FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 34;
		Projectile.tileCollide = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 720;
		Projectile.friendly = true;
		Projectile.minionSlots = 1;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.minion = true; //这玩意会捆绑武器Item的伤害
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
	}

	public override void AI()
	{
		UpdateDrawParameter();

		FindEnemies();

		CheckKill();

		CheckPlayerDistance();

		CheckRecoverMagic();

		Projectile.damage = (int)(Projectile.damage * Power);
	}

	private int SpecialTimeAfterSpawn = 60;
	private Vector2 TargetPos = Vector2.Zero;
	private int AutoAddingTimer = 0;
	private float[] OldRotation = new float[12];
	private int[] OldFrame = new int[12];
	private bool SleepOutside = false;
	private float Power = 0.25f;

	private void UpdateDrawParameter()
	{
		Player player = Main.player[Projectile.owner];
		MothOwner mothOwner = player.GetModPlayer<MothOwner>();

		if (mothOwner.WhoSleepInPlayer[player.whoAmI] == Projectile.whoAmI)
		{
			Projectile.spriteDirection = -player.direction;
			Projectile.rotation = -1.0f * player.direction + player.fullRotation;
			Projectile.frame = 1;
			return;
		}
		if (SleepOutside)
		{
			Projectile.spriteDirection = -player.direction;
			Projectile.rotation = -1.0f * player.direction + player.fullRotation;
			Projectile.frame = 1;
			return;
		}
		Projectile.rotation *= 0.9f;
		AutoAddingTimer++;
		if ((AutoAddingTimer + (int)Projectile.ai[1]) % 4 == 0)
		{
			if (Projectile.frame < 3)
				Projectile.frame++;
			else
			{
				Projectile.frame = 0;
			}
		}
		OldRotation[0] = Projectile.rotation;
		for (int i = 11; i > 0; i--)
		{
			OldRotation[i] = OldRotation[i - 1];
		}
		OldFrame[0] = Projectile.frame;
		if (Math.Abs(Projectile.velocity.X) > 0.3f)
			Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
		for (int i = 11; i > 0; i--)
		{
			OldFrame[i] = OldFrame[i - 1];
		}
		if (SpecialTimeAfterSpawn == 60)
		{
			if (Main.rand.NextFloat(0, 10f) > 5)
				Projectile.spriteDirection = -1;
			else
			{
				Projectile.spriteDirection = 1;
			}
		}
		if (SpecialTimeAfterSpawn > 0)
		{
			Projectile.velocity *= 0.92f;
			SpecialTimeAfterSpawn--;
		}
	}

	private void CheckKill()
	{
		Player player = Main.player[Projectile.owner];
		if (player.dead || !player.active)
		{
			player.ClearBuff(ModContent.BuffType<GlowMothBuff>());
			Projectile.Kill();
		}
		if (player.HasBuff(ModContent.BuffType<GlowMothBuff>()))
			Projectile.timeLeft = 2;
		else
		{
			Projectile.Kill();
		}
	}

	private void CheckPlayerDistance()
	{
		Player player = Main.player[Projectile.owner];
		if ((player.MountedCenter - Projectile.Center).Length() > 2000)
		{
			Projectile.Center = player.MountedCenter + new Vector2(0, Main.rand.Next(-80, -20)).RotatedBy(Main.rand.NextFloat(-1.8f, 1.8f));
			for (int i = 0; i < 18; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.6f);
			}
			for (int i = 0; i < 6; i++)
			{
				int index = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f));
				Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
				Main.dust[index].noGravity = true;
			}
		}
	}

	private void FindEnemies()
	{
		Player player = Main.player[Projectile.owner];
		MothOwner mothOwner = player.GetModPlayer<MothOwner>();
		bool flag = false;
		for (int j = 0; j < 200; j++)
		{
			if (Main.npc[j].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[j].Center, 1, 1))
			{
				Vector2 v0 = Main.npc[j].Center;
				Vector2 v1 = Projectile.Center;

				if ((v0 - v1).Length() < 600)
				{
					flag = true;
					TargetPos = v0;
					break;
				}
			}
		}
		for (int j = 0; j < 200; j++)
		{
			if (player.MinionAttackTargetNPC == j)
			{
				Vector2 v0 = Main.npc[j].Center;
				flag = true;
				TargetPos = v0;
				break;
			}
		}
		if (flag/* && (AutoAddingTimer + (int)Projectile.ai[1]) % 20 >= 10*/)
		{
			Vector2 v0 = TargetPos - Projectile.Center;
			Vector2 v1 = new Vector2(0, Projectile.ai[0] / 2f + 120f).RotatedBy(Projectile.ai[1] + Main.time * (0.03 + Projectile.ai[0] / 12000d));
			Vector2 v2 = v0 + v1;
			if ((AutoAddingTimer + (int)Projectile.ai[0]) % 72 == 0)
			{
				var p = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, v0.SafeNormalize(Vector2.Zero) * 3, ModContent.ProjectileType<BlackCorruptRainFriendly>(), (int)(Projectile.damage * Power * 4), Projectile.knockBack, Projectile.owner);
				p.CritChance = Projectile.CritChance;
				p.friendly = true;
				if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
				{
					if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
					{
						p.tileCollide = false;
						if (Main.rand.NextBool(10))
						{
							var p3 = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, v0.SafeNormalize(Vector2.Zero) * 3, ModContent.ProjectileType<BlackCorruptRain3Friendly>(), (int)(Projectile.damage * Power * 5), Projectile.knockBack, Projectile.owner);
							p3.CritChance = Projectile.CritChance;
							p3.friendly = true;
						}
					}
					else
					{
						p.tileCollide = true;
					}
				}
			}
			Projectile.velocity = (Projectile.velocity * 10f + v2 / v2.Length() * 9f) / 11f;
			mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
			if (Power >= 0.0125f)
				Power -= 0.0003f;
		}
		if (!flag)
		{
			NoFindAnyEmeny();
			if (Power <= 0.25f)
				Power += 0.002f;
		}
	}

	private void CheckRecoverMagic()
	{
		foreach (Projectile p in Main.projectile)
		{
			if (p.active)
			{
				if (p.type == ModContent.ProjectileType<MothMagicArray>())
				{
					if ((p.Center - Projectile.Center).Length() < 300)
					{
						if (Power <= 0.25f)
							Power += 0.001f;
					}
				}
			}
		}
	}

	private void NoFindAnyEmeny()
	{
		Player player = Main.player[Projectile.owner];
		MothOwner mothOwner = player.GetModPlayer<MothOwner>();
		//SleepInPlayer
		Vector2 PlayerBody = player.TopLeft + player.fullRotationOrigin + new Vector2(-20 * player.direction, -32).RotatedBy(player.fullRotation);
		if (player.mount._type == -1)
			PlayerBody = player.Hitbox.Center() + new Vector2(-16 * player.direction, -0);
		if (mothOwner.WhoSleepInPlayer[player.whoAmI] < 0/*此时没有飞停留在玩家身上*/)
		{
			Vector2 v = player.MountedCenter + ProduceFlyTrace(Projectile.ai[0]) - Projectile.Center;
			Vector2 v1 = PlayerBody - Projectile.Center;
			if (v1.Length() < 10)
				mothOwner.WhoSleepInPlayer[player.whoAmI] = Projectile.whoAmI;
			Projectile.velocity = (Projectile.velocity * 10f + v / v.Length() * 5f) / 11f;
		}
		else if (mothOwner.WhoSleepInPlayer[player.whoAmI] == Projectile.whoAmI/*停留在玩家身上的正是本蛾*/)
		{
			if (player.velocity.Length() < 6f && Math.Abs(player.fullRotation) < 0.3)
				Projectile.Center = PlayerBody;
			else
			{
				mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
			}
		}
		else/*其他的飞蛾*/
		{
			Vector2 v = player.MountedCenter + ProduceFlyTrace(Projectile.ai[0]) - Projectile.Center;

			Projectile.velocity = (Projectile.velocity * 10f + v / v.Length() * 5f) / 11f;
			if (!Main.projectile[mothOwner.WhoSleepInPlayer[player.whoAmI]].active || Main.projectile[mothOwner.WhoSleepInPlayer[player.whoAmI]].type != Projectile.type)
				mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
		}

		//CheckSleepInPlayer
		if (mothOwner.WhoSleepInPlayer[player.whoAmI] >= 0)
		{
			if (!Main.projectile[mothOwner.WhoSleepInPlayer[player.whoAmI]].active)
			{
				mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
				return;
			}
			if (Main.projectile[mothOwner.WhoSleepInPlayer[player.whoAmI]].type != Projectile.type)
			{
				mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
				return;
			}
		}
	}

	private Vector2 ProduceFlyTrace(float baseValue)
	{
		float k0 = Projectile.ai[1] * 0.05f;
		return new Vector2((float)Math.Sin(baseValue + Main.time / (30d + k0)) * 150, (float)Math.Sin(baseValue + Main.time / (120d + 4 * k0)) * 40 - 40);
	}

	private void ProduceDust()
	{
		if (AutoAddingTimer % 144 == 0)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MothBlue>(), Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.6f, 0.8f));
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0.3f, 1.4f)).RotatedByRandom(Math.PI);

			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpookyWood, v.X + Projectile.velocity.X, v.Y + Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.3f, 0.7f));
		}
		if (AutoAddingTimer % 144 == 72)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MothBlue2>(), Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.6f, 0.8f));
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0.3f, 1.4f)).RotatedByRandom(Math.PI);

			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpookyWood, v.X + Projectile.velocity.X, v.Y + Projectile.velocity.Y, 0, default, Main.rand.NextFloat(0.3f, 0.7f));
		}
	}

	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		MothOwner mothOwner = player.GetModPlayer<MothOwner>();
		if (player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()] == 1)
			mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;
		if (mothOwner.WhoSleepInPlayer[player.whoAmI] == Projectile.whoAmI)
			mothOwner.WhoSleepInPlayer[player.whoAmI] = -1;

		if (timeLeft != 0)
		{
			for (int i = 0; i < 2; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpookyWood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.6f);
			}
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		MothOwner mothOwner = player.GetModPlayer<MothOwner>();
		int Length = Projectile.oldPos.Length;
		int iStart = 1;
		if (mothOwner.WhoSleepInPlayer[player.whoAmI] == Projectile.whoAmI)
		{
			iStart = 0;
			Length = 1;
		}
		for (int i = iStart; i < Length; i++)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Texture2D Gtexture = ModAsset.GlowMothGlow.Value;
			Vector2 DrawPos = Projectile.oldPos[i] + new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects sf = SpriteEffects.None;
			float AddRotation = 0;
			if (Projectile.spriteDirection == -1)
				sf = SpriteEffects.FlipHorizontally;
			if (player.gravDir == -1)
				sf = SpriteEffects.FlipVertically;
			if (Projectile.spriteDirection == -1 && player.gravDir == -1)
			{
				sf = SpriteEffects.None;
				AddRotation = (float)Math.PI;
			}

			var DrawRect = new Rectangle(0, OldFrame[i] * Projectile.height, Projectile.width, Projectile.height);
			float kColor = (Length - i + 1) / 2.5f * Power;
			var Draworigin = new Vector2(texture.Width / 2f, texture.Height / 8f);

			//Main.spriteBatch.Draw(texture, DrawPos, DrawRect, new Color(c0.R * kColor / 255f, c0.G * kColor / 255f, c0.B * kColor / 255f, kColor), OldRotation[i], Draworigin, Projectile.scale, sf, 0);
			if (mothOwner.WhoSleepInPlayer[player.whoAmI] != Projectile.whoAmI)
				Main.spriteBatch.Draw(Gtexture, DrawPos, DrawRect, new Color(kColor, kColor, kColor, 0), OldRotation[i] + AddRotation, Draworigin, Projectile.scale, sf, 0);
			if (i == 0)
			{
				DrawRect = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
				Main.spriteBatch.Draw(Gtexture, Projectile.Center - Main.screenPosition, DrawRect, new Color(1f, 1f, 1f, 0), Projectile.rotation + AddRotation, Draworigin, Projectile.scale, sf, 0);
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return true;
	}
}

internal class MothOwner : ModPlayer
{
	public int[] WhoSleepInPlayer = new int[256];

	public override void OnEnterWorld()
	{
		WhoSleepInPlayer[Player.whoAmI] = -1;
	}

	public override void PostUpdate()
	{
		if (Player.ownedProjectileCounts[ModContent.ProjectileType<GlowMoth>()] == 0)
			WhoSleepInPlayer[Player.whoAmI] = -1;
		//Player.fullRotation = (float)(Main.time / 100d);
	}
}
