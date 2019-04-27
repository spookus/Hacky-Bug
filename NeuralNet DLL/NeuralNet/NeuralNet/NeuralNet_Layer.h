// Jack Hamilton		- 100550931
// Daniel MacCormick	- 100580519
// Wen Bo Yu			- 100579309
// Rob Savaglio			- 100591436

// *****
// NeuralNet_Layer.h
// This file contains the layer structure which is simply a way to easily represent one of the layers in the neural network
// This structure is really only going to be applied when initializing the neural network
// *****

#ifndef NEURALNETLAYER_H
#define NEURALNETLAYER_H

#include "Eigen/Core"
#include "Eigen/Dense"
#include "NeuralNet_LayerType.h"

struct NeuralNet_Layer
{
public:
	//--- Constructors and Destructor ---//
	NeuralNet_Layer(NeuralNet_LayerType _layerType, int _numNodes)
	{
		this->layerType = _layerType;
		this->numNodes = _numNodes;
	}

	//--- Data ---//
	NeuralNet_LayerType layerType;
	int numNodes;
};

#endif
