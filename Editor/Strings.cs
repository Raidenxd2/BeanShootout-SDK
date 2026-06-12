public class Strings
{
    #region Dialogs
    public const string Config_NoDataFolder = "Game Path doesn't contain 'BeanShootout_Data', you may not have selected where the game is located.";
    public const string Config_NoSupportFile = "This game version doesn't support Build and Run, please make sure you have game version 0.9.0-PublicBeta or higher.";
    public const string PlayMode_NoSaveFile = "Warning: The default save file doesn't exist at {0}. A blank save file will be used instead.";
    public const string Build_NoGamePath = "Please set a Game Path in the config!";
    public const string Build_GamePathInvalid = "Game Path isn't valid.\n{0}";
    public const string Build_CleanupAllBuildFolders = "Are you sure you want to cleanup build folders for all levels?\nThis will delete all built files.";
    public const string Build_CleanupCurrentLevelBuildFolders = "Are you sure you want to cleanup build folders for the current level?\nThis will delete all built files.";
    public const string Build_InvalidWindowsGraphicsAPIs = "The GraphicsAPI setting for Windows are not set to the required values (Direct3D 11, Direct3D 12, and Vulkan). You must support these GraphicsAPIs. Do you want to configure them automatically?";
    public const string Build_Finished = "Build created under {0}";
    public const string LevelCreator_AlreadyExists = "This level already exists!";
    public const string LevelThumbnail_Created = "Created thumbnail under {0}";
    public const string LevelThumbnail_Failed = "Failed to save level thumbnail.";
    public const string Publishing_Created = "Created Zip file under {0}";
    public const string Setup_GammaColorSpace = "This project is currently using the Gamma color space. This will cause the color in-game to look off, and is unsupported. Would you like to change the color space to Linear?";
    public const string Setup_LegacyInput = "This project is currently configured to use the legacy Input Manager, which is not supported by Bean Shootout. Do you want to change the active input handling to the Input System? This will restart the Unity Editor.";
    public const string Setup_BothLegacyInputAndInputSystem = "This project is currently configured to use both the legacy Input Manager and the new Input System. The legacy Input Manager is not supported by Bean Shootout. Do you want to change the active input handling to be only the new Input System? this will restart the Unity Editor.";
    public const string Setup_InstalledPackage = "You have just installed the Bean Shootout SDK, it is highly recommended that you install this package in an empty Unity project. Do you want to setup this project for the SDK?";
    public const string Setup_Failed = "Failed to setup project.";
    #endregion

    public const string Setup_SetupFileContent = "This file is used to check if setup has been done. Do not delete this file.";

    public const string SetupFilePath = "Assets/DO_NOT_DELETE_THIS_BeanShootout";
    public const string ConfigPath = "Assets/BeanShootoutConfig.asset";
}