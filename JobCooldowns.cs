using System.Collections.Generic;
using System.Reflection;
using MSCLoader;
using JobCooldowns.Handlers;
using UnityEngine;

namespace JobCooldowns
{
    /// <summary>
    /// Job Cooldowns Mod - Configure cooldowns for various jobs in My Summer Car.
    /// </summary>
    public class JobCooldowns : Mod
    {
        public override string ID => "JobCooldowns";
        public override string Name => "Job Cooldowns";
        public override string Author => "byomess";
        public override string Version => "1.1.0";
        public override string Description => "Configure cooldown times for various jobs in the game.";
        
        // Mod icon
        public override byte[] Icon => LoadIcon();

        // List of all job cooldown handlers (scalable for future additions)
        private readonly List<IJobCooldownHandler> _jobHandlers = new List<IJobCooldownHandler>();

        // Settings sliders for each job
        private readonly Dictionary<string, SettingsSliderInt> _settingsSliders = new Dictionary<string, SettingsSliderInt>();

        // GUI for monitoring cooldowns
        private CooldownMonitorGUI _monitorGUI;
        private SettingsKeybind _toggleGUIKeybind;
        private SettingsCheckBox _showResetButtonsCheckbox;
        private SettingsCheckBox _devModeCheckbox;
        
        // Keybind debounce tracking
        private float _lastToggleTime;
        private const float TOGGLE_COOLDOWN = 0.3f; // 300ms cooldown between toggles

        public override void ModSetup()
        {
            // Register mod functions
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.OnGUI, Mod_OnGUI);
            SetupFunction(Setup.OnSave, Mod_OnSave);
        }
        
