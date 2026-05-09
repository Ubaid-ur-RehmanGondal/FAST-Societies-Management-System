using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Student
{
    /// <summary>
    /// Form for students to view events and register.
    /// Built programmatically without .Designer.cs.
    /// Adheres to 40 lines per method rule by breaking down UI generation.
    /// </summary>
    public class StudentEventsForm : Form
    {
        private readonly EventBLL _eventBll;
        private readonly Form _dashboardForm;

        private Label lblEventsHeader = null!;
        private Label lblTicketsHeader = null!;
        private DataGridView dgvEvents = null!;
        private DataGridView dgvMyTickets = null!;
        private Button btnRegister = null!;
        private Button btnViewTicket = null!;
        private Button btnBack = null!;

        private static readonly Color BgColor = Color.FromArgb(245, 247, 250);
        private static readonly Color HeaderColor = Color.FromArgb(33, 37, 41);
        private static readonly Color PrimaryColor = Color.DodgerBlue;

        public StudentEventsForm(Form dashboardForm)
        {
            _eventBll = new EventBLL();
            _dashboardForm = dashboardForm;
            InitializeForm();
            BuildAllControls();
            Load += StudentEventsForm_Load;
            FormClosed += (s, e) => _dashboardForm.Show();
        }

        private void InitializeForm()
        {
            Text = "FAST Societies — Events";
            ClientSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);
            BackColor = BgColor;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildAllControls()
        {
            BuildHeaders();
            BuildGrids();
            BuildButtons();

            Controls.AddRange(new Control[]
            {
                lblEventsHeader, lblTicketsHeader,
                dgvEvents, dgvMyTickets,
                btnRegister, btnViewTicket, btnBack
            });
        }

        private void BuildHeaders()
        {
            lblEventsHeader = new Label
            {
                Text = "Upcoming Events",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(24, 20),
                Size = new Size(500, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTicketsHeader = new Label
            {
                Text = "My Tickets",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = HeaderColor,
                Location = new Point(624, 20),
                Size = new Size(360, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private void BuildGrids()
        {
            dgvEvents = CreateGrid(new Point(24, 60), new Size(580, 520));
            ConfigureEventColumns(dgvEvents);

            dgvMyTickets = CreateGrid(new Point(624, 60), new Size(450, 520));
            ConfigureTicketColumns(dgvMyTickets);
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
            StyleGridHeaders(grid);
            return grid;
        }

        private void StyleGridHeaders(DataGridView grid)
        {
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 226, 255);
            grid.DefaultCellStyle.SelectionForeColor = HeaderColor;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
        }

        private void ConfigureEventColumns(DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventId", DataPropertyName = "EventId", Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", DataPropertyName = "Title", HeaderText = "Title", FillWeight = 30 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "SocietyName", DataPropertyName = "SocietyName", HeaderText = "Society", FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventDate", DataPropertyName = "EventDate", HeaderText = "Date", FillWeight = 25, DefaultCellStyle = { Format = "dd MMM yy HH:mm" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Capacity", DataPropertyName = "Capacity", HeaderText = "Cap", FillWeight = 10 });
        }

        private void ConfigureTicketColumns(DataGridView grid)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TicketCode", DataPropertyName = "TicketCode", HeaderText = "Ticket Code", FillWeight = 30 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventTitle", DataPropertyName = "EventTitle", HeaderText = "Event", FillWeight = 40 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventDate", DataPropertyName = "EventDate", HeaderText = "Date", FillWeight = 30, DefaultCellStyle = { Format = "dd MMM yy" } });
        }

        private void BuildButtons()
        {
            btnRegister = CreateButton("Register for Event", new Point(24, 600), PrimaryColor);
            btnRegister.Click += BtnRegister_Click;

            btnViewTicket = CreateButton("View Ticket", new Point(624, 600), Color.FromArgb(108, 117, 125));
            btnViewTicket.Click += BtnViewTicket_Click;

            btnBack = CreateButton("Back to Dashboard", new Point(220, 600), Color.White);
            btnBack.ForeColor = HeaderColor;
            btnBack.FlatAppearance.BorderColor = Color.LightGray;
            btnBack.FlatAppearance.BorderSize = 1;
            btnBack.Click += (s, e) => Close();
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(180, 38),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData()
        {
            try
            {
                dgvEvents.DataSource = null;
                dgvEvents.DataSource = _eventBll.GetApprovedEvents();

                dgvMyTickets.DataSource = null;
                dgvMyTickets.DataSource = _eventBll.GetMyTickets(SessionManager.LoggedInUserId);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StudentEventsForm_Load(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvEvents.CurrentRow == null || dgvEvents.CurrentRow.DataBoundItem is not EventModel selected)
                {
                    MessageBox.Show("Please select an event.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var confirm = MessageBox.Show($"Register for '{selected.Title}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                _eventBll.RegisterForEvent(SessionManager.LoggedInUserId, selected.EventId);

                MessageBox.Show($"Successfully registered for '{selected.Title}'!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error in BtnRegister_Click.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewTicket_Click(object? sender, EventArgs e)
        {
            if (dgvMyTickets.CurrentRow == null || dgvMyTickets.CurrentRow.DataBoundItem is not EventRegistrationModel ticket)
            {
                MessageBox.Show("Please select a ticket.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ticketForm = new StudentTicketForm(ticket);
            ticketForm.ShowDialog();
        }
    }
}
