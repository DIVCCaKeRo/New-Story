using UnityEngine;
using System.Collections;

public class move : MonoBehaviour
{
    float stopTime, moveTime, velX, velZ, timeCounter1, timeCounter2;
    float maxPosX = 0.3f, maxPosZ = 0.5f, minPosX = -0.3f, minPosZ = -0.5f;
    
    void Start(){
        Change();
    }

    void Update(){
        timeCounter1 += Time.deltaTime;
        if (timeCounter1 < moveTime)
            transform.Translate(velX, velZ, 0, Space.Self);
        else{
            timeCounter2 += Time.deltaTime;
            if (timeCounter2 > stopTime){
                Change();
                timeCounter1 = 0;
                timeCounter2 = 0;
            }
        }
        Check();
    }

    void Change(){
        stopTime = Random.Range(1, 3);
        moveTime = Random.Range(1, 20);
        velX = Random.Range(0.1f, 1.1f);
        velZ = Random.Range(0.1f, 1.1f);
    }

    void Check(){
        if (transform.localPosition.x > maxPosX){
            velX = -velX;
            transform.localPosition = new Vector3(maxPosX, 0, transform.localPosition.z);
        }
        if (transform.localPosition.x < minPosX){
            velX = -velX;
            transform.localPosition = new Vector3(minPosX, 0, transform.localPosition.z);
        }
        if (transform.localPosition.z > maxPosZ){
            velZ = -velZ;
            transform.localPosition = new Vector3(transform.localPosition.x, 0, maxPosZ);
        }
        if (transform.localPosition.z < minPosZ){
            velZ = -velZ;
            transform.localPosition = new Vector3(transform.localPosition.x, 0, minPosZ);
        }
    }
}