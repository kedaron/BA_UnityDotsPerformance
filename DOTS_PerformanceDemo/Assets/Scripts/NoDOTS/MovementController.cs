using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float movementSpeed;
    private bool doPseudoCalculations;
    
    void Start()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        doPseudoCalculations = gameManager.doPseudoCalculations;
        movementSpeed = UnityEngine.Random.Range(1.0f, 6.0f);
    }

    void Update()
    {
        var direction = GetComponent<DirectionController>().direction;
        transform.position += direction * movementSpeed * Time.deltaTime;
        // Pseudo calculations
        if (doPseudoCalculations)
        {
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int drop = i * j;
                }
            }
        }
    }
}
