using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
        public bool isMute;

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
                masterVolume = masterVolume,
                isBgmOn = isBgmOn,
                bgmVolume = bgmVolume,
                sfxVolume = sfxVolume,
                isMute = isMute,
                screenMode = screenMode,
                resolutionIndex = resolutionIndex,
                qualityIndex = qualityIndex,
                frameRateMode = frameRateMode,
                mouseSensitivity = mouseSensitivity,
                useScreenShake = useScreenShake
            };
        }
    }

    // 인게임 플레이 데이터 클래스
    [Serializable]
    public class InGameSaveData
    {
        public string playerName = string.Empty;
        
        public int currentStageIndex;
        
        public int totalPlayTimeSec;

        public int killCount;
        public int totalKillCount;
        public int highScore;

        // 필요 시 인벤토리 목록 등 추가 확장 가능
    }
    
    public static class SaveSystem
    {
        private static readonly string Path = "gpsW6VLEUx6q2vM";
        private static readonly string SaveDirectory = System.IO.Path.Combine(Application.persistentDataPath, Path);

        private static readonly string EncryptKey = "HaHaHuHe_SecretKey_32Bytes_Long!"; 
        private static readonly string EncryptIV = "HaHaHuHe_IV_16B!";
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 지정한 파일명으로 데이터를 JSON 저장합니다.
        /// </summary>
        public static void Save<T>(string fileName, T data) where T : class
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return;
#else
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                var fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
                
                // 용량 최적화 및 구조 파악 방지를 위해 prettyPrint를 false로 설정
                var json = JsonUtility.ToJson(data, false);
                
                // JSON 문자열을 암호화된 바이트 배열로 변환 후 저장
                var encryptedBytes = EncryptStringToBytes(json);
                File.WriteAllBytes(fullPath, encryptedBytes);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
        }

        /// <summary>
        /// 지정한 파일명의 JSON을 읽어옵니다. 파일이 없으면 자동으로 새 인스턴스를 반환합니다.
        /// </summary>
        public static T Load<T>(string fileName) where T : class, new()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new T();
#else
            var fullPath = System.IO.Path.Combine(SaveDirectory, fileName);
            try
            {
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"[SaveSystem] {fileName} 파일이 없어 새로 생성합니다.");
                    var newData = new T();
                    Save(fileName, newData); // 기존 중복 코드를 Save 호출로 간소화
                    return newData;
                }

                // 암호화된 바이트 배열 읽기 및 복호화
                var encryptedBytes = File.ReadAllBytes(fullPath);
                var json = DecryptBytesToString(encryptedBytes);
                
                var data = JsonUtility.FromJson<T>(json);
                return data ?? new T();
            }
            catch (CryptographicException)
            {
                // 유저가 메모장이나 헥스 에디터로 파일을 임의 수정해 복호화가 깨진 경우
                var resetData = new T();
                Save(fileName, resetData);
                return resetData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new T();
            }
#endif
        }
        
        private static byte[] EncryptStringToBytes(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptKey);
                aes.IV = Encoding.UTF8.GetBytes(EncryptIV);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                    }

                    return ms.ToArray();
                }
            }
        }

        private static string DecryptBytesToString(byte[] cipherText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptKey);
                aes.IV = Encoding.UTF8.GetBytes(EncryptIV);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherText))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
