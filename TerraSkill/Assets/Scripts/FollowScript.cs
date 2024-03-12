using System.Collections;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Transform targetObj;
    public float sightRange = 10f;
    public bool isChasing = false;
    private float cooldownTimer = 0f;
    private float cooldownDuration = 0.6f;
    public float moveSpeed = 7.5f; //enemy movespeed
    public float rotSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0 && PlayerInSight())
        {
            isChasing = true;
            isWandering = false;
            StopAllCoroutines();
            ChasePlayer();
        }

        if (isWandering == false && !PlayerInSight())
        {
            StartCoroutine(Wander());
        }
        if (isRotatingRight == true && !isChasing)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if (isRotatingLeft == true && !isChasing)
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if (isWalking == true && !isChasing)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    bool PlayerInSight()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, targetObj.position);
        return distanceToPlayer <= sightRange;
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetObj.position, moveSpeed * Time.deltaTime);
        float distanceToPlayer = Vector3.Distance(transform.position, targetObj.position);
        if (distanceToPlayer <= 1f)
        {
            isChasing = false;
            cooldownTimer = cooldownDuration;
            timer += Time.deltaTime;
            if (timer >= 0.6)
            {
                isChasing = false;
            }
        }
    }

    IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(1, 4);
        int rotateLorR = Random.Range(1, 2);
        int walkWait = Random.Range(1, 5);
        int walkTime = Random.Range(1, 6);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }
}