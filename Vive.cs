
using System.Collections;
using UnityEngine;

public class Vive : MonoBehaviour
{
    static public Vive instance;

    private static GameObject _headsetCameraRig;
    private static GameObject _headsetCamera;
    private static GameObject _controllerL;
    private static GameObject _controllerR;
    private static float _userHeight;

    private static SteamVR_Controller.Device _controllerLScript;
    private static SteamVR_Controller.Device _controllerRScript;
    private SteamVR_TrackedObject trackedControllerL;
    private SteamVR_TrackedObject trackedControllerR;
    private static float _headsetHeight;

    private GameObject currentCollider;
    private Vector3 lastPosition;
    private BoxCollider[] boxColliders;

    private uint errorMsgCounter = 0;


    //=============================================================================
    // Initialization functions
    //=============================================================================


    void Awake()
    {
        instance = this;

        RegisterRightController(GameObject.Find("Controller (right)")); //#ToDo: verbessern?
        RegisterLeftController(GameObject.Find("Controller (left)"));

        trackedControllerL = _controllerL.GetComponent<SteamVR_TrackedObject>(); //#ToDo: name etc verbessern
        trackedControllerR = _controllerR.GetComponent<SteamVR_TrackedObject>(); //#ToDo: name etc verbessern
    }

    void Start()
    {
        InitHeadsetReferencePoint();
        InitCustom();
    }

    public void RegisterRightController(GameObject controllerObject)
    {
        try
        {
            _controllerR = controllerObject;
            Debug.Log("[SPY] Right Controller found.");
        }
        catch
        {
            Debug.LogError("[SPY] Couldn't find Right Controller!");
        }

    }

    public void RegisterLeftController(GameObject controllerObject)
    {
        try
        {
            _controllerL = controllerObject;
            Debug.Log("[SPY] Left Controller found.");
        }
        catch
        {
            Debug.LogError("[SPY] Couldn't find Left Controller!");
        }

    }

    private void InitHeadsetReferencePoint()
    {
        //Transform eyeCamera = GameObject.FindObjectOfType<SteamVR_Camera>().GetComponent<Transform>();
        _headsetCamera = GameObject.FindObjectOfType<SteamVR_Camera>().gameObject;
        if (_headsetCamera == null)
            Debug.LogError("[SPY] Couldn't find SteamVR camera! Please import SteamVr Camera Rig Prefab.");
        // The referece point for the camera is two levels up from the SteamVR_Camera
        //HeadsetCameraRig = eyeCamera.parent.parent;
        _headsetCameraRig = _headsetCamera.transform.parent.gameObject;
        _headsetHeight = _headsetCameraRig.transform.position.y;
    }

    /// <summary>
    /// Custom initialization function.
    /// </summary>
    private void InitCustom()
    {
        lastPosition = transform.position;

        boxColliders = GetComponents<BoxCollider>();

        if (boxColliders.Length == 0)
            Debug.LogWarning("Couldn't find CameraRig colliders");
    }


    //=============================================================================
    // Getter methods
    //=============================================================================

    public static GameObject HeadsetCamera
    {
        get { return _headsetCamera; }
    }

    public static GameObject HeadsetCameraRig
    {
        get { return _headsetCameraRig; }
    }

    public static GameObject ControllerL
    {
        get { return _controllerL; }
    }

    public static GameObject ControllerR
    {
        get { return _controllerR; }
    }

    public static bool IsLeftSystemPressed
    {
        get
        {
            try { return _controllerLScript.GetPress(SteamVR_Controller.ButtonMask.System); }
            catch { return false; }
        }
    }

    public static bool IsRightSystemPressed
    {
        get
        {
            try { return _controllerRScript.GetPress(SteamVR_Controller.ButtonMask.System); }
            catch { return false; }
        }
    }

    public static bool IsLeftTriggerPressed
    {
        get
        {
            try { return _controllerLScript.GetPress(SteamVR_Controller.ButtonMask.Trigger); }
            catch { return false; }
        }
    }

    public static bool IsRightTriggerPressed
    {
        get
        {
            try { return _controllerRScript.GetPress(SteamVR_Controller.ButtonMask.Trigger); }
            catch { return false; }
        }
    }

    public static bool IsRightTouchpadPressed
    {
        get
        {
            try { return _controllerRScript.GetPress(SteamVR_Controller.ButtonMask.Touchpad); }
            catch { return false; }
        }
    }

    public static bool IsLeftTouchpadPressed
    {
        get
        {
            try { return _controllerLScript.GetPress(SteamVR_Controller.ButtonMask.Touchpad); }
            catch { return false; }
        }
    }

    /// <summary>
    /// Returns the user height in local coordinates.
    /// </summary>
    public static float UserHeight
    {
        get { return _userHeight; }
    }

    /*public Vector2 GetTouchpadValues()
{
    return device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
}*/

    //=============================================================================
    // Main functions
    //=============================================================================

    public static void FadeToBlack(float duration)
    {
        SteamVR_Fade.Start(Color.black, duration);
    }

    public static void FadeToWhite(float duration)
    {
        SteamVR_Fade.Start(Color.white, duration);
    }

    public static void FadeToBlank(float duration)
    {
        SteamVR_Fade.Start(Color.clear, duration);
    }

    public static void TeleportPlayerToPos(Vector3 pos)
    {
        HeadsetCameraRig.transform.position = pos;
    }

    public static void VibrateRController(float duration)
    {
        if (_controllerRScript != null)
            instance.StartCoroutine(instance.Vibrate((uint)(40f * duration), _controllerRScript));
    }

    public static void VibrateLController(float duration)
    {
        if(_controllerLScript!=null)
            instance.StartCoroutine(instance.Vibrate((uint)(40f * duration), _controllerLScript));
    }



    //=============================================================================
    // Standard functions
    //=============================================================================

    void Update()
    {
        try
        {
            foreach (var collider in boxColliders)
                collider.center = new Vector3(_headsetCamera.transform.localPosition.x, collider.center.y, _headsetCamera.transform.localPosition.z);

            lastPosition = transform.position;
            _userHeight = Mathf.Max(_headsetCamera.transform.localPosition.y, _userHeight);
        }
        catch { }
    }

    void FixedUpdate()
    {
        try
        {
            _controllerLScript = SteamVR_Controller.Input((int)trackedControllerL.index);
            _controllerRScript = SteamVR_Controller.Input((int)trackedControllerR.index);
        }
        catch
        {
            if (errorMsgCounter < 5)
            {
                Debug.LogWarning("[SPY] There is no SteamVR_TrackedObject script assigned to the current controller! Are the controllers activated?");
                errorMsgCounter++;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        currentCollider = col.gameObject;
        //Debug.Log(currentCollider); 
    }


    //=============================================================================
    // Helper functions
    //=============================================================================

    /// <summary>
    /// Triggers one controller pulse. one pulse is 2.5 milliseconds.
    /// </summary>
    public IEnumerator Vibrate(uint pulses, SteamVR_Controller.Device controller)
    {
        for (int i = 0; i < pulses; i++)
        {
            controller.TriggerHapticPulse((ushort)2500);
            yield return new WaitForSeconds(0.0025f);
        }
    }


}
