using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class CorMoth4DProj : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/ButterflyDream";

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Dream Butterfly");
		Main.projFrames[Projectile.type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = maxTimeleft = 900;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	private NPC Owner => Main.npc[(int)Projectile.ai[0]];

	public Vector4 targetPos;
	private Vector4 v4Position;
	private int maxTimeleft;

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(targetPos.X);
		writer.Write(targetPos.Y);
		writer.Write(targetPos.Z);
		writer.Write(targetPos.W);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		targetPos.X = reader.ReadSingle();
		targetPos.Y = reader.ReadSingle();
		targetPos.Z = reader.ReadSingle();
		targetPos.W = reader.ReadSingle();
	}

	public override void AI()
	{
		if (Projectile.timeLeft % 3 == 0 && Main.rand.NextBool())
		{
			Projectile.frame++;
		}

		int t = maxTimeleft - Projectile.timeLeft;
		if (t == 0)
		{
			Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
			Projectile.ai[1] = 1;
		}
		// 逐维度展开
		if (t < 50)
		{
			v4Position.Y = MathHelper.Lerp(v4Position.Y, targetPos.Y, 0.05f);
		}
		else if (t < 100)
		{
			v4Position.X = MathHelper.Lerp(v4Position.X, targetPos.X, 0.05f);
		}
		else if (t < 150)
		{
			v4Position.Z = MathHelper.Lerp(v4Position.Z, targetPos.Z, 0.05f);
		}
		else if (t < 200)
		{
			v4Position.W = MathHelper.Lerp(v4Position.W, targetPos.W, 0.05f);
		}
		else
		{
			v4Position = VecRotByYoZ(v4Position, 0.01f);
			Projectile.ai[1] += 0.001f;
			// Position = Vector4.Normalize(Position) * (Position.Length() + 1f);
		}

		if (!Owner.active)
		{
			Projectile.Kill();
			return;
		}

		Vector3 v3 = Projection(v4Position * Projectile.ai[1] + new Vector4(Owner.Center, 0, 0), 1000);
		if (v3.Z < 800)
		{
			Projectile.hostile = true;
			// Vector2 pos = Projection2(v3, Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2, out float scale, 1000);
			Projectile.Center = new(v3.X, v3.Y);
		}
		else
		{
			Projectile.hostile = false;
		}

		if (Projectile.timeLeft < 60)
		{
			Projectile.scale -= 0.015f;
			Projectile.hostile = false;
		}
	}

	private static Vector4 VecRotByYoZ(Vector4 vec, float rot)
	{
		Vector2 v = new Vector2(vec.X, vec.W).RotatedBy(rot);
		return new Vector4(v.X, vec.Y, vec.Z, v.Y);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		var origin = new Vector2(tex.Width / 2, tex.Height / 6);
		Rectangle sourceRec = tex.Frame(1, 4, 0, Projectile.frame % 4);
		Vector3 v3 = Projection(v4Position * Projectile.ai[1] + new Vector4(Owner.Center, 0, 0), 1000);
		if (v3.Z < 800)
		{
			Vector2 pos = Projection2(v3, Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2, out float scale, 1000);

			var c = new Color(55, 125, 255, 0);
			Main.spriteBatch.Draw(Light, pos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, c * 0.2f, Projectile.rotation, Light.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(tex, pos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRec, c, Projectile.rotation, origin, Projectile.scale * scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		else
		{
		}
		return false;
	}

	public void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex, float AddValue = 0)
	{
		float Wid = 24f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();
		float Value0 = (float)(Main.timeForVisualEffects / 291d + 20 + AddValue) % 1f;
		float Value1 = (float)(Main.timeForVisualEffects / 291d + 20.1 + AddValue) % 1f;
		if (Value1 < Value0)
		{
			float D0 = 1 - Value0;
			Vector2 Delta = EndPos - StartPos;

			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width, color2, new Vector3(1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width, color2, new Vector3(1, 1, 0)));

			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 + Width, color1, new Vector3(0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos + Delta * D0 - Width, color1, new Vector3(0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(Value1, 1, 0)));
		}
		else
		{
			vertex2Ds.Add(new Vertex2D(StartPos + Width, color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width, color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width, color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width, color2, new Vector3(Value1, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertex2Ds.ToArray(), 0, vertex2Ds.Count - 2);
	}

	private Vector3 Projection(Vector4 vec, float viewZ)
	{
		float k1 = -viewZ / (vec.W - viewZ);
		var v3 = new Vector3(vec.X, vec.Y, vec.Z);
		v3 += (k1 - 1) * (v3 - new Vector3(Owner.Center, 0));
		return v3;
	}

	private static Vector2 Projection2(Vector3 v3, Vector2 center, out float scale, float viewZ)
	{
		float k2 = -viewZ / (v3.Z - viewZ);
		scale = k2;
		var v = new Vector2(v3.X, v3.Y);
		return v + (k2 - 1) * (v - center);
	}
}
