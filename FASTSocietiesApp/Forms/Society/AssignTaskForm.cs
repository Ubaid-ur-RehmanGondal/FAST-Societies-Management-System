using FASTSocietiesApp.BLL;
using FASTSocietiesApp.Helpers;
using FASTSocietiesApp.Models;

namespace FASTSocietiesApp.Forms.Society
{
    public class AssignTaskForm : Form
    {
        private readonly TaskBLL _taskBll;
        private readonly SocietyBLL _societyBll;
        private readonly int _societyId;

        private Label lblHeader = null!;
        private TextBox txtTitle = null!;
        private TextBox txtDescription = null!;
        private DateTimePicker dtpDueDate = null!;
        private ComboBox cmbMembers = null!;
        private Button btnAssign = null!;

        public AssignTaskForm(int societyId)
        {
            _societyId = societyId;
            _taskBll = new TaskBLL();
            _societyBll = new SocietyBLL();
            InitializeForm();
            BuildControls();
            Load += AssignTaskForm_Load;
        }

        private void InitializeForm()
        {
            Text = "Assign Task";
            ClientSize = new Size(400, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5F);
        }

        private void BuildControls()
        {
            lblHeader = new Label { Text = "Assign New Task", Font = new Font("Segoe UI", 16F, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(360, 40) };
            
            var lblTitle = new Label { Text = "Task Name:", Location = new Point(20, 80), Size = new Size(100, 25) };
            txtTitle = new TextBox { Location = new Point(130, 80), Size = new Size(230, 25) };

            var lblDesc = new Label { Text = "Description:", Location = new Point(20, 120), Size = new Size(100, 25) };
            txtDescription = new TextBox { Location = new Point(130, 120), Size = new Size(230, 60), Multiline = true };

            var lblDate = new Label { Text = "Deadline:", Location = new Point(20, 200), Size = new Size(100, 25) };
            dtpDueDate = new DateTimePicker { Location = new Point(130, 200), Size = new Size(230, 25), Format = DateTimePickerFormat.Short };

            var lblMember = new Label { Text = "Assign To:", Location = new Point(20, 240), Size = new Size(100, 25) };
            cmbMembers = new ComboBox { Location = new Point(130, 240), Size = new Size(230, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMembers.DisplayMember = "ApplicantFullName";
            cmbMembers.ValueMember = "UserId";

            btnAssign = new Button { Text = "Assign Task", Location = new Point(130, 300), Size = new Size(150, 35), BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            btnAssign.FlatAppearance.BorderSize = 0;
            btnAssign.Click += BtnAssign_Click;

            Controls.AddRange(new Control[] { lblHeader, lblTitle, txtTitle, lblDesc, txtDescription, lblDate, dtpDueDate, lblMember, cmbMembers, btnAssign });
        }

        private void AssignTaskForm_Load(object? sender, EventArgs e)
        {
            try
            {
                var members = _societyBll.GetApprovedMembers(_societyId);
                if (members.Count == 0)
                {
                    MessageBox.Show("There are no approved members to assign tasks to.", "No Members", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAssign.Enabled = false;
                }
                cmbMembers.DataSource = members;
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error Loading Members", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAssign_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbMembers.SelectedValue == null)
                {
                    MessageBox.Show("Please select a member.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var task = new TaskModel
                {
                    SocietyId = _societyId,
                    AssignedToUserId = (int)cmbMembers.SelectedValue,
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    DueDate = dtpDueDate.Value
                };

                _taskBll.AssignTask(task);
                MessageBox.Show("Task assigned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error assigning task.", ex);
                MessageBox.Show("An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
