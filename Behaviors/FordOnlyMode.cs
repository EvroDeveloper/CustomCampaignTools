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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Labworks
{
    internal class FordOnlyMode
    {
        public static void InitializeLevel()
        {
            if (LabworksSaving.IsFordOnlyMode)
            {
                if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort")
                {
                    var camera = Player.rigManager.GetComponentInChildren<Camera>(true);
                    Player.rigManager.SwapAvatarCrate("SLZ.BONELAB.Content.Avatar.FordBW");
                }
            }
        }
    }
}
