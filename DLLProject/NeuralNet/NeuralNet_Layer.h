#ifndef NEURALNETLAYER_H
#define NEURALNETLAYER_H

#include <Eigen/Core>
#include <Eigen/Dense>
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
