using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoadPlayerAssets : NetworkBehaviour
{
    [SerializeField] private GameObject healthPrefabSlider;

    [SerializeField] private GameObject movementJoystick;
    //[SerializeField] private GameObject viewingJoystick;
    [SerializeField] private GameObject jumpButton;
    [SerializeField] private GameObject shootButton;
    [SerializeField] private GameObject reloadButton;
    [SerializeField] private GameObject grenadeButton;
    [SerializeField] private GameObject trapButton;

    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject cineCam;

    void Start() {
        if (!IsOwner)  return;
        
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            Debug.Log("Loading with Mobile into Junkyard!");
            //Load();
        }

        LoadCamera();
        LoadEssentials();

        MobileCheck.CheckForMobile += Load;
    }

    public void LoadCamera() {
        Instantiate(mainCam);
        Instantiate(cineCam);
    }

    public void LoadEssentials() {
        Transform canvas = Object.FindObjectOfType<Canvas>().transform;
        Instantiate(healthPrefabSlider, canvas);
    }

    public void Load() {
        Debug.Log("<color=green>Loaded Player Assets!</color>");
        Transform canvas = Object.FindObjectOfType<Canvas>().transform;

        Joystick moveStick = Instantiate(movementJoystick).GetComponent<Joystick>();
        //Joystick viewStick = Instantiate(viewingJoystick).GetComponent<Joystick>();
        Button jump        = Instantiate(jumpButton).GetComponent<Button>();
        Button shoot       = Instantiate(shootButton).GetComponent<Button>();
        Button reload      = Instantiate(reloadButton).GetComponent<Button>();
        Button grenade     = Instantiate(grenadeButton).GetComponent<Button>();
        Button trap        = Instantiate(trapButton).GetComponent<Button>();

        moveStick.transform.SetParent(canvas, false);
        //viewStick.transform.SetParent(canvas, false);
        jump.transform.SetParent(canvas, false);
        shoot.transform.SetParent(canvas, false);
        reload.transform.SetParent(canvas, false);
        grenade.transform.SetParent(canvas, false);
        trap.transform.SetParent(canvas, false);

        RectTransform moveRect    = moveStick.GetComponent<RectTransform>();
        //RectTransform viewRect  = viewStick.GetComponent<RectTransform>();
        RectTransform jumpRect    = jump.GetComponent<RectTransform>();
        RectTransform shootRect   = shoot.GetComponent<RectTransform>();
        RectTransform reloadRect  = reload.GetComponent<RectTransform>();
        RectTransform grenadeRect = grenade.GetComponent<RectTransform>();
        RectTransform trapRect    = trap.GetComponent<RectTransform>();

        /*moveRect.anchoredPosition  = new Vector2( 320f, 320f);
        //viewRect.anchoredPosition  = new Vector2(-320f, 320f);
        jumpRect.anchoredPosition  = new Vector2(-600f, 100f);
        shootRect.anchoredPosition = new Vector2(-500f, 400f);*/

        moveRect.offsetMin    = Vector2.zero;   moveRect.offsetMax    = Vector2.zero;
        jumpRect.offsetMin    = Vector2.right;  jumpRect.offsetMax    = Vector2.right;
        shootRect.offsetMin   = Vector2.right;  shootRect.offsetMax   = Vector2.right;
        reloadRect.offsetMin  = Vector2.right;  reloadRect.offsetMax  = Vector2.right;
        grenadeRect.offsetMin = Vector2.right;  grenadeRect.offsetMax = Vector2.right;
        trapRect.offsetMin    = Vector2.right;  trapRect.offsetMax    = Vector2.right;

        float cHeight = canvas.GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log("<color=gray>Size Delta Y of MainCanvas is:  " + cHeight + "</color>");
        moveRect.sizeDelta    = cHeight * 0.25f * Vector2.one;      moveRect.anchoredPosition    = (50f + cHeight * 0.125f) * Vector2.one;
        jumpRect.sizeDelta    = cHeight * 0.2f * Vector2.one;       jumpRect.anchoredPosition    = new Vector2(-cHeight * 0.25f - 40f, 20f);
        shootRect.sizeDelta   = cHeight * 0.25f * Vector2.one;      shootRect.anchoredPosition   = new Vector2(-20f, 20f);
        reloadRect.sizeDelta  = cHeight * 0.2f * Vector2.one;       reloadRect.anchoredPosition  = new Vector2(-cHeight * 0.25f - 20f, cHeight * 0.25f + 20f);
        grenadeRect.sizeDelta = cHeight * 0.15f * Vector2.one;      grenadeRect.anchoredPosition = new Vector2(-20f, cHeight * 0.25f + 30f);
        trapRect.sizeDelta    = cHeight * 0.15f * Vector2.one;      trapRect.anchoredPosition    = new Vector2(-20f, (cHeight * 0.25f + 30f) + (cHeight * 0.15f + 20f));

        moveRect.GetChild(0).GetComponent<RectTransform>().sizeDelta    *= 1f - 0.25f;
        jumpRect.GetChild(0).GetComponent<RectTransform>().sizeDelta    *= 1f - 0.2f;
        shootRect.GetChild(0).GetComponent<RectTransform>().sizeDelta   *= 1f + 0.25f;
        reloadRect.GetChild(0).GetComponent<RectTransform>().sizeDelta  *= 1f - 0.2f;
        grenadeRect.GetChild(0).GetComponent<RectTransform>().sizeDelta *= 1f - 0.35f;
        trapRect.GetChild(0).GetComponent<RectTransform>().sizeDelta    *= 1f - 0.35f;

        GetComponent<PlayerMovement>().joystick = moveStick;
        //gameObject.GetComponent<CameraWork>().viewJoystick = viewStick;
        jump.onClick.AddListener(delegate() { GetComponent<PlayerMovement>().Jump(); });
        //shoot.onClick.AddListener(delegate() { GetComponent<Shoot>().FirePressedDown(); });
        reload.onClick.AddListener(delegate() { GetComponent<Shoot>().Reload(); });
        grenade.onClick.AddListener(delegate() { GetComponent<ThrowBehaviour>().Throw(); });
        trap.onClick.AddListener(delegate() { GetComponent<DeployTrap>().Deploy(); });

        shoot.gameObject.AddComponent<ShootButtonBehaviour>().shoot = GetComponent<Shoot>();

        /*EventTrigger shootTrigger = shoot.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener( (eventData) => { GetComponent<Shoot>().FirePressedDown(); });
        shootTrigger.triggers.Add(entry);*/

        this.enabled = false;
    }

    void OnDisable() {
        MobileCheck.CheckForMobile -= Load;
    }
}
