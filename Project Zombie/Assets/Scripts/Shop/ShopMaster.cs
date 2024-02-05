using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopMaster : MonoBehaviour
{
    [SerializeField] private Transform[] scrollContentPanels;
    [SerializeField] private Transform[] weaponViewParents;
    [SerializeField] private Button buy_equip_button;
    [Space]
    [SerializeField] private GunSO[] primaryWeapons;
    [SerializeField] private GunSO[] secondaryWeapons;
    [SerializeField] private GunSO[] meleeWeapons;
    [HideInInspector] public string p_equip, s_equip, m_equip;
    [Space]
    [HideInInspector] public List<string> boughtPrimaries = new List<string>();
    [HideInInspector] public List<string> boughtSecondaries = new List<string>();
    [HideInInspector] public List<string> boughtMelees = new List<string>();
    [Space]
    [SerializeField] private GameObject slotPanelPrefab;
    [SerializeField] private GameObject lockedPrefab;
    [Space]
    public Transform selectedPanel;

    [Space]
    public string starterPrimary = "Assult_Simple";
    public string starterSecondary = "Handgun";
    public string starterMelee = "Knife";


    public ShopMaster() {
        // Placing starter weapons
        boughtPrimaries.Add(starterPrimary);
        boughtSecondaries.Add(starterSecondary);
        boughtMelees.Add(starterMelee);
    }

    void Start() {
        LoadInventory();

        // We instantiate a 'slotPanelPrefab' for each GunSO weapon we find in the arrays.
        // The panels should have a cost text field, we can assign the cost to the panels.
        Init_Panels();

        // We default to the normal weapons equipped
        EquipSavedWeaponSlots();
    }

    void Init_Panels() {
        for (int i = 0; i < 3; i++) {
            foreach (Transform child in scrollContentPanels[i]) {
                Destroy(child.gameObject);
            }
        }

        SlotLoop(primaryWeapons, 0);
        SlotLoop(secondaryWeapons, 1);
        SlotLoop(meleeWeapons, 2);
    }

    void SlotLoop(GunSO[] arr, int slotNum) {
        foreach (GunSO w in arr) {
            GameObject panel = Instantiate(slotPanelPrefab, scrollContentPanels[slotNum]);
            WeaponHolder weaponHolder = panel.GetComponent<WeaponHolder>();
            weaponHolder.weapon = w;
            weaponHolder.slot = slotNum;

            Button button = panel.GetComponent<Button>();
            //button.onClick.AddListener(delegate() { ReplaceImageInViewport(slotNum, w); });
            button.onClick.AddListener(delegate() { Selected(button.gameObject); });

            // Panel has a cost text field and an image field
            TextMeshProUGUI text = panel.GetComponentInChildren<TextMeshProUGUI>();
            text.text = string.Format("{0:n0}", w.cost);

            Transform parent = panel.transform.Find("Object Parent");
            Transform prefab = Instantiate(w.prefab, parent).transform;
            prefab.localScale = new Vector3(5f, 5f, 5f);
            prefab.localPosition = Vector3.zero;
            prefab.localEulerAngles = new Vector3(30f, 90f, 0f);

            prefab.gameObject.layer = LayerMask.NameToLayer("UI");
            foreach (Transform child in prefab) {
                child.gameObject.layer = LayerMask.NameToLayer("UI");
            }

            // Checking if we should lock it...
            bool bought = CheckIfBought(w.weaponName, slotNum);
            if (bought) {
                // Give checkmark and stuff
                weaponHolder.isUnlocked = true;
                ReplaceCostSymbol(panel);
            } else {
                // Lock it
                weaponHolder.isUnlocked = false;
                Instantiate(lockedPrefab, panel.transform);
            }
        }
    }

    public void ReplaceImageInViewport(int slot, GunSO newWeapon) {
        foreach (Transform child in weaponViewParents[slot]) {
            Destroy(child.gameObject);
        }

        Transform prefab = Instantiate(newWeapon.prefab, weaponViewParents[slot]).transform;
        prefab.localPosition = Vector3.zero;
        prefab.localEulerAngles = new Vector3(-30f, 90f, 0f);
        prefab.localScale = Vector3.one;

        prefab.gameObject.layer = LayerMask.NameToLayer("UI");
        foreach (Transform child in prefab) {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

    public void Selected(GameObject buttonObject) {
        if (selectedPanel != null)  { Destroy(selectedPanel.GetComponent<Outline>()); }
        selectedPanel = buttonObject.transform;
        int slotNum = buttonObject.GetComponent<WeaponHolder>().slot;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.5f, 0f);
        outline.effectDistance = new Vector2(2f, 2f);


        Button button = buttonObject.GetComponent<Button>();
        buy_equip_button.onClick.RemoveAllListeners();
        TextMeshProUGUI text = buy_equip_button.GetComponentInChildren<TextMeshProUGUI>();

        WeaponHolder holder = buttonObject.GetComponent<WeaponHolder>();
        if (holder.isUnlocked) {        // If it's unlocked, we display "Equip"
            text.text = "Equip";
            buy_equip_button.onClick.AddListener(delegate() { EquipSlot(buttonObject); });
        } else {                        // Else we display "Buy"
            text.text = "Buy";
            buy_equip_button.onClick.AddListener(delegate() { BuySlot(buttonObject); });
        }
    }

    bool CheckIfBought(string weaponName, int slotNum) {
        if (slotNum == 0) {
            if (boughtPrimaries.Contains(weaponName))
                return true;
            return false;
        } else if (slotNum == 1) {
            if (boughtSecondaries.Contains(weaponName))
                return true;
            return false;
        } else if (slotNum == 2) {
            if (boughtMelees.Contains(weaponName))
                return true;
            return false;
        } else {
            Debug.LogError("<color=red>Error. Invalid 'slotNum' value in ShopMaster.CheckIfBought().");
            return false;
        }
    }

    public void SaveInventory()  {
        SaveSystem.SaveShopInventory(this);

        string saveMessage = "<color=lime>Inventory Saved: ";
        foreach (string w in boughtPrimaries)    { saveMessage += w + ", "; }
        foreach (string w in boughtSecondaries)  { saveMessage += w + ", "; }
        foreach (string w in boughtMelees)       { saveMessage += w + ", "; }

        saveMessage += "</color>";
        Debug.Log(saveMessage);
    }
    public void LoadInventory()  {
        UserData data = SaveSystem.LoadShopInventory();

        if (data == null) {     // Our save file doesn't exist -- it's our first time loading the game
            Debug.Log("[ShopMaster] Inventory 'data' is null.");
            SetStarterWeapons();
        }

        string loadMessage = "<color=green>Inventory Loaded: ";
        foreach (string w in data.primaryWeapons)    { loadMessage += w + ", "; }
        foreach (string w in data.secondaryWeapons)  { loadMessage += w + ", "; }
        foreach (string w in data.meleeWeapons)      { loadMessage += w + ", "; }

        loadMessage += "</color>";
        Debug.Log(loadMessage);

        this.boughtPrimaries   = new List<string>(data.primaryWeapons);
        this.boughtSecondaries = new List<string>(data.secondaryWeapons);
        this.boughtMelees      = new List<string>(data.meleeWeapons);

        this.p_equip = data.equippedPrimary;
        this.s_equip = data.equippedSecondary;
        this.m_equip = data.equippedMelee;
    }

    public void SetStarterWeapons() {
        p_equip = "Assult_Simple";
        s_equip = "Handgun";
        m_equip = "Knife";

        boughtPrimaries.Clear();
        boughtSecondaries.Clear();
        boughtMelees.Clear();

        boughtPrimaries.Add(p_equip);
        boughtSecondaries.Add(s_equip);
        boughtMelees.Add(m_equip);

        Init_Panels();
        EquipSavedWeaponSlots();
    }

    public void BuySlot(GameObject slotObj) {
        // Do buying stuff

        int cost = int.Parse(slotObj.GetComponentInChildren<TextMeshProUGUI>().text);       // TODO: Add a check to throw an error if funds are insufficient.
        FindObjectOfType<CurrencyManager>().Cash -= cost;

        ReplaceCostSymbol(slotObj);

        WeaponHolder holder = slotObj.GetComponent<WeaponHolder>();
        holder.isUnlocked = true;
        GetCorrectSlot(holder.slot).Add(holder.weapon.weaponName);

        Selected(slotObj);
        SaveInventory();
    }

    public void EquipSlot(GameObject slotObj) {
        InventoryHolder holder = GameObject.FindObjectOfType<InventoryHolder>();
        GunSO weapon  = slotObj.GetComponent<WeaponHolder>().weapon;
        int slotPlace = slotObj.GetComponent<WeaponHolder>().slot;

        if (slotPlace == 0) {
            RemoveEquipOutline(holder.primaryEquip);
            holder.primaryEquip = weapon;
            p_equip = weapon.weaponName;
        } else if (slotPlace == 1) {
            RemoveEquipOutline(holder.secondaryEquip);
            holder.secondaryEquip = weapon;
            s_equip = weapon.weaponName;
        } else if (slotPlace == 2) {
            RemoveEquipOutline(holder.meleeEquip);
            holder.meleeEquip = weapon;
            m_equip = weapon.weaponName;
        } else {
            Debug.Log("<color=red>[ShopMaster] Error. 'slot' is not an accepted value.");
            return;
        }

        Outline outline = slotObj.AddComponent<Outline>();
        outline.effectColor = Color.green;
        outline.effectDistance = new Vector2(2f, 2f);

        ReplaceImageInViewport(slotPlace, weapon);

        SaveInventory();
    }

    public void EquipSavedWeaponSlots() {
        WeaponHolder[] holders = FindObjectsOfType<WeaponHolder>();
        Debug.Log("<color=orange>Number of holders: " + holders.Length + "</color>");

        foreach (WeaponHolder slot in holders) {
            if      (slot.weapon.name == p_equip)  { EquipSlot(slot.gameObject); }
            else if (slot.weapon.name == s_equip)  { EquipSlot(slot.gameObject); }
            else if (slot.weapon.name == m_equip)  { EquipSlot(slot.gameObject); }
        }
    }

    public void ResetShopInventory() {
        SetStarterWeapons();      // This equips STARTER weapons

        SaveSystem.SaveShopInventory(this);

        Debug.Log("<color=blue>Resetting Inventory...</color>");
    }

    void ReplaceCostSymbol(GameObject slotObj) {
        Destroy(slotObj.GetComponentInChildren<TextMeshProUGUI>().gameObject);
        try   { Destroy(slotObj.transform.Find(lockedPrefab.name + "(Clone)").gameObject); }
        catch {};
    }

    void RemoveEquipOutline(GunSO oldWeapon) { 
        WeaponHolder[] slots = FindObjectsOfType<WeaponHolder>();

        foreach (WeaponHolder slot in slots) {
            try {
                if (slot.weapon.weaponName == oldWeapon.name) {
                    Destroy(slot.GetComponent<Outline>());
                }
            } catch {}
        }
    }

    List<string> GetCorrectSlot(int slotNum) {
        if      (slotNum == 0)  { return boughtPrimaries; }
        else if (slotNum == 1)  { return boughtSecondaries; }
        else if (slotNum == 2)  { return boughtMelees; }
        return null;
    }
}
