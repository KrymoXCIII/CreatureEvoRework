using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;

    private float[,] weightsInputHidden;
    private float[,] weightsHiddenOutput;

    // Constructeur pour initialiser la taille du r�seau de neurones et les poids al�atoirement
    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        // Initialiser les poids al�atoirement entre -1 et 1
        weightsInputHidden = new float[inputSize, hiddenSize];
        weightsHiddenOutput = new float[hiddenSize, outputSize];
        InitializeRandomWeights();
    }

    // Initialiser les poids al�atoirement
    private void InitializeRandomWeights()
    {
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weightsInputHidden[i, j] = Random.Range(-1f, 1f);
            }
        }

        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weightsHiddenOutput[i, j] = Random.Range(-1f, 1f);
            }
        }
    }

    // M�thode pour effectuer la propagation avant dans le r�seau de neurones
    public float[] FeedForward(float[] inputs)
    {
        float[] hiddenLayer = new float[hiddenSize];
        float[] outputLayer = new float[outputSize];

        // Calculer la couche cach�e
        for (int i = 0; i < hiddenSize; i++)
        {
            float sum = 0f;
            for (int j = 0; j < inputSize; j++)
            {
                sum += inputs[j] * weightsInputHidden[j, i];
            }
            hiddenLayer[i] = ActivationFunction(sum);
        }

        // Calculer la couche de sortie
        for (int i = 0; i < outputSize; i++)
        {
            float sum = 0f;
            for (int j = 0; j < hiddenSize; j++)
            {
                sum += hiddenLayer[j] * weightsHiddenOutput[j, i];
            }
            outputLayer[i] = ActivationFunction(sum);
        }

        return outputLayer;
    }

    // Fonction d'activation (vous pouvez utiliser une fonction diff�rente selon les besoins)
    private float ActivationFunction(float value)
    {
        return Mathf.Clamp(value, -1f, 1f);
    }

    // M�thode pour obtenir les poids du r�seau de neurones
    public float[] GetWeights()
    {
        List<float> weightsList = new List<float>();
        foreach (float weight in weightsInputHidden)
        {
            weightsList.Add(weight);
        }
        foreach (float weight in weightsHiddenOutput)
        {
            weightsList.Add(weight);
        }
        return weightsList.ToArray();
    }

    // M�thode pour d�finir les poids du r�seau de neurones
    public void SetWeights(float[] weights)
    {
        int count = 0;
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weightsInputHidden[i, j] = weights[count];
                count++;
            }
        }
        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weightsHiddenOutput[i, j] = weights[count];
                count++;
            }
        }
    }
}
