using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionSonEnnemi : MonoBehaviour
{

    /*
     * Script qui permet de gérer les sons ponctuels des Ennmis (combat), NPCs (dialogue).
     * 
     * Auteur : Charles Noel
     * Modifié: 13/12/2020
     *   
    */

    //script collé sur l'objet GestionSonEnnemis

    public AudioSource audioAttaqueEnnemi; // source Gestionnaire Ennemi ou Npc
    public AudioClip sonHurtBob; // son Hurt Bob
    public AudioClip sonHurtCarl; // son Hurt Carl
    public AudioClip sonHurtCroc; // son Hurt Croc

    public AudioClip sonMortBob; // son mort Bob
    public AudioClip sonMortCarl; // son mort Carl
    public AudioClip sonMortCroc; // son mort Croc

    public AudioClip sonMortEnnemi; // son mort ennemi

    public AudioClip sonNPC; // son dialogue NPC

    public AudioClip sonAttaqueEnnemi2; // son son Hurt Croc

    void Start()
    {
        audioAttaqueEnnemi = GetComponent<AudioSource>(); //  référence au component AudioSource de l'objet GestionSonEnnemis.
    }
}
