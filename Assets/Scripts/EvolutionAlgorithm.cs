using System.Collections.Generic;
using UnityEngine;

public class EvolutionAlgorithm : MonoBehaviour
{
    public int populationSize = 100;
    public int inputSize = 4; // Remplacez cela par la taille des entrées réelle de votre créature
    public int hiddenSize = 8;
    public int outputSize = 2; // Remplacez cela par la taille des sorties réelle de votre créature

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
    

    // Appeler cette méthode pour démarrer l'évolution
    public void StartEvolution()
    {
        population = new List<NeuralNetwork>();

        // Créer une population initiale de réseaux de neurones
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(inputSize, hiddenSize, outputSize);
            population.Add(neuralNetwork);
        }

        // Lancer la simulation avec la première créature de la population
        if (population.Count > 0)
        {
            currentCreatureIndex = 0;
            ApplyNeuralNetworkToCreature(population[currentCreatureIndex]);
        }
    }

    // Appeler cette méthode pour passer à la créature suivante dans la population
    private void NextCreature()
    {
        currentCreatureIndex++;
        if (currentCreatureIndex < population.Count)
        {
            ApplyNeuralNetworkToCreature(population[currentCreatureIndex]);
        }
        else
        {
            // Toutes les créatures ont été testées, déclencher l'étape d'évolution
            EvolvePopulation();
        }
    }

    // Appliquer le réseau de neurones à la créature pour la tester
    private void ApplyNeuralNetworkToCreature(NeuralNetwork neuralNetwork)
    {
        creatureController = GameObject.Find("Creature").GetComponent<CreatureController>();

        // Obtenir les inputs de l'environnement à partir du CreatureController
        float[] inputs = creatureController.GetEnvironmentInputs();

        // Appliquer les inputs au réseau de neurones
        float[] outputs = neuralNetwork.FeedForward(inputs);

        // Contrôler les jambes avec les forces calculées par le réseau de neurones
        creatureController.SetOutputValues(outputs[0], outputs[1]);


    }

    // Méthode pour évoluer la population
    private void EvolvePopulation()
    {
        // Sélectionner deux réseaux parents aléatoires
        int parentIndex1 = Random.Range(0, population.Count);
        int parentIndex2 = Random.Range(0, population.Count);

        // Créer un nouveau réseau enfant en croisant les parents
        NeuralNetwork parent1 = population[parentIndex1];
        NeuralNetwork parent2 = population[parentIndex2];
        NeuralNetwork child = CrossOver(parent1, parent2);

        // Appliquer une petite mutation au réseau enfant
        Mutate(child);

        // Remplacer le réseau le moins performant dans la population par le réseau enfant
        int worstIndex = FindWorstNetwork();
        population[worstIndex] = child;

        // Passer à la créature suivante dans la population
        NextCreature();
    }

    private NeuralNetwork CrossOver(NeuralNetwork parent1, NeuralNetwork parent2)
    {
        NeuralNetwork child = new NeuralNetwork(inputSize, hiddenSize, outputSize);

        // Croiser les poids des parents pour créer un nouvel enfant
        float[] parent1Weights = parent1.GetWeights();
        float[] parent2Weights = parent2.GetWeights();
        float[] childWeights = new float[parent1Weights.Length];

        // Choisissez un point de croisement aléatoire
        int crossoverPoint = Random.Range(0, parent1Weights.Length);

        // Appliquez les poids du parent1 jusqu'au point de croisement
        for (int i = 0; i < crossoverPoint; i++)
        {
            childWeights[i] = parent1Weights[i];
        }

        // Appliquez les poids du parent2 à partir du point de croisement
        for (int i = crossoverPoint; i < parent2Weights.Length; i++)
        {
            childWeights[i] = parent2Weights[i];
        }

        // Appliquer les poids au réseau enfant
        child.SetWeights(childWeights);

        return child;
    }

    // Méthode pour appliquer une petite mutation à un réseau de neurones
    private void Mutate(NeuralNetwork neuralNetwork)
    {
        float[] weights = neuralNetwork.GetWeights();

        // Choisissez un index aléatoire pour effectuer la mutation
        int mutationIndex = Random.Range(0, weights.Length);

        // Appliquez une petite variation au poids sélectionné
        float mutationAmount = Random.Range(-0.1f, 0.1f);
        weights[mutationIndex] += mutationAmount;

        // Appliquer les poids mutés au réseau de neurones
        neuralNetwork.SetWeights(weights);
    }

    // Méthode pour trouver l'index du réseau le moins performant dans la population
    private int FindWorstNetwork()
    {
        float worstFitness = float.MaxValue;
        int worstIndex = 0;

        // Parcourez tous les réseaux de la population pour trouver le moins performant
        for (int i = 0; i < population.Count; i++)
        {
            NeuralNetwork network = population[i];
            // Remplacez fitnessFunction par la méthode que vous utilisez pour évaluer la performance des réseaux
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
        // Réinitialiser la position de la créature avant de tester le réseau de neurones
        creatureController.transform.position = creatureController.startingPosition;
        creatureController.distanceTraveled = 0f;

        // Durée maximale pour tester le réseau de neurones (par exemple, 10 secondes)
        float maxTestDuration = 10f;
        float testDuration = 0f;

        while (testDuration < maxTestDuration)
        {
            // Obtenir les inputs de l'environnement à partir du CreatureController
            float[] inputs = creatureController.GetEnvironmentInputs();

            // Appliquer les inputs au réseau de neurones
            float[] outputs = neuralNetwork.FeedForward(inputs);

            // Contrôler les jambes avec les forces calculées par le réseau de neurones
            creatureController.SetOutputValues(outputs[0], outputs[1]);

            // Mettre à jour la durée du test
            testDuration += Time.deltaTime;

            // Mettre à jour la distance parcourue par la créature
            creatureController.distanceTraveled = Vector3.Distance(creatureController.startingPosition, creatureController.transform.position);

            // Sortir de la boucle si la créature atteint l'objectif
            if (creatureController.distanceToTarget < 1f)
            {
                break;
            }
        }

        // Plus la créature atteint rapidement l'objectif, meilleure est sa performance (fitness)
        float fitness = 1f / testDuration;

        return fitness;
    }


}
