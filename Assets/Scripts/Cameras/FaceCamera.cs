using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);

        // transform.LookAt(transform.position + mainCameraTransform.forward,
        //                     mainCameraTransform.up);

        // transform.rotation = Quaternion.LookRotation(mainCameraTransform.forward,
        //                                      mainCameraTransform.up);

        //healthBarParent.transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized);
    }
}
