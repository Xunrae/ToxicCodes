using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/*
 *Script qui permet aux protagonistes d'attaquer
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
//script collé sur l'objet gestionCombat

public class attaque : MonoBehaviour
{
    //le dommage qui sera infligé
    int dommage;
    //le dommage de base du personnage
    float baseDommage;

    //dommage minimum/maximum pour calcul
    float minDommage;
    float maxDommage;

    //le protagoniste à qui c'est le tour
    GameObject personnageActif;

    //est-ce que le perso est en attente de son attaque?
    bool persoVeutAttaquer = false;
    int laCible;

    //la section qui contient les cibles
    public GameObject lesCibles;

    //est-ce que le bouton a déjà été appuyé? debug
    bool boutonPeser = false;

    // Auteur: Charles Noel, appellation de son ponctuel de d'attaque des protagonistes et le son "Hurt" des ennemis selon le script GestionSonEnnemi.
    AudioSource audioAttaque; // source Gestionnaire Attaque
    public AudioClip sonAttaqueBob; // son Attaque Bob
    public AudioClip sonAttaqueCarl; // son Attaque Carl
    public AudioClip sonAttaqueCroc; // son Attaque Croc

    public AudioClip sonHurtEnnemi; // son Hurt Ennemi
    public AudioClip sonHurtEnnemi2; // son Hurt Ennemi
    public AudioClip sonHurtEnnemi3; // son Hurt Ennemi

    double nbSonHurt; //utilisé pour générer des sons aléatoires lors de l'attaque contre l'ennemi

    void Start()
    {
        audioAttaque = GetComponent<AudioSource>(); //  référence au component AudioSource du protagoniste actif.
    }

    void Update()
    {
        //si le bool est true, personnage en attente d'attaquer
        if(persoVeutAttaquer == true)
        {
            //mémorise la cible pour le prochain allié qui aura son tour
            if (EventSystem.current.currentSelectedGameObject.name == "Cible1" || EventSystem.current.currentSelectedGameObject.name == "Cible2" || EventSystem.current.currentSelectedGameObject.name == "Cible3")
            {
                VariablesGlobales.derniereCible = EventSystem.current.currentSelectedGameObject;
            }

            //desactive les cibles d'ennemis
            lesCibles.SetActive(false);

            //appelle la fonction pour frapper l'ennemi ciblé
            frapper(laCible);
        }
    }

     /*
     *fonction qui permet au personnage actif d'attaquer
     * 
     * autheur : Antoine Côté-L'Écuyer
     * 
     * param : int cible, # de l'ennemi à attaquer
     */
    public void frapper(int cible)
    {
        personnageActif = VariablesGlobales.persoActif;

        //s'il y a un personnage actif, que le bouton n'est pas deja pesé et que le combat n'est pas deja gagné, le perso peut attaquer
        if (personnageActif != null && boutonPeser == false && VariablesGlobales.unActeurAttaque == false && VariablesGlobales.victoire == false)
        {
            //le bouton vient d'être pesé
            boutonPeser = true;

            //Quelqu'un est en train d'attaquer
            VariablesGlobales.unActeurAttaque = true;

            //le personnage ne veut plus attaquer
            persoVeutAttaquer = false;

            //calcule le dommage à infliger
            baseDommage = personnageActif.GetComponent<StatsPersos>().baseDommage;
            minDommage = (float)(baseDommage * 0.8f);
            maxDommage = (float)(baseDommage * 1.2f);

            dommage = (int)Math.Round(UnityEngine.Random.Range(minDommage, maxDommage));

            //baisse la vie de l'ennemi ciblé
            GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<StatsPersos>().Pv -= dommage;

            //pour l'instant on montre la vie de l'ennemi en console log
            print(GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<StatsPersos>().Pv);

            //réinitialise le tour et le timer du personnage actif
            personnageActif.GetComponent<StatsPersos>().tourPerso = false;
            personnageActif.GetComponent<StatsPersos>().tourTimer = 0f;

            //désactive l'aura verte
            personnageActif.transform.Find("auraPersoActif").gameObject.SetActive(false);

            // Auteur: Charles Noel, appellation de son ponctuel de "Mort" selon le script GestionSonEnnemi et selon le nom du personnage attaqué.
            // Auteur: Antoine Côté-L'Écuyer, remet le highlight du menu de combat à blanc et remet l'ordre des étages de sprites à leurs valeurs normales
            switch (personnageActif.name)
            {
                case "Bob":
                    // son
                    personnageActif.GetComponent<AudioSource>().PlayOneShot(sonAttaqueBob); // son AttaqueBob actif.
                    GameObject.Find("CadrePJ1").GetComponent<Image>().color = Color.white;
                    personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    break;

                case "Carl":
                    // son
                    personnageActif.GetComponent<AudioSource>().PlayOneShot(sonAttaqueCarl); // son AttaqueBob actif.
                    GameObject.Find("CadrePJ2").GetComponent<Image>().color = Color.white;
                    personnageActif.GetComponent<SpriteRenderer>().flipX = false;//carl doit se tourner du bon côté pour attaquer :)
                    personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    break;

                case "Croc":
                    // son
                    personnageActif.GetComponent<AudioSource>().PlayOneShot(sonAttaqueCroc); // son AttaqueBob actif.
                    GameObject.Find("CadrePJ3").GetComponent<Image>().color = Color.white;
                    personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    break;

                default: break;
            }

            //change les bool de l'animator du personnage
            personnageActif.GetComponent<Animator>().SetBool("attaque", true);
            personnageActif.GetComponent<Animator>().SetBool("isIdle", false);

            
  
            //change les bool de l'animator de l'ennemi ciblé
            GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<Animator>().SetBool("aMal", true);
            GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<Animator>().SetBool("isIdle", false);

            // Charles Noel : son
            //génere un nombre aléatoire pour choisir la chanson de combat
            nbSonHurt = UnityEngine.Random.Range(0, 3);//crée un nombre entre 0 et 1
            nbSonHurt = Math.Ceiling(nbSonHurt);
            if (nbSonHurt == 0)
            { 
                audioAttaque.PlayOneShot(sonHurtEnnemi); // son sonAttaqueBob actif.
            }
            else if (nbSonHurt == 1)
            {
                audioAttaque.PlayOneShot(sonHurtEnnemi2); // son sonAttaqueBob actif.
            }
            else if (nbSonHurt == 2)
            {
                audioAttaque.PlayOneShot(sonHurtEnnemi3); // son sonAttaqueBob actif.
            }


            //fonctions à activer pour animator ennemi/protagoniste
            StartCoroutine(enleverDouleur(cible));
            StartCoroutine(arreterAttaque(personnageActif));

            //réinitialise le bool mitSurAttaque pour que la sélection se remette sur le bouton attaque au début du prochain tour de ce personnage
            VariablesGlobales.persoActif.GetComponent<tourProtagoniste>().mitSurAttaque = false;

            //enleve ce personnage comme personnage actif
            VariablesGlobales.persoActif = null;
        }

    }

    //petite fonction pour n'avoir qu'à peser une fois sur le bouton attaque pour que le perso lance son attaque quand il peut
    public void selectionAttaque(int cible)
    {
        persoVeutAttaquer = true;
        laCible = cible;
    }

    /*
    *fonction qui remet l'ennemi en idle
    * 
    * auteur : Antoine Côté-L'Écuyer
    * 
    * param : int cible, # de l'ennemi qui a mal
    */
    IEnumerator enleverDouleur(int cible)
    {
        //attend 1s
        yield return new WaitForSeconds(1);
        //change les bool de l'ennemi pour animator
        GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<Animator>().SetBool("aMal", false);
        GameObject.Find("RencontreEnnemis").transform.GetChild(cible).GetComponent<Animator>().SetBool("isIdle", true);
    }

    /*
    *fonction qui remet le personnage en idle
    * 
    * auteur : Antoine Côté-L'Écuyer
    * 
    * param : GameObject personnage, le protagoniste qui a attaqué
    */
    IEnumerator arreterAttaque(GameObject personnage)
    {
        //attend 1s
        yield return new WaitForSeconds(1);

        //un prochain personnage peut attaquer
        VariablesGlobales.unActeurAttaque = false;

        //le joueur peut repeser sur le bouton attaquer
        boutonPeser = false;

        //change les bool du personnage pour animator
        personnage.GetComponent<Animator>().SetBool("attaque", false);
        personnage.GetComponent<Animator>().SetBool("isIdle", true);
    }
}
