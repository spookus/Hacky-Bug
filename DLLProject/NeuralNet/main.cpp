#include "NeuralNet.h"
#include <iostream>
#include <vector>
#include <time.h>

#define NN_DLL __declspec(dllexport)

std::vector<NeuralNet> nets;
std::vector<NeuralNet> tempNets;

const int numBirds = 100;

float fitnesses[numBirds];
float birdScores[numBirds];

// Sets all of the fitnesses on the c++ side
void setFitnesses(float _fitness, int _index)
{
	fitnesses[_index] += _fitness;
}

// Creates the new generation of neural nets
void createNewGeneration()
{
	tempNets = nets;
	float selectedFitness;


	for (int i = 0; i < tempNets.size(); i++) // Get the main net container ready for new nets
	{
		nets.pop_back();
	}

	// Natural Selection
	for (int i = 0; i < tempNets.size(); i++) // Repopulates the number of nets in net, based on the previous size
	{
		selectedFitness = (float)rand() / RAND_MAX;
		for (int j = 0; j < numBirds; j++) // Iterates through potential fitnesses
		{
			if (selectedFitness <= fitnesses[j]) // Selects the fitness
			{
				nets.push_back(tempNets[j]); 
				break;
			}
		}
	}

	// Mutate like x-men
	for (int i = 0; i < nets.size(); i++)
	{
		nets[i].ApplyMutations(0.1, 0.1);
	}
}

void initNets(int _nnCount)
{
	for (int i = 0; i < _nnCount; i++)
	{
		// Construct a neural net
		std::vector<NeuralNet_Layer> netLayers = std::vector<NeuralNet_Layer>();
		netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::INPUT, 5));
		netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 3));
		netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 2));
		netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::OUTPUT, 1));
		NeuralNet nn = NeuralNet(netLayers);

		// Add to nets container
		nets.push_back(netLayers);
	}
}

float EvaluateDll(int _nnIndex, float _gapx, float _gap1y, float _gap2y, float _coinx, float _coiny)
{
	Eigen::VectorXf _inputs(5);
	_inputs << _gapx, _gap1y, _gap2y, _coinx, _coiny;
	Eigen::VectorXf result(1);
	result << nets[_nnIndex].Evaluate(_inputs);
	return result[0];
}

// Test Evaluations en Masse
void testEval()
{
	Eigen::VectorXf netInputs = Eigen::VectorXf();
	float netOutputs = 0;

	for (int i = 0; i < nets.size(); i++)
	{
		netInputs = netInputs.setRandom(5);
		netOutputs = EvaluateDll(i, netInputs[0], netInputs[1], netInputs[2], netInputs[3], netInputs[4]);
	}
	std::cout << netOutputs << std::endl;
}

// Test the set finesses en masse
void testSetFitnesses()
{
	float totalScore = 0;

	for (int i = 0; i < 100; i++)
	{
		birdScores[i] += rand() % 100;
		totalScore += birdScores[i];
	}
	for (int i = 0; i < 100; i++)
	{
		// Fitnesses are calculated like this in order to better apply a rand, this way there are ranges from fitness[i-1] to fitness[i]
		fitnesses[i] = fitnesses[i - 1] + (birdScores[i] / totalScore);
		if (i == 0)
		{
			fitnesses[i] = birdScores[i] / totalScore;
		}
	}
}


int main()
{
	std::string stop = "n";
	// Seed random
	srand(time(0));

	// A simple testing loop
	initNets(100);

	while (stop == "n")
	{
		testEval();
		testSetFitnesses();
		createNewGeneration();
		std::cin >> stop;
		std::cout << "\n";
	}

	// Construct a neural net
	//std::vector<NeuralNet_Layer> netLayers = std::vector<NeuralNet_Layer>();
	//netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::INPUT, 5));
	//netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 3));
	//netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 2));
	//netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::OUTPUT, 1));
	//NeuralNet nn = NeuralNet(netLayers);

	// Create a list of inputs
	//Eigen::VectorXf netInputs = Eigen::VectorXf();
	//netInputs = netInputs.setRandom(5);

	// Output the nn's guesses
	//Eigen::VectorXf netOutputs = nn.Evaluate(netInputs);
	//std::cout << netOutputs << std::endl;

	// Success
	system("pause");
	return 0;
}