// Jack Hamilton		- 100550931
// Daniel MacCormick	- 100580519
// Wen Bo Yu			- 100579309
// Rob Savaglio			- 100591436

// *****
// NeuralNet.cpp
// This file contains the implementation for most of the actual neural network logic and calculations
// The key functions is Evaluate() which performs the feedforward algorithm and calculations
// *****

#include "NeuralNet.h"

//--- Constructors and Destructor ---//
NeuralNet::NeuralNet()
{

}

NeuralNet::NeuralNet(std::vector<NeuralNet_Layer> _layers)
{
	// The number of connection groups is one less than the number of layers since the connections are BETWEEN the layers
	numConnectionSets = _layers.size() - 1;

	// Construct matrices for each of the connections between the layers
	for (int i = 0; i < numConnectionSets; i++)
	{
		// Create a weight matrix that is [rows = number of outgoing connections, cols = number of incoming connections]
		// Then, randomize the matrix since we want to init the neural nets to be random
		Eigen::MatrixXf newWeightMatrix = Eigen::MatrixXf(_layers[i + 1].numNodes, _layers[i].numNodes);
		newWeightMatrix = newWeightMatrix.setRandom();
		weightMatrices.push_back(newWeightMatrix);

		// Create a bias matrix that is [rows = number of incoming connections, 1]
		// Then randomize the matrix since we want to init the net to be random
		Eigen::MatrixXf newBiasMatrix = Eigen::MatrixXf(_layers[i + 1].numNodes, 1);
		newBiasMatrix = newBiasMatrix.setRandom();
		biasMatrices.push_back(newBiasMatrix);
	}
}

NeuralNet::~NeuralNet()
{

}



//--- Methods ---//
Eigen::VectorXf NeuralNet::Evaluate(Eigen::VectorXf _networkInputs)
{
	// Create a vector to hold the outputs from the final layer nodes
	Eigen::VectorXf networkOutputs;

	// This vector will store the values coming into each layer (ie: also the outputs from the previous layer)
	Eigen::VectorXf lastLayerOutputs = _networkInputs;

	// Loop through each of the connection sets and feed the values forward
	for (int i = 0; i < numConnectionSets; i++)
	{
		// Perform the feedforward algorithm (weighted sum of inputs + biases, all activated)
		Eigen::MatrixXf weightsTimesInputs = weightMatrices[i] * lastLayerOutputs;
		Eigen::MatrixXf resultPlusBiases = weightsTimesInputs + biasMatrices[i];
		Eigen::MatrixXf resultActivated = ApplyActivation(resultPlusBiases);

		// The next layer's inputs are this layer's outputs
		lastLayerOutputs = resultActivated;
	}

	// When there are no more layers to feed forward to into, we have found the final result
	networkOutputs = lastLayerOutputs;

	// Return the outputs
	return networkOutputs;
}

void NeuralNet::Save(std::string _path)
{

}

void NeuralNet::ApplyMutations(float _probability, float _maxOffsetAmount)
{
	// Loop through all of the weights and biases. Randomly decide to mutate or not
	for (int i = 0; i < numConnectionSets; i++)
	{
		// Roll a random number
		//float randomNumber = RandomInRange(0.0f, 1.0f);
		//
		//// If the random number falls within the probability, we are going to mutate this weight and bias matrix
		//if (randomNumber <= _probability)
		//{
			// Mutate both the weights and biases
			weightMatrices[i] = MutateMatrix(weightMatrices[i], _maxOffsetAmount, _probability);
			biasMatrices[i] = MutateMatrix(biasMatrices[i], _maxOffsetAmount, _probability);
		//}
	}
}

Eigen::MatrixXf NeuralNet::ApplyActivation(Eigen::MatrixXf _mat)
{
	// Loop through the matrix array and apply the activation function to all of its elements individually
	for (int row = 0; row < _mat.array().rows(); row++)
	{
		for (int col = 0; col < _mat.array().cols(); col++)
		{
			// This neural net implementation uses the sigmoid function
			// Each of the elements in the matrix is the X for the function and we are going to set them to the resulting Y
			_mat.array()(row, col) = Sigmoid(_mat.array()(row, col));
		}
	}

	// Return the "activated" matrix
	return _mat;
}



//--- Utility Functions ---//
float NeuralNet::Sigmoid(float _x)
{
	// Sigmoid function for the activation:
	// http://mathworld.wolfram.com/SigmoidFunction.html
	// exp(-x) function is the equivalent of e^(-x)
	return 1.0f / (1.0f + exp(-_x));
}

float NeuralNet::Mutate(float _x, float _maxOffsetAmount)
{
	// Mutate by as much as the offset threshold, either positive or negative
	float mutationAmount = 1 + RandomInRange(-_maxOffsetAmount, _maxOffsetAmount);

	// Apply the mutation to the value that was passed in and return it
	return _x *= mutationAmount;
}

// Note to Dan: You don't need to create an ArrayXf object here, there was a bug in the conversion. You can just call .Array() without losing any efficiencies
// I left in your code in case there is a future issue
Eigen::MatrixXf NeuralNet::MutateMatrix(Eigen::MatrixXf _mat, float _maxOffsetAmount, float _probability)
{
	// Loop through the matrix array and apply the mutation function to all of its elements individually
	for (int row = 0; row < _mat.array().rows(); row++)
	{
		float randomNumber;
		for (int col = 0; col < _mat.array().cols(); col++)
		{
			// Roll a number
			randomNumber = RandomInRange(0.0f, 1.0f);

			// If the random number falls within the probability, we are going to mutate this weight and bias matrix
			if (randomNumber <= _probability)
			{
				// Mutate the value by up to the given amount
				_mat.array()(row, col) = Mutate(_mat.array()(row, col), _maxOffsetAmount);
			}
		}
	}

	// Return the mutated matrix
	return _mat;
}

float NeuralNet::RandomInRange(float _min, float _max)
{
	// Return a random value in between the minimum and maximum
	// Based off of this post: https://stackoverflow.com/questions/686353/random-float-number-generation, specifically the top answer by John Dibling
	return _min + (float)(rand()) / (float)(RAND_MAX / (_max - _min));
}