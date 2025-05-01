using UnityEngine;
using TMPro;

public class Line : MonoBehaviour
{
    public float distance = 1f;                   
    private float distTraveled = 0f;
    private Vector3 prevPosition;
    private int count = 0;

    public TextMeshProUGUI pointDisplay;          

    void Start()
    {
        prevPosition = transform.position;

        if (pointDisplay != null)
        {
            pointDisplay.text = "Points: 0";
        }
    }

    void FixedUpdate()
    {
        float deltaZ = transform.position.z - prevPosition.z;

        if (deltaZ > 0)
        {
            distTraveled += deltaZ;
        }
        else
        {
            distTraveled = 0;
        }

        if (distTraveled >= distance)
        {
            distTraveled = 0;
            count++;
            if (pointDisplay != null)
            {
                pointDisplay.text = "Points: " + count;
            }
        }

        print(distTraveled);

        prevPosition = transform.position;
    }
}
