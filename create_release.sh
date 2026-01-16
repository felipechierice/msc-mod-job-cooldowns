#!/bin/bash

# Job Cooldowns - Release Package Creator
# Creates a .zip file ready for Nexusmods upload

VERSION="1.1.0"
MOD_NAME="JobCooldowns"
RELEASE_DIR="release_package"
ZIP_NAME="${MOD_NAME}_v${VERSION}.zip"

echo "Creating release package for ${MOD_NAME} v${VERSION}..."

# Clean previous release folder
rm -rf "$RELEASE_DIR"
mkdir -p "$RELEASE_DIR"

# Check if DLL exists
DLL_PATH="../../Mods/${MOD_NAME}.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "ERROR: ${MOD_NAME}.dll not found at $DLL_PATH"
    echo "Please compile the mod first: dotnet build JobCooldowns.csproj -c Release"
    exit 1
fi

# Copy DLL
echo "Copying ${MOD_NAME}.dll..."
cp "$DLL_PATH" "$RELEASE_DIR/"

# Create instructions.txt
echo "Creating instructions.txt..."
cat > "$RELEASE_DIR/instructions.txt" << 'EOF'
================================================================================
                        JOB COOLDOWNS v1.1.0
                        Installation Instructions
================================================================================

REQUIREMENTS:
- My Summer Car (latest version recommended)
- MSCLoader 0.5 or newer
  Download: https://www.nexusmods.com/mysummercar/mods/147

INSTALLATION:
1. Make sure MSCLoader is installed and working
2. Copy JobCooldowns.dll to your Mods folder:
   - Windows: C:\Program Files (x86)\Steam\steamapps\common\My Summer Car\Mods\
   - Linux:   ~/.steam/steam/steamapps/common/My Summer Car/Mods/
3. Launch My Summer Car
4. The mod will load automatically

CONFIGURATION:
- Open MSCLoader Settings (Ctrl + M in main menu → Settings)
- Find "Job Cooldowns" in the mod list
- Configure each job's cooldown time (1-480 minutes)
- Enable/disable Reset buttons and Developer Mode
- Customize the toggle keybind (default: Right Ctrl + J)

USAGE:
- Press Right Ctrl + J (or your custom keybind) to open the monitor
- Press ESC to show cursor and interact with buttons
- View real-time cooldowns and job states
- Reset jobs instantly with Reset buttons (if enabled)

SUPPORT:
- Mod Page: https://www.nexusmods.com/mysummercar/mods/10417
- Report bugs in the mod page comments or bugs section
- Include your game version, MSCLoader version, and log files

FEATURES:
✓ 10 configurable job cooldowns
✓ Real-time monitoring GUI with color-coded timers
✓ Job state indicators (Firewood job)
✓ Developer Mode with verbose logging
✓ Phone bill warnings
✓ Optional instant reset buttons
✓ Persistent settings

================================================================================
                    Enjoy faster job cycles! 🚗🔧
================================================================================
EOF

# Create the zip file
echo "Creating ${ZIP_NAME}..."
cd "$RELEASE_DIR"
zip -q "../${ZIP_NAME}" *
cd ..

# Verify zip was created
if [ -f "$ZIP_NAME" ]; then
    echo ""
    echo "✅ SUCCESS! Release package created:"
    echo "   📦 $ZIP_NAME"
    echo "   📄 Size: $(du -h "$ZIP_NAME" | cut -f1)"
    echo ""
    echo "Package contents:"
    unzip -l "$ZIP_NAME"
    echo ""
    echo "Ready to upload to Nexusmods!"
    echo "Mod page: https://www.nexusmods.com/mysummercar/mods/10417"
else
    echo "❌ ERROR: Failed to create zip file"
    exit 1
fi

# Clean up
echo ""
read -p "Delete temporary folder? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    rm -rf "$RELEASE_DIR"
    echo "Cleaned up temporary files"
fi

echo "Done!"
