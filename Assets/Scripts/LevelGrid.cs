using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGrid {


    // public Vector2Int foodGridPosition;
    // public GameObject foodGameObject;

    private int width;
    private int height;

    // private GameHandler gameHandler;
    private SnakeAgent snake;


    public LevelGrid(int width , int height) {
        this.width = width;
        this.height = height;
    }

    public void Setup(SnakeAgent snake) {
        this.snake = snake;
        // SpawnFood();
    }

    /*
    public void SpawnFood() {

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));

        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition) {
        if (snakeGridPosition == (Vector2)foodGameObject.transform.localPosition) {
            UnityEngine.Object.Destroy(foodGameObject);
            return true;
        } else {
            return false;
        }
    }
    */
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition) {
        if (gridPosition.x < -9) {
            // snake.state = SnakeAgent.State.Dead;
            //snake.SetReward(-1f);
            //snake.EndEpisode();
            // gridPosition.x = width;
        }
        if (gridPosition.x > width) {
            // snake.state = SnakeAgent.State.Dead;
            //snake.SetReward(-1f);
            //snake.EndEpisode();
            // gridPosition.x = -9;
        }
        if (gridPosition.y < -9) {
            // snake.state = SnakeAgent.State.Dead;
            //snake.SetReward(-1f);
            //snake.EndEpisode();
            // gridPosition.y = height;
        }
        if (gridPosition.y > height) {
            // snake.state = SnakeAgent.State.Dead;
            //snake.SetReward(-1f);
            //snake.EndEpisode();
            // gridPosition.y = -9;
        }
        return gridPosition;
    }

}