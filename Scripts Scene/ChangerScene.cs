using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangerScene : MonoBehaviour {

    public int NumScene;

    string nomScene;

    void Start()
    {
        Scene laScene = SceneManager.GetActiveScene();
        nomScene = laScene.name;
        VariablesGlobales.combat = false;
    }
    // Update is called once per frame
    void Update()
    {
        //si nous sommes dans la scene du jeu
        if (nomScene == "Toxic_city")
        {
            //et que l'equipe au complet est morte
            if (VariablesGlobales.equipeMorte == true)
            {
                //active la scene de fin
                SceneManager.LoadScene("SceneFinMort");

            }
            //et que l'equipe au complet est morte
            if (VariablesGlobales.jeuFin == true)
            {
                //active la scene de fin
                SceneManager.LoadScene("SceneFinJeu");

            }
        }

        //si c'est un autre nom de scène
        else
        {
            //on ne veut pas que l'équipe soit déjà morte
            VariablesGlobales.equipeMorte = false;
        }
    }

    public void ChangerDeScene (int NumScene) {
        VariablesGlobales.ResetVariables();
        SceneManager.LoadScene(NumScene);
	}
}
