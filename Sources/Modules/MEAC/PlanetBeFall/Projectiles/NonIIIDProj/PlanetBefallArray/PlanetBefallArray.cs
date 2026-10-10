using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.PlanetBefallArray
{
	public class PlanetBefallArray : ModProjectile // ,IBloomProjectile
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 300;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
		}

		internal float Timer = 0;
		internal float alpha = 1;
		public float BloomIntensity = 1;
		public int PlanetBeFallProj;

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.velocity *= 0;
			float f = 0.025f;
			if (Timer < 40)
			{
				Timer = MathHelper.Lerp(Timer, 20, f);
			}
			if (Projectile.timeLeft < 150)
			{
				alpha *= 0.95f;
			}

			if (!Main.projectile[PlanetBeFallProj].active)
			{
				Projectile.ai[0]++;
				BloomIntensity = MathF.Sin(Projectile.ai[0] / (5 * MathF.PI)) / 3f + 1;
			}
		}

		public override void OnKill(int timeLeft)
		{
			BloomIntensity = 0;
			base.OnKill(timeLeft);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.hide = false;

			DrawMagicArray();

			return false;
		}

		public void DrawBloom()
		{
			Color c = Color.White;
			PreDraw(ref c);
		}

		public void DrawMagicArray()
		{
			Player player = Main.player[Projectile.owner];
			Texture2D PlantBeFallIn = ModAsset.PlantBeFallIn.Value;
			Texture2D PlantBeFallOut = ModAsset.PlantBeFallOut.Value;
			Texture2D GeoElement = ModAsset.GeoElement.Value;
			Texture2D GeoElement_black = ModAsset.GeoElement_black.Value;
			Texture2D GeoElement_glow = ModAsset.GeoElement_glow.Value;
			float scaleGeo = 0.8f;
			float timeValue = Projectile.timeLeft / 300f;
			Vector2 drawCenter = Projectile.Center - Main.screenPosition + new Vector2(0, -100);
			float mulGeoColor = MathF.Sin(MathF.Pow(Math.Clamp(1.4f - timeValue, 0, 1), 5f) * MathHelper.Pi);
			if (mulGeoColor < 0)
			{
				mulGeoColor = 0;
			}
			Color geoColor = new Color(0.9f * MathF.Sin(timeValue * MathHelper.Pi), 0.4f * mulGeoColor, 0, 0f) * mulGeoColor * 2f;
			Main.spriteBatch.Draw(GeoElement_black, drawCenter, null, Color.White * 0.3f * mulGeoColor, 0, GeoElement_black.Size() / 2f, scaleGeo, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(GeoElement, drawCenter, null, geoColor, 0, GeoElement.Size() / 2f, scaleGeo, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(GeoElement_glow, drawCenter, null, geoColor * 0.2f, 0, GeoElement_glow.Size() / 2f, scaleGeo, SpriteEffects.None, 0);

			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			float mulOutSideCircleColor = MathF.Sin(MathF.Pow(Math.Clamp(1.4f - timeValue, 0, 1), 8f) * MathHelper.Pi);

			Color outideCircleColor = new Color(0.5f, 0.2f, 0.1f, 0f) * mulOutSideCircleColor * 3f * alpha;

			DrawTexCircle(610, 120, Color.White * alpha, drawCenter, ModAsset.PlantBeFallOut_black.Value, Timer * 0.1f);
			DrawTexCircle(610, 120, outideCircleColor, drawCenter, PlantBeFallOut, Timer * 0.1f);
			for (int radious = 450; radious < 620; radious += 50)
			{
				float mulCircleColor = MathF.Sin(MathF.Pow(Math.Clamp(2.1f - radious / 800f - timeValue, 0, 1), 24f) * MathHelper.Pi);
				Color circleColor = new Color(0.9f, 0.6f, 0.1f, 0f) * mulCircleColor * 0.5f;
				DrawTexCircle(radious, 120, Color.White * mulCircleColor * 0.5f * alpha, Projectile.Center - Main.screenPosition - new Vector2(0, 100), ModAsset.PlantBeFall_black.Value, 0);
				DrawTexCircle(radious, 120, circleColor * alpha, Projectile.Center - Main.screenPosition - new Vector2(0, 100), ModAsset.PlantBeFall_line.Value, 0);
			}
			float mulInsideCircleColor = MathF.Sin(MathF.Pow(Math.Clamp(1.4f - timeValue, 0, 1), 10f) * MathHelper.Pi);
			float mulInsideCircleColorBlack = MathF.Sin(MathF.Pow(Math.Clamp(1.35f - timeValue, 0, 1), 3f) * MathHelper.Pi);
			Color insideCircleColor = new Color(0.9f, 0.4f, 0.1f, 0f) * mulInsideCircleColor * 2f * alpha;
			var barsInside = new List<Vertex2D>();
			float range = 550;
			Vector2 Point1 = drawCenter + new Vector2(range).RotatedBy(Math.PI * 0);
			Vector2 Point2 = drawCenter + new Vector2(range).RotatedBy(Math.PI * 1 / 2d);
			Vector2 Point3 = drawCenter + new Vector2(range).RotatedBy(Math.PI * 2 / 2d);
			Vector2 Point4 = drawCenter + new Vector2(range).RotatedBy(Math.PI * 3 / 2d);
			barsInside.Add(new Vertex2D(Point1, Color.White * mulInsideCircleColorBlack * alpha, new Vector3(0, 0, 0)));
			barsInside.Add(new Vertex2D(Point2, Color.White * mulInsideCircleColorBlack * alpha, new Vector3(1, 0, 0)));
			barsInside.Add(new Vertex2D(Point4, Color.White * mulInsideCircleColorBlack * alpha, new Vector3(0, 1, 0)));
			barsInside.Add(new Vertex2D(Point3, Color.White * mulInsideCircleColorBlack * alpha, new Vector3(1, 1, 0)));
			if (barsInside.Count > 0)
			{
				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.PlantBeFallIn_black.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsInside.ToArray(), 0, 2);
			}
			barsInside = new List<Vertex2D>();
			barsInside.Add(new Vertex2D(Point1 + Main.screenPosition, insideCircleColor, new Vector3(0, 0, 0)));
			barsInside.Add(new Vertex2D(Point2 + Main.screenPosition, insideCircleColor, new Vector3(1, 0, 0)));
			barsInside.Add(new Vertex2D(Point4 + Main.screenPosition, insideCircleColor, new Vector3(0, 1, 0)));
			barsInside.Add(new Vertex2D(Point3 + Main.screenPosition, insideCircleColor, new Vector3(1, 1, 0)));
			if (barsInside.Count > 0)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
				Effect dissolve = ModAsset.PlantBeFallInsideDissolve.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
				dissolve.Parameters["uTransform"].SetValue(model * projection);
				dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_perlin.Value);
				dissolve.Parameters["uTime"].SetValue((1 - timeValue) * 1.5f);
				dissolve.CurrentTechnique.Passes[0].Apply();

				Main.graphics.GraphicsDevice.Textures[0] = PlantBeFallIn;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsInside.ToArray(), 0, 2);
				Main.graphics.GraphicsDevice.Textures[0] = ModAsset.PlantBeFallIn_glow.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsInside.ToArray(), 0, 2);
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}

		private static void DrawTexCircle(float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
		{
			var circle = new List<Vertex2D>();
			for (int h = 0; h < radious / 2; h++)
			{
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 1, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0, 0)));
			}
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
			if (circle.Count > 0)
			{
				Main.graphics.GraphicsDevice.Textures[0] = tex;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 4);
			}
		}
	}
}
