using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/*
 *Script qui donne les tours aux protagonistes
 * 
 * auteur : Antoine Côté-L'Écuyer
 *
 *
 * Gestion de la valeur visuelle (slider) dans le menu combat statspersos: : tourTimer
 * 
 * auteur : Charles Noël
 */
//script collé sur tous les protagonistes
public class tourProtagoniste : MonoBehaviour
{
    public GameObject boutonAttaque;
    public bool mitSurAttaque = false;

    //Charles * Ref visuelle dans le menu comat
    public Slider timerSlider;

    GameObject auraPersoActif;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Si l'acteur est en combat et que ce n'est pas son tour
        if (VariablesGlobales.combat == true && gameObject.GetComponent<StatsPersos>().tourPerso == false && VariablesGlobales.victoire == false)
        {
            //la minuterie de tour du personnage est incrémentée avec le slider (l'image d'horloge)
            timerSlider.value = gameObject.GetComponent<StatsPersos>().tourTimer;
        }

        //si le perso actif est le personnage à qui ce script est collé
        if (VariablesGlobales.persoActif == gameObject && VariablesGlobales.victoire == false)
        {
            //active l'aura verte
            gameObject.transform.Find("auraPersoActif").gameObject.SetActive(true);
            //met ce personnage en premier plan
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;

            //le cadre de menu de combat du protagoniste devient rouge
            switch (gameObject.name)
            {
                case "Bob": GameObject.Find("CadrePJ1").GetComponent<Image>().color = Color.red;
                    break;

                case "Carl": GameObject.Find("CadrePJ2").GetComponent<Image>().color = Color.red;
                    break;

                case "Croc": GameObject.Find("CadrePJ3").GetComponent<Image>().color = Color.red;
                    break;

                default: break;
            }

            //met la selection à attaque
            if (mitSurAttaque == false)
            {
                EventSystem.current.SetSelectedGameObject(boutonAttaque);
                mitSurAttaque = true;
            }
        }
    }
}
