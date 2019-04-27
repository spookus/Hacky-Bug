// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// Bird.cs
// This script controls the bird objects themselves
// Each bird is connected to its own neural net. It will continue to fly until it runs into a pipe and dies
// *****

using UnityEngine;

public class Bird : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Control Parameters")]
    public float flapStrength;

    [Header("Score Parameters")]
    public int timeScoreIncrement;
    public float timeScoreInterval;

    [Header("Sprite Rendering")]
    public Sprite defaultSprite;
    public Sprite flapSprite;
    public Sprite deathSprite;
    public SpriteRenderer fillSpriteRenderer;



    //--- Private Variables ---//
    private Rigidbody2D rb;
    private Pipe_Manager pipeManager;
    private NN_InputManager nnInputManager;
    private Bird_Manager birdManager;
    private float timeSinceLastScore;
    private float[] nnInputs;
    private bool isAlive;
    private float score;
    private int nnIndex;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        rb = GetComponent<Rigidbody2D>();
        timeSinceLastScore = 0.0f;
        nnInputs = new float[(int)NN_Inputs.COUNT];
        isAlive = true;
        score = 0;

        // Reset to Default Sprite
        this.GetComponent<SpriteRenderer>().sprite = defaultSprite;

        // Random Colour
        fillSpriteRenderer.color = Random.ColorHSV();
    }

    private void Update()
    {
        // Only allow for input and score gathering if the bird is alive
        if (isAlive)
        {
            // Increase this bird's score if enough time has passed
            CheckForScoreIncrease();

            // Calculate the values of the inputs that we need to pass to the neural net
            CalculateNNInputs();

            // Ask the neural net to make a decision. It returns a float from 0 - 1
            float decisionEval = NN_Bridge.Evaluate(nnIndex, nnInputs, (int)NN_Inputs.COUNT);

            // Flap if the neural net returned a value above 0.5
            if (decisionEval >= 0.5f)
                FlapWings();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle the different trigger collision
        if (collision.tag == "Pipe")
        {
            // Kill bird when it hits the pipe
            KillBird();

            // Stop the velocity and let it fall straight down
            this.transform.parent = collision.transform;

            // Changes the breakpoints to be happy
            if (collision.GetComponent<BreakpointSpriteManager>() != null)
            {
                collision.GetComponent<BreakpointSpriteManager>().SetToHappySprite();
            }
        }
        else if (collision.tag == "Bounds")
        {
            // Decrease score in addition to killing the bird (decentivise floor and ceiling)
            IncreaseScore(-10);
            KillBird();
        }
        else if (collision.tag == "PassedPipe")
        {
			// Give more score for passing a pipe to encourage good play
            IncreaseScore(10);
			
            // Tell the pipe manager that the birds should start caring about the next pipe
            pipeManager.PipePassed(collision.gameObject.GetComponentInParent<Pipe>());

            // Update the breakpoints to be sad :(
            collision.gameObject.GetComponentInParent<Pipe>().SetSadBreakpoints();
        }
    }

    private void OnDestroy()
    {
        // Tell the bird manager to check if this bird's score is the new high score
        birdManager.CheckForHighestScore(this.score);
    }



    //--- Methods ---//
    public void FlapWings()
    {
        // Reset the velocity
        rb.velocity = Vector2.zero;

        // Add the force upwards
        rb.AddForce(Vector2.up * flapStrength);

        this.GetComponent<SpriteRenderer>().sprite = flapSprite;
        transform.Rotate(0,0, 10);
        Invoke("ResetSprite", 0.1f);
    }

    public void ResetSprite()
    {
        this.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        transform.Rotate(0,0,-10);
    }

    public void IncreaseScore(float _amount)
    {
        // Increment the score
        score += _amount;
    }

    public void KillBird()
    {
        // If already killed, just return
        if (!isAlive)
            return;

        // No longer alive
        isAlive = false;
        CalculateNNInputs(); 

        // Subtract the distance from the center of the gap so we can incentivise going towards them more
        IncreaseScore(10.0f * -(1.0f - Mathf.Abs(nnInputs[(int)NN_Inputs.DIST_GAP_X])));
        IncreaseScore(10.0f * -Mathf.Abs(nnInputs[(int)NN_Inputs.DIST_GAP_Y]));

        // Tell the bird manager this bird is dead
        birdManager.HandleBirdKilled();

        // Change to death sprite [Rob]
        this.GetComponent<SpriteRenderer>().sprite = deathSprite;
    }

    public void CalculateNNInputs()
    {
        // Ask the NN input manager to calculate the inputs for this bird
        nnInputs = nnInputManager.CalculateNNInputs(this);
    }

    public void CheckForScoreIncrease()
    {
        // Increase the time since the last score increased occurred
        timeSinceLastScore += Time.deltaTime;

        // If enough time has passed, increase the score and reset the timer
        if (timeSinceLastScore >= timeScoreInterval)
        {
            IncreaseScore(timeScoreIncrement);
            timeSinceLastScore = 0.0f;
        }
    }



    //--- Setters ---//
    public void SetIsAlive(bool _isAlive) { this.isAlive = _isAlive; }
    public void SetScore(int _score) { this.score = _score; }
    public void SetNNIndex(int _index) { this.nnIndex = _index; }
    public void SetPipeManager(Pipe_Manager _pipeManager) { this.pipeManager = _pipeManager; }
    public void SetNNInputManager(NN_InputManager _nnInputManager) { this.nnInputManager = _nnInputManager; }
    public void SetBirdManager(Bird_Manager _birdManager) { this.birdManager = _birdManager; }



    //--- Getters ---//
    public bool GetIsAlive() { return isAlive; }
    public float GetScore() { return score; }
    public int GetNNIndex() { return nnIndex; }
}
