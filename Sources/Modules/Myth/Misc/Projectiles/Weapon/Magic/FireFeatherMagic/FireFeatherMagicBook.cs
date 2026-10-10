using Everglow.SpellAndSkull.Projectiles;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;

internal class FireFeatherMagicBook : MagicBookProjectile
{
	public override string Texture => "Everglow/" + ModAsset.FireFeatherMagic_Path;

	public override void SetDef()
	{
		ProjType = ModContent.ProjectileType<FireFeather>();
		DustType = ModContent.DustType<Dusts.FireFeather>();
		ItemType = ModContent.ItemType<Items.Weapons.FireFeatherMagic>();
		MulStartPosByVelocity = 2f;
		UseGlow = false;
		FrontTexture = ModAsset.FireFeatherMagic.Value;
		PaperTexture = ModAsset.FireFeatherMagic_paper.Value;
		GlowTexture = ModAsset.FireFeatherMagic.Value;

		effectColor = new Color(105, 75, 45, 100);
		TexCoordTop = new Vector2(16, -1);
		TexCoordLeft = new Vector2(-1, 29);
		TexCoordDown = new Vector2(28, 37);
		TexCoordRight = new Vector2(41, 10);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f; // 书跟着玩家飞
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ItemType && player.active && !player.dead)// 检测手持物品
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (timer < 30)
			{
				timer++;
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount playerCASA = Player.CompositeArmStretchAmount.Full; // 玩家动作

		player.SetCompositeArmFront(true, playerCASA, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, playerCASA, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;
		SpecialAI();
		if (ProjType == -1)
		{
			return;
		}

		if (player.itemTime == player.itemTimeMax - 2 && player.HeldItem.type == ItemType)
		{
			for (int x = 0; x < 4; x++)
			{
				var p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -4) + new Vector2(0, Main.rand.NextFloat(50f)).RotateRandom(6.283) + Vector2.Normalize(vTOMouse) * 12, Vector2.Zero, ModContent.ProjectileType<FlameShoot>(), (int)(player.HeldItem.damage * MulDamage), player.HeldItem.knockBack, player.whoAmI, Main.rand.NextFloat(0.5f, 2.5f), 0, player.HeldItem.shootSpeed);
				p2.timeLeft = Main.rand.Next(200, 300);
			}
		}
	}

	/// <summary>
	/// 对于书本前部的绘制
	/// </summary>
	/// <param name="tex"></param>
	/// <param name="GlowType">颜色模式：0 使用环境光照，2 使用 effectColor，其他值使用 GlowColor。</param>
	/// <param name="MulSize">相对于 BookScale 的额外绘制尺寸倍率。</param>
	public override void DrawFront(Texture2D tex, int GlowType = 0, float MulSize = 1f)
	{
		Player player = (DrawPlayer ?? Main.player[Projectile.owner]);
		Vector2 x0 = new Vector2(BookScale * player.direction, BookScale * player.gravDir) * 0.5f * MulSize;
		Vector2 y0 = new Vector2(BookScale * player.direction, -BookScale * player.gravDir) * 0.707f * MulSize;
		Color c0 = GlowColor;
		if (GlowType == 0)
		{
			c0 = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));
		}

		if (GlowType == 2)
		{
			c0 = effectColor;
		}

		var bars = new List<Vertex2D>();
		for (int i = 0; i < 10; ++i)
		{
			double rot = -timer / 270d - i * timer / 400d * (1 + Math.Sin(Main.timeForVisualEffects / 7d + 1) * 0.4);
			rot += Projectile.rotation;
			Vector2 basePos = Projectile.Center + x0 - x0.RotatedBy(rot) * i / 4.5f - y0 * 0.05f - x0 * 0.02f;

			float upX = MathHelper.Lerp(TexCoordTop.X / tex.Width, TexCoordRight.X / tex.Width, i / 9f);
			float upY = MathHelper.Lerp(TexCoordTop.Y / tex.Height, TexCoordRight.Y / tex.Height, i / 9f);
			var upPos = new Vector2(upX, upY);
			Vector2 downLeft = upPos + new Vector2((TexCoordLeft.X - TexCoordTop.X) / tex.Width, (TexCoordLeft.Y - TexCoordTop.Y) / tex.Height);
			Vector2 downRight = upPos + new Vector2((TexCoordDown.X - TexCoordRight.X) / tex.Width, (TexCoordDown.Y - TexCoordRight.Y) / tex.Height);
			var downPos = Vector2.Lerp(downLeft, downRight, i / 9f);
			if (Math.Abs(rot) > Math.PI / 2d)
			{
				if (player.direction * player.gravDir == 1)
				{
					bars.Add(new Vertex2D(basePos - y0 - Main.screenPosition, c0, new Vector3(downPos, 0)));
					bars.Add(new Vertex2D(basePos + y0 - Main.screenPosition, c0, new Vector3(upPos, 0)));
				}
				else
				{
					bars.Add(new Vertex2D(basePos + y0 - Main.screenPosition, c0, new Vector3(upPos, 0)));
					bars.Add(new Vertex2D(basePos - y0 - Main.screenPosition, c0, new Vector3(downPos, 0)));
				}
			}
			else
			{
				if (player.direction * player.gravDir == 1)
				{
					bars.Add(new Vertex2D(basePos + y0 - Main.screenPosition, c0, new Vector3(upPos, 0)));
					bars.Add(new Vertex2D(basePos - y0 - Main.screenPosition, c0, new Vector3(downPos, 0)));
				}
				else
				{
					bars.Add(new Vertex2D(basePos - y0 - Main.screenPosition, c0, new Vector3(downPos, 0)));
					bars.Add(new Vertex2D(basePos + y0 - Main.screenPosition, c0, new Vector3(upPos, 0)));
				}
			}
		}
		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Vector2 basePos2 = Projectile.Center - y0 * 0.05f - x0 * 0.02f;
		bars = new List<Vertex2D>
		{
			new Vertex2D(basePos2 + new Vector2(17 * player.direction, -11) - Main.screenPosition, c0, new Vector3(0, 0, 0)),
		};
		if (player.direction * player.gravDir == 1)
		{
			bars.Add(new Vertex2D(basePos2 + new Vector2(24 * player.direction, 0) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(basePos2 + new Vector2(0 * player.direction, 0) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
		}
		else
		{
			bars.Add(new Vertex2D(basePos2 + new Vector2(0 * player.direction, 0) - Main.screenPosition, c0, new Vector3(0, 1, 0)));
			bars.Add(new Vertex2D(basePos2 + new Vector2(24 * player.direction, 0) - Main.screenPosition, c0, new Vector3(1, 0, 0)));
		}
		bars.Add(new Vertex2D(basePos2 + new Vector2(7 * player.direction, 11) - Main.screenPosition, c0, new Vector3(1, 1, 0)));

		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.FireFeatherMagic_feather.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		basePos2 = Projectile.Center + y0 * 0.25f + x0 * 1.3f;
		double roprTot = -timer / 270d - 9 * timer / 400d * (1 + Math.Sin(Main.timeForVisualEffects / 7d + 1) * 0.4);
		roprTot += Projectile.rotation;
		Vector2 ropeLeft = Projectile.Center + x0 - x0.RotatedBy(roprTot) * 2 + y0 * 0.15f - x0 * 0.02f;

		Vector2 normalized = Vector2.Normalize(ropeLeft - basePos2).RotatedBy(1.57) * 2.1f;

		bars = new List<Vertex2D>
		{
			new Vertex2D(ropeLeft + normalized - Main.screenPosition, c0, new Vector3(0, 0, 0)),
			new Vertex2D(basePos2 + normalized - Main.screenPosition, c0, new Vector3(1, 0, 0)),
			new Vertex2D(ropeLeft - normalized - Main.screenPosition, c0, new Vector3(0, 1, 0)),
			new Vertex2D(basePos2 - normalized - Main.screenPosition, c0, new Vector3(1, 1, 0)),
		};
		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.FireFeatherMagic_rope.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
