namespace JobCooldowns
{
    /// <summary>
    /// Interface for job cooldown handlers.
    /// Implement this interface to add new job cooldown configurations.
    /// </summary>
    public interface IJobCooldownHandler
    {
        /// <summary>
        /// Unique identifier for this job cooldown handler.
        /// </summary>
        string JobId { get; }

        /// <summary>
        /// Display name for this job in the settings menu.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Default cooldown time in minutes.
        /// </summary>
        int DefaultCooldownMinutes { get; }

        /// <summary>
        /// Minimum cooldown time in minutes.
        /// </summary>
        int MinCooldownMinutes { get; }

        /// <summary>
        /// Maximum cooldown time in minutes.
        /// </summary>
        int MaxCooldownMinutes { get; }

        /// <summary>
        /// Current cooldown time in minutes.
        /// </summary>
        int CurrentCooldownMinutes { get; set; }

        /// <summary>
        /// Applies the cooldown to the game.
        /// Called when the game loads and when settings change.
        /// </summary>
        void ApplyCooldown();

        /// <summary>
        /// Checks if the job GameObject exists in the game.
        /// </summary>
        /// <returns>True if the job is available, false otherwise.</returns>
        bool IsAvailable();

        /// <summary>
        /// Gets the remaining cooldown time in seconds.
        /// </summary>
        /// <returns>Remaining time in seconds, or -1 if not available.</returns>
        float GetRemainingCooldownSeconds();

        /// <summary>
        /// Resets the cooldown to zero (makes the job immediately available).
        /// </summary>
        void ResetCooldown();

        /// <summary>
        /// Gets the current FSM state name of the job.
        /// This indicates what the job is currently doing (e.g., "State 0", "Random wait", "Call", etc.)
        /// </summary>
        /// <returns>The current state name, or null if not available.</returns>
        string GetCurrentState();
    }
}
