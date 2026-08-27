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

echo "Build complete."
