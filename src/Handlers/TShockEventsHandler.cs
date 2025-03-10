using System;
using System.IO;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Crossplay.Configuration;
using Terraria;
using Terraria.Localization;

namespace Crossplay.Handlers
{
    static class TShockEventsHandler
    {
        public static void RegisterHandlers()
        {
            ServerApi.Hooks.GameInitialize.Register(Crossplay.Instance, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Register(Crossplay.Instance, OnPostInitialize);
            ServerApi.Hooks.NetGetData.Register(Crossplay.Instance, OnGetData, int.MaxValue);
            ServerApi.Hooks.ServerLeave.Register(Crossplay.Instance, OnLeave);
            GeneralHooks.ReloadEvent += OnReload;
        }

        public static void UnregisterHandlers()
        {
            ServerApi.Hooks.GameInitialize.Deregister(Crossplay.Instance, OnInitialize);
            ServerApi.Hooks.GamePostInitialize.Deregister(Crossplay.Instance, OnPostInitialize);
            ServerApi.Hooks.NetGetData.Deregister(Crossplay.Instance, OnGetData);
            ServerApi.Hooks.ServerLeave.Deregister(Crossplay.Instance, OnLeave);
            GeneralHooks.ReloadEvent -= OnReload;
        }

        private static void OnInitialize(EventArgs args) => Config.Reload();

        private static void OnReload(ReloadEventArgs args) => Config.Reload();

        private static void OnPostInitialize(EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------------------------");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Crossplay has been enabled & has whitelisted the following versions:");
            Console.WriteLine(string.Join(", ", TerrariaVersions.SupportedVersions.Values));
            Console.WriteLine("\nReport issues: https://github.com/TerraZ-Team/Crossplay");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("-------------------------------------");
            Console.ResetColor();
        }

        private static void HandleConnectRequest(GetDataEventArgs args)
        {
            using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
            {
                string clientVersion = reader.ReadString();
                if (clientVersion.Length != TerrariaVersions.VERSION_STRING_LEN)
                {
                    args.Handled = true;
                    return;
                }

                if (!int.TryParse(clientVersion.AsSpan(clientVersion.Length - 3), out int versionNumber))
                    return;

                if (!TerrariaVersions.SupportedVersions.ContainsKey(versionNumber))
                    return;

                Crossplay.ClientVersions[args.Msg.whoAmI] = versionNumber;

                if (versionNumber == Main.curRelease)
                    return;

                NetMessage.SendData((int)PacketTypes.Status, args.Msg.whoAmI, -1, NetworkText.FromLiteral("Fixing Version..."), 1);
                byte[] connectRequest = new PacketFactory()
                    .SetType((int)PacketTypes.ConnectRequest)
                    .PackString($"Terraria{TerrariaVersions.LAST_VERSION_NUM}")
                    .GetByteData();
                Logger.LogMessage($"Changing version of index {args.Msg.whoAmI} from {TerrariaVersions.SupportedVersions[versionNumber]} => {TerrariaVersions.LastVersion}", color: ConsoleColor.Green);

                Buffer.BlockCopy(connectRequest, 0, args.Msg.readBuffer, args.Index - 3, connectRequest.Length);
            }
        }

        private static void HandlePlayerInfo(GetDataEventArgs args)
        {
            if (!Config.Settings.SupportJourneyClients)
                return;

            ref byte difficulty = ref args.Msg.readBuffer[args.Length - 1];

            if (Main.GameModeInfo.IsJourneyMode)
            {
                if ((difficulty & (byte)DifficultyFlags.Creative) == 0)
                {
                    Logger.LogMessage($"Enabled journey mode for index {args.Msg.whoAmI}", color: ConsoleColor.Green);
                    difficulty |= (byte)DifficultyFlags.Creative;
                    if (Main.ServerSideCharacter)
                        NetMessage.SendData((int)PacketTypes.PlayerInfo, args.Msg.whoAmI, -1, null, args.Msg.whoAmI);
                }
                return;
            }
            if (TShock.Config.Settings.SoftcoreOnly &&
                (difficulty & (byte)(DifficultyFlags.Hardcore | DifficultyFlags.Mediumcore)) != 0)
                return;

            if ((difficulty & (byte)DifficultyFlags.Creative) != 0)
            {
                Logger.LogMessage($"Disabled journey mode for index {args.Msg.whoAmI}", color: ConsoleColor.Green);
                difficulty &= (byte)~DifficultyFlags.Creative;
            }
        }

        private static void OnGetData(GetDataEventArgs args)
        {
            switch (args.MsgID)
            {
                case PacketTypes.ConnectRequest:
                    HandleConnectRequest(args);
                    break;
                case PacketTypes.PlayerInfo when Crossplay.ClientVersions[args.Msg.whoAmI] != 0:
                    HandlePlayerInfo(args);
                    break;
            }
        }

        private static void OnLeave(LeaveEventArgs args)
        {
            Crossplay.ClientVersions[args.Who] = 0;
        }
    }
}
