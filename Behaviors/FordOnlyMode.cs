using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using Labworks.Data;
using SLZ.Marrow.SceneStreaming;
using SLZ.Props;
using SLZ.VRMK;

namespace Labworks
{
    internal class FordOnlyMode
    {
        public static async Task InitializeLevel()
        {
            if (LabworksSaving.IsFordOnlyMode)
            {
                if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort")
                {
                    var pullCordDevice = Player.physicsRig.transform.GetComponentInChildren<PullCordDevice>(true);
                    while (pullCordDevice.isActiveAndEnabled)
                    {
                        pullCordDevice.gameObject.SetActive(false);
                        await Task.Delay(10);
                    }
                    Player.rigManager.SwapAvatarCrate("SLZ.BONELAB.Content.Avatar.FordBW");
                }
            }
        }
    }
}
