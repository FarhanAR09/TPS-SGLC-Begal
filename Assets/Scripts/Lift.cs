using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Lift : MonoBehaviour
{
    public UnityEvent<int> OnLiftArrivedOnFloor = new();
    
    [SerializeField]
    private LiftManager liftManager;

    private readonly int capacity = 10;
    public List<Student> Occupants { get; private set; } = new();

    public int CurrentFloorNumber { get; private set; } = 1;
    public bool IsIdle { get; private set; } = true;
    private float idleTimer = 0f;

    private Coroutine moving = null;
    public bool debugging = false;

    private void OnEnable()
    {
        OnLiftArrivedOnFloor.AddListener(ExitLift);
    }

    private void OnDisable()
    {
        OnLiftArrivedOnFloor.RemoveListener(ExitLift);
    }

    //private void Start()
    //{
    //    IEnumerator WaitCall()
    //    {
    //        yield return new WaitForSeconds(5f);
    //        OnLiftArrivedOnFloor.Invoke(CurrentFloorNumber);
    //    }
    //    StartCoroutine(WaitCall());
    //}

    private void FixedUpdate()
    {
        if (debugging)
            Debug.Log(IsIdle + " " + gameObject.name);
        //Lift idle for more than 5 secs, start checking queue
        if (IsIdle)
        {
            if (idleTimer < 2f)
            {
                idleTimer += Time.fixedDeltaTime;
            }
            else
            {
                OnLiftArrivedOnFloor.Invoke(CurrentFloorNumber);
                idleTimer = 0f;
                if (debugging)
                    Debug.Log("Idle Check " + gameObject.name);
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }

    private void ExitLift(int floorNumber)
    {
        for (int i = Occupants.Count - 1; i >= 0; i--)
        {
            if (Occupants[i].targetFloor == CurrentFloorNumber)
            {
                //occupants[i].gameObject.SetActive(false);
                Destroy(Occupants[i].gameObject);
            }
        }
    }

    public void EnterLift(Queue<Student> studentQueue)
    {
        IEnumerator LiftMoving()
        {
            if (IsIdle)
            {
                IsIdle = false;

                //Students Entering
                while (Occupants.Count < capacity && studentQueue.Count > 0)
                {
                    Student enteringStudent = studentQueue.Dequeue();
                    Occupants.Add(enteringStudent);

                    Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 2f;
                    enteringStudent.Move(transform.position + new Vector3(randomCircle.x, 0, randomCircle.y), 1f);
                    yield return new WaitForSeconds(1f);
                    enteringStudent.transform.parent = transform;
                }

                if (CurrentFloorNumber == 1)
                {
                    //Get Students Destinations
                    List<int> targetFloors = new();
                    foreach (Student student in Occupants)
                    {
                        targetFloors.Add(student.targetFloor);
                    }
                    targetFloors = new List<int>(new HashSet<int>(targetFloors));
                    targetFloors.Sort();
                    Queue<int> visitingFloors = new(targetFloors);

                    //Move Up
                    while (visitingFloors.Count > 0)
                    {
                        int currentTarget = visitingFloors.Dequeue();
                        float travelDuration = 1f * Mathf.Abs(currentTarget - CurrentFloorNumber);
                        float travelTimer = 0f;
                        Vector3 oldPos = transform.position;
                        while (travelTimer < travelDuration)
                        {
                            travelTimer += Time.fixedDeltaTime;
                            transform.position = Vector3.Lerp(oldPos, liftManager.floors[currentTarget - 1].transform.position, 0.5f * (1 - Mathf.Cos(Mathf.PI * travelTimer / travelDuration)));
                            yield return new WaitForFixedUpdate();
                        }
                        CurrentFloorNumber = currentTarget;
                        OnLiftArrivedOnFloor.Invoke(CurrentFloorNumber);
                    }
                    Occupants.Clear();

                    //Move Down
                    int currentTargetDown = 1;
                    if (liftManager.floors[1].queue.Count > 0)
                    {
                        currentTargetDown = 2;
                    }
                    float travelDurationDown = 1f * Mathf.Abs(currentTargetDown - CurrentFloorNumber);
                    float travelTimerDown = 0f;
                    Vector3 oldPosDown = transform.position;
                    while (travelTimerDown < travelDurationDown)
                    {
                        travelTimerDown += Time.fixedDeltaTime;
                        transform.position = Vector3.Lerp(oldPosDown, liftManager.floors[currentTargetDown - 1].transform.position, 0.5f * (1 - Mathf.Cos(Mathf.PI * travelTimerDown / travelDurationDown)));
                        yield return new WaitForFixedUpdate();
                    }
                    CurrentFloorNumber = currentTargetDown;
                    OnLiftArrivedOnFloor.Invoke(CurrentFloorNumber);
                }
                else
                {
                    //Get down from 2 to 1
                    int currentTargetDown = 1;
                    float travelDurationDown = 1f * Mathf.Abs(currentTargetDown - CurrentFloorNumber);
                    float travelTimerDown = 0f;
                    Vector3 oldPosDown = transform.position;
                    while (travelTimerDown < travelDurationDown)
                    {
                        travelTimerDown += Time.fixedDeltaTime;
                        transform.position = Vector3.Lerp(oldPosDown, liftManager.floors[currentTargetDown - 1].transform.position, 0.5f * (1 - Mathf.Cos(Mathf.PI * travelTimerDown / travelDurationDown)));
                        yield return new WaitForFixedUpdate();
                    }
                    CurrentFloorNumber = currentTargetDown;
                    OnLiftArrivedOnFloor.Invoke(CurrentFloorNumber);
                }

                IsIdle = true;
            }
        }
        if (moving == null)
            StartCoroutine(LiftMoving());
    }
}
