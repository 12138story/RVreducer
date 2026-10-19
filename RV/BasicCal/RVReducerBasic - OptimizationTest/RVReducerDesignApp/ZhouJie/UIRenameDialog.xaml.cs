using System.Windows;

namespace RVReducerDesignApp
{
    public partial class UIRenameDialog : Window
    {
        public string NewName { get; private set; }

        public UIRenameDialog(string title, string defaultName)
        {
            InitializeComponent();
            this.Title = title ?? "重命名";
            txtName.Text = defaultName ?? string.Empty;
            txtName.SelectAll();
            txtName.TextChanged += TxtName_TextChanged;
            UpdateOkButton();
        }

        private void TxtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateOkButton();
        }

        private void UpdateOkButton()
        {
            btnOk.IsEnabled = !string.IsNullOrWhiteSpace(txtName.Text);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            NewName = txtName.Text.Trim();
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// 静态帮助方法：显示对话框并返回新名称；用户取消时返回 null。
        /// owner 可为 null。
        /// </summary>
        public static string ShowDialog(Window owner, string title, string defaultName)
        {
            var dlg = new UIRenameDialog(title, defaultName);
            if (owner != null)
                dlg.Owner = owner;
            var r = dlg.ShowDialog();
            return r == true ? dlg.NewName : null;
        }
    }
}