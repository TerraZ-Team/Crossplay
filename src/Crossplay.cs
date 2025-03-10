using System;
using Terraria;
using TerrariaApi.Server;
using Crossplay.Handlers;

namespace Crossplay
{
    [ApiVersion(2, 1)]
    public class Crossplay : TerrariaPlugin
    {
        public override string Name => "Crossplay";
        public override string Author => "Moneylover3246";
        public override string Description => "Enables crossplay for terraria";
        public override Version Version => new("2.3");

        public static int[] ClientVersions { get; internal set; } = new int[Main.maxPlayers];

        internal static TerrariaPlugin Instance { get; private set; }

        public Crossplay(Main game) : base(game)
        {
            Instance = this;
            Order = -1;
        }

        public override void Initialize()
        {
            if (TerrariaVersions.LastVersion != Main.versionNumber)
                throw new NotSupportedException("The provided version of this plugin is outdated and will not function properly. Check for any updates here: https://github.com/Moneylover3246/Crossplay");

            TerrariaEventsHandler.RegisterHandlers();
            TShockEventsHandler.RegisterHandlers();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TerrariaEventsHandler.UnregisterHandlers();
                TShockEventsHandler.UnregisterHandlers();
            }
            base.Dispose(disposing);
        }
    }
}
