using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    #region Player Stats
    public int health = 1;
    public float speed;
    public float jumpHeight;
    public float platDisty;
    public float platDistx;
    public float enemyDistx;
    public float knockback;
    #endregion
    #region GC and RB
    public Rigidbody2D rb;
    public GameObject GameController;
    #endregion
    #region Bools
    [HideInInspector] public bool jumping = false;
    [HideInInspector] public bool aPress = false;
    [HideInInspector] public bool dPress = false;
    [HideInInspector] public bool jPress = false;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (GameController == null)
        {
            GameController = GameObject.Find("Game Controller");
        }
        #region Player Input Checker
        if (Input.GetKey("a"))
        {
            aPress = true;
        }
        else { aPress = false; }
        if (Input.GetKey("d"))
        {
            dPress = true;
        }
        else { dPress = false; }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            aPress = true;
        }
        else { aPress = false; }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dPress = true;
        }
        else { dPress = false; }

        if (!jumping)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                jPress = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameController.GetComponent<Game_Controller>().red = !GameController.GetComponent<Game_Controller>().red;
        }
        #endregion
    }

    void FixedUpdate()
    {
        #region Movement Code
        float hAdd = 0f;

        Vector3 jump = new Vector3(0, jumpHeight, 0);

        if (dPress == true) { hAdd += speed; }
        if (aPress == true) { hAdd -= speed; }

        rb.velocity = new Vector2(hAdd, rb.velocity.y);

        if (jPress) {
            rb.AddForce(jump);
            jPress = false;
            jumping = true;
        }
        #endregion
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Platform"))
        {
            if (collider.gameObject.transform.position.y - transform.position.y < platDisty && Mathf.Abs(collider.gameObject.transform.position.x - transform.position.x) < platDistx)
            {
                jumping = false;
            }
        }
        if (collider.gameObject.CompareTag("Floor"))
        {
            jumping = false;
        }

        if (collider.gameObject.CompareTag("Enemy"))
        {
            if (collider.gameObject.transform.position.y - transform.position.y < platDisty && Mathf.Abs(collider.gameObject.transform.position.x - transform.position.x) < enemyDistx)
            {
                Vector3 jump = new Vector3(0, jumpHeight * 2, 0);
                rb.AddForce(jump);
            }
            else
            {
                if (collider.gameObject.transform.position.y - transform.position.y < platDisty && Mathf.Abs(collider.gameObject.transform.position.x - transform.position.x) >= enemyDistx)
                {
                    rb.AddForce(new Vector3(-Mathf.Clamp01(collider.gameObject.transform.position.x - transform.position.x) * knockback, knockback/2,0));
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Spike"))
        {
            Instantiate(GameController.GetComponent<Game_Controller>().PlayerPreFab, GameController.GetComponent<Game_Controller>().curCheckPoint.transform.position ,this.transform.rotation);
            Destroy(this.gameObject);
        }

        if (col.gameObject.CompareTag("Checkpoint"))
        {
            GameController.GetComponent<Game_Controller>().curCheckPoint = col.gameObject;
        }
    }
}