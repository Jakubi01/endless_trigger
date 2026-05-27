using System;
using System.IO;
using Types;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class SettingData
    {
        public float masterVolume = 1.0f;
        public bool isBgmOn = true;
        public float bgmVolume = 1.0f;
        public float sfxVolume = 1.0f;
        public bool isMute = false;

        public ScreenMode screenMode = ScreenMode.FullScreen;
        public int resolutionIndex = -1;
        public int qualityIndex = 2;
        public FrameRateMode frameRateMode = FrameRateMode.FPS60;

        public float mouseSensitivity = 1.0f;
        public bool useScreenShake = true;

        public SettingData Clone()
        {
            return new SettingData
            {
                masterVolume = this.masterVolume,
                isBgmOn = this.isBgmOn,
                bgmVolume = this.bgmVolume,
                sfxVolume = this.sfxVolume,
                isMute = this.isMute,
                screenMode = this.screenMode,
                resolutionIndex = this.resolutionIndex,
                qualityIndex = this.qualityIndex,
                frameRateMode = this.frameRateMode,
                mouseSensitivity = this.mouseSensitivity,
                useScreenShake = this.useScreenShake
            };
        }
    }

    // SaveGame.json 형태로 저장될 인게임 플레이 데이터 클래스
    [Serializable]
    public class InGameSaveData
    {
        public string playerName = "HaHaHuHe";
        public int currentStageIndex = 0;
        public int totalPlayTimeSec = 0;
        public int gold = 0;
    
        // 필요 시 인벤토리 목록 등 추가 확장 가능
    }
    
    public static class SaveSystem
    {
        private static readonly string SaveDirectory = Path.Combine(Application.persistentDataPath, "SaveData");

        /// <summary>
        /// 지정한 파일명으로 데이터를 JSON 저장합니다.
        /// </summary>
        public static void Save<T>(string fileName, T data) where T : class
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                string fullPath = Path.Combine(SaveDirectory, fileName);
                string json = JsonUtility.ToJson(data, true);
            
                File.WriteAllText(fullPath, json);
                Debug.Log($"[SaveSystem] 저장 완료 -> 경로: {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 저장 실패 ({fileName}): {e.Message}");
            }
        }

        /// <summary>
        /// 지정한 파일명의 JSON을 읽어옵니다. 파일이 없으면 자동으로 새 인스턴스를 반환합니다.
        /// </summary>
        public static T Load<T>(string fileName) where T : class, new()
        {
            try
            {
                string fullPath = Path.Combine(SaveDirectory, fileName);

                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"[SaveSystem] {fileName} 파일이 없어 새로 생성합니다.");
                    
                    T newData = new T();
            
                    // 파일이 저장될 폴더가 없을 수 있으므로 체크 후 생성
                    if (!Directory.Exists(SaveDirectory))
                    {
                        Directory.CreateDirectory(SaveDirectory);
                    }

                    // 디스크에 기본값 JSON 파일 물리적 생성
                    string jsonStr = JsonUtility.ToJson(newData, true);
                    File.WriteAllText(fullPath, jsonStr);
                    
                    Debug.Log($"[SaveSystem] 저장 완료 -> 경로: {fullPath}");
                    return newData;
                }

                string json = File.ReadAllText(fullPath);
                T data = JsonUtility.FromJson<T>(json);

                return data ?? new T();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] 로드 실패 예외 ({fileName}): {e.Message}");
                return new T();
            }
        }
    }
}