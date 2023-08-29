using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestionUsuario : MonoBehaviour
{
    public InputField userInputField, passwordInputField;
    public string userName;
    public int userScore;

    public bool logged = false;

    public void Connect()
    {
        StartCoroutine(Login());
    }

    public void Register()
    {
        StartCoroutine (RegisterUser());
    }
    IEnumerator Login()
    {
        WWW connection = new WWW(
            "http://127.0.0.1/videogame/login.php?"
            + "db_user=" + userInputField.text
            + "&db_pass=" + passwordInputField.text
            );

        yield return connection;

        if (connection.text == "404")
        {
            print("Error de conexion");
        }
        else
        {
            Debug.Log("Conectado Correctamente");
            logged = true;
            SceneManager.LoadScene(1);
        }
        
    }
    IEnumerator RegisterUser()
    {
        WWW conexion = new WWW(
            "http://127.0.0.1/videogame/register.php?"
            + "db_user=" + userInputField.text
            + "&db_pass=" + passwordInputField.text
            );

        yield return (conexion);

        if (conexion.text == "402")
        {
            print("Usuario ya existee!");
        }
        else
        {
            if (conexion.text == "201")
            {
                userName = userInputField.text;
                userScore = 0;
                Debug.Log("Registrado");
                SceneManager.LoadScene(1);
            }
            else
            {
                Debug.LogError("ERROR!! en la conexion");
            }
        }
    }
    
}
