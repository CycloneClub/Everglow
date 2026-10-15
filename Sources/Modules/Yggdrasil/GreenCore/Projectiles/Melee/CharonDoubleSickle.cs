using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.GreenCore.Projectiles.Melee;

public class CharonDoubleSickle : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] += 500;
	}

	public override void SetDefaults()
	{
		Projectile.width = 66;
		Projectile.height = 68;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.timeLeft = 30;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;

		Projectile.extraUpdates = 1;
		oldPos = new Vector2[15];
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
		Projectile.DamageType = DamageClass.MeleeNoSpeed;
	}

	private Vector2[] oldPos;

	private Player Player => Main.player[Projectile.owner];

	private Vector2 PlayerMouseWorld => Player.MouseWorld();

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[2] = 100;
	}

	public override bool? CanDamage()
	{
		return !(Projectile.ai[1] == 0 && Vector2.Distance(Projectile.Center, Player.Center) < 30);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		if (Projectile.hide)
		{
			overPlayers.Add(index);
		}
	}

	private bool canSpawnHitEff = false;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// 命中特效
		if (canSpawnHitEff)
		{
			FogVFX fog = MEACVFX.Create<FogVFX>(Projectile.Center, Main.rand.NextVector2Circular(2, 2) + Projectile.velocity * 0.1f, 0);

			// fog.substract = true;
			fog.DrawColor = new Color(0.15f, 0.3f, 0.2f, 0f);
			fog.SetTimeLeft(90);
			fog.Scale = 1.5f * Projectile.scale;

			fog = MEACVFX.Create<FogVFX>(Projectile.Center, Main.rand.NextVector2Circular(2, 2) + Projectile.velocity * 0.1f, 0);
			fog.Subtract = true;
			fog.DrawColor = new Color(0.6f, 0.2f, 0.1f, 1f);
			fog.SetTimeLeft(70);
			fog.Scale = 1.2f * Projectile.scale;
			canSpawnHitEff = false;
		}
		SoundStyle sound = SoundID.NPCDeath44;
		sound.Volume = 0.2f;
		sound.Pitch = -0.2f;
		SoundEngine.PlaySound(sound, Projectile.Center); // 命中音效
	}

	public override void AI()
	{
		canSpawnHitEff = true;

		int speed = 45; // 初速度

		float rangeFactor = 0.20f; // 偏移角度

		float acc = 0.01f; // 回拉加速度

		int prepareTime = 25; // 冷却时间

		Projectile.rotation = SafeDirectionTo(Player, Projectile.Center).ToRotation();
		Projectile.spriteDirection = 1;
		Projectile.ownerHitCheck = true; // 不穿墙
		Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.4f, 0.1f));
		if (Player.channel)
		{
			Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - 1.5f);
			Projectile.timeLeft++;
		}
		else
		{
			Projectile.ai[0] = 1;
		}
		for (int i = oldPos.Length - 1; i > 0; i--)
		{
			oldPos[i] = oldPos[i - 1];
		}
		oldPos[0] = Projectile.Center;
		if (Projectile.ai[0] == 1)
		{
			Projectile.velocity *= 0.8f;
			Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center, 0.5f);
			return;
		}

		// if (Main.myPlayer == Projectile.owner)
		{
			ref float timer = ref Projectile.ai[2];
			if (Projectile.ai[1] == 0)// 收回
			{
				Projectile.velocity *= 0.9f;
				Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center, 0.28f);

				// if (Vector2.Distance(projectile.Center, player.Center) < 50)
				{
					if (timer++ > prepareTime)// 切换到攻击
					{
						if (Main.rand.NextBool(15))// 随机的阴间音效
						{
							SoundStyle sound = SoundID.NPCDeath52;
							sound.Volume = 0.15f;
							sound.Pitch = -Main.rand.NextFloat(0, 0.5f);
							SoundEngine.PlaySound(sound, Projectile.Center);
						}
						SoundEngine.PlaySound(SoundID.Item71.WithPitchOffset(Main.rand.NextFloat(0, 0.3f)), Projectile.Center); // 使用音效

						Projectile.ai[1] = Main.rand.Next(1, 3);
						timer = 0;
					}
				}
			}
			if (Projectile.ai[1] == 1)
			{
				Projectile.spriteDirection = 1;
				Player.direction = PlayerMouseWorld.X > Player.Center.X ? 1 : -1;

				if (timer++ == 0)
				{
					Projectile.velocity = Player.DirectionTo(PlayerMouseWorld).RotatedBy(-rangeFactor) * speed + Player.velocity;
				}
				if (timer > 0 && timer < 30)
				{
					Projectile.velocity += acc * SafeDirectionTo(Projectile, Player.Center).RotatedBy(-rangeFactor * 0.7f) * Vector2.Distance(Projectile.Center, Player.Center);
				}
				if (timer > 30)
				{
					Projectile.ai[1] = 0;
					timer = 0;
				}
			}
			if (Projectile.ai[1] == 2)
			{
				Projectile.spriteDirection = -1;
				Player.direction = PlayerMouseWorld.X > Player.Center.X ? 1 : -1;
				if (timer++ == 0)
				{
					Projectile.velocity = Player.DirectionTo(PlayerMouseWorld).RotatedBy(rangeFactor) * speed + Player.velocity;
				}
				if (timer > 0 && timer < 30)
				{
					Projectile.velocity += acc * SafeDirectionTo(Projectile, Player.Center).RotatedBy(rangeFactor * 0.7f) * Vector2.Distance(Projectile.Center, Player.Center);
				}
				if (timer > 30)
				{
					Projectile.ai[1] = 0;
					timer = 0;
				}
			}
		}
		if (Projectile.ai[1] > 0 && !Main.rand.NextBool(3))// 粒子
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
			d.velocity = Projectile.velocity * 0.5f;
			d.noGravity = true;
		}
	}

	public Vector2 SafeDirectionTo(Entity entity, Vector2 t)
	{
		return t - entity.Center != Vector2.Zero ? Vector2.Normalize(t - entity.Center) : Vector2.Zero;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		float rot = Projectile.rotation + 0.78f + (Projectile.spriteDirection == 1 ? 0 : 1.57f);

		float chainScale = 1.4f;
		Vector2 chainCenter = Projectile.Center + new Vector2(-33, 34).RotatedBy(Projectile.rotation + 0.78f) * Projectile.scale;
		Vector2 chainEnd = Player.Center + Projectile.rotation.ToRotationVector2() * 20;
		Vector2 chainVec = (chainEnd - chainCenter).SafeNormalize(Vector2.Zero);

		for (float i = 0; i < Vector2.Distance(chainCenter, chainEnd); i += 12 * chainScale)
		{
			Texture2D chain = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
			Vector2 drawPos = chainCenter + chainVec * i;
			Color l = Color.White * 0.8f;
			Main.spriteBatch.Draw(chain, drawPos - Main.screenPosition, null, l, chainVec.ToRotation(), new Vector2(0, chain.Height / 2), chainScale, 0, 0);
		}
		for (int i = 0; i < oldPos.Length; i++)
		{
			Color c = Color.White;
			float alpha = (1 - (float)i / oldPos.Length) * Main.rand.NextFloat(0.0f, 0.35f);
			c.A = 150;
			c *= alpha;
			Main.spriteBatch.Draw(tex, oldPos[i] - Main.screenPosition, null, c, rot, tex.Size() / 2, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, rot, tex.Size() / 2, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		return false;
	}
}
