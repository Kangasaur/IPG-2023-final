using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSword : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2[] lookVectors;
    private Vector2 averageLookVector = Vector2.zero;
    Vector3 lookDirection = Vector3.up;

    [SerializeField] float returnSpeed = 1f;
    [SerializeField] float swordSensitivity = 60f;
    //[SerializeField] float swordMoveSensitivity = 0.0006f;
    [SerializeField] int swordWeight = 15;
    [SerializeField] GameObject lookSphere;
    [SerializeField] Transform swordPoint;
    [SerializeField] ControlCamera cam;

    bool isReturning;
    Quaternion startRotation;
    Quaternion returnTarget;
    float returnTimer = 0;

    void Start()
    {
        lookVectors = new Vector2[swordWeight];
        for (int i = 0; i < swordWeight; i++) lookVectors[i] = Vector2.zero;
        returnTarget = lookSphere.transform.rotation;
    }



    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currLookVec = new Vector2(Input.GetAxis("Mouse X") * swordSensitivity, Input.GetAxis("Mouse Y") * -swordSensitivity);
            for (int i = 0; i < lookVectors.Length - 1; i++)
            {
                lookVectors[i] = lookVectors[i + 1];
                averageLookVector += lookVectors[i];
            }
            lookVectors[lookVectors.Length - 1] = currLookVec;
            averageLookVector += currLookVec;
            averageLookVector /= swordWeight;

            lookSphere.transform.Rotate(new Vector3(averageLookVector.y, averageLookVector.x, 0f));
            cam.usingSword = true;
        }
        else
        {
            Vector2 currLookVec = Vector2.zero;
            for (int i = 0; i < lookVectors.Length - 1; i++)
            {
                lookVectors[i] = lookVectors[i + 1];
                averageLookVector += lookVectors[i];
            }
            lookVectors[lookVectors.Length - 1] = currLookVec;
            averageLookVector += currLookVec;
            averageLookVector /= swordWeight;

            lookSphere.transform.Rotate(new Vector3(averageLookVector.y, averageLookVector.x, 0f));
            cam.usingSword = false;
        }
        Vector3 a = new Vector3(Mathf.Abs(averageLookVector.y), Mathf.Abs(averageLookVector.x), 0f).normalized;
        if (a == Vector3.zero)
        {
            if (!isReturning) StartReturn();
            returnTimer = Mathf.Min(1, returnTimer + Time.deltaTime * returnSpeed);
            lookSphere.transform.localRotation = Quaternion.Slerp(startRotation, returnTarget, EaseInOut(returnTimer));
        }
        else
        {
            isReturning = false;
        }
        transform.LookAt(swordPoint, lookDirection);
    }

    void StartReturn()
    {
        startRotation = lookSphere.transform.localRotation;
        returnTimer = 0;
        isReturning = true;
    }

    //Simple easing functions from https://www.febucci.com/2018/08/easing-functions/

    float Flip(float x)
    {
        return 1 - x;
    }
    float EaseIn(float t)
    {
        return Mathf.Pow(t, 2f);
    }
    float EaseOut(float t)
    {
        return Flip(EaseIn(Flip(t)));
    }
    float EaseInOut(float t)
    {
        return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(averageLookVector.magnitude);
        }
    }
}
