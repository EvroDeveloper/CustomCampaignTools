#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
source_dir="$script_dir/CustomCampaignTools/CustomCampaignTools.SDK"

usage() {
    cat <<EOF
Usage: $(basename "$0") <destination>

Copies the contents of CustomCampaignTools/CustomCampaignTools.SDK to the
destination directory. Existing files with the same names are overwritten.

Example:
  $(basename "$0") /path/to/UnityProject/Assets/CustomCampaignTools.SDK
EOF
}

if [[ $# -ne 1 || "$1" == "-h" || "$1" == "--help" ]]; then
    usage
    [[ $# -eq 1 ]] && exit 0
    exit 2
fi

if [[ ! -d "$source_dir" ]]; then
    echo "SDK source directory was not found: $source_dir" >&2
    exit 1
fi

destination_dir="$1"
mkdir -p -- "$destination_dir"

source_dir="$(cd -- "$source_dir" && pwd -P)"
destination_dir="$(cd -- "$destination_dir" && pwd -P)"

case "$destination_dir/" in
    "$source_dir/"*)
        echo "Destination cannot be the SDK source directory or a directory inside it." >&2
        exit 1
        ;;
esac

cp -Rf -- "$source_dir/." "$destination_dir/"

echo "Copied SDK to: $destination_dir"
