// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// Pipe.cs
// This script is attached to the pipe objects that are the obstacles for the birds to get through
// It randomly creates a gap when spawned
// *****

using UnityEngine;

public class Pipe : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Spawning")]
    public GameObject[] pipeSections;

    [Header("Movement")]
    public float movementSpeed;
    public float destroyPositionX;



    //--- Private Variables ---//
    private Pipe_Manager pipeManager;
    private int firstGapIndex;
    private int secondGapIndex;
    


    //--- Unity Functions ---//
    private void Start()
    {
        // Init the gap in the pipe
        InitPipe();
    }

    private void Update()
    {
        // Slowly move from the right to the left of the screen
        this.transform.position += (Vector3.left * movementSpeed * Time.deltaTime);

        // If now off-screen, destroy this pipe
        if (this.transform.position.x <= destroyPositionX)
            DestroyPipe();
    }



    //--- Methods ---//
    public void InitPipe()
    {
        // Pick two section in a in a row to delete and create a gap
        firstGapIndex = Random.Range(0, pipeSections.Length - 1);
        secondGapIndex = firstGapIndex + 1;

        // Disable the two randomly selected sections to make the gap
        pipeSections[firstGapIndex].SetActive(false);
        pipeSections[secondGapIndex].SetActive(false);
    }

    public void DestroyPipe()
    {
        // Tell the pipe manager to remove this pipe from the list
        pipeManager.DeletePipe(this);

        // Now actually remove the gameobject from the scene
        Destroy(this.gameObject);
    }

    public void SetSadBreakpoints()
    {
        // When a bug passes through this pipe, all of its animated breakpoints should be sad
        for (int i = 0; i < pipeSections.Length; i++)
        {
            pipeSections[i].GetComponent<BreakpointSpriteManager>().SetToSadSprite();
        }
    }


    //--- Setters ---//
    public void SetPipeManager(Pipe_Manager _manager) {
        this.pipeManager = _manager;
    }



    //--- Getters ---//
    public Vector3 GetGapPosition()
    {
        // Return the average of the two gaps (which is the middle of the large gap)
        return (pipeSections[firstGapIndex].transform.position + pipeSections[secondGapIndex].transform.position) * 0.5f;
    }
}
