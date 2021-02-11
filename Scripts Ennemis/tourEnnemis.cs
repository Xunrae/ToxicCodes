using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *Script qui donne les tours aux ennemis et les fait attaquer un protagoniste (vivant) au hasard
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
//script collé sur tous les ennemis de TableauEnnemi, aussi ajouté au boss et ennemisTuto par code dans script CollisionEvenements
public class tourEnnemis : MonoBehaviour
{
    //les statistiques du personnage
    StatsPersos statsEnnemi;

    //numéro du protagoniste à attaquer
    int noProta;


    //le dommage qui sera infligé
    int dommage;
    //le dommage de base du personnage
    float baseDommage;

    //dommage minimum/maximum pour calcul
    float minDommage;
    float maxDommage;

    bool protagonisteMort;


    // Start is called before the first frame update
    void Start()
    {
        statsEnnemi = gameObject.GetComponent<StatsPersos>();
        baseDommage = statsEnnemi.baseDommage;
    }

    // Update is called once per frame
    void Update()
    {
        //si c'est le tour de l'ennemi et que personne n'est en train d'attaquer
        if(statsEnnemi.tourPerso == true && VariablesGlobales.unActeurAttaque == false) {

            //quelqu'un est en train d'attaquer
            VariablesGlobales.unActeurAttaque = true;

            //choisit un protagoniste au hasard
            noProta = (int)UnityEngine.Random.Range(0, 3) +2;

            //va chercher si sa cible est deja morte
            protagonisteMort = GameObject.Find("Equipe").transform.GetChild(noProta).GetComponent<StatsPersos>().estMort;

            //tant que la cible est morte
            while (protagonisteMort == true)
            {
                //choisit un protagoniste au hasard
                noProta = (int)UnityEngine.Random.Range(0, 3) + 2;
                protagonisteMort = GameObject.Find("Equipe").transform.GetChild(noProta).GetComponent<StatsPersos>().estMort;
                
                //si le resultat est bon, sort de la boucle
                if(protagonisteMort == false) { break;}
            }


            // Auteur: Charles Noel, appelation de son ponctuel selon le script GestionSonEnnemi.
            GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonAttaqueEnnemi2); // son d'attaque de l'ennemi actif.

            //attaque
            attaquer(noProta);
        }
    }

    /*
     *fonction qui permet d'attaquer un protagoniste aléatoirement
     * 
     * auteur : Antoine Côté-L'Écuyer
     * 
     * param : int protagoniste ; le numéro du protagoniste à cibler selon leur ordre dans l'objet Equipe
     */
    public void attaquer(int protagoniste)
    {
    
        //calcule le dommage à infliger
        baseDommage = statsEnnemi.baseDommage;
        minDommage = (float)(baseDommage * 0.8f);
        maxDommage = (float)(baseDommage * 1.2f);

        dommage = (int)Math.Round(UnityEngine.Random.Range(minDommage, maxDommage));
        
        //baisse la vie du protagoniste ciblé
        GameObject.Find("Equipe").transform.GetChild(protagoniste).GetComponent<StatsPersos>().Pv -= dommage;

        // Auteur: Charles Noel, appellation de son ponctuel de "hurt" selon le script GestionSonEnnemi et selon le nom du personnage attaqué.
        switch (GameObject.Find("Equipe").transform.GetChild(protagoniste).name)
        {
            case "Bob":
                // son de "hurt" bob
                GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonHurtBob);
                break;

            case "Carl":
                // son de "hurt" Carl
                GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonHurtCarl);
                break;

            case "Croc":
                // son de "hurt" Croc
                GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonHurtCroc);
                break;

            default: break;
        }

        //change les bool de l'animator du personnage
        gameObject.GetComponent<Animator>().SetBool("attaque", true);
        gameObject.GetComponent<Animator>().SetBool("isIdle", false);

        //change les bool de l'animator de sa cible
        GameObject.Find("Equipe").transform.GetChild(protagoniste).GetComponent<Animator>().SetBool("isIdle", false);
        GameObject.Find("Equipe").transform.GetChild(protagoniste).GetComponent<Animator>().SetBool("aMal", true);

        //si c'est carl, il doit être flipped en X
        if (GameObject.Find("Equipe").transform.GetChild(protagoniste).name == "Carl")
        {
            GameObject.Find("Equipe").transform.GetChild(protagoniste).gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }

        //fonctions à activer pour animator ennemi/protagoniste
        StartCoroutine(enleverDouleur(protagoniste));
        StartCoroutine(arreterAttaque(gameObject));
    }

    /*
    *fonction qui remet le protagoniste en idle
    * 
    * auteur : Antoine Côté-L'Écuyer
    * 
    * param : int cible, # du protagoniste qui a mal
    */
    IEnumerator enleverDouleur(int cible)
    {
        //attend 1s
        yield return new WaitForSeconds(1);

        //change les bool du protagoniste pour animator
        GameObject.Find("Equipe").transform.GetChild(cible).GetComponent<Animator>().SetBool("aMal", false);
        GameObject.Find("Equipe").transform.GetChild(cible).GetComponent<Animator>().SetBool("isIdle", true);

        //Si c'est Carl, on le dé-flip en X
        if(GameObject.Find("Equipe").transform.GetChild(cible).name == "Carl")
        {
            GameObject.Find("Equipe").transform.GetChild(cible).gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    /*
    *fonction qui remet l'ennemi en mode idle
    * 
    * auteur : Antoine Côté-L'Écuyer
    * 
    * param : GameObject personnage, l'ennemi qui a attaqué
    */
    IEnumerator arreterAttaque(GameObject personnage)
    {
        //attend 1s
        yield return new WaitForSeconds(1);

        //un prochain personnage peut attaquer
        VariablesGlobales.unActeurAttaque = false;

        //change les bool de l'ennemi pour animator
        personnage.GetComponent<Animator>().SetBool("attaque", false);
        personnage.GetComponent<Animator>().SetBool("isIdle", true);

        //réinitialise le tour et le timer de l'ennemi
        statsEnnemi.tourTimer = 0f;
        statsEnnemi.tourPerso = false;
    }
}
