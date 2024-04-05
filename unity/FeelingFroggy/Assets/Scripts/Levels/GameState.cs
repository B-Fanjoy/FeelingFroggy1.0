/// <summary>
/// The game state. This is used to determine what the player is currently doing.
/// </summary>
/// <remarks>
/// Current game state is found in <see cref="LevelManager.GameState"/>
/// </remarks>
public enum GameState
{
    /// <summary>
    /// The player is in the main menu screen.
    /// </summary>
    MainMenu,

    /// <summary>
    /// The player is starting a level, but has not yet started.
    /// </summary>
    /// <remarks>
    /// For example: starting cut scene
    /// </remarks>
    LevelStarting,

    /// <summary>
    /// The player is playing a level.
    /// </summary>
    LevelStarted,

    /// <summary>
    /// The player has ended the level. Player has either won or lost. End menu is shown.
    /// </summary>
    LevelEnded
}
