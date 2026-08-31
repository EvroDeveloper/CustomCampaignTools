using System;

namespace CustomCampaignTools
{
    public struct CampaignVersion
    {
        public uint VersionMajor { get; private set; }
        public uint VersionMinor { get; private set; }
        public uint VersionPatch { get; private set; }

        public CampaignVersion(uint Major, uint Minor, uint Patch)
        {
            this.VersionMajor = Major;
            this.VersionMinor = Minor;
            this.VersionPatch = Patch;
        }

        public CampaignVersion(string versionString)
        {
            string[] dotSplit = versionString.Split('.');
            if(dotSplit.Length != 3) throw new ArgumentException("Input is not a valid Campaign Version");

            VersionMajor = uint.Parse(dotSplit[0]);
            VersionMinor = uint.Parse(dotSplit[1]);
            VersionPatch = uint.Parse(dotSplit[2]);
        }

        public static bool operator <(CampaignVersion lhs, CampaignVersion rhs)
        {
            if(lhs.VersionMajor < rhs.VersionMajor) return true;
            else if(lhs.VersionMajor > rhs.VersionMajor) return false;

            // VersionMajor is Equal
            if(lhs.VersionMinor < rhs.VersionMinor) return true;
            else if(lhs.VersionMinor > rhs.VersionMinor) return false;

            // VersionMinor is Equal
            if(lhs.VersionPatch < rhs.VersionPatch) return true;
            else if(lhs.VersionPatch > rhs.VersionPatch) return false;

            return false;
        }

        public static bool operator >(CampaignVersion lhs, CampaignVersion rhs)
        {
            if(lhs.VersionMajor > rhs.VersionMajor) return true;
            else if(lhs.VersionMajor < rhs.VersionMajor) return false;

            // VersionMajor is Equal
            if(lhs.VersionMinor > rhs.VersionMinor) return true;
            else if(lhs.VersionMinor < rhs.VersionMinor) return false;

            // VersionMinor is Equal
            if(lhs.VersionPatch > rhs.VersionPatch) return true;
            else if(lhs.VersionPatch < rhs.VersionPatch) return false;

            return false;
        }

        public override string ToString()
        {
            return $"{VersionMajor}.{VersionMinor}.{VersionPatch}";
        }
    }
}