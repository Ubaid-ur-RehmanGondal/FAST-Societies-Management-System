using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Forms.Shared;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Society
{
    public class SocietyHeadDashboardForm : Form
    {
        private readonly SocietyBLL _societyBll;
        private readonly EventBLL _eventBll;
        private int _societyId;

        private Label lblWelcome = null!;
        private Button btnLogout = null!;
        
        private Label lblRequestsHeader = null!;
        private DataGridView dgvPendingRequests = null!;
        private Button btnApprove = null!;
        private Button btnReject = null!;

        private Label lblEventsHeader = null!;
        private DataGridView dgvSocietyEvents = null!;
        private Button btnCreateEvent = null!;

        private static readonly Color HeaderColor = Color.FromArgb(33, 37, 41);
        private static readonly Color PrimaryColor = Color.DodgerBlue;

        public SocietyHeadDashboardForm()
        {
            _societyBll = new SocietyBLL();
            _eventBll = new EventBLL();
            InitializeForm();
            LoadSocietyData();
            BuildAllControls();
            Load += (s, e) => LoadDashboardData();
        }

        private void InitializeForm()
        {
            Text = "FAST Societies — Society Head Dashboard";
            ClientSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F);
        }

        private void LoadSocietyData()
        {
            var allSocieties = _societyBll.GetActiveSocieties();
            var mySociety = allSocieties.FirstOrDefault(s => s.HeadUserId == SessionManager.LoggedInUserId);
            if (mySociety == null)
            {
                MessageBox.Show("You are not assigned to any active society.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _societyId = 0;
            }
            else
            {
                _societyId = mySociety.SocietyId;
            }
        }

        private void BuildAllControls()
        {
            BuildHeaderBar();
            BuildRequestsPanel();
            BuildEventsPanel();

            Controls.AddRange(new Control[]
            {
                lblWelcome, btnLogout,
                lblRequestsHeader, dgvPendingRequests, btnApprove, btnReject,
                lblEventsHeader, dgvSocietyEvents, btnCreateEvent
            });
        }

        private void BuildHeaderBar()
        {
            lblWelcome = new Label
            {
                Text = $"Welcome, {SessionManager.LoggedInUserName}!",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 18),
                Size = new Size(800, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnLogout = new Button
            {
                Text = "Logout",
                Location = new Point(980, 22),
                Size = new Size(96, 34),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
        }

        private void BuildRequestsPanel()
        {
            lblRequestsHeader = new Label
            {
                Text = "Pending Membership Requests",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 78),
                Size = new Size(500, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvPendingRequests = CreateGrid(new Point(24, 108), new Size(680, 240));
            dgvPendingRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "MembershipId", DataPropertyName = "MembershipId", Visible = false });
            dgvPendingRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "ApplicantFullName", DataPropertyName = "ApplicantFullName", HeaderText = "Applicant", FillWeight = 60 });
            dgvPendingRequests.Columns.Add(new DataGridViewTextBoxColumn { Name = "AppliedAt", DataPropertyName = "AppliedAt", HeaderText = "Applied On", FillWeight = 40, DefaultCellStyle = { Format = "dd MMM yyyy" } });

            btnApprove = CreateButton("Approve", new Point(24, 360), Color.SeaGreen);
            btnApprove.Click += BtnApprove_Click;

            btnReject = CreateButton("Reject", new Point(140, 360), Color.Crimson);
            btnReject.Click += BtnReject_Click;
        }

        private void BuildEventsPanel()
        {
            lblEventsHeader = new Label
            {
                Text = "Society Events",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 420),
                Size = new Size(500, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvSocietyEvents = CreateGrid(new Point(24, 450), new Size(680, 180));
            dgvSocietyEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventId", DataPropertyName = "EventId", Visible = false });
            dgvSocietyEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Title", FillWeight = 40 });
            dgvSocietyEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventDate", DataPropertyName = "EventDate", HeaderText = "Date", FillWeight = 30, DefaultCellStyle = { Format = "dd MMM yyyy" } });
            dgvSocietyEvents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Status", FillWeight = 30 });

            btnCreateEvent = CreateButton("Create Event", new Point(24, 640), PrimaryColor);
            btnCreateEvent.Size = new Size(150, 30);
            btnCreateEvent.Click += BtnCreateEvent_Click;
        }

        private DataGridView CreateGrid(Point location, Size size)
        {
            var grid = new DataGridView
            {
                Location = location,
                Size = size,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 }
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 226, 255);
            grid.DefaultCellStyle.SelectionForeColor = HeaderColor;
            return grid;
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(100, 30),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadDashboardData()
        {
            if (_societyId <= 0) return;

            try
            {
                dgvPendingRequests.DataSource = null;
                dgvPendingRequests.DataSource = _societyBll.GetPendingRequests(_societyId);

                var events = _eventBll.GetApprovedEvents().Where(e => e.SocietyId == _societyId).ToList();
                dgvSocietyEvents.DataSource = null;
                dgvSocietyEvents.DataSource = events;
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnApprove_Click(object? sender, EventArgs e)
        {
            ProcessMembership("Approved");
        }

        private void BtnReject_Click(object? sender, EventArgs e)
        {
            ProcessMembership("Rejected");
        }

        private void ProcessMembership(string status)
        {
            try
            {
                if (dgvPendingRequests.CurrentRow == null || dgvPendingRequests.CurrentRow.DataBoundItem is not MembershipModel req)
                {
                    MessageBox.Show("Please select a pending request.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _societyBll.UpdateMembershipStatus(req.MembershipId, status);
                MessageBox.Show($"Request {status} successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDashboardData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Action Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error processing membership.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateEvent_Click(object? sender, EventArgs e)
        {
            if (_societyId <= 0) return;
            using var form = new CreateEventForm(_societyId);
            form.ShowDialog(this);
            LoadDashboardData();
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
