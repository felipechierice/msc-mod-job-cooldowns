using MSCLoader;
using HutongGames.PlayMaker;
using UnityEngine;

namespace JobCooldowns.Handlers
{
    /// <summary>
    /// Generic handler for sewage well jobs (HouseShit1-5)
    /// Each well has its own cooldown
    /// </summary>
    public class SewageJobHandler : IJobCooldownHandler
    {
        private readonly int _wellNumber; // 1-5
        private readonly string _gameObjectPath;
        private const string FSM_NAME = "Level";
        private const string FSM_WAIT_VAR = "Wait";

        public SewageJobHandler(int wellNumber)
        {
            _wellNumber = wellNumber;
            _gameObjectPath = $"JOBS/HouseShit{wellNumber}/WasteWell_2000litre/Shit/Level{wellNumber}/ShitLevelTrigger";
        }

        public string JobId => $"Sewage{_wellNumber}";
        public string DisplayName => $"Sewage Well {_wellNumber}";
        public int DefaultCooldownMinutes => 50; // ~50 minutes default (game varies 30-60 min based on ShitRate)
        public int MinCooldownMinutes => 1;
        public int MaxCooldownMinutes => 480;
        public int CurrentCooldownMinutes { get; set; }

        public void ApplyCooldown()
        {
            var jobObject = GameObject.Find(_gameObjectPath);
            if (jobObject == null)
            {
                ModConsole.Warning($"[JobCooldowns] GameObject '{_gameObjectPath}' not found!");
                return;
            }

            var levelFsm = GetLevelFSM(jobObject);
            if (levelFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{_gameObjectPath}'!");
                return;
            }

            float cooldownSeconds = CurrentCooldownMinutes * 60f;
            
            var waitVar = levelFsm.FsmVariables.FindFsmFloat(FSM_WAIT_VAR);
            if (waitVar != null)
            {
                // Only set if current value is greater (don't interrupt active job)
                if (waitVar.Value > cooldownSeconds)
                {
                    waitVar.Value = cooldownSeconds;
                }
                
                ModConsole.Print($"[JobCooldowns] Sewage Well {_wellNumber} cooldown set to {CurrentCooldownMinutes} minutes ({cooldownSeconds}s)");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find Wait variable on Sewage Well {_wellNumber}!");
            }
        }

        public bool IsAvailable()
        {
            var jobObject = GameObject.Find(_gameObjectPath);
            if (jobObject == null) return false;

            var levelFsm = GetLevelFSM(jobObject);
            if (levelFsm == null) return false;

            var waitVar = levelFsm.FsmVariables.FindFsmFloat(FSM_WAIT_VAR);
            if (waitVar == null) return false;

            // Check if wait time is low (job is ready)
            // Also check if Called flag is not set (job hasn't been called yet)
            var calledVar = levelFsm.FsmVariables.FindFsmBool("Called");
            bool notCalled = calledVar != null && !calledVar.Value;
            
            return waitVar.Value <= 0f && notCalled;
        }

        public float GetRemainingCooldownSeconds()
        {
            var jobObject = GameObject.Find(_gameObjectPath);
            if (jobObject == null) return 0f;

            var levelFsm = GetLevelFSM(jobObject);
            if (levelFsm == null) return 0f;

            var waitVar = levelFsm.FsmVariables.FindFsmFloat(FSM_WAIT_VAR);
            if (waitVar != null)
            {
                return Mathf.Max(0f, waitVar.Value);
            }

            return 0f;
        }

        public void ResetCooldown()
        {
            var jobObject = GameObject.Find(_gameObjectPath);
            if (jobObject == null)
            {
                ModConsole.Warning($"[JobCooldowns] GameObject '{_gameObjectPath}' not found!");
                return;
            }

            var levelFsm = GetLevelFSM(jobObject);
            if (levelFsm == null)
            {
                ModConsole.Warning($"[JobCooldowns] FSM '{FSM_NAME}' not found on '{_gameObjectPath}'!");
                return;
            }

            var waitVar = levelFsm.FsmVariables.FindFsmFloat(FSM_WAIT_VAR);
            if (waitVar != null)
            {
                waitVar.Value = 0f;
                ModConsole.Print($"[JobCooldowns] Sewage Well {_wellNumber} cooldown reset!");
            }
            else
            {
                ModConsole.Warning($"[JobCooldowns] Could not find Wait variable on Sewage Well {_wellNumber}!");
            }
        }

        private PlayMakerFSM GetLevelFSM(GameObject gameObject)
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
            var jobObject = GameObject.Find(_gameObjectPath);
            if (jobObject == null) return null;

            var levelFsm = GetLevelFSM(jobObject);
            if (levelFsm == null) return null;

            return levelFsm.ActiveStateName;
        }
    }
}
