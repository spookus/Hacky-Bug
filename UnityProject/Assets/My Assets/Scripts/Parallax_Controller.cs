// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// Parallax_Controller.cs
// This script handles all of the parallax scrolling background elements
// The parallax objects appear on the right side of the screen and slowly drift to the right before being destroyed
// *****

using UnityEngine;

public class Parallax_Controller : MonoBehaviour
{
    //--- Public Variables ---//
    public GameObject[] Objects;
    public float[] SpawnTimer;
    public float[] Speed;
    public Vector3[] InitPosition;
    public Vector3[] SpawnPosition;
    public float[] DespawnPositionX;



    //--- Private Variables ---//
    private GameObject objClone;
    private float[] timer;
    


    //--- Unity Functions ---//
    void Start()
    {
        //initalize timer and initial object locations
        timer = new float[] { 0, 0, 0, 0 };

        for (int i = 0; i < Objects.Length; i++)
        {
            objClone = Instantiate(Objects[i]);
            objClone.transform.position = InitPosition[i];
            objClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Speed[i], 0));
            objClone.GetComponent<SelfDestruct>().destroyPosition = DespawnPositionX[i];
        }
    }

    void Update()
    {
        //spawn object based on their spawn timer
        for (int i = 0; i < Objects.Length; i++)
        {
            //increment time
            timer[i] += Time.deltaTime;

            if (timer[i] > SpawnTimer[i])
            {
                objClone = Instantiate(Objects[i]);
                objClone.transform.position = SpawnPosition[i];
                objClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Speed[i], 0));
                objClone.GetComponent<SelfDestruct>().destroyPosition = DespawnPositionX[i];
                timer[i] = 0;
            }
        }
    }
}
