# Job Cooldowns Mod for My Summer Car

A comprehensive mod that allows you to configure cooldown times for various jobs in My Summer Car, giving you full control over how often NPCs call you for work.

## Features

### 10 Configurable Jobs
- **Firewood Delivery (Livaloinen)** - Default: 500 minutes (8.3 hours)
- **Sewage Well 1-5** - Default: 50 minutes each
- **Mummola (Grandmother)** - Fish and grocery deliveries - Default: 150 minutes (2.5 hours)
- **Farm Jobs** - Haybale and combine work - Default: 100 minutes (1.7 hours)
- **Jokke Moving** - Moving job - Default: 167 minutes (2.8 hours)

### Real-Time Monitoring
- **Interactive GUI** - Press `Right Ctrl + J` to toggle the cooldown monitor
- **Color-coded timers**:
  - 🟢 Green: Job available
  - 🟡 Yellow: Less than 5 minutes remaining
  - 🟠 Orange: Less than 30 minutes remaining
  - 🔴 Red: More than 30 minutes remaining
- **Formatted display**: Shows time in hours, minutes, and seconds (e.g., "2h 15m", "45s")
- **Draggable window**: Position the GUI anywhere on screen

### Advanced Controls
- **Individual sliders**: Configure each job's cooldown from 1 to 480 minutes in MSCLoader Settings
- **Instant reset buttons**: Optional reset buttons for each job (toggle via checkbox in settings)
- **Persistent settings**: Your configurations are saved between game sessions

### Balanced Defaults
All default values are set to match or closely approximate the vanilla game's cooldown times, ensuring no gameplay imbalance unless you choose to modify them.

## Installation

### Requirements
- My Summer Car (latest version)
- MSCLoader (0.5 or newer)

### Steps
1. Download the latest release
2. Extract `JobCooldowns.dll` to your `Mods` folder:
   - Windows: `C:\Program Files (x86)\Steam\steamapps\common\My Summer Car\Mods\`
   - Linux (Proton/Wine): `~/.steam/steam/steamapps/common/My Summer Car/Mods/`
3. Launch the game
4. Open MSCLoader mod menu (`Ctrl + M`) and verify the mod is loaded

## Usage

### Configuring Cooldowns
1. Open MSCLoader Settings (`Ctrl + M` → `Settings`)
2. Find "Job Cooldowns" in the mod list
3. Adjust each job's cooldown using the sliders (1-480 minutes)
4. Enable "Show Reset Buttons" if you want instant reset functionality
5. Configure the GUI toggle keybind (default: `Right Ctrl + J`)

### Monitoring Jobs
1. Press `Right Ctrl + J` (or your configured keybind) to open the cooldown monitor
2. View real-time countdown for each job
3. Click "Reset" buttons (if enabled) to instantly make a job available
4. Drag the window by its title bar to reposition

### Understanding the Display
- **"Available!"** - Job is ready, NPC will call soon
- **Time remaining** - Countdown until the job becomes available
- **"Not in game"** - You need to load a save game first

## Technical Details

### How It Works
The mod interfaces with My Summer Car's PlayMaker FSM (Finite State Machine) system to modify job cooldown timers in real-time. It safely adjusts the `WaitTime` and `CoolDownTime` variables without breaking game mechanics.

### Compatibility
- ✅ Compatible with most other mods
- ✅ Works in both new and existing save games
- ✅ Safe to remove (cooldowns will revert to vanilla behavior)
- ✅ No game file modification required

### Performance
- Minimal performance impact
- Efficient FSM querying
- Optimized GUI rendering

## Building from Source

### Prerequisites
- .NET SDK (with .NET Framework 3.5 support)
- My Summer Car installation
- MSCLoader references

### Build Instructions
```bash
cd JobCooldowns
dotnet build
```

The compiled DLL will be output to your game's `Mods` folder.

## Troubleshooting

### Jobs not appearing in GUI
- Make sure you're in-game (load a save)
- Some jobs require game progression to unlock

### Reset button doesn't work immediately
- Phone takes 1-5 real-time minutes to ring after resetting
- This is normal game behavior, not a bug

### Settings not saving
- Ensure MSCLoader has write permissions to the config folder
- Check `Mods/Config/Mod Settings/JobCooldowns/` for config files

## Credits

**Author**: byomess  
**Version**: 1.0.0  
**Framework**: MSCLoader  

Special thanks to:
- MSCLoader developers
- My Summer Car modding community
- PlayMaker FSM documentation contributors

## License

This mod is provided as-is for personal use. Feel free to modify for personal use, but please credit the original author if sharing modifications.

## Changelog

### Version 1.0.0 (Initial Release)
- 10 configurable job cooldowns
- Real-time GUI monitoring with color-coded timers
- Instant reset functionality
- Balanced default values matching vanilla game
- Persistent settings
- Draggable GUI window
- Full MSCLoader integration

## Support

If you encounter issues or have suggestions:
- Report bugs with detailed descriptions
- Include your MSCLoader log file
- Specify your game version and other installed mods

---

**Enjoy faster (or slower) job cycles in My Summer Car!** 🚗🔧
