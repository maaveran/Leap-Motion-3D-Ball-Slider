/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

namespace MadLevelManager {

/// <summary>
/// Interface of MadLevelProfile save system backend. Implement it and set as
/// MadLevelProfile.backend to override default PlayerPrefs backend.
/// </summary>
public interface IMadLevelProfileBackend {

    /// <summary>
    /// Executed at the start of lifetime. At this point all properties are set.
    /// </summary>
    void Start();

    /// <summary>
    /// Loads the profile string. It should be the same string from SaveProfile() or null if
    /// it is not available.
    /// </summary>
    /// <param name="profileName">The profile name.</param>
    /// <returns>Profile string.</returns>
    string LoadProfile(string profileName);

    /// <summary>
    /// Requests the current profile value to be saved. This method will be called on each change,
    /// so it may be a good idea to buffer these calls.
    /// </summary>
    /// <param name="profileName">The profile name</param>
    /// <param name="value">Profile string.</param>
    void SaveProfile(string profileName, string value);

    /// <summary>
    /// Forces all the data to be saved immediately.
    /// </summary>
    void Flush();


    /// <summary>
    /// Should return true if this backend can be used in the edit mode (editor).
    /// </summary>
    bool CanWorkInEditMode();
}

} // namespace