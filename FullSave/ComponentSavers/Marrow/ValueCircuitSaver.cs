using System;
using FullSave.Utilities;
using Il2CppSLZ.Marrow.Circuits;

namespace FullSave.ComponentSavers.Marrow;

[ComponentSaver(typeof(ValueCircuit))]
public class ValueCircuitSaver : ComponentSaver<ValueCircuit>
{
    public override void RestoreComponentState(ValueCircuit component, byte[] stateData)
    {
        ByteStreamer reader = new(stateData);
        component.Value = reader.ReadFloat();
    }

    public override byte[] SaveComponentState(ValueCircuit component)
    {
        ByteStreamer reader = new();
        reader.WriteFloat(component.Value);
        return reader.ToByteArray();
    }
}
