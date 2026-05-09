using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Society
{
    public class CreateEventForm : Form
    {
        private readonly EventBLL _eventBll;
        private readonly int _societyId;

        private Label lblHeader = null!;
        private TextBox txtTitle = null!;
        private TextBox txtDescription = null!;
        private TextBox txtVenue = null!;
        private DateTimePicker dtpEventDate = null!;
        private NumericUpDown numCapacity = null!;
        private Button btnSave = null!;

        public CreateEventForm(int societyId)
        {
            _societyId = societyId;
            _eventBll = new EventBLL();
            InitializeForm();
            BuildControls();
        }

        private void InitializeForm()
        {
            Text = "Create Event";
            ClientSize = new Size(400, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildControls()
        {
            lblHeader = new Label { Text = "Create New Event", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(360, 40) };
            
            var lblTitle = new Label { Text = "Title:", Location = new Point(20, 80), Size = new Size(100, 25) };
            txtTitle = new TextBox { Location = new Point(130, 80), Size = new Size(230, 25) };

            var lblDesc = new Label { Text = "Description:", Location = new Point(20, 120), Size = new Size(100, 25) };
            txtDescription = new TextBox { Location = new Point(130, 120), Size = new Size(230, 60), Multiline = true };

            var lblVenue = new Label { Text = "Venue:", Location = new Point(20, 200), Size = new Size(100, 25) };
            txtVenue = new TextBox { Location = new Point(130, 200), Size = new Size(230, 25) };

            var lblDate = new Label { Text = "Date:", Location = new Point(20, 240), Size = new Size(100, 25) };
            dtpEventDate = new DateTimePicker { Location = new Point(130, 240), Size = new Size(230, 25), Format = DateTimePickerFormat.Short };

            var lblCap = new Label { Text = "Capacity:", Location = new Point(20, 280), Size = new Size(100, 25) };
            numCapacity = new NumericUpDown { Location = new Point(130, 280), Size = new Size(230, 25), Maximum = 10000, Minimum = 1, Value = 100 };

            btnSave = new Button { Text = "Save Event", Location = new Point(130, 340), Size = new Size(150, 35), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[] { lblHeader, lblTitle, txtTitle, lblDesc, txtDescription, lblVenue, txtVenue, lblDate, dtpEventDate, lblCap, numCapacity, btnSave });
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                var ev = new EventModel
                {
                    SocietyId = _societyId,
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Venue = txtVenue.Text.Trim(),
                    EventDate = dtpEventDate.Value,
                    Capacity = (int)numCapacity.Value
                };

                _eventBll.CreateEvent(ev);
                MessageBox.Show("Event created successfully! Pending admin approval.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error creating event.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
