using UnityEngine;
using TMPro;

namespace Sammoh.Advertisement
{
    public class AdDemoKeyboardController : MonoBehaviour
    {
        private AdDemoController adDemoController;

        // UI references for dropdowns
        [SerializeField] private TMP_Dropdown bannerPositionDropdown;
        [SerializeField] private TMP_Dropdown bannerSizeDropdown;
        [SerializeField] private TMP_Dropdown nativeTemplateDropdown;

        public void Initialize(AdDemoController controller,
            TMP_Dropdown bannerPosition, TMP_Dropdown bannerSize, TMP_Dropdown nativeTemplate)
        {
            adDemoController = controller;
            bannerPositionDropdown = bannerPosition;
            bannerSizeDropdown = bannerSize;
            nativeTemplateDropdown = nativeTemplate;
        }
        
        private void Update()
        {
            // Banner show/hide
            if (Input.GetKeyDown(KeyCode.Alpha1))
                adDemoController?.OnShowBannerClicked();
            if (Input.GetKeyDown(KeyCode.Alpha2))
                adDemoController?.OnHideBannerClicked();

            // Interstitial load/show
            if (Input.GetKeyDown(KeyCode.Alpha3))
                adDemoController?.OnLoadInterstitialClicked();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                adDemoController?.OnShowInterstitialClicked();

            // Rewarded video load/show
            if (Input.GetKeyDown(KeyCode.Alpha5))
                adDemoController?.OnLoadRewardedVideoClicked();
            if (Input.GetKeyDown(KeyCode.Alpha6))
                adDemoController?.OnShowRewardedVideoClicked();

            // Playable load/show
            if (Input.GetKeyDown(KeyCode.Alpha7))
                adDemoController?.OnLoadPlayableClicked();
            if (Input.GetKeyDown(KeyCode.Alpha8))
                adDemoController?.OnShowPlayableClicked();

            // Native load/show/remove
            if (Input.GetKeyDown(KeyCode.Alpha9))
                adDemoController?.OnLoadNativeClicked();
            if (Input.GetKeyDown(KeyCode.Alpha0))
                adDemoController?.OnShowNativeClicked();
            if (Input.GetKeyDown(KeyCode.Minus))
                adDemoController?.OnRemoveNativeClicked();

            // Add coins
            if (Input.GetKeyDown(KeyCode.Equals))
                adDemoController?.OnAddCoinsClicked();

            // Dropdown navigation
            HandleDropdownNavigation(bannerPositionDropdown, KeyCode.Q, KeyCode.W);
            HandleDropdownNavigation(bannerSizeDropdown, KeyCode.E, KeyCode.R);
            HandleDropdownNavigation(nativeTemplateDropdown, KeyCode.T, KeyCode.Y);
        }

        private void HandleDropdownNavigation(TMP_Dropdown dropdown, KeyCode prevKey, KeyCode nextKey)
        {
            if (dropdown == null) return;

            if (Input.GetKeyDown(prevKey))
            {
                dropdown.value = Mathf.Max(0, dropdown.value - 1);
                dropdown.RefreshShownValue();
            }
            if (Input.GetKeyDown(nextKey))
            {
                dropdown.value = Mathf.Min(dropdown.options.Count - 1, dropdown.value + 1);
                dropdown.RefreshShownValue();
            }
        }
    }
}
