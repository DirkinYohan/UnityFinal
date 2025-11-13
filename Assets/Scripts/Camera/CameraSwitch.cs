using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    private bool firstPersonEnable = true;

    public Transform[] weaponsTransformsFirstPerson;
    public Transform[] weaponsTransformsThirdPerson;

    public GameObject[] weapons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            firstPersonEnable = !firstPersonEnable;
            ChangeCamera();
        }
    }

    public void ChangeCamera()
    {
        if (firstPersonEnable)
        {
            firstPersonCamera.enabled = true;
            thirdPersonCamera.enabled = false;

            ChangeWeaponsFirstPerson();  
        }
        else
        {
            firstPersonCamera.enabled = false;
            thirdPersonCamera.enabled = true;

            ChangeWeaponsThirdPerson();  
        }
    }

    public void ChangeWeaponsFirstPerson()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.position = weaponsTransformsFirstPerson[i].position;
            weapons[i].transform.rotation = weaponsTransformsFirstPerson[i].rotation;
            weapons[i].transform.localScale = weaponsTransformsFirstPerson[i].localScale;
        }
    }

    public void ChangeWeaponsThirdPerson()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.position = weaponsTransformsThirdPerson[i].position;
            weapons[i].transform.rotation = weaponsTransformsThirdPerson[i].rotation;
            weapons[i].transform.localScale = weaponsTransformsThirdPerson[i].localScale;
        }
    }
}
