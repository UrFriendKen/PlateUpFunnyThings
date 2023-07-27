using UnityEngine;

namespace FunnyThings.Utils
{
    public static class BundleUtils
    {
        public static AudioClip LoadAudioClipFromAssetBundle(AssetBundle bundle, string assetPath)
        {
            var asset = bundle?.LoadAsset<AudioClip>(assetPath);

            if (asset == null)
            {
                Debug.LogError($"Failed to load asset {assetPath} from AssetBundle");
                return null;
            }

            return asset;
        }
    }
}
