using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
/*
 *Script qui contient les statistiques de l'acteur auquel il est lié
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
public class StatsPersos : MonoBehaviour
{
    //est-ce que c'est le tour de l'acteur?
    public bool tourPerso = false;
    //est-ce que ce personnage est mort?
    public bool estMort = false;

    //est-ce que ce personnage ennemi est mort et dejà compté comme étant mort?
    public bool compteMort = false;

    //modificateurs de points de vies, points d'energie et d'armure propres à chaque personnage
    public float modificateurPersoPv = 1.2f;
    public float modificateurPersoPe = 1.2f;
    public float modificateurPersoAr = 1.2f;
    public float modificateurPersoDmg = 1.2f;
    public float modificateurPersoSpcDmg = 1.2f; // dommage des attaques speciales
    public float modificateurPersoTour = 1.2f;

    //statistiques propres à chaque personnage
    public int Vit = 10;//vitalité
    public int For = 10;//force
    public int Agi = 10;//agilité
    public int Int = 10;//intelligence
    public int Cha = 10;//chance

    //integers utilisées pour recevoir des calculs dans Start
    public int maxPv;
    public int maxPe;
    public float baseDommage;
    public int armure;

    //points de vie et energie en temps réel du personnage
    public int Pv;
    public int Pe;

    //variables pour la minuterie de tour du personnage
    public float tourTimer = 0;
    public int seuilTimer = 300;

    //experience du personnage
    public int exp = 0;
    int nivUpExp = 2000;
    public int niveau = 1;

    //experience que l'ennemi donne au joueur
    public int ennemiExp = 0;
    bool expDonner = false;

    //multiplicateur pour atteindre le prochain niveau
    public float modNiv = 0.8f;

    //stats que le joueur peut donner à son personnage
    public int pointsStats = 0;

    public int typeEnnemi;

    // verification d'appellation unique dans update ou FixedUptade
    public bool sonUnique = false;

    //le protagoniste à qui c'est le tour
    GameObject personnageActif;

    //calculs des hp, ep, armure, dommage de base
    void Start()
    {
        calculMaxPv();
        calculMaxPe();
        calculMaxDmgEtArmure();
    }

    //Calcul de temps 
    void Update()
    {
        //Si l'acteur est en combat et que ce n'est pas son tour
        if (VariablesGlobales.combat == true && tourPerso == false && VariablesGlobales.victoire == false)
        {
            //la minuterie de tour du personnage est incrémentée s'il n'est pas mort
            if (estMort == false)
            {
                tourTimer += (0.05f * Agi * modificateurPersoTour);
            }

            //passé le seuil indiqué, c'est maintenant au tour du perso
            if (tourTimer >= seuilTimer) 
            {
                tourPerso = true;
            }
        }
       
        //met bob, carl ou croc comme personnage actif si c'est son tour et qu'il n'est pas mort
        if (tourPerso == true && VariablesGlobales.persoActif == null && (gameObject.name == "Bob" || gameObject.name == "Carl" || gameObject.name == "Croc") && estMort == false)
        {
            VariablesGlobales.persoActif = gameObject;
        }

        //Quand le personnage atteint le seuil de son niveau
        if (exp >= nivUpExp)
        {
            //monte de niveau
            niveau++;
            //donne un(peut-être plus éventuellement?) point de stat à dépenser
            pointsStats++;
            //soustrait le niveau de l'exp qu'il a en ce moment
            exp = exp - nivUpExp;
            //calcule l'exp que le personnage a besoin pour son prochain niveau
            nivUpExp = (int)(nivUpExp * (modNiv * niveau));
        }

        //si les points de vie sont 0 ou moins
        if (Pv <= 0)
        {
            //le personnage meurt
            estMort = true;

            // Auteur: Charles Noel, appellation de son ponctuel de "Mort" selon le script GestionSonEnnemi et selon le nom du personnage attaqué.
            switch (gameObject.name)
            {
                case "Bob":
                    // debug pour que le son soit appelé une seule fois
                    if (!sonUnique)
                    {
                        sonUnique = true; // le son a été appelé
                        GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonMortBob); // son mort de bob
                    }
                    break;

                case "Carl":
                    // debug pour que le son soit appelé une seule fois
                    if (!sonUnique)
                    {
                        sonUnique = true; // le son a été appelé
                        GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonMortCarl); // son mort de Carl
                    }
                    break;

                case "Croc":
                    // debug pour que le son soit appelé une seule fois
                    if (!sonUnique)
                    {
                        sonUnique = true; // le son a été appelé
                        GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonMortCroc); // son mort de Croc
                    }
                    break;
                default: break;
            }
            // Auteur: Charles Noel, appelation de son ponctuel de "Mort" selon le script GestionSonEnnemi et si le tag de l'object est ennemi.
            if (gameObject.tag == "ennemi")
            {
                // debug pour que le son soit appelé une seule fois
                if (!sonUnique)
                {
                    sonUnique = true; // le son a été appelé
                    GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().audioAttaqueEnnemi.PlayOneShot(GameObject.Find("GestionSonEnnemis").GetComponent<GestionSonEnnemi>().sonMortEnnemi); // son mort de ennemi
                }
            
            }

      
            //si c'est un ennemi et qu'il n'a pas déja donné de l'experience, il donne de l'experience
            if (gameObject.tag == "ennemi" && expDonner == false)
            {
                //variable globale le l'exp accumulée incrémentée
                VariablesGlobales.expAccumuler += ennemiExp;

                //l'ennemi a donné son exp
                expDonner = true;
            }

            //si c'est un des personnages principaux et que c'est le personnage actif,termine son tour
            if (gameObject.tag == "protagoniste" && VariablesGlobales.persoActif == gameObject)
            {
                //met le personnage dans cette valeur pour y accéder plus facilement
                personnageActif = VariablesGlobales.persoActif;

                //réinitialise le tour et le timer du personnage actif
                tourPerso = false;
                tourTimer = 0f;

                //désactive l'aura verte
                personnageActif.transform.Find("auraPersoActif").gameObject.SetActive(false);

                //remet le highlight du menu de combat à blanc et remet l'ordre des étages de sprites à leurs valeurs normales
                switch (personnageActif.name)
                {
                    case "Bob":
                        GameObject.Find("CadrePJ1").GetComponent<Image>().color = Color.white;
                        personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                        break;

                    case "Carl":
                        GameObject.Find("CadrePJ2").GetComponent<Image>().color = Color.white;
                        personnageActif.GetComponent<SpriteRenderer>().flipX = false;//carl doit se tourner du bon côté pour attaquer :)
                        personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        break;

                    case "Croc":
                        GameObject.Find("CadrePJ3").GetComponent<Image>().color = Color.white;
                        personnageActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                        break;

                    default: break;
                }

                //réinitialise le personnage actif
                VariablesGlobales.persoActif = null;
            }
            //change les booléens de l'animateur si le perso est en combat
            if (VariablesGlobales.combat == true)
            {
                gameObject.GetComponent<Animator>().SetBool("isIdle", false);
                gameObject.GetComponent<Animator>().SetBool("estMort", estMort);
            }
        }

        //si les points de vie dépassent le maximum, les points de vie sont au maximum.
        if (Pv > maxPv){Pv = maxPv;}
    }

    //petites fonctions pour calculer les statistiques de base du personnage
    void calculMaxPv()
    {
        maxPv = (int)(Vit * 10 * modificateurPersoPv);
        Pv = maxPv;
    }
    void calculMaxPe()
    {
        maxPe = (int)(Int * 10 * modificateurPersoPe);
        Pe = maxPe;
    }

    void calculMaxDmgEtArmure()
    {
        baseDommage = (float)(For * modificateurPersoDmg);
        armure = (int)((For + Vit) / 2 * modificateurPersoAr);
    }
}
