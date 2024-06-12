using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField]
    private Transform queuePosition;
    public float yPosition;
    public Queue<Student> queue { get; private set; } = new();

    private void Awake()
    {
        yPosition = transform.position.y;
    }

    public void Enqueue(Student student)
    {
        Vector2 randomCircle = Random.insideUnitCircle * 2f;
        student.Move(new Vector3(queuePosition.position.x + randomCircle.x, yPosition, queuePosition.position.z + randomCircle.y), 1f);
        queue.Enqueue(student);
    }

    public void EnterLift(Lift lift)
    {
        if (queue.Count > 0)
            lift.EnterLift(queue);
    }

    public bool QueueIsEmpty()
    {
        return queue.Count == 0;
    }
}
