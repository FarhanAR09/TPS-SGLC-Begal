using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject studentPrefab;
    [SerializeField]
    private List<Transform> spawnPositions;
    [SerializeField]
    private List<Floor> floors;

    private float spawnTimer = 0f;
    private readonly float spawnCooldown = 0.5f;

    private int queueingNumber = 0;

    private void FixedUpdate()
    {
        if (spawnTimer < spawnCooldown)
        {
            spawnTimer += Time.fixedDeltaTime;
        }
        else
        {
            spawnTimer = 0f;
            if (studentPrefab != null)
            {
                
                Student arrivingStudent = Instantiate(studentPrefab, spawnPositions[Random.Range(0, spawnPositions.Count)].position, new Quaternion()).GetComponent<Student>();
                arrivingStudent.targetFloor = Random.Range(3, 12);
                GameEvents.OnStudentArrived.Publish(arrivingStudent);
            }
        }
    }
}
