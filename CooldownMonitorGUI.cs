using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace JobCooldowns
{
    /// <summary>
    /// GUI for monitoring job cooldowns in real-time.
    /// </summary>
    public class CooldownMonitorGUI
    {
        private readonly List<IJobCooldownHandler> _jobHandlers;
        private bool _isVisible;
        private bool _showResetButtons;
        private bool _devMode;
        private Rect _windowRect = new Rect(20, 20, 400, 300);
        private const int WINDOW_ID = 12345;

        // GUI Styles
        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _timeStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _unavailableStyle;
        private bool _stylesInitialized;

        public CooldownMonitorGUI(List<IJobCooldownHandler> jobHandlers, bool devMode = false)
        {
            _jobHandlers = jobHandlers;
            _isVisible = false;
            _devMode = devMode;
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public bool ShowResetButtons
        {
            get => _showResetButtons;
            set => _showResetButtons = value;
        }
        
        public bool DevMode
        {
            get => _devMode;
            set => _devMode = value;
        }

        public void ToggleVisibility()
        {
            _isVisible = !_isVisible;
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized) return;

            // Window style
            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f)) },
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };

            // Header style
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.3f, 0.8f, 1f) }
            };

            // Label style
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            // Time style
            _timeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = new Color(0.3f, 1f, 0.3f) }
            };

            // Button style
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                hover = { textColor = new Color(1f, 1f, 0.5f) },
                active = { textColor = Color.yellow }
            };

            // Unavailable style
            _unavailableStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public void DrawGUI()
        {
            if (!_isVisible) return;

            InitializeStyles();
            _windowRect = GUILayout.Window(WINDOW_ID, _windowRect, DrawWindow, "Job Cooldowns Monitor", _windowStyle);
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            // Header
            GUILayout.Label("Active Job Cooldowns", _headerStyle);
            GUILayout.Space(10);

            bool anyJobAvailable = false;

            // Display each job cooldown
            foreach (var handler in _jobHandlers)
            {
                if (!handler.IsAvailable())
                {
                    // Job not available (not in game scene)
                    GUILayout.BeginHorizontal(GUI.skin.box);
                    GUILayout.Label(handler.DisplayName, _labelStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Unavailable", _unavailableStyle);
                    GUILayout.EndHorizontal();
                    continue;
                }

                anyJobAvailable = true;
                float remainingSeconds = handler.GetRemainingCooldownSeconds();
                string currentState = handler.GetCurrentState();
                
                // Only show states for Firewood job (when not in dev mode) or all jobs (when in dev mode)
                string displayState = null;
                bool isFirewoodJob = handler.JobId == "firewood_delivery";
                
                if (_devMode)
                {
                    // Dev mode: show raw state for all jobs
                    displayState = currentState;
                }
                else if (isFirewoodJob)
                {
                    // Normal mode: only show friendly state for Firewood job
                    displayState = GetFriendlyStateName(currentState);
                }
                // For other jobs in normal mode: displayState remains null (hidden)

                GUILayout.BeginHorizontal(GUI.skin.box);

                // Job name
                GUILayout.Label(handler.DisplayName, _labelStyle, GUILayout.Width(180));

                // Current state (always reserve space for alignment, even if empty)
                if (!string.IsNullOrEmpty(displayState))
                {
                    GUIStyle stateStyle = new GUIStyle(_labelStyle)
                    {
                        fontSize = 11,
                        fontStyle = FontStyle.Italic,
                        normal = { textColor = _devMode ? new Color(1f, 0.5f, 0.2f) : new Color(0.7f, 0.9f, 1f) }
                    };
                    GUILayout.Label($"[{displayState}]", stateStyle, GUILayout.Width(100));
                }
                else
                {
                    // Reserve space for alignment even when state is not shown
                    GUILayout.Space(100);
                }

                // Remaining time
                string timeText = FormatTime(remainingSeconds);
                Color timeColor = GetTimeColor(remainingSeconds);
                
                GUIStyle dynamicTimeStyle = new GUIStyle(_timeStyle)
                {
                    normal = { textColor = timeColor }
                };

                GUILayout.Label(timeText, dynamicTimeStyle, GUILayout.Width(100));

                // Reset button (only show if enabled in settings)
                if (_showResetButtons)
                {
                    bool shouldEnable = remainingSeconds > 0;
                    
                    // For Firewood job, only enable when in "Cooldown" state
                    if (isFirewoodJob && !string.IsNullOrEmpty(currentState))
                    {
                        string lowerState = currentState.ToLower();
                        shouldEnable = shouldEnable && (lowerState.Contains("random") && lowerState.Contains("wait"));
                    }
                    
                    GUI.enabled = shouldEnable;
                    if (GUILayout.Button("Reset", _buttonStyle, GUILayout.Width(70)))
                    {
                        handler.ResetCooldown();
                    }
                    GUI.enabled = true;
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            if (!anyJobAvailable)
            {
                GUILayout.Space(10);
                GUILayout.Label("Start or load a game to monitor cooldowns.", _unavailableStyle);
            }

            GUILayout.Space(10);

            // Footer with instructions and warnings
            GUILayout.BeginVertical();
            
            // Interaction instruction
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Press ESC to show cursor and interact with buttons", new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.7f, 0.9f, 1f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // Toggle window instruction
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Press Right Ctrl + J to toggle this window", new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // Important notice about phone calls
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Note: Jobs require a phone call to activate even after cooldown ends", new GUIStyle(GUI.skin.label)
            {
                fontSize = 9,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 1f, 0.7f) }
            });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            // Phone bill warning (only show if unpaid)
            if (!IsPhoneBillPaid())
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("⚠ UNPAID PHONE BILL! Jobs won't call until you pay ⚠", new GUIStyle(GUI.skin.label)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(1f, 0.2f, 0.2f) }
                });
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndVertical();

            GUILayout.EndVertical();

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private string FormatTime(float seconds)
        {
            if (seconds <= 0)
                return "Available!";

            if (seconds < 60)
                return $"{seconds:F0}s";

            int totalMinutes = Mathf.FloorToInt(seconds / 60f);
            int remainingSeconds = Mathf.FloorToInt(seconds % 60f);

            if (totalMinutes < 60)
                return $"{totalMinutes}m {remainingSeconds}s";

            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours}h {minutes}m";
        }

        private Color GetTimeColor(float seconds)
        {
            if (seconds <= 0)
                return new Color(0.3f, 1f, 0.3f); // Green - Available

            if (seconds < 300) // Less than 5 minutes
                return new Color(1f, 1f, 0.3f); // Yellow - Soon

            if (seconds < 1800) // Less than 30 minutes
                return new Color(1f, 0.7f, 0.3f); // Orange

            return new Color(1f, 0.3f, 0.3f); // Red - Long wait
        }

        /// <summary>
        /// Converts technical FSM state names to player-friendly descriptions.
        /// </summary>
        /// <param name="stateName">The FSM state name (e.g., "State 0", "Random wait", "Call")</param>
        /// <returns>A user-friendly description of the state</returns>
        private string GetFriendlyStateName(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return "";

            // Convert state name to lowercase for case-insensitive matching
            string lowerState = stateName.ToLower();

            // Map technical FSM states to player-friendly descriptions
            if (lowerState.Contains("state 0") || lowerState == "state0")
                return "Idle";
            
            if (lowerState.Contains("random") && lowerState.Contains("wait"))
                return "Cooldown";
            
            if (lowerState == "call" || lowerState.Contains("calling"))
                return "Will call";
            
            if (lowerState.Contains("wait") && lowerState.Contains("call"))
                return "Waiting Call";
            
            if (lowerState.Contains("ring"))
                return "Ringing...";
            
            if (lowerState.Contains("work") || lowerState.Contains("job") || lowerState.Contains("active"))
                return "In Progress";
            
            if (lowerState.Contains("done") || lowerState.Contains("complete") || lowerState.Contains("finish"))
                return "Completed";
            
            if (lowerState.Contains("available") || lowerState.Contains("ready"))
                return "Ready";
            
            if (lowerState.Contains("waiting") || lowerState.Contains("wait"))
                return "Waiting";
            
            // If no match found, return the original state with first letter capitalized
            if (stateName.Length > 0)
                return char.ToUpper(stateName[0]) + stateName.Substring(1);
            
            return stateName;
        }

        /// <summary>
        /// Checks if the phone bill is paid.
        /// </summary>
        /// <returns>True if phone bill is paid, false otherwise.</returns>
        private bool IsPhoneBillPaid()
        {
            try
            {
                GameObject systems = GameObject.Find("Systems");
                if (systems == null) return true; // Assume paid if not in game
                
                Transform phoneBillsTransform = systems.transform.Find("PhoneBills");
                if (phoneBillsTransform == null) return true;
                
                PlayMakerFSM phoneBillsFSM = phoneBillsTransform.GetComponent<PlayMakerFSM>();
                if (phoneBillsFSM == null) return true;
                
                FsmBool phonePaid = phoneBillsFSM.FsmVariables.FindFsmBool("PhonePaid");
                if (phonePaid == null) return true;
                
                return phonePaid.Value;
            }
            catch
            {
                // If any error occurs, assume phone is paid to avoid false alarms
                return true;
            }
        }
    }
}
