// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// SelfDestruct.cs
// This script is attached to the parallax objects in the background
// They automatically get destroyed when they leave the bounds of the screen
// *****

using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    //--- Public Variables ---//
    public float destroyPosition;



    //--- Private Variables ---//
    private float timer;

    
    
    //--- Unity Functions ---//
    void Update()
    {
        // Destroy the gameobject when it moves off the screen
        if (this.transform.position.x < destroyPosition)
            Destroy(this.gameObject);
    }
}
