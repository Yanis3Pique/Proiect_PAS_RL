using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAgent : Agent
{
    [SerializeField]
    private int teamID;

    [SerializeField]
    private Transform ball;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 200f;

    [SerializeField]
    private Transform initialPosition;

    private Rigidbody rb;


    [SerializeField] 
    private Transform ownGoal;
    [SerializeField] 
    private Transform adversaryGoal;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnEpisodeBegin()
    {
        if (initialPosition != null)
        {
            transform.position = initialPosition.position;
            transform.rotation = initialPosition.rotation;
        }
     
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        float moveX = actions.ContinuousActions[1];
        float rotate = actions.ContinuousActions[2];

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 move = transform.TransformDirection(moveDir) * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        Quaternion deltaRot = Quaternion.Euler(0f, rotate * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRot);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((ball.position - transform.position).normalized);
        sensor.AddObservation(Vector3.Distance(transform.position, ball.position));
        sensor.AddObservation((float)teamID);

        sensor.AddObservation(ownGoal.position - transform.position);
        sensor.AddObservation(adversaryGoal.position - transform.position);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ca = actionsOut.ContinuousActions;

        if (Keyboard.current == null)
        {
            
            ca[0] = 0f;
            ca[1] = 0f;
            ca[2] = 0f;
            return;
        }

        var k = Keyboard.current;
        ca[0] = k.wKey.isPressed ? 1f : k.sKey.isPressed ? -1f : 0f;
        ca[1] = k.dKey.isPressed ? 1f : k.aKey.isPressed ? -1f : 0f;
        ca[2] = k.eKey.isPressed ? 1f : k.qKey.isPressed ? -1f : 0f;
    }

}
