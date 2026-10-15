using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

[Pipeline(typeof(WCSPipeline))]
public class FemaleLampLeaves_leaf : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void OnSpawn()
	{
		Texture = ModAsset.FemaleLampWood_leaves_dark.Value;
	}

	public override void Update()
	{
		foreach (Player player in Main.player)
		{
			if (player != null && player.active && !player.dead)
			{
				if (Collision.CheckAABBvLineCollision(player.position, new Vector2(player.Hitbox.Width, player.Hitbox.Height), Position, Position + new Vector2(80, 140).RotatedBy(rotation) * scale))
				{
					omega -= Vector3.Cross(new Vector3(player.velocity, 0), new Vector3(new Vector2(80, 140).RotatedBy(rotation), 0)).Z / 30000f;
				}
			}
		}
		omega += (startRotation - rotation) / 100f;
		rotation += omega;
		rotation = rotation * 0.92f + startRotation * 0.08f;
		base.Update();
	}

	public float rotation;
	public float omega;
	public float startRotation;
	public float scale;
	public int Style;
	public bool Flip_H;

	public override void Draw()
	{
		Rectangle frame = new Rectangle(Style * 80, 0, 80, 140);
		float frameY = 0f;
		if (Style >= 6)
		{
			frameY = 0.5f;
		}

		// 镜像处理真是麻烦
		Vector2 point2 = new Vector2(frame.Width, 0);
		Vector2 point3 = new Vector2(0, frame.Height);
		Vector2 point4 = new Vector2(frame.Width, frame.Height);

		if (Flip_H)
		{
			point2 = Vector2.Normalize(point4).RotatedBy(point4.ToRotation() - point2.ToRotation()) * frame.Width;
			point3 = Vector2.Normalize(point4).RotatedBy(point4.ToRotation() - point3.ToRotation()) * frame.Height;
		}

		Color lightColor0 = Lighting.GetColor(Position.ToTileCoordinates());
		Color lightColor1 = Lighting.GetColor((Position + point2.RotatedBy(rotation) * scale).ToTileCoordinates());
		Color lightColor2 = Lighting.GetColor((Position + point3.RotatedBy(rotation) * scale).ToTileCoordinates());
		Color lightColor3 = Lighting.GetColor((Position + point4.RotatedBy(rotation) * scale).ToTileCoordinates());
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position, lightColor0, new Vector3((0 + Style) / 6f, frameY, 0)),
			new Vertex2D(Position + point2.RotatedBy(rotation) * scale, lightColor1, new Vector3((1 + Style) / 6f, frameY, 0)),

			new Vertex2D(Position + point3.RotatedBy(rotation) * scale, lightColor2, new Vector3((0 + Style) / 6f, frameY + 0.5f, 0)),
			new Vertex2D(Position + new Vector2(frame.Width, frame.Height).RotatedBy(rotation) * scale, lightColor3, new Vector3((1 + Style) / 6f, frameY + 0.5f, 0)),
		};
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}

public class FemaleLampLeaves_leaf_fore : TileVFX
{

	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public override void OnSpawn()
	{
		Texture = ModAsset.FemaleLampWood_leaves.Value;
	}

	public override void Update()
	{
		float stability = 3f;
		if (Style >= 6)
		{
			stability = 18f;
		}
		if (Style >= 12)
		{
			Lighting.AddLight(Position + new Vector2(0, 100).RotatedBy(rotation - 0.4f), new Vector3(1f, 0.8f, 0.3f));
		}
		foreach (Player player in Main.player)
		{
			if (player != null && player.active && !player.dead)
			{
				if (Collision.CheckAABBvLineCollision(player.position, new Vector2(player.Hitbox.Width, player.Hitbox.Height), Position, Position + new Vector2(80, 140).RotatedBy(rotation) * scale))
				{
					omega -= Vector3.Cross(new Vector3(player.velocity, 0), new Vector3(new Vector2(80, 140).RotatedBy(rotation), 0)).Z / stability / 10000f;
				}
			}
		}
		omega += (startRotation - rotation) / 100f;
		rotation += omega;
		rotation = rotation * 0.92f + startRotation * 0.08f;
		base.Update();
	}

	public float rotation;
	public float omega;
	public float startRotation;
	public float scale;
	public int Style;
	public bool Flip_H;

