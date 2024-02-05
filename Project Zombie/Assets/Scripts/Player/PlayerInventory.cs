using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : NetworkBehaviour
{
    public GunSO primary;
    public GunSO secondary;
    public GunSO melee;

    private Transform t_primary;
    private Transform t_secondary;
    private Transform t_melee;
    private int p_clip, s_clip;
    private int p_res, s_res;
    [Space]
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform modelHeadEnd;
    [Space]
    [SerializeField] private GameObject ui_SlotPrefab;
    private Canvas mainCanvas;
    [Space]
    public GunSO equippedWeapon;
    private int prevSlotEquipped = 0;

    void Start() {
        if (!IsOwner)  return;

        OnSceneLoad();
        Debug.Log("<color=gray>[PlayerInventory] was initialized!</color>");
    }

    void OnSceneLoad() {
        InventoryHolder invHolder = FindObjectOfType<InventoryHolder>();

        if (invHolder) {
            primary = invHolder.primaryEquip;
            secondary = invHolder.secondaryEquip;
            melee = invHolder.meleeEquip;

            t_primary   = Instantiate(primary.prefab, playerModel).transform;       Init_Model(t_primary, primary);         AddAssets(t_primary.gameObject, primary);
            t_secondary = Instantiate(secondary.prefab, playerModel).transform;     Init_Model(t_secondary, secondary);     AddAssets(t_secondary.gameObject, secondary);
            t_melee     = Instantiate(melee.prefab, playerModel).transform;         Init_Model(t_melee, melee);             AddAssets(t_melee.gameObject, melee);

            Debug.Log("<color=yellow>[PlayerInventory]  Loaded Inventory:\t[1] " + primary.weaponName
                                                                    + "  - [2] " + secondary.weaponName
                                                                    + "  - [3] " + melee.weaponName + "</color>");

            p_clip = primary.clipSize;      p_res = primary.ammoAmount;
            s_clip = secondary.clipSize;    s_res = secondary.ammoAmount;

            foreach (Transform child in t_primary)    { child.gameObject.layer = LayerMask.NameToLayer("Player"); }    
            foreach (Transform child in t_secondary)  { child.gameObject.layer = LayerMask.NameToLayer("Player"); }
            foreach (Transform child in t_melee)      { child.gameObject.layer = LayerMask.NameToLayer("Player"); }

            Init_Slots();
            StartCoroutine(TryDelayEquip(0.8f));
            EnableAimAnimation();
            EquipSkinAndAttributes(invHolder);
        }
    }

    void Update() {
        if (!IsOwner)  return;

        if      (Input.GetKeyDown(KeyCode.Alpha1))  { EnableWeapon(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  { EnableWeapon(2); }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  { EnableWeapon(3); }
    }

    public bool EnableWeapon(int slot) {
        try {
            t_primary.gameObject.SetActive(false);
            t_secondary.gameObject.SetActive(false);
            t_melee.gameObject.SetActive(false);

            // When we change slots, we take the ammo information OUT of the Shoot script and swap them INTO this script.

            Shoot shoot = GetComponent<Shoot>();

            if        (prevSlotEquipped == 1) {
                shoot.SaveAmmoInInv(out p_clip, out p_res);
            } else if (prevSlotEquipped == 2) {
                shoot.SaveAmmoInInv(out s_clip, out s_res);
            }

            if (slot == 1) {
                t_primary.gameObject.SetActive(true);
                equippedWeapon = primary;
                shoot.AttachWeapon(equippedWeapon, t_primary, p_clip, p_res);
            } else if (slot == 2) {
                t_secondary.gameObject.SetActive(true);
                equippedWeapon = secondary;
                shoot.AttachWeapon(equippedWeapon, t_secondary, s_clip, s_res);
            } else if (slot == 3) {
                t_melee.gameObject.SetActive(true);
                equippedWeapon = melee;
                shoot.AttachWeapon(equippedWeapon, t_melee, -1, -1);
            }


            prevSlotEquipped = slot;

            EnableAimAnimation();
            return true;
        } catch {       // If we catch, that means we return false;
            //Debug.Log("Error. [PlayerInventory] Weapons not initialized.");
            return false;
        }
    }

    public int DisableWeapons() {
        int prevActive = 0;
        try {
            if      (t_primary.gameObject.activeInHierarchy)    { prevActive = 1; }
            else if (t_secondary.gameObject.activeInHierarchy)  { prevActive = 2; }
            else if (t_melee.gameObject.activeInHierarchy)      { prevActive = 3; }

            t_primary.gameObject.SetActive(false);
            t_secondary.gameObject.SetActive(false);
            t_melee.gameObject.SetActive(false);

            DisableAimAnimation();
        } catch {}

        return prevActive;
    }

    void AddAssets(GameObject model, GunSO weapon) {
        if (!model.TryGetComponent<AudioSource>(out AudioSource source)) {
            AudioSource s = model.AddComponent<AudioSource>();
            s.clip = weapon.s_gunshot;
            s.volume = weapon.s_volume;
            s.spatialBlend = 1f;
            s.playOnAwake = false;
        }
    }

    void Init_Model(Transform model, GunSO weapon) {
        model.localPosition    = weapon.handheldPos;
        model.localEulerAngles = weapon.handheldRot;
        model.localScale       = weapon.handheldScl;
    }

    void Init_Slots() {
        Canvas[] cs = FindObjectsOfType<Canvas>();
        foreach (Canvas c in cs) {
            if (c.gameObject.tag == "MainCanvas") {
                mainCanvas = c;
                break;
            }
        }

        GameObject layoutGroup = new GameObject();
        layoutGroup.transform.SetParent(mainCanvas.transform);
        RectTransform _rt = layoutGroup.AddComponent<RectTransform>();

        _rt.localPosition = Vector3.zero;
        _rt.anchorMin = new Vector2(0.5f, 0f);
        _rt.anchorMax = new Vector2(0.5f, 0f);
        _rt.pivot     = new Vector2(0.5f, 0f);

        HorizontalLayoutGroup group = layoutGroup.AddComponent<HorizontalLayoutGroup>();
        group.padding.left = -90;
        group.spacing = 20f;
        group.childControlWidth = false;
        group.childControlHeight = false;

        Button slot1 = Instantiate(ui_SlotPrefab, layoutGroup.transform).GetComponent<Button>();
        Button slot2 = Instantiate(ui_SlotPrefab, layoutGroup.transform).GetComponent<Button>();
        Button slot3 = Instantiate(ui_SlotPrefab, layoutGroup.transform).GetComponent<Button>();
        slot1.transform.SetParent(layoutGroup.transform, false);
        slot2.transform.SetParent(layoutGroup.transform, false);
        slot3.transform.SetParent(layoutGroup.transform, false);
        slot1.onClick.AddListener(delegate() { EnableWeapon(1); });
        slot2.onClick.AddListener(delegate() { EnableWeapon(2); });
        slot3.onClick.AddListener(delegate() { EnableWeapon(3); });


        // Setting items in slots
        Camera canvasCamera = new GameObject("Canvas Camera").AddComponent<Camera>();
        canvasCamera.transform.position = Vector3.up * -100f;
        canvasCamera.orthographic = true;
        canvasCamera.clearFlags = CameraClearFlags.Nothing;
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mainCanvas.worldCamera = canvasCamera;

        Instantiate(primary.prefab, slot1.transform.Find("Object Parent"));
        Instantiate(secondary.prefab, slot2.transform.Find("Object Parent"));
        Instantiate(melee.prefab, slot3.transform.Find("Object Parent"));
    }

    void EnableAimAnimation() {
        Animator p_animator = GetComponent<Animator>();
        p_animator.SetLayerWeight(1, 1f);
    }

    void DisableAimAnimation() {
        Animator p_animator = GetComponent<Animator>();
        p_animator.SetLayerWeight(1, 0f);
    }

    IEnumerator TryDelayEquip(float time) {
        if (!EnableWeapon(1)) {
            yield return new WaitForSeconds(time);
            StartCoroutine(TryDelayEquip(0.2f));
        }
    }

    void EquipSkinAndAttributes(InventoryHolder hol) {
        GetComponentInChildren<SkinnedMeshRenderer>().material.SetTexture("_MainTex", hol.playerSkin);

        Transform hatModel = Instantiate(hol.playerHat.prefab).transform;
        hatModel.parent = modelHeadEnd;
        hatModel.localPosition = hol.playerHat.headPos;
        hatModel.localEulerAngles = hol.playerHat.headRot;
        hatModel.localScale = hol.playerHat.headScale;
        
        hatModel.gameObject.layer = LayerMask.NameToLayer("Player");

        Debug.Log("<color=orange>Equipping Skin and Hat Stuff</color>");
    }
}
