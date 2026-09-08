using System.Collections.Generic;
using UnityEngine;

public class DummyPeople : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private List<Transform> targetQueue = new List<Transform>();
    [Header("Movement & Rotation")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.5f;

    private int currentIndex = 0;
    private bool isLookingAtCamera = false;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }
    void Update()
    {   
        if (isLookingAtCamera)
        {
            LookAtCamera();
            return;
        }
        Transform currentTarget = GetCurrentTarget();
        if (currentTarget == null) 
        {
            return;
        }

        Vector3 targetPosition = GetFlatPosition(currentTarget.position);
        float distance = Vector3.Distance(transform.position, targetPosition);

        RotateTowards(targetPosition);

        if (distance > stoppingDistance)
        {
            MoveTowards(targetPosition);
        }
        else
        {
            AdvanceToNextTarget();
        }
    }
    protected virtual void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position,targetPosition,moveSpeed * Time.deltaTime);
    }
    private void LookAtCamera()
    {
        if (mainCam == null) 
        {
            return;
        }

        Vector3 cameraFlatPos = GetFlatPosition(mainCam.transform.position);
        RotateTowards(cameraFlatPos);
    }
    private Transform GetCurrentTarget()
    {
        if (targetQueue == null || targetQueue.Count == 0) 
        {
            return null;
        }

        Transform target = targetQueue[currentIndex];

        if (target == null)
        {
            AdvanceToNextTarget();
            return null;
        }

        return target;
    }
    private Vector3 GetFlatPosition(Vector3 worldPos)
    {
        return new Vector3(worldPos.x, transform.position.y, worldPos.z);
    }
    private void AdvanceToNextTarget()
    {
        currentIndex++;
        if (currentIndex >= targetQueue.Count)
        {
            isLookingAtCamera = true;
        }
    }
}
