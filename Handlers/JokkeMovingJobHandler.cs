using MSCLoader;
using HutongGames.PlayMaker;
using UnityEngine;

namespace JobCooldowns.Handlers
{
    /// <summary>
    /// Handler for Jokke Moving job cooldown
    /// </summary>
    public class JokkeMovingJobHandler : IJobCooldownHandler
    {
        private const string GAME_OBJECT_PATH = "JOBS/HouseDrunk/Moving";
        private const string FSM_NAME = "Data";
        private const string FSM_WAIT_TIME_VAR = "WaitTime";

        public string JobId => "JokkeMoving";
        public string DisplayName => "Jokke Moving";
        public int DefaultCooldownMinutes => 167; // ~2.8 hours (10000 seconds default)
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

            var dataFsm = GetDataFSM(jobObject);
            if (dataFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{GAME_OBJECT_PATH}'!");
                return;
            }

            float cooldownSeconds = CurrentCooldownMinutes * 60f;
            
            var waitTimeVar = dataFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            if (waitTimeVar != null)
            {
                // Only set if current value is greater (don't interrupt active job)
                if (waitTimeVar.Value > cooldownSeconds)
                {
                    waitTimeVar.Value = cooldownSeconds;
                }
                
                ModConsole.Print($"[JobCooldowns] Jokke Moving cooldown set to {CurrentCooldownMinutes} minutes ({cooldownSeconds}s)");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find WaitTime variable on Jokke Moving job!");
            }
        }

        public bool IsAvailable()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return false;

            var dataFsm = GetDataFSM(jobObject);
            if (dataFsm == null) return false;

            var waitTimeVar = dataFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            return waitTimeVar != null && waitTimeVar.Value <= 0f;
        }

        public float GetRemainingCooldownSeconds()
        {
            var jobObject = GameObject.Find(GAME_OBJECT_PATH);
            if (jobObject == null) return 0f;

            var dataFsm = GetDataFSM(jobObject);
            if (dataFsm == null) return 0f;

            var waitTimeVar = dataFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
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

            var dataFsm = GetDataFSM(jobObject);
            if (dataFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{GAME_OBJECT_PATH}'!");
                return;
            }

            var waitTimeVar = dataFsm.FsmVariables.FindFsmFloat(FSM_WAIT_TIME_VAR);
            if (waitTimeVar != null)
            {
                waitTimeVar.Value = 0f;
                ModConsole.Print($"[JobCooldowns] Jokke Moving cooldown reset!");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find WaitTime variable on Jokke Moving job!");
            }
        }

        private PlayMakerFSM GetDataFSM(GameObject gameObject)
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

            var dataFsm = GetDataFSM(jobObject);
            if (dataFsm == null) return null;

            return dataFsm.ActiveStateName;
        }
    }
}
