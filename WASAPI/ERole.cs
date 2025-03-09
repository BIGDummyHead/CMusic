namespace CMusic.WASAPI;

/// <summary>
/// The ERole enumeration defines constants that indicate the role that the system has assigned to an audio endpoint device.
/// </summary>
public enum ERole : int
{
    /// <summary>
    /// Games, system notification sounds, and voice commands.
    /// </summary>
    eConsole = 0,
    /// <summary>
    /// Music, movies, narration, and live music recording.
    /// </summary>
    eMultimedia,
    /// <summary>
    /// Voice communications (talking to another person).
    /// </summary>
    eCommunications,
    /// <summary>
    /// The number of members in the ERole enumeration (not counting the ERole_enum_count member).
    /// </summary>
    ERole_enum_count
}