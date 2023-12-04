using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {

    public static GameHandler instance;

    private static int score;

    [SerializeField] private SnakeAgent snake;
    // [SerializeField] public GameObject food;

    private LevelGrid levelGrid;

    // public Vector2Int foodGridPosition;

    // private int width;
    // private int height;


    private void Awake() {
        instance = this;
    }

    private void Start() {

        levelGrid = new LevelGrid(9, 9);


        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        // levelGrid.foodGameObject.transform.SetParent(food.transform);
        // PositionFood();
    }

    /*
    public void Update()
    {
        bool snakeAteFood = levelGrid.TrySnakeEatFood(snake.gridPosition);
        if (snakeAteFood)
        {
            AddScore();
            // levelGrid.SpawnFood();
            // PositionFood();
        }

    }
    /*
    public void PositionFood()
    {
        levelGrid.foodGameObject.transform.SetParent(food.transform);

        do
        {
            foodGridPosition = new Vector2Int(UnityEngine.Random.Range(-9, width), UnityEngine.Random.Range(-9, height));
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);

        levelGrid.foodGameObject.transform.localPosition = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    */
    public static int GetScore() {
        return score;
    }

    public static void AddScore() {
        score++;
    }

}