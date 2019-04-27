// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// NN_InputMangaer.cs
// This script handles all of the calculations for the inputs to the neural network
// It normalizes the inputs before they are sent to the neural nets
// *****

using UnityEngine;

// The list of inputs going into the neural network, named with enum for better understanding
public enum NN_Inputs
{
    DIST_GAP_X,
    DIST_GAP_Y,

    COUNT
}

public class NN_InputManager : MonoBehaviour
{
    //--- Private Variables ---//
    private Pipe nearestPipe;
    private Camera mainCam;



    //--- Unity Functions ---//
    private void Start()
    {
        // Init the private variables
        nearestPipe = null;
        mainCam = Camera.main;
    }



    //--- Methods ---//
    public float[] CalculateNNInputs(Bird _bird)
    {
        // Init the input array
        float[] inputs = new float[(int)NN_Inputs.COUNT];

        // Calculate each of the inputs relating to the pipe. Need to normalize all of them into a range of [-1, 1]
        if (nearestPipe != null)
        {
            // Find the nearest pipe in the scene and calculate the X and Y distances to the center of its gap. Normalize them so they are on a better range for the neural net to work with
            inputs[(int)NN_Inputs.DIST_GAP_X] =  (ToScreenPosition(nearestPipe.transform.position).x - ToScreenPosition(_bird.transform.position).x) / Screen.width;
            inputs[(int)NN_Inputs.DIST_GAP_X] = 1.0f - inputs[(int)NN_Inputs.DIST_GAP_X];   // Reverse the X so that its at 1 when the bird is right at it, because it should be most important then
            inputs[(int)NN_Inputs.DIST_GAP_Y] = (ToScreenPosition(nearestPipe.GetGapPosition()).y - ToScreenPosition(_bird.transform.position).y) / Screen.height;
        }
        else
        {
            // If no pipe exists, set all of the relevant values to -1
            inputs[(int)NN_Inputs.DIST_GAP_X] = -1.0f;
            inputs[(int)NN_Inputs.DIST_GAP_Y] = -1.0f;
        }

        // Return the input array
        return inputs;
    }



    //--- Utility Functions ---//
    private Vector3 ToScreenPosition(Vector3 _position) {
        return mainCam.WorldToScreenPoint(_position);
    }



    //--- Setters ---//
    public void SetNearestPipe(Pipe _pipe) { nearestPipe = _pipe; }



    //--- Getters ---//
    public Pipe GetNearestPipe() { return nearestPipe; }
}
