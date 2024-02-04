using MelonLoader;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Labworks_Ammo_Saver
{
    [RegisterTypeInIl2Cpp]
    public class SumOfBest : MonoBehaviour
    {
        public SumOfBest(IntPtr ptr) : base(ptr) { }

        private int levelIndex;

        // Use this for initialization
        void Start()
        {
            levelIndex = int.Parse(gameObject.name);

            transform.parent.GetComponent<TextMeshProUGUI>().text = AmmoFunctions.GetAmmoTotalByLevel(levelIndex).ToString();
        }
    }
}