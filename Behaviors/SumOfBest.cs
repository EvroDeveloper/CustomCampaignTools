using MelonLoader;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Labworks.Behaviors
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class SumOfBest : MonoBehaviour
    {
#if MELONLOADER
        public SumOfBest(IntPtr ptr) : base(ptr) { }
#endif

        private string levelBarcode;

        // Use this for initialization
        void Start()
        {
#if MELONLOADER
            levelBarcode = gameObject.name;

            transform.parent.GetComponent<TextMeshProUGUI>().text = AmmoFunctions.GetAmmoTotalByLevel(levelBarcode).ToString();
#endif
        }
    }
}