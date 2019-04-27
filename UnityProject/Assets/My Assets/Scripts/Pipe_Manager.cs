// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// PipeManager.cs
// This script handles the spawning and deletion of the pipes
// The pipes are spawned at regular intervals and are only deleted when they go offscreen or when all the birds in the gen die off
// *****

using UnityEngine;
using System.Collections.Generic;

public class Pipe_Manager : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Spawning Controls")]
    public GameObject pipePrefab;
    public Transform spawnPosition;
    public Transform pipeListParent;
    public float spawnInterval;



    //--- Private Variables ---//
    private NN_InputManager nnInputManager;
    private List<Pipe> pipes;
    private float spawnTimer;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        nnInputManager = GameObject.FindObjectOfType<NN_InputManager>();
        pipes = new List<Pipe>();
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        // If enough time has passed, spawn a new pipe
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            // Spawn a new pipe
            SpawnPipe();

            // Reset the spawn timer
            spawnTimer = 0.0f;
        }
    }



    //--- Methods ---//
    public void SpawnPipe()
    {
        // Spawn the pipe just off the screen. Grab the reference to it and pass it this manager
        Pipe newPipe = Instantiate(pipePrefab, spawnPosition.position, Quaternion.identity, pipeListParent).GetComponent<Pipe>();
        newPipe.SetPipeManager(this);
        pipes.Add(newPipe);

        // If the NN input manager isn't currently tracking a pipe, give it this one
        if (nnInputManager.GetNearestPipe() == null)
            nnInputManager.SetNearestPipe(newPipe);
    }

    public void ClearPipes()
    {
        // Destroy all of the pipes currently in the list
        for (int i = pipes.Count - 1; i >= 0; i--)
            Destroy(pipes[i].gameObject);

        // Clear the list
        pipes.Clear();
    }

    public void ResetPipes()
    {
        // Clear all of the pipes
        ClearPipes();

        // Reset the spawn timer
        spawnTimer = spawnInterval;

        // Reset the tracked objects in the input manager
        nnInputManager.SetNearestPipe(null);
    }

    public void DeletePipe(Pipe _pipe)
    {
        // Remove the pipe from the list. Call when the pipe is being despawned
        pipes.Remove(_pipe);
    }

    public void PipePassed(Pipe _pipe)
    {
        // Get the index for the pipe that was just passed
        int pipeIndex = pipes.IndexOf(_pipe);
        int nextPipeIndex = pipeIndex + 1;

        // Tell the input manager to start caring about the next pipe in the list, if a pipe exists at that index
        if (nextPipeIndex < pipes.Count)
            nnInputManager.SetNearestPipe(pipes[nextPipeIndex]);
        else
            nnInputManager.SetNearestPipe(null);
    }
}
