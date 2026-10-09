using Everglow.Myth.Common;
using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles.Typeless.FixCoins;

public abstract class FixCoinProjectile : ModProjectile
{
	public virtual string HeatMapTexture()
	{
		return "";
	}
	public virtual int PrefixID()
	{
		return 0;
	}
	public virtual int Level()
	{
		return 1;
	}
	internal float LightColorI = 0;
	internal float LightColorII = 0;
	internal Vector2[] IniV = new Vector2[5];
	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 150;
	}
	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 5; i++)
		{
			if (IniV[i] == Vector2.Zero)
			{
				IniV[i] = new Vector2(0, Main.rand.NextFloat(2f, 4f)).RotatedByRandom(6.283);
			}
		}
	}
	public override void AI()
	{
		Projectile.rotation = 0;
		Projectile.velocity *= 0.98f * Projectile.timeLeft / 150f;
		if (Projectile.velocity.Length() > 0.3f)
		{
			Projectile.velocity.Y -= 0.75f * Projectile.timeLeft / 150f;
		}

		if (Projectile.timeLeft > 50 && Projectile.timeLeft < 120)
			LightColorII += 1 / 70f;
		else
		{
			LightColorII *= 0.95f;
		}
		LightColorI += 1 / 150f;
	}
	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.Center);
		int type = ModContent.DustType<PureWhite>();
		switch (Level())
		{
			case 2:
				type = ModContent.DustType<PureLime>();
				break;
			case 3:
				type = ModContent.DustType<PurePurple>();
				break;
			case 4:
				type = ModContent.DustType<PureCeleste>();
				break;
			case 5:
				type = ModContent.DustType<PureOrange>();
				break;
		}
		var ColorVec = new Color(0.03f, 0.03f, 0.03f, 1);
		switch (Level())
		{
			case 2:
				ColorVec = new Color(0.1f, 1f, 0.00f, 1);
				break;
			case 3:
				ColorVec = new Color(0.0f, 0.0f, 1f, 1);
				break;
			case 4:
				ColorVec = new Color(0.5f, 0.0f, 0.8f, 1);
				break;
			case 5:
				ColorVec = new Color(1f, 0.8f, 0.00f, 1);
				break;
		}
		for (int h = 0; h < 20; h++)
		{
			Vector2 v3 = new Vector2(0, (float)Math.Sin(h * Math.PI / 4d + Projectile.ai[0]) + 5).RotatedBy(h * Math.PI / 10d) * Main.rand.NextFloat(0.2f, 1.1f);
			int r = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(4, 4), 0, 0, type, 0, 0, 0, default, 15f * Main.rand.NextFloat(0.4f, 1.1f));
			Main.dust[r].noGravity = true;
			Main.dust[r].velocity = v3;
		}
		Player player = Main.player[Projectile.owner];
		int X0 = Main.rand.Next(58);
		for (int x = X0; x < 58; x++)
		{
			if (player.inventory[x].accessory)
			{
				player.inventory[x].prefix = PrefixID();
				SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, player.Center);
				for (int h = 0; h < 20; h++)
				{
					Vector2 v3 = new Vector2(0, (float)Math.Sin(h * Math.PI / 4d) + 5).RotatedBy(h * Math.PI / 10d) * Main.rand.NextFloat(0.2f, 1.1f);
					int r = Dust.NewDust(new Vector2(player.Center.X, player.Center.Y) - new Vector2(4, 4), 0, 0, type, 0, 0, 0, default, 15f * Main.rand.NextFloat(0.4f, 1.1f));
					Main.dust[r].noGravity = true;
					Main.dust[r].velocity = v3;
				}
				// TODO:你的饰品得到了附魔
				string tex1 = "Your ";
				string tex2 = " get prefix";
				if (Language.ActiveCulture.Name == "zh-Hans")
				{
					tex1 = "你的[";
					tex2 = "]得到了附魔";
				}
				CombatText.NewText(new Rectangle((int)player.Center.X - 10, (int)player.Center.Y - 10, 20, 20), ColorVec, tex1 + player.inventory[x].Name + tex2);
				return;
			}
		}
		for (int x = X0; x >= 0; x--)
		{
			if (player.inventory[x].accessory)
			{
				player.inventory[x].prefix = PrefixID();
				SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, player.Center);
				for (int h = 0; h < 20; h++)
				{
					Vector2 v3 = new Vector2(0, (float)Math.Sin(h * Math.PI / 4d) + 5).RotatedBy(h * Math.PI / 10d) * Main.rand.NextFloat(0.2f, 1.1f);
					int r = Dust.NewDust(new Vector2(player.Center.X, player.Center.Y) - new Vector2(4, 4), 0, 0, type, 0, 0, 0, default, 15f * Main.rand.NextFloat(0.4f, 1.1f));
					Main.dust[r].noGravity = true;
					Main.dust[r].velocity = v3;
				}
				// TODO:你的饰品得到了附魔
				string tex1 = "Your ";
				string tex2 = " get prefix";
				if (Language.ActiveCulture.Name == "zh-Hans")
				{
					tex1 = "你的[";
					tex2 = "]得到了附魔";
				}
				CombatText.NewText(new Rectangle((int)player.Center.X - 10, (int)player.Center.Y - 10, 20, 20), ColorVec, tex1 + player.inventory[x].Name + tex2);
				return;
			}
		}
		// TODO:你的背包中没有饰品
		string tex3 = "Please put at lease 1 accessory item in your inventory";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			tex3 = "你的背包中没有饰品";
		}

		Item.NewItem(null, Projectile.Center, ModContent.ItemType<Misc.FixCoins.FixCoinCrit1>());
		CombatText.NewText(new Rectangle((int)player.Center.X - 10, (int)player.Center.Y - 10, 20, 20), Color.LightGray, tex3);
	}
	public override void PostDraw(Color lightColor)
	{
		var ColorVec = new Vector4(0.03f, 0.03f, 0.03f, 1);

		switch (Level())
		{
			case 2:
				ColorVec = new Vector4(0.1f, 1f, 0.00f, 1);
				break;
			case 3:
				ColorVec = new Vector4(0.0f, 0.0f, 1f, 1);
				break;
			case 4:
				ColorVec = new Vector4(0.5f, 0.0f, 0.8f, 1);
				break;
			case 5:
				ColorVec = new Vector4(0.8f, 0.6f, 0.00f, 1);
				break;
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		for (int j = 0; j < Level(); j++)
		{
			var bars = new List<Vertex2D>();
			Player player = Main.player[Projectile.owner];
			Vector2 v0 = Projectile.Center;
			Vector2 Vi = IniV[j];
			for (int i = 1; i < 300; ++i)
			{
				Vector2 v1 = player.Center - v0;
				if (v1.Length() < 5)
				{
					break;
				}

				v1 /= v1.Length();
				Vector2 v2 = v0;
				v0 += Vi + v1 * 5;
				Vi *= 0.99f;
				int width = (int)(9 * LightColorII);
				var normalDir = v2 - v0;
				normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
				var factor = Math.Abs(i / 23f % 1 - 0.5f);
				var color2 = new Color(ColorVec.X, ColorVec.Y, ColorVec.Z, 0);
				bars.Add(new Vertex2D(v0 + normalDir * width - Main.screenPosition, color2, new Vector3((float)Math.Sqrt(factor) + (float)Main.timeForVisualEffects * 0.01f, 1, 0)));
				bars.Add(new Vertex2D(v0 + normalDir * -width - Main.screenPosition, color2, new Vector3((float)Math.Sqrt(factor) + (float)Main.timeForVisualEffects * 0.01f, 0, 0)));
			}
			if (bars.Count > 2)
			{
				RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
				Texture2D Color = ModAsset.ElecLine.Value;

				Main.graphics.GraphicsDevice.Textures[0] = Color;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
				Main.graphics.GraphicsDevice.RasterizerState = originalState;
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Texture2D LightE = ModAsset.LightEffect.Value;

		Main.spriteBatch.Draw(LightE, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(ColorVec.X * LightColorI * LightColorI, ColorVec.Y * LightColorI * LightColorI, ColorVec.Z * LightColorI * LightColorI, 0), -(float)Math.Sin(Main.time / 26d) + 0.6f, new Vector2(128f, 128f), 0.5f + (float)(0.25 * Math.Sin(Main.time / 26d)), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(LightE, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(ColorVec.X * LightColorI * LightColorI, ColorVec.Y * LightColorI * LightColorI, ColorVec.Z * LightColorI * LightColorI, 0), (float)Math.Sin(Main.time / 12d + 2) + 1.6f, new Vector2(128f, 128f), 0.5f + (float)(0.25 * Math.Sin(Main.time / 26d)), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(LightE, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(ColorVec.X * LightColorI * LightColorI, ColorVec.Y * LightColorI * LightColorI, ColorVec.Z * LightColorI * LightColorI, 0), (float)Math.PI / 2f + (float)(Main.time / 9d), new Vector2(128f, 128f), 0.5f + (float)(0.25 * Math.Sin(Main.time / 26d + 1.57)), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(LightE, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(ColorVec.X * LightColorI * LightColorI, ColorVec.Y * LightColorI * LightColorI, ColorVec.Z * LightColorI * LightColorI, 0), (float)(Main.time / 26d), new Vector2(128f, 128f), 0.5f + (float)(0.25 * Math.Sin(Main.time / 26d + 3.14)), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(LightE, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(ColorVec.X * LightColorI * LightColorI, ColorVec.Y * LightColorI * LightColorI, ColorVec.Z * LightColorI * LightColorI, 0), -(float)(Main.time / 26d), new Vector2(128f, 128f), 0.5f + (float)(0.25 * Math.Sin(Main.time / 26d + 4.71)), SpriteEffects.None, 0);
		var Ball = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Texture2D Circle = ModAsset.FixCoinFramework.Value;
		Texture2D Light = ModAsset.FixCoinLight.Value;
		Color color = Lighting.GetColor((int)(Projectile.Center.X / 16d), (int)(Projectile.Center.Y / 16d));
		color = Projectile.GetAlpha(color) * ((255 - Projectile.alpha) / 255f);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color((int)(255 * LightColorII * ColorVec.X), (int)(255 * LightColorII * ColorVec.Y), (int)(255 * LightColorII * ColorVec.Z), 0), Projectile.rotation, new Vector2(56f, 56f), Projectile.scale, SpriteEffects.None, 0);
		if (Projectile.timeLeft > 50)
			Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(255 + color.R, 255 + color.G, 255 + color.B, color.A), Projectile.rotation, new Vector2(14f, 14f), Projectile.scale, SpriteEffects.None, 0);
		else
		{
			Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color((int)(255 * LightColorII) + color.R, (int)(255 * LightColorII) + color.G, (int)(255 * LightColorII) + color.B, color.A), Projectile.rotation, new Vector2(14f, 14f), Projectile.scale, SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(Circle, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, 28 * ((int)(Main.time / 6d + Projectile.ai[0]) % 5), 28, 28), color, Projectile.rotation, new Vector2(14f, 14f), Projectile.scale, SpriteEffects.None, 0);
	}
}
