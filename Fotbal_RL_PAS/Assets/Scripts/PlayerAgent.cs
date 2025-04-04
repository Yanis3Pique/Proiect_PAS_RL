using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;

public class PlayerAgent : Agent
{
    // Identificatorul echipei: 1 (RedTeam) sau 2 (BlueTeam)
    public int teamID;

    // Referință către mingea din scenă (setată în Inspector)
    public Transform ball;

    // Viteza de mișcare a jucătorului
    public float moveSpeed = 5f;

    // Viteza de rotație a jucătorului
    public float rotationSpeed = 100f;

    public override void OnEpisodeBegin()
    {
        // Resetează starea agentului (poziție, rotație, etc.) după necesități
    }

    // Observații: se adaugă 4 valori total (3 de la vectorul normalizat și 1 scalar pentru distanță)
    public override void CollectObservations(VectorSensor sensor)
    {
        // Vectorul diferență (3 componente)
        sensor.AddObservation((ball.position - transform.position).normalized);
        // Distanța (1 componentă)
        sensor.AddObservation(Vector3.Distance(transform.position, ball.position));
    }

    // Aplică acțiunile primite de la rețeaua antrenată
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Acțiunea 0: mișcare înainte/înapoi
        float moveZ = actions.ContinuousActions[0];
        // Acțiunea 1: mișcare laterală
        float moveX = actions.ContinuousActions[1];
        // Acțiunea 2: rotație (stânga/dreapta)
        float rotate = actions.ContinuousActions[2];

        // Mișcarea în planul XZ
        Vector3 movement = new Vector3(moveX, 0, moveZ);
        transform.position += movement * Time.deltaTime * moveSpeed;

        // Rotația în jurul axei Y
        transform.Rotate(Vector3.up, rotate * Time.deltaTime * rotationSpeed);
    }

    // Metodă Heuristic folosind noul Input System (pentru control manual în testare)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            continuousActionsOut[0] = 0f;
            continuousActionsOut[1] = 0f;
            continuousActionsOut[2] = 0f;
            return;
        }

        float moveZ = 0f;
        float moveX = 0f;
        float rotate = 0f;

        // Mișcarea: tastele W/S pentru fata/spate și A/D pentru stânga/dreapta
        if (keyboard.wKey.isPressed)
            moveZ = 1f;
        else if (keyboard.sKey.isPressed)
            moveZ = -1f;

        if (keyboard.dKey.isPressed)
            moveX = 1f;
        else if (keyboard.aKey.isPressed)
            moveX = -1f;

        // Rotația: tastele Q/E (Q pentru rotație spre stânga, E pentru spre dreapta)
        if (keyboard.qKey.isPressed)
            rotate = -1f;
        else if (keyboard.eKey.isPressed)
            rotate = 1f;

        continuousActionsOut[0] = moveZ;
        continuousActionsOut[1] = moveX;
        continuousActionsOut[2] = rotate;
    }
}
