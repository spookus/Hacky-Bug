// Jack Hamilton		- 100550931
// Daniel MacCormick	- 100580519
// Wen Bo Yu			- 100579309
// Rob Savaglio			- 100591436

// *****
// NeuralNet.h
// This file contains the NeuralNet class
// It defines all of the functions needed for the NeuralNet but they are actually implemented in NeuralNet.cpp
// *****

#ifndef NEURALNET_H
#define NEURALNET_H

#include <string>
#include <vector>
#include "NeuralNet_Layer.h"
#include "Eigen/Core"
#include "Eigen/Dense"


class NeuralNet
{
public:
	//--- Constructors and Destructor ---//
	NeuralNet();
	NeuralNet(std::vector<NeuralNet_Layer> _layers);
	~NeuralNet();

	//--- Methods ---//
	Eigen::VectorXf Evaluate(Eigen::VectorXf _networkInputs);
	void Save(std::string _filePath);
	void ApplyMutations(float _probability, float _maxOffsetAmount);
	Eigen::MatrixXf ApplyActivation(Eigen::MatrixXf _mat);

	//--- Utility Functions ---//
	float Sigmoid(float _x);
	Eigen::MatrixXf MutateMatrix(Eigen::MatrixXf _mat, float _maxOffsetAmount, float _probability);
	float Mutate(float _x, float _maxOffsetAmount);
	float RandomInRange(float _min, float _max);
	
	std::vector<Eigen::VectorXf> biasMatrices;			// Each set of connections biases are stored as a column vector

private:
	//--- Data ---//
	std::vector<Eigen::MatrixXf> weightMatrices;		// Each set of connection weights are stored as a matrix
	int numConnectionSets;								// The length of the above two vectors, basically how many "layers" of connections there are between the node layers
};

#endif
