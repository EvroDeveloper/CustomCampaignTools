using System;
using FullSave.Utilities;
using Il2CppSLZ.Marrow.AI;

namespace FullSave.ComponentSavers.Marrow;

public class EncounterSaver : ComponentSaver<Encounter>
{
    public override void RestoreComponentState(Encounter component, byte[] stateData)
    {
        ByteStreamer reader = new(stateData);
        if(reader.ReadBool()) // isEncounterActive
            component.StartEncounter();
    }

    public override byte[] SaveComponentState(Encounter component)
    {
        ByteStreamer writer = new();
        writer.WriteBool(component._isEncounterActive);
        
        return writer.ToByteArray();
    }
}
