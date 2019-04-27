// Jack Hamilton		- 100550931
// Daniel MacCormick	- 100580519
// Wen Bo Yu			- 100579309
// Rob Savaglio			- 100591436

// *****
// Main.cpp
// This file is going to be the DLL bridge on the C++ side that connects to the corresponding file in Unity
// It contains all of the entry points and much of the logic for handling the lists of neural networks
// *****

#include "NeuralNet.h"
#include <iostream>
#include <vector>
#include <time.h>

// Used for connecting with unity
#define NN_DLL __declspec(dllexport)
extern "C"
{
	NN_DLL int CheckNetsSize();
	NN_DLL void SetFitnessesDll(float _fitness, int _index);
	NN_DLL void CreateNewGenerationDll();
	NN_DLL void InitNetsDll(int _nnCount);
	NN_DLL float EvaluateDll(int _nnIndex, float _gapx, float _gapy);
}

// Object for containing a fitness and its corresponding position in the Neural Net
struct fitObj
{
	int index;
	float fitness;

	std::string fitString()
	{
		return std::to_string(index) + " : " + std::to_string(fitness) + ", ";
	}
};

// Used for sorting Fitnesses
bool compareFits(fitObj left, fitObj right)
{
	return left.fitness > right.fitness;
}

// Containers
std::vector<NeuralNet> nets;
std::vector<NeuralNet> tempNets;
std::vector<fitObj> fitnesses;

// Returns a randomly intialized Neural Net
NeuralNet randNet()
{
	// Construct a neural net
	std::vector<NeuralNet_Layer> netLayers = std::vector<NeuralNet_Layer>();
	netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::INPUT, 2));
	netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 6));
	netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::HIDDEN, 6));
	netLayers.push_back(NeuralNet_Layer(NeuralNet_LayerType::OUTPUT, 1));
	NeuralNet nn = NeuralNet(netLayers);

	return nn;
}

// Clears nets
void clearNets()
{
	nets.clear();
}

// Interbreeds two Neural Nets
void crossover(NeuralNet _a, NeuralNet _b)
{
	NeuralNet temp = _a;

	_a.biasMatrices = _b.biasMatrices;
	_b.biasMatrices = temp.biasMatrices;

	if (rand() % 2 == 1)
		nets.push_back(_a);
	else
		nets.push_back(_b);
}

extern "C"
{
	// Used to output Neural Network container size to unity (debugging)
	int CheckNetsSize()
	{
		return nets.size();
	}

	// Sets all of the fitnesses on the c++ side
	void SetFitnessesDll(float _fitness, int _index)
	{
		fitObj tempFit;
		tempFit.fitness = _fitness;
		tempFit.index = _index;
		fitnesses.push_back(tempFit);
	}

	// Creates the new generation of neural nets
	void CreateNewGenerationDll()
	{
		tempNets = nets;

		// Clear the nets
		nets.clear();

		// Find winners
		std::sort(fitnesses.begin(), fitnesses.end(), compareFits);

		// Determine what bugs to breed
		int top30 = 0.3f * tempNets.size();
		int crossMutate = 0.2f * tempNets.size();


		// Breed the bugs
		for (int i = 0; i < tempNets.size(); i++)
		{
			// Take the top 30 bugs and pass them into the new neural net
			if (i < top30)
			{
				nets.push_back(tempNets[fitnesses[i].index]); 
			}
			else if (i == top30)
			{
				crossover(tempNets[fitnesses[i - top30].index], tempNets[fitnesses[i - top30 + 1].index]);
			}
			// Take the top 20 bugs and crossover them, then pass both children into the new neural net container
			else if (i > top30 && i < (top30 + crossMutate))
			{
				//nets.push_back(tempNets[fitnesses[i - top30].index]);
				//nets[i].ApplyMutations(1.0f, 0.05f);
				crossover(tempNets[fitnesses[i - top30].index], tempNets[fitnesses[i - top30 + 1].index]);
			}
			// Take the top 20 bugs and pass them into the neural net container, and then apply mutations while in the container
			else if (i >= (top30 + crossMutate) && i < (top30 + crossMutate * 2))
			{
				nets.push_back(tempNets[fitnesses[i - (top30 + crossMutate)].index]);
				nets.push_back(tempNets[fitnesses[i - (top30 + crossMutate) + 1].index]);
				nets[i].ApplyMutations(0.2f, 0.05f);
				nets[i + 1].ApplyMutations(0.2f, 0.05f);
				i++;
			}
			// Take the top 20 bugs and crossover them, pass them into the neural net container, and then apply mutations while in the container
			else if (i >= (top30 + crossMutate * 2) && i < (top30 + crossMutate * 3))
			{
				crossover(tempNets[fitnesses[i - (top30 + crossMutate * 2)].index], tempNets[fitnesses[i - (top30 + crossMutate * 2) + 1].index]);
				nets[i].ApplyMutations(0.2f, 0.05f);
			}
			else
			{
				nets.push_back(randNet());
			}
		}

		// Clear the fitnesses for next time
		fitnesses.clear();
	}

	// Initialize Neural Nets
	void InitNetsDll(int _nnCount)
	{
		// Clear DLL memory
		clearNets();
		fitnesses.clear();

		// Set random seed
		srand(time(NULL));

		// Construct neural net container
		for (int i = 0; i < _nnCount; i++)
		{
			// Construct a randomly initialized Neural Net
			nets.push_back(randNet());
		}
	}

	// Evaluates whether or not to flap
	float EvaluateDll(int _nnIndex, float _gapx, float _gapy)
	{
		Eigen::VectorXf _inputs(2);
		_inputs << _gapx, _gapy;
		Eigen::VectorXf result(1);
		result << nets[_nnIndex].Evaluate(_inputs);
		return result[0];
	}
}