using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *Script qui permet de fuir un combat lorsque c'est le tour d'un personnage
 * 
 * auteur : Antoine Côté-L'Écuyer
 */
//script collé sur gameobject gestionCombat
public class courir : MonoBehaviour
{
    //camera
    public GameObject cameraCombat;
    public GameObject cameraMain;
    public GameObject InterfaceCombat;

    public GameObject ControlleurMusique;

    // Ref minimap
    public GameObject miniMap;

    //bool pour éviter que les messages s'effacent rapidement
    bool boutonPeser = false;

    //script GestionMenuCombat
    GestionMenuCombat gestionMenuCombat;

    //script equipe
    Equipe equipe;
    
    //le nombre d'ennemis sur le terrain
    int nbEnnemis;

    AudioSource audioCombat; // source Gestionnaire Menu
    public AudioClip sonCourir; // son Select

    //lance fonction quitterCombat quand true avec Update
    bool persoVeutFuir = false;

    void Start()
    {
        audioCombat = GetComponent<AudioSource>(); //  référence au component faire jouer le son du combat.
        equipe = GameObject.Find("Equipe").GetComponent<Equipe>();
        gestionMenuCombat = GameObject.Find("GererMenus").GetComponent<GestionMenuCombat>();
    }

    void Update()
    {
        //apelle la fonction quitterCombat tant que le bool est true
        if (persoVeutFuir == true)
        {
            quitterCombat();
        }
    }

    //permet d'enclencher la fonction quitter combat aussitôt que c'est possible
    public void selectionCourir()
    {
        if (VariablesGlobales.persoActif != null)
        {
            persoVeutFuir = true;
            audioCombat.PlayOneShot(sonCourir); // son sonCourir actif.
        }
    }

    /*
     *Fonction qui permet de fuir un combat lorsque c'est le tour d'un personnage
     * 
     * auteur : Antoine Côté-L'Écuyer
     */
    void quitterCombat()
    {
        if (boutonPeser == false && VariablesGlobales.unActeurAttaque == false)
        {
            //arrête d'apeller la fonction
            persoVeutFuir = false;

            if (VariablesGlobales.fuitePossible == true && VariablesGlobales.persoActif != null)
            {

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

                //ferme les fenêtres d'objet dans le menu de combat, au cas ou celles-ci soient actives
                GameObject.Find("GererMenus").GetComponent<GestionMenuCombat>().sectionInventaire.SetActive(false);
                GameObject.Find("GererMenus").GetComponent<GestionMenuCombat>().sectionPersonnagesItems.SetActive(false);

                //réinitialise le bool mitSurAttaque pour que la sélection se remette sur le bouton attaque au début du prochain tour de ce personnage
                VariablesGlobales.persoActif.GetComponent<tourProtagoniste>().mitSurAttaque = false;

                //réinitialise les variables globales
                VariablesGlobales.fuiteCombat = true;
                VariablesGlobales.persoActif = null;

                //réactive les boutons de cibles au besoin
                gestionMenuCombat.FinTour();

                VariablesGlobales.combat = false;
                //personne n'est en train d'attaquer
                VariablesGlobales.unActeurAttaque = false;
                //il n'y a pas d'ennemi en collision
                VariablesGlobales.ennemiCollision = null;
                //la derniere cible devient null
                VariablesGlobales.derniereCible = null;

                //s'il y a une aura active, on l'enlève
                if (GameObject.Find("auraPersoActif"))
                {
                    GameObject.Find("auraPersoActif").gameObject.SetActive(false);
                }

                //réinitialise les timers de tours des protagonistes
                for (int i = 2; i < GameObject.Find("Equipe").transform.childCount; i++)
                {
                    GameObject.Find("Equipe").transform.GetChild(i).GetComponent<StatsPersos>().tourTimer = 0f;
                    GameObject.Find("Equipe").transform.GetChild(i).GetComponent<StatsPersos>().tourPerso = false;

                    if (i == GameObject.Find("Equipe").transform.childCount) { break; }
                }

                //tous les cadres du menu de combat deviennent blanc
                GameObject.Find("CadrePJ1").GetComponent<Image>().color = Color.white;
                GameObject.Find("CadrePJ2").GetComponent<Image>().color = Color.white;
                GameObject.Find("CadrePJ3").GetComponent<Image>().color = Color.white;

                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade(); // active la camera main et le fade

                //change les booléens des caméras
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().camCombatActive = false; // forcer le bool de la cam combat
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().camMainActive = true; // forcer le bool de la cam principal

                //ramène l'équipe à leurs positions avant le combat
                GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[0].transform.position = VariablesGlobales.posAvantCombat;
                GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[1].transform.position = VariablesGlobales.posAvantCombat;
                GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[2].transform.position = VariablesGlobales.posAvantCombat;

                //désactive la preparation aux animations de combat pour tous les personnages
                GameObject.Find("Equipe").GetComponent<Equipe>().equipiers[0].gameObject.GetComponent<BobScriptDeplacement>().animationsCombat = false;

                //désactive le mode "fuite" apres 5s
                Invoke("desactiveFuite", 5f);

                // on reactive la minimap
                miniMap.SetActive(true);

                //change la caméra
                cameraCombat.SetActive(false);
                cameraMain.SetActive(true);

                InterfaceCombat.SetActive(false);

                //le nombre d'ennemis
                nbEnnemis = GameObject.Find("RencontreEnnemis").transform.childCount;
                //détruit les ennemis du tableau RencontreEnnemis
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

                //change le bool des animators de chaque protagoniste
                foreach (GameObject personnage in equipe.equipiers)
                {
                    personnage.GetComponent<Animator>().SetBool("aMal", false);
                    personnage.GetComponent<Animator>().SetBool("isIdle", true);
                }
            }
            else if (VariablesGlobales.fuitePossible == false)
            {
                //active
                boutonPeser = true;
                GameObject.Find("CanvasJeu").transform.Find("fuiteImpossible").transform.gameObject.SetActive(true);
                Invoke("reinitialiserBouton", 2f);
                Invoke("desactiverTexteFuite", 2f);
            }
        }
    }

    //fonction pour fermer la boîte de texte quand la fuite n'est pas possible
    void desactiverTexteFuite()
    {
        GameObject.Find("fuiteImpossible").SetActive(false);
    }

    //fonction qui remet fuiteCombat à false : les ennemis recommencent à pourchasser Bob
    void desactiveFuite()
    {
        VariablesGlobales.fuiteCombat = false;
    }

    //fonction qui remet boutonPeser à false : le joueur peut re-peser sur le bouton courir
    void reinitialiserBouton()
    {
        boutonPeser = false;
    }
}
