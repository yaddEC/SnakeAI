using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Food:MonoBehaviour
{

    public void PlaceRandomly(Snake snake)
    {
        bool isPositionOccupied;
        Vector2 potentialPosition;

        do
        {
            isPositionOccupied = false;


            int x =(int) Random.Range(0, GameManager.gridSize.x);
            int y = (int)Random.Range(0, -GameManager.gridSize.y);
            potentialPosition = new Vector2(x, y);


            foreach (var segment in snake.bodyPos)
            {
                if (segment == potentialPosition)
                {
                    isPositionOccupied = true;
                    break;
                }
            }
        } while (isPositionOccupied);


        transform.position = new Vector3(potentialPosition.x, potentialPosition.y, 1);
    }
}

