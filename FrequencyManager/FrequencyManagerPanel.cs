using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SDRSharp.Radio;
using SDRSharp.Common;

namespace SDRSharp.FrequencyManager
{

    public delegate void RadioInfo(object sender, MemoryInfoEventArgs e);

    [DesignTimeVisible(true)]
    [Category("SDRSharp")]
    [Description("RF Memory Management Panel")]
    public partial class FrequencyManagerPanel : UserControl
    {       
        private readonly SortableBindingList<MemoryEntry> _displayedEntries = new SortableBindingList<MemoryEntry>();
        private readonly List<MemoryEntry> _entries;
        private readonly SettingsPersister _settingsPersister;
        private readonly List<string> _groups = new List<string>();
        private const string AllGroups = "[All Groups]";
        private const string FavouriteGroup = "[Favourites]";

        private ISharpControl _controlInterface;

        public FrequencyManagerPanel(ISharpControl control)
        {
            InitializeComponent();

            _controlInterface = control;

            if (LicenseManager.UsageMode==LicenseUsageMode.Runtime)
            {
                _settingsPersister = new SettingsPersister();
                _entries = _settingsPersister.ReadStoredFrequencies();
                _groups = GetGroupsFromEntries();
                ProcessGroups(null);
            }

            memoryEntryBindingSource.DataSource = _displayedEntries;            
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
                _displayedEntries.Remove(entry);
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
                    if ((string)comboGroups.SelectedItem == AllGroups || (string)comboGroups.SelectedItem == memoryEntry.GroupName ||
                        ((string)comboGroups.SelectedItem == FavouriteGroup && memoryEntry.IsFavourite))
                    {
                        if (isNew)
                            _displayedEntries.Add(memoryEntry);                            
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
                e.Value = GetFrequencyDisplay(frequency);
                e.FormattingApplied = true;
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
            comboGroups.Items.Add(FavouriteGroup);
            comboGroups.Items.AddRange(_groups.ToArray());
            if (selectedGroupName != null)
                comboGroups.SelectedItem = selectedGroupName;
            else
                comboGroups.SelectedIndex = 0;
        }

        private void comboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            memoryEntryBindingSource.Clear();
            _displayedEntries.Clear();
            if (comboGroups.SelectedIndex != -1)
            {
                var selectedGroup = (string) comboGroups.SelectedItem;
                
                foreach (MemoryEntry entry in _entries)
                {
                    if (selectedGroup == AllGroups || entry.GroupName == selectedGroup || (selectedGroup == FavouriteGroup && entry.IsFavourite ))
                    {
                       
                        _displayedEntries.Add(entry);
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
            
            //if (_controlInterface.Frequency == 0) return;
            if (!_controlInterface.IsPlaying) return;

            var memoryEntry = new MemoryEntry();

            memoryEntry.DetectorType = _controlInterface.DetectorType;
            memoryEntry.CenterFrequency = _controlInterface.CenterFrequency;
            memoryEntry.Frequency = _controlInterface.Frequency;
            memoryEntry.FilterBandwidth = _controlInterface.FilterBandwidth;
            memoryEntry.Shift = _controlInterface.FrequencyShiftEnabled ? _controlInterface.FrequencyShift : 0;
            
            memoryEntry.GroupName = "Misc";
            if (_controlInterface.DetectorType == DetectorType.WFM)
            {
                var stationName = _controlInterface.RdsProgramService.Trim();
                memoryEntry.Name = string.Empty;
                if (!string.IsNullOrEmpty(stationName))
                {
                    memoryEntry.Name = stationName;
                }
                else
                {
                    memoryEntry.Name = GetFrequencyDisplay(_controlInterface.Frequency) + " " + memoryEntry.DetectorType;
                }
            }
            else
            {
                memoryEntry.Name = GetFrequencyDisplay(_controlInterface.Frequency) + " " + memoryEntry.DetectorType;
            }
            memoryEntry.IsFavourite = true;
            DoEdit(memoryEntry, true);
        }

        public void Navigate()
        {            
            //if (_controlInterface.Frequency == 0) return;
            if (!_controlInterface.IsPlaying) return;


            var rowIndex = frequencyDataGridView.SelectedCells.Count > 0 ? frequencyDataGridView.SelectedCells[0].RowIndex : -1;
            if (rowIndex != -1)
            {
                var memoryEntry = (MemoryEntry)memoryEntryBindingSource.List[rowIndex];

                _controlInterface.DetectorType = memoryEntry.DetectorType;
                _controlInterface.CenterFrequency = memoryEntry.CenterFrequency;
                _controlInterface.Frequency = memoryEntry.Frequency;
                _controlInterface.FilterBandwidth = (int) memoryEntry.FilterBandwidth;
                _controlInterface.FrequencyShiftEnabled = memoryEntry.Shift != 0;
                _controlInterface.FrequencyShift = memoryEntry.Shift;
            }
        }

        private static string GetFrequencyDisplay(long frequency)
        {
            string result;
            var absFrequency = Math.Abs(frequency);
            if (absFrequency == 0)
            {
                result = "DC";
            }
            else if (absFrequency > 1500000000)
            {
                result = string.Format("{0:#,0.000 000} GHz", frequency / 1000000000.0);
            }
            else if (absFrequency > 30000000)
            {
                result = string.Format("{0:0,0.000#} MHz", frequency / 1000000.0);
            }
            else if (absFrequency > 1000)
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
