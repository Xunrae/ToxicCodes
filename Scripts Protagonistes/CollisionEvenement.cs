using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;


/*
    Gestion des évènements de Collision. (Map)
    Par: 
    Charles Noël: Gestion collision son ponctuel et musique, positionnement, animation de fade.
    Dalianne Gosselin: Gestion collision de bonus pour l'inventaire.
    Antoine Coté-l'écuyer: Amélioration de la section positionnement, création des combats.
    modifié: 11/09/2020
*/


public class CollisionEvenement : MonoBehaviour
{

    StatsPersos statsBob; // référence à l'objet stats


    /** variable de type audio**/

    public GameObject controlleurMusique; // GameObject controlleur musique
    AudioSource audioPersonnage; // source personnage

    public AudioClip sonBonusPillule; // son BonusEtoile
    public AudioClip sonBonusParchemin; // son BonusManuscrit
    public AudioClip sonBonusBaril; // son BonusToxique
    public AudioClip sonBonusCerveau; // son BonusCerveau

    public AudioClip sonDebutCombat; // son monstre1
    public AudioClip sonDebutCombatEn; // son monstre2
    

    public AudioClip sonAttaqueEnnemi; // son "pain"
    public AudioClip sonAttaqueJoueur; // son "pain2"

    public AudioClip sonOuverturePorteBonus; // son "pain2"
    public AudioClip sonOuverturePorteFin; // son "pain2"

    public double nbChansonCombat; //utilisé pour générer de la musique aleatoire

    /** variable pour bonus et ennemis**/

    public GameObject refInventaireBonus; // référence aux bonus

    public GameObject rencontreEnnemis; //l'objet qui regroupe les ennemis en combat

    public GameObject tableauEnnemis; // tableau des ennemis 
    double nbEnnemisD; //utilisé pour générer les ennemis lors des collisions avec eux hors combat
    int nbEnnemis;

    public GameObject[] TableauSectionCombat; // Tableau qui contient les différentes sections de la map.
    public int numSection; // int pour l'incrémentation du TableauSectionCombat

    public GameObject InterfaceCombat;
    public GameObject miniMap;
    public GameObject porteBonus;
    public GameObject porteFin;

    //int pour generer un ennemi aléatoire
    int ennemiRandom;

    bool tutoFait = false;//est-ce qu'on est déja entré dans le tutoriel?
    bool bossCombat = false;//est-ce qu'on est déja entré dans le combat du boss?

    //Raccourcis
    Transform transformEnnemiTuto;
    Transform transformEnnemi;
    Equipe equipe;
    GameObject Bob;
    Transform collisionTransform;

    /** Instance **/

    //type d'ennemi de l'ennemi en collision
    int typeEnnemi;
    //instances des ennmis en combat
    GameObject cloneX;
    GameObject cloneY;
    GameObject cloneZ;

    bool elmActif;

    void Start()
    {
        elmActif = false;
        numSection = 0; // On commence à la position 0 du TableauSectionCombat (Arène tutoriel)
        audioPersonnage = GetComponent<AudioSource>(); //  référence au component faire jouer le son du personnage.
        VariablesGlobales.fuitePossible = true; // réinitialisation de la fuite
        porteBonus.SetActive(true); // la porte bonus commence active
        porteFin.SetActive(true); // la porte fin commence active

        //initialisation des raccourcis
        transformEnnemiTuto = GameObject.Find("EnnemisMotarsTuto").transform;
        equipe = GameObject.Find("Equipe").GetComponent<Equipe>();
        Bob = GameObject.Find("Bob");
    }

    void FixedUpdate()
    {
        // si on gagne 5 victoire la porte bonus s'ouvre
        if (VariablesGlobales.nbVictoire == 5)
        {
            porteBonus.SetActive(false); // désativation de l'objet porte bonus

            // debug on appel une seule fois le son
            if (!elmActif)
            {
                elmActif = true; // le son a été appelé
                audioPersonnage.PlayOneShot(sonOuverturePorteBonus); // son sonOuverturePorteFin actif.
            }
        }
    }

