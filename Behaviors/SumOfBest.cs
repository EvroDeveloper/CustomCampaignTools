using MelonLoader;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class SumOfBest : MonoBehaviour
    {
        public SumOfBest(IntPtr ptr) : base(ptr) { }

        private string levelBarcode;
        private int levelIndex;

        // Use this for initialization
        void Start()
        {
            levelBarcode = gameObject.name;

            transform.parent.GetComponent<TextMeshProUGUI>().text = AmmoFunctions.GetAmmoTotalByLevel(levelBarcode).ToString();
        }
    }
}