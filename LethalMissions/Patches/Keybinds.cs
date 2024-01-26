#pragma warning disable Harmony003
using GameNetcodeStuff;
using HarmonyLib;
using LethalMissions.Input;
using UnityEngine.InputSystem;

namespace LethalMissions.Patches
{
    [HarmonyPatch]
    public static class Keybinds
    {
        public static PlayerControllerB LocalPlayerController { get { return StartOfRound.Instance?.localPlayerController; } }



        public static InputActionAsset Asset;
        public static InputActionMap ActionMap;
        static InputAction OpenMissionsMenuHotkey;
        public static InputAction RawScrollAction;

        public static void Initialize()
        {
            bool InputUtilsLoaded = InputUtilsCompat.Enabled;
            if (InputUtilsLoaded)
            {
                Asset = InputUtilsCompat.Asset;
                ActionMap = Asset.actionMaps[0];

                OpenMissionsMenuHotkey = InputUtilsCompat.OpenMissionsMenuHotkey;
                RawScrollAction = new InputAction("LethalMissions.ScrollMenu", binding: "<Mouse>/scroll");
            }
            else
            {
                Asset = new InputActionAsset();
                ActionMap = Asset.AddActionMap("LethalMissions");
                Asset.AddActionMap(ActionMap);
                OpenMissionsMenuHotkey = ActionMap.AddAction("LethalMissions.OpenMissionsMenuHotkey", binding: "<keyboard>/j", interactions: "Press");
                RawScrollAction = new InputAction("LethalMissions.ScrollMenu", binding: "<Mouse>/scroll");
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "OnEnable")]
        [HarmonyPostfix]
        public static void OnEnable()
        {
            Asset.Enable();
            OpenMissionsMenuHotkey.performed += OnPressMissionMenuHotkey;
            OpenMissionsMenuHotkey.canceled += OnPressMissionMenuHotkey;
        }

        [HarmonyPatch(typeof(StartOfRound), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisable()
        {
            Asset.Disable();
            OpenMissionsMenuHotkey.performed -= OnPressMissionMenuHotkey;
            OpenMissionsMenuHotkey.canceled -= OnPressMissionMenuHotkey;
        }

        static void OnPressMissionMenuHotkey(InputAction.CallbackContext context)
        {
            if (LocalPlayerController == null)
            {
                Plugin.LogError("LocalPlayerController is null");
                return;
            }

            if (context.performed && MenuManager.CanOpenMenu())
            {
                MenuManager.ToggleMissionsMenu();
            }
        }
    }
}