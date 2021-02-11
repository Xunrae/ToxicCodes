using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

/*
 *Script qui permet de gagner un combat et d'en sortir
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
//coller ce script sur le GameObject gestionCombat

public class victoire : MonoBehaviour
{
    //raccourci pour chercher le transform du tableau de rencontre
    Transform rencontreEnnemis;

    //combien d'ennemis sont morts?
    public int ennemisMorts = 0;

    //combien d'ennemis sont présents?
    public int nbEnnemis;

    //est-ce que le bouton a été peser pour le texte de victoire?
    bool boutonPeser = false;
    //est-ce que c'est le message affiché est le dernier?
    bool dernierMsg = false;

    //cameras
    public GameObject cameraCombat;
    public GameObject cameraMain;

    //UI combat
    public GameObject InterfaceCombat;

    //controlleur de musique
    public GameObject ControlleurMusique;

    // Ref minimap
    public GameObject miniMap;

    //Les protagonistes
    Equipe equipe;

    //est-ce qu'on a deja appellé la fonction pour sortir du combat?
    bool sortieCombatActif = false;

    //est-ce que l'incrémentation du nombre de victoires est faite?
    bool victoireDonner = false;

    AudioSource audioCombat; // source Gestionnaire Menu
    public AudioClip sonVictoire; // son Select

    //les cibles des ennemis qui sont à désactiver quand c'est gagné
    public GameObject cibles;

    // Start is called before the first frame update
    void Start()
    {
        audioCombat = GetComponent<AudioSource>(); //  référence au component faire jouer le son du combat.
        rencontreEnnemis = GameObject.Find("RencontreEnnemis").transform;
        equipe = GameObject.Find("Equipe").GetComponent<Equipe>();
    }

    // Update is called once per frame
    void Update()
    {
        //si on est en combat
        if (VariablesGlobales.combat == true)
        {
            //trouve le nombre d'ennemis dans le tableau de rencontre
            nbEnnemis = rencontreEnnemis.childCount;

            //si un ennemi est mort, on l'ajoute au compte d'ennemis morts
            foreach (Transform unEnnemi in rencontreEnnemis)
            {
                if (unEnnemi.GetComponent<StatsPersos>().estMort == true && unEnnemi.GetComponent<StatsPersos>().compteMort == false)
                {
                    unEnnemi.GetComponent<StatsPersos>().compteMort = true;
                    ennemisMorts++;
                }
            }

            //si tous les ennemis sont morts
            if (ennemisMorts == nbEnnemis && dernierMsg == false)
            {

                //le combat est gagné
                VariablesGlobales.victoire = true;

                //aucun protagoniste n'est actif
                VariablesGlobales.persoActif = null;

                //la derniere cible du menu de combat devient null
                VariablesGlobales.derniereCible = null;

                //si la fuite était impossible, la fuite est possible
                VariablesGlobales.fuitePossible = true;

                //désactive les cibles
                GameObject.Find("GererMenus").GetComponent<GestionMenuCombat>().FinTour();

                //s'il y a une aura verte, on la désactive
                if (GameObject.Find("auraPersoActif"))
                {
                    GameObject.Find("auraPersoActif").SetActive(false);
                }

                //Incrementation du nombre de victoire
                if (victoireDonner == false)
                {
                    audioCombat.PlayOneShot(sonVictoire); // son sonVictoire actif.
                    VariablesGlobales.nbVictoire++;
                    victoireDonner = true;
                }

                //change l'annonce pour afficher victoire
                GameObject.Find("Annonce").transform.Find("Text").GetComponent<Text>().text = "Victoire!";

                //change l'animation des personnages
                foreach (GameObject personnage in equipe.equipiers)
                {
                    personnage.GetComponent<Animator>().SetBool("aMal", false);
                    personnage.GetComponent<Animator>().SetBool("isIdle", true);
                    personnage.GetComponent<Animator>().SetBool("victorieux", true);
                }
            }

            //si le texte Victoire! est affiché
            if (GameObject.Find("Annonce").transform.Find("Text").GetComponent<Text>().text == "Victoire!" && Input.GetKeyDown(KeyCode.Space) && boutonPeser == false)
            {
                //le jouer a appuyé sur le bouton
                boutonPeser = true;

                //c'est le dernier message de l'annonceur
                dernierMsg = true;

                //remplace le texte par les points d'expérience gagnés
                GameObject.Find("Annonce").transform.Find("Text").GetComponent<Text>().text = "L'équipe gagne " + VariablesGlobales.expAccumuler + " points d'expérience.";

                Invoke("desactiveBouton", 1f);
            }

            //si on appuie sur espace quand le dernier message est affiché, on sort du combat
            if (dernierMsg == true && boutonPeser == false && Input.GetKeyDown(KeyCode.Space) && sortieCombatActif == false)
            {
                Invoke("sortirDuCombat", 2f);
                sortieCombatActif = true;
            }
        }
    }

    
    void sortirDuCombat()
    {
        //enlève le texte de l'annonceur
        GameObject.Find("Annonce").transform.Find("Text").GetComponent<Text>().text = "";

        //détruit l'ennemi dans la map qui a collisionné avec Bob
        if (VariablesGlobales.ennemiCollision != null)
        {
            Destroy(VariablesGlobales.ennemiCollision);
        }

        //donne l'expérience aux personnages
        for (int i = 2; i < GameObject.Find("Equipe").transform.childCount; i++)
        {
            GameObject.Find("Equipe").transform.GetChild(i).GetComponent<StatsPersos>().exp += VariablesGlobales.expAccumuler;
        }

        //réinitialise le montant d'exp en réserve
        VariablesGlobales.expAccumuler = 0;

        //vide le tableau de rencontre d'ennemis
        if (GameObject.Find("RencontreEnnemis").transform.GetChild(0))
        {
            for (int i = 0; i < nbEnnemis; i++)
            {
                Destroy(GameObject.Find("RencontreEnnemis").transform.GetChild(i).gameObject);

                if (i == nbEnnemis)
                {
                    return;
                }
            }
        }

        // Switch qui active la bonne musique selon le numero de la section du jeu.
        switch (GameObject.Find("Bob").GetComponent<CollisionEvenement>().numSection)
        {
            case 0:
                ControlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(1); // musique ville
                break;
            case 1:
                ControlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(1); // musique ville 
                break;
            case 2:
                ControlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(2); // musique hideout
                break;
            case 3:
                ControlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(3); // musique ville boss
                break;
            default:
                break;
        }

        //si un personnage a son tour, enlever le highlight de sa section du menu de combat
        if(VariablesGlobales.persoActif != null)
        {
            //s'il y a une aura active, on l'enlève
            if (GameObject.Find("auraPersoActif"))
            {
                GameObject.Find("auraPersoActif").gameObject.SetActive(false);
            }

            //remet le highlight du menu de combat blanc et remet son ordre dans les sprites à son état initial
            switch (VariablesGlobales.persoActif.name)
            {
                case "Bob":
                    GameObject.Find("CadrePJ1").GetComponent<Image>().color = Color.white;
                    VariablesGlobales.persoActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    break;

                case "Carl":
                    GameObject.Find("CadrePJ2").GetComponent<Image>().color = Color.white;
                    VariablesGlobales.persoActif.GetComponent<SpriteRenderer>().sortingOrder = 4;
                    break;

                case "Croc":
                    GameObject.Find("CadrePJ3").GetComponent<Image>().color = Color.white;
                    VariablesGlobales.persoActif.GetComponent<SpriteRenderer>().sortingOrder = 5;
                    break;

                default: break;
            }
        }

        //réinitialise les variables globales de combat
        VariablesGlobales.combat = false;
        VariablesGlobales.fuiteCombat = true;
        VariablesGlobales.persoActif = null;
        VariablesGlobales.victoire = false;

        //si il y a une aura verte d'active, on la désactive
        if (GameObject.Find("auraPersoActif"))
        {
            GameObject.Find("auraPersoActif").SetActive(false);
        }

        //réinitialise les paramètres de tours des equipiers
        for (int i = 2; i < GameObject.Find("Equipe").transform.childCount; i++)
        {
            GameObject.Find("Equipe").transform.GetChild(i).GetComponent<StatsPersos>().tourTimer = 0f;
            GameObject.Find("Equipe").transform.GetChild(i).GetComponent<StatsPersos>().tourPerso = false;

            if (i == GameObject.Find("Equipe").transform.childCount) { break; }
        }

        //change l'animation des personnages
        foreach (GameObject personnage in equipe.equipiers)
        {
            personnage.GetComponent<Animator>().SetBool("aMal", false);
            personnage.GetComponent<Animator>().SetBool("isIdle", true);
            personnage.GetComponent<Animator>().SetBool("victorieux", false);
        }

        //remets les équipiers à leur position avant combat
        GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[0].transform.position = VariablesGlobales.posAvantCombat;
        GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[1].transform.position = VariablesGlobales.posAvantCombat;
        GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[2].transform.position = VariablesGlobales.posAvantCombat;

        //change la caméra
        cameraCombat.SetActive(false);
        cameraMain.SetActive(true);
        //change les booléens des caméras
        GameObject.Find("GestionCamera").GetComponent<GestionCamera>().camCombatActive = false; // forcer le bool de la cam combat
        GameObject.Find("GestionCamera").GetComponent<GestionCamera>().camMainActive = true; // forcer le bool de la cam principal

        //désactive la preparation aux animations de combat pour tous les personnages
        GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[0].gameObject.GetComponent<BobScriptDeplacement>().animationsCombat = false;

        //désactive l'interface de combat
        InterfaceCombat.SetActive(false);

        //on peut redonner une prochaine victoire aux variables globales
        victoireDonner = false;

        //bool dernier message redevient false
        dernierMsg = false;

        //nombre d'ennemis tués retourne à 0
        ennemisMorts = 0;

        //on peut à nouveau sortir d'un combat
        sortieCombatActif = false;

        // on reactive la minimap
        miniMap.SetActive(true);

        //permet aux ennemis de pourchasser bob et de retourner en combat apres 5s
        Invoke("desactiveFuite", 5f);
    }

    //fonction qui permet aux ennemis de pourchasser bob et de retourner en combat
    void desactiveFuite()
    {
        VariablesGlobales.fuiteCombat = false;
    }

    //fonction qui remet le bool boutonPeser à false
    void desactiveBouton()
    {
        boutonPeser = false;
    }
}
