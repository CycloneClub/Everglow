namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.RandomNPC;

public class YggdrasilTownPersonManager
{
	public static List<VFXPerson> People = new List<VFXPerson>();

	public static void AddRandomPerson()
	{
		VFXPerson person = new VFXPerson()
		{
			Active = true,
			Visible = true,
			Position = Main.MouseWorld,
			Velocity = Vector2.Zero,
			Timer = 0,
			MaxTime = 3000000,
			State = (int)VFXPerson.MoveState.Walk,
			SkinColor = ChooseSkinColor(),
			HairColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 0.25f), MathF.Pow(Main.rand.NextFloat(), 0.5f)).HSVToRGB_Color(1f),
			ClothColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 2f), MathF.Pow(Main.rand.NextFloat(), 1.2f)).HSVToRGB_Color(1f),
			ShoeColor = new Vector3(Main.rand.NextFloat(-30, 60), MathF.Pow(Main.rand.NextFloat(), 2f), MathF.Pow(Main.rand.NextFloat(), 2.5f)).HSVToRGB_Color(1f),
			EyeColor = new Vector3(Main.rand.NextFloat(360), MathF.Pow(Main.rand.NextFloat(), 0.5f), Main.rand.NextFloat()).HSVToRGB_Color(1f),
			HairStyle = Main.rand.Next(165),
			Direction = Main.rand.NextBool() ? -1 : 1,
			Sex = Main.rand.NextBool() ? 0 : 1,
			ShoeStyle = Main.rand.Next(8),
		};
		Ins.VFXManager.Add(person);
		People.Add(person);
	}

	public static Color ChooseSkinColor()
	{
		switch (Main.rand.Next(7))
		{
			case 0:
				return new Color(209, 160, 156);
			case 1:
				return new Color(255, 221, 221);
			case 2:
				return new Color(255, 221, 193);
			case 3:
				return new Color(206, 135, 111);
			case 4:
				return new Color(104, 66, 75);
			case 5:
				return new Color(232, 205, 176);
			case 6:
				return new Color(232, 202, 194);
		}

		return new Color(209, 160, 156);
	}

	public static void Update()
	{
		for (int k = People.Count - 1; k >= 0; k--)
		{
			if (!People[k].Active)
			{
				People.RemoveAt(k);
			}
		}
	}
}
