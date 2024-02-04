using BoneLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks_Ammo_Saver
{
    public static class ContentLoader
    {
        public static AssetBundle ContentBundle { get; private set; }

        public static GameObject ElevatorPrefab { get; private set; }

        public static void OnBundleLoad()
        {
            // Kitchen Boy Help Plz i want to load an embedded asset bundle but idk how to and fusions code for this is so sprawled out womp womp.
        }
    }
}
