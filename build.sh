#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
configuration="${1:-Debug}"

support_project="$script_dir/CustomCampaignTools.BonelabSupport/CustomCampaignTools.BonelabSupport.csproj"
main_project="$script_dir/CustomCampaignTools/CustomCampaignTools.csproj"

build_arguments=(--configuration "$configuration")
if [[ -n "${BONELAB_DIR:-}" ]]; then
    build_arguments+=("/p:BONELAB_DIR=$BONELAB_DIR")
fi

final_build_arguments=("${build_arguments[@]}")
if [[ "${SKIP_MOD_INSTALL:-false}" == "true" ]]; then
    final_build_arguments+=("/p:SkipModInstall=true")
fi

echo "Building BonelabSupport ($configuration)..."
dotnet build "$support_project" "${build_arguments[@]}" /p:SkipModInstall=true

echo "Building CustomCampaignTools with the updated embedded BonelabSupport DLL ($configuration)..."
dotnet build "$main_project" --no-restore "${final_build_arguments[@]}"

mod_dll="$script_dir/CustomCampaignTools/bin/$configuration/net6.0/CustomCampaignTools.dll"
quest_mods_dir="/sdcard/MelonLoader/com.StressLevelZero.BONELAB/Mods"

if [[ "${SKIP_QUEST_INSTALL:-false}" != "true" ]]; then
    if [[ ! -f "$mod_dll" ]]; then
        echo "Build succeeded, but the mod DLL was not found at $mod_dll" >&2
        exit 1
    fi
    if ! command -v adb >/dev/null 2>&1; then
        echo "Build succeeded, but adb is not installed or is not on PATH. Set SKIP_QUEST_INSTALL=true to build without deploying." >&2
        exit 1
    fi
    if ! adb get-state >/dev/null 2>&1; then
        echo "Build succeeded, but adb could not find one ready device. Connect and authorize your Quest 3 (or set ANDROID_SERIAL), then retry." >&2
        exit 1
    fi

    echo "Installing CustomCampaignTools.dll to Quest 3 at $quest_mods_dir..."
    adb shell mkdir -p "$quest_mods_dir"
    adb push "$mod_dll" "$quest_mods_dir/CustomCampaignTools.dll"
fi

echo "Build complete."
