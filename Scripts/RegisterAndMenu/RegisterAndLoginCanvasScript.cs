
using Newtonsoft.Json.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game_Designer_Online_Scripts
{
    public class RegisterAndLoginCanvasScript : MonoBehaviour
    {
        #region Main Menu Functions
        [Header("Main Menu Variables")]
        [SerializeField] private Button loginButtonReference;
        [SerializeField] private Button registerButtonReference;
        private void OnLoginButtonClicked()
        {
            loginMenuReference.SetActive(true);
            mainMenuReference.SetActive(false);
        }
        private void OnRegisterButtonClicked()
        {
            registerMenuReference.SetActive(true);
            mainMenuReference.SetActive(false);          
        }
        private void SetupMainMenuButtons()
        {
            loginButtonReference.onClick.AddListener(OnLoginButtonClicked);
            registerButtonReference.onClick.AddListener(OnRegisterButtonClicked);
        }
        private void DeactivateOthersScreensExceptMainMenu()
        {
            loginMenuReference.SetActive(false);
            registerMenuReference.SetActive(false);
            accountRecoveryMenuReference.SetActive(false);
            mainMenuReference.SetActive(true);
        }
        #endregion

        #region Login Menu Functions
        [Header("Login Menu Variables")]
        [SerializeField] private Button loginMenuBackButtonReference;
        [SerializeField] private Button loginMenuLoginButtonReference;

        [SerializeField] private Button loginMenuForgotPasswordButtonReference;

        private string _LoginEmailAddress, _LoginPassword;
        public void OnLoginEmailAdressInputFieldValueChanged(string valueChanged)
        {
            _LoginEmailAddress = valueChanged;
        }
        public void OnLoginPasswordInputFieldValueChanged(string valueChanged)
        {
            _LoginPassword = valueChanged;
        }

        private void SetupLoginMenuButtons()
        {
            loginMenuBackButtonReference.onClick.AddListener(DeactivateOthersScreensExceptMainMenu);
            loginMenuLoginButtonReference.onClick.AddListener(OnLoginMenuLoginButtonClicked);
            loginMenuForgotPasswordButtonReference.onClick.AddListener(OnLoginMenuForgotPasswordButtonClicked);
        }
        private void OnLoginMenuLoginButtonClicked()
        {
            if (string.IsNullOrEmpty(_LoginEmailAddress))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Email Address"));
                return;
            }
            if (_LoginEmailAddress.Contains("@") == false)
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter a valid Email"));
                return;
            }
            if (string.IsNullOrEmpty(_LoginPassword))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Password"));
                return;
            }

            var loginRequest = new LoginWithEmailAddressRequest 
            {
             Email = _LoginEmailAddress,
             Password = _LoginPassword
            };


            //telling buttons to stop interacting
            loginMenuLoginButtonReference.interactable = false;
            loginMenuBackButtonReference.interactable = false;
            loginMenuForgotPasswordButtonReference.interactable = false;

            PlayFabClientAPI.LoginWithEmailAddress(
                loginRequest,
                resultCallback: result =>
                {
                    print("Login success");
                    StartCoroutine(Routine_DisplayMessage("Login was successfull"));
                    StartCoroutine(Routine_RegistrationSuccessful());
                    loginMenuLoginButtonReference.interactable = true;
                    loginMenuBackButtonReference.interactable = true;
                },
                errorCallback: error=>
                {
                    StartCoroutine(Routine_DisplayMessage(error.ErrorMessage));
                    loginMenuLoginButtonReference.interactable = true;
                    loginMenuBackButtonReference.interactable = true;
                    loginMenuForgotPasswordButtonReference.interactable = true;
                }
                );
        }
        private void OnLoginMenuForgotPasswordButtonClicked()
        {
            accountRecoveryMenuReference.SetActive(true);
            loginMenuReference.SetActive(false);
        }
        #endregion

        #region Register Menu Functions
        [Header("Login Menu Variables")]
        [SerializeField] private Button registerMenuBackButtonReference;
        [SerializeField] private Button registerMenuLoginButtonReference;

        private string _RegisterEmailAddress, _RegisterPassword, _RegisterUsername;
        public void OnRegisterEmailAdressInputFieldValueChanged(string valueChanged)
        {
            _RegisterEmailAddress = valueChanged;
        }
        public void OnRegisterPasswordInputFieldValueChanged(string valueChanged)
        {
            _RegisterPassword = valueChanged;
        }
        public void OnRegisterUsernameInputFieldValueChanged(string valueChanged)
        {
            _RegisterUsername = valueChanged;
        }

        private void SetupRegisterMenuButtons()
        {
            registerMenuBackButtonReference.onClick.AddListener(DeactivateOthersScreensExceptMainMenu);
            registerMenuLoginButtonReference.onClick.AddListener(OnRegisterMenuRegisterButtonClicked);
        }
        private void OnRegisterMenuRegisterButtonClicked()
        {
            if (string.IsNullOrEmpty(_RegisterUsername))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter a User Name"));
                return;
            }
            if (string.IsNullOrEmpty(_RegisterEmailAddress))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Email Address"));
                return;
            }
            if (string.IsNullOrEmpty(_RegisterPassword))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Password"));
                return;
            }
            if (_RegisterPassword.Length < 6)
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Password with more than 6 Characters"));
                return;
            }
            if (_RegisterUsername.Length < 6)
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter User Name with more than 6 Characters"));
                return;
            }
            if(_RegisterEmailAddress.Contains("@") == false) 
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter a valid Email"));
                return;
            }

            //telling buttons to stop interacting
            registerMenuLoginButtonReference.interactable = false;
            registerMenuBackButtonReference.interactable = false;

            //Request for Player Registration
            var registerRequest = new RegisterPlayFabUserRequest
            {
                Username = _RegisterUsername,
                DisplayName = _RegisterUsername,
                Email = _RegisterEmailAddress,
                Password = _RegisterPassword
            };

            //Sending registration Request
            PlayFabClientAPI.RegisterPlayFabUser(
                registerRequest,
                    resultCallback: result => 
                    {
                        print("Registration success");
                        SetupWinLossDataOnRegistration();
                        //UpdateContantEmailAddressFunction();
                    },
                    errorCallback: error =>
                    {
                        print("Fail to register");
                        StartCoroutine(Routine_DisplayMessage(error.ErrorMessage));

                        registerMenuLoginButtonReference.interactable = true;
                        registerMenuBackButtonReference.interactable = true;
                    }
            );
        }
        private void SetupWinLossDataOnRegistration()
        {
            var executeCloudScriptRequest = new ExecuteCloudScriptRequest
            {
                FunctionName = "setupWinLossData"
            };
            PlayFabClientAPI.ExecuteCloudScript(
                executeCloudScriptRequest,
                    result => 
                    {
                        //Getting result and message from server
                        var serializedResult = JObject.Parse
                        (
                         PluginManager.GetPlugin<ISerializerPlugin>
                        (PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult)
                        );
                        print(serializedResult["message"]);

                        StartCoroutine(Routine_DisplayMessage("Registration was successfull"));
                        StartCoroutine(Routine_RegistrationSuccessful());
                    },
                    error => { print(error.ErrorMessage); }
            );
        }

        private IEnumerator Routine_RegistrationSuccessful()
        {
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        #endregion

        #region Account Recovery Menu Functions
        [Header("Account Recovery Menu Variables")]
        [SerializeField] private Button AccountRecoveryMenuBackButtonReference;
        [SerializeField] private Button AccountRecoveryMenuRecoveryButtonReference;

        private string _AccountRecoveryMenuEmailAddress;

        private void OnAccountRecoverySendEmailButtonClicked()
        {
            if (string.IsNullOrEmpty(_AccountRecoveryMenuEmailAddress))
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter Email Address"));
                return;
            }
            if (_AccountRecoveryMenuEmailAddress.Contains("@") == false)
            {
                StartCoroutine(Routine_DisplayMessage("Please Enter a valid Email"));
                return;
            }
            AccountRecoveryMenuBackButtonReference.interactable = false;
            AccountRecoveryMenuRecoveryButtonReference.interactable = false;

            var accountRecoveryRequest = new SendAccountRecoveryEmailRequest 
            {
                Email = _AccountRecoveryMenuEmailAddress,
                TitleId = "52462"
            };
            PlayFabClientAPI.SendAccountRecoveryEmail(
                accountRecoveryRequest,
                result => 
                {
                    StartCoroutine(Routine_SendEmailRoutine());
                    StartCoroutine(Routine_DisplayMessage("Email to Reset Password has been sent. Please check your email"));
                },
                error => 
                {
                    print(error.ErrorMessage);
                    StartCoroutine(Routine_DisplayMessage(error.ErrorMessage));
                    AccountRecoveryMenuBackButtonReference.interactable = true;
                    AccountRecoveryMenuRecoveryButtonReference.interactable = true;
                }
                );
        }

        public void OnAccountRecoverySendEmailInputFieldValueChanged(string valueChanged)
        {
            _AccountRecoveryMenuEmailAddress = valueChanged;
        }

        private void SetupAccountRecoveryMenuButtons()
        {
            AccountRecoveryMenuBackButtonReference.onClick.AddListener(DeactivateOthersScreensExceptMainMenu);
            AccountRecoveryMenuRecoveryButtonReference.onClick.AddListener(OnAccountRecoverySendEmailButtonClicked);
        }

        private IEnumerator Routine_SendEmailRoutine()
        {
            yield return new WaitForSeconds(2.0f);
            AccountRecoveryMenuBackButtonReference.interactable = true;
            AccountRecoveryMenuRecoveryButtonReference.interactable = true;
            DeactivateOthersScreensExceptMainMenu();
        }
        #endregion

        #region Update Contact Email Address functions
        private void UpdateContantEmailAddressFunction()
        {
            var updateContactEmailRequest = new AddOrUpdateContactEmailRequest
            {
                EmailAddress = _RegisterEmailAddress,
            };

            PlayFabClientAPI.AddOrUpdateContactEmail
            (
                updateContactEmailRequest,
                    result =>
                    {
                        print("Contact Email Updated Successfully");
                    },
                    error => 
                    {
                        print(error.ErrorMessage);
                    }
            );
        }
        #endregion

        #region Functions for error Messages
        private IEnumerator Routine_DisplayMessage(string message)
        {
            messageErrorText.gameObject.SetActive(true);
            messageErrorText.text = message;
            yield return new WaitForSeconds(3.0f);
            messageErrorText.gameObject.SetActive(false);
        }
        #endregion

        #region References to Objects, Scripts, and Components
        [Header("References to Objects, Scripts, and Components")]
        [SerializeField] private GameObject mainMenuReference;
        [SerializeField] private GameObject registerMenuReference;
        [SerializeField] private GameObject loginMenuReference;
        [SerializeField] private GameObject accountRecoveryMenuReference;
        [SerializeField] private TextMeshProUGUI messageErrorText;
        #endregion

        #region Unity Functions
        private void Start()
        {
            DeactivateOthersScreensExceptMainMenu();
            SetupMainMenuButtons();
            SetupLoginMenuButtons();
            SetupRegisterMenuButtons();
            SetupAccountRecoveryMenuButtons();

            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = "52462";
            }
        }
        #endregion
    }
}

