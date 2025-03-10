using System;

namespace Crossplay
{
    [Flags]
    enum DifficultyFlags : byte
    {
        Softcore = 0,
        Mediumcore = 1,
        Hardcore = 2,
        ExtraAccessory = 4,
        Creative = 8
    }
}
