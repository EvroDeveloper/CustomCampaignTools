using System;
using FullSave.SDK.MonoBehaviours;
using FullSave.Utilities;

namespace FullSave.ComponentSavers.SDK;

[ComponentSaver(typeof(FloatSaver))]
public class FloatComponentSaver : ComponentSaver<FloatSaver>
{
    public override byte[] SaveComponentState(FloatSaver component)
    {
        ByteStreamer streamer = new();
        streamer.WriteFloat(component._float.Get());
        return streamer.ToByteArray();
    }

    public override void RestoreComponentState(FloatSaver component, byte[] stateData)
    {
        ByteStreamer reader = new(stateData);
        component.floatValue = reader.ReadFloat();
        component.onStateRestored.Invoke();
    }
}
