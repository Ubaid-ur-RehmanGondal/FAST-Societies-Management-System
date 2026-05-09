using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Forms.Shared;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Admin
{
    public class AdminDashboardForm : Form
    {
        private readonly AdminBLL _adminBll;

        private Label lblWelcome = null!;
        private Button btnLogout = null!;
        
        private Label lblReportSummary = null!;

        private Label lblUsersHeader = null!;
        private DataGridView dgvUsers = null!;
        private Button btnToggleUserStatus = null!;

        private Label lblSocietiesHeader = null!;
        private DataGridView dgvSocieties = null!;
        private Button btnApproveSociety = null!;
        private Button btnSuspendSociety = null!;

        private Label lblPendingEventsHeader = null!;
        private DataGridView dgvPendingEvents = null!;
        private Button btnApproveEvent = null!;

        private static readonly Color HeaderColor = Color.FromArgb(33, 37, 41);
        private static readonly Color PrimaryColor = Color.DodgerBlue;

        public AdminDashboardForm()
        {
            _adminBll = new AdminBLL();
            InitializeForm();
            BuildAllControls();
            Load += (s, e) => LoadDashboardData();
        }

        private void InitializeForm()
        {
            Text = "FAST Societies — Admin Dashboard";
            ClientSize = new Size(1150, 1000);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildAllControls()
        {
            BuildHeaderAndReport();
            BuildUsersPanel();
            BuildSocietiesPanel();
            BuildEventsPanel();

            Controls.AddRange(new Control[]
            {
                lblWelcome, btnLogout, lblReportSummary,
                lblUsersHeader, dgvUsers, btnToggleUserStatus,
                lblSocietiesHeader, dgvSocieties, btnApproveSociety, btnSuspendSociety,
                lblPendingEventsHeader, dgvPendingEvents, btnApproveEvent
            });
        }

        private void BuildHeaderAndReport()
        {
            lblWelcome = new Label
            {
                Text = $"Admin Dashboard - {SessionManager.LoggedInUserName}",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 18),
                Size = new Size(800, 40)
            };

            btnLogout = CreateButton("Logout", new Point(1020, 22), Color.FromArgb(220, 53, 69));
            btnLogout.Click += BtnLogout_Click;

            lblReportSummary = new Label
            {
                Text = "Loading statistics...",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                Location = new Point(24, 60),
                Size = new Size(800, 30)
            };
        }

        private void BuildUsersPanel()
        {
            lblUsersHeader = new Label
            {
                Text = "Manage Users",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(24, 110),
                Size = new Size(200, 25)
            };

            dgvUsers = CreateGrid(new Point(24, 140), new Size(600, 250));
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserId", DataPropertyName = "UserId", Visible = false });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", DataPropertyName = "FullName", HeaderText = "Name", FillWeight = 40 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", DataPropertyName = "Role", HeaderText = "Role", FillWeight = 30 });
            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsActive", DataPropertyName = "IsActive", HeaderText = "Active?", FillWeight = 30 });

            btnToggleUserStatus = CreateButton("Toggle Status", new Point(24, 400), PrimaryColor);
            btnToggleUserStatus.Size = new Size(130, 30);
            btnToggleUserStatus.Click += BtnToggleUserStatus_Click;
        }

        private void BuildSocietiesPanel()
        {
            lblSocietiesHeader = new Label
            {
                Text = "Manage Societies",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(24, 450),
                Size = new Size(200, 25)
            };

            dgvSocieties = CreateGrid(new Point(24, 480), new Size(850, 200));
            dgvSocieties.Columns.Add(new DataGridViewTextBoxColumn { Name = "SocietyId", DataPropertyName = "SocietyId", Visible = false });
            dgvSocieties.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", HeaderText = "Society", FillWeight = 40 });
            dgvSocieties.Columns.Add(new DataGridViewTextBoxColumn { Name = "HeadFullName", DataPropertyName = "HeadFullName", HeaderText = "Head", FillWeight = 30 });
            dgvSocieties.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Status", FillWeight = 30 });

            btnApproveSociety = CreateButton("Approve", new Point(24, 690), Color.SeaGreen);
            btnApproveSociety.Click += BtnApproveSociety_Click;

            btnSuspendSociety = CreateButton("Suspend", new Point(140, 690), Color.Crimson);
            btnSuspendSociety.Click += BtnSuspendSociety_Click;
        }

        private void BuildEventsPanel()
        {
            lblPendingEventsHeader = new Label
            {
                Text = "Pending Events",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(24, 740),
                Size = new Size(200, 25)
            };

            dgvPendingEvents = CreateGrid(new Point(24, 770), new Size(850, 150));
            dgvPendingEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventId", DataPropertyName = "EventId", Visible = false });
            dgvPendingEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Title", FillWeight = 30 });
            dgvPendingEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "SocietyName", DataPropertyName = "SocietyName", HeaderText = "Society", FillWeight = 30 });
            dgvPendingEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventDate", DataPropertyName = "EventDate", HeaderText = "Date", FillWeight = 20, DefaultCellStyle = { Format = "dd MMM yyyy" } });
            dgvPendingEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Status", FillWeight = 20 });

            btnApproveEvent = CreateButton("Approve Event", new Point(24, 940), Color.SeaGreen);
            btnApproveEvent.Size = new Size(150, 30);
            btnApproveEvent.Click += BtnApproveEvent_Click;
        }

        private DataGridView CreateGrid(Point location, Size size)
        {
            var grid = new DataGridView
            {
                Location = location, Size = size, BackgroundColor = Color.White,
                AllowUserToAddRows = false, ReadOnly = true, RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 36, RowTemplate = { Height = 30 }
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            return grid;
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            var btn = new Button
            {
                Text = text, Location = location, Size = new Size(100, 30),
                BackColor = backColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadDashboardData()
        {
            try
            {
                var report = _adminBll.GetUniversityWideReport();
                lblReportSummary.Text = $"Stats: {report.TotalStudents} Active Students | {report.TotalActiveSocieties} Active Societies | {report.TotalEvents} Events";

                dgvUsers.DataSource = null;
                dgvUsers.DataSource = _adminBll.GetAllUsers();

                dgvSocieties.DataSource = null;
                dgvSocieties.DataSource = _adminBll.GetAllSocieties();

                dgvPendingEvents.DataSource = null;
                dgvPendingEvents.DataSource = _adminBll.GetPendingEvents();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnToggleUserStatus_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.DataBoundItem is not UserModel user)
                {
                    MessageBox.Show("Please select a user.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool newStatus = !user.IsActive;
                _adminBll.UpdateUserStatus(user.UserId, newStatus);
                
                string statStr = newStatus ? "Active" : "Suspended";
                MessageBox.Show($"User {user.FullName} is now {statStr}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDashboardData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error toggling user status.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApproveSociety_Click(object? sender, EventArgs e)
        {
            ProcessSocietyStatus("Active");
        }

        private void BtnSuspendSociety_Click(object? sender, EventArgs e)
        {
            ProcessSocietyStatus("Suspended");
        }

        private void ProcessSocietyStatus(string newStatus)
        {
            try
            {
                if (dgvSocieties.CurrentRow == null || dgvSocieties.CurrentRow.DataBoundItem is not SocietyModel soc)
                {
                    MessageBox.Show("Please select a society.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _adminBll.UpdateSocietyStatus(soc.SocietyId, newStatus);
                MessageBox.Show($"Society is now {newStatus}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDashboardData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error processing society.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApproveEvent_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvPendingEvents.CurrentRow == null || dgvPendingEvents.CurrentRow.DataBoundItem is not EventModel ev)
                {
                    MessageBox.Show("Please select an event.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _adminBll.ApproveEvent(ev.EventId);
                MessageBox.Show("Event approved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDashboardData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Logout", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SessionManager.EndSession();
                var login = new LoginForm();
                login.FormClosed += (_, _) => Application.ExitThread();
                Hide();
                login.Show();
            }
        }
    }
}
