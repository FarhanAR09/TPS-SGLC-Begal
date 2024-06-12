using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftManager : MonoBehaviour
{
    [SerializeField]
    private Lift lift;
    [SerializeField]
    public List<Floor> floors;

    private void OnEnable()
    {
        lift.OnLiftArrivedOnFloor.AddListener(NotifyFloorOnLiftArrival);
    }

    private void OnDisable()
    {
        lift.OnLiftArrivedOnFloor.RemoveListener(NotifyFloorOnLiftArrival);
    }

    private void NotifyFloorOnLiftArrival(int floorNumber)
    {
        floors[floorNumber - 1].EnterLift(lift);
    }
}
