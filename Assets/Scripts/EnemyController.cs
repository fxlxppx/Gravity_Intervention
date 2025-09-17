using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform[] waypoints;
    public string waypointsRootName = "Waypoints";
    public float speed = 2f;
    public float waitTime = 0.8f;
    public bool loop = true;
    [Tooltip("Se true o inimigo irá teleportar para o primeiro waypoint no Start")]
    public bool snapToFirstWaypoint = false;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private Vector3[] points;
    private int currentIndex = 0;
    private bool waiting = false;
    private Coroutine waitCoroutine;

    private void Awake()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            points = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                points[i] = waypoints[i].position;
            }
        }
        else
        {
            points = new Vector3[1];
            points[0] = transform.position;
        }
    }

    private void Start()
    {
        currentIndex = 0;
        if (snapToFirstWaypoint && points.Length > 0)
        {
            transform.position = points[0];
        }
    }

    private void Update()
    {
        if (waiting || points.Length == 0) return;

        Vector3 target = points[currentIndex];

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (spriteRenderer != null)
        {
            if (target.x > transform.position.x) spriteRenderer.flipX = false;
            else if (target.x < transform.position.x) spriteRenderer.flipX = true;
        }

        if (Vector3.Distance(transform.position, target) < 0.03f)
        {
            AdvanceToNext();
        }
    }

    private void AdvanceToNext()
    {
        if (waitTime > 0f)
        {
            if (waitCoroutine != null) StopCoroutine(waitCoroutine);
            waitCoroutine = StartCoroutine(WaitAndAdvance());
        }
        else
        {
            IncrementIndex();
        }
    }

    private IEnumerator WaitAndAdvance()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        IncrementIndex();
        waiting = false;
        waitCoroutine = null;
    }

    private void IncrementIndex()
    {
        currentIndex++;
        if (currentIndex >= points.Length)
        {
            if (loop) currentIndex = 0;
            else
            {
                currentIndex = points.Length - 1;
                enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            if (loop && waypoints.Length > 1 && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
