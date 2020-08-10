using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionController : MonoBehaviour
{
    private bool useJobs;
    private void Start()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        useJobs = gameManager.useJobs;
    }

    void Update()
    {
        if (!useJobs)
        {
            var direction = GetComponent<MovementController>().direction;
            var position = GetComponent<MovementController>().transform.position;
            if ((direction.x == 1 && position.x > 10f)
            || (direction.x == -1 && position.x < -10f))
            {
                GetComponent<MovementController>().direction = direction * -1;
            }
        }
    }
}
