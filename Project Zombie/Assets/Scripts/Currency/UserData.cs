[System.Serializable]
public class UserData
{
    public int Cash;
    public int Level;
    public int XP;

    public string[] primaryWeapons;
    public string[] secondaryWeapons;
    public string[] meleeWeapons;

    public string equippedPrimary;
    public string equippedSecondary;
    public string equippedMelee;

    public string equippedSkin;
    public string equippedHat;

    public UserData(CurrencyManager manager) {
        Cash = manager.Cash;
    }

    public UserData(ShopMaster manager) {
        primaryWeapons = manager.boughtPrimaries.ToArray();
        secondaryWeapons = manager.boughtSecondaries.ToArray();
        meleeWeapons = manager.boughtMelees.ToArray();

        equippedPrimary = manager.p_equip;
        equippedSecondary = manager.s_equip;
        equippedMelee = manager.m_equip;
    }

    public UserData(CharacterSelectBehaviour manager) {
        Level = manager.Level;
        XP = manager.CurrXP;

        equippedSkin = manager.equippedSkin;
        equippedHat = manager.equippedHat;
    }
}
