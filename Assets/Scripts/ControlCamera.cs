using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A first person camera control script in C#/Unity
public class ControlCamera : MonoBehaviour
{
    [SerializeField] float lookSpeed = 0.5f;
    [SerializeField] float swordLookMultiplier = 0.25f;
    [SerializeField] float swordLookYMultiplier = 0.5f;
    [SerializeField] int smoothAmount = 8;
    Vector2[] lookVectors;
    Vector2 currLookVec = Vector2.zero;
    private Vector2 averageLookVector = Vector2.zero;
    public bool usingSword = false;

    //Start is called before the first frame update
    void Start()
    {
        lookVectors = new Vector2[smoothAmount];
        for (int i = 0; i < smoothAmount; i++) lookVectors[i] = Vector2.zero;
    }

    void Update()
    {
        averageLookVector = Vector2.zero;
        Vector2 currLookVec = new Vector2(Input.GetAxis("Mouse X") * lookSpeed, Input.GetAxis("Mouse Y") * -lookSpeed);
        if (usingSword)
        {
            currLookVec *= swordLookMultiplier;
            currLookVec.y *= swordLookYMultiplier;
        }

        //Smooths camera rotation by averaging the raw mouse delta out over a number of frames
        for (int i = 0; i < lookVectors.Length-1; i++)
        {
            lookVectors[i] = lookVectors[i + 1];
            averageLookVector += lookVectors[i];
        }
        lookVectors[lookVectors.Length-1] = currLookVec;
        averageLookVector += currLookVec;
        averageLookVector /= smoothAmount;
        
        
        transform.parent.Rotate(new Vector3(0f, averageLookVector.x, 0f)); //Rotate the camera's parent (the player game object) on the horizontal axis to fit with WASD controls
        transform.Rotate(new Vector3(averageLookVector.y, 0f, 0f));

        transform.localRotation = Quaternion.Euler(ClampAngle(transform.rotation.eulerAngles.x, -80f, 80f), 0f, 0f); //Clamp vertical look rotation
    }

    /// <summary> Clamps an angle between a minimum and maximum value. </summary>
    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0f) angle += 360f;
        if (angle < 180f) return Mathf.Min(angle, min + 360f);
        else return Mathf.Max(angle, max);
    }
}
