using System;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Utilities;

namespace CustomCampaignTools.Data.SimpleSerializables;

public class MarrowEntityPoseSer
{
    public SimpleTransformSer[] bodyPoses;

    public MarrowEntityPoseSer() {}
    public MarrowEntityPoseSer(MarrowEntityPose pose)
    {
        bodyPoses = new SimpleTransformSer[pose.bodyPoses.Length];
        for (int i = 0; i < pose.bodyPoses.Count; i++)
        {
            bodyPoses[i] = new(pose.bodyPoses[i]);
        }
    }

    public MarrowEntityPose ToMarrowEntityPose()
    {
        MarrowEntityPose pose = new();
        SimpleTransform[] normalBodyPose = new SimpleTransform[bodyPoses.Length];

        for (int i = 0; i < pose.bodyPoses.Count; i++)
        {
            normalBodyPose[i] = bodyPoses[i].ToSimpleTransform();
        }

        pose.bodyPoses = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<SimpleTransform>(normalBodyPose);
        return pose;
    }
}
