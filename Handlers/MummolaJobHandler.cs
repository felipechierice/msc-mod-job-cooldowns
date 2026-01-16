using MSCLoader;
using HutongGames.PlayMaker;
using UnityEngine;

namespace JobCooldowns.Handlers
{
    /// <summary>
    /// Handler for Mummola (Grandmother) job cooldown - Fish and grocery deliveries
    /// </summary>
    public class MummolaJobHandler : IJobCooldownHandler
    {
        private const string GAME_OBJECT_PATH = "JOBS/Mummola";
        private const string FSM_NAME = "Logic";
        private const string FSM_COOLDOWN_VAR = "CoolDownTime";
        private const string FSM_WAIT_TIME_VAR = "WaitTime";

        public string JobId => "Mummola";
        public string DisplayName => "Mummola (Grandmother)";
        public int DefaultCooldownMinutes => 150; // 2.5 hours default
        public int MinCooldownMinutes => 1;
        public int MaxCooldownMinutes => 480;
        public int CurrentCooldownMinutes { get; set; }

        public void ApplyCooldown()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null)
            {
                ModConsole.Warning($"[JobCooldowns] GameObject '{GAME_OBJECT_PATH}' not found!");
                return;
            }

            var logicFsm = GetLogicFSM(jobObject);
            if (logicFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{GAME_OBJECT_PATH}'!");
                return;
            }

            float cooldownSeconds = CurrentCooldownMinutes * 60f;
            
            var cooldownVar = logicFsm.FsmVariables.FindFsmFloat(FSM_COOLDOWN_VAR);
            var waitTimeVar = logicFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);

            if (cooldownVar != null && waitTimeVar != null)
            {
                cooldownVar.Value = cooldownSeconds;
                
                // If current wait time is greater than new cooldown, adjust it
                if (waitTimeVar.Value > cooldownSeconds)
                {
                    waitTimeVar.Value = cooldownSeconds;
                }
                
                ModConsole.Print($"[JobCooldowns] Mummola cooldown set to {CurrentCooldownMinutes} minutes ({cooldownSeconds}s)");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find FSM variables on Mummola job!");
            }
        }

        public bool IsAvailable()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return false;

            var logicFsm = GetLogicFSM(jobObject);
            if (logicFsm == null) return false;

            var waitTimeVar = logicFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            return waitTimeVar != null && waitTimeVar.Value <= 0f;
        }

        public float GetRemainingCooldownSeconds()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return 0f;

            var logicFsm = GetLogicFSM(jobObject);
            if (logicFsm == null) return 0f;

            var waitTimeVar = logicFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            if (waitTimeVar != null)
            {
                return Mathf.Max(0f, waitTimeVar.Value);
            }

            return 0f;
        }

        public void ResetCooldown()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null)
            {
                ModConsole.Warning($"[JobCooldowns] GameObject '{GAME_OBJECT_PATH}' not found!");
                return;
            }

            var logicFsm = GetLogicFSM(jobObject);
            if (logicFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{GAME_OBJECT_PATH}'!");
                return;
            }

            var waitTimeVar = logicFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            if (waitTimeVar != null)
            {
                waitTimeVar.Value = 0f;
                ModConsole.Print($"[JobCooldowns] Mummola cooldown reset!");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find WaitTime variable on Mummola job!");
            }
        }

        private PlayMakerFSM GetLogicFSM(GameObject gameObject)
        {
            var fsms = gameObject.GetComponents<PlayMakerFSM>();
            foreach (var fsm in fsms)
            {
                if (fsm.FsmName == FSM_NAME)
                {
                    return fsm;
                }
            }
            return null;
        }

        public string GetCurrentState()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return null;

            var logicFsm = GetLogicFSM(jobObject);
            if (logicFsm == null) return null;

            return logicFsm.ActiveStateName;
        }
    }
}
