using BoneLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks
{
    public static class ContentLoader
    {
        public static GameObject ElevatorPrefab { get; private set; }

        private const string ElevatorPrefabPath = "Assets/elevator/elevator_gauntlet.prefab";

        public static void OnBundleLoad()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (HelperMethods.IsAndroid())
            {
                var bundle = HelperMethods.LoadEmbeddedAssetBundle(assembly, "Labworks_Addon.Resources.Android.elevator.labworks");
                ElevatorPrefab = bundle.LoadPersistentAsset<GameObject>(ElevatorPrefabPath);
            }
            else
            {
                var bundle = HelperMethods.LoadEmbeddedAssetBundle(assembly, "Labworks_Addon.Resources.Windows.elevator.labworks");
                ElevatorPrefab = bundle.LoadPersistentAsset<GameObject>(ElevatorPrefabPath);
            }
        }
    }
}