        /// <summary>
        /// Loads the mod icon from embedded resources.
        /// </summary>
        private byte[] LoadIcon()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "JobCooldowns.icon.png";
                
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] iconBytes = new byte[stream.Length];
                        stream.Read(iconBytes, 0, iconBytes.Length);
                        return iconBytes;
                    }
                }
            }
            catch (System.Exception e)
            {
                ModConsole.Error($"[JobCooldowns] Failed to load icon: {e.Message}");
            }
            
            return null;
        }

        /// <summary>
        /// Initialize job handlers. Add new handlers here for scalability.
        /// </summary>
        private void InitializeJobHandlers()
        {
            _jobHandlers.Clear();

            // Add all job handlers here
            // To add a new job cooldown, simply create a new handler class
            // implementing IJobCooldownHandler and add it here.
            
            // Firewood delivery (Livaloinen)
            _jobHandlers.Add(new FirewoodJobHandler());
            
            // Sewage wells (5 different locations)
            _jobHandlers.Add(new SewageJobHandler(1));
            _jobHandlers.Add(new SewageJobHandler(2));
            _jobHandlers.Add(new SewageJobHandler(3));
            _jobHandlers.Add(new SewageJobHandler(4));
            _jobHandlers.Add(new SewageJobHandler(5));
            
            // Grandmother deliveries (fish and groceries)
            _jobHandlers.Add(new MummolaJobHandler());
            
            // Farm jobs (haybale and combine)
            _jobHandlers.Add(new FarmJobHandler());
            
            // Jokke moving job
            _jobHandlers.Add(new JokkeMovingJobHandler());
        }

        /// <summary>
        /// Called when mod is loaded (in game).
        /// </summary>
        private void Mod_OnLoad()
        {
            // Initialize dev logger
            bool devMode = _devModeCheckbox?.GetValue() ?? false;
            DevLogger.Initialize(devMode);
            DevLogger.Log("Mod loaded - initializing...");
            
            // Initialize GUI
            _monitorGUI = new CooldownMonitorGUI(_jobHandlers, devMode);
            UpdateGUISettings();
            
            DevLogger.Log("Applying all cooldowns...");
            ApplyAllCooldowns();
            DevLogger.Log("Mod initialization complete");
        }

        /// <summary>
        /// Update loop for checking keybinds.
        /// </summary>
        private void Mod_Update()
        {
            // Toggle GUI visibility with keybind (with debounce to prevent multiple toggles)
            if (_toggleGUIKeybind != null && _toggleGUIKeybind.GetKeybind())
            {
                float currentTime = Time.time;
                if (currentTime - _lastToggleTime >= TOGGLE_COOLDOWN)
                {
                    _monitorGUI?.ToggleVisibility();
                    _lastToggleTime = currentTime;
                }
            }
        }

        /// <summary>
        /// Draw the GUI.
        /// </summary>
        private void Mod_OnGUI()
        {
            _monitorGUI?.DrawGUI();
        }

        /// <summary>
        /// Called when settings are loaded from file.
        /// </summary>
        private void Mod_SettingsLoaded()
        {
            // Update handler values from saved settings
            foreach (var handler in _jobHandlers)
            {
                if (_settingsSliders.TryGetValue(handler.JobId, out SettingsSliderInt slider))
                {
                    handler.CurrentCooldownMinutes = slider.GetValue();
                }
            }

            // Apply cooldowns if we're in game
            ApplyAllCooldowns();
        }

        /// <summary>
        /// Creates the settings UI.
        /// </summary>
        private void Mod_Settings()
        {
            // Initialize handlers first
            InitializeJobHandlers();

            // Add keybind for toggling GUI
            _toggleGUIKeybind = Keybind.Add(
                id: "toggle_monitor_gui",
                name: "Toggle Cooldown Monitor",
                key: KeyCode.J,
                modifier: KeyCode.RightControl
            );

            Settings.AddHeader("Job Cooldown Settings");
            Settings.AddText("Configure cooldown times for various jobs (in minutes).");
            Settings.AddText("Default game cooldowns are usually 8 real-time hours (480 minutes).");
            Settings.AddText("<color=cyan>Press <b>Right Ctrl + J</b> to toggle real-time cooldown monitor</color>");

            Settings.AddHeader("GUI Options");
            _showResetButtonsCheckbox = Settings.AddCheckBox(
                settingID: "show_reset_buttons",
                name: "Show Reset Buttons in Monitor",
                value: false,
                onValueChanged: () => UpdateGUISettings()
            );
            Settings.AddText("Enable to show buttons that instantly reset cooldowns to zero.");
            
            _devModeCheckbox = Settings.AddCheckBox(
                settingID: "dev_mode",
                name: "Developer Mode",
                value: false,
                onValueChanged: () => OnDevModeChanged()
            );
            Settings.AddText("<color=orange>Shows raw FSM state names, verbose console logs, and saves logs to file. Leave it off as it may impact performance and clutter logs, unless you are debugging or experiencing issues suspecting this mod is involved, so you can provide the logs file to the developer in your bug report on the Nexusmods page.</color>");

            foreach (var handler in _jobHandlers)
            {
                Settings.AddHeader(handler.DisplayName, UnityEngine.Color.cyan);

                var slider = Settings.AddSlider(
                    settingID: $"cooldown_{handler.JobId}",
                    name: $"Cooldown (minutes)",
                    minValue: handler.MinCooldownMinutes,
                    maxValue: handler.MaxCooldownMinutes,
                    value: handler.DefaultCooldownMinutes,
                    onValueChanged: () => OnCooldownChanged(handler)
                );

                _settingsSliders[handler.JobId] = slider;

                // Add helper text showing the time in hours
                Settings.AddText($"Range: {handler.MinCooldownMinutes} min to {handler.MaxCooldownMinutes} min ({handler.MaxCooldownMinutes / 60}h)");
            }
        }

        /// <summary>
        /// Called when a cooldown slider value changes.
        /// Only updates the handler value - actual application happens on game load.
        /// </summary>
        private void OnCooldownChanged(IJobCooldownHandler handler)
        {
            if (_settingsSliders.TryGetValue(handler.JobId, out SettingsSliderInt slider))
            {
                handler.CurrentCooldownMinutes = slider.GetValue();
                
                // Only apply if we're in game (the job GameObject exists)
                if (handler.IsAvailable())
                {
                    handler.ApplyCooldown();
                }
                else
                {
                    ModConsole.Print($"[JobCooldowns] {handler.DisplayName} will be set to {handler.CurrentCooldownMinutes} minutes when you start/load the game.");
                }
            }
        }

        /// <summary>
        /// Applies all configured cooldowns to the game.
        /// </summary>
        private void ApplyAllCooldowns()
        {
            DevLogger.Log("Applying cooldowns to all available jobs...");
            
            foreach (var handler in _jobHandlers)
            {
                if (handler.IsAvailable())
                {
                    DevLogger.Log($"Applying cooldown for {handler.DisplayName}");
                    handler.ApplyCooldown();
                    DevLogger.LogJobInfo(handler);
                }
                else
                {
                    DevLogger.Log($"{handler.DisplayName} not available (not in game)");
                }
            }
        }

        /// <summary>
        /// Updates GUI settings based on checkbox values.
        /// </summary>
        private void UpdateGUISettings()
        {
            if (_monitorGUI != null && _showResetButtonsCheckbox != null)
            {
                _monitorGUI.ShowResetButtons = _showResetButtonsCheckbox.GetValue();
            }
            
            if (_monitorGUI != null && _devModeCheckbox != null)
            {
                _monitorGUI.DevMode = _devModeCheckbox.GetValue();
            }
        }
        
        /// <summary>
        /// Called when dev mode checkbox changes.
        /// </summary>
        private void OnDevModeChanged()
        {
            bool devMode = _devModeCheckbox.GetValue();
            DevLogger.SetDevMode(devMode);
            
            if (_monitorGUI != null)
            {
                _monitorGUI.DevMode = devMode;
            }
            
            if (devMode)
            {
                DevLogger.Log("Developer mode enabled!");
                DevLogger.Log("Logging all job information...");
                
                foreach (var handler in _jobHandlers)
                {
                    DevLogger.LogJobInfo(handler);
                }
            }
            else
            {
                DevLogger.Log("Developer mode disabled");
                DevLogger.FlushToFile();
            }
        }
        
        /// <summary>
        /// Called when mod is unloading.
        /// </summary>
        private void Mod_OnSave()
        {
            DevLogger.Cleanup();
        }
    }
}
