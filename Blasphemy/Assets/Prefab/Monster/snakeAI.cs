﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snakeAI : MonoBehaviour
{
    public string NAME;
    public string type;
    public int ATK;
    public int HP;
    public int DEF;
    public float mySpeed;

    public bool inSight;
    private Ouros Ouros;
    public float sightX;
    public float sightY;
    [SerializeField]
    private int randomed;
    public GameObject poison;
    public GameObject dotArea;
    private float timecount = 0.0f;
    public float delay = 3.0f;
    private int poisonCount = 0;
    public Transform spawnPoint;
    public bool dotSpawned = false;
    private Rigidbody2D rid2d;
    private Animator animator;
    //public BoxCollider2D meleeCollider;

    public float faceDelay;
    private float faceCount;
    public int direction = 1;

    public bool red = false;
    public bool isInvisible;
    public bool isDead;
    public float deadCount;
    public float deadTime;
    private float invicount;
    public float invitime;
    public Color inviColor;
    public Color normalColor;
    public Color damagedColor;

    public int _currentAnimationState = 0;
    public void changeState(int stateI)
    {
        if (_currentAnimationState == stateI)
        {
            return;
        }
        else
        {
            animator.SetInteger("state", stateI);
            _currentAnimationState = stateI;
        }
    }
    // Use this for walkDurationialization
    void Start()
    {
        Ouros = FindObjectOfType<Ouros>();
        rid2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        poisonCount = 0;
        timecount = 0;
    }

    void facePlayer()
    {
        faceCount += Time.deltaTime;
        if (faceCount > faceDelay)
        {

            if (Ouros.GetComponent<Transform>().position.x < this.GetComponent<Transform>().position.x)
            {
                direction = 1;
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Ouros.GetComponent<Transform>().position.x > this.GetComponent<Transform>().position.x)
            {
                direction = -1;
                GetComponent<SpriteRenderer>().flipX = false;
            }
            faceCount = 0;
        }

    }

    void checkInsight()
    {
        if (Ouros.GetComponent<Transform>().position.y < this.GetComponent<Transform>().position.y + sightY)
        {
            if (Ouros.GetComponent<Transform>().position.y > this.GetComponent<Transform>().position.y - sightY)
            {
                if (Ouros.GetComponent<Transform>().position.x > this.GetComponent<Transform>().position.x - sightX)
                {
                    if (Ouros.GetComponent<Transform>().position.x < this.GetComponent<Transform>().position.x + sightX)
                    {
                        inSight = true;
                        return;
                    }
                }
            }
        }
        inSight = false;
    }
   
    void EnemyAction()
    {
        //facePlayer();

        if (poisonCount == 0 || dotSpawned == true)
        {
            randomed = Random.Range(1, 4);
        }

        if (randomed == 1)
        {
            timecount += Time.deltaTime;
            // bool dotSpawned = false;
            if (timecount >= delay)
            {
                if (dotArea != null && dotSpawned == false)
                {
                    //changestate
                    Vector3 theNewDirection = new Vector3(Ouros.transform.position.x, Ouros.transform.position.y + 5.0f, Ouros.transform.position.z);
                    Instantiate(dotArea, theNewDirection, spawnPoint.transform.rotation);

                    //timecount -= 0.5f;
                    dotSpawned = true;
                    timecount = 0.0f;
                }
            }
        }
         else if (randomed>1)
        {
            timecount += Time.deltaTime;
            if (timecount >= delay && poisonCount<3)
            {
                if (poison != null)
                {
                    //change state
                    Instantiate(poison, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    
                    poisonCount++;
                    timecount = 0.0f;
                }
            }
        }
    }

   
    public void blinking()
    {
        if (isInvisible == true)
        {

            if (invicount > invitime - (invitime*80 / 100) && red == false)
            {
                if (GetComponent<SpriteRenderer>().material.color == damagedColor)
                {
                    GetComponent<SpriteRenderer>().material.SetColor("_Color", normalColor);
                    red = true;
                }
            }
            invicount += Time.deltaTime;
            if (red == true)
            {
                if (invicount > invitime)
                {
                    invicount = 0;
                    red = false;
                    isInvisible = false;
                    GetComponent<SpriteRenderer>().material.SetColor("_Color", normalColor);
                    return;
                }

                if (GetComponent<SpriteRenderer>().material.color == inviColor)
                {
                    GetComponent<SpriteRenderer>().material.SetColor("_Color", normalColor);
                }
                else if (GetComponent<SpriteRenderer>().material.color == normalColor)
                {
                    GetComponent<SpriteRenderer>().material.SetColor("_Color", inviColor);
                }
            }


        }
    }
    public void knockBack(float dis)
    {
        GetComponent<SpriteRenderer>().material.SetColor("_Color", damagedColor);
        if (GetComponent<SpriteRenderer>().flipX == true)
        {
            // this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.right * dis);
            transform.Translate(Vector3.right * dis);
        }
        else if (GetComponent<SpriteRenderer>().flipX == false)
        {
            // this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.left * dis);
            transform.Translate(Vector3.left * dis);
        }

        isInvisible = true;
    }
    void checkDie()
    {
        if (this.HP <= 0)
        {
            isDead = true;
        }
    }
    void die()
    {
        if (isDead == true)
        {
            deadCount += Time.deltaTime;
            if (deadCount > deadTime)
            {
                Destroy(gameObject);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerWeapon")
        {
            HP -= Ouros.ATK - this.DEF / Ouros.ATK;
            knockBack(0.0f);

        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update ()
    {
        checkInsight();
        checkDie();
        die();
        blinking();
        if (inSight == true)
        {
            EnemyAction();
        }
        if(poisonCount >= 3 || dotSpawned ==true)
        {
            dotSpawned = false;
            poisonCount = 0;
            timecount = 0.0f;
        }
    }
}
