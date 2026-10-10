using Everglow.Myth.Common;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class World : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 150;
		Projectile.height = 150;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Melee;
	}

	internal Vector2[] OldMouseWorld = new Vector2[60];
	internal Vector2[] OldMouseWorldII = new Vector2[60];
	internal Vector2[] OldMouseWorldIII = new Vector2[120];
	internal Vector2[] OldMouseWorldIV = new Vector2[120];
	internal Vector2[] OldMouseWorldV = new Vector2[240];
	internal float[] OldRotation = new float[60];
	internal float[] OldWidth = new float[240];
	internal float[] OldWidthII = new float[240];
	internal float[] OldWidthIII = new float[240];
	internal float[] OldWidthIV = new float[240];
	internal float[] OldWidthV = new float[240];
	internal int SecFrame = 8;
	internal Vector2 CatchPos = Vector2.Zero;
	internal int Mode = 1;
	internal Vector2 CrackPoint;
	internal int timer = 0;
	internal int Aimtimer = 0;
	internal int[] HasNOHitV = new int[200];
	internal float Omega = 0;

	private void UpdateOldWidth(ref float[] value)
	{
		value[0] = Math.Clamp((OldMouseWorldV[2] - OldMouseWorldV[4]).Length() / 6f - 6f, 0, 32);
		value[1] = Math.Clamp((OldMouseWorldV[3] - OldMouseWorldV[5]).Length() / 6f - 6f, 0, 32); // 记录数据模板,这里记录撕裂宽度(由鼠标速度决定）
		if (SecFrame > 0)// 第二帧需要特殊处理
		{
			value[0] = 0;
			value[1] = 0;
			SecFrame--;
		}
		for (int f = 239; f > 1; f -= 2)
		{
			value[f] = value[f - 2] * 0.88f;
			value[f - 1] = value[f - 3] * 0.88f;
		}
	}

	public override void AI()
	{
		timer++;
		Player player = Main.player[Projectile.owner];
		if ((player.controlUseItem || Projectile.ai[0] > 0) && player.HeldItem.type == ModContent.ItemType<Items.Weapons.World>())
		{
			Projectile.timeLeft = 60;
		}

		Projectile.Center = player.Center;

		if (CatchPos == Vector2.Zero)
		{
			CatchPos = Main.MouseWorld;
		}

		CatchPos = Main.MouseWorld * 0.75f + CatchPos * 0.25f;
		if (Mode == 1 && timer >= Aimtimer && (player.controlUseItem || Projectile.ai[0] > 0))
		{
			if (Projectile.ai[0] > 0)
			{
				Projectile.ai[0] -= 1;
			}

			Aimtimer = Main.rand.Next(3, 9);
			float range = Main.rand.NextFloat(420f, 600f);
			CrackPoint = Main.MouseWorld + Vector2.Normalize(Main.MouseWorld - CrackPoint).RotatedBy(Main.rand.NextFloat(Main.rand.NextFloat(Main.rand.NextFloat(-0.5f, -0.2f), 0.2f), 0.5f)) * range;
			if (Projectile.ai[0] <= 0)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/Knife").WithPitchOffset(Main.rand.NextFloat(0.7f, 1f) - MathF.Min(timer / 15f, 1f)).WithVolumeScale(range / 600f), Projectile.Center);
			}

			timer = 0;
		}
		OldMouseWorld[0] = Main.MouseWorld; // 记录数据模板,这里记录鼠标坐标
		if (Mode == 1)
		{
			OldMouseWorld[0] = CrackPoint;
		}

		for (int f = OldMouseWorld.Length - 1; f > 0; f--)
		{
			OldMouseWorld[f] = OldMouseWorld[f - 1];
		}
		if (OldMouseWorld[2] != Vector2.Zero)// 第二帧需要特殊处理
		{
			OldMouseWorldII[0] = (Main.MouseWorld + OldMouseWorld[2]) / 2f;
			if (Mode == 1)
			{
				OldMouseWorldII[0] = (CrackPoint + OldMouseWorld[2]) / 2f;
			}
		}// 记录数据模板,这里取中点,为了平滑
		for (int f = OldMouseWorldII.Length - 1; f > 0; f--)
		{
			OldMouseWorldII[f] = OldMouseWorldII[f - 1];
		}

		OldMouseWorldIII[0] = OldMouseWorldII[0]; // 记录数据模板,这里记录鼠标坐标
		if (OldMouseWorldII[2] != Vector2.Zero)// 第二帧需要特殊处理
		{
			OldMouseWorldIII[1] = (OldMouseWorldII[1] + OldMouseWorldII[2] + OldMouseWorld[2]) / 3f; // 记录数据模板,这里取重心,为了平滑
		}

		for (int f = OldMouseWorldIII.Length - 1; f > 1; f -= 2)
		{
			OldMouseWorldIII[f] = OldMouseWorldIII[f - 2];
			OldMouseWorldIII[f - 1] = OldMouseWorldIII[f - 3];
		}

		if (OldMouseWorldIII[1] != Vector2.Zero)// 第二帧需要特殊处理
		{
			OldMouseWorldIV[0] = (Main.MouseWorld + OldMouseWorldIII[1]) / 2f; // 记录数据模板,这里在取了一次重心的重心曲线上取中点,再次平滑
			if (Mode == 1)
			{
				OldMouseWorldIV[0] = (CrackPoint + OldMouseWorldIII[1]) / 2f;
			}
		}
		for (int f = OldMouseWorldIV.Length - 1; f > 0; f--)
		{
			OldMouseWorldIV[f] = OldMouseWorldIV[f - 1];
		}
		if (OldMouseWorldIV[2] != Vector2.Zero)// 第二帧需要特殊处理
		{
			OldMouseWorldV[0] = OldMouseWorldIV[0]; // 记录数据模板,这里记录鼠标坐标
			OldMouseWorldV[1] = (OldMouseWorldIV[1] + OldMouseWorldIV[2] + OldMouseWorldIII[1]) / 3f; // 记录数据模板,这里取重心,为了平滑
		}
		for (int f = 239; f > 1; f -= 2)
		{
			OldMouseWorldV[f] = OldMouseWorldV[f - 2];
			OldMouseWorldV[f - 1] = OldMouseWorldV[f - 3];
		}

		Vector2 ArrowToMouse = Main.MouseWorld - player.Center;
		if (Mode == 1)
		{
			ArrowToMouse = CrackPoint - player.Center;
		}

		OldRotation[0] = (float)Math.Atan2(ArrowToMouse.Y, ArrowToMouse.X); // 记录数据模板,这里记录旋转角度
		for (int f = OldRotation.Length - 1; f > 0; f--)
		{
			OldRotation[f] = OldRotation[f - 1];
		}

		UpdateOldWidth(ref OldWidth);
		UpdateOldWidth(ref OldWidthII);
		UpdateOldWidth(ref OldWidthIII);
		UpdateOldWidth(ref OldWidthIV);
		UpdateOldWidth(ref OldWidthV);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Texture2D MainKnife = ModAsset.Weapons_World.Value;
		var Knife = new List<Vertex2D>();
		float KnifeLength = 180;
		float StartLength = -90;
		Color lightC = Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f));
		float Kcolor = Math.Clamp((7 - (OldWidth[1] + OldWidth[2] + OldWidth[3]) / 3f) / 12f, 0, 1);
		var LineRot = OldMouseWorldV[1] - OldMouseWorldV[2];
		if (Kcolor < 0.4f)
		{
			Projectile.rotation = (float)(Math.Atan2(LineRot.Y, LineRot.X) + Math.PI) * 0.25f + Projectile.rotation * 0.75f;
			Omega = 0;
		}
		else
		{
			if (!Main.gamePaused)
			{
				Projectile.rotation += Omega;
				if (Omega < 0.02f)
				{
					Omega += 0.002f;
				}
			}
		}

		float Kvec = Math.Clamp((60 - Projectile.timeLeft) / 30f, 0, 1);
		Kvec = (float)Math.Sqrt(Kvec);
		if (player.controlUseItem || Projectile.ai[0] > 0)
		{
			Kvec = 0;
		}
		else
		{
			Kcolor = 1;
		}
		if (Projectile.timeLeft < 15)
		{
			Kcolor *= Projectile.timeLeft * Projectile.timeLeft / 225f;
		}

		float R0 = lightC.R / 255f * Kcolor;
		float G0 = lightC.G / 255f * Kcolor;
		float B0 = lightC.B / 255f * Kcolor;
		float A0 = lightC.A / 255f * Kcolor;
		float AimRot = 2f;
		if (player.direction == -1)
		{
			AimRot = 1.14f;
		}

		var trueC = new Color(R0, G0, B0, A0);
		Vector2 DrawPos = OldMouseWorldV[1] * (1 - Kvec) + (player.Center + new Vector2(-24 * player.direction, 0)) * Kvec; // 滑动到玩家
		float TrueRot = Projectile.rotation * (1 - Kvec) + AimRot * Kvec;
		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength, 0).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(0, 1, 0)));
		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength + KnifeLength, 0).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(1, 0, 0)));
		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength + KnifeLength / 2f, KnifeLength / 2f).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(1, 1, 0)));

		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength, 0).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(0, 1, 0)));
		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength + KnifeLength / 2f, -KnifeLength / 2f).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(0, 0, 0)));
		Knife.Add(new Vertex2D(DrawPos + new Vector2(StartLength + KnifeLength, 0).RotatedBy(TrueRot) - Main.screenPosition, trueC, new Vector3(1, 0, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = MainKnife;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Knife.ToArray(), 0, Knife.Count / 3);

		if (player.controlUseItem || Projectile.ai[0] > 0)
		{
			player.heldProj = Projectile.whoAmI;
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(OldRotation[1] - Math.PI / 2d));
		}

		for (int k = 0; k < 5; ++k)
		{
			var bars = new List<Vertex2D>();
			var barsII = new List<Vertex2D>();
			float colorS = 254f / 255f;
			if (k > 0)
			{
				colorS = 54f / 255f;
			}

			for (int i = 1; i < 240; ++i)
			{
				float width = OldWidth[i] * Math.Clamp((i - 1) / 4f, 0, 1);
				if (k == 1)
				{
					width = OldWidthII[i] * Math.Clamp((i - 1) / 4f, 0, 1);
				}

				if (k == 2)
				{
					width = OldWidthIII[i] * Math.Clamp((i - 1) / 4f, 0, 1);
				}

				if (k == 3)
				{
					width = OldWidthIV[i] * Math.Clamp((i - 1) / 4f, 0, 1);
				}

				if (k == 4)
				{
					width = OldWidthV[i] * Math.Clamp((i - 1) / 4f, 0, 1);
				}

				width *= Projectile.timeLeft / 60f;
				if (OldMouseWorldV[i] == Vector2.Zero)
				{
					break;
				}

				var normalDir = OldMouseWorldV[i - 1] - OldMouseWorldV[i];

				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = i / 240f;
				var w = MathHelper.Lerp(1f, 0.05f, factor);
				Vector2 zero = Vector2.Zero;
				if (k > 0)
				{
					zero = new Vector2(8, 6).RotatedBy(k * Math.PI / 2.5d) * Math.Clamp(i / 15f, 0, 1);
				}
				else
				{
					if ((OldMouseWorldV[i] - OldMouseWorldV[i - 1]).Length() > 5)
					{
						Vector2 Vpi = Vector2.Normalize(OldMouseWorldV[i] - OldMouseWorldV[i - 1]) * 5;
						for (int j = 0; j < (OldMouseWorldV[i] - OldMouseWorldV[i - 1]).Length() / 5; j++)
						{
							Lighting.AddLight(OldMouseWorldV[i - 1] + Vpi * j, (255 - Projectile.alpha) * 0.04f / 50f * (1 - Math.Clamp(factor, 0, 1)) * width / 18f, (255 - Projectile.alpha) * 0.14f / 50f * (1 - Math.Clamp(factor, 0, 1)) * width / 24f, 0);
						}
					}
				}
				float TrueC = colorS * (1 - Math.Clamp(factor * 0.2f, 0, 1)) * 1.9f;
				bars.Add(new Vertex2D(OldMouseWorldV[i] + zero + normalDir * width - Main.screenPosition, new Color(TrueC, TrueC, TrueC, Math.Clamp(0.45f - TrueC, 0, 1)), new Vector3(factor, 1, w)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] + zero + normalDir * -width - Main.screenPosition, new Color(TrueC, TrueC, TrueC, Math.Clamp(0.45f - TrueC, 0, 1)), new Vector3(factor, 0, w)));
				if (k == 0)
				{
					if (i == 4 && !Main.gamePaused && Projectile.ai[0] <= 0)
					{
						Vector2 v0 = OldMouseWorldV[i - 1] - OldMouseWorldV[i];
						float value = v0.Length();
						if (value > 10f)
						{
							for (int f = 0; f < value * 0.1f; f++)
							{
								float x = Main.rand.NextFloat(-1.2f, 1.2f);
								var d = Dust.NewDustDirect(OldMouseWorldV[i] * x + OldMouseWorldV[i - 1] * (1 - x) - new Vector2(4) + new Vector2(0, width * 1.6f).RotateRandom(6.283), 0, 0, DustID.GemEmerald, 0, 0, 0, default, Main.rand.NextFloat(2f));
								d.velocity = v0 * Main.rand.NextFloat(0.2f);
								d.noGravity = true;
							}
						}
						for (int v = 0; v < 200; v++)
						{
							if (Math.Abs(Main.npc[v].Center.Y - OldMouseWorldV[i].Y) < Main.npc[v].height + 70 + width * 2 && Math.Abs(Main.npc[v].Center.X - OldMouseWorldV[i].X) < Main.npc[v].width + 70 + width * 2 && !Main.npc[v].dontTakeDamage && !Main.npc[v].friendly && Main.npc[v].active && HasNOHitV[v] == 0 && !Main.gamePaused)
							{
								HasNOHitV[v] = 8;
								float Damk = Math.Clamp(width / 12f, 0.1f, 15f);
								if (Mode == 0)
								{
									Damk = Math.Clamp(width / 6f, 0.1f, 15f);
								}

								int Dam = (int)(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f) * Damk);
								bool Crit = Main.rand.Next(100) < player.GetCritChance(DamageClass.Generic) + player.GetCritChance(DamageClass.Melee) + 15 + width * 1.5f;
								NPC.HitModifiers npchitmodifier = new NPC.HitModifiers();
								NPC.HitInfo hit = npchitmodifier.ToHitInfo(Dam, Crit, 2 * width / 72f);
								Main.npc[v].StrikeNPC(hit, true, true);
								NetMessage.SendStrikeNPC(Main.npc[v], hit);

								if (Crit)
								{
									MythContentPlayer myplayer = player.GetModPlayer<MythContentPlayer>();
									player.addDPS((int)((Dam - Main.npc[v].defDefense) * (0.6 + myplayer.CriticalDamage)));
								}
								else
								{
									player.addDPS(Dam - Main.npc[v].defDefense);
								}
								if (Mode == 1)
								{
									Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Main.npc[v].Center, Vector2.Zero, ModContent.ProjectileType<WorldHit>(), 0, Projectile.knockBack, Projectile.owner, Math.Clamp(width / 12f, 0, 1f), 0f);
								}
								else
								{
									Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Main.npc[v].Center, Vector2.Zero, ModContent.ProjectileType<WorldHit>(), 0, Projectile.knockBack, Projectile.owner, Math.Clamp(width / 4f, 0, 1f), 0f);
								}
							}
							if (HasNOHitV[v] > 0)
							{
								HasNOHitV[v]--;
							}
						}
					}
					barsII.Add(new Vertex2D(OldMouseWorldV[i] + zero + normalDir * Math.Clamp(width * 1.6f, 0, 72) - Main.screenPosition, new Color(255, 255, 255, 255), new Vector3(Math.Clamp(factor, 0, 1), 1, w)));
					barsII.Add(new Vertex2D(OldMouseWorldV[i] + zero + normalDir * -Math.Clamp(width * 1.6f, 0, 72) - Main.screenPosition, new Color(255, 255, 255, 255), new Vector3(Math.Clamp(factor, 0, 1), 0, w)));
				}
			}
			var Vx = new List<Vertex2D>();
			if (bars.Count > 2)
			{
				Vx.Add(bars[0]);
				var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(colorS, colorS, colorS, 0), new Vector3(0, 0.5f, 1));
				Vx.Add(bars[1]);
				Vx.Add(vertex);
				for (int i = 0; i < bars.Count - 2; i += 2)
				{
					Vx.Add(bars[i]);
					Vx.Add(bars[i + 2]);
					Vx.Add(bars[i + 1]);

					Vx.Add(bars[i + 1]);
					Vx.Add(bars[i + 2]);
					Vx.Add(bars[i + 3]);
				}
			}

			if (k == 0)
			{
				var VxII = new List<Vertex2D>();
				if (barsII.Count > 2)
				{
					VxII.Add(barsII[0]);
					var vertex = new Vertex2D((barsII[0].position + barsII[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(colorS, colorS, colorS, 0), new Vector3(0, 0.5f, 1));
					VxII.Add(barsII[1]);
					VxII.Add(vertex);
					for (int i = 0; i < barsII.Count - 2; i += 2)
					{
						VxII.Add(barsII[i]);
						VxII.Add(barsII[i + 2]);
						VxII.Add(barsII[i + 1]);

						VxII.Add(barsII[i + 1]);
						VxII.Add(barsII[i + 2]);
						VxII.Add(barsII[i + 3]);
					}
				}
				Texture2D t0 = Commons.ModAsset.Trail_black.Value;
				Main.graphics.GraphicsDevice.Textures[0] = t0;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, VxII.ToArray(), 0, VxII.Count / 3);
			}
			Texture2D t = ModAsset.VisualTextures_World.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// Texture2D glow = MythContent.QuickTexture("Misc/Projectiles/Weapon/Melee/World_glow");
		// Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, glow.Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		var bars = new List<Vertex2D>();
		for (int i = 1; i < 240; ++i)
		{
			float width = OldWidth[i] * OldWidth[i] * Math.Clamp((i - 1) / 4f, 0, 1) * 0.3f;
			if (OldMouseWorldV[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = OldMouseWorldV[i - 1] - OldMouseWorldV[i];

			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = 1 - i / 240f;
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			Vector2 DeltaV0 = OldMouseWorldV[i] - OldMouseWorldV[i - 1];
			float d = DeltaV0.ToRotation() + 3.14f + 1.57f;
			if (d > 6.28f)
			{
				d -= 6.28f;
			}

			float dir = d / MathHelper.TwoPi;
			Vector2 DeltaV1 = DeltaV0;
			float dir1 = dir;
			if (i > 1)
			{
				DeltaV1 = OldMouseWorldV[i - 1] - OldMouseWorldV[i - 2];
				float d1 = DeltaV1.ToRotation() + 3.14f + 1.57f;
				if (d1 > 6.28f)
				{
					d1 -= 6.28f;
				}

				dir1 = d1 / MathHelper.TwoPi;
			}

			float strength = width / 300f;
			if (dir - dir1 > 0.5)
			{
				var MidValue = (1 - dir) / (1 - dir + dir1);
				var MidPoint = MidValue * DeltaV0 + (1 - MidValue) * DeltaV1;
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition, new Color(0, strength, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, strength, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition, new Color(1, strength, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, strength, 0, 1), new Vector3(factor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var MidValue = (1 - dir1) / (1 - dir1 + dir);
				var MidPoint = MidValue * DeltaV1 + (1 - MidValue) * DeltaV0;
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition, new Color(1, strength, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(1, strength, 0, 1), new Vector3(factor, 0, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition, new Color(0, strength, 0, 1), new Vector3(factor, 1, 1)));
				bars.Add(new Vertex2D(OldMouseWorldV[i] - Main.screenPosition - MidPoint * Projectile.scale * 1.1f, new Color(0, strength, 0, 1), new Vector3(factor, 0, 1)));
			}

			bars.Add(new Vertex2D(OldMouseWorldV[i] + normalDir * width - Main.screenPosition, new Color(dir, strength, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(OldMouseWorldV[i] + normalDir * -width - Main.screenPosition, new Color(dir, strength, 0, 1), new Vector3(factor, 0, w)));
		}
		var Vx = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			Vx.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(0, 0, 0, 0), new Vector3(0, 0.5f, 1));
			Vx.Add(bars[1]);
			Vx.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				Vx.Add(bars[i]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 1]);

				Vx.Add(bars[i + 1]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 3]);
			}
		}
		spriteBatch.Draw(ModAsset.CrystalClub_trail.Value, Vx, PrimitiveType.TriangleList);
	}
}
