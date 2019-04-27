// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// BirdManager.cs
// This script manages all of the birds in the scene at a time
// It handles the inital setup of the neural nets and when the birds are all dead, it connects to the DLL to create the next generation.
// *****

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Bird_Manager : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Spawning Contols")]
    public GameObject birdPrefab;
    public int birdsPerGeneration;
    public Transform spawnLocation;

    [Header("UI")]
    public Text txtGenerationNumber;
    public Text txtGenerationTimer;
    public Text txtGenerationScore;
    public Text txtHighestScore;
    public Text txtGenCount;



    //--- Private Variables ---//
    private Pipe_Manager pipeManager;
    private NN_InputManager nnInputManager;
    private List<Bird> birds;
    public int generationNumber;
    public float highestScore = 0;
    private float generationTimer;
    private float generationScore;
    private int numBirdsLeft;



    //--- Unity Functions ---//
    private void Awake()
    {
        // Init the private variables
        pipeManager = GameObject.FindObjectOfType<Pipe_Manager>();
        nnInputManager = GameObject.FindObjectOfType<NN_InputManager>();
        generationNumber = 0;
        generationTimer = 0.0f;

        // Construct the neural networks on the C++ side
        NN_Bridge.InitNets(birdsPerGeneration);

        // Now, create all of the birds and tell them which network it is going to 'listen' to
        birds = new List<Bird>();
        CreateBirds();
    }

    private void Update()
    {
        // Update the duration of this generation
        generationTimer += Time.deltaTime;

        // Determine the generation's current score
        DetermineGenScrore();

        // Update the UI to represent the current state of the system
        UpdateUI();
    }



    //--- Methods ---//
    public void CreateBirds()
    {
        // Clear all of the existing birds
        foreach (Bird bird in birds)
        {
            // Some of the birds might have been destroyed with the pipes that they stuck to so we need to check if they aren't already null
            if (bird != null)
                Destroy(bird.gameObject);
        }

        // Reset the bird list
        birds = new List<Bird>();

        // Instantiate all of the birds and temporarily keep track of their controlling classes
        for (int i = 0; i < birdsPerGeneration; i++)
        {
            // Grab the bird component from the newly spawned object and pass it its corresponding neural net ID
            Bird newBird = Instantiate(birdPrefab, spawnLocation.position, Quaternion.identity, null).GetComponent<Bird>();
            newBird.SetNNIndex(i);

            // Also, pass the bird the pipe manager and the input manager so it doesn't have to find them
            newBird.SetPipeManager(pipeManager);
            newBird.SetNNInputManager(nnInputManager);
            newBird.SetBirdManager(this);

            // Add the bird into the list
            birds.Add(newBird);
        }

        // Store the number of birds left
        numBirdsLeft = birds.Count;
    }

    public void DetermineGenScrore()
    {
        // Loop through all of the birds and get the best score
        for (int i = 0; i < birds.Count; i++)
        {
            // Update the generation score while we're in this loop
            if (birds[i].GetScore() > generationScore)
                generationScore = birds[i].GetScore();
        }
    }

    public float[] GetAllBirdScores()
    {
        // Create the int array to hold the scores
        float[] birdScores = new float[birds.Count];

        // Loop through the birds and store their scores in the array
        for (int i = 0; i < birds.Count; i++)
        {
            birdScores[i] = birds[i].GetScore();
            if (birdScores[i] > highestScore)
                highestScore = birdScores[i];
        }

        // Return the list of scores
        return birdScores;
    }

    public void UpdateUI()
    {
        // Update the UI fields at the bottom of the screen
        txtGenerationNumber.text = "Gen #" + generationNumber.ToString();
        txtGenerationTimer.text = ToFormattedMinuteString("Time", generationTimer);
        txtGenerationScore.text = ToFormattedScoreString("Score", (int)generationScore);
        txtHighestScore.text = ToFormattedScoreString("High Score", (int)highestScore);
        txtGenCount.text = ToFormattedScoreString("Bugs", numBirdsLeft);
    }

    public void HandleBirdKilled()
    {
        // Decrease the number of birds alive
        numBirdsLeft--;

        // If all of the birds are dead, reset the generation
        if (numBirdsLeft <= 0)
        {
            // Collect the final scores for all of the birds in this generation
            float[] birdScores = GetAllBirdScores();

            // Create the next generation of neural nets
            NN_Bridge.CreateNewGeneration(birdScores);

            // Increase the generation number
            generationNumber++;

            // Clear all of the pipes from the scene
            pipeManager.ResetPipes();

            // Reset the duration of this generation
            generationTimer = 0.0f;

            // Reset the score of this generation
            generationScore = 0.0f;

            // Respawn all of the birds
            CreateBirds();
        }
    }

    public void CheckForHighestScore(float _score)
    {
        // Update the high score if a new score is better
        if (_score > highestScore)
            highestScore = _score;
    }



    //--- Utility Functions ---//
    private string ToFormattedMinuteString(string _label, float _time)
    {
        // Calculate the number of minutes and seconds
        int numMinutes = (int)_time / 60;
        int numSeconds = (int)_time % 60;

        // This will hold the finalized string
        string timeStr = _label + " - ";

        // Convert the number of minutes into a string and add an additional 0 if need be
        timeStr += (numMinutes < 10) ? ("0" + numMinutes.ToString()) : numMinutes.ToString();

        // Add the colon separator
        timeStr += ":";

        // Do the same formatting for the seconds as well
        timeStr += (numSeconds < 10) ? ("0" + numSeconds.ToString()) : numSeconds.ToString();

        // Return the final formatted string
        return timeStr;
    }

    private string ToFormattedScoreString(string _label, int _score)
    {
        // This will store the string
        string scoreStr = _label + " - ";

        // Add as many zeroes as need be
        if (_score < 10000)
            scoreStr += "0";
        if (_score < 1000)
            scoreStr += "0";
        if (_score < 100)
            scoreStr += "0";
        if (_score < 10)
            scoreStr += "0";

        // Add the score string itself
        scoreStr += _score.ToString();

        // Return the score string
        return scoreStr;
    }
}
