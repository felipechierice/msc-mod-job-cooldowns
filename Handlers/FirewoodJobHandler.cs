using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace JobCooldowns.Handlers
{
    /// <summary>
    /// Handles the cooldown configuration for the Firewood delivery job (Livaloinen).
    /// The default game cooldown is 8 real-time hours (28800 seconds / 480 minutes).
    /// </summary>
    public class FirewoodJobHandler : IJobCooldownHandler
    {
        private const string GAME_OBJECT_PATH = "JOBS/HouseWood1";
        private const string FSM_NAME = "Logic";
        private const string FSM_COOLDOWN_VAR = "CoolDownTime";
        private const string FSM_WAIT_TIME_VAR = "WaitTime";

        public string JobId => "firewood_delivery";
        public string DisplayName => "Firewood Delivery (Livaloinen)";
        public int DefaultCooldownMinutes => 500; // 8.3 hours (30000 seconds)
        public int MinCooldownMinutes => 1;
        public int MaxCooldownMinutes => 480; // 8 hours max (game default)
        public int CurrentCooldownMinutes { get; set; }

        public FirewoodJobHandler()
        {
            CurrentCooldownMinutes = DefaultCooldownMinutes;
        }

        public bool IsAvailable()
        {
            return GameObject.Find(GAME_OBJECT_PATH) != null;
        }

        public void ApplyCooldown()
        {
            GameObject jobObject = GameObject.Find(GAME_OBJECT_PATH);
            
            if (jobObject == null)
            {
                // Silently fail - this is expected when not in game
                return;
            }

            PlayMakerFSM fsm = GetLogicFSM(jobObject);
            if (fsm == null)
            {
                ModConsole.Error($"[JobCooldowns] No PlayMakerFSM '{FSM_NAME}' found on {GAME_OBJECT_PATH}");
                return;
            }

            // Convert minutes to seconds for the game
            int cooldownSeconds = CurrentCooldownMinutes * 60;

            FsmFloat cooldownTime = fsm.FsmVariables.FindFsmFloat(FSM_COOLDOWN_VAR);
            FsmFloat waitTime = fsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);

            if (cooldownTime == null)
            {
                ModConsole.Error($"[JobCooldowns] Could not find FSM variable '{FSM_COOLDOWN_VAR}'");
                return;
            }

            // If the current wait time is greater than the new cooldown, adjust it
            // This prevents issues where the timer would never finish
            if (waitTime != null && cooldownSeconds <= waitTime.Value)
            {
                waitTime.Value = cooldownSeconds - 60; // Set 1 minute less than cooldown
            }

            cooldownTime.Value = cooldownSeconds;

            ModConsole.Print($"[JobCooldowns] {DisplayName} cooldown set to {CurrentCooldownMinutes} minutes ({cooldownSeconds} seconds)");
        }

        private PlayMakerFSM GetLogicFSM(GameObject jobObject)
        {
            PlayMakerFSM[] fsms = jobObject.GetComponents<PlayMakerFSM>();
            foreach (PlayMakerFSM fsm in fsms)
            {
                if (fsm.FsmName == FSM_NAME)
                {
                    return fsm;
                }
            }
            return null;
        }

        public float GetRemainingCooldownSeconds()
        {
            GameObject jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return -1f;

            PlayMakerFSM fsm = GetLogicFSM(jobObject);
            if (fsm == null) return -1f;

            FsmFloat waitTime = fsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            FsmFloat cooldownTime = fsm.FsmVariables.FindFsmFloat(FSM_COOLDOWN_VAR);
            
            if (waitTime == null || cooldownTime == null) return -1f;

            // WaitTime increments from 0 to CoolDownTime
            // Remaining time = CoolDownTime - WaitTime
            float remaining = cooldownTime.Value - waitTime.Value;
            return Mathf.Max(0f, remaining);
        }

        public void ResetCooldown()
        {
            GameObject jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null)
            {
                ModConsole.Warning($"[JobCooldowns] Cannot reset cooldown - {GAME_OBJECT_PATH} not found. Are you in the game?");
                DevLogger.Log($"ResetCooldown failed for {DisplayName} - GameObject not found");
                return;
            }

            PlayMakerFSM fsm = GetLogicFSM(jobObject);
            if (fsm == null)
            {
                ModConsole.Error($"[JobCooldowns] No PlayMakerFSM '{FSM_NAME}' found on {GAME_OBJECT_PATH}");
                DevLogger.Log($"ResetCooldown failed for {DisplayName} - FSM not found");
                return;
            }

            FsmFloat waitTime = fsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            FsmFloat cooldownTime = fsm.FsmVariables.FindFsmFloat(FSM_COOLDOWN_VAR);
            
            if (waitTime == null || cooldownTime == null)
            {
                ModConsole.Error($"[JobCooldowns] Could not find required variables");
                DevLogger.Log($"ResetCooldown failed for {DisplayName} - Variables not found");
                return;
            }

            DevLogger.Log($"Resetting {DisplayName} cooldown...");
            ModConsole.Print($"[JobCooldowns] Resetting {DisplayName} cooldown...");
            ModConsole.Print($"  WaitTime before: {waitTime.Value} seconds");
            ModConsole.Print($"  CoolDownTime: {cooldownTime.Value} seconds");
            
            DevLogger.Log($"  Before: WaitTime={waitTime.Value}s, CoolDownTime={cooldownTime.Value}s, State={fsm.ActiveStateName}");
            
            // Set WaitTime = CoolDownTime to make job immediately available
            // The game increments WaitTime from 0 to CoolDownTime, so when they're equal, job is ready
            waitTime.Value = cooldownTime.Value;
            
            ModConsole.Print($"  WaitTime after: {waitTime.Value} seconds");

            ModConsole.Print($"[JobCooldowns] {DisplayName} cooldown reset!");
            ModConsole.Print($"  Job should trigger when the game checks next (may take a few minutes)");
            ModConsole.Print($"  Current FSM State: {fsm.ActiveStateName}");
            
            DevLogger.Log($"  After: WaitTime={waitTime.Value}s, State={fsm.ActiveStateName}");
            DevLogger.Log($"{DisplayName} cooldown reset complete");
        }

        public string GetCurrentState()
        {
            GameObject jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return null;

            PlayMakerFSM fsm = GetLogicFSM(jobObject);
            if (fsm == null) return null;

            return fsm.ActiveStateName;
        }
    }
}