    // Collision sous-section Importante (changer l'arène dans la même section, combat tutoriel, combat boss)
    void OnTriggerEnter2D(Collider2D collisionSectionImportante)
    {
        //initialisation raccourci pour aller chercher le transform
        collisionTransform = collisionSectionImportante.transform;

        switch (collisionSectionImportante.gameObject.tag)
        {
            case "FinDemo":
                VariablesGlobales.jeuFin = true;
                break;
            case "ville":
                numSection = 1;
                break;
            case "CombatTuto":
                if (tutoFait == false)
                {
                    numSection = 0;
                    VariablesGlobales.combat = true;
                    VariablesGlobales.fuitePossible = false;
                    VariablesGlobales.posAvantCombat = equipe.equipiers[0].transform.position;
                    //change la variable globale pour les boutons cibles
                    VariablesGlobales.nbEnnemis = 3;

                    tutoFait = true;

                    if (VariablesGlobales.combat == true)
                    {
                        Bob.GetComponent<GestionTexteDialogue>().desactiverTexteDebut();
                        Bob.GetComponent<GestionTexteDialogue>().desactiverTexteTuto();


                        //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                        GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();

                        InterfaceCombat.SetActive(true);
                        miniMap.SetActive(false);

                        audioPersonnage.PlayOneShot(sonDebutCombat); // son sonDebutCombat actif.
                        controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(4);

                        equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; // Changement de position de l'équipier 0 (Bob) pour la position 0
                        equipe.equipiers[1].transform.position = collisionTransform.GetChild(1).position; // Changement de position de l'équipier 1 (Carl) pour la position 1
                        equipe.equipiers[2].transform.position = collisionTransform.GetChild(2).position; // Changement de position de l'équipier 2 (Croc) pour la position 2

                        // aucune vélocité en combat
                        equipe.equipiers[0].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[1].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[2].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

                        //place le script pour faire attaquer sur les ennemis
                        transformEnnemiTuto.GetChild(0).gameObject.AddComponent<tourEnnemis>();
                        transformEnnemiTuto.GetChild(1).gameObject.AddComponent<tourEnnemis>();
                        transformEnnemiTuto.GetChild(2).gameObject.AddComponent<tourEnnemis>();

                        transformEnnemiTuto.GetChild(0).position = collisionTransform.GetChild(5).position; // Changement de position de l'ennemi 0 pour la position 0 de l'arène Tuto
                        transformEnnemiTuto.GetChild(1).position = collisionTransform.GetChild(4).position; // Changement de position de l'ennemi 1 pour la position 1 de l'arène Tuto
                        transformEnnemiTuto.GetChild(2).position = collisionTransform.GetChild(6).position; // Changement de position de l'ennemi 2 pour la position 2 de l'arène Tuto

                        //change le parent des motards du tutoriel pour RencontreEnnemis
                        transformEnnemiTuto.GetChild(0).transform.SetParent(rencontreEnnemis.transform); // Changement de position de l'ennemi 0 pour la position 0 de l'arène Tuto
                        transformEnnemiTuto.GetChild(0).transform.SetParent(rencontreEnnemis.transform); // Changement de position de l'ennemi 1 pour la position 1 de l'arène Tuto
                        transformEnnemiTuto.GetChild(0).transform.SetParent(rencontreEnnemis.transform); // Changement de position de l'ennemi 2 pour la position 2 de l'arène Tuto
                    }
                    else
                    {
                        InterfaceCombat.SetActive(false);
                        miniMap.SetActive(true);
                    }
                }
                break;
            case "combatBoss":

                //
                controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(7);
                
                if (bossCombat == false)
                {

                    VariablesGlobales.combat = true; // le combat est actif
                    VariablesGlobales.fuitePossible = false;
                    VariablesGlobales.posAvantCombat = equipe.equipiers[0].transform.position;
                    //change la variable globale pour les boutons cibles
                    VariablesGlobales.nbEnnemis = 1;

                    bossCombat = true;

                    if (VariablesGlobales.combat == true)
                    {
                        //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                        GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();

                        audioPersonnage.PlayOneShot(sonOuverturePorteFin); // son sonOuverturePorteFin actif.
                        porteFin.SetActive(false); // ouverture de la porte qui mène à la fin du jeu 
                        InterfaceCombat.SetActive(true); // On affiche le menu du combat
                        miniMap.SetActive(false); // la mini map n'est plus affiché

                    
                        controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(7); // Change la chanson selon le controlleur de musique 

                        equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; // Changement de position de l'équipier 0 (Bob) pour la position 0 de l'arène boss
                        equipe.equipiers[1].transform.position = collisionTransform.GetChild(1).position; // Changement de position de l'équipier 1 (Carl) pour la position 1 de l'arène boss
                        equipe.equipiers[2].transform.position = collisionTransform.GetChild(2).position; // Changement de position de l'équipier 2 (Croc) pour la position 2 de l'arène boss

                        // aucune vélocité en combat
                        equipe.equipiers[0].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[1].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[2].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

                        //ajoute au boss le script qui lui permet d'attaquer
                        GameObject.Find("EnnemisBoss").transform.GetChild(0).gameObject.AddComponent<tourEnnemis>();

                        GameObject.Find("EnnemisBoss").transform.GetChild(0).position = collisionTransform.GetChild(5).position; // Changement de position de l'ennemi Boss pour la position 1 de l'arène boss

                        GameObject.Find("EnnemisBoss").transform.GetChild(0).transform.SetParent(rencontreEnnemis.transform); // Changement de position du Boss pour la position 0 du tableau rencontreEnnemis

                    }
                    else
                    {
                        InterfaceCombat.SetActive(false);
                        miniMap.SetActive(true);
                        
                    }
                }
                break;
            default: return;
        }
    }


