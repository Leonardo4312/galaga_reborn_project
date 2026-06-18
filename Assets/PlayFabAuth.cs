using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabAuth : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    // Funzione associata al bottone LOGIN
    public void OnClickLogin()
    {
        if (usernameInput.text.Length < 3 || passwordInput.text.Length < 6)
        {
            statusText.text = "L'username deve avere 3 caratteri e la password almeno 6!";
            return;
        }

        statusText.text = "Connessione a PlayFab...";

        var request = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    // Funzione associata al bottone REGISTRATI
    public void OnClickRegister()
    {
        if (usernameInput.text.Length < 3 || passwordInput.text.Length < 6)
        {
            statusText.text = "Username (min 3) o Password (min 6) non validi!";
            return;
        }

        statusText.text = "Creazione account...";

        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        statusText.text = "Accesso consentito!";
        // Comunichiamo al GameManager che il login è andato a buon fine
        GameManager.instance.OnLoginVerified();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        statusText.text = "Errore Login: " + error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        statusText.text = "Account Creato! Premi LOGIN per entrare.";
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        statusText.text = "Errore Reg: " + error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }
}
