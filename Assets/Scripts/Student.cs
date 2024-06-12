using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Student : MonoBehaviour
{
    public int targetFloor = -1;

    //Move
    private Vector3 movePosition, lastPosition;
    private float moveDuration;

    //Time
    private DateTime startTime;

    private void Start()
    {
        startTime = DateTime.Now;
    }

    private void OnDestroy()
    {
        GameEvents.OnStudentReachClass.Publish((float)(DateTime.Now - startTime).TotalSeconds);
    }

    public void Move(Vector3 targetPosition, float duration)
    {
        StopCoroutine(MoveAction());
        lastPosition = transform.position;
        movePosition = targetPosition;
        moveDuration = duration;
        StartCoroutine(MoveAction());
    }

    private IEnumerator MoveAction()
    {
        float time = 0f;
        while (time < moveDuration)
        {
            time += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(lastPosition, movePosition, 0.5f * (1 - Mathf.Cos(Mathf.PI * time / moveDuration)));
            yield return new WaitForFixedUpdate();
        }
    }
}
