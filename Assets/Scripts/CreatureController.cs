using UnityEngine;

public class CreatureController : MonoBehaviour
{
    //Variables d'environnement  
    private float distanceTraveled;
    private float slopeAngle;   
    private float speed;
    private float distanceToTarget;
    //

    private Vector3 startingPosition;
    public Transform leftFoot;
    public Transform rightFoot;
    private Rigidbody rb;
    // 

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

    private void Start()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        // Récupérer les `HingeJoint` attachés aux Rigidbodies des jambes
        leftHingeJoint = leftLeg.GetComponent<HingeJoint>();
        rightHingeJoint = rightLeg.GetComponent<HingeJoint>();
    }


    private void Update()
    {
        // Calculez la distance parcourue depuis le début de la simulation
        distanceTraveled = Vector3.Distance(startingPosition, transform.position);

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

        // Obtenez la vitesse linéaire du Rigidbody
        speed = rb.velocity.magnitude;


       
    }

    // Méthode pour appliquer les forces aux articulations
    private void FixedUpdate()
    {
       Debug.Log("L : " + leftLegForce + " R : " + rightLegForce);
        MoveLegs(10, 10);//leftLegForce, rightLegForce);
    }


    public void SetOutputValues(float leftForce, float rightForce)
    {
        leftLegForce = leftForce;
        rightLegForce = rightForce;
    }


    // Méthode pour déplacer les jambes avec les `HingeJoint`
    private void MoveLegs(float leftLegForce, float rightLegForce)
    {
        // Convertir la force des jambes en angles pour les `HingeJoint`
        float leftLegAngle = leftLegForce * 45f; // Angle de -45 à 45 degrés
        float rightLegAngle = rightLegForce * 45f; // Angle de -45 à 45 degrés

        // Contrôler les `HingeJoint` avec les angles calculés
        SetLegAngles(leftLegAngle, rightLegAngle);
    }



    // Méthode pour contrôler les jambes avec les `HingeJoint`
    public void SetLegAngles(float leftAngle, float rightAngle)
    {
        // Appliquer l'angle aux `HingeJoint`
        JointSpring leftSpring = leftHingeJoint.spring;
        leftSpring.targetPosition = leftAngle;
        leftHingeJoint.spring = leftSpring;

        JointSpring rightSpring = rightHingeJoint.spring;
        rightSpring.targetPosition = rightAngle;
        rightHingeJoint.spring = rightSpring;
    }

  

    // Méthode pour obtenir les informations d'environnement (inputs)
    // Cette méthode doit être appelée dans la mise à jour de votre simulation pour mettre à jour les valeurs des inputs
    public float[] GetEnvironmentInputs()
    {
        // Remplacez les valeurs ci-dessous par les véritables données d'environnement que votre créature reçoit
        // Par exemple, vous pouvez obtenir la distance parcourue, l'angle de la pente, la vitesse, etc.

        distanceToTarget = Vector3.Distance(transform.position, target.position); // Distance entre la créature et l'objectif

        // Retourner les valeurs des inputs sous forme d'un tableau
        return new float[] { distanceTraveled, slopeAngle, speed, distanceToTarget};
    }
}