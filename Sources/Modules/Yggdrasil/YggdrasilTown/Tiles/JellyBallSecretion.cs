using System.Collections.Generic;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Audio;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallSecretion : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		TileID.Sets.NegatesFallDamage[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;

		DustType = ModContent.DustType<JellyBallGel>();
		HitSound = SoundID.NPCHit1;

		AddMapEntry(new Color(25, 39, 160), ModContent.GetInstance<Items.Placeables.JellyBallCube>().DisplayName);
	}

	internal static bool IsJelly(Point p) => WorldGen.InWorld(p.X, p.Y, 1)
		&& Main.tile[p.X, p.Y].HasUnactuatedTile && Main.tile[p.X, p.Y].TileType == ModContent.TileType<JellyBallSecretion>();

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.dedServ)
		{
			return true;
		}

		Lighting.AddLight(i, j, 0, 0.5f, 0.94f);
		var tile = Main.tile[i, j];
		var ripples = ModContent.GetInstance<JellyRipples>();
		var point = new Point(i, j);
		if (tile.IsHalfBlock || tile.Slope != SlopeType.Solid || !tile.HasUnactuatedTile || !TileDrawing.IsVisible(tile) || !ripples.TryGetWave(point, out var face, out float amount, out float tilt))
		{
			return true;
		}

		Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 position = point.ToVector2() * 16 - Main.screenPosition + offset;
		var texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
		var light = tile.IsTileFullbright ? Color.White : Lighting.GetColor(i, j);
		var tint = Color.Lerp(light, Color.White, Math.Min(Math.Abs(amount) * 0.035f, 0.22f));
		var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
		if (WorldGen.SolidTile(i - face.X, j - face.Y))
		{
			spriteBatch.Draw(texture, position, frame, tint);
			return false;
		}

		// Only move the exposed edge. The opposite edge stays joined to the next tile.
		bool vertical = face.Y != 0;
		for (int strip = 0; strip < 16; strip += 4)
		{
			float inset = MathHelper.Clamp(amount + tilt * ((strip + 2) / 16f - 0.5f), -6f, 6f);
			var source = vertical ? new Rectangle(frame.X + strip, frame.Y, 4, 16) : new Rectangle(frame.X, frame.Y + strip, 16, 4);
			var stripPosition = vertical ? new Vector2(strip, face.Y > 0 ? inset : 0) : new Vector2(face.X > 0 ? inset : 0, strip);
			var scale = vertical ? new Vector2(1, 1 - inset / 16f) : new Vector2(1 - inset / 16f, 1);
			spriteBatch.Draw(texture, position + stripPosition, source, tint, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Tile tileLeft = Main.tile[i + 1, j];
		Tile tileRight = Main.tile[i - 1, j];
		if (tile.Slope == SlopeType.Solid && tileLeft.TileType == Type && tileRight.TileType == Type && tileLeft.HasTile && tileRight.HasTile)
		{
			if (Main.rand.NextBool(1))
			{
				Tile t2 = Main.tile[i, j + 1];
				if (!t2.HasTile)
				{
					t2.TileType = (ushort)ModContent.TileType<JellyBeadString>();
					t2.HasTile = true;
				}
			}
			if (Main.rand.NextBool(1))
			{
				Tile t2 = Main.tile[i, j - 1];
				if (!t2.HasTile)
				{
					t2.TileType = (ushort)ModContent.TileType<HaloGrass>();
					t2.TileFrameX = (short)(18 * Main.rand.Next(6));
					t2.HasTile = true;
				}
			}
		}
	}
}

public class JellyRipples : ModSystem
{
	private const int MaxPulses = 24;
	private readonly Queue<Pulse> pulses = new();

	private sealed record Pulse(ulong Start, Point Origin, Point Direction, float Amplitude, Dictionary<Point, int> Distances);

	public override void ClearWorld() => pulses.Clear();

	public override void PostUpdateEverything()
	{
		while (pulses.TryPeek(out var pulse) && Main.GameUpdateCount - pulse.Start >= 52)
		{
			pulses.Dequeue();
		}
	}

	internal void Emit(Point origin, Point direction, float speed)
	{
		int radius = speed >= 6f ? 12 : 5;
		var distances = new Dictionary<Point, int> { [origin] = 0 };
		var frontier = new Queue<Point>();
		frontier.Enqueue(origin);
		while (frontier.TryDequeue(out var p))
		{
			int next = distances[p] + 1;
			if (next > radius)
			{
				continue;
			}

			Visit(new Point(p.X + 1, p.Y));
			Visit(new Point(p.X - 1, p.Y));
			Visit(new Point(p.X, p.Y + 1));
			Visit(new Point(p.X, p.Y - 1));

			void Visit(Point neighbor)
			{
				if (JellyBallSecretion.IsJelly(neighbor) && distances.TryAdd(neighbor, next))
				{
					frontier.Enqueue(neighbor);
				}
			}
		}

		if (pulses.Count >= MaxPulses)
		{
			pulses.Dequeue();
		}

		pulses.Enqueue(new Pulse(Main.GameUpdateCount, origin, direction, MathHelper.Clamp(speed * 0.8f, 2f, 8f), distances));
	}

