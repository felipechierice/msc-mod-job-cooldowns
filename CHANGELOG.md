# Changelog

All notable changes to the Job Cooldowns mod will be documented in this file.

## [1.1.0] - 2026-01-16

### Added
- Real-time job state monitoring in GUI (Firewood job only in normal mode)
- Developer Mode with verbose logging and FSM state inspection
- Dev log file output (`JobCooldowns_DevLog.txt` in game root directory)
- Phone bill warning in GUI footer when bill is unpaid
- Information notices about phone calls requirement for job activation
- ESC key instruction for GUI interaction in footer
- Color-coded state display (orange for dev mode, blue for normal mode)

### Changed
- GUI now only shows states for Firewood job when Dev Mode is off (other jobs in future updates)
- State names are now user-friendly ("Cooldown" instead of "Random wait", "Will call" instead of "Call")
- Improved GUI footer with multiple helpful messages
- Enhanced DevLogger with file output and structured logging

### Fixed
- Corrected JobId comparison bug (`firewood_delivery` vs `Firewood`)
- Improved keybind debouncing for GUI toggle

### Removed
- Obsolete debug files not related to Dev Mode feature

## [1.0.0] - 2026-01-15

### Added
- Initial release
- 10 configurable job cooldowns
- Real-time monitoring GUI with color-coded timers
- Individual sliders for each job (1-480 minutes range)
- Optional instant reset buttons
- Persistent settings across game sessions
- Keybind to toggle GUI (Right Ctrl + J)
- Balanced default values matching vanilla game times

### Jobs Supported
- Firewood Delivery (Livaloinen)
- Sewage Wells 1-5
- Mummola (Grandmother fish/grocery deliveries)
- Farm Jobs (Haybale and Combine)
- Jokke Moving Job
