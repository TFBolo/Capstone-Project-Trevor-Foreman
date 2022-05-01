using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(InventoryObject playerInventory, PetAI petAI, string saveChoice)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        // this will change to hold 3 saves later
        string path = Path.Combine(Application.persistentDataPath, saveChoice);
        
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            PlayerData data = new PlayerData(petAI, JsonUtility.ToJson(playerInventory, true));

            formatter.Serialize(stream, data);
        }
    }

    public static PlayerData LoadPlayer(string saveChoice)
    {
        string path = Path.Combine(Application.persistentDataPath, saveChoice);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;

                return data;
            }
        }
        else
        {
            Debug.Log("File not found" + path);
            return null;
        }
    }
}
