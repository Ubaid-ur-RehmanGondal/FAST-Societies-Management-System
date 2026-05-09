using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Student
{
    /// <summary>
    /// Displays the details of a registered event ticket.
    /// Built programmatically without .Designer.cs.
    /// </summary>
    public class StudentTicketForm : Form
    {
        private readonly EventRegistrationModel _ticket;

        private Label lblHeader = null!;
        private Label lblTicketCodeTitle = null!;
        private Label lblTicketCodeValue = null!;
        private Label lblEventTitleTitle = null!;
        private Label lblEventTitleValue = null!;
        private Label lblDateTitle = null!;
        private Label lblDateValue = null!;
        private Label lblVenueTitle = null!;
        private Label lblVenueValue = null!;
        private Button btnClose = null!;

        public StudentTicketForm(EventRegistrationModel ticket)
        {
            _ticket = ticket;
            InitializeForm();
            BuildControls();
        }

        private void InitializeForm()
        {
            Text = "Ticket Details";
            ClientSize = new Size(400, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildControls()
        {
            lblHeader = new Label
            {
                Text = "Event Ticket",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Location = new Point(0, 20),
                Size = new Size(400, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            int startY = 80;
            int gapY = 40;

            lblTicketCodeTitle = CreateTitleLabel("Ticket Code:", startY);
            lblTicketCodeValue = CreateValueLabel(_ticket.TicketCode, startY, true);

            lblEventTitleTitle = CreateTitleLabel("Event:", startY + gapY);
            lblEventTitleValue = CreateValueLabel(_ticket.EventTitle, startY + gapY, false);

            lblDateTitle = CreateTitleLabel("Date:", startY + gapY * 2);
            lblDateValue = CreateValueLabel(_ticket.EventDate.ToString("f"), startY + gapY * 2, false);

            lblVenueTitle = CreateTitleLabel("Venue:", startY + gapY * 3);
            lblVenueValue = CreateValueLabel(_ticket.Venue ?? "TBA", startY + gapY * 3, false);

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(140, startY + gapY * 4 + 20),
                Size = new Size(120, 35),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblHeader,
                lblTicketCodeTitle, lblTicketCodeValue,
                lblEventTitleTitle, lblEventTitleValue,
                lblDateTitle, lblDateValue,
                lblVenueTitle, lblVenueValue,
                btnClose
            });
        }

        private Label CreateTitleLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(108, 117, 125),
                Location = new Point(30, y),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleRight
            };
        }

        private Label CreateValueLabel(string text, int y, bool isHighlight)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, isHighlight ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isHighlight ? Color.DodgerBlue : Color.Black,
                Location = new Point(140, y),
                Size = new Size(230, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
        }
    }
}
