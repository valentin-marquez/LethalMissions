

using UnityEngine.InputSystem;

namespace LethalMissions.Input
{
    internal class InputUtilsCompat

    {
        internal static InputActionAsset Asset { get { return IngameKeybinds.GetAsset(); } }
        internal static bool Enabled => Plugin.IsModLoaded("com.rune580.LethalCompanyInputUtils");


        public static InputAction OpenMissionsMenuHotkey => IngameKeybinds.Instance.OpenMissionsMenuHotkey;
    }
}