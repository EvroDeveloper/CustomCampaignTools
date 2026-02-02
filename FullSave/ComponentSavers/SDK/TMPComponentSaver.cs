using System;
using FullSave.SDK.MonoBehaviours;
using FullSave.Utilities;

namespace FullSave.ComponentSavers.SDK;

[ComponentSaver(typeof(TMP_TextSaver))]
public class TMPStateSaver : ComponentSaver<TMP_TextSaver>
{
    public override byte[] SaveComponentState(TMP_TextSaver component)
    {
        ByteStreamer writer = new();
        writer.WriteString(component.textMeshPro.text);
        return writer.ToByteArray();
    }

    public override void RestoreComponentState(TMP_TextSaver component, byte[] stateData)
    {
        ByteStreamer reader = new(stateData);
        component.textMeshPro.text = reader.ReadString();
        component.onStateRestored.Invoke();
    }
}
