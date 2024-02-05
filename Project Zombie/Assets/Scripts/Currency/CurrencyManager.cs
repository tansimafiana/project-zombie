using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    [SerializeField]
    private int _cash;
    public int Cash {
        get { return _cash; }
        set {
            _cash = value;
            SaveCash();
            UpdateCashDisplay();
        }
    }

    [SerializeField] float commonMultiplier = 1f;
    [SerializeField] float fastMultiplier = 2f;
    [SerializeField] float bruteMultiplier = 4f;


    void Start() {
        if (!Application.isEditor) {        // NOTE: If the application is NOT in editor, the default value will be changed to 0.
            //Cash = 0;
        }

        LoadCash();
        CalculateMatchCash();

        UpdateCashDisplay();
    }

    public void UpdateCashDisplay() {
        string text = string.Format("{0:n0}", Cash);
        displayText.text = text;
    }

    void OnValidate() {             // NOTE: This kinda messes with the Editor's saving system. Although, the saving works just fine.
        Cash = _cash;
    }

    public void SaveCash()  {
        SaveSystem.SaveCash(this);
        Debug.Log("<color=lime>Cash Saved: " + this.Cash + "</color>");
    }
    public void LoadCash()  {
        UserData data = SaveSystem.LoadCash();
        Debug.Log("<color=green>Cash Loaded: " + data.Cash + "</color>");

        Cash = data.Cash;
    }

    void CalculateMatchCash() {
        bool loadedFromRound = PlayerPrefs.GetInt("round_complete", 0) != 0;
        if (loadedFromRound) {
            int earnedCash = 0; // = PlayerPrefs.GetInt("zombies_killed", 0);

            earnedCash += (int)(PlayerPrefs.GetInt("commons_killed", 0) * commonMultiplier);
            earnedCash += (int)(PlayerPrefs.GetInt("fasts_killed", 0)   * fastMultiplier);
            earnedCash += (int)(PlayerPrefs.GetInt("brutes_killed", 0)  * bruteMultiplier);

            Debug.Log("<Color=red>Cash earned: " + earnedCash + "</color>");
            Cash += earnedCash;

            //PlayerPrefs.SetInt("commons_killed", 0);        // Must reset value afterwards.
            //PlayerPrefs.SetInt("fasts_killed", 0);
            //PlayerPrefs.SetInt("brutes_killed", 0);
        }
    }

    void OnDisable() {
        PlayerPrefs.SetInt("commons_killed", 0);        // Must reset value afterwards.
        PlayerPrefs.SetInt("fasts_killed", 0);
        PlayerPrefs.SetInt("brutes_killed", 0);
    }
}
