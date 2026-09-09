using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown;

public class FurnacePlayer : ModPlayer
{
	public int CurrentFurnaceScore;
	public int TotalFurnaceScore;

	public List<bool> ReceivedReward = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false };

	public override void Load()
	{
		base.Load();
	}

	public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
	{
		// ModPacket packet = Mod.GetPacket();
		// packet.Write(MessageID.PlayerLifeMana);
		// packet.Write((byte)Player.whoAmI);
		// packet.Write((byte)TotalFurnaceScore);
		// packet.Send(toWho, fromWho);
	}

	// Called in ExampleMod.Networking.cs
	public void ReceivePlayerSync(BinaryReader reader)
	{
		TotalFurnaceScore = reader.ReadByte();
	}

	public override void CopyClientState(ModPlayer targetCopy)
	{
		var clone = (FurnacePlayer)targetCopy;
		clone.TotalFurnaceScore = TotalFurnaceScore;
	}

	public override void SendClientChanges(ModPlayer clientPlayer)
	{
		var clone = (FurnacePlayer)clientPlayer;

		if (TotalFurnaceScore != clone.TotalFurnaceScore)
		{
			SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
		}
	}

	public override void SaveData(TagCompound tag)
	{
		tag["TotalFurnaceScore"] = TotalFurnaceScore;
		tag["CurrentFurnaceScore"] = CurrentFurnaceScore;
		tag.Add("ReceivedReward", ReceivedReward);
	}

	public override void LoadData(TagCompound tag)
	{
		TotalFurnaceScore = tag.GetInt("TotalFurnaceScore");
		CurrentFurnaceScore = tag.GetInt("CurrentFurnaceScore");
		ReceivedReward = tag.Get<List<bool>>("ReceivedReward");
		if (ReceivedReward.Count < 14)
		{
			ReceivedReward = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		}
	}
}
