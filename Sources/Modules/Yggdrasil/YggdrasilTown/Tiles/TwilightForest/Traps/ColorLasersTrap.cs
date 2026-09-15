using Everglow.Commons.VFX.Scene;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Traps;

[Pipeline(typeof(WCSPipeline))]

public class ColorLasersTrap : TileVFX
{
	public float Rotation;
	public float Omega;
	public float StartRotation;
	public int Style;
	private int length;

	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public override void OnSpawn()
	{
		Texture = ModAsset.ColorLasersTrap.Value;
	}

	public Vector3 GetColor()
	{
		Vector3 colorV3 = new Vector3(0);
		switch (Style)
		{
			case 0:
				return new Vector3(1f, 0, 0);
			case 1:
				return new Vector3(0.01f, 0.63f, 0.3f);
			case 2:
				return new Vector3(0f, 0.3f, 1);
			case 3:
				return new Vector3(0.85f, 0.57f, 0);
			case 4:
				return new Vector3(0.25f, 0f, 0.95f);
			case 5:
				return new Vector3(1f, 0.3f, 0f);
			case 6:
				return new Vector3(0.3f, 1f, 1f);
			case 7:
				return new Vector3(0.6f, 1f, 0.1f);
		}
		return colorV3;
	}

	public override void Update()
	{
		Lighting.AddLight(Position, GetColor());
		Position = OriginTilePos.ToWorldCoordinates() + new Vector2(2) + new Vector2(0, -16).RotatedBy(Rotation);
		Rotation = StartRotation;
		Vector2 collisionUnit = new Vector2(0, -8).RotatedBy(Rotation);
		int count = 0;
		for (int step = 1; step < 1000; step++)
		{
			count++;
			if (Collision.SolidCollision(Position + step * collisionUnit - new Vector2(4), 8, 8))
			{
				break;
			}
		}
		length = count;
		foreach (var player in Main.player)
		{
			if (player != null && player.active && player.GetModPlayer<ColorLaserPlayer>().ImmuneStyle != Style)
			{
				if (Collision.CheckAABBvLineCollision(player.Hitbox.TopLeft(), player.Hitbox.Size(), Position, Position + collisionUnit * length))
				{
					player.Hurt(PlayerDeathReason.ByCustomReason("Try to across in a laser net"), 999, player.velocity.X > 0 ? 1 : -1, false, false, -1, false, 999, 0, 0);
				}
			}
		}
		base.Update();
	}

	public override void Draw()
	{
		var frame = new Rectangle(Style * 18, 0, 16, 16);

		var point0 = new Vector2(-frame.Width * 0.5f, -frame.Height * 0.5f);
		var point1 = new Vector2(frame.Width * 0.5f, -frame.Height * 0.5f);
		var point2 = new Vector2(-frame.Width * 0.5f, frame.Height * 0.5f);
		var point3 = new Vector2(frame.Width * 0.5f, frame.Height * 0.5f);

		Color lightColor0 = Lighting.GetColor((Position + point0.RotatedBy(Rotation)).ToTileCoordinates());
		Color lightColor1 = Lighting.GetColor((Position + point1.RotatedBy(Rotation)).ToTileCoordinates());
		Color lightColor2 = Lighting.GetColor((Position + point2.RotatedBy(Rotation)).ToTileCoordinates());
		Color lightColor3 = Lighting.GetColor((Position + point3.RotatedBy(Rotation)).ToTileCoordinates());
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + point0.RotatedBy(Rotation), lightColor0, new Vector3(frame.X / (float)Texture.Width, 0, 0)),
			new Vertex2D(Position + point1.RotatedBy(Rotation), lightColor1, new Vector3((frame.X + 16) / (float)Texture.Width, 0, 0)),
			new Vertex2D(Position + point2.RotatedBy(Rotation), lightColor2, new Vector3(frame.X / (float)Texture.Width, 16f / Texture.Height, 0)),

			new Vertex2D(Position + point2.RotatedBy(Rotation), lightColor2, new Vector3(frame.X / (float)Texture.Width, 16f / Texture.Height, 0)),
			new Vertex2D(Position + point1.RotatedBy(Rotation), lightColor1, new Vector3((frame.X + 16) / (float)Texture.Width, 0, 0)),
			new Vertex2D(Position + point3.RotatedBy(Rotation), lightColor3, new Vector3((frame.X + 16) / (float)Texture.Width, 16f / Texture.Height, 0)),
		};
		Vector2 collisionUnit = new Vector2(0, -8).RotatedBy(Rotation);
		Vector2 normalUnit = collisionUnit.RotatedBy(MathHelper.PiOver2) * 0.5f;
		Color laserColor = new Color(GetColor().X, GetColor().Y, GetColor().Z, 0);
		if (Main.LocalPlayer.GetModPlayer<ColorLaserPlayer>().ImmuneStyle == Style)
		{
			laserColor *= 0.2f;
		}
		bars.Add(Position + normalUnit, laserColor, new Vector3(0, 18f / Texture.Height, 0));
		bars.Add(Position - normalUnit, laserColor, new Vector3(0, 152f / Texture.Height, 0));
		bars.Add(Position + normalUnit + collisionUnit * length, laserColor, new Vector3(length / 24f, 18f / Texture.Height, 0));

		bars.Add(Position + normalUnit + collisionUnit * length, laserColor, new Vector3(length / 24f, 18f / Texture.Height, 0));
		bars.Add(Position - normalUnit, laserColor, new Vector3(0, 152f / Texture.Height, 0));
		bars.Add(Position - normalUnit + collisionUnit * length, laserColor, new Vector3(length / 24f, 152f / Texture.Height, 0));

		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}
}

public class ColorLaserPlayer : ModPlayer
{
	public int ImmuneStyle = -1;

	public int ImmuneTimer = 0;

	public override void ResetEffects()
	{
		if (ImmuneTimer > 0)
		{
			ImmuneTimer--;
		}
		else
		{
			ImmuneTimer = 0;
			ImmuneStyle = -1;
		}
	}

	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		base.ModifyDrawInfo(ref drawInfo);
	}
}
