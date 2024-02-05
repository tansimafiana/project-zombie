using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveCash(CurrencyManager manager) {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/manager.cash";
        FileStream stream = new FileStream(path, FileMode.Create);

        UserData data = new UserData(manager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static UserData LoadCash() {
        string path = Application.persistentDataPath + "/manager.cash";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            UserData data = formatter.Deserialize(stream) as UserData;              // TODO: Implement a system to generate new file if one is not found,
                                                                                    //  similar to InventoryLoad()'s system.
            stream.Close();
            return data;
        } else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveShopInventory(ShopMaster manager) {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/shopinv.inv";
        FileStream stream = new FileStream(path, FileMode.Create);

        UserData data = new UserData(manager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static UserData LoadShopInventory() {
        string path = Application.persistentDataPath + "/shopinv.inv";

        if (!File.Exists(path)) {   // We first instantiate a new save file if we can't find it
            Debug.LogError("Save file not found in " + path);

            ShopMaster tempMaster = new ShopMaster();
            string loadMessage = "<color=blue>Instantiating new inventory file...\tSaved weapons:  ";
            loadMessage += tempMaster.starterPrimary + ", ";
            loadMessage += tempMaster.starterSecondary + ", ";
            loadMessage += tempMaster.starterMelee + ".</color>";
            Debug.Log(loadMessage);

            SaveSystem.SaveShopInventory(tempMaster);
        }
        // Then, we perform the loading procedure...

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        UserData data = formatter.Deserialize(stream) as UserData;

        stream.Close();
        return data;
    }

    public static void SaveLevelData(CharacterSelectBehaviour manager) {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/charSelect.lvl";
        FileStream stream = new FileStream(path, FileMode.Create);

        UserData data = new UserData(manager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static UserData LoadLevelData() {
        string path = Application.persistentDataPath + "/charSelect.lvl";

        if (!File.Exists(path)) {   // We first instantiate a new save file if we can't find it
            Debug.LogError("Save file not found in " + path);

            CharacterSelectBehaviour tempMaster = new CharacterSelectBehaviour();
            string loadMessage = "<color=blue>Instantiating new charSelect.lvl file...</color>";
            Debug.Log(loadMessage);

            SaveSystem.SaveLevelData(tempMaster);
        }
        // Then, we perform the loading procedure...

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        UserData data = formatter.Deserialize(stream) as UserData;

        stream.Close();
        return data;
    }
}
