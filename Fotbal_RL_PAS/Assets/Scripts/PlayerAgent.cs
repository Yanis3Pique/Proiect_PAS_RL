using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAgent : Agent
{
	public int teamID;
	public Transform ball;

	public float moveSpeed = 5f;
	public float rotationSpeed = 200f;

	private Rigidbody rb;

	public override void Initialize()
	{
		rb = GetComponent<Rigidbody>();

		// Blocăm rotația ca să nu se rastoarne
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		float moveZ = actions.ContinuousActions[0];  // Înainte/înapoi
		float moveX = actions.ContinuousActions[1];  // Stânga/dreapta
		float rotate = actions.ContinuousActions[2]; // Rotație

		// Direcție de deplasare (față-spate + lateral), relativă la orientarea agentului
		Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
		Vector3 move = transform.TransformDirection(moveDir) * moveSpeed * Time.fixedDeltaTime;

		rb.MovePosition(rb.position + move);

		// Rotație controlată (pe Y)
		Quaternion deltaRot = Quaternion.Euler(0f, rotate * rotationSpeed * Time.fixedDeltaTime, 0f);
		rb.MoveRotation(rb.rotation * deltaRot);
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation((ball.position - transform.position).normalized);
		sensor.AddObservation(Vector3.Distance(transform.position, ball.position));
	}

	public override void OnEpisodeBegin()
	{
		// Poți adăuga aici random spawn etc.
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var ca = actionsOut.ContinuousActions;
		var k = Keyboard.current;
		ca[0] = k.wKey.isPressed ? 1f : k.sKey.isPressed ? -1f : 0f;
		ca[1] = k.dKey.isPressed ? 1f : k.aKey.isPressed ? -1f : 0f;
		ca[2] = k.eKey.isPressed ? 1f : k.qKey.isPressed ? -1f : 0f;
	}
}