	internal bool TryGetWave(Point tile, out Point face, out float amount, out float tilt)
	{
		face = default;
		amount = tilt = 0f;
		float strongest = 0.15f;
		foreach (var pulse in pulses)
		{
			if (pulse.Distances.TryGetValue(tile, out int distance))
			{
				float age = (Main.GameUpdateCount - pulse.Start) - distance * 1.25f;
				if (age < 0)
				{
					continue;
				}

				float envelope = pulse.Amplitude * MathF.Exp(-age * 0.085f) / (1f + distance * 0.22f);
				if (envelope > strongest)
				{
					strongest = envelope;
					face = pulse.Direction;
					float phase = age * 0.56f;
					amount = envelope * MathF.Sin(phase);
					int tangent = face.Y != 0 ? tile.X - pulse.Origin.X : tile.Y - pulse.Origin.Y;
					tilt = -Math.Sign(tangent) * envelope * 0.7f * MathF.Cos(phase);
				}
			}
		}

		return face != default;
	}
}

public class JellyBallSecretionPlayer : ModPlayer
{
	private const float ImpactSpeed = 6f;
	private const float Restitution = 0.9f;
	private Vector2 incoming;
	private Vector2 startPosition;
	private float stepDistance;

	public override void PreUpdateMovement()
	{
		incoming = Player.velocity;
		startPosition = Player.position;
	}

	public override void PostUpdate()
	{
		if (Player.dead || Player.ghost || Main.dedServ)
		{
			return;
		}

		bool impact = false;
		bool bounced = false;
		Point source = default;
		if (HitFace(incoming.X, Player.position.X - startPosition.X) && TryContact(true, Math.Sign(incoming.X), out var wall))
		{
			bounced = Math.Abs(incoming.X) >= ImpactSpeed;
			if (bounced && Player.whoAmI == Main.myPlayer)
			{
				Player.velocity.X = -incoming.X * Restitution;
			}

			impact = true;
			source = wall;
			ModContent.GetInstance<JellyRipples>().Emit(wall, new Point(Math.Sign(incoming.X), 0), Math.Abs(incoming.X));
		}

		if (HitFace(incoming.Y, Player.position.Y - startPosition.Y) && TryContact(false, Math.Sign(incoming.Y), out var floor))
		{
			bool bounceY = Math.Abs(incoming.Y) >= ImpactSpeed;
			bounced |= bounceY;
			if (bounceY && Player.whoAmI == Main.myPlayer)
			{
				Player.velocity.Y = -incoming.Y * Restitution;
				if (incoming.Y * Player.gravDir > 0)
				{
					Player.fallStart = (int)(Player.position.Y / 16f);
				}
			}

			impact = true;
			source = floor;
			ModContent.GetInstance<JellyRipples>().Emit(floor, new Point(0, Math.Sign(incoming.Y)), Math.Abs(incoming.Y));
		}

		if (impact)
		{
			stepDistance = 0;
			if (bounced)
			{
				SoundEngine.PlaySound(SoundID.NPCHit1, source.ToWorldCoordinates());
			}
		}
		else if (Math.Abs(Player.velocity.Y) < 0.05f && TryContact(false, (int)Player.gravDir, out var ground))
		{
			stepDistance += Math.Abs(Player.velocity.X);
			if (stepDistance >= 24f)
			{
				stepDistance %= 24f;
				ModContent.GetInstance<JellyRipples>().Emit(ground, new Point(0, (int)Player.gravDir), 2f);
			}
		}
		else
		{
			stepDistance = 0;
		}
	}

	private static bool HitFace(float speed, float travelled) =>
		Math.Abs(speed) >= 2f && travelled * Math.Sign(speed) < Math.Abs(speed) - 0.001f;

	private bool TryContact(bool horizontal, int direction, out Point contact)
	{
		float face = horizontal
			? (direction > 0 ? Player.Right.X : Player.Left.X)
			: (direction > 0 ? Player.Bottom.Y : Player.Top.Y);
		float start = horizontal ? Player.Top.Y : Player.Left.X;
		float end = horizontal ? Player.Bottom.Y : Player.Right.X;
		int fixedTile = (int)MathF.Floor((face + direction * 0.1f) / 16f);
		for (int t = (int)MathF.Floor((start + 0.01f) / 16f); t <= (int)MathF.Floor((end - 0.01f) / 16f); t++)
		{
			contact = horizontal ? new Point(fixedTile, t) : new Point(t, fixedTile);
			if (JellyBallSecretion.IsJelly(contact))
			{
				return true;
			}
		}

		contact = default;
		return false;
	}
}
