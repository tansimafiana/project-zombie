using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectBehaviour : MonoBehaviour
{
    [SerializeField] SkinSO[] skins;
    [SerializeField] HatSO[] hats;
    [SerializeField] GameObject characterPanel;
    [SerializeField] Transform displayModelTransform;
    [SerializeField] Transform displayModelHeadEnd;
    [HideInInspector] public int selectedSkinID;
    [HideInInspector] public int selectedHatID;

    [Space]
    [SerializeField] float commonMultiplier;
    [SerializeField] float fastMultiplier;
    [SerializeField] float bruteMultiplier;

    [Space]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] Slider xpSlider;

    [Space]
    [SerializeField] int _level = 1;
    public int Level {
        get { return _level; }
        set { _level = value; 
              maxXP = value * xp_levelup_add;
              levelText.text = "" + value; }
    }
    [SerializeField] int maxXP = 20;
    [SerializeField] int _currXP = 0;
    public int CurrXP {
        get { return _currXP; }
        set { _currXP = value;//  LevelUpLoop(); 
              xpText.text = "(" + _currXP + " / " + maxXP + ")"; 
              xpSlider.value = (float)value / (float)maxXP; }
    }
    [SerializeField] int xp_levelup_add = 20;


    // Selected Saving Fields
    internal string equippedSkin = "";
    internal string equippedHat = "";

    [SerializeField] private GameObject selectionPanelEntity;
    [SerializeField] private Transform necessitiesTransform;



    void Start() {

        UserData data = SaveSystem.LoadLevelData();
        Debug.Log("Loaded: " + data.Level + " Levels and " + data.XP + " Current XP");
            
        CurrXP = data.XP;
        Level = data.Level;
        equippedSkin = data.equippedSkin;
        equippedHat = data.equippedHat;

        Init_SkinPanels();

        bool loadedFromRound = PlayerPrefs.GetInt("round_complete", 0) != 0;
        if (loadedFromRound) {
            int earnedXP = 0; // = PlayerPrefs.GetInt("zombies_killed", 0);

            earnedXP += (int)(PlayerPrefs.GetInt("commons_killed", 0) * commonMultiplier);
            earnedXP += (int)(PlayerPrefs.GetInt("fasts_killed", 0)   * fastMultiplier);
            earnedXP += (int)(PlayerPrefs.GetInt("brutes_killed", 0)  * bruteMultiplier);

            //PlayerPrefs.SetInt("commons_killed", 0);        // Must reset value afterwards.
            //PlayerPrefs.SetInt("fasts_killed", 0);
            //PlayerPrefs.SetInt("brutes_killed", 0);

            CurrXP += earnedXP;
            Debug.Log("<color=green>Loaded back from round! Gained [" + earnedXP + "] XP!</color>");

            LevelUpLoop();

            //SaveSystem.SaveLevelData(this);
        }

        GameObject entity = Instantiate(selectionPanelEntity, necessitiesTransform);
        entity.GetComponent<SelectionPanelEntityBehaviour>().enabled = true;
        entity.GetComponent<Image>().enabled = true;

        SaveSystem.SaveLevelData(this);
    }

    public void Init_SkinPanels() {
        ClearSelectionPanelContents();

        for (int i = 0; i < skins.Length; i++) {
            GameObject panel_button = Instantiate(characterPanel, transform);
            panel_button.GetComponentInChildren<SkinnedMeshRenderer>().material.SetTexture("_MainTex", skins[i].texture);
            panel_button.name = "" + i;

            Button button = panel_button.GetComponent<Button>();
            button.interactable = false;
            button.onClick.AddListener(delegate() { SelectSkin(panel_button); } );
            
            // Level-Lock Check
            if (Level >= skins[i].levelLock) {
                button.interactable = true;
                panel_button.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (equippedSkin == skins[i]._name)  { SelectSkin(panel_button); }
        }
    }

    public void Init_HatPanels() {
        ClearSelectionPanelContents();

        for (int i = 0; i < hats.Length; i++) {
            GameObject panel_button = Instantiate(characterPanel, transform);
            Destroy(panel_button.transform.GetChild(0).GetChild(0).gameObject);

            //panel_button.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = hats[i]
            GameObject hatModel = Instantiate(hats[i].prefab);
            hatModel.transform.parent = panel_button.transform.GetChild(0);
            hatModel.transform.localPosition = hats[i].shopPos;
            hatModel.transform.localEulerAngles = hats[i].shopRot;
            hatModel.transform.localScale = hats[i].shopScale;
            hatModel.layer = LayerMask.NameToLayer("UI");

            //Instantiate(hats[i], panel_button.transform.GetChild(0));

            Button button = panel_button.GetComponent<Button>();

            panel_button.name = "" + i;
            button.onClick.AddListener(delegate() { SelectHat(panel_button); } );
            button.interactable = false;


            if (Level >= skins[i].levelLock) {
                button.interactable = true;
                panel_button.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (equippedHat == hats[i]._name)  { SelectHat(panel_button); }
        }
    }

    void SelectSkin(GameObject buttonObj) {
        this.selectedSkinID = int.Parse(buttonObj.name);
        this.displayModelTransform.GetComponentInChildren<SkinnedMeshRenderer>().material.SetTexture("_MainTex", skins[selectedSkinID].texture);

        equippedSkin = skins[selectedSkinID]._name;
        SaveSystem.SaveLevelData(this);

        // Tweening Selection Animation
        SelectionPanelEntityBehaviour.Instance.SelectTarget(buttonObj);

        Debug.Log("<color=cyan>Picked Skin ID: " + selectedSkinID + "</color>");
    }

    void SelectHat(GameObject buttonObj) {
        foreach (Transform child in displayModelHeadEnd) {
            Destroy(child.gameObject);
        }

        this.selectedHatID = int.Parse(buttonObj.name);
        
        GameObject hatModel = Instantiate(hats[selectedHatID].prefab);
        hatModel.transform.parent = displayModelHeadEnd;
        hatModel.transform.localPosition = hats[selectedHatID].headPos;
        hatModel.transform.localEulerAngles = hats[selectedHatID].headRot;
        hatModel.transform.localScale = hats[selectedHatID].headScale;
        hatModel.layer = LayerMask.NameToLayer("UI");

        equippedHat = hats[selectedHatID]._name;
        SaveSystem.SaveLevelData(this);

        // Tweening Selection Animation
        SelectionPanelEntityBehaviour.Instance.SelectTarget(buttonObj);

        Debug.Log("<color=cyan>Picked Hat ID: " + selectedHatID + "</color>");
    }

    void ClearSelectionPanelContents() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    void OnDisable() {
        //SaveSystem.SaveLevelData(this);

        InventoryHolder hol = Object.FindObjectOfType<InventoryHolder>();
        hol.playerSkin = skins[selectedSkinID].texture;
        hol.playerHat = hats[selectedHatID];

        PlayerPrefs.SetInt("round_complete", 0);
        SaveSystem.SaveLevelData(this);

        Debug.Log("Saved: " + Level + " Levels and " + CurrXP + " Current XP");
    }

    void LevelUpLoop() {
        while (CurrXP >= maxXP) {
            CurrXP -= maxXP;
            //maxXP += xp_levelup_add;
            Level++;
            CurrXP = CurrXP;
        }

        Init_SkinPanels();
        Init_HatPanels();
    }

    public void GiveXP(int amount){
        CurrXP += amount;
        LevelUpLoop();
    }

    public void ResetLevels() {
        CurrXP = 0;
        Level = 1;
        LevelUpLoop();
        SaveSystem.SaveLevelData(this);
        Level = Level;
        CurrXP = CurrXP;
    }
}
