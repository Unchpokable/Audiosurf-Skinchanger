﻿namespace Audiosurf_SkinChanger
{
    using System.Collections.Generic;
    using Audiosurf_SkinChanger.Utilities;

    public static class EnvironmentalVeriables
    {
        internal static readonly IList<SkinLink> Skins = new List<SkinLink>();
        internal static string gamePath = "";
        internal static string skinsFolderPath = "None";
        internal static readonly string CliffImagesMask = "cliff";
        internal static readonly string HitImageMask = "hit";
        internal static readonly string ParticlesImageMask = "particles";
        internal static readonly string RingsImageMask = "ring";
        internal static readonly string SkysphereImagesMask = "skysphere";
        internal static readonly string TileFlyupImageMask = "tileflyup.png";
        internal static readonly string TilesImageName = "tiles.png";
        internal static bool DCSWarningsAllowed = true;
        internal static DCSBehaviour ControlSystemBehaviour = DCSBehaviour.OnBoot;
    }
}
