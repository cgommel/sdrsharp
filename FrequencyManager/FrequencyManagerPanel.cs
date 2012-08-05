using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.FrequencyManager
{

    public delegate void RadioInfo(object sender, MemoryInfoEventArgs e);

    [DesignTimeVisible(true)]
    [Category("SDRSharp")]
    [Description("RF Memory Management Panel")]
    public partial class FrequencyManagerPanel : UserControl
    {
        public event RadioInfo MemoryInfoNeeded;
        public event RadioInfo MemoryInfoAvailable;
        private readonly List<MemoryEntry> _entries;
        private readonly SettingsPersister _settingsPersister;
        private readonly List<string> _groups = new List<string>();
        private const string AllGroups = "[All Groups]";

        public FrequencyManagerPanel()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode==LicenseUsageMode.Runtime)
            {
                _settingsPersister = new SettingsPersister();
                _entries = _settingsPersister.ReadStoredFrequencies();
                _groups = GetGroupsFromEntries();
                ProcessGroups(null);
            }
        }

        public String SelectedGroup
        {
            get { return (string)comboGroups.SelectedItem; }
            set
            {
                if (value!=null && comboGroups.Items.IndexOf(value) != -1)
                {
                    comboGroups.SelectedIndex = comboGroups.Items.IndexOf(value);
                }
            }
        }

        private void btnNewEntry_Click(object sender, EventArgs e)
        {
            Bookmark();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (memoryEntryBindingSource.Current != null)
                DoEdit((MemoryEntry)memoryEntryBindingSource.Current, false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var entry = (MemoryEntry) memoryEntryBindingSource.Current;
            if (entry != null && MessageBox.Show("Are you sure that you want to delete '"
              + entry.Name + "'?", "Delete Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _entries.Remove(entry);
                _settingsPersister.PersistStoredFrequencies(_entries);
                memoryEntryBindingSource.Remove(entry);
            }
        }

        private void DoEdit(MemoryEntry memoryEntry, bool isNew)
        {
            var dialog = new DialogEntryInfo(memoryEntry, _groups);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (isNew)
                {
                    _entries.Add(memoryEntry);
                    _entries.Sort((e1, e2) => e1.Frequency.CompareTo(e2.Frequency));
                }
                _settingsPersister.PersistStoredFrequencies(_entries);
                if (!_groups.Contains(memoryEntry.GroupName))
                {
                    _groups.Add(memoryEntry.GroupName);
                    ProcessGroups(memoryEntry.GroupName);
                }
                else
                {
                    if ((string)comboGroups.SelectedItem == AllGroups || (string)comboGroups.SelectedItem == memoryEntry.GroupName)
                    {
                        if (isNew) memoryEntryBindingSource.Add(memoryEntry);
                    }
                    else
                        comboGroups.SelectedItem = memoryEntry.GroupName;
                }
            }
        }

        private List<String> GetGroupsFromEntries()
        {
            var groups = new List<string>();
            foreach (MemoryEntry entry in _entries)
            {
                if (!groups.Contains(entry.GroupName))
                    groups.Add(entry.GroupName);
            }
            return groups;
        }

        private void frequencyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (frequencyDataGridView.Columns[e.ColumnIndex].DataPropertyName == "Frequency" && e.Value != null)
            {
                var frequency = (long)e.Value;
                e.Value = GetFrequencyDisplay(frequency); //frequency.ToString("N0");
                //e.FormattingApplied = true;
            }
        }

        private void frequencyDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Navigate();
        }

        private void ProcessGroups(String selectedGroupName)
        {
            _groups.Sort();
            comboGroups.Items.Clear();
            comboGroups.Items.Add(AllGroups);
            comboGroups.Items.AddRange(_groups.ToArray());
            if (selectedGroupName != null)
                comboGroups.SelectedItem = selectedGroupName;
            else
                comboGroups.SelectedIndex = 0;
        }

        private void comboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            memoryEntryBindingSource.Clear();
            if (comboGroups.SelectedIndex != -1)
            {
                var selectedGroup = (string) comboGroups.SelectedItem;
                foreach (MemoryEntry entry in _entries)
                {
                    if (selectedGroup == AllGroups || entry.GroupName == selectedGroup)
                    {
                        memoryEntryBindingSource.Add(entry);
                    }
                }
            }
        }

        private void frequencyDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = frequencyDataGridView.SelectedRows.Count > 0;
            btnEdit.Enabled = frequencyDataGridView.SelectedRows.Count > 0;
        }

        public void Bookmark()
        {
            var memoryEntry = new MemoryEntry();
            var info = new MemoryInfoEventArgs(memoryEntry);
            if (MemoryInfoNeeded != null)
                MemoryInfoNeeded(this, info);
            if (memoryEntry.Frequency == 0) return;
            memoryEntry.GroupName = "Misc";
            memoryEntry.Name = GetFrequencyDisplay(memoryEntry.Frequency) + " " + memoryEntry.DetectorType;
            DoEdit(memoryEntry, true);
        }

        public void Navigate()
        {
            var rowIndex = frequencyDataGridView.SelectedCells.Count > 0 ? frequencyDataGridView.SelectedCells[0].RowIndex : -1;
            if (rowIndex != -1)
            {
                var memoryEntry = (MemoryEntry) memoryEntryBindingSource.List[rowIndex];
                if (MemoryInfoAvailable != null)
                {
                    MemoryInfoAvailable(this, new MemoryInfoEventArgs(new MemoryEntry(memoryEntry)));
                }
            }
        }

        private static string GetFrequencyDisplay(long frequency)
        {
            string result;
            if (frequency == 0)
            {
                result = "DC";
            }
            else if (Math.Abs(frequency) > 1500000000)
            {
                result = string.Format("{0:#,0.000 000} GHz", frequency / 1000000000.0);
            }
            else if (Math.Abs(frequency) > 30000000)
            {
                result = string.Format("{0:0,0.000#} MHz", frequency / 1000000.0);
            }
            else if (Math.Abs(frequency) > 1000)
            {
                result = string.Format("{0:#,#.###} kHz", frequency / 1000.0);
            }
            else
            {
                result = frequency.ToString();
            }
            return result;
        }

        private void frequencyDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate();
                e.Handled = true;
            }
        }
    }
}
