using System.Text.RegularExpressions;

namespace EchoRelay.App.Forms.Controls
{
    public partial class AccessControlListRuleEditor : UserControl
    {
        public string RuleSetName
        {
            get
            {
                if (lblRulesHeader.Text.Length == 0)
                    return "";

                return lblRulesHeader.Text.Substring(0, lblRulesHeader.Text.Length - 1);
            }
            set
            {
                lblRulesHeader.Text = value + ":";
            }
        }
        public string[] RuleSet
        {
            get
            {
                return listRules.Items.OfType<string>().ToArray();
            }
            set
            {
                listRules.Items.Clear();
                listRules.Items.AddRange(value);
            }
        }
        public Regex ruleRegex = new Regex("^[\\.\\:\\*0-9A-Fa-f]+$");

        public event EventHandler? RuleSetChanged;

        public AccessControlListRuleEditor()
        {
            InitializeComponent();
        }

        private bool ValidateRule()
        {
            bool match = ruleRegex.IsMatch(txtRule.Text);
            if (!match)
                MessageBox.Show("Invalid IP address filter. You must provide a string representing IPv4/IPv6 form (wildcards (\"*\") are allowed).");
            return match;
        }

        private void listRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRule.Text = listRules.SelectedItem?.ToString() ?? "";
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listRules.SelectedItem != null)
            {
                if (!ValidateRule()) return;
                string ruleText = txtRule.Text;
                listRules.Items.Remove(listRules.SelectedItem);
                listRules.SelectedIndex = listRules.Items.Add(ruleText);
                RuleSetChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateRule()) return;
            listRules.SelectedIndex = listRules.Items.Add(txtRule.Text);
            RuleSetChanged?.Invoke(this, EventArgs.Empty);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listRules.SelectedItem != null)
            {
                int index = listRules.SelectedIndex;
                listRules.Items.Remove(listRules.SelectedItem);
                listRules.SelectedIndex = Math.Min(index, listRules.Items.Count - 1);
                RuleSetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
