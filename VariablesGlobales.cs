using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesGlobales : MonoBehaviour
{
    //est-ce que les acteurs sont en combat
    public static bool combat = false;

    //est-ce que la totalité de l'equipe du joueur est morte
    public static bool equipeMorte = false;

    //est-ce que c'est la fin du jeu
    public static bool jeuFin = false;

    //position de bob avant que le combat ne commence
    public static Vector3 posAvantCombat;

    //est-ce que le joueur vient de courir
    public static bool fuiteCombat = false;

    //est-ce que c'est possible de fuir le combat?
    public static bool fuitePossible = true;

    //personnage qui a son tour en ce moment
    public static GameObject persoActif = null;

    //expérience accumulée durant le combat
    public static int expAccumuler = 0;

    //est-ce que le combat est gagné?
    public static bool victoire = false;

    //est-ce qu'il y a un personnage qui attaque en ce moment?
    public static bool unActeurAttaque = false;

    //nombre de victoire accumulé
    public static int nbVictoire = 0;

    //l'ennemi sur la map qui est entré en collision avec Bob
    public static GameObject ennemiCollision = null;

    //le nombre d'ennemis qui sont entrés en combat pour avoir le bon nombre de cibles actives en combat
    public static int nbEnnemis = 0;

    //quelle est la derniere cible utilisée?
    public static GameObject derniereCible = null;

    public static void ResetVariables()
    {
        //réinitialise les acteurs sont en combat
        combat = false;
        //réinitialise la totalité de l'equipe du joueur est morte
        equipeMorte = false;
        //réinitialise c'est la fin du jeu
        jeuFin = false;
        //réinitialise le joueur vient de courir
        fuiteCombat = false;
        //réinitialise c'est possible de fuir le combat?
        fuitePossible = true;
        //réinitialise personnage qui a son tour en ce moment
        persoActif = null;
        //réinitialise l'expérience accumulée
        expAccumuler = 0;

        //réinitialise nombre de victoire accumulé
        nbVictoire = 0;
    }    
}
