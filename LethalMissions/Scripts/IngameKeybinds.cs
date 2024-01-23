
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;


namespace LethalMissions.Input
{

    internal class IngameKeybinds : LcInputActions
    {
        internal static IngameKeybinds Instance = new IngameKeybinds();
        internal static InputActionAsset GetAsset() => Instance.Asset;

        [InputAction("<keyboard>/j", Name = "[LethalMissions]\nOpen Missions Menu")]
        public InputAction OpenMissionsMenuHotkey { get; set; }
    }


    internal class InputUtilsCompat
    {
        internal static InputActionAsset Asset { get { return IngameKeybinds.GetAsset(); } }
        internal static bool Enabled => Plugin.IsModLoaded("com.rune580.LethalCompanyInputUtils");

        public static InputAction OpenMissionsMenuHotkey => IngameKeybinds.Instance.OpenMissionsMenuHotkey;
    }


}