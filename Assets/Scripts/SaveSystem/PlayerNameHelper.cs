using System;
using System.Globalization;
using UnityEngine;

#if STEAMWORKS_NET
using Steamworks;
#endif

namespace SaveSystem
{
    public static class PlayerNameHelper
    {
        private const string DefaultName = "player";

        public static string GetDefaultPlayerName()
        {
            // 플랫폼 SDK (Steamworks.NET 기준, Project Settings > Scripting Define Symbols에 등록 필요)
#if STEAMWORKS_NET
            try
            {
                if (SteamManager.Initialized)
                {
                    string steamName = SteamFriends.GetPersonaName();
                    if (!string.IsNullOrWhiteSpace(steamName))
                    {
                        return steamName.Trim();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PlayerNameHelper] Steam 닉네임 로드 실패: {e.Message}");
                return GetFallbackName();
            }
#endif

            // 플랫폼별 OS / 기기 소유자 이름 추출
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                // WebGL은 보안상 OS 계정명(Environment.UserName) 접근이 차단되거나 빈 값을 반환하므로 즉시 Fallback
                return GetFallbackName();

#elif UNITY_ANDROID || UNITY_IOS
                // 모바일은 기기 이름을 활용
                string deviceName = SystemInfo.deviceName;
                if (IsValidName(deviceName) && !IsGenericMobileDeviceName(deviceName))
                {
                    return deviceName.Trim();
                }
#else
                // PC(Windows / macOS / Linux) 및 에디터 환경: OS 로그인 계정명 사용
                var osName = Environment.UserName;
                if (IsValidName(osName))
                {
                    return osName.Trim();
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PlayerNameHelper] 로컬 환경 이름 로드 예외: {e.Message}");
            }

            // 모든 추출이 실패하거나 무의미한 기본값일 경우 최종 대체 이름 반환
            return GetFallbackName();
        }

        private static string GetFallbackName()
        {
            return $"{DefaultName}_" +
                 $"{UnityEngine.Random.Range(1000, 9999)}_" +
                 $"{Guid.NewGuid().ToString("N")[..6].ToUpper(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// 공용 PC나 의미 없는 기본 계정명 필터링
        /// </summary>
        private static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            var trimmed = name.Trim();
            return !trimmed.Equals("User", StringComparison.OrdinalIgnoreCase) &&
                   !trimmed.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                   !trimmed.Equals("Administrator", StringComparison.OrdinalIgnoreCase) &&
                   !trimmed.Equals("Default", StringComparison.OrdinalIgnoreCase) &&
                   !trimmed.Equals("Unknown", StringComparison.OrdinalIgnoreCase) &&
                   !trimmed.Equals("<unknown>", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 모바일 기기 이름이 유저 개인 이름이 아닌 단순 기기 모델명인 경우 필터링
        /// </summary>
        private static bool IsGenericMobileDeviceName(string deviceName)
        {
            var trimmed = deviceName.Trim();
            return trimmed.Equals("iPhone", StringComparison.OrdinalIgnoreCase) ||
                   trimmed.Equals("iPad", StringComparison.OrdinalIgnoreCase) ||
                   trimmed.Equals("Android", StringComparison.OrdinalIgnoreCase);
        }
    }
}
