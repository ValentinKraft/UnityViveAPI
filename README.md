# UnityViveAPI
Single class that tries to implement an easy access to all HTC Vive sensor data/controls in Unity3D.
To access the Vive data just type
```C#
Vive.[attribute]
```
anywhere in your code.

***available data:***
- Vive.ControllerL
- Vive.ControllerR
- Vive.HeadsetCamera
- Vive.HeadsetCameraRig
- Vive.IsLeftTriggerPressed
- Vive.IsRightTriggerPressed
- Vive.UserHeight

***available functions:***
- Vive.VibrateRController(float)
- Vive.VibrateLController(float)
- Vive.TeleportPlayerToPos(UnityEngine.Vector3)
- Vive.FadeToWhite(float)
- Vive.FadeToBlank(float)
- Vive.FadeToBlack(float)

## Installation
- Include Steam's SteamVR in your project.
- Include the "[CameraRig]"-prefab in your scene.
- Add the Vive-script to your scene, e.g. to the CameraRig.
