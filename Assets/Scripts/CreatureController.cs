using System;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    // Variables d'environnement  
    public float distanceTraveled;
    public float slopeAngle;
    //public float headHeight;
    public float speed;
    public float distanceToTarget;

    private Vector3 startingPosition;
    public Transform leftFoot;
    public Transform rightFoot;
    private Rigidbody rb;

    private NeuralNetwork neuralNetwork;

    // Variables pour les articulations de la créature
    public Rigidbody leftLeg;
    public Rigidbody rightLeg;

    // Variables pour contrôler les `HingeJoint`
    private HingeJoint leftHingeJoint;
    private HingeJoint rightHingeJoint;

    // Variables pour les valeurs de sortie du réseau de neurones
    private float leftLegForce;
    private float rightLegForce;

    // Variable pour l'objectif
    public Transform target;

    // variable pour le compteur
    private DateTime startTime;


    private void Awake()
    {
        target = GameObject.Find("Target").transform;
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        // Récupérer les `HingeJoint` attachés aux Rigidbodies des jambes
        leftHingeJoint = leftLeg.GameObject().GetComponent<HingeJoint>();
        rightHingeJoint = rightLeg.GameObject().GetComponent<HingeJoint>();
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startTime = DateTime.Now;
    }

    private void Update()
    {
        // Calculez la distance parcourue depuis le début de la simulation
        distanceTraveled = Vector3.Distance(startingPosition, transform.position);
        // Distance entre la créature et l'objectif
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        // Raycast vers le bas à partir des pieds de la créature pour détecter la pente
        
        RaycastHit leftHit;
        RaycastHit rightHit;
        float maxRaycastDistance = 1.0f; // Ajustez cette valeur selon la hauteur de vos pieds par rapport au sol

        if (Physics.Raycast(leftFoot.position, Vector3.down, out leftHit, maxRaycastDistance))
        {
            if (Physics.Raycast(rightFoot.position, Vector3.down, out rightHit, maxRaycastDistance))
            {
                // Calculez l'angle de la pente en utilisant les deux points de contact
                Vector3 slopeVector = leftHit.point - rightHit.point;
                slopeAngle = Vector3.Angle(Vector3.up, slopeVector);
            }
        }

       /*RaycastHit headHit;
       if(Physics.Raycast(transform.position,Vector3.down, out headHit))
       {
           headHeight = Vector3.Distance(transform.position, headHit.point);
       }
        */
       
        // Obtenez la vitesse linéaire du Rigidbody
        speed = rb.velocity.magnitude;
        
        // Obtenez les informations d'environnement (inputs) à chaque mise à jour
        float[] environmentInputs = GetEnvironmentInputs();

        // Utilisez les valeurs d'environnement dans le réseau de neurones
        float[] outputs = neuralNetwork.FeedForward(environmentInputs);
        SetOutputValues(outputs[0], outputs[1]);

        // Appliquer les forces aux articulations pour marcher
        leftLeg.AddForce(Vector3.forward * leftLegForce);
        rightLeg.AddForce(Vector3.forward * rightLegForce);
    }

    // Méthode pour appliquer les forces aux articulations
    private void FixedUpdate()
    {
        // Appliquer les forces aux articulations pour marcher
        leftLeg.AddForce(Vector3.forward * leftLegForce);
        rightLeg.AddForce(Vector3.forward * rightLegForce);
    }

    public void SetNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        this.neuralNetwork = neuralNetwork;
    }

    
    // Method to set the neural network's weights
    public void SetNeuralNetworkWeights(float[] weights)
    {
        // Set the neural network's weights using the provided array
        if (neuralNetwork != null)
        {
            neuralNetwork.SetWeights(weights);
        }
    }
    
    // Méthode pour définir les valeurs de sortie du réseau de neurones
    public void SetOutputValues(float leftForce, float rightForce)
    {
        leftLegForce = leftForce;
        rightLegForce = rightForce;
        
    }

    // Méthode pour contrôler les jambes avec les `HingeJoint`
    public void SetLegAngles(float leftAngle, float rightAngle)
    {
        // Appliquer l'angle aux `HingeJoint`
        try
        {
            JointSpring leftSpring = leftHingeJoint.spring;
            leftSpring.targetPosition = leftAngle;
            leftHingeJoint.spring = leftSpring;

            JointSpring rightSpring = rightHingeJoint.spring;
            rightSpring.targetPosition = rightAngle;
            rightHingeJoint.spring = rightSpring;
        }
        catch (Exception e)
        {
            Debug.Log("Exception : " + e);
        }
        
    }

    // Méthode pour obtenir les informations d'environnement (inputs)
    // Cette méthode doit être appelée dans la mise à jour de votre simulation pour mettre à jour les valeurs des inputs
    public float[] GetEnvironmentInputs()
    {
        Debug.Log("GetEnvironmentInputs()");



        // Retourner les valeurs des inputs sous forme d'un tableau
        return new float[] { distanceTraveled, slopeAngle, speed, distanceToTarget };
    }

    public bool TerminationConditionMet()
    {
        if (distanceToTarget <= 1 || DateTime.Now >= startTime.AddSeconds(10) ) return true;
        else return false;
    }
}
