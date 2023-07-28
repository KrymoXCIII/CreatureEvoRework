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
    

    public EvolutionAlgorithm EA;
    // Start is called before the first frame update
    void Start()
    {
        temps.text = WFC.timeUse;
        Nbcreature.text = EA.currentCreatureIndex.ToString();
        generation.text = EA.currentGenerationIndex.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
