using FunnyThings.GarageDoor;
using FunnyThings.Utils;
using Kitchen.Components;
using KitchenData;
using KitchenMods;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace FunnyThings
{
    public class Main : BaseMain
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Funny Things";
        public const string MOD_VERSION = "0.1.0";

        internal static AssetBundle Bundle;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_VERSION, Assembly.GetExecutingAssembly()) { }

        public override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            LogInfo("Done loading asset bundle.");
        }

        public override void PreInject()
        {
        }

        public override void PostInject()
        {
            if (GameData.Main.TryGet(-233558851, out Appliance garageDecorations))
            {
                if (!garageDecorations.Properties.Select(x => x.GetType()).Contains(typeof(CGarageDecorations)))
                {
                    garageDecorations.Properties.Add(new CGarageDecorations());
                }
                Transform doorLight = garageDecorations.Prefab?.transform.Find("Door Light");
                if (doorLight != null)
                {
                    doorLight.transform.localPosition = new Vector3(-2.51f, 1.5f, -3.58f);
                    doorLight.transform.localScale = new Vector3(0.2544f, 2.5369f, 3f);
                }
                Transform door = garageDecorations.Prefab?.transform.Find("LoadoutDoor");
                Transform shutter = door?.Find("Cube");
                if (door != null)
                {
                    GarageDoorView garageDoorView = door.gameObject.AddComponent<GarageDoorView>();
                    garageDoorView.GarageShutterDoor = shutter;
                    garageDoorView.ParcelPrefab = GameData.Main.TryGet(-1936421857, out Appliance parcel) ? parcel.Prefab : null;

                    SoundSource soundSource = door.gameObject.AddComponent<SoundSource>();
                    soundSource.TransitionTime = 0.1f;
                    soundSource.ShouldLoop = false;
                    garageDoorView.SoundSource = soundSource;
                    garageDoorView.AudioClip = BundleUtils.LoadAudioClipFromAssetBundle(Bundle, "CarCrashSkid.wav");
                }
            }
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
