using UnityEngine;
using System.Collections;

public class move : MonoBehaviour
{
    float stopTime, moveTime, velX, velZ, timeCounter1, timeCounter2;
    float maxPosX = 0.2f, maxPosZ = -0.1f, minPosX = -0.1f, minPosZ = -0.3f;
    public static  bool IsCollision=false;
    public GameObject explosion;
    Rigidbody rid ;
    Vector3 materialPos = new Vector3(0, 0, 0);
    public GameObject Exptemp;
    public GameObject material1, material2, material3;
    public GameObject handmaterial, building;
    GameObject CloneMaterial;
    void Start(){
        Change();
    }

    void Update(){
        if (!IsCollision)
        {
            timeCounter1 += Time.deltaTime;
            if (timeCounter1 < moveTime)
                transform.Translate(velX, velZ, 0, Space.Self);
            else
            {
                timeCounter2 += Time.deltaTime;
                if (timeCounter2 > stopTime)
                {
                    Change();
                    timeCounter1 = 0;
                    timeCounter2 = 0;
                }
            }
            Check();
        }
        else if (handmaterial.active == true)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, building.transform.position, 5f * Time.deltaTime);
            if(transform.position == building.transform.position)
            {
                building.SetActive(true);
                IsCollision = false;
                handmaterial.SetActive(false);
            }
            

        }
        else
        {
            Vector3 centerPos=new Vector3();
  
            if (material1.active == true)
            {
                centerPos = material1.transform.position;
              
            }
            if (material2.active == true)
            {
                centerPos = material2.transform.position;
            
            }
            if (material3.active == true)
            {
                centerPos = material3.transform.position;
        
            }

            transform.position = Vector3.MoveTowards(transform.position, centerPos, 5f * Time.deltaTime);
            if (transform.position == centerPos)
            {
                //IsCollision = false;
                rid.isKinematic = false;
                Destroy(CloneMaterial);
                Destroy(Exptemp);
                handmaterial.SetActive(true);
                material1.SetActive(false);
                material2.SetActive(false);
                material3.SetActive(false);
            }
            
        }
        



    }

    void movetobuild()
    {

    }

    void OnCollisionEnter(Collision hand)
    {  
        if (hand.gameObject.name == "bone3"&&( material1.active == true || material2.active == true || material3.active == true))
        {  
            if(!IsCollision)
            {
                Exptemp = (GameObject)Instantiate(explosion, transform.position, Quaternion.identity);
                rid = gameObject.GetComponent<Rigidbody>();
                rid.isKinematic = true;
                //materialPos.x = Random.Range(-8f, 8f);
                //materialPos.z = Random.Range(-14f, 0.0f);
                //CloneMaterial = (GameObject)Instantiate(material, materialPos, Quaternion.identity);
            }
           // print(materialPos.x+ " " + materialPos.z);

            IsCollision = true;
        }
            

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
            transform.localPosition = new Vector3(maxPosX,Mathf.Clamp(transform.localPosition.y,-0.015f, 2.0f) , transform.localPosition.z);
        }
        if (transform.localPosition.x < minPosX){
            velX = -velX;
            transform.localPosition = new Vector3(minPosX, Mathf.Clamp(transform.localPosition.y, -0.015f, 2.0f), transform.localPosition.z);
        }
        if (transform.localPosition.z > maxPosZ){
            velZ = -velZ;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, -0.015f, 2.0f), maxPosZ);
        }
        if (transform.localPosition.z < minPosZ){
            velZ = -velZ;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, -0.015f, 2.0f), minPosZ);
        }
    }
}