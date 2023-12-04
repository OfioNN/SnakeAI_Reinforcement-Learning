using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class SnakeAgent : Agent {

    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    public enum State {
        Alive,
        Dead
    }

    public List<Transform> segments;
    public Transform segmentPrefab;

    public int initialSize = 4;

    private int move = 3;

    public State state;

    private Direction gridMoveDirecrion;
    public Vector2Int gridPosition;

    private float gridMoveTimer;
    private float gridMoveTimerMax;

    private LevelGrid levelGrid;

    public static SnakeAgent instance;

    public GameHandler gameHandler;
    [SerializeField] private Transform food;

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
    }

    private void Awake() {

        instance = this;

        gridPosition = new Vector2Int(0, 0);
        gridMoveTimerMax = 0.2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirecrion = Direction.Right;

        state = State.Alive;

    }
    private void Start() {
        // segments = new List<Transform>();
        // segments.Add(transform);

    }
    public override void OnEpisodeBegin() {

        gridPosition = new Vector2Int(0, 0);
        gridMoveTimerMax = 0.2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirecrion = Direction.Right;
        move = 3;

        state = State.Alive;

        segments = new List<Transform>();
        segments.Add(transform);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(food.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions) {

        switch (actions.DiscreteActions[0]) {
            case 0: {
                    if (gridMoveDirecrion != Direction.Down) {
                        gridMoveDirecrion = Direction.Up;               // W
                    }
                    break;
                }
            case 1: {
                    if (gridMoveDirecrion != Direction.Up) {
                        gridMoveDirecrion = Direction.Down;             // S
                    }
                    break;
                }
            case 2: {
                    if (gridMoveDirecrion != Direction.Right) {
                        gridMoveDirecrion = Direction.Left;             // A
                    }
                    break;
                }
            case 3: {
                    if (gridMoveDirecrion != Direction.Left) {
                        gridMoveDirecrion = Direction.Right;            // D
                    }
                    break;
                }
        }


    }

    public override void Heuristic(in ActionBuffers actionsOut) {

        if (Input.GetKey(KeyCode.W)) move = 0;
        if (Input.GetKey(KeyCode.S)) move = 1;
        if (Input.GetKey(KeyCode.A)) move = 2;
        if (Input.GetKey(KeyCode.D)) move = 3;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = move;

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
    private float GetAngleFromVector(Vector2Int dir) {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private void HandleGridMovement() {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;

            // Debug.Log("Snake: " + gridPosition);

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
            /*
            for (int i = segments.Count - 1; i > 0; i--) {
                segments[i].localPosition = segments[i - 1].localPosition;
            }
            */
            transform.localPosition = new Vector3(gridPosition.x, gridPosition.y);
            transform.localEulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

        }
    }
    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    private void Grow() {
        if (segments.Count <= 1) {
            segmentPrefab.GetComponent<BoxCollider2D>().enabled = false;
        }

        Transform segment = Instantiate(this.segmentPrefab, gameHandler.transform);
        

        segment.position = segments[segments.Count - 1].position;


        segments.Add(segment);


        Invoke("EnableSegmentBox", gridMoveTimerMax += Time.deltaTime);
    }

    private void EnableSegmentBox() {
        segmentPrefab.GetComponent<BoxCollider2D>().enabled = true;

    }

    private void RestartGame() {
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(transform);
        gridPosition = new Vector2Int(0, 0);

        EndEpisode();


        /*
        for (int i = 1; i < this.initialSize; i++) {
            segments.Add(Instantiate(this.segmentPrefab, gameHandler.transform));
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent<Food>(out Food food)) {
            Debug.Log("SCORE");
            SetReward(1);
            // Grow();
            EndEpisode();
        }

        if (collision.TryGetComponent<Wall>(out Wall wall)) {
            Debug.Log("DEAD");
            AddReward(-1);
            EndEpisode();
        }
    }
    /*
    public List<Vector2Int> GetFullSnakeGridPositionList() {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(segments);
        return gridPositionList;
    }
    */
    
}
