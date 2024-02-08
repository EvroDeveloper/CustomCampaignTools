using MelonLoader;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Labworks
{
    [RegisterTypeInIl2Cpp]
    public class SumOfBest : MonoBehaviour
    {
        public SumOfBest(IntPtr ptr) : base(ptr) { }

        private string levelBarcode;

        // Use this for initialization
        void Start()
        {
            levelBarcode = gameObject.name;

            transform.parent.GetComponent<TextMeshProUGUI>().text = AmmoFunctions.GetAmmoTotalByLevel(levelBarcode).ToString();
        }
    }
}