using EncryptionTool.models;

namespace winforms
{
    public partial class PasswordForm : Form
    {
        public string password = string.Empty;
        bool moveForm;
        Point moveFormPosition = Point.Empty;

        public PasswordForm(IEnumerable<string> paths, CommandAction action = CommandAction.encrypt)
        {
            InitializeComponent();

            // Shorten and display paths
            foreach (string path in paths)
            {
                var splitPath = path.Split('\\');
                var compactPath = splitPath.Length >= 2
                    ? $".../{splitPath[^2]}/{splitPath[^1]}"
                    : path;

                list_paths.Items.Add(compactPath);
            };
            input_password.Focus();

            if (action == CommandAction.decrypt) proceed_btn.Text = "Decrypt";
        }

        // Window bar stuff

        void window_bar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveForm = true;
                moveFormPosition = e.Location;
            }
        }

        void window_bar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveForm = false;
            }
        }

        void window_bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
            {
                var currentScreenPos = PointToScreen(e.Location);

                Location = new Point(
                    currentScreenPos.X - moveFormPosition.X,
                    currentScreenPos.Y - moveFormPosition.Y);
            }
        }

        // Password input

        void input_password_TextChanged(object sender, EventArgs e)
        {
            password = input_password.Text;
        }

        // Form controls

        void proceed_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        void cancel_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        void close_btn_MouseUp(object sender, MouseEventArgs e)
        {
            var mousePos = close_btn.PointToClient(Cursor.Position);

            if (close_btn.ClientRectangle.Contains(mousePos)) Close();
        }
    }
}
