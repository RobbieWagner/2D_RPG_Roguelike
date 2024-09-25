using UnityEngine;

namespace RobbieWagnerGames
{
    public interface IDataService
    {
        bool SaveData<T>(string RelativePath, T Data, bool Encrypt = false);
        T LoadDataRelative<T>(string RelativePath, T DefaultData, bool saveDefaultIfMissing, bool isEncrypted);
        bool PurgeData();
    }
}