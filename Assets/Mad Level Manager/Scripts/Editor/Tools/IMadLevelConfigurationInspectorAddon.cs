/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

namespace MadLevelManager {

/// <summary>
/// Implement it to be displayed right below level properties in the Level Configuration inspector.
/// </summary>
public interface IMadLevelConfigurationInspectorAddon {

    int GetOrder();

    void OnInspectorGUI(MadLevelConfiguration.Level level);

}

} // namespace