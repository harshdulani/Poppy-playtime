using System;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-10)]
        public class YCDataManager : ADataManager {

            private static string ADVERTISING_ID = "ADVERTISING_ID";
            private static string ADSSHOW = "ADSSHOW";
            private static string LANGUAGE = "LANGUAGE";

            private static string GDPR_ADS = "GDPR_ADS";
            private static string GDPR_ANALYTICS = "GDPR_ANALYTICS";
            private static string INTERSTITIALS_NB = "INTERSTITIALS_NB";
            private static string REWARDEDS_NB = "REWARDEDS_NB";
            private static string TIMESTAMP = "TIMESTAMP";

            void Awake() {
                if (this.HasKey(TIMESTAMP) == false) {
                    this.SetInt(TIMESTAMP, this.GetTimestamp());
                }
            }

            public int GetTimestamp() {
                return (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000);
            }

            public int GetDiffTimestamp() {
                return this.GetTimestamp() - this.GetInt(TIMESTAMP);
            }

            //ADVERTISING ID
            public string GetAdvertisingId() {
                return this.GetString(ADVERTISING_ID, "");
            }
            public void SetAdvertisingId(string id) {
                this.SetString(ADVERTISING_ID, id);
            }

            // ADS
            public bool GetAdsShow() {
                return this.GetInt(ADSSHOW, 1) == 1;
            }
            public void BuyAdsShow() {
                this.SetInt(ADSSHOW, 0);
            }

            // LANGUAGE
            public void SetLanguage(string lang) {
                this.SetString(LANGUAGE, lang);
            }
            public string GetLanguage() {
                return this.GetString(LANGUAGE, "EN");
            }
            public bool HasLanguage() {
                return this.HasKey(LANGUAGE);
            }

            // GDPR
            public void SetGdprAds(bool consent) {
                this.SetBool(GDPR_ADS, consent);
            }
            public bool GetGdprAds() {
                return this.GetBool(GDPR_ADS, true);
            }

            public void SetGdprAnalytics(bool analytics) {
                this.SetBool(GDPR_ANALYTICS, analytics);
            }
            public bool GetGdprAnalytics() {
                return this.GetBool(GDPR_ANALYTICS, true);
            }

            // NB INTERSTITIALS
            public int GetInterstitialsNb() {
                return this.GetInt(INTERSTITIALS_NB, 0);
            }
            public void IncrementInterstitialsNb() {
                this.SetInt(INTERSTITIALS_NB, this.GetInterstitialsNb() + 1);
            }

            // NB INTERSTITIALS
            public int GetRewardedsNb() {
                return this.GetInt(REWARDEDS_NB, 0);
            }
            public void IncrementRewardedsNb() {
                this.SetInt(REWARDEDS_NB, this.GetRewardedsNb() + 1);
            }

        }
    }
}