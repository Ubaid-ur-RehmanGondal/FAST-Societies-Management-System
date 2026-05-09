using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Forms.Shared;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Student
{
    /// <summary>
    /// Landing page for an authenticated Student. Shows the catalogue of
    /// active societies on the left, the student's own membership history
    /// on the right, and exposes Apply / View Events / Logout actions.
    /// Built entirely in code; no .Designer.cs partner file.
    /// </summary>
    public class StudentDashboardForm : Form
    {
        private readonly SocietyBLL _societyBll;

        private Label lblWelcome = null!;
        private Button btnLogout = null!;
        private Label lblSocietiesHeader = null!;
        private Label lblMembershipsHeader = null!;
        private DataGridView dgvSocieties = null!;
        private DataGridView dgvMyMemberships = null!;
        private Button btnApply = null!;
        private Button btnViewEvents = null!;
        private Button btnRefresh = null!;

        private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
        private static readonly Color HeaderColor = Color.FromArgb(33, 37, 41);
        private static readonly Color PrimaryColor = Color.DodgerBlue;
        private static readonly Color SecondaryColor = Color.FromArgb(108, 117, 125);
        private static readonly Color DangerColor = Color.FromArgb(220, 53, 69);

        public StudentDashboardForm()
        {
            _societyBll = new SocietyBLL();
            InitializeForm();
            BuildControls();
            Load += StudentDashboardForm_Load;
        }

        private void InitializeForm()
        {
            Text = "FAST Societies — Student Dashboard";
            ClientSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);
            BackColor = BackgroundColor;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildControls()
        {
            BuildHeadersAndLogout();
            BuildGrids();
            BuildButtons();

            Controls.AddRange(new Control[]
            {
                lblWelcome, btnLogout,
                lblSocietiesHeader, dgvSocieties,
                btnApply, btnViewEvents, btnRefresh,
                lblMembershipsHeader, dgvMyMemberships
            });
        }

        private void BuildHeadersAndLogout()
        {
            lblWelcome = new Label
            {
                Name = "lblWelcome",
                Text = $"Welcome, {SessionManager.LoggedInUserName}!",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = HeaderColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(24, 18),
                Size = new Size(800, 40)
            };

            btnLogout = new Button
            {
                Name = "btnLogout",
                Text = "Logout",
                Location = new Point(980, 22),
                Size = new Size(96, 34),
                BackColor = DangerColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;

            lblSocietiesHeader = new Label
            {
                Text = "Active Societies",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 78),
                Size = new Size(500, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblMembershipsHeader = new Label
            {
                Text = "My Memberships",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(724, 78),
                Size = new Size(360, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private void BuildGrids()
        {
            dgvSocieties = new DataGridView
            {
                Name = "dgvSocieties",
                Location = new Point(24, 108),
                Size = new Size(680, 380),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 }
            };
            StyleGridHeaders(dgvSocieties);
            ConfigureSocietyColumns(dgvSocieties);

            dgvMyMemberships = new DataGridView
            {
                Name = "dgvMyMemberships",
                Location = new Point(724, 108),
                Size = new Size(352, 430),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 }
            };
            StyleGridHeaders(dgvMyMemberships);
            ConfigureMembershipColumns(dgvMyMemberships);
        }

        private void BuildButtons()
        {
            btnApply = new Button
            {
                Name = "btnApply",
                Text = "Apply for Membership",
                Location = new Point(24, 500),
                Size = new Size(220, 38),
                BackColor = PrimaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;

            btnViewEvents = new Button
            {
                Name = "btnViewEvents",
                Text = "View Events",
                Location = new Point(254, 500),
                Size = new Size(150, 38),
                BackColor = SecondaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewEvents.FlatAppearance.BorderSize = 0;
            btnViewEvents.Click += BtnViewEvents_Click;

            btnRefresh = new Button
            {
                Name = "btnRefresh",
                Text = "Refresh",
                Location = new Point(414, 500),
                Size = new Size(120, 38),
                BackColor = Color.White,
                ForeColor = HeaderColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.LightGray;
            btnRefresh.FlatAppearance.BorderSize = 1;
            btnRefresh.Click += (s, e) => LoadDashboardData();
        }

        private static void StyleGridHeaders(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 226, 255);
            grid.DefaultCellStyle.SelectionForeColor = HeaderColor;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
        }

        private static void ConfigureSocietyColumns(DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SocietyId",
                DataPropertyName = nameof(SocietyModel.SocietyId),
                HeaderText = "ID",
                Visible = false
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = nameof(SocietyModel.Name),
                HeaderText = "Society Name",
                FillWeight = 28
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                DataPropertyName = nameof(SocietyModel.Description),
                HeaderText = "Description",
                FillWeight = 44
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HeadFullName",
                DataPropertyName = nameof(SocietyModel.HeadFullName),
                HeaderText = "Society Head",
                FillWeight = 18
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = nameof(SocietyModel.Status),
                HeaderText = "Status",
                FillWeight = 10
            });
        }

        private static void ConfigureMembershipColumns(DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SocietyName",
                DataPropertyName = nameof(MembershipModel.SocietyName),
                HeaderText = "Society",
                FillWeight = 50
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = nameof(MembershipModel.Status),
                HeaderText = "Status",
                FillWeight = 22
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AppliedAt",
                DataPropertyName = nameof(MembershipModel.AppliedAt),
                HeaderText = "Applied On",
                FillWeight = 28,
                DefaultCellStyle = { Format = "dd MMM yyyy" }
            });
        }

        /// <summary>
        /// Loads (or reloads) the data for both grids. Centralised so apply,
        /// refresh, and form-load all flow through the same path.
        /// </summary>
        private void LoadDashboardData()
        {
            try
            {
                if (!SessionManager.IsLoggedIn)
                {
                    throw new AppException("Your session has expired. Please log in again.");
                }

                List<SocietyModel> societies = _societyBll.GetActiveSocieties();
                List<MembershipModel> memberships =
                    _societyBll.GetMyMemberships(SessionManager.LoggedInUserId);

                dgvSocieties.DataSource = null;
                dgvSocieties.DataSource = societies;

                dgvMyMemberships.DataSource = null;
                dgvMyMemberships.DataSource = memberships;
            }
            catch (AppException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Could Not Load Dashboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error loading student dashboard.", ex);
                MessageBox.Show(
                    "An unexpected error occurred. Please try again.",
                    "FAST Societies",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StudentDashboardForm_Load(object? sender, EventArgs e)
        {
            LoadDashboardData();
        }

        /// <summary>
        /// Reads the selected society id from the grid and asks the BLL to
        /// submit a membership application. On success, refreshes both grids.
        /// </summary>
        private void BtnApply_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvSocieties.CurrentRow == null ||
                    dgvSocieties.CurrentRow.DataBoundItem is not SocietyModel selected)
                {
                    MessageBox.Show(
                        "Please select a society to apply to.",
                        "No Society Selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    $"Submit a membership application to '{selected.Name}'?",
                    "Confirm Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                _societyBll.ApplyForMembership(
                    SessionManager.LoggedInUserId, selected.SocietyId);

                MessageBox.Show(
                    $"Your application to '{selected.Name}' has been submitted and is pending approval.",
                    "Application Submitted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDashboardData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Application Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error in BtnApply_Click.", ex);
                MessageBox.Show(
                    "An unexpected error occurred. Please try again.",
                    "FAST Societies",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnViewEvents_Click(object? sender, EventArgs e)
        {
            StudentEventsForm eventsForm = new StudentEventsForm(this);
            this.Hide();
            eventsForm.Show();
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            SessionManager.EndSession();
            Logger.Info("Student logged out.");

            LoginForm login = new LoginForm();
            login.FormClosed += (_, _) => Application.ExitThread();
            Hide();
            login.Show();
        }
    }
}
