using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Canvas : MonoBehaviour
{
    public WaveFunctionCollapse WFC;
    public TextMeshProUGUI temps;
    public TextMeshProUGUI Nbcreature;
    public TextMeshProUGUI generation;
    public TextMeshProUGUI fitness;
    public NeuralNetwork NeuralN;
    

    public EvolutionAlgorithm EA;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        temps.text = WFC.timeUse;
        Nbcreature.text = EA.currentCreatureIndex.ToString();
        generation.text = EA.currentGenerationIndex.ToString();
        fitness.text = NeuralN.fitness.ToString();
    }
}
