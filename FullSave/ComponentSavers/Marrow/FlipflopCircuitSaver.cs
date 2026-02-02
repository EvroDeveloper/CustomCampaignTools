using System;
using FullSave.Utilities;
using Il2CppSLZ.Marrow.Circuits;

namespace FullSave.ComponentSavers.Marrow;

[ComponentSaver(typeof(FlipflopCircuit))]
public class FlipflopCircuitSaver : ComponentSaver<FlipflopCircuit>
{
    public override byte[] SaveComponentState(FlipflopCircuit component)
    {
        ByteStreamer writer = new();
        writer.WriteFloat(component.sensorValue); // guh? i think this is how it saves the flipped flop
        return writer.ToByteArray();
    }

    public override void RestoreComponentState(FlipflopCircuit component, byte[] stateData)
    {
        ByteStreamer reader = new(stateData);
        component.sensorValue = reader.ReadFloat(); // urgh
    }
}
