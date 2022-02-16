using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDroneFire : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private GameObject bulletPrefab;


    // Start is called before the first frame update
    void Start()
    {
        //This sure be somewhere else in the future, but for now it's fine.
        Cursor.lockState = CursorLockMode.Locked;

        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            var bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity).GetComponent<TestDroneBullet>();
            bullet.SetBullet(CheckActualSpawnPosition(),mainCam.transform.forward);
        }
    }

    // Finalize the LOGICAL spawning point of the bullet, which COULD vary for different type of bullets. For example...
    // * The "raycast" type bullet would actually spawn in front of the camera instead of drone muzzle.
    // * The "projectile" type bullet would spawn in front of the drone muzzle.
    //
    // Just in case you're wondering, the placeholder you currently see here is for the "raycast" type bullet.
    //
    // This also means that this calculation should be implemented within the bullet instead of here.
    // You're currently seeing this just for the sake of testing drone firing logic.
    // Therefore...
    //
    //TODO: Move this into player bullet implementation in the future.
    Vector3 CheckActualSpawnPosition()
    {
        Vector3 camPlaneToMuzzle = muzzle.position - mainCam.transform.position;
        float dot = Vector3.Dot(camPlaneToMuzzle, mainCam.transform.forward);
        return mainCam.transform.position + mainCam.transform.forward * dot;
    }
}
