using System;
using System.IO;
using System.Text;
using MSCLoader;
using UnityEngine;

namespace JobCooldowns
{
    /// <summary>
    /// Logger for developer mode - logs to both console and file.
    /// </summary>
    public static class DevLogger
    {
        private static string _logFilePath;
        private static bool _isDevModeEnabled;
        private static StringBuilder _logBuffer = new StringBuilder();
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initialize the dev logger with the log file path.
        /// </summary>
        public static void Initialize(bool devModeEnabled)
        {
            _isDevModeEnabled = devModeEnabled;
            
            if (_isDevModeEnabled)
            {
                // Set log file path to game root directory
                string gameRootPath = Path.Combine(Application.dataPath, "..");
                _logFilePath = Path.Combine(gameRootPath, "JobCooldowns_DevLog.txt");
                
                // Clear existing log or create new
                try
                {
                    lock (_lockObject)
                    {
                        string header = $"=== JobCooldowns Dev Log ===\nSession started: {DateTime.Now}\n\n";
                        File.WriteAllText(_logFilePath, header);
                        _logBuffer.Length = 0; // Clear buffer (.NET 3.5 compatible)
                    }
                    
                    ModConsole.Print($"[JobCooldowns] Dev mode enabled - logging to: {_logFilePath}");
                }
                catch (Exception e)
                {
                    ModConsole.Error($"[JobCooldowns] Failed to initialize dev log file: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Set dev mode state.
        /// </summary>
        public static void SetDevMode(bool enabled)
        {
            _isDevModeEnabled = enabled;
            if (enabled)
            {
                Initialize(true);
            }
        }

        /// <summary>
        /// Log a message (only if dev mode is enabled).
        /// </summary>
        public static void Log(string message)
        {
            if (!_isDevModeEnabled) return;

            string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            
            // Log to console
            ModConsole.Print($"[JobCooldowns DEV] {message}");
            
            // Add to buffer
            lock (_lockObject)
            {
                _logBuffer.AppendLine(timestampedMessage);
                
                // Flush to file every 10 lines to avoid performance issues
                if (_logBuffer.Length > 1000)
                {
                    FlushToFile();
                }
            }
        }

        /// <summary>
        /// Log detailed information about a job handler.
        /// </summary>
        public static void LogJobInfo(IJobCooldownHandler handler)
        {
            if (!_isDevModeEnabled) return;

            Log($"--- Job Info: {handler.DisplayName} ---");
            Log($"  Job ID: {handler.JobId}");
            Log($"  Available: {handler.IsAvailable()}");
            Log($"  Current State: {handler.GetCurrentState() ?? "null"}");
            Log($"  Remaining Cooldown: {handler.GetRemainingCooldownSeconds()}s");
            Log($"  Configured Cooldown: {handler.CurrentCooldownMinutes} minutes");
        }

        /// <summary>
        /// Log FSM state change.
        /// </summary>
        public static void LogStateChange(string jobName, string oldState, string newState)
        {
            if (!_isDevModeEnabled) return;
            Log($"STATE CHANGE: {jobName} - '{oldState}' -> '{newState}'");
        }

        /// <summary>
        /// Flush the buffer to file.
        /// </summary>
        public static void FlushToFile()
        {
            if (!_isDevModeEnabled || string.IsNullOrEmpty(_logFilePath)) return;

            try
            {
                lock (_lockObject)
                {
                    if (_logBuffer.Length > 0)
                    {
                        File.AppendAllText(_logFilePath, _logBuffer.ToString());
                        _logBuffer.Length = 0; // Clear buffer (.NET 3.5 compatible)
                    }
                }
            }
            catch (Exception e)
            {
                ModConsole.Error($"[JobCooldowns] Failed to write to dev log: {e.Message}");
            }
        }

        /// <summary>
        /// Called when mod is being unloaded or game is closing.
        /// </summary>
        public static void Cleanup()
        {
            if (_isDevModeEnabled)
            {
                Log("=== Session ended ===");
                FlushToFile();
            }
        }
    }
}
