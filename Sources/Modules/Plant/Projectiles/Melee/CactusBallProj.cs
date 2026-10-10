using Everglow.Commons.MEAC;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Plant.Buffs;
using Everglow.Plant.Common;
using Everglow.Plant.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace Everglow.Plant.Projectiles.Melee;

public class CactusBallProj : ModProjectile, IWarpProjectile
{
	public override string Texture => "Terraria/Images/Projectile_727";

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
	}

	public override void SetDefaults()
	{
		Projectile.width = Projectile.height = 32;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.extraUpdates = 1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProjOverHand;
		SetDef();
		trailVecsUp = new Queue<Vector2>(trailLength + 1);
		trailVecsDown = new Queue<Vector2>(trailLength + 1);
	}

	internal int trailLength = 16;

	internal Queue<Vector2> trailVecsUp;

	internal Queue<Vector2> trailVecsDown;

	internal float SpinAcc = 0f;

	internal int SwirlTime = 0;

	public void SetDef()
	{
	}

	public override void AI()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.Default;
		Player player = Main.player[Projectile.owner];

		Vector2 Radial = Projectile.Center;
		Vector2 mainVecUp = Radial;
		Vector2 mainVecDown = Radial * Projectile.scale * 0.25f;

		trailVecsUp.Enqueue(mainVecUp);
		if (trailVecsUp.Count > trailLength)
		{
			trailVecsUp.Dequeue();
		}

		trailVecsDown.Enqueue(mainVecDown);
		if (trailVecsDown.Count > trailLength)
		{
			trailVecsDown.Dequeue();
		}

		if (Projectile.owner == 255)
		{
			Projectile.ai[1] = 1f;
		}

		if (Projectile.ai[1] != 0f)
		{
			Projectile.ignoreWater = false;
			Projectile.ai[1] += 1f;
			if (Projectile.ai[1] >= 25f)
			{
				if (Projectile.ai[1] == 25f)
				{
					SoundEngine.PlaySound(SoundID.NPCHit11);
				}

				Projectile.aiStyle = ProjAIStyleID.Boulder;
				return;
			}
		}
		if (Projectile.ai[1] > 10f)
		{
			Projectile.tileCollide = true;
		}

		if (!player.active || player.dead || player.CCed || !player.channel)
		{
			if (Projectile.ai[1] == 0f)
			{
				Vector2 v0 = Utils.ToRotationVector2(MathHelper.ToRadians(Projectile.ai[0]));
				v0.Y /= 3f;
				Vector2 v1 = player.Center + v0 * 72f;
				v0 = Utils.ToRotationVector2(MathHelper.ToRadians(Projectile.ai[0] - SpinAcc * player.GetTotalAttackSpeed(DamageClass.Melee) * player.gravDir));
				v0.Y /= 3f;
				Vector2 v2 = player.Center + v0 * 72f;
				Vector2 v3 = Utils.SafeNormalize(v1 - v2, new Vector2(1, 0));
				Vector2 v4 = Utils.SafeNormalize(Main.MouseWorld - Projectile.Center, new Vector2(1, 0));
				if ((v4 - v3).Length() < 0.1f + SpinAcc * 0.01f)
				{
					Projectile.ai[1] = 1f;
					if (Projectile.owner == Main.myPlayer)
					{
						Projectile.velocity = Projectile.DirectionToSafe(Main.MouseWorld) * SpinAcc * player.GetTotalAttackSpeed(DamageClass.Melee);
						Projectile.netUpdate2 = true;
						Projectile.netImportant = true;
					}
					return;
				}
				Projectile.penetrate = 15;
			}
		}
		player.direction = Math.Sign(player.DirectionToSafe(Projectile.Center).X);
		player.itemTime = player.itemTimeMax - 1;
		player.SetCompositeArmFront(true, 0, player.AngleToSafe(Projectile.Center) * player.gravDir - MathHelper.PiOver2);
		if (SpinAcc < 17f)
		{
			SpinAcc += 0.05f;
		}
		else
		{
			SwirlTime++;
			if (SwirlTime > 300)
			{
				player.AddBuff(BuffID.Confused, Math.Clamp((SwirlTime - 300) / 6, 0, 120));
			}
		}
		if (Projectile.ai[1] == 0f)
		{
			if (Projectile.soundDelay == 0)
			{
				SoundEngine.PlaySound(SoundID.Item1);
				Projectile.soundDelay = (int)(240f / (SpinAcc + 3));
			}
			Projectile.ai[0] += SpinAcc * player.GetTotalAttackSpeed(DamageClass.Melee) * player.gravDir;

			Vector2 vector = Utils.ToRotationVector2(MathHelper.ToRadians(Projectile.ai[0]));
			vector.Y /= 3f;
			Projectile.Center = player.Center + vector * 72f;
			player.fullRotationOrigin = new Vector2(0, 60f);
			player.fullRotation = -vector.X * 0.01f * SpinAcc;
			if (vector.Y > 0f)
			{
				player.heldProj = Projectile.whoAmI;
				Projectile.drawLayer = ProjectileDrawLayerID.HeldProjOverHand;
			}

			Projectile.velocity = Projectile.DirectionFromSafe(player.Center);
			if (Main.mouseRightRelease)
			{
				player.velocity.X += SpinAcc * Math.Clamp((Main.MouseWorld.X - player.Center.X) / 300f, -1f, 1f) * 0.04f / (player.velocity.Length() + 3);
			}

			Projectile.knockBack = SpinAcc * 0.4f + 1;
		}
		else
		{
			player.fullRotation = 0;
		}

		Vector2 vector2 = player.Center - Projectile.Center;
		Projectile.rotation = (float)(Math.Atan2(vector2.Y, vector2.X) + Math.PI / 2d);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// TODO 144
		// int size = target.width * target.height;
		// if (Projectile.ai[1] <= 10f)
		// damage = (int)(damage / 5d * 0.45f * SpinAcc);
		// else
		// damage = (int)(Projectile.damage * 0.45f * SpinAcc);
		// if (size > 1000 && Projectile.ai[1] != 0)
		// {
		// Projectile.penetrate = 0;
		// Projectile.Kill();
		// }
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<CactusBallBuff>(), 150, false);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(ModContent.BuffType<CactusBallBuff>(), 150, false);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.ai[1] = 60f;
		if (Projectile.velocity.X == oldVelocity.X)
		{
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
			}

			if (Math.Abs(Projectile.velocity.Y) > 1.4f)
			{
				SoundEngine.PlaySound(SoundID.Dig);
				if (Main.myPlayer == Projectile.owner)
				{
					for (int i = 0; i < Main.rand.Next(6, 9); i++)
					{
						Vector2 vector = Utils.ToRotationVector2(Main.rand.NextFloat(MathHelper.Pi));
						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vector * 5f, vector * 3f,
							ModContent.ProjectileType<CactusBallSpike>(), Projectile.damage / 3, 0f, Projectile.owner, 0f, 0f);
					}
				}
			}
			float k0 = Projectile.oldVelocity.Length() * 0.5f;
			for (int j = 0; j < 3 * k0; j++)
			{
				Vector2 v0 = new Vector2(Main.rand.NextFloat(0.2f, 0.6f), 0).RotatedByRandom(6.283) * Projectile.scale * k0;
				int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<CactusJuice>(), v0.X, v0.Y, 100, default(Color), Main.rand.NextFloat(3.7f, 5.1f) * k0 * 0.02f);
				Main.dust[dust1].alpha = 0;
				Main.dust[dust1].rotation = 0;
			}
			Collision.HitTiles(Projectile.Center - new Vector2(36), Projectile.velocity * 0.5f, 72, 72);
			return false;
		}
		return true;
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		width = height = 1;
		return true;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Dig);
		float k0 = Projectile.oldVelocity.Length();
		for (int i = 0; i < 2 * k0; i++)
		{
			Vector2 vector = new Vector2(Main.rand.NextFloat(0.06f, 0.24f), 0).RotatedByRandom(6.283) * Projectile.scale * k0;
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vector * 5f, vector * 3f,
				ModContent.ProjectileType<CactusBallSpike>(), Projectile.damage / 3, 0f, Projectile.owner, 0f, 0f);
		}

		for (int j = 0; j < 3 * k0; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0.2f, 0.6f), 0).RotatedByRandom(6.283) * Projectile.scale * k0;
			int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<CactusJuice>(), v0.X, v0.Y, 100, default(Color), Main.rand.NextFloat(3.7f, 5.1f) * k0 * 0.02f);
			Main.dust[dust1].alpha = 0;
			Main.dust[dust1].rotation = 0;
		}
		for (int i = 0; i < 2; i++)
		{
			for (int x = 0; x < 8; x++)
			{
				Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(40)).RotatedByRandom(6.283) - Projectile.velocity * 3, new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283) * k0, ModContent.Find<ModGore>("Everglow/CactusGore" + x.ToString()).Type);
			}
		}
		for (int j = 0; j < 2 * k0; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0.1f, 0.4f), 0).RotatedByRandom(6.283) * Projectile.scale * k0;
			int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<CactusSmog>(), v0.X, v0.Y, 100, default(Color), Main.rand.NextFloat(0.17f, 0.31f) * k0);
			Main.dust[dust1].alpha = 180;
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
		}

		Collision.HitTiles(Projectile.Center - new Vector2(48), Projectile.velocity, 96, 96);
		Main.player[Projectile.owner].fullRotation = 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawTrail(lightColor);
		Color alpha = Projectile.GetAlpha(lightColor);
		if (Projectile.ai[1] < 25f && (Projectile.Center - player.Center).Length() <= 120)
		{
			Texture2D chain = PlantUtils.GetTexture("Everglow/Plant/Projectiles/Melee/CactusBallChain");
			Main.spriteBatch.Draw(chain, (Projectile.Center + player.Center) / 2f - Main.screenPosition, null, alpha, Projectile.AngleToSafe(player.Center),
				chain.Size() / 2f, new Vector2((Projectile.Center - player.Center).Length() / 62f, Projectile.scale), 0, 0f);
		}
		Texture2D flower = PlantUtils.GetTexture("Everglow/Plant/Projectiles/Melee/CactusBallFlower");
		float dir = Projectile.ai[1] < 15f ? Projectile.AngleFromSafe(player.Center) : Projectile.velocity.ToRotation();
		Main.spriteBatch.Draw(flower, Projectile.Center - dir.ToRotationVector2() * 25f * Projectile.scale - Main.screenPosition, null,
			alpha, dir, flower.Size() / 2f, 1f, 0, 0f);
		Texture2D cactus = PlantUtils.GetTexture(Texture);
		Main.spriteBatch.Draw(cactus, Projectile.Center - Main.screenPosition, null,
			alpha, Projectile.rotation, cactus.Size() / 2f, Projectile.scale, 0, 0f);
		if (Projectile.ai[1] != 0f)
		{
			int amt = Projectile.oldPos.Length;
			for (int i = 0; i < amt; i++)
			{
				Color color = Color.Lerp(alpha, Color.Transparent, (float)i / amt);
				Main.spriteBatch.Draw(cactus, Projectile.oldPos[i] + Utils.Size(cactus) / 2f - Main.screenPosition, null,
					color, Projectile.rotation, cactus.Size() / 2f, Projectile.scale, 0, 0f);
			}
		}
		return false;
	}

	public virtual string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public virtual string TrailColorTex()
	{
		return Commons.ModAsset.MEAC_Color1_Mod;
	}

	public virtual float TrailAlpha(float factor)
	{
		float w;
		if (factor > 0.5f)
		{
			w = MathHelper.Lerp(0.5f, 0.7f, factor);
		}
		else
		{
			w = MathHelper.Lerp(0f, 0.5f, factor * 2f);
		}

		return w;
	}

	public void DrawTrail(Color color)
	{
		Player player = Main.player[Projectile.owner];
		List<Vector2> SmoothTrailXUp = GraphicsUtils.CatmullRom(trailVecsUp.ToList()); // 平滑
		var SmoothTrailUp = new List<Vector2>();
		for (int x = 0; x < SmoothTrailXUp.Count; x++)
		{
			SmoothTrailUp.Add(SmoothTrailXUp[x]);
		}

		int length = SmoothTrailUp.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trailUp = SmoothTrailUp.ToArray();

		var bars = new List<Vertex2D>();

		for (int i = 1; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = TrailAlpha(factor);
			Vector2 Radial = Utils.SafeNormalize(trailUp[i] - trailUp[i - 1], new Vector2(1, 0));
			Radial = Radial.RotatedBy(Math.PI / 2d);
			Color c0 = Lighting.GetColor((int)(trailUp[i].X / 16), (int)(trailUp[i].Y / 16));
			c0.R = (byte)(c0.R * 0.07f * SpinAcc / 15f);
			c0.G = (byte)(c0.G * 0.21f * SpinAcc / 15f);
			c0.B = (byte)(c0.B * 0.07f * SpinAcc / 15f);
			c0.A = 10;
			bars.Add(new Vertex2D(trailUp[i] + Radial * 25f - Main.screenPosition, c0, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(trailUp[i] - Radial * 15f - Main.screenPosition, c0, new Vector3(factor, 0, w)));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Everglow/Plant/Projectiles/Melee/CactusBallTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public void DrawWarp(VFXBatch sb)
	{
		Player player = Main.player[Projectile.owner];
		List<Vector2> SmoothTrailXUp = GraphicsUtils.CatmullRom(trailVecsUp.ToList()); // 平滑
		var SmoothTrailUp = new List<Vector2>();
		for (int x = 0; x < SmoothTrailXUp.Count; x++)
		{
			SmoothTrailUp.Add(SmoothTrailXUp[x]);
		}
		var SmoothTrailDown = new List<Vector2>();
		List<Vector2> SmoothTrailXDown = GraphicsUtils.CatmullRom(trailVecsDown.ToList()); // 平滑
		for (int x = 0; x < SmoothTrailXDown.Count; x++)
		{
			SmoothTrailDown.Add(SmoothTrailXDown[x]);
		}
		int length = SmoothTrailUp.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trailUp = SmoothTrailUp.ToArray();

		var bars = new List<Vertex2D>();
		for (int i = 1; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = 1f;
			float d = trailUp[i].ToRotation() + 1.57f;
			float dir = d / MathHelper.TwoPi;
			Vector2 Radial = Utils.SafeNormalize(trailUp[i] - trailUp[i - 1], new Vector2(1, 0));
			Radial = Radial.RotatedBy(Math.PI / 2d);

			bars.Add(new Vertex2D(trailUp[i] + Radial * 25f - Main.screenPosition, new Color(dir * SpinAcc / 15f, w * SpinAcc / 150f, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(trailUp[i] - Radial * 15f - Main.screenPosition, new Color(dir * SpinAcc / 15f, w * SpinAcc / 150f, 0, 1), new Vector3(factor, 0, w)));
		}

		sb.Draw(ModContent.Request<Texture2D>("Everglow/Plant/Projectiles/Melee/CactusBallTrail").Value, bars, PrimitiveType.TriangleStrip);
	}
}
