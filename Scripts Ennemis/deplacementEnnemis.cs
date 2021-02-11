using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using UnityEngine;
/*
 *Script de déplacement des ennemis
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
//Script collé sur chaque ennemi actif de la scène (qui sont présents en dehors des combats)
public class deplacementEnnemis : MonoBehaviour
{
    //l'objet qui doit être détecté pour le déplacement
    public GameObject Bob;

    //distance entre l'objet et l'ennemi
    float distance;

    //la destination de l'ennemi
    Vector3 destination;

    //position originale de l'ennemi
    Vector3 posInit;

    //variables pour les animations
    float vx;
    float vy;
    bool isIdle = false;
    Animator animEnnemi;

    // Use this for initialization
    void Start()
    {
        posInit = gameObject.transform.position;

        animEnnemi = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //calcule la distance entre ennemi et personnage
        distance = Vector3.Distance(Bob.transform.position, gameObject.transform.position);

        //en dessous d'un seuil de distance et si le jeu n'est pas en mode combat et que le joueur n'est pas en fuite
        if (distance <= 5f && VariablesGlobales.combat == false && VariablesGlobales.fuiteCombat == false)
        {
            destination = Bob.transform.position;
            isIdle = false;
            //la velocité de l'ennemi est dirigée vers le personnage principal de l'equipe
            gameObject.GetComponent<Rigidbody2D>().velocity = (destination - gameObject.transform.position );

            vx = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            vy = gameObject.GetComponent<Rigidbody2D>().velocity.y;
            if (vx < 0)
            {
                //Change l'ennemi de côté
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else
        {
            isIdle = false;
            //calcule la distance entre ennemi et sa position initiale
            distance = Vector3.Distance(posInit, gameObject.transform.position);
            
            //destination devient sa position initiale
           destination = posInit;

            //la velocité de l'ennemi est dirigée vers sa position initiale
           gameObject.GetComponent<Rigidbody2D>().velocity = (destination - gameObject.transform.position);

            //sauvegarde la velocité x et y
            vx = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            vy = gameObject.GetComponent<Rigidbody2D>().velocity.y;

            if (vx <0)
            {
                //Change l'ennemi de côté
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
        }

        //si ses valeurs de vélocité sont très petites, il est en Idle
        if(vy <= 0.1f && System.Math.Abs(vx) <= 0.2f)
        {
           isIdle = true;
        }

        //valeur absolue vx pour son animator
        vx = System.Math.Abs(gameObject.GetComponent<Rigidbody2D>().velocity.x);
        vy = gameObject.GetComponent<Rigidbody2D>().velocity.y;

        //change les valeurs de son animator
        animEnnemi.SetBool("isIdle", isIdle);
        animEnnemi.SetFloat("Vx", vx);
        animEnnemi.SetFloat("Vy", vy);
    }
}
