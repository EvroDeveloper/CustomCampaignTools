#if MELONLOADER
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Utilities;
#else
using SLZ.Marrow.Interaction;
using SLZ.Marrow.Utilities;
#endif

namespace SimpleSerializables.Types
{
    public class MarrowEntityPoseSer
    {
        public SimpleTransformSer[] bodyPoses;

        public MarrowEntityPoseSer() {}
        public MarrowEntityPoseSer(MarrowEntityPose pose)
        {
            bodyPoses = new SimpleTransformSer[pose.bodyPoses.Length];
            for (int i = 0; i < pose.bodyPoses.Length; i++)
            {
                bodyPoses[i] = new(pose.bodyPoses[i]);
            }
        }

        public MarrowEntityPose ToMarrowEntityPose()
        {
            MarrowEntityPose pose = new();
            SimpleTransform[] normalBodyPose = new SimpleTransform[bodyPoses.Length];

            for (int i = 0; i < bodyPoses.Length; i++)
            {
                if(bodyPoses[i] == null)
                {
                    normalBodyPose[i] = new SimpleTransform();
                }
                normalBodyPose[i] = bodyPoses[i].ToSimpleTransform();
            }

#if MELONLOADER
            pose.bodyPoses = new(normalBodyPose);
#else
            pose.bodyPoses = normalBodyPose;
#endif
            return pose;
        }
    }
}
