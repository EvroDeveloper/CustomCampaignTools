using System;
using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;

namespace FullSave.MonoBehaviours;

[RegisterTypeInIl2Cpp]
public class EntitySpawnerLink : MonoBehaviour
{
    public EntitySpawnerLink(IntPtr ptr) : base(ptr) {}

    public CrateSpawner birthgiver;
}
