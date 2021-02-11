using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Script qui vérifie si l'équipe est morte
 * Contient aussi le tableau equipiers pour donner une autre méthode d'accès aux GameObjects des alliés
 * 
 * auteur : Antoine Côté-L'Écuyer
 * 
 * 
 * Gestion des valeurs visuelles  (slider) dans le menu combat statspersos: vie et énergie 
 * 
 * auteur : Charles Noël
 */
public class Equipe : MonoBehaviour
{
    // Tableau des équipiers
    public GameObject[] equipiers;

    // Tableau de Slider vie des équipiers
    public GameObject[] PvSlider;

    // Tableau de Slider énergie des équipiers
    public GameObject[] PeSlider;

    void Update()
    {
        // feedBack visuel slider de la vie des protagonistes
        PvSlider[0].GetComponent<Slider>().value = equipiers[0].GetComponent<StatsPersos>().Pv; // Bob
        PvSlider[1].GetComponent<Slider>().value = equipiers[1].GetComponent<StatsPersos>().Pv; // Carl
        PvSlider[2].GetComponent<Slider>().value = equipiers[2].GetComponent<StatsPersos>().Pv; // Croc

        // feedBack visuel slider de l'énergie des protagonistes
        PeSlider[0].GetComponent<Slider>().value = equipiers[0].GetComponent<StatsPersos>().Pe; // Bob
        PeSlider[1].GetComponent<Slider>().value = equipiers[1].GetComponent<StatsPersos>().Pe; // Carl
        PeSlider[2].GetComponent<Slider>().value = equipiers[2].GetComponent<StatsPersos>().Pe; // Croc

        //si les booléens estMort de chaque personnage de l'equipe est vrai, la partie est finie
        if (equipiers[0].GetComponent<StatsPersos>().estMort == true && equipiers[1].GetComponent<StatsPersos>().estMort == true && equipiers[2].GetComponent<StatsPersos>().estMort == true) {
            VariablesGlobales.equipeMorte = true;
        }
    }
}
