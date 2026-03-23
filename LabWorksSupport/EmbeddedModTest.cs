#if FALSE
using System;
using BoneLib;
using MelonLoader;

namespace LabWorksSupport;

public class EmbeddedModTest : EmbeddedMod
{
    public override void OnAssemblyLoaded()
    {
        MelonLogger.Msg("Embedded Support was LOADED!!");
    }
}
#endif