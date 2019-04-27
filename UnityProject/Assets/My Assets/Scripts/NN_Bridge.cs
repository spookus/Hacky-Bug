// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// NNBridge.cs
// This script has all of the entry points and some helper functions for calling them
// This side of the bridge connects to all of the entry points in the main.cpp in the C++ DLL project
// *****

using System.Runtime.InteropServices;

public class NN_Bridge
{
    //--- Entry Points ---//
    [DllImport("NeuralNet", EntryPoint = "CheckNetsSize")]
    public static extern int CheckNetsSize();

    [DllImport("NeuralNet", EntryPoint = "SetFitnessesDll")]
    public static extern void SetFitnessesDll(float _fitness, int _index);

    [DllImport("NeuralNet", EntryPoint = "InitNetsDll")]
    public static extern void InitNetsDll(int _nnCount);

    [DllImport("NeuralNet", EntryPoint = "CreateNewGenerationDll")]
    public static extern void CreateNewGenerationDll();

    [DllImport("NeuralNet", EntryPoint = "EvaluateDll")]
    public static extern float EvaluateDll(int _nnIndex, float _gapx, float _gapy);



    //--- Helper Functions ---//
    // Calculates whether or not to flap based on inputs passed
    public static float Evaluate(int _nnIndex, float[] _inputs, int _numInputs)
    {
        // Pass the inputs to the DLL and see what it responds with
        float val = EvaluateDll(_nnIndex, _inputs[(int)NN_Inputs.DIST_GAP_X], _inputs[(int)NN_Inputs.DIST_GAP_Y]);
        
        // If the value is above 0.5, the bird is going flap. Otherwise, it will do nothing
        return (val >= 0.50f) ? 1.0f : 0.0f;
    }

    // Initializes the Nets container with random input values
    public static void InitNets(int _nnCount)
    {
        // Call initNets(int _nnCount) in the DLL to set up the list of neural networks on that side
        InitNetsDll(_nnCount);
    }

    // Creates a new generation of birds and replaces the Nets container with the selected children
    public static void CreateNewGeneration(float[] _birdScores)
    {
        // Call setFitnesses in the DLL and pass in the bird score along with it
        for (int i = 0; i < _birdScores.Length; i++)
        {
            SetFitnessesDll(_birdScores[i], i);
        }

        // Call createNewGeneration() in the DLL to actually perform the mutations and genetic algorithm
        CreateNewGenerationDll();
    }
}
