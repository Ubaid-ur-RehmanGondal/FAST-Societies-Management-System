using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Forms.Student;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Shared
{
    /// <summary>
    /// Entry-point form. Collects email + password, delegates authentication
    /// to UserBLL, and on success initialises SessionManager.
    /// Built entirely in code; there is no .Designer.cs partner file.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly UserBLL _userBll;

        private Label lblTitle = null!;
        private Label lblEmail = null!;
        private TextBox txtEmail = null!;
        private Label lblPassword = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Button btnGoToRegister = null!;
        private Label lblError = null!;

        public LoginForm()
        {
            _userBll = new UserBLL();
            InitializeForm();
            BuildControls();
        }

        /// <summary>
        /// Configures the form-level properties (size, title, behaviour).
        /// </summary>
        private void InitializeForm()
        {
            Text = "FAST Societies — Login";
            ClientSize = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 9F);
        }

        /// <summary>
        /// Instantiates and lays out every control, then attaches events.
        /// </summary>
        private void BuildControls()
        {
            lblTitle = new Label
            {
                Text = "FAST Societies Management System",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 30),
                Size = new Size(460, 40)
            };

            lblEmail = new Label
            {
                Text = "Email",
                Location = new Point(90, 100),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtEmail = new TextBox
            {
                Name = "txtEmail",
                Location = new Point(170, 100),
                Size = new Size(240, 23),
                MaxLength = 100
            };

            lblPassword = new Label
            {
                Text = "Password",
                Location = new Point(90, 145),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtPassword = new TextBox
            {
                Name = "txtPassword",
                Location = new Point(170, 145),
                Size = new Size(240, 23),
                MaxLength = 100,
                PasswordChar = '\u25CF'
            };

            btnLogin = new Button
            {
                Name = "btnLogin",
                Text = "Login",
                Location = new Point(170, 200),
                Size = new Size(150, 32)
            };
            btnLogin.Click += BtnLogin_Click;

            btnGoToRegister = new Button
            {
                Name = "btnGoToRegister",
                Text = "Don't have an account? Register",
                Location = new Point(140, 245),
                Size = new Size(220, 28),
                FlatStyle = FlatStyle.Flat
            };
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.Click += BtnGoToRegister_Click;

            lblError = new Label
            {
                Name = "lblError",
                Text = string.Empty,
                ForeColor = Color.Red,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 300),
                Size = new Size(460, 40),
                Visible = false
            };

            AcceptButton = btnLogin;
            Controls.AddRange(new Control[]
            {
                lblTitle, lblEmail, txtEmail, lblPassword, txtPassword,
                btnLogin, btnGoToRegister, lblError
            });
        }

        /// <summary>
        /// Handles the Login click. Calls UserBLL, on success starts the
        /// session and shows a confirmation MessageBox. Phase 1 stops here;
        /// Phase 2+ will route to the role-specific dashboard.
        /// </summary>
        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            HideError();

            try
            {
                UserModel? user = _userBll.AuthenticateUser(txtEmail.Text, txtPassword.Text);

                if (user == null)
                {
                    ShowError("Invalid email or password.");
                    return;
                }

                UserRole role = _userBll.ParseRole(user.Role);
                SessionManager.StartSession(user.UserId, role, user.FullName);

                OpenDashboardForRole(role);
            }
            catch (AppException ex)
            {
                ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error in BtnLogin_Click.", ex);
                ShowError("An unexpected error occurred. Please try again.");
            }
        }

        private void OpenDashboardForRole(UserRole role)
        {
            Form? dashboard = role switch
            {
                UserRole.Student => new StudentDashboardForm(),
                UserRole.SocietyHead => new FASTSocietiesApp.Forms.Society.SocietyHeadDashboardForm(),
                UserRole.Admin => new FASTSocietiesApp.Forms.Admin.AdminDashboardForm(),
                _ => null
            };

            if (dashboard == null)
            {
                MessageBox.Show(
                    $"You are logged in as {role}. The dashboard for this role is not implemented yet.",
                    "Login Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            Hide();
            dashboard.FormClosed += (_, _) => Close();
            dashboard.Show();
        }

        private void BtnGoToRegister_Click(object? sender, EventArgs e)
        {
            try
            {
                using RegisterForm registerForm = new RegisterForm();
                Hide();
                registerForm.ShowDialog(this);
                Show();
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to open RegisterForm.", ex);
                ShowError("Could not open the registration screen.");
                Show();
            }
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
