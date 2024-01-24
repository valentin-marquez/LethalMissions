
using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;


namespace LethalMissions.Input
{

    internal class IngameKeybinds : LcInputActions
    {
        internal static IngameKeybinds Instance = new();
        internal static InputActionAsset GetAsset() => Instance.Asset;


        [InputAction("<keyboard>/j", Name = "[LethalMissions]\nOpen Missions Menu")]
        public InputAction OpenMissionsMenuHotkey { get; set; }
    }


}