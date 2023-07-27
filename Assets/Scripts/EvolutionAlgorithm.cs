using System.Collections.Generic;
using UnityEngine;

public class EvolutionAlgorithm : MonoBehaviour
{
    public int populationSize = 100;
    public int inputSize = 4; // Remplacez cela par la taille des entr�es r�elle de votre cr�ature
    public int hiddenSize = 8;
    public int outputSize = 2; // Remplacez cela par la taille des sorties r�elle de votre cr�ature

    private List<NeuralNetwork> population;
    private int currentCreatureIndex = 0;

    private CreatureController creatureController;

    private void Awake()
    {
        creatureController = GameObject.Find("Creature").GetComponent<CreatureController>();
    }

    private void Start()
    {
        StartEvolution();
    }
    

    // Appeler cette m�thode pour d�marrer l'�volution
    public void StartEvolution()
    {
        population = new List<NeuralNetwork>();

        // Cr�er une population initiale de r�seaux de neurones
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(inputSize, hiddenSize, outputSize);
            population.Add(neuralNetwork);
        }

        // Lancer la simulation avec la premi�re cr�ature de la population
        if (population.Count > 0)
        {
            currentCreatureIndex = 0;
            ApplyNeuralNetworkToCreature(population[currentCreatureIndex]);
        }
    }

    // Appeler cette m�thode pour passer � la cr�ature suivante dans la population
    private void NextCreature()
    {
        currentCreatureIndex++;
        if (currentCreatureIndex < population.Count)
        {
            ApplyNeuralNetworkToCreature(population[currentCreatureIndex]);
        }
        else
        {
            // Toutes les cr�atures ont �t� test�es, d�clencher l'�tape d'�volution
            EvolvePopulation();
        }
    }

    // Appliquer le r�seau de neurones � la cr�ature pour la tester
    private void ApplyNeuralNetworkToCreature(NeuralNetwork neuralNetwork)
    {
        creatureController = GameObject.Find("Creature").GetComponent<CreatureController>();

        // Obtenir les inputs de l'environnement � partir du CreatureController
        float[] inputs = creatureController.GetEnvironmentInputs();

        // Appliquer les inputs au r�seau de neurones
        float[] outputs = neuralNetwork.FeedForward(inputs);

        // Contr�ler les jambes avec les forces calcul�es par le r�seau de neurones
        creatureController.SetOutputValues(outputs[0], outputs[1]);


    }

    // M�thode pour �voluer la population
    private void EvolvePopulation()
    {
        // S�lectionner deux r�seaux parents al�atoires
        int parentIndex1 = Random.Range(0, population.Count);
        int parentIndex2 = Random.Range(0, population.Count);

        // Cr�er un nouveau r�seau enfant en croisant les parents
        NeuralNetwork parent1 = population[parentIndex1];
        NeuralNetwork parent2 = population[parentIndex2];
        NeuralNetwork child = CrossOver(parent1, parent2);

        // Appliquer une petite mutation au r�seau enfant
        Mutate(child);

        // Remplacer le r�seau le moins performant dans la population par le r�seau enfant
        int worstIndex = FindWorstNetwork();
        population[worstIndex] = child;

        // Passer � la cr�ature suivante dans la population
        NextCreature();
    }

    private NeuralNetwork CrossOver(NeuralNetwork parent1, NeuralNetwork parent2)
    {
        NeuralNetwork child = new NeuralNetwork(inputSize, hiddenSize, outputSize);

        // Croiser les poids des parents pour cr�er un nouvel enfant
        float[] parent1Weights = parent1.GetWeights();
        float[] parent2Weights = parent2.GetWeights();
        float[] childWeights = new float[parent1Weights.Length];

        // Choisissez un point de croisement al�atoire
        int crossoverPoint = Random.Range(0, parent1Weights.Length);

        // Appliquez les poids du parent1 jusqu'au point de croisement
        for (int i = 0; i < crossoverPoint; i++)
        {
            childWeights[i] = parent1Weights[i];
        }

        // Appliquez les poids du parent2 � partir du point de croisement
        for (int i = crossoverPoint; i < parent2Weights.Length; i++)
        {
            childWeights[i] = parent2Weights[i];
        }

        // Appliquer les poids au r�seau enfant
        child.SetWeights(childWeights);

        return child;
    }

    // M�thode pour appliquer une petite mutation � un r�seau de neurones
    private void Mutate(NeuralNetwork neuralNetwork)
    {
        float[] weights = neuralNetwork.GetWeights();

        // Choisissez un index al�atoire pour effectuer la mutation
        int mutationIndex = Random.Range(0, weights.Length);

        // Appliquez une petite variation au poids s�lectionn�
        float mutationAmount = Random.Range(-0.1f, 0.1f);
        weights[mutationIndex] += mutationAmount;

        // Appliquer les poids mut�s au r�seau de neurones
        neuralNetwork.SetWeights(weights);
    }

    // M�thode pour trouver l'index du r�seau le moins performant dans la population
    private int FindWorstNetwork()
    {
        float worstFitness = float.MaxValue;
        int worstIndex = 0;

        // Parcourez tous les r�seaux de la population pour trouver le moins performant
        for (int i = 0; i < population.Count; i++)
        {
            NeuralNetwork network = population[i];
            // Remplacez fitnessFunction par la m�thode que vous utilisez pour �valuer la performance des r�seaux
            float fitness = fitnessFunction(network);
            if (fitness < worstFitness)
            {
                worstFitness = fitness;
                worstIndex = i;
            }
        }

        return worstIndex;
    }

    private float FitnessFunction(NeuralNetwork neuralNetwork)
    {
        // R�initialiser la position de la cr�ature avant de tester le r�seau de neurones
        creatureController.transform.position = creatureController.startingPosition;
        creatureController.distanceTraveled = 0f;

        // Dur�e maximale pour tester le r�seau de neurones (par exemple, 10 secondes)
        float maxTestDuration = 10f;
        float testDuration = 0f;

        while (testDuration < maxTestDuration)
        {
            // Obtenir les inputs de l'environnement � partir du CreatureController
            float[] inputs = creatureController.GetEnvironmentInputs();

            // Appliquer les inputs au r�seau de neurones
            float[] outputs = neuralNetwork.FeedForward(inputs);

            // Contr�ler les jambes avec les forces calcul�es par le r�seau de neurones
            creatureController.SetOutputValues(outputs[0], outputs[1]);

            // Mettre � jour la dur�e du test
            testDuration += Time.deltaTime;

            // Mettre � jour la distance parcourue par la cr�ature
            creatureController.distanceTraveled = Vector3.Distance(creatureController.startingPosition, creatureController.transform.position);

            // Sortir de la boucle si la cr�ature atteint l'objectif
            if (creatureController.distanceToTarget < 1f)
            {
                break;
            }
        }

        // Plus la cr�ature atteint rapidement l'objectif, meilleure est sa performance (fitness)
        float fitness = 1f / testDuration;

        return fitness;
    }


}
