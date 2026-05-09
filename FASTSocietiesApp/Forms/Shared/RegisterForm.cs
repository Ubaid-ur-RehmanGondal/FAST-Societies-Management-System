using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Helpers;

namespace FASTSocietiesApp.Forms.Shared
{
    /// <summary>
    /// Self-registration form for new students. Role is forced to 'Student'
    /// by UserBLL.RegisterStudent; this form merely collects input.
    /// Built entirely in code; no .Designer.cs partner file.
    /// </summary>
    public class RegisterForm : Form
    {
        private readonly UserBLL _userBll;

        private Label lblTitle = null!;
        private Label lblFullName = null!;
        private TextBox txtFullName = null!;
        private Label lblEmail = null!;
        private TextBox txtEmail = null!;
        private Label lblPassword = null!;
        private TextBox txtPassword = null!;
        private Label lblConfirmPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private Button btnRegister = null!;
        private Button btnBackToLogin = null!;
        private Label lblError = null!;

        public RegisterForm()
        {
            _userBll = new UserBLL();
            InitializeForm();
            BuildControls();
        }

        private void InitializeForm()
        {
            Text = "FAST Societies — Register";
            ClientSize = new Size(500, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 9F);
        }

        private void BuildControls()
        {
            lblTitle = new Label
            {
                Text = "Create Student Account",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 20),
                Size = new Size(460, 40)
            };

            lblFullName = new Label
            {
                Text = "Full Name",
                Location = new Point(60, 90),
                Size = new Size(120, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            txtFullName = new TextBox
            {
                Name = "txtFullName",
                Location = new Point(180, 90),
                Size = new Size(260, 23),
                MaxLength = 100
            };

            lblEmail = new Label
            {
                Text = "Email",
                Location = new Point(60, 130),
                Size = new Size(120, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            txtEmail = new TextBox
            {
                Name = "txtEmail",
                Location = new Point(180, 130),
                Size = new Size(260, 23),
                MaxLength = 100
            };

            lblPassword = new Label
            {
                Text = "Password",
                Location = new Point(60, 170),
                Size = new Size(120, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(180, 170),
                Size = new Size(260, 23),
                MaxLength = 100,
                PasswordChar = '\u25CF'
            };

            lblConfirmPassword = new Label
            {
                Text = "Confirm Password",
                Location = new Point(60, 210),
                Size = new Size(120, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };
            txtConfirmPassword = new TextBox
            {
                Name = "txtConfirmPassword",
                Location = new Point(180, 210),
                Size = new Size(260, 23),
                MaxLength = 100,
                PasswordChar = '\u25CF'
            };

            btnRegister = new Button
            {
                Name = "btnRegister",
                Text = "Register",
                Location = new Point(180, 270),
                Size = new Size(125, 32)
            };
            btnRegister.Click += BtnRegister_Click;

            btnBackToLogin = new Button
            {
                Name = "btnBackToLogin",
                Text = "Back to Login",
                Location = new Point(315, 270),
                Size = new Size(125, 32)
            };
            btnBackToLogin.Click += BtnBackToLogin_Click;

            lblError = new Label
            {
                Name = "lblError",
                Text = string.Empty,
                ForeColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 330),
                Size = new Size(460, 60),
                Visible = false
            };

            AcceptButton = btnRegister;
            CancelButton = btnBackToLogin;
            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblFullName, txtFullName,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword,
                btnRegister, btnBackToLogin,
                lblError
            });
        }

        /// <summary>
        /// Handles the Register click. Delegates all validation, hashing,
        /// and persistence to UserBLL.RegisterStudent. Displays errors via
        /// the inline label and, on success, a confirmation MessageBox.
        /// </summary>
        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            HideError();

            try
            {
                int newUserId = _userBll.RegisterStudent(
                    txtFullName.Text,
                    txtEmail.Text,
                    txtPassword.Text,
                    txtConfirmPassword.Text);

                MessageBox.Show(
                    $"Account created successfully (User ID: {newUserId}). Please log in.",
                    "Registration Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AppException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error in BtnRegister_Click.", ex);
                ShowError("An unexpected error occurred. Please try again.");
            }
        }

        private void BtnBackToLogin_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private void HideError()
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;
        }
    }
}
