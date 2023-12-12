using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class Food : MonoBehaviour {

    public Vector2Int foodGridPosition;
    public SnakeAgent snake;

    private void Start() {
         RandomizePosition();
    }

    private void RandomizePosition() {

        do {
            foodGridPosition = new Vector2Int(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4));
                // Debug.Log("FOOD: " + foodGridPosition);
        } while (snake.GetAllSegmentsPosition().IndexOf(foodGridPosition) != -1);
        
        transform.localPosition = new Vector3(foodGridPosition.x, foodGridPosition.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            RandomizePosition();
        }
    }
}
