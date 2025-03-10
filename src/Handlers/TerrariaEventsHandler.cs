using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Net;

namespace Crossplay.Handlers
{
    static class TerrariaEventsHandler
    {
        public static void RegisterHandlers()
        {
            On.Terraria.Net.NetManager.Broadcast_NetPacket_int += OnBroadcast;
            On.Terraria.Net.NetManager.SendToClient += OnSendToClient;
        }

        public static void UnregisterHandlers()
        {
            On.Terraria.Net.NetManager.Broadcast_NetPacket_int -= OnBroadcast;
            On.Terraria.Net.NetManager.SendToClient -= OnSendToClient;
        }

        private static void OnBroadcast(On.Terraria.Net.NetManager.orig_Broadcast_NetPacket_int orig, NetManager self, NetPacket packet, int ignoreClient)
        {
            for (int i = 0; i <= Main.maxPlayers; i++)
            {
                if (i != ignoreClient && Netplay.Clients[i].IsConnected() && !InvalidNetPacket(packet, i))
                {
                    self.SendData(Netplay.Clients[i].Socket, packet);
                }
            }
        }

        private static void OnSendToClient(On.Terraria.Net.NetManager.orig_SendToClient orig, NetManager self, NetPacket packet, int playerId)
        {
            if (!InvalidNetPacket(packet, playerId))
            {
                orig(self, packet, playerId);
            }
        }

        private static bool InvalidNetPacket(NetPacket packet, int playerId)
        {
            if (packet.Id == (int)PacketTypes.PlayerSlot)
            {
                var itemNetID = Unsafe.As<byte, short>(ref packet.Buffer.Data[3]); // https://unsafe.as/

                return itemNetID > TerrariaVersions.MaxItems[Crossplay.ClientVersions[playerId]];
            }
            return false;
        }
    }
}
