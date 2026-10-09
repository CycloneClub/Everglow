using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Buffs;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class DarkFan : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 58;
		Projectile.height = 58;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.extraUpdates = 1;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
	}

	private Vector2 v_1 = new Vector2(-24, -14);
	private Vector2 v2 = Vector2.Zero;
	private bool dir = false;
	private int pdir = 1;
	private float prot = 0;
	private bool extraKnife = false;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<FanHit>(), 0, 0, player.whoAmI, 10, Main.rand.NextFloat(6.283f));
		int[] array = Projectile.localNPCImmunity;
		bool flag = (!Projectile.usesLocalNPCImmunity && !Projectile.usesIDStaticNPCImmunity) || (Projectile.usesLocalNPCImmunity && array[target.whoAmI] == 0) || (Projectile.usesIDStaticNPCImmunity && Projectile.IsNPCIndexImmuneToProjectileType(Projectile.type, target.whoAmI));
		if (target.active && !target.dontTakeDamage && flag && (target.aiStyle != 112 || target.ai[2] <= 1f))
		{
			if (target.active)
			{
				Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			}
		}
		target.AddBuff(ModContent.BuffType<OnMoth>(), 300);

		target.AddBuff(ModContent.BuffType<FireflyInferno>(), 120);

		if (MothBuffTarget.mothStack[target.whoAmI] < 5)
		{
			MothBuffTarget.mothStack[target.whoAmI] += 1;
		}
		else
		{
			MothBuffTarget.mothStack[target.whoAmI] = 5;
		}
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (!dir)
		{
			pdir = Math.Sign(Main.mouseX - player.Center.X + Main.screenPosition.X);
			Vector2 vc = -(new Vector2(Main.mouseX, Main.mouseY) - player.Center + Main.screenPosition);
			prot = (float)Math.Atan2(vc.Y, vc.X);
			if (pdir == 1)
			{
				prot += (float)Math.PI;
			}

			dir = true;
		}
		Vector2 v0 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - Projectile.timeLeft));
		if (Projectile.timeLeft < 30)
		{
			Projectile.Kill();
			v0 = v_1.RotatedBy(1.6 * Math.PI);
		}
		Projectile.spriteDirection = pdir;

		v0.X *= pdir;
		Vector2 v1 = new Vector2(v0.X, v0.Y * 0.5f).RotatedBy(prot) - new Vector2(29, 29);
		Projectile.position = player.Center + v1;
		v2 = Projectile.Center - player.Center;
		v2.X *= pdir;
		float Rot = (float)(Math.Atan2(v2.Y, v2.X) + Math.PI / 4d * pdir);
		Projectile.rotation = Rot;
		Projectile.velocity = v2.RotatedBy(Math.PI / 2d) / v2.Length();
		if (extraKnife)
		{
			if (Projectile.timeLeft % 2 == 0)
			{
				if (Projectile.extraUpdates > 1)
				{
					Projectile.extraUpdates--;
				}
			}
		}
		else
		{
			if (Projectile.timeLeft % 4 == 0)
			{
				if (Projectile.extraUpdates < 9)
				{
					Projectile.extraUpdates++;
				}
			}
		}
		if (Projectile.timeLeft == 32 && !extraKnife)
		{
			extraKnife = true;
			Projectile.timeLeft = 70;
		}
		int frequency = 270 / (2 + player.maxMinions);
		if (Projectile.timeLeft % frequency == 0)
		{
			Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Center, (v1 + new Vector2(29, 29)) / 8f, ModContent.ProjectileType<GlowingButterfly>(), Projectile.damage / 3 * 2, Projectile.knockBack, player.whoAmI, Main.rand.Next(2), 0f);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D t = ModAsset.DarkFan.Value;
		var drawOrigin = new Vector2(t.Width * 0.5f, t.Height * 0.5f);
		if (extraKnife)
		{
			int g000 = 0;
			for (int k = 0; k < /*(Projectile.timeLeft - 25) * 2*/Projectile.oldPos.Length; k++)
			{
				if (k % (Projectile.timeLeft / 7) == 0)
				{
					Vector2 v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - k - Projectile.timeLeft * (Projectile.timeLeft - 30) / 40f)) * 1.5f;
					v3.X *= pdir;
					Vector2 v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
					player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, v4.ToRotation() - 1.57f);
					Vector2 v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
					Vector2 drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
					Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
					Color color2 = color;
					float Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
					SpriteEffects S = SpriteEffects.None;
					if (pdir == -1)
					{
						S = SpriteEffects.FlipHorizontally;
					}

					Main.spriteBatch.Draw(t, drawPos, null, color2, Rot, drawOrigin, Projectile.scale * 1.5f, S, 0f);
					if (g000 < 4)
					{
						if (pdir == 1)
						{
							aDc[g000 * 3 + 0] = color;
							aDc[g000 * 3 + 1] = color;
							aDc[g000 * 3 + 2] = color;
							vFanP[g000 * 3 + 0] = drawPos - new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
							vFanP[g000 * 3 + 2] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
							v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - (k + Projectile.timeLeft / 7) - Projectile.timeLeft * (Projectile.timeLeft - 30) / 40f)) * 1.5f;
							v3.X *= pdir;
							v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
							v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
							Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
							drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
							vFanP[g000 * 3 + 1] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
						}
						else
						{
							aDc[g000 * 3 + 0] = color;
							aDc[g000 * 3 + 1] = color;
							aDc[g000 * 3 + 2] = color;
							vFanP[g000 * 3 + 0] = drawPos - new Vector2(22, 22).RotatedBy(Rot - Math.PI);
							vFanP[g000 * 3 + 1] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI);
							v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - (k + Projectile.timeLeft / 7) - Projectile.timeLeft * (Projectile.timeLeft - 30) / 40f)) * 1.5f;
							v3.X *= pdir;
							v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
							v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
							Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
							drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
							vFanP[g000 * 3 + 2] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI);
						}
					}
					g000 += 1;
					if (g000 >= 5)
					{
						break;
					}
				}
			}
		}
		else
		{
			int g000 = 0;
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				if (Projectile.oldPos[k] == Vector2.Zero)
				{
					break;
				}

				if (k % 10 == 0)
				{
					Vector2 v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - Projectile.timeLeft - k)) * 1.5f;
					v3.X *= pdir;
					Vector2 v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
					player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, v4.ToRotation() - 1.57f);
					Vector2 v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
					Vector2 drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
					Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
					Color color2 = color;
					float Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
					SpriteEffects S = SpriteEffects.None;
					if (pdir == -1)
					{
						S = SpriteEffects.FlipHorizontally;
					}

					Main.spriteBatch.Draw(t, drawPos, null, color2, Rot, drawOrigin, Projectile.scale * 1.5f, S, 0f);
					if (g000 < 4)
					{
						if (pdir == 1)
						{
							aDc[g000 * 3 + 0] = color;
							aDc[g000 * 3 + 1] = color;
							aDc[g000 * 3 + 2] = color;
							vFanP[g000 * 3 + 0] = drawPos - new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
							vFanP[g000 * 3 + 2] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
							v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - Projectile.timeLeft - (k + 10))) * 1.5f;
							v3.X *= pdir;
							v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
							v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
							Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
							drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
							vFanP[g000 * 3 + 1] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI / 2d /* (Pdir + 1) / 2d*/);
						}
						else
						{
							aDc[g000 * 3 + 0] = color;
							aDc[g000 * 3 + 1] = color;
							aDc[g000 * 3 + 2] = color;
							vFanP[g000 * 3 + 0] = drawPos - new Vector2(22, 22).RotatedBy(Rot - Math.PI);
							vFanP[g000 * 3 + 1] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI);
							v3 = v_1.RotatedBy(1.6 / 170d * Math.PI * (200 - Projectile.timeLeft - (k + 10))) * 1.5f;
							v3.X *= pdir;
							v4 = new Vector2(v3.X, v3.Y * 0.5f).RotatedBy(prot);
							v5 = new Vector2(v3.X, v3.Y).RotatedBy(prot);
							Rot = (float)(Math.Atan2(v5.Y, v5.X) + Math.PI / 4d * pdir + Math.PI * (1 - pdir) / 2d);
							drawPos = player.Center + v4 - Main.screenPosition + drawOrigin + new Vector2(1f, Projectile.gfxOffY) - new Vector2(29);
							vFanP[g000 * 3 + 2] = drawPos + new Vector2(22, 22).RotatedBy(Rot - Math.PI);
						}
					}
					g000 += 1;
					if (g000 >= 5)
					{
						break;
					}
				}
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
	}

	private Vector2[] vFanP = new Vector2[20];
	private Color[] aDc = new Color[20];

	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var Vx = new List<Vertex2D>();
		for (int ad = 0; ad < 5; ad++)
		{
			if (vFanP[ad * 3] == Vector2.Zero)
			{
				break;
			}

			if (ad == 0)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], aDc[ad * 3 + 0], new Vector3(30f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], aDc[ad * 3 + 1], new Vector3(34f / 88f, 2f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], aDc[ad * 3 + 2], new Vector3(57f / 88f, 0f / 88f, 0)));
			}
			if (ad == 1)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], aDc[ad * 3 + 0], new Vector3(34f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], aDc[ad * 3 + 1], new Vector3(56f / 88f, 7f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], aDc[ad * 3 + 2], new Vector3(73f / 88f, 12f / 88f, 0)));
			}
			if (ad == 2)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], aDc[ad * 3 + 0], new Vector3(40f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], aDc[ad * 3 + 1], new Vector3(74f / 88f, 18f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], aDc[ad * 3 + 2], new Vector3(85f / 88f, 29f / 88f, 0)));
			}
			if (ad == 3)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], aDc[ad * 3 + 0], new Vector3(46f / 88f, 53f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], aDc[ad * 3 + 1], new Vector3(82f / 88f, 34f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], aDc[ad * 3 + 2], new Vector3(85f / 88f, 49f / 88f, 0)));
			}
			if (ad == 4)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], aDc[ad * 3 + 0], new Vector3(46f / 88f, 53f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], aDc[ad * 3 + 1], new Vector3(82f / 88f, 34f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], aDc[ad * 3 + 2], new Vector3(85f / 88f, 49f / 88f, 0)));
			}
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.DarknessFan.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
		for (int ad = 0; ad < 5; ad++)
		{
			if (vFanP[ad * 3] == Vector2.Zero)
			{
				break;
			}

			if (ad == 0)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], new Color(255, 255, 255, 0), new Vector3(30f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], new Color(255, 255, 255, 0), new Vector3(34f / 88f, 2f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], new Color(255, 255, 255, 0), new Vector3(57f / 88f, 0f / 88f, 0)));
			}
			if (ad == 1)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], new Color(255, 255, 255, 0), new Vector3(34f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], new Color(255, 255, 255, 0), new Vector3(56f / 88f, 7f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], new Color(255, 255, 255, 0), new Vector3(73f / 88f, 12f / 88f, 0)));
			}
			if (ad == 2)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], new Color(255, 255, 255, 0), new Vector3(40f / 88f, 51f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], new Color(255, 255, 255, 0), new Vector3(74f / 88f, 18f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], new Color(255, 255, 255, 0), new Vector3(85f / 88f, 29f / 88f, 0)));
			}
			if (ad == 3)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], new Color(255, 255, 255, 0), new Vector3(46f / 88f, 53f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], new Color(255, 255, 255, 0), new Vector3(82f / 88f, 34f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], new Color(255, 255, 255, 0), new Vector3(85f / 88f, 49f / 88f, 0)));
			}
			if (ad == 4)
			{
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 0], new Color(255, 255, 255, 0), new Vector3(46f / 88f, 53f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 1], new Color(255, 255, 255, 0), new Vector3(82f / 88f, 34f / 88f, 0)));
				Vx.Add(new Vertex2D(vFanP[ad * 3 + 2], new Color(255, 255, 255, 0), new Vector3(85f / 88f, 49f / 88f, 0)));
			}
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.DarknessFan_glow.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);
	}
}