	public override void Draw()
	{
		Rectangle frame = new Rectangle(Style * 80, 0, 80, 140);

		// 镜像处理真是麻烦
		Vector2 point2 = new Vector2(frame.Width, 0);
		Vector2 point3 = new Vector2(0, frame.Height);
		Vector2 point4 = new Vector2(frame.Width, frame.Height);

		if (Flip_H)
		{
			point2 = Vector2.Normalize(point4).RotatedBy(point4.ToRotation() - point2.ToRotation()) * frame.Width;
			point3 = Vector2.Normalize(point4).RotatedBy(point4.ToRotation() - point3.ToRotation()) * frame.Height;
		}
		float frameY = 0f;
		if (Style >= 6)
		{
			if (Style < 12)
			{
				int newStyle = Style - 6;
				frameY = 0.25f;
				float newRot = rotation - 0.4f;
				Vector2 dirH = new Vector2(-30 * scale * (Flip_H ? -1 : 1), 0).RotatedBy(newRot);
				Vector2 dirV = new Vector2(0, 210 * scale).RotatedBy(newRot);
				Color lightColor0 = Lighting.GetColor((Position + dirH).ToTileCoordinates());
				Color lightColor1 = Lighting.GetColor((Position - dirH).ToTileCoordinates());
				Color lightColor2 = Lighting.GetColor((Position + dirH + dirV).ToTileCoordinates());
				Color lightColor3 = Lighting.GetColor((Position - dirH + dirV).ToTileCoordinates());
				List<Vertex2D> bars = new List<Vertex2D>()
				{
					new Vertex2D(Position + dirH, lightColor0, new Vector3((0 + newStyle) / 6f, frameY + 0.25f, 0)),
					new Vertex2D(Position - dirH, lightColor1, new Vector3((1 + newStyle) / 6f, frameY + 0.25f, 0)),

					new Vertex2D(Position + dirH + dirV, lightColor2, new Vector3((0 + newStyle) / 6f, frameY, 0)),
					new Vertex2D(Position - dirH + dirV, lightColor3, new Vector3((1 + newStyle) / 6f, frameY, 0)),
				};
				Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
			}
			else
			{
				int newStyle = Style - 12;
				frameY = 0.5f;
				float newRot = rotation - 0.4f;
				Vector2 dirH = new Vector2(-120 * scale * (Flip_H ? -1 : 1), 0).RotatedBy(newRot);
				Vector2 dirV = new Vector2(0, 140 * scale).RotatedBy(newRot);
				Color lightColor0 = Lighting.GetColor((Position + dirH).ToTileCoordinates());
				Color lightColor1 = Lighting.GetColor((Position - dirH).ToTileCoordinates());
				Color lightColor2 = Lighting.GetColor((Position + dirH + dirV).ToTileCoordinates());
				Color lightColor3 = Lighting.GetColor((Position - dirH + dirV).ToTileCoordinates());
				Color glowColor = new Color(1f, 1f, 1f, 0);
				List<Vertex2D> bars = new List<Vertex2D>()
				{
					new Vertex2D(Position + dirH, lightColor0, new Vector3((0 + newStyle) / 2f, frameY + 0.25f, 0)),
					new Vertex2D(Position - dirH, lightColor1, new Vector3((1 + newStyle) / 2f, frameY + 0.25f, 0)),
					new Vertex2D(Position + dirH + dirV, lightColor2, new Vector3((0 + newStyle) / 2f, frameY, 0)),

					new Vertex2D(Position + dirH + dirV, lightColor2, new Vector3((0 + newStyle) / 2f, frameY, 0)),
					new Vertex2D(Position - dirH + dirV, lightColor3, new Vector3((1 + newStyle) / 2f, frameY, 0)),
					new Vertex2D(Position - dirH, lightColor1, new Vector3((1 + newStyle) / 2f, frameY + 0.25f, 0)),

					new Vertex2D(Position + dirH, glowColor, new Vector3((0 + newStyle) / 2f, frameY + 0.5f, 0)),
					new Vertex2D(Position - dirH, glowColor, new Vector3((1 + newStyle) / 2f, frameY + 0.5f, 0)),
					new Vertex2D(Position + dirH + dirV, glowColor, new Vector3((0 + newStyle) / 2f, frameY + 0.25f, 0)),

					new Vertex2D(Position + dirH + dirV, glowColor, new Vector3((0 + newStyle) / 2f, frameY + 0.25f, 0)),
					new Vertex2D(Position - dirH + dirV, glowColor, new Vector3((1 + newStyle) / 2f, frameY + 0.25f, 0)),
					new Vertex2D(Position - dirH, glowColor, new Vector3((1 + newStyle) / 2f, frameY + 0.5f, 0)),
				};
				Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
			}
		}
		else
		{
			Color lightColor0 = Lighting.GetColor(Position.ToTileCoordinates());
			Color lightColor1 = Lighting.GetColor((Position + point2.RotatedBy(rotation) * scale).ToTileCoordinates());
			Color lightColor2 = Lighting.GetColor((Position + point3.RotatedBy(rotation) * scale).ToTileCoordinates());
			Color lightColor3 = Lighting.GetColor((Position + point4.RotatedBy(rotation) * scale).ToTileCoordinates());
			List<Vertex2D> bars = new List<Vertex2D>()
			{
				new Vertex2D(Position, lightColor0, new Vector3((0 + Style) / 6f, frameY, 0)),
				new Vertex2D(Position + point2.RotatedBy(rotation) * scale, lightColor1, new Vector3((1 + Style) / 6f, frameY, 0)),

				new Vertex2D(Position + point3.RotatedBy(rotation) * scale, lightColor2, new Vector3((0 + Style) / 6f, frameY + 0.25f, 0)),
				new Vertex2D(Position + new Vector2(frame.Width, frame.Height).RotatedBy(rotation) * scale, lightColor3, new Vector3((1 + Style) / 6f, frameY + 0.25f, 0)),
			};
			Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
		}
	}
}
