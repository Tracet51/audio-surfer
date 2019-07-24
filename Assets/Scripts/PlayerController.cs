using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("In ms^-1")]
    float XSpeed = 100f;

    [SerializeField]
    [Tooltip("In ms^-1")]
    float XRange = 10f;

    [SerializeField]
    float RollFactor = 10f;

    float XThrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
    }

    private void ProcessTranslation()
    {
        XThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        var xOffsetThisFrame = XThrow * XSpeed * Time.deltaTime;

        var rawPosition = transform.localPosition.x + xOffsetThisFrame;
        var clampedPosition = Mathf.Clamp(rawPosition, -XRange, XRange);

        transform.localPosition = new Vector3(clampedPosition, transform.localPosition.y, transform.localPosition.z);

    }
    private void ProcessRotation()
    {
        var roll = XThrow * RollFactor; 
        transform.localRotation = Quaternion.Euler(0f, 0f, -roll);
    }
}
