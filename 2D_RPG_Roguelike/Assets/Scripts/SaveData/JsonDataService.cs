using Newtonsoft.Json;
using RobbieWagnerGames.TurnBasedCombat;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace RobbieWagnerGames.Utilities.SaveData
{
    public class JsonDataService : IDataService
    {
        public static JsonDataService Instance {get; private set;}
        public JsonDataService()
        {
            if (Instance != null && Instance != this) 
            { 
                return;
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        public void ResetInstance() => Instance = null;

        public bool SaveData<T>(string RelativePath, T Data, bool Encrypt = false)
        {
            string path = CreateValidDataPath(RelativePath);
            bool result = SaveDataInternal(path, Data, Encrypt);
            return result;
        }

        private bool SaveDataInternal<T>(string FullPath, T Data, bool Encrypt)
        {
            try
            {
                if (File.Exists(FullPath))
                    File.Delete(FullPath);
                
                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
                FileStream stream = File.Create(FullPath);
                stream.Close();
                string saveData = JsonConvert.SerializeObject(Data);
                File.WriteAllText(FullPath, saveData);
                return true;
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());    
                return false;
            }
        }

        public async Task<bool> SaveDataAsync<T>(string RelativePath, T Data, bool Encrypt = false)
        {
            string path = CreateValidDataPath(RelativePath);
            bool result = await Task.Run(() => SaveDataInternal(RelativePath, Data, Encrypt));
            return result;
        }

        public T LoadDataRelative<T>(string RelativePath, T DefaultData, bool saveDefaultIfMissing = false, bool isEncrypted = false)
        {
            string path = CreateValidDataPath(RelativePath);
            return LoadData(path, DefaultData, saveDefaultIfMissing, isEncrypted);
        }

        public T LoadData<T>(string FullPath, T DefaultData, bool saveDefaultIfMissing = false,  bool isEncrypted = false)
        {
            if(!File.Exists(FullPath))
            {
                Debug.LogWarning($"File at path {FullPath} not found, returning default data...");
                if(saveDefaultIfMissing)
                    SaveDataInternal(FullPath, DefaultData, isEncrypted);
                return DefaultData;
            }

            try
            {
                string dataString = File.ReadAllText(FullPath);
                T data = JsonConvert.DeserializeObject<T>(dataString);
                return data;
            }
            catch(Exception e)
            {
                Debug.LogError($"Error loading data: {e}");
                return DefaultData;
            }
        }

        public async Task<T> LoadDataAsync<T>(string RelativePath, T DefaultData, bool saveDefaultIfMissing = false, bool isEncrypted = false)
        {
            string path = CreateValidDataPath(RelativePath);
            T result = await Task.Run(() => LoadData(RelativePath, DefaultData, saveDefaultIfMissing, isEncrypted));
            return result;
        }

        public bool PurgeData()
        {
            string path = StaticGameStats.PersistentDataPath;
            try
            {
                DirectoryInfo pathInfo = new DirectoryInfo(path);
                if(pathInfo != null)
                {
                    foreach (FileInfo file in pathInfo.EnumerateFiles())
                        file.Delete(); 
                    foreach (DirectoryInfo dir in pathInfo.EnumerateDirectories())
                        dir.Delete(true); 
                }

                Debug.LogWarning("All save data has been purged...");

                return true;
            }
            catch(Exception e)
            {
                Debug.LogWarning($"Data could not be purged due to exception\n({e})\n, aborting purge process.");
                return false;
            }
        }

        private string CreateValidDataPath(string relativePath)
        {
            string result = relativePath;
            if(!result.StartsWith('/')) 
                result = '/' + relativePath;
            if(!result.EndsWith(".json"))
                result += ".json";

            return StaticGameStats.PersistentDataPath + result;
        }
    }
}