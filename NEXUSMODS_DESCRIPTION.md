# Job Cooldowns - Take Control of Job Timers

Tired of waiting 8+ hours for firewood deliveries? Want sewage jobs to come faster? **Job Cooldowns** gives you complete control over how often NPCs call you for work in My Summer Car.

## 🎮 What Does This Mod Do?

This mod allows you to **customize the cooldown times** for all major jobs in the game. Make jobs available instantly, keep vanilla timings, or even slow them down for a more relaxed playthrough.

### ✨ Key Features

**🔧 10 Configurable Jobs**
- Firewood Delivery (Livaloinen)
- 5 Sewage Wells (each configurable independently)
- Mummola Jobs (Grandmother's fish/grocery deliveries)
- Farm Jobs (Haybale and combine work)
- Jokke's Moving Job

**📊 Real-Time Monitoring**
- Press `Right Ctrl + J` to open the cooldown monitor
- See exactly how long until each job is available
- **View current job state** for Firewood job (Cooldown, Calling, etc.)
- **Developer Mode**: Enable to see FSM states for all jobs + verbose logging
- Color-coded timers (green when ready, red when waiting)
- **Phone bill warnings**: GUI alerts you if your phone bill is unpaid
- Clean, draggable GUI that doesn't obstruct gameplay

**⚡ Instant Reset**
- Optional reset buttons for each job
- No waiting - make jobs available immediately
- Toggle button visibility in settings

**🎚️ Individual Control**
- Each job has its own slider (1-480 minutes)
- Configure in MSCLoader Settings menu
- Settings persist between game sessions

## 📸 Screenshots

[The cooldown monitor shows all jobs with their remaining time in an easy-to-read format]

[Configure each job's cooldown time individually in the MSCLoader Settings menu]

[Color-coded display: Green = Available, Yellow = Soon, Orange = Minutes away, Red = Long wait]

## 🎯 Why Use This Mod?

**For Speedrunners**: Reduce cooldowns to complete jobs faster and maximize income

**For Casual Players**: Increase cooldowns to reduce interruptions and play at your own pace

**For Testing**: Instantly reset cooldowns to test job mechanics or showcase builds

**For Balance**: Fine-tune job frequency to match your preferred playstyle

## 🔍 How It Works

The mod safely interfaces with the game's internal job system (PlayMaker FSM) to modify cooldown timers. It doesn't modify any game files and can be safely removed at any time.

**Default values match the vanilla game**, so there's no balance change unless you adjust the settings yourself.

## 📋 Installation

**Requirements**:
- My Summer Car (latest version recommended)
- MSCLoader 0.5 or newer

**Steps**:
1. Download and install MSCLoader if you haven't already
2. Download this mod
3. Extract `JobCooldowns.dll` to your `Mods` folder
4. Launch the game and verify the mod loads (check MSCLoader menu with `Ctrl + M`)

**Mod Folder Location**:
- Windows: `C:\Program Files (x86)\Steam\steamapps\common\My Summer Car\Mods\`
- Linux: `~/.steam/steam/steamapps/common/My Summer Car/Mods/`

## ⚙️ Configuration

1. Open MSCLoader Settings (`Ctrl + M` → Settings)
2. Find "Job Cooldowns" in the mod list
3. Adjust sliders for each job (1 to 480 minutes)
4. Enable/disable reset buttons via checkbox
5. **Toggle Developer Mode** for FSM state inspection and verbose logging
6. Customize the GUI toggle keybind

## 🎮 Usage

**Open Monitor**: `Right Ctrl + J` (configurable)

**Reading the Display**:
- "Available!" = Job is ready
- Time shown = Countdown until available (e.g., "2h 15m", "45s")
- **State indicators** = Current job status (e.g., "[Cooldown]", "[Calling...]", "[In Progress]")
- "Not in game" = Load a save first

**Reset a Job**: Click the "Reset" button next to any job (if enabled in settings)

## 🔧 Job Default Cooldowns

All defaults match or approximate vanilla timings:

- **Firewood**: 500 minutes (~8.3 hours)
- **Sewage Wells**: 50 minutes each
- **Mummola**: 150 minutes (~2.5 hours)
- **Farm**: 100 minutes (~1.7 hours)
- **Jokke Moving**: 167 minutes (~2.8 hours)

## 🛡️ Compatibility

- ✅ Works with existing save games
- ✅ Compatible with most other mods
- ✅ Safe to install/uninstall anytime
- ✅ No game file modifications
- ✅ Minimal performance impact

## ⚠️ Known Issues

- After resetting a cooldown, the phone may take 1-2 in-game minutes to ring (this is normal game behavior)
- Some jobs require game progression before they appear

## 🐛 Troubleshooting

**Jobs not showing in GUI?**
→ Make sure you've loaded a save game

**Reset button not working?**
→ Enable "Show Reset Buttons" in mod settings

**Settings not saving?**
→ Check that MSCLoader has write permissions to the Mods folder

## 📝 Changelog

**Version 1.1.0** (Latest)
- **NEW**: Real-time job state monitoring
  - Shows current FSM state for Firewood job (more jobs in future updates)
  - User-friendly state names ("Cooldown", "Will call", "In Progress")
  - Color-coded display (blue in normal mode, orange in dev mode)
- **NEW**: Developer Mode
  - Toggle verbose logging for debugging
  - View raw FSM state names for all jobs
  - Dev log file output (`JobCooldowns_DevLog.txt`)
- **NEW**: Phone bill warning system
  - GUI shows warning when phone bill is unpaid
  - Helpful notice about job phone call requirements
- **IMPROVED**: Enhanced GUI footer with usage instructions
  - ESC key instruction for cursor interaction
  - Keybind reminder for toggling window
- **FIXED**: JobId comparison bug affecting state display
- **REMOVED**: Obsolete debug files

**Version 1.0.0** (Initial Release)
- 10 configurable job cooldowns
- Real-time monitoring GUI with color-coded timers
- Optional instant reset buttons
- Balanced defaults matching vanilla game
- Persistent settings across sessions
- Draggable GUI window
- Full MSCLoader integration

## 🙏 Credits

**Created by**: byomess  
**Built with**: MSCLoader, .NET Framework 3.5  
**Special Thanks**: MSC modding community, PlayMaker FSM documentation contributors

## 📜 Permissions

- ✅ Modify for personal use
- ✅ Include in modpacks (with credit)
- ❌ Re-upload without permission
- ✅ Share configuration presets

## 💬 Support

Found a bug? Have a suggestion? Post in the comments or bugs section!

**Please include**:
- Your game version
- MSCLoader version
- Other installed mods
- MSCLoader log file (if experiencing crashes)

---

**Take control of your job schedule and play My Summer Car your way!** 🚗🔧

## Tags

gameplay, jobs, cooldown, timer, utility, quality-of-life, configurable, MSCLoader
