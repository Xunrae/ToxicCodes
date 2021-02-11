using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using UnityEngine;
/*
 *Script qui contrôle les mouvements du personnage principal
 *          les personnages secondaires suivent le principal à une certaine distance
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
public class BobScriptDeplacement : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rbBob;

    StatsPersos statsBob;
    CollisionEvenement collisionsScript;

    //parametres pour l'animator (on utilise le nom isIdle depuis la premiere session donc on se permet de le garder)
    bool isIdle = true;

    //bool pour permettre de changer une seule fois les parametres du Animator
    public bool animationsCombat = false;

    //forces
    public float vx;
    public float vy;
    public float vxMax = 5;
    public float vyMax = 5;
    //forces appliquées aux equipiers
    public float vitesseEquipiers = 5;

    //positions des personnages alliés par rapport au perso
    public Vector3 posPerso;
    public Vector3 posEquipier1;
    public Vector3 posEquipier2;
    Equipe equipe;


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        equipe = GetComponentInParent<Equipe>();


        rbBob = GetComponent<Rigidbody2D>();

        statsBob = GetComponent<StatsPersos>();

        collisionsScript = GetComponent<CollisionEvenement>();
    }


    // Update is called once per frame
    void Update()
    {
        //si il n'y a pas de combat
        if (VariablesGlobales.combat == false) {
            posPerso = gameObject.transform.position;
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                vxMax = 7;
                vyMax = 7;
            }

            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                vxMax = 5;
                vyMax = 5;
            }
                            //deplacement horizontal vers la gauche
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                vx = -vxMax;//vit. imposée à Bob

                //positions des equipiers quand le personnage va vers la gauche
                posEquipier1 = posPerso - new Vector3(-2, 0, 0);
                posEquipier2 = posEquipier1 - new Vector3(-2, 0, 0);

                //Si le joueur pèse sur les boutons haut/bas
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    vy = vyMax;

                    //positions des equipiers quand le personnage va vers la gauche + haut
                    posEquipier1 = posPerso - new Vector3(-2, 3, 0);
                    posEquipier2 = posEquipier1 - new Vector3(-2, 2, 0);
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    vy = -vyMax;

                    //positions des equipiers quand le personnage va vers la gauche + bas
                    posEquipier1 = posPerso - new Vector3(-2, -3, 0);
                    posEquipier2 = posEquipier1 - new Vector3(-2, -2, 0);
                }
                else { vy = 0; }

                isIdle = false;

                //change Bob de côté
                GetComponent<SpriteRenderer>().flipX = true;
                //Change Carl de côté
                equipe.equipiers[1].GetComponent<SpriteRenderer>().flipX = false;
                //Change Croc de côté
                equipe.equipiers[2].GetComponent<SpriteRenderer>().flipX = true;
            }

            //deplacement horizontal vers la droite
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                vx = vxMax;//vit. imposée à Bob


                //positions des equipiers quand le personnage va vers la droite
                posEquipier1 = posPerso - new Vector3(2, 0, 0);
                posEquipier2 = posEquipier1 - new Vector3(2, 0, 0);

                //Si le joueur pèse sur les boutons haut/bas
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    vy = vyMax;

                    //positions des equipiers quand le personnage va vers la droite + haut
                    posEquipier1 = posPerso - new Vector3(2, 3, 0);
                    posEquipier2 = posEquipier1 - new Vector3(2, 2, 0);
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    vy = -vyMax;

                    //positions des equipiers quand le personnage va vers la droite + bas
                    posEquipier1 = posPerso - new Vector3(2, -3, 0);
                    posEquipier2 = posEquipier1 - new Vector3(2, -2, 0);
                }
                else {
                    vy = 0;
                }

                isIdle = false;

                //change Bob de côté
                GetComponent<SpriteRenderer>().flipX = false;
                //Change Carl de côté
                equipe.equipiers[1].GetComponent<SpriteRenderer>().flipX = true;
                //Change Croc de côté
                equipe.equipiers[2].GetComponent<SpriteRenderer>().flipX = false;
            }

            //deplacement vertical vers le haut
            else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                vy = vyMax;//vit. imposée à Bob
                vx = 0;
                isIdle = false;

                //positions des equipiers quand le personnage va vers le haut
                posEquipier1 = posPerso - new Vector3(0, 2, 0);
                posEquipier2 = posEquipier1 - new Vector3(0, 2, 0);

            }

            //deplacement vertical vers le bas
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                vy = -vyMax;//vit. imposée à Bob
                vx = 0;
                isIdle = false;

                //positions des equipiers quand le personnage va vers le bas
                posEquipier1 = posPerso - new Vector3(0, -2, 0);
                posEquipier2 = posEquipier1 - new Vector3(0, -2, 0);

            }

            //Si on ne touche à rien le personnage ne se déplace pas
            else
            {
                vx = vy = 0;
                isIdle = true;
            }

            //velocité du personnage devient celle définie par les touches
            rbBob.velocity = new Vector2(vx, vy);

            equipe.equipiers[1].GetComponent<Rigidbody2D>().velocity = (posEquipier1 - equipe.equipiers[1].transform.position) * vitesseEquipiers;
            equipe.equipiers[2].GetComponent<Rigidbody2D>().velocity = (posEquipier2 - equipe.equipiers[2].transform.position) * vitesseEquipiers;

            //vx devient un nombre absolu pour utiliser dans animator
            vx = Math.Abs(vx);

            foreach(GameObject personnage in equipe.equipiers)
            {
                personnage.GetComponent<Animator>().SetFloat("Vx", vx);
                personnage.GetComponent<Animator>().SetFloat("Vy", vy);
                personnage.GetComponent<Animator>().SetBool("isIdle", isIdle);
                personnage.GetComponent<Animator>().SetBool("estMort", false);
            }
        }
        //si les personnages sont en combat, on veut mettre tout le monde en "Idle" d'un coup et ne pas leur dire d'etre en Idle 60 fois par seconde
        else if(VariablesGlobales.combat == true && animationsCombat == false)
        {
            animationsCombat = true;
            personnagesEnCombat();
        }
    }

    //fonction qui change les animators de tous les equipiers pour le combat
    void personnagesEnCombat()
    {
        foreach (GameObject personnage in equipe.equipiers)
        {
            personnage.GetComponent<Animator>().SetFloat("Vx", 0f);
            personnage.GetComponent<Animator>().SetFloat("Vy", 0f);
            personnage.GetComponent<Animator>().SetBool("isIdle", true);
            personnage.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
