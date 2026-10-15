using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.IRProbe;

[Pipeline(typeof(WCSPipeline))]

public class IRProbe_Normal_Laser : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public float Rotation;
	public float Omega;
	public float StartRotation;
	public int Style;
	public bool AnyPlayerCollision = false;
	public bool NoneRotation;

	/// <summary>
	/// Allow to make a custon logic for the behaviors of laser.
	/// </summary>
	public delegate void ScanningLogic(IRProbe_Normal_Laser laser);

	public event ScanningLogic LaserScan;

	private int length;

	public override void OnSpawn()
	{
		Texture = ModAsset.IRProbe_Normal.Value;
	}

	public Vector3 GetColor()
	{
		var colorV3 = new Vector3(0);
		switch (Style)
		{
			case 0:
				return new Vector3(0.5f, 0, 0);
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
		bool oldPlayerHit = AnyPlayerCollision;
		Position = OriginTilePos.ToWorldCoordinates();
		if (!NoneRotation)
		{
			Position += new Vector2(0, -8).RotatedBy(Rotation);
		}
		Rotation = StartRotation;
		LaserScan?.Invoke(this);
		if (!NoneRotation)
		{
			Vector2 collisionUnit = new Vector2(0, -8).RotatedBy(Rotation);
			int count = 0;
			for (int step = 1; step < 1000; step++)
			{
				count++;
				if (Collision.SolidCollision(Position + step * collisionUnit - new Vector2(4), 8, 8))
				{
					AnyPlayerCollision = false;
					break;
				}
				Vector2 pos = Position + step * collisionUnit - new Vector2(4);
				var collisionRectangle = new Rectangle((int)pos.X, (int)pos.Y, 8, 8);
				bool playerCollision = false;
				foreach (var player in Main.player)
				{
					if (player != null && player.active)
					{
						if (Rectangle.Intersect(player.Hitbox, collisionRectangle) != Rectangle.emptyRectangle)
						{
							playerCollision = true;
							break;
						}
					}
				}
				if (playerCollision)
				{
					AnyPlayerCollision = true;
					break;
				}
			}
			length = count;
		}
		else
		{
			length = 0;
			AnyPlayerCollision = false;
			Vector2 pos = Position - new Vector2(4);
			var collisionRectangle = new Rectangle((int)pos.X, (int)pos.Y, 8, 8);
			bool playerCollision = false;
			foreach (var player in Main.player)
			{
				if (player != null && player.active)
				{
					if (Rectangle.Intersect(player.Hitbox, collisionRectangle) != Rectangle.emptyRectangle)
					{
						playerCollision = true;
						break;
					}
				}
			}
			if (playerCollision)
			{
				AnyPlayerCollision = true;
			}
		}
		if (AnyPlayerCollision && !oldPlayerHit)
		{
			Wiring.TripWire(OriginTilePos.X, OriginTilePos.Y, 1, 1);
		}
		base.Update();
	}

	public override void Draw()
	{
		var frame = new Rectangle(0, 74, 16, 16);
		var laserColor = new Color(GetColor().X, GetColor().Y, GetColor().Z, 0) * 0.2f;
		if (Main.LocalPlayer.GetModPlayer<VisionPlayer>().IR_Vision)
		{
			laserColor = new Color(GetColor().X, GetColor().Y, GetColor().Z, 0);
		}
		if (NoneRotation)
		{
			if (!AnyPlayerCollision)
			{
				frame = new Rectangle(0, 120, 72, 22);
				float subRot = Rotation + MathHelper.PiOver4;
				Vector2 starX = new Vector2(36, 0).RotatedBy(subRot);
				Vector2 starY = new Vector2(0, 11).RotatedBy(subRot);
				var star = new List<Vertex2D>();
				star.Add(Position - starX - starY, laserColor, new Vector3(frame.X / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX + starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

				subRot = Rotation - MathHelper.PiOver4;
				starX = new Vector2(36, 0).RotatedBy(subRot);
				starY = new Vector2(0, 11).RotatedBy(subRot);
				star.Add(Position - starX - starY, laserColor, new Vector3(frame.X / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX + starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

				subRot = Rotation;
				starX = new Vector2(6, 0).RotatedBy(subRot);
				starY = new Vector2(0, 6).RotatedBy(subRot);
				frame = new Rectangle(0, 94, 24, 24);
				laserColor = Color.Lerp(laserColor, new Color(1f, 1f, 1f, 0), 0.2f);
				star.Add(Position - starX - starY, laserColor, new Vector3(frame.X / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

				star.Add(Position - starX + starY, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
				star.Add(Position + starX - starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
				star.Add(Position + starX + starY, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
				Ins.Batch.Draw(Texture, star, PrimitiveType.TriangleList);
			}
			return;
		}
		Vector2 stepPos = Position;
		Vector2 collisionUnit = new Vector2(0, -8).RotatedBy(Rotation);
		Vector2 collisionUnit_T = new Vector2(0, -8).RotatedBy(Rotation + MathHelper.PiOver2);
		var bars = new List<Vertex2D>();
		bars.Add(stepPos + collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
		bars.Add(stepPos + collisionUnit + collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
		bars.Add(stepPos - collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

		bars.Add(stepPos - collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
		bars.Add(stepPos + collisionUnit + collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
		bars.Add(stepPos + collisionUnit - collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
		frame = new Rectangle(28, 74, 16, 16);
		for (int i = 0; i < length; i++)
		{
			stepPos += collisionUnit;
			if (i == length - 1)
			{
				frame = new Rectangle(56, 74, 16, 16);
			}
			bars.Add(stepPos + collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
			bars.Add(stepPos + collisionUnit + collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
			bars.Add(stepPos - collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));

			bars.Add(stepPos - collisionUnit_T, laserColor, new Vector3(frame.X / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
			bars.Add(stepPos + collisionUnit + collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, frame.Y / (float)Texture.Height, 0));
			bars.Add(stepPos + collisionUnit - collisionUnit_T, laserColor, new Vector3((frame.X + frame.Width) / (float)Texture.Width, (frame.Y + frame.Height) / (float)Texture.Height, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleList);
	}

	public override void Kill()
	{
		UnregisterCustomDraw(LaserScan);
		base.Kill();
	}

	/// <summary>
	/// Allow you register a custom logic.
	/// </summary>
	/// <param name="customLogic"></param>
	public void RegisterCustomLogic(ScanningLogic customLogic)
	{
		LaserScan += customLogic;
	}

	/// <summary>
	/// Unregister a custom logic.
	/// </summary>
	/// <param name="customLogic"></param>
	public void UnregisterCustomDraw(ScanningLogic customLogic)
	{
		LaserScan -= customLogic;
	}
}
