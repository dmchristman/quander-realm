using TMPro;
using UnityEngine;

namespace Wrapper
{
    public class LoginScreen : MonoBehaviour
    {
        [SerializeField] private QButton loginButton;
        [SerializeField] private TMP_InputField field;
        [SerializeField] private GameObject errorGO;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Animator animator;

        [Header("Error Text")]
        [SerializeField] private string connectionErrorText = "Login failed: Please check your internet connection and try again.";
        [SerializeField] private string formatErrorText = "Login failed: Research codes must be 6 characters long and only contain alphanumerics. (Codes are case-sensitive)";
        [SerializeField] private string databaseErrorText = "Login failed: Could not connect to the user database. Please check your internet connection and try again.";
        [SerializeField] private string nonexistentUserErrorText = "Login failed: There is no user associated with this research code. Please verify your code and try again. (Codes are case-sensitive)";

        private void Awake()
        {
            Events.OpenLoginScreen += Open;
            Events.UpdateLoginStatus += UpdateLoginScreen;
            Events.CloseLoginScreen += Close;

            loginButton.onClick.AddListener(SubmitCode);
            Close();
        }

        private void OnDestroy()
        {
            Events.OpenLoginScreen -= Open;
            Events.UpdateLoginStatus -= UpdateLoginScreen;
            Events.CloseLoginScreen -= Close;
            loginButton.onClick.RemoveListener(SubmitCode);
        }

        private void Open()
        {
            gameObject.SetActive(true);
            field.text = "";
            animator.SetBool("On", true);
        }

        /// <summary> Called by animation event </summary>
        private void Close()
        {
            gameObject.SetActive(false);
        }

        virtual protected void SubmitCode()
        {
            Events.SubmitResearchCode?.Invoke(field.text);
            animator.SetBool("Loading", true);
        }

        private void UpdateLoginScreen(LoginStatus status)
        {
            animator.SetBool("Loading", false);

            switch (status)
            {
                case LoginStatus.Success: 
                    animator.SetBool("On", false);
                    Events.ToggleTitleScreen?.Invoke(true);
                    break;
                case LoginStatus.FormatError: DisplayError(formatErrorText); break;
                case LoginStatus.DatabaseError: DisplayError(databaseErrorText); break;
                case LoginStatus.ConnectionError: DisplayError(connectionErrorText); break;
                case LoginStatus.NonExistentUserError: DisplayError(nonexistentUserErrorText); break;
            }
        }

        private void DisplayError(string text)
        {
            errorText.text = text;
            errorGO.SetActive(true);
            animator.SetTrigger("Error");
        }
    }
}
