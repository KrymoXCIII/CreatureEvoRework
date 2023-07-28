using System;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    // Variables d'environnement  
    public float distanceTraveled;
    public float slopeAngle;

    public float headAngleX;
    public float headAngleZ;
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
        // Angle de la tête
        headAngleX = transform.rotation.x;
        headAngleZ = transform.rotation.z;
        
        // Raycast vers le bas à partir des pieds de la créature pour détecter la pente
      /*  RaycastHit leftHit;
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
        */
       
       
        // Obtenez la vitesse linéaire du Rigidbody
        speed = rb.velocity.magnitude;
        
        // Obtenez les informations d'environnement (inputs) à chaque mise à jour
        float[] environmentInputs = GetEnvironmentInputs();

        // Utilisez les valeurs d'environnement dans le réseau de neurones
        float[] outputs = neuralNetwork.FeedForward(environmentInputs);
        SetOutputValues(outputs[0], outputs[1]);

        // Appliquer les forces aux articulations pour marcher
        //leftLeg.AddForce(Vector3.forward * leftLegForce);
        //rightLeg.AddForce(Vector3.forward * rightLegForce);
        
        
        
        
        
    }

    // Méthode pour appliquer les forces aux articulations
    private void FixedUpdate()
    {
        
        Debug.Log("L : " + leftLegForce + " R : " + rightLegForce);
        // Appliquer les forces aux articulations pour marcher en utilisant le moteur
        float motorSpeedMultiplier = 100f; // Ajustez cette valeur selon la vitesse souhaitée
        
        leftHingeJoint.motor = new JointMotor { targetVelocity = leftLegForce * motorSpeedMultiplier, force = 100f };

        
        rightHingeJoint.motor = new JointMotor { targetVelocity = rightLegForce * motorSpeedMultiplier, force = 100f };

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

    
    // Méthode pour obtenir les informations d'environnement (inputs)
    // Cette méthode doit être appelée dans la mise à jour de votre simulation pour mettre à jour les valeurs des inputs
    public float[] GetEnvironmentInputs()
    {
        Debug.Log("GetEnvironmentInputs()");



        // Retourner les valeurs des inputs sous forme d'un tableau
        return new float[] { distanceTraveled, headAngleX, headAngleZ, speed, distanceToTarget };
    }

    public bool TerminationConditionMet()
    {
        if (distanceToTarget <= 5 || (DateTime.Now >= startTime.AddSeconds(2)) && distanceTraveled < 2 ) return true;
        else return false;
    }
}
