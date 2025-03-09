namespace CMusic.WASAPI;

public enum AUDCLNT_SHAREMODE
{
    /// <summary>
    /// The audio stream will run in shared mode. For more information, see Remarks.
    /// </summary>
    AUDCLNT_SHAREMODE_SHARED,
    /// <summary>
    /// The audio stream will run in exclusive mode. For more information, see Remarks.
    /// </summary>
    AUDCLNT_SHAREMODE_EXCLUSIVE
}