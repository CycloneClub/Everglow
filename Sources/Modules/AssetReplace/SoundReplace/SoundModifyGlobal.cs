using Terraria.Audio;
using Terraria.UI;

namespace Everglow.AssetReplace.SoundReplace;

public abstract class SoundModifyGlobal : GlobalItem, IModifyItemPickSound
{
	public abstract SoundStyle PickSound();

	public abstract string TxtFileName();

	private int[] ItemIDs = Array.Empty<int>();

	public override void Load()
	{
		this.ReadFromTxtFile(TxtFileName(), out ItemIDs);
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) =>
		lateInstantiation && ItemIDs.Contains(entity.type);

	public void ModifyItemPickSound(Item item, int context, bool putIn, ref SoundStyle? customSound, ref bool playOriginalSound)
	{
		if (context == ItemSlot.Context.InventoryItem)
		{
			playOriginalSound = false;
			SoundEngine.PlaySound(PickSound());
		}
	}

	public void Dispose()
	{
	}
}

public class BalloonPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Balloon", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "BalloonSoundID";
}

public class BonePickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Bone", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "BoneSoundID";
}

public class DustPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Dust", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "DustSoundID";
}

public class FurPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Fur", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "FurSoundID";
}

public class GlassPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Glass", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.3f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "GlassSoundID";
}

public class MagicPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Magic", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "MagicSoundID";
}

public class MeatPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Meat", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "MeatSoundID";
}

public class MetalPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Metal", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.3f, 0.5f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "MetalSoundID";
}

public class PaperPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Paper", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "PaperSoundID";
}

public class PotionPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Potion", SoundType.Sound)
	{
		Volume = 0.8f,
		PitchRange = (-0.3f, 0.3f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "PotionSoundID";
}

public class SilkPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Silk", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "SilkSoundID";
}

public class SlimePickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Slime", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "SlimeSoundID";
}

public class StonePickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Stone", SoundType.Sound)
	{
		Volume = 1f,
		PitchRange = (-0.1f, 0.1f),
		MaxInstances = 0,
	};

	public override string TxtFileName() => "StoneSoundID";
}

public class WaterBucketPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/WaterBucket", SoundType.Sound)
	{
		Volume = 0.8f,
		PitchRange = (-0.2f, 0.1f),
		MaxInstances = 2,
	};

	public override string TxtFileName() => "WaterBucketSoundID";
}

public class LavaBucketPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/LavaBucket", SoundType.Sound)
	{
		Volume = 0.8f,
		PitchRange = (-0.2f, 0.1f),
		MaxInstances = 2,
	};

	public override string TxtFileName() => "LavaBucketSoundID";
}

public class WoodPickSoundModify : SoundModifyGlobal
{
	public override SoundStyle PickSound() => new($"Everglow/AssetReplace/Resources/Sounds/PickSound/Wood", SoundType.Sound)
	{
		Volume = 0.8f,
		PitchVariance = 0.4f,
		MaxInstances = 0,
	};

	public override string TxtFileName() => "WoodSoundID";
}
