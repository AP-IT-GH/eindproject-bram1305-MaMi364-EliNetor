using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float moveDistance = 20f;
    public float speed = 5f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;

    private Transform player;
    private bool playerOnPlatform = false;

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector3.right * moveDistance;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (movingToEnd)
        {
            MovePlatform(endPosition);
        }
        else
        {
            MovePlatform(startPosition);
        }

        if (playerOnPlatform)
        {
            if (player != null)
            {
                player.parent = transform; // Als de speler op het platform maken we het een child object het platform zodat hij mee moved
            }
        }
    }

    void MovePlatform(Vector3 targetPosition)
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (transform.position == targetPosition)
        {
            movingToEnd = !movingToEnd;

            if (player != null)
            {
                player.parent = null; // Als hij weg jumped reversen we dit
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            if (player != null)
            {
                player.parent = null;
            }
        }
    }
}