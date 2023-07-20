using TMPro;
using UnityEngine;

namespace Wrapper
{
    public class QuantimeLoginScreen : LoginScreen
    {
        [SerializeField] private QButton loginButton;
        //[SerializeField] private TMP_InputField field;
        //[SerializeField] private GameObject errorGO;
        //[SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Animator animator;

        [Header("Error Text")]
        [SerializeField] private string connectionErrorText = "Login failed: Please check your internet connection and try again.";
        [SerializeField] private string formatErrorText = "Login failed: Research codes must be 6 characters long and only contain alphanumerics. (Codes are case-sensitive)";
        [SerializeField] private string databaseErrorText = "Login failed: Could not connect to the user database. Please check your internet connection and try again.";
        [SerializeField] private string nonexistentUserErrorText = "Login failed: There is no user associated with this research code. Please verify your code and try again. (Codes are case-sensitive)";


        //private void Awake()
        //{
        //    loginButton.onClick.AddListener(SubmitCode);    
        //}

        //private void OnEnable()
        //{
        //    Events.OpenLoginScreen += Open;
        //    Events.UpdateLoginStatus += UpdateLoginScreen;
        //}

        //private void OnDisable()
        //{
        //    Events.OpenLoginScreen -= Open;
        //    Events.UpdateLoginStatus -= UpdateLoginScreen;
        //}

        //private void Open()
        //{
        //    animator.SetBool("On", true);
        //}

        ///// <summary> Called by animation event </summary>
        //private void Close()
        //{
        //    gameObject.SetActive(false);
        //}

        override protected void SubmitCode()
        {
            //Debug.Log("hi");
            Events.SubmitResearchCode?.Invoke("dgm001");
            animator.SetBool("Loading", true);
        }

        //private void UpdateLoginScreen(LoginStatus status)
        //{
        //    animator.SetBool("Loading", false);

        //    switch (status)
        //    {
        //        case LoginStatus.Success: animator.SetBool("On", false); break;
        //        case LoginStatus.FormatError: DisplayError(formatErrorText); break;
        //        case LoginStatus.DatabaseError: DisplayError(databaseErrorText); break;
        //        case LoginStatus.ConnectionError: DisplayError(connectionErrorText); break;
        //        case LoginStatus.NonExistentUserError: DisplayError(nonexistentUserErrorText); break;
        //    }
        //}

        //private void DisplayError(string text)
        //{
        //    errorText.text = text;
        //    errorGO.SetActive(true);
        //    animator.SetTrigger("Error");
        //}
    }
}
