using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField]
    private bool begalEnabled = true;
    [SerializeField]
    private List<Floor> liftQueues, begalLiftQueues;
    [SerializeField]
    private Material begalMaterial;

    private void OnEnable()
    {
        GameEvents.OnStudentArrived.Add(GiveQueue);
    }

    private void OnDisable()
    {
        GameEvents.OnStudentArrived.Remove(GiveQueue);
    }

    private void GiveQueue(Student toStudent)
    {
        if (liftQueues != null)
        {
            int queueingNumber = 0;
            foreach (Floor floor in liftQueues)
            {
                queueingNumber += floor.queue.Count;
            }
            float begalChance = 1f / (1 + Mathf.Pow(2.7182818f, -0.25f * (queueingNumber - 35)));

            if (begalEnabled && Random.Range(0f, 1f) < begalChance)
            {
                toStudent.GetComponent<MeshRenderer>().material = begalMaterial;
                begalLiftQueues[Random.Range(0, begalLiftQueues.Count)].Enqueue(toStudent);
            }
            else
            {
                liftQueues[Random.Range(0, liftQueues.Count)].Enqueue(toStudent);
            }
        }
    }
}
