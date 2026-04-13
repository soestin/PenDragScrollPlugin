#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
DOTNET="${DOTNET:-/tmp/dotnet/sdk/dotnet}"
PLUGIN_DIR="${HOME}/.config/OpenTabletDriver/Plugins/Pen Drag Scroll"

"$DOTNET" build "$ROOT/PenDragScroll.csproj" -c Release
mkdir -p "$PLUGIN_DIR"
cp "$ROOT/bin/Release/net8.0/PenDragScroll.dll" "$PLUGIN_DIR/"

echo "Installed to: $PLUGIN_DIR"