    void OnCollisionEnter2D(Collision2D collisionMap)
    {
        collisionTransform = collisionMap.transform;
        //Detection de collision avec les objets
        if (collisionMap.gameObject.tag == "BonusToxique" || collisionMap.gameObject.tag == "BonusManuscrit" || collisionMap.gameObject.tag == "BonusEtoile" || collisionMap.gameObject.tag == "BonusCerveau")
        {
            refInventaireBonus.GetComponent<Inventaire>().AjouterItem(collisionMap.gameObject.tag);
            collisionMap.gameObject.SetActive(false);
        }

        //Detection de collision par cas
        switch (collisionMap.gameObject.tag)
        {
            case "BonusToxique":
                audioPersonnage.PlayOneShot(sonBonusBaril); // son sonBonusBaril actif.
                GetComponent<GestionTexteDialogue>().MessageBoutonDebut();
                GetComponent<GestionTexteDialogue>().TexteBouton.text = "Toxique trouvé!";
                GetComponent<GestionTexteDialogue>().bouton.text = "i";
                break;

            case "BonusManuscrit":
                audioPersonnage.PlayOneShot(sonBonusParchemin); // son sonBonusParchemin actif.
                GetComponent<GestionTexteDialogue>().MessageBoutonDebut();
                GetComponent<GestionTexteDialogue>().TexteBouton.text = "Parchemin trouvé!";
                GetComponent<GestionTexteDialogue>().bouton.text = "i";
                break;

            case "BonusEtoile":
                audioPersonnage.PlayOneShot(sonBonusPillule); // son sonBonusPilule actif.
                GetComponent<GestionTexteDialogue>().MessageBoutonDebut();
                GetComponent<GestionTexteDialogue>().TexteBouton.text = "Pilule trouvée!";
                GetComponent<GestionTexteDialogue>().bouton.text = "i";
                break;

            case "BonusCerveau":
                audioPersonnage.PlayOneShot(sonBonusCerveau); // son sonBonusCerveau actif.
                GetComponent<GestionTexteDialogue>().MessageBoutonDebut();
                GetComponent<GestionTexteDialogue>().TexteBouton.text = "Cerveau trouvé!";
                GetComponent<GestionTexteDialogue>().bouton.text = "i";
                break;

            case "Teleport": // hideout
                             //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                Bob.GetComponent<BobScriptDeplacement>().vxMax = 0;
                Bob.GetComponent<BobScriptDeplacement>().vyMax = 0;
                Invoke("ArretDeplacement", 1.5f);
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();
                numSection ++;
                controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(2);

                equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; //Perso1
                equipe.equipiers[1].transform.position = collisionTransform.GetChild(0).position; //Perso2
                equipe.equipiers[2].transform.position = collisionTransform.GetChild(0).position; //Perso3
                break;

            case "Return": // ville
                           //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                Bob.GetComponent<BobScriptDeplacement>().vxMax = 0;
                Bob.GetComponent<BobScriptDeplacement>().vyMax = 0;
                Invoke("ArretDeplacement", 1.5f);
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();
                numSection --;
                controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(1);

                equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; //Perso1
                equipe.equipiers[1].transform.position = collisionTransform.GetChild(0).position; //Perso2
                equipe.equipiers[2].transform.position = collisionTransform.GetChild(0).position; //Perso3
                break;

            case "Boss": // high level
                         //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                Bob.GetComponent<BobScriptDeplacement>().vxMax = 0;
                Bob.GetComponent<BobScriptDeplacement>().vyMax = 0;
                Invoke("ArretDeplacement", 1.5f);
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();
                numSection ++;
                controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(3);

                equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; //Perso1
                equipe.equipiers[1].transform.position = collisionTransform.GetChild(0).position; //Perso2
                equipe.equipiers[2].transform.position = collisionTransform.GetChild(0).position; //Perso3
                break;

            case "Return2": // hideout
                            //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                Bob.GetComponent<BobScriptDeplacement>().vxMax = 0;
                Bob.GetComponent<BobScriptDeplacement>().vyMax = 0;
                Invoke("ArretDeplacement", 1.5f);
                GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();
                numSection --;
                controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(2);

                equipe.equipiers[0].transform.position = collisionTransform.GetChild(0).position; //Perso1
                equipe.equipiers[1].transform.position = collisionTransform.GetChild(0).position; //Perso2
                equipe.equipiers[2].transform.position = collisionTransform.GetChild(0).position; //Perso3

                break;

            case "ennemi":

                if (VariablesGlobales.fuiteCombat == false)
                {
                    VariablesGlobales.combat = true;//le jeu entre en mode combat
                    VariablesGlobales.posAvantCombat = equipe.equipiers[0].transform.position;
                    VariablesGlobales.ennemiCollision = collisionMap.gameObject;

                    if (VariablesGlobales.combat == true)
                    {
                        Bob.GetComponent<GestionTexteDialogue>().desactiverTexteDebut();
                        Bob.GetComponent<GestionTexteDialogue>().desactiverTexteTuto();

                        //active le fadein/fadeout de la lumiere pour la transition vers l'arene
                        GameObject.Find("GestionCamera").GetComponent<GestionCamera>().ActiveCameraEtFade();

                        InterfaceCombat.SetActive(true);
                        miniMap.SetActive(false);
                        audioPersonnage.PlayOneShot(sonDebutCombatEn); // son sonDebutCombat actif.
                     
                        //controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(4);
                        //Reinitialise les velocités des personnages
                        equipe.equipiers[0].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[1].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                        equipe.equipiers[2].GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);

                         
                        //change la position des equipiers
                        equipe.equipiers[0].transform.position = TableauSectionCombat[numSection].transform.GetChild(0).position; //Perso1
                        equipe.equipiers[1].transform.position = TableauSectionCombat[numSection].transform.GetChild(1).position; //Perso2
                        equipe.equipiers[2].transform.position = TableauSectionCombat[numSection].transform.GetChild(2).position; //Perso3

                        //génere un nombre aléatoire pour choisir la chanson de combat
                        nbChansonCombat = UnityEngine.Random.Range(0, 2);//crée un nombre entre 0 et 1
                        nbChansonCombat = Math.Ceiling(nbChansonCombat);
                        if (nbChansonCombat == 0)
                        { controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(5); }
                        else if (nbChansonCombat == 1)
                        {
                            controlleurMusique.GetComponent<ControlleurMusique>().ChangeChanson(6);
                        }

                        //génere un nombre aléatoire d'ennemis
                        nbEnnemisD = UnityEngine.Random.Range(0, 4);//crée un nombre entre 0 et 4
                        nbEnnemisD = Math.Ceiling(nbEnnemisD);
                        if (nbEnnemisD == 0) { nbEnnemisD = 1; }
                        else if (nbEnnemisD == 4) { nbEnnemisD = 3; }

                        nbEnnemis = (int)nbEnnemisD;
                        //change la variable globale pour les boutons cibles
                        VariablesGlobales.nbEnnemis = nbEnnemis;

                        //va chercher le type d'ennemi qui a entré en collision avec le personnage
                        typeEnnemi = collisionMap.gameObject.GetComponent<StatsPersos>().typeEnnemi;

                        switch (nbEnnemis)
                        {
                            case 1:
                                //cloneX = l'ennemi qui est entré en collision avec le joueur
                                cloneX = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[typeEnnemi], TableauSectionCombat[numSection].transform.GetChild(5).position, TableauSectionCombat[numSection].transform.GetChild(5).rotation);
                                cloneX.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneX.SetActive(true);

                                break;

                            case 2:
                                //cloneX = l'ennemi qui est entré en collision avec le joueur
                                cloneX = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[typeEnnemi], TableauSectionCombat[numSection].transform.GetChild(5).position, TableauSectionCombat[numSection].transform.GetChild(5).rotation);
                                cloneX.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneX.SetActive(true);

                                //crée un nombre aléatoire
                                ennemiRandom = UnityEngine.Random.Range(0, 4);
                                if(ennemiRandom == 4) { ennemiRandom = 3; }

                                //le clone #2 devient cet ennemi aléatoire
                                cloneY = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[ennemiRandom], TableauSectionCombat[numSection].transform.GetChild(4).position, TableauSectionCombat[numSection].transform.GetChild(4).rotation);
                                cloneY.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneY.SetActive(true);

                                break;

                            case 3:
                                //cloneX = l'ennemi qui est entré en collision avec le joueur
                                cloneX = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[typeEnnemi], TableauSectionCombat[numSection].transform.GetChild(5).position, TableauSectionCombat[numSection].transform.GetChild(5).rotation);
                                cloneX.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneX.SetActive(true);


