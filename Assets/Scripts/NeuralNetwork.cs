using UnityEngine;

public class NeuralNetwork 
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;

    private float[,] weightsInputHidden;
    private float[,] weightsHiddenOutput;

    public float fitness;

    // Constructeur pour initialiser la taille du réseau de neurones et les poids aléatoirement
    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        // Initialiser les poids aléatoirement entre -1 et 1
        weightsInputHidden = new float[inputSize, hiddenSize];
        weightsHiddenOutput = new float[hiddenSize, outputSize];
        InitializeRandomWeights();
    }

    // Initialiser les poids aléatoirement
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

    // Méthode pour effectuer la propagation avant dans le réseau de neurones
    public float[] FeedForward(float[] inputs)
    {
        float[] hiddenLayer = new float[hiddenSize];
        float[] outputLayer = new float[outputSize];

        // Calculer la couche cachée
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

    // Fonction d'activation (vous pouvez utiliser une fonction différente selon les besoins)
    private float ActivationFunction(float value)
    {
        return Mathf.Clamp(value, -1f, 1f);
    }

    // Méthode pour obtenir les poids du réseau de neurones
    public float[] GetWeights()
    {
        int totalWeights = inputSize * hiddenSize + hiddenSize * outputSize;
        float[] weightsList = new float[totalWeights];
        int count = 0;

        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weightsList[count] = weightsInputHidden[i, j];
                count++;
            }
        }

        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weightsList[count] = weightsHiddenOutput[i, j];
                count++;
            }
        }

        return weightsList;
    }

    // Méthode pour définir les poids du réseau de neurones
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
