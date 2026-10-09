using Everglow.Commons.DataStructures;
using Everglow.Commons.MEAC.VFX;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.Commons.MEAC;

public abstract partial class MeleeProj_3D : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	public bool EnableSphereCoordDraw = false;

	public bool SelfLuminous = false;

	public bool Visible = true;

	public float ReflectionSharpValue = 1f;

	public Color SlashColor = new Color(1f, 1f, 1f, 0);

	public Matrix ProjectionMatrix()
	{
		return Matrix.CreatePerspectiveFieldOfView(
		   MeleeProj_3D_Configs.AngleofFOV,
		   Main.screenWidth / (float)Main.screenHeight,
		   0.1f,
		   2000f);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (!Visible)
		{
			return false;
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (EnableSphereCoordDraw)
		{
			DrawReferenceSphere(RadialDistance);
		}
		DrawWeapon(WeaponAxis, MainAxis * 0.15f, new Vector3(0, 0, CenterZ));
		DrawTrail();

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		for (int k = SlashEffects.Count - 1; k >= 0; k--)
		{
			SlashEffect sEffect = SlashEffects[k];
			if (sEffect.SlashTrail_Smoothed is null || !sEffect.Active)
			{
				continue;
			}
			int starIndex = sEffect.SlashTrail_Smoothed.Count - 1;
			if (starIndex < 0)
			{
				continue;
			}
			Vector3 wldPos3D = sEffect.SlashTrail_Smoothed[starIndex] + new Vector3(0, 0, CenterZ);
			Vector2 wldPos = Project(wldPos3D, ProjectionMatrix()) + Projectile.Center;
			float starScale = 1f;
			Color starColor = SlashColor;
			float threthod = 40;
			if (sEffect.Timer > sEffect.MaxTime - threthod)
			{
				float value = 1 - (sEffect.Timer - sEffect.MaxTime + threthod) / threthod;
				value -= 0.2f;
				if (value < 0)
				{
					value = 0;
				}
				starColor *= value;
				value -= 0.5f;
				if (value < 0)
				{
					value = 0;
				}
				starScale *= value;
			}
			if (!SelfLuminous)
			{
				Color lightC = Lighting.GetColor(wldPos.ToTileCoordinates());
				starColor.R = (byte)(lightC.R * starColor.R / 255f);
				starColor.G = (byte)(lightC.G * starColor.G / 255f);
				starColor.B = (byte)(lightC.B * starColor.B / 255f);
			}
			starColor *= 1.2f;
			Texture2D star = ModAsset.StarSlash.Value;
			Main.spriteBatch.Draw(star, wldPos - Main.screenPosition, null, starColor, 0, star.Size() * 0.5f, starScale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, wldPos - Main.screenPosition, null, starColor, MathHelper.PiOver2, star.Size() * 0.5f, starScale, SpriteEffects.None, 0);
		}

		return false;
	}

	public virtual void CustomDustDraw(MeleeProj_3D_Dust dust)
	{
		Vector3 wldPos3D = dust.Position_Space + new Vector3(0, 0, CenterZ);
		Vector2 wldPos = Project(wldPos3D, ProjectionMatrix()) + Projectile.Center;
		Texture2D tex_black = ModAsset.NormalDust_small_black.Value;
		Texture2D tex = ModAsset.NormalDust_small.Value;
		var dustColor = SlashColor;
		if (!SelfLuminous)
		{
			Color lightC = Lighting.GetColor(wldPos.ToTileCoordinates());
			dustColor.R = (byte)(lightC.R * dustColor.R / 255f);
			dustColor.G = (byte)(lightC.G * dustColor.G / 255f);
			dustColor.B = (byte)(lightC.B * dustColor.B / 255f);
		}
		float colorVariation = MathF.Sin((dust.MaxTime - dust.Timer) * 1f) + 1;
		colorVariation *= 0.5f;
		colorVariation = MathF.Pow(colorVariation, 1);
		dustColor *= ReflectionSharpValue * 5f * colorVariation;
		Ins.Batch.Draw(tex_black, wldPos, null, Color.White * 0.3f, dust.Rotation, tex_black.Size() * 0.5f, dust.Scale, SpriteEffects.None);
		Ins.Batch.Draw(tex, wldPos, null, dustColor, dust.Rotation, tex.Size() * 0.5f, dust.Scale, SpriteEffects.None);
	}

	public virtual void EnchantmentDustDraw(MeleeProj_3D_Dust dust)
	{
		Vector3 wldPos3D = dust.Position_Space + new Vector3(0, 0, CenterZ);
		Vector2 wldPos = Project(wldPos3D, ProjectionMatrix()) + Projectile.Center;
		Texture2D tex = TextureAssets.Dust.Value;
		Rectangle frame = new Rectangle(0, (int)(dust.ai[2] * 10), 10, 10);
		Color dustColor = new Color(1f, 1f, 1f, 0.5f);
		float sizeMul = 1f;
		if (dust.MaxTime - dust.Timer < 10)
		{
			sizeMul = (dust.MaxTime - dust.Timer) / 10f;
		}

		// Ins.Batch.Draw(tex_black, wldPos, null, Color.White, dust.Rotation, tex_black.Size() * 0.5f, dust.Scale * sizeMul, SpriteEffects.None);
		switch (dust.EnchantmentType)
		{
			case 1: // Venom
				frame.X = (DustID.Venom % 100) * 10;
				frame.Y += (DustID.Venom - (DustID.Venom % 100)) / 100 * 30;
				dustColor = Lighting.GetColor(wldPos.ToTileCoordinates()) * 0.5f;
				dustColor.A = 150;
				sizeMul = MathF.Sin(dust.Timer / dust.MaxTime * MathHelper.Pi);
				break;
			case 2: // Cursed Flames
				frame.X = DustID.CursedTorch * 10;
				break;
			case 3: // Fire
				frame.X = DustID.Torch * 10;
				break;
			case 4: // Gold
				frame.X = (DustID.GoldCoin % 100) * 10;
				frame.Y += (DustID.GoldCoin - (DustID.GoldCoin % 100)) / 100 * 30;
				dustColor = Lighting.GetColor(wldPos.ToTileCoordinates()) * 0.5f;
				dustColor.A = 150;
				break;
			case 5: // Ichor
				frame.X = (DustID.Ichor % 100) * 10;
				frame.Y += (DustID.Ichor - (DustID.Ichor % 100)) / 100 * 30;
				break;
			case 6: // Nanites
				frame.X = (DustID.IceTorch % 100) * 10;
				frame.Y += (DustID.IceTorch - (DustID.IceTorch % 100)) / 100 * 30;
				break;
			case 7: // Party
				int type = 139; // confetti
				type += (int)dust.ai[3];
				frame.X = (type % 100) * 10;
				frame.Y += (type - (type % 100)) / 100 * 30;
				dustColor = Lighting.GetColor(wldPos.ToTileCoordinates());
				if (dust.MaxTime - dust.Timer < 30)
				{
					sizeMul = (dust.MaxTime - dust.Timer) / 30f;
				}
				break;
			case 8: // Poison
				frame.X = (DustID.Poisoned % 100) * 10;
				frame.Y += (DustID.Poisoned - (DustID.Poisoned % 100)) / 100 * 30;
				dustColor = Lighting.GetColor(wldPos.ToTileCoordinates()) * 0.5f;
				dustColor.A = 150;
				sizeMul = MathF.Sin(dust.Timer / dust.MaxTime * MathHelper.Pi);
				break;
		}
		Ins.Batch.Draw(tex, wldPos, frame, dustColor, dust.Rotation, frame.Size() * 0.5f, dust.Scale * sizeMul, SpriteEffects.None);
	}

	public void DrawReferenceSphere(float radius = 120f)
	{
		Color drawColor = new Color(0.1f, 0.3f, 0.4f, 0);

		List<Vector2> basicRing = new List<Vector2>();
		for (int i = 0; i < 30; i++)
		{
			Vector2 ringPos = new Vector2(radius, 0).RotatedBy(i / 30f * MathHelper.TwoPi);
			basicRing.Add(ringPos);
		}

		Main.graphics.graphicsDevice.Textures[0] = ModAsset.StarSlashGray.Value;

		// Latitude Rings
		for (int l = 0; l < 9; l++)
		{
			List<Vector3> latitudeRing = ToLatitudeRing(basicRing, (l + 0.5f) / 9f * MathHelper.Pi - MathHelper.PiOver2, radius);
			Draw3DCurve(latitudeRing, drawColor, ScreenPositionOffset);
		}

		// LongitudeRing Rings
		for (int l = 0; l < 9; l++)
		{
			List<Vector3> longitudeRing = ToLongitudeRing(basicRing, l / 9f * MathHelper.Pi + (float)Main.time * 0.003f);
			Draw3DCurve(longitudeRing, drawColor, ScreenPositionOffset);
		}

		// Shear Surface
		List<Vector2> shearRing = new List<Vector2>();
		for (int i = 0; i < basicRing.Count; i++)
		{
			Vector2 ringPos = Project(new Vector3(basicRing[i], CenterZ), ProjectionMatrix());
			shearRing.Add(ringPos);
		}
		List<Vertex2D> shearSurface = new List<Vertex2D>();
		for (int i = 0; i <= shearRing.Count; i++)
		{
			Vector2 ringPos = shearRing[i % shearRing.Count];
			shearSurface.Add(ringPos + ScreenPositionOffset, drawColor, new Vector3(0.5f, 0.5f, 0));
			shearSurface.Add(ringPos * 0.9f + ScreenPositionOffset, drawColor, new Vector3(0.7f, 0.5f, 0));
		}
		if (shearSurface.Count >= 4)
		{
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.StarSlashGray.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, shearSurface.ToArray(), 0, shearSurface.Count - 2);
		}

		// Current
		DrawPositionArrow(drawColor);
		Draw3DLine(RotatedAxis, Vector3.zero, Color.Red * 0.5f, new Vector3(0, 0, CenterZ));
		Draw3DLine(WeaponAxis, MainAxis, Color.Purple * 0.5f, new Vector3(0, 0, CenterZ));
		Color currentColor = new Color(1f, 0.05f, 0.1f, 0);
		List<Vector3> latitudeRing_current = ToLatitudeRing(basicRing, MathHelper.PiOver2 - PolarAngle, RadialDistance);
		DrawCurrentCoordRing(latitudeRing_current, currentColor, ScreenPositionOffset, 15);

		List<Vector3> longitudeRing_current = ToLongitudeRing(basicRing, AzimuthalAngle + MathHelper.PiOver2);
		DrawCurrentCoordRing(longitudeRing_current, currentColor, ScreenPositionOffset, 15);
	}

	public void DrawPositionText(Vector2 drawPos)
	{
		var coordText = MainAxis.ToString();
		coordText += "\n[R:" + SphericalCoordPos.X + " ,Theta:" + SphericalCoordPos.Y / MathHelper.TwoPi * 360 + " ,Phi:" + SphericalCoordPos.Z / MathHelper.TwoPi * 360 + "]";
		Main.spriteBatch.DrawString(FontAssets.MouseText.Value, coordText, drawPos + ScreenPositionOffset, Color.White, 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
	}

	public void DrawPositionArrow(Color drawColor)
	{
		Draw3DLine(MainAxis, Vector3.zero, drawColor, new Vector3(0, 0, CenterZ));
	}

	public void Draw3DLine(Vector3 start, Vector3 end, Color drawColor, Vector3 offset)
	{
		List<Vertex2D> arrow = new List<Vertex2D>();
		Vector2 start2 = Project(start + offset, ProjectionMatrix());
		Vector2 end2 = Project(end + offset, ProjectionMatrix());
		Vector2 arrowWidth = (start2 - end2).NormalizeSafe().RotatedBy(MathHelper.PiOver2);
		float startScale = GetSizeZ((start + offset).Z);
		float endScale = GetSizeZ((end + offset).Z);

		arrow.Add(start2 + arrowWidth * startScale + ScreenPositionOffset, drawColor, new Vector3(0.5f, 0.5f, 0));
		arrow.Add(start2 - arrowWidth * startScale + ScreenPositionOffset, drawColor, new Vector3(0.5f, 0.5f, 0));

		arrow.Add(end2 + arrowWidth * endScale + ScreenPositionOffset, drawColor, new Vector3(0.5f, 0.5f, 0));
		arrow.Add(end2 - arrowWidth * endScale + ScreenPositionOffset, drawColor, new Vector3(0.5f, 0.5f, 0));
		if (arrow.Count >= 4)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arrow.ToArray(), 0, arrow.Count - 2);
		}
	}

	public virtual void DrawTrail()
	{
		for (int k = SlashEffects.Count - 1; k >= 0; k--)
		{
			SlashEffect sEffect = SlashEffects[k];
			if (sEffect.SlashTrail_Smoothed is null || !sEffect.Active)
			{
				continue;
			}

			// Draw Slash
			List<Vertex2D> trails_black = new List<Vertex2D>();
			List<Vertex2D> trails = new List<Vertex2D>();
			List<Vertex2D> trails_tip_black = new List<Vertex2D>();
			List<Vertex2D> trails_tip = new List<Vertex2D>();
			List<Vertex2D> trails_reflection = new List<Vertex2D>();
			float timeValue = 0; // -(float)Main.time * 0.03f;
			for (int i = 0; i < sEffect.SlashTrail_Smoothed.Count; i++)
			{
				Vector3 currentPos3D = sEffect.SlashTrail_Smoothed[i] + new Vector3(0, 0, CenterZ);
				Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());

				Vector3 currentPos3D_Edge = sEffect.SlashTrail_Smoothed[i] * 0.6f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos_Edge = Project(currentPos3D_Edge, ProjectionMatrix());

				Vector3 currentPos3D_Inner = sEffect.SlashTrail_Smoothed[i] * 0.2f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos_Inner = Project(currentPos3D_Inner, ProjectionMatrix());

				float value = i / (float)sEffect.SlashTrail_Smoothed.Count;
				float fade = GetFade(i, sEffect);
				Color drawColor_dark = GetTrailColor(0, currentPos + Projectile.Center, i, ref value, fade);
				Color drawColor_dark_Inner = GetTrailColor(0, currentPos_Inner + Projectile.Center, i, ref value, 0);
				Color drawColor = GetTrailColor(1, currentPos + Projectile.Center, i, ref value, fade);
				Color drawColor_Inner = GetTrailColor(1, currentPos_Inner + Projectile.Center, i, ref value, 0);
				Color drawColor_Edge = GetTrailColor(3, currentPos + Projectile.Center, i, ref value, fade);
				Color drawColor_reflection = GetTrailColor(4, currentPos + Projectile.Center, i, ref value, fade);
				Color drawColor_reflection_Inner = GetTrailColor(4, currentPos_Inner + Projectile.Center, i, ref value, 0);

				trails_black.Add(currentPos + ScreenPositionOffset, drawColor_dark, new Vector3(value + timeValue, 1f, 0));
				trails_black.Add(currentPos_Inner + ScreenPositionOffset, drawColor_dark_Inner, new Vector3(value + timeValue, 0f, 0));

				trails.Add(currentPos + ScreenPositionOffset, drawColor, new Vector3(value + timeValue, 1f, 0));
				trails.Add(currentPos_Inner + ScreenPositionOffset, drawColor_Inner, new Vector3(value + timeValue, 0f, 0));

				trails_tip_black.Add(currentPos + ScreenPositionOffset, drawColor_dark, new Vector3(0.5f, 0.5f + value * 0.5f, 0));
				trails_tip_black.Add(currentPos_Edge + ScreenPositionOffset, drawColor_dark_Inner, new Vector3(0.3f, 0.5f + value * 0.5f, 0));

				trails_tip.Add(currentPos + ScreenPositionOffset, drawColor_Edge, new Vector3(0.5f, 0.5f + value * 0.5f, 0));
				trails_tip.Add(currentPos_Edge + ScreenPositionOffset, drawColor_Inner, new Vector3(0.3f, 0.5f + value * 0.5f, 0));

				trails_reflection.Add(currentPos + ScreenPositionOffset, drawColor_reflection, new Vector3(value + timeValue, 1f, 0));
				trails_reflection.Add(currentPos_Inner + ScreenPositionOffset, drawColor_reflection_Inner, new Vector3(value + timeValue, 0f, 0));
			}
			DrawSlashVertices(trails_black, ModAsset.Noise_flame_3_black.Value);
			DrawSlashVertices(trails, ModAsset.Noise_flame_3.Value);
			DrawSlashVertices(trails_tip, ModAsset.StarSlash.Value);
			DrawSlashVertices(trails_tip_black, ModAsset.StarSlash_black.Value);
			DrawSlashVertices(trails_tip, ModAsset.StarSlash.Value);
			DrawSlashVertices(trails_reflection, ModAsset.Noise_flame_3.Value);
		}
	}

	public void DrawSlashVertices(List<Vertex2D> bars, Texture2D texture)
	{
		if (bars.Count >= 4)
		{
			Main.graphics.graphicsDevice.Textures[0] = texture;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	/// <summary>
	/// Get the color while adding trailing primitives.Allow adding custon logics for trail color.<br></br>
	/// style == 0: Black background<br></br>
	/// style == 1: Normal<br></br>
	/// style == 2: Bloom<br></br>
	/// style == 3: Edge<br></br>
	/// style == 4: Reflection<br></br>
	/// style >= 5: Custom
	/// </summary>
	/// <param name="style"></param>
	/// <param name="worldPos"></param>
	/// <param name="index"></param>
	/// <param name="factor"></param>
	/// <param name="extraValue0">Fade multiplier for the background, normal, edge, and reflection colors; also controls reflection intensity.</param>
	/// <param name="extraValue1">Additional value for custom color overrides; unused by the base implementation.</param>
	/// <returns></returns>
	public virtual Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		Color drawColor = Color.White;
		if (style == 0)
		{
			drawColor *= factor * extraValue0;
		}
		if (style == 1 || style == 3 || style == 4)
		{
			drawColor = SlashColor;
			drawColor *= factor * extraValue0;
			if (!SelfLuminous)
			{
				Color lightC = Lighting.GetColor(worldPos.ToTileCoordinates());
				drawColor.R = (byte)(lightC.R * drawColor.R / 255f);
				drawColor.G = (byte)(lightC.G * drawColor.G / 255f);
				drawColor.B = (byte)(lightC.B * drawColor.B / 255f);
			}
			if (style == 3)
			{
				drawColor *= 1.7f;
			}
			if (style == 4)
			{
				float rot = (worldPos - Projectile.Center).ToRotation() - (float)Main.time * 0.01f;
				drawColor *= MathF.Pow(Math.Max(0, 0.5f + 0.5f * MathF.Cos(rot * 2)), 16) * 0.4f * ReflectionSharpValue * MathF.Pow(extraValue0, 2);
				drawColor.A = 0;
			}
		}
		return drawColor;
	}

	/// <summary>
	/// Allow you modify the trailing coords.<br></br>
	/// phase:0,1→bars0;<br></br>
	/// phase:2,3→bars1;<br></br>
	/// phase:4,5→bars2;<br></br>
	/// timeValue default to Tiemr * 0.05f
	/// </summary>
	/// <param name="factor"></param>
	/// <param name="timeValue"></param>
	/// <param name="phase"></param>
	/// <param name="widthValue"></param>
	/// <returns></returns>
	public virtual Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor + timeValue;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public virtual void DrawWeapon(Vector3 start, Vector3 end, Vector3 offset)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Vector2 arrowTip = Project(start + offset, ProjectionMatrix());
		Vector2 center = Project(end + offset, ProjectionMatrix());
		Vector2 middle = (arrowTip + center) / 2f;
		float width = tex.Size().Length() / 2f;
		Vector2 dirWidth = (arrowTip - center).NormalizeSafe().RotatedBy(MathHelper.PiOver2) * width;

		int middleCoord0 = 0;
		int middleCoord1 = 1;
		if (Owner.direction == 1)
		{
			(middleCoord0, middleCoord1) = (middleCoord1, middleCoord0);
		}

		List<Vertex2D> weapon = new List<Vertex2D>();
		AddVertex(weapon, center + ScreenPositionOffset, new Vector3(0, 1, 0));
		AddVertex(weapon, middle + dirWidth + ScreenPositionOffset, new Vector3(middleCoord0, middleCoord0, 0));

		AddVertex(weapon, middle - dirWidth + ScreenPositionOffset, new Vector3(middleCoord1, middleCoord1, 0));
		AddVertex(weapon, arrowTip + ScreenPositionOffset, new Vector3(1, 0, 0));

		if (weapon.Count >= 4)
		{
			Main.graphics.graphicsDevice.Textures[0] = tex;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, weapon.ToArray(), 0, weapon.Count - 2);
		}
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 screenPos, Vector3 coord)
	{
		bars.Add(screenPos, Lighting.GetColor((screenPos + Main.screenPosition).ToTileCoordinates()), coord);
	}

	public void Draw3DCurve(List<Vector3> curve, Color drawColor, Vector2 ScreenPositionOffset, float lineWidth = 3f)
	{
		for (int r = 0; r < 2; r++)
		{
			Vector2 normal = new Vector2(0, lineWidth).RotatedBy(r / 2f + MathHelper.Pi);
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int i = 0; i <= curve.Count; i++)
			{
				Vector3 ringPos3D = curve[i % curve.Count];
				Vector2 ringPos2D = Project(ringPos3D, ProjectionMatrix());
				var drawColor2 = drawColor;
				if (ringPos3D.Z > CenterZ)
				{
					drawColor2 *= 0.3f;
				}
				float size = GetSizeZ(ringPos3D.Z);
				bars.Add(ringPos2D + ScreenPositionOffset, drawColor2, new Vector3(0.5f, 0.5f, 0));
				bars.Add(ringPos2D - normal * size + ScreenPositionOffset, drawColor2, new Vector3(0.7f, 0.5f, 0));
			}
			if (bars.Count >= 4)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}

	public void DrawCurrentCoordRing(List<Vector3> curve, Color drawColor, Vector2 ScreenPositionOffset, float lineWidth = 3f)
	{
		for (int r = 0; r < 2; r++)
		{
			Vector2 normal = new Vector2(0, lineWidth).RotatedBy(r / 2f + MathHelper.Pi);
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int i = 0; i <= curve.Count; i++)
			{
				Vector3 ringPos3D = curve[i % curve.Count];
				Vector2 ringPos2D = Project(ringPos3D, ProjectionMatrix());
				var drawColor2 = drawColor;
				if ((ringPos3D.Z - CenterZ) * MainAxis.Z < 0)
				{
					drawColor2 *= 0.1f;
				}
				float size = GetSizeZ(ringPos3D.Z);
				bars.Add(ringPos2D + ScreenPositionOffset, drawColor2, new Vector3(0.5f, 0.5f, 0));
				bars.Add(ringPos2D - normal * size + ScreenPositionOffset, drawColor2, new Vector3(0.7f, 0.5f, 0));
			}
			if (bars.Count >= 4)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}

	public float GetFade(int indexInSmootheTrail, SlashEffect sEffect)
	{
		float value = indexInSmootheTrail / (float)sEffect.SlashTrail_Smoothed.Count * sEffect.SlashFade.Count;
		if (sEffect.SlashFade is null || sEffect.SlashFade.Count == 0)
		{
			return 0f;
		}
		float fade = 1f;
		float fadeThrethod = 24f;
		if (sEffect.Timer >= sEffect.MaxTime - fadeThrethod)
		{
			fade *= 1 - (sEffect.Timer - sEffect.MaxTime + fadeThrethod) / fadeThrethod;
		}
		int index = (int)Math.Clamp(value, 0, sEffect.SlashFade.Count - 1);
		return sEffect.SlashFade.ToArray()[index] * fade;
	}

	public virtual void DrawBloom()
	{
		if (SelfLuminous)
		{
			DrawTrail();
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float timeValue = 0;
		for (int k = 0; k < SlashEffects.Count; k++)
		{
			SlashEffect sEffect = SlashEffects[k];
			if (sEffect.SlashTrail_Smoothed is null)
			{
				continue;
			}
			if (!sEffect.Active)
			{
				continue;
			}
			List<Vertex2D> trails = new List<Vertex2D>();

			for (int i = 0; i < sEffect.SlashTrail_Smoothed.Count; i++)
			{
				Vector3 currentPos3D = sEffect.SlashTrail_Smoothed[i] + new Vector3(0, 0, CenterZ);
				Vector2 currentPos = Project(currentPos3D, ProjectionMatrix());

				Vector3 currentPos3D_Inner = sEffect.SlashTrail_Smoothed[i] * 0.2f + new Vector3(0, 0, CenterZ);
				Vector2 currentPos_Inner = Project(currentPos3D_Inner, ProjectionMatrix());

				Vector2 warpDir = Vector2.zeroVector;
				if (i >= 1)
				{
					Vector3 currentPos3D_Old = sEffect.SlashTrail_Smoothed[i - 1] + new Vector3(0, 0, CenterZ);
					Vector2 currentPos_Old = Project(currentPos3D_Old, ProjectionMatrix());
					warpDir = currentPos_Old - currentPos;
				}
				float warpThrethod = 220;
				if (warpDir.Length() > warpThrethod)
				{
					warpDir = warpDir.SafeNormalize(Vector2.zeroVector) * warpThrethod;
				}
				warpDir *= 0.5f / warpThrethod;
				float value = 1 - i / (float)sEffect.SlashTrail_Smoothed.Count;
				float fade = 1f;
				float fadeThrethod = 24f;
				if (sEffect.Timer >= sEffect.MaxTime - fadeThrethod)
				{
					fade *= 1 - (sEffect.Timer - sEffect.MaxTime + fadeThrethod) / fadeThrethod;
				}
				Color drawColor = new Color(0.5f + warpDir.X, 0.5f + warpDir.Y, (1 - value) * 1f * fade, 1);
				Color drawColorInner = new Color(0.5f + warpDir.X, 0.5f + warpDir.Y, 0, 1);

				trails.Add(currentPos + ScreenPositionOffset, drawColor, new Vector3(value + timeValue, 1f, 0));
				trails.Add(currentPos_Inner + ScreenPositionOffset, drawColorInner, new Vector3(value + timeValue, 0f, 0));
			}

			if (trails.Count > 2)
			{
				spriteBatch.Draw(ModAsset.Noise_flame_3.Value, trails, PrimitiveType.TriangleStrip);
			}
		}
	}
}
