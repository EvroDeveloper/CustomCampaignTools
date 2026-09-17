#if MELONLOADER
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using UnityEngine;

namespace CustomCampaignTools
{
    public class AvatarStatRanges
    {
        public float heightRangeLow;
        public float heightRangeHigh;

        public float massRangeLow;
        public float massRangeHigh;
        
        public float armRangeLow;
        public float armRangeHigh;

        public AvatarStatRanges() { }

        public AvatarStatRanges(Vector2 heightRange, Vector2 massRange, Vector2 armRange)
        {
            heightRangeLow = heightRange.x;
            heightRangeHigh = heightRange.y;

            massRangeLow = massRange.x;
            massRangeHigh = massRange.y;

            armRangeLow = armRange.x;
            armRangeHigh = armRange.y;
        }
    }
}
