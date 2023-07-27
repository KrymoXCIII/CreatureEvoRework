using System.Collections.Generic;
using UnityEngine;

public class EvolutionAlgorithm : MonoBehaviour
{
    public int populationSize = 100;
    public int inputSize = 4; // Replace this with the actual input size of your creature
    public int hiddenSize = 8;
    public int outputSize = 2; // Replace this with the actual output size of your creature

    public GameObject creaturePrefab; // Drag your creature prefab here
    public Transform spawnPoint; // Drag the spawn point GameObject here

    private List<NeuralNetwork> population;
    private int currentCreatureIndex = 0;

    private CreatureController currentCreatureController;
    private bool evolving = false;

    private void Start()
    {
        StartEvolution();
    }

    // Call this method to start the evolution
    public void StartEvolution()
    {
        evolving = true;
        population = new List<NeuralNetwork>();

        // Create an initial population of neural networks
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork neuralNetwork = new NeuralNetwork(inputSize, hiddenSize, outputSize);
            population.Add(neuralNetwork);
        }

        // Spawn the first creature of the population
        if (population.Count > 0)
        {
            currentCreatureIndex = 0;
            SpawnCreature(population[currentCreatureIndex]);
        }
    }

    // Call this method to stop the evolution
    public void StopEvolution()
    {
        evolving = false;
    }

    // Method to spawn a new creature with the given neural network
    private void SpawnCreature(NeuralNetwork neuralNetwork)
    {

        if (currentCreatureController != null)
        {
            Destroy(currentCreatureController.gameObject);
        }
                

        GameObject newCreature = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
        currentCreatureController = newCreature.GetComponent<CreatureController>();
        currentCreatureController.SetOutputValues(0f, 0f); // Optionally set initial output values
        currentCreatureController.SetLegAngles(0f, 0f); // Optionally set initial leg angles
        currentCreatureController.SetNeuralNetwork(neuralNetwork); // Set the neural network of the creature
    
        

    }

    // Method to move on to the next creature in the population
    private void NextCreature()
    {
        currentCreatureIndex++;
        if (currentCreatureIndex < population.Count)
        {
            SpawnCreature(population[currentCreatureIndex]);
        }
        else
        {
            // All creatures have been tested, trigger the evolution step
            EvolvePopulation();
        }
    }

    // Method to evolve the population
    private void EvolvePopulation()
    {
        // Perform the evolution algorithm here (selection, reproduction, mutation, etc.)

        // For this example, we will simply reset the simulation with the first creature of the population
        currentCreatureIndex = 0;
        SpawnCreature(population[currentCreatureIndex]);
    }

    private void Update()
    {
        // Check if the evolution is in progress
        if (evolving)
        {
            // Check the termination condition for the current creature (e.g., reached target, distance traveled, etc.)
            // If the termination condition is met, calculate the fitness and move to the next creature
            if (currentCreatureController.TerminationConditionMet())
            {
                float fitness = CalculateFitness(currentCreatureController);
                population[currentCreatureIndex].fitness = fitness;
                NextCreature();
            }
        }
    }

    // Method to calculate the fitness of a creature (you can customize this based on your requirements)
    private float CalculateFitness(CreatureController creature)
    {
        // Example fitness calculation:
        float distanceScore = 1f / (1f + creature.distanceTraveled);
        float proximityScore = 1f / (1f + creature.distanceToTarget);
        return (distanceScore + proximityScore) / 2f;
    }
}
