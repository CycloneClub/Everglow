using Everglow.Commons.Vertex;
using Terraria.DataStructures;

namespace Everglow.MEAC.NonTrueMeleeProj;

public class GoldShield_backTextureSubProj : ModProjectile
{
	public override string Texture => ModAsset.GoldShield_Mod;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1200;
		Projectile.penetrate = -1;
	}

	public Projectile MainProj;

	public override void OnSpawn(IEntitySource source)
	{
		if (Main.projectile == null)
		{
			foreach (var projectile in Main.projectile)
			{
				if (projectile != null && projectile.active)
				{
					if (projectile.owner == Projectile.owner)
					{
						if (Projectile.type == ModContent.ProjectileType<GoldShield>())
						{
							MainProj = projectile;
							break;
						}
					}
				}
			}
		}
		base.OnSpawn(source);
	}

	public override void AI()
	{
		if (MainProj == null || !MainProj.active)
		{
			Projectile.Kill();
		}
		Projectile.Center = Main.player[Projectile.owner].Center;
	}

	public void DrawPost(Color color, int widthCount, float halfHeight, float initialPhase, Texture2D texture)
	{
		if (!Projectile.active || Projectile == null)
		{
			return;
		}
		var vertex2Ds = new List<Vertex2D>();
		Vector2 DrawCen = Main.player[Projectile.owner].Center - Main.screenPosition;
		DrawCen += new Vector2(0, -10);
		float SPos = initialPhase;
		float R = widthCount;
		for (int x = -widthCount; x < widthCount; x++)
		{
			float y = (float)Math.Sqrt(R * R - x * x);
			float newy = (float)Math.Sqrt(R * R - (x + 1) * (x + 1));

			float r1 = (float)(Math.Acos(Math.Clamp(x / R, -1, 1)) / Math.PI);
			float r2 = (float)(Math.Acos(Math.Clamp((x + 1) / R, -1, 1)) / Math.PI);

			float deltaY = newy - y;
			float length = (float)Math.Sqrt(deltaY * deltaY + 1);
			const float scale = 0.19f;
			const float frequency = 0.3f;

			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, halfHeight), color, new Vector3(r1 * frequency, 0, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, halfHeight), color, new Vector3(r2 * frequency, 0, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, -halfHeight), color, new Vector3(r2 * frequency, 1, 0)));

			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale + 0.5f, -halfHeight), color, new Vector3(r2 * frequency, 1, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, halfHeight), color, new Vector3(r1 * frequency, 0, 0)));
			vertex2Ds.Add(new Vertex2D(DrawCen + new Vector2(x * scale - 0.5f, -halfHeight), color, new Vector3(r1 * frequency, 1, 0)));
			SPos += length;
		}
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (Ins.VisualQuality.High)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect Post = ModAsset.Post_xnb.Value;
			Post.Parameters["uTime"].SetValue(-(float)Main.time * 0.002f);
			Post.CurrentTechnique.Passes["Test2"].Apply();
			if (GoldShield.ShieldTexture != null)
			{
				DrawPost(new Color(255, 255, 255, 255), 200, 40, 1, GoldShield.ShieldTexture);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		return false;
	}
}
