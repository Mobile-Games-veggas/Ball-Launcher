using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSprintJoint;

    private Camera mainCamera; //нужно для метода, который конвертирует координаты
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable() 
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable() 
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (currentBallRigidbody == null) { return; }

        if(Touch.activeTouches.Count == 0) //если приконснулись
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            return;
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition); //конвертация координат

        currentBallRigidbody.position = worldPosition; //мяч становится туда, куда прикоснулись
    }

    private void SpawnNewBall() //созданеи нового мяча
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>(); //привязка компонентов к созданному объекту
        currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSprintJoint.connectedBody = pivot; //установка параметра "к кому прикрепляться" компонента
    }

    private void LaunchBall() //запуск мяча
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null; // подчистили ссылку

        Invoke(nameof(DetachBall), detachDelay); //отключили компонент через время
    }

    private void DetachBall() //отсоединение мяча
    {
        currentBallSprintJoint.enabled = false;
        currentBallSprintJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay); //создание нового мяча спустя время
    }
}
