using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EvolutionAlgorithm : MonoBehaviour
{
    public int populationSize = 5;
    public int inputSize = 5; // Replace this with the actual input size of your creature
    public int hiddenSize = 30;
    public int outputSize = 2; // Replace this with the actual output size of your creature

    public GameObject creaturePrefab; // Drag your creature prefab here
    public Transform spawnPoint; // Drag the spawn point GameObject here

    private List<NeuralNetwork> population;
    public int currentCreatureIndex = 0;
    public int currentGenerationIndex = 1;

    private CreatureController currentCreatureController;
    private bool evolving = false;

    public WaveFunctionCollapse WFC;
    public FoodSpawner foodSpawner;

    bool canStartEvo = true;

    UnityEngine.SceneManagement.Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "SceneCreature")
        {
            StartEvolution();
        }
        else
        {
            foodSpawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        }
        //StartEvolution();
        //WFC = GameObject.Find("WaveManager").GetComponent<WaveFunctionCollapse>();
       
    }

    // Call this method to start the evolution
    public void StartEvolution()
    {
        Debug.Log("Start Evolution");
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
        Debug.Log(("Spawning Creature  " + currentCreatureIndex + " Of Generation " + currentGenerationIndex));
        if (currentCreatureController != null)
        {
            Destroy(currentCreatureController.gameObject);
        }
                

        GameObject newCreature = Instantiate(creaturePrefab, spawnPoint.position, Quaternion.identity);
        currentCreatureController = newCreature.GetComponent<CreatureController>();
        currentCreatureController.SetOutputValues(0f, 0f); // Optionally set initial output values
        //currentCreatureController.SetLegAngles(0f, 0f); // Optionally set initial leg angles
        currentCreatureController.SetNeuralNetwork(neuralNetwork); // Set the neural network of the creature
    
        

    }

    // Method to move on to the next creature in the population
    private void NextCreature()
    {
        Debug.Log("Next Creature");
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
    // Method to evolve the population
    private void EvolvePopulation()
    {
        Debug.Log("Evolve Population");
        currentGenerationIndex++;

        // Sort the population by fitness in descending order
        population.Sort((a, b) => b.fitness.CompareTo(a.fitness));

        // Select the top-performing individuals as parents for the next generation
        List<NeuralNetwork> parents = new List<NeuralNetwork>();
        int numParents = Mathf.Min(populationSize / 2, population.Count); // Select the top half of the population as parents

        for (int i = 0; i < numParents; i++)
        {
            parents.Add(population[i]);
        }

        // Create the next generation through crossover and mutation
        List<NeuralNetwork> nextGeneration = new List<NeuralNetwork>();

        while (nextGeneration.Count < populationSize)
        {
            NeuralNetwork parent1 = parents[Random.Range(0, numParents)];
            NeuralNetwork parent2 = parents[Random.Range(0, numParents)];

            // Perform crossover to create a child neural network
            NeuralNetwork child = parent1.Crossover(parent2);

            // Perform mutation on the child neural network
            child.Mutate();

            // Add the child to the next generation
            nextGeneration.Add(child);
        }

        // Replace the old population with the new generation
        population = nextGeneration;

        // Reset the index for the current creature
        currentCreatureIndex = 0;

        // Spawn the first creature of the new population
        SpawnCreature(population[currentCreatureIndex]);
    }

    

    




    private void Update()
    {
        if (scene.name != "SceneCreature")
        {




            if (foodSpawner.foodIsSpawned)
            {
                if (canStartEvo)
                {
                    StartEvolution();
                    canStartEvo = false;
                }
            }


        }
        
        
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
        
        // Calculate the difference between the current angle and the target angle
        float angleDifferenceX = Mathf.Abs(creature.headAngleX);
        float angleDifferenceZ = Mathf.Abs(creature.headAngleZ);
        // Normalize the angle differences to a range of 0 to 1
        float normalizedDifferenceX = 1f - Mathf.Clamp01(angleDifferenceX / 180f);
        float normalizedDifferenceZ = 1f - Mathf.Clamp01(angleDifferenceZ / 180f);
        
        float headAngleScore = (normalizedDifferenceX) + (normalizedDifferenceZ);
        
        // Orientation verticale (plus proche de la verticale est meilleur)
        //float verticalOrientationScore = 1f / (1f + Mathf.Abs(180f - creature.slopeAngle));

        // Orientation verticale (plus proche de la verticale est meilleur)
        //float verticalOrientationScore = 1f / (1f + Mathf.Abs(creature.slopeAngle));

        // Distance entre la créature et l'objectif (plus proche de l'objectif est meilleur)
        float proximityScore = 1f / (1f + creature.distanceToTarget);

        float distanceScore = 1f / (1f + creature.distanceTraveled);

        // Poids attribués à chaque critère (ajustez ces valeurs selon vos préférences)
        //float verticalOrientationWeight = 0.7f;
        float proximityWeight = 0.7f;
        float headAngleWeight = 0.3f;
        float distanceWeight = 0.7f;

        
        // Calcul de la fitness en combinant les critères avec les poids
        float fitness = (headAngleWeight * headAngleScore + proximityWeight * proximityScore + distanceWeight * distanceScore) / 3;
        Debug.Log("Fitness : " + fitness);
        return fitness;
    }


}
