using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using JetBrains.Annotations;
using static UnityEngine.Rendering.HableCurve;

public class SnakeAgent : Agent {

    // Kierunki
    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    // Stany ¿ycia
    public enum State {
        Alive,
        Dead
    }

    // Lista czêœci wê¿a i Prefab ogona
    public List<Transform> segments;
    public Transform segmentPrefab;

    // Poruszanie siê wê¿a i jego pozycja
    public Vector2Int gridPosition;
    private Direction gridMoveDirecrion;
    private int move;

    private bool makeMove = false;

    // Stan w jakim siê znajduje i czy mo¿e siê poruszaæ
    public State state;

    // Czas co ile w¹¿ siê porusza
    private float gridMoveTimer;
    private float gridMoveTimerMax;

    // Odwo³ania do innych skryptów
    private LevelGrid levelGrid;
    public GameHandler gameHandler;

    // Pozycja celu do zabrania
    [SerializeField] private Transform food;

    // Funkcja pobieraj¹ca skrypt levelGrid
    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
    }

    // Na starcie

    public override void OnEpisodeBegin() {

        gridPosition = new Vector2Int(0, 0);
        gridMoveDirecrion = Direction.Right;
        move = 3;

        gridMoveTimerMax = 0.1f;
        gridMoveTimer = gridMoveTimerMax;

        state = State.Alive;

        segments = new List<Transform>(); 
        segments.Add(this.transform);

    }

    private void Update() {
        switch (state) {
            case State.Alive:
                HandleGridMovement();
                break;
            case State.Dead:
                Debug.Log("Dead");
                AddReward(-1f);
                EndEpisode();
                break;
        }
    }

    // Zbieranie informacji
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(food.transform.localPosition);
    }

    // Podejmowanie akcji przez Agenta
    public override void OnActionReceived(ActionBuffers actions) {

        switch (actions.DiscreteActions[0]) {

            case 0: {
                    if (gridMoveDirecrion != Direction.Down && makeMove == false) {
                        gridMoveDirecrion = Direction.Up;               // W
                        makeMove = true;

                    }
                    break;
                }
            case 1: {
                    if (gridMoveDirecrion != Direction.Up && makeMove == false) {
                        gridMoveDirecrion = Direction.Down;             // S
                        makeMove = true;

                    }
                    break;
                }
            case 2: {
                    if (gridMoveDirecrion != Direction.Right && makeMove == false) {
                        gridMoveDirecrion = Direction.Left;             // A
                        makeMove = true;

                    }
                    break;
                }
            case 3: {
                    if (gridMoveDirecrion != Direction.Left && makeMove == false) {
                        gridMoveDirecrion = Direction.Right;            // D
                         // canMove = false;
                        makeMove = true;
                         // Invoke(nameof(WaitOneSecond), 0.001f);

                    }
                    break;
                }
        }
    }

    // Testowanie przez u¿ytkownika
    public override void Heuristic(in ActionBuffers actionsOut) {

        if (Input.GetKey(KeyCode.W)) move = 0;
        if (Input.GetKey(KeyCode.S)) move = 1;
        if (Input.GetKey(KeyCode.A)) move = 2;
        if (Input.GetKey(KeyCode.D)) move = 3;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = move;

    }

    // Obraca wê¿a w odpowiedni¹ stronê
    private float GetAngleFromVector(Vector2Int dir) {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    // Wywo³ywanie w funkcji Update() co jedn¹ klatkê 
    private void HandleGridMovement() {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax) {

            gridMoveTimer -= gridMoveTimerMax;
            Vector2Int gridMoveDirectionVector;


            switch (gridMoveDirecrion) {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }


            gridPosition += gridMoveDirectionVector;

            // gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            for (int i = segments.Count - 1; i > 0; i--) {
                segments[i].localPosition = segments[i - 1].localPosition;
            }

            transform.localPosition = new Vector3(gridPosition.x, gridPosition.y);
            transform.localEulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

            makeMove = false;

        }
    }

    public void WaitOneSecond() {
        // canMove = true;
    }

    // Pobierz pozycjê g³owy wê¿a
    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    private void InitialSize() {
        Transform initialSegments = Instantiate(this.segmentPrefab, gameHandler.transform);
        initialSegments.position = segments[segments.Count - 1].position;
        segments.Add(initialSegments);
    }

    // Dodawanie kolejnych czêœci ogona
    private void Grow() {
        // Kopiuj Prefab ogona, ustaw rodzica i zapisz do zmiennej segment
        Transform segment = Instantiate(this.segmentPrefab, gameHandler.transform);

        // Je¿eli Lista jest mniejsza, albo równa 1, wy³¹cz kolizjê na Prefabie (g³owa = 1, pierwsza czêœæ ogona = 2 elemety)
        /*
        if (segments.Count <= 1) {
            segment.GetComponent<BoxCollider2D>().enabled = false;
        }
        */
        segment.position = segments[segments.Count - 1].position; // Ustawienie pozycji kopii Prefaba na ostatni¹ pozycjê

        
        segments.Add(segment); // Dodanie czêœci do Listy (segments)

        // Wywo³anie funkcji EnableSegmentBox po czasie gridMoveTimer * Time.deltaTime --> 0.25f * 0.01f = czas w sekundach
        // --> Time.deltaTime = 1/FPS (np.1/100 = 0.01s) 

        // Invoke(nameof(EnableSegmentBox), gridMoveTimerMax * Time.deltaTime);
        
    }


    // Restartowanie gry
    private void RestartGame() {
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(transform);

        EndEpisode();

        /*
        for (int i = 1; i < this.initialSize; i++) {
            segments.Add(Instantiate(this.segmentPrefab, gameHandler.transform));
        }
        */
    }


    // Kolizje  
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent<Food>(out Food food)) {
            Debug.Log("SCORE");
            AddReward(1);
            Grow();
        }

        else if (collision.TryGetComponent<Wall>(out Wall wall)) {
            Debug.Log("WALL");
            AddReward(-1);
            RestartGame();
        }
    }
}