                                //crée un nombre aléatoire
                                ennemiRandom = UnityEngine.Random.Range(0, 4);
                                if (ennemiRandom == 4) { ennemiRandom = 3; }

                                //le clone #2 devient cet ennemi aléatoire
                                cloneY = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[ennemiRandom], TableauSectionCombat[numSection].transform.GetChild(4).position, TableauSectionCombat[numSection].transform.GetChild(4).rotation);
                                cloneY.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneY.SetActive(true);


                                //crée un nombre aléatoire
                                ennemiRandom = UnityEngine.Random.Range(0, 4);
                                if (ennemiRandom == 4) { ennemiRandom = 3; }

                                //le clone #3 devient cet ennemi aléatoire
                                cloneZ = Instantiate(tableauEnnemis.GetComponent<scriptTableauEnnemis>().tousEnnemis[ennemiRandom], TableauSectionCombat[numSection].transform.GetChild(6).position, TableauSectionCombat[numSection].transform.GetChild(6).rotation);
                                cloneZ.gameObject.transform.parent = GameObject.Find("RencontreEnnemis").transform;
                                cloneZ.SetActive(true);

                                break;

                            default: return;
                        }
                    }
                }
                break;

            case "piegeMort":
                VariablesGlobales.equipeMorte = true; // les protagonistes sont mort
                break;

            default: return;
        }
    }

    // fonction qui remet la vitesse du pesonnage apres x temps d'arret
    public void ArretDeplacement()
    {
        Bob.GetComponent<BobScriptDeplacement>().vxMax = 7; 
        Bob.GetComponent<BobScriptDeplacement>().vyMax = 7;
    }


}
