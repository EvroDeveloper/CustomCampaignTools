namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class BoneworksMenu : MonoBehaviour
    {
        public BoneworksMenu(IntPtr ptr) : base(ptr) {}

        public GameObject creditsPage;
        public GameObject menu;


        public void SetNPCUsage(int index, bool classic)
        {
            
        }

        public void SetFordOnly(bool b)
        {
            LabworksSaving.IsFordOnlyMode = b;
        }
    }
}