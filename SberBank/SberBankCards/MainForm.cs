using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Parking.Data;
using Parking.SberBank.Extensions;
using Parking.SberBank.Properties;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Services;

namespace Parking.SberBank
{
    using Internal;

    /// <summary>
  /// Окно сервера
  /// </summary>
  public partial class MainForm : InvisibleForm
  {
    #region [ const ]

    private const string StatusStateReady = "Готов";
    private const string StatusNetworkType = "Сеть";

    private const string GroupExtensionsText = "Расширения";
    private const string GroupMonitorText = "Мониторинг";

    private const string CaptionExtensionAttach = "Подключить";
    private const string CaptionExtensionDetach = "Отключить";

    private const int ImageIndexExtension = 2;
    private const int ImageIndexExtensionOff = 3;
    private const int ImageIndexMonitoring = 4;
    private const int ImageIndexWarning = 5;

    private static readonly Color _colorDisabled = Color.LightGray;
    private static readonly Color _colorNormal = SystemColors.ControlText;
    private static readonly Color _colorWarning = Color.Red;
    private static readonly Color _colorWarningBack = Color.FromArgb(248, 220, 220);

    #endregion

    private ISberBankApplication sberBank;
    private SettingsForm settingsForm;
    private AboutForm aboutForm;
    private readonly InputForm inputForm;
    private readonly ToolStripStateTimer stateTimer;
    private readonly Dictionary<int, HotKeyAction> actions;
    private ContextMenuStrip menuMain;

    #region [ properties ]

    [InjectionProperty]
    private IApplication SberBank
    {
      set
      {
        sberBank = (ISberBankApplication)value;

        sberBank.AfterRun += (state) =>
        {
          this.SafeInvoke(new Action<object>(OnAfterRun), state);
        };
        sberBank.BeforeShutdown += OnBeforeShutdown;
        sberBank.SettingsChanged += OnSettingsChanged;
        sberBank.RuntimeExtensionsLoaded += (e, f) =>
        {
          this.SafeInvoke(new Action<IEnumerable<ISberBankExtension>, int>(OnRuntimeExtensionsLoaded), e, f);
        };
        sberBank.ExtensionControlCompleted += (e, a, r) =>
        {
          this.SafeInvoke(new Action<ISberBankExtension, bool, bool>(OnExtensionControlCompleted), e, a, r);
        };
        sberBank.GetThreadState += () => (ThreadState)this.SafeInvoke(new Func<ThreadState>(OnGetThreadState));
      }
    }

    [InjectionProperty(Required = false, Name = ApplicationFormServices.AboutWindowName)]
    private Form AboutForm
    {
      set { aboutForm = (AboutForm)value; }
    }

    [InjectionProperty(ExactBinding = false, Required = false)]
    private GenericTray Tray
    {
      set
      {
        if (value == null)
          return;

        menuMain = value.Menu;
        if (menuMain != null)
        {
          value.MenuClick += OnMenuClick;
          linkMenu.Enabled = true;
        }
      }
    }

 #endregion

    public MainForm()
      : base(true) 
    {
        InitializeComponent();

      //this.SetLogoIcon();
      this.SetLogoImages(picTop, status);
      UpdateCaption();

      tslblStatusState.SetLogoForeColor();
      tslblStatusNetwork.SetLogoForeColor();

      imgServices.Images.Add(Images.OK);
      imgServices.Images.Add(Images.Error);
      imgServices.Images.Add(Images.Extension);
      imgServices.Images.Add(Images.Extension.MakeGrayscale());
      imgServices.Images.Add(Images.Find);
      imgServices.Images.Add(Images.Warning);

      lblExtensionsWarning.ImageIndex = ImageIndexWarning;
      lblMonitorWarning.ImageIndex = ImageIndexWarning;

      var lvgc = lvwMonitor.Groups;
      lvgc.Clear();
      foreach (var mt in RuntimeHelper.GetEnumElements<MonitoringObjectCategory>())
        lvgc.Add(mt.ToString(), mt.GetString());

      sberBank = null;
      settingsForm = null;
      aboutForm = null;
      inputForm = new InputForm();
      stateTimer = new ToolStripStateTimer(tslblStatusState, TimeSpan.FromSeconds(2));
      stateTimer.DefaultText = StatusStateReady;
      stateTimer.UpdateStateText();

      var chordHandler = new ChordKeyHandler(this);
      actions = new Dictionary<int, HotKeyAction>();
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.F), HotKeyAction.OpenLogFolder);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.D), HotKeyAction.DeleteLogFiles);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.O), HotKeyAction.OpenLogFile);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.W), HotKeyAction.LogComment);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.R), HotKeyAction.LogRuntimeInfo);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.D, Keys.N), HotKeyAction.NetworkControl);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.S, Keys.E), HotKeyAction.ShowExtensionFilter);
      actions.Add(chordHandler.AddChord(Keys.Control, Keys.L, Keys.C), HotKeyAction.LogControl);
      chordHandler.ChordPressed += OnChordPressed;

      menuMain = null;
    }

    #region [ window events ]

    private void picTop_Click(object sender, EventArgs e)
    {
      if (aboutForm != null)
        aboutForm.ShowAbout(this);
    }

    private void tabMain_Selected(object sender, TabControlEventArgs e)
    {
      if (e.TabPage == tabPageMonitor)
        UpdateMonitoring(false);
    }

    private void lvwExtensions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      ListViewItem lvi = null;
      if (Safe.GetCount(lvwExtensions.SelectedItems) > 0)
        lvi = lvwExtensions.SelectedItems[0];

      EnableExtensionControl(e.IsSelected ? lvi : null, (lvi == null) || IsExtensionAttached(lvi));
    }

    private void lvwExtensions_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //ProcessListViewDoubleClick(lvwExtensions, e);
    }

    private void lvwExtensions_MouseClick(object sender, MouseEventArgs e)
    {
      //ProcessListViewClick(lvwExtensions, e);
    }

    private void btnExtensionsControl_Click(object sender, EventArgs e)
    {
      ListViewItem lvi = null;
      if (Safe.GetCount(lvwExtensions.SelectedItems) > 0)
        lvi = lvwExtensions.SelectedItems[0];

      if (lvi == null)
        return;

      if (!CheckTechnicalPassword())
        return;

      SetExtensionBusy(lvi, true);
      bool attached = IsExtensionAttached(lvi);
      EnableExtensionControl(lvi, attached);
      sberBank.ExtensionControl((ISberBankExtension)lvi.Tag, !attached);
      lvwExtensions.Select();
    }

    private void btnExtensionsLoad_Click(object sender, EventArgs e)
    {
      if (!CheckTechnicalPassword())
        return;

      OpenFileDialog ofd = new OpenFileDialog();
      ofd.RestoreDirectory = true;
      ofd.Filter = "Расширения сервера (*.dll)|*.dll|Все файлы (*.*)|*.*";
      ofd.Multiselect = false;
      if (ofd.ShowDialog(this) != DialogResult.OK)
        return;

      btnExtensionsLoad.Enabled = false;
      stateTimer.UpdateStateText();
      sberBank.LoadExtensionsRuntime(ofd.FileName);
      lvwExtensions.Select();
    }

    private void btnMonitoringRefresh_Click(object sender, EventArgs e)
    {
      UpdateMonitoring(false);
    }

    private void btnMonitoringReset_Click(object sender, EventArgs e)
    {
      if (!CheckTechnicalPassword())
        return;

      UpdateMonitoring(true);
    }

    private void timerMonitoring_Tick(object sender, EventArgs e)
    {
      if (!Visible)
        return;

      //if (tabMain.SelectedTab == tabPageMonitor)
        UpdateMonitoring(false);
    }

    private void lvwMonitor_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //ProcessListViewDoubleClick(lvwMonitor, e);
    }

    private void linkMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      menuMain.ShowAtControl(linkMenu);
    }

    #endregion

    #region [ application events ]

    private void OnAfterRun(object state)
    {
      UpdateCaption();

      //status
      UpdateStatusNetwork(sberBank.Network);

      //extensions tab
      AppendExtensionsToList(sberBank.Extensions.Cast<ISberBankExtension>().OrderBy<ISberBankExtension, int>(ext => ext.Priority), ((SberBankApplicationState)state).FailedExtensions);
      EnableExtensionControl(null, true);

      //monitoring tab
      foreach (IMonitoringObject mo in GetMonitoringObjects())
        AppendMonitoringObject(mo);

      tabPageMonitor.SetGroupCaption(GroupMonitorText, lvwMonitor.Items.Count);
      lvwMonitor.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      timerMonitoring.Start();
    }

    private void OnBeforeShutdown(object state)
    {
      timerMonitoring.Stop();
    }

    private void OnSettingsChanged(SettingsResult result)
    {
      timerMonitoring.Interval = (int)Settings.Default.MonitorRefreshTime.TotalMilliseconds;
    }

    private void OnRuntimeExtensionsLoaded(IEnumerable<ISberBankExtension> extensions, int failedCount)
    {
      AppendExtensionsToList(extensions, null);

      foreach (var me in extensions.OfType<ISupportMonitoring>())
        foreach (var mo in me.MonitoringObjects)
        {
          mo.Refresh();
          AppendMonitoringObject(mo);
        }

      lvwMonitor.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
      tabPageMonitor.SetGroupCaption(GroupMonitorText, lvwMonitor.Items.Count);

      StringBuilder sb = new StringBuilder(64);
      sb.AppendFormat("Подключено расширений : {0}", extensions.Count());
      if (failedCount > 0)
        sb.AppendFormat(", не подключено : {0}", failedCount);

      stateTimer.UpdateStateText(sb.ToString(), true);
      btnExtensionsLoad.Enabled = true;
    }

    private void OnExtensionControlCompleted(ISberBankExtension extension, bool attached, bool result)
    {
      var lvi = lvwExtensions.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == extension);
      if (lvi == null)
        return;

      SetExtensionBusy(lvi, false);
      if (result)
      {
        SetExtensionAttached(lvi, attached);
        UpdateExtensionsGroupCaption();
        if (lvi.Selected)
          EnableExtensionControl(lvi, attached);
      }
    }

    private ThreadState OnGetThreadState()
    {
      return Thread.CurrentThread.ThreadState;
    }

    #endregion

    #region [ extensions ]

    private void AppendExtensionsToList(IEnumerable<ISberBankExtension> extensions, IEnumerable<string> failedExtensions)
    {
      lvwExtensions.SuspendLayout();
      lvwExtensions.BeginUpdate();

      foreach (ISberBankExtension extension in extensions)
      {
        ListViewItem lvi = lvwExtensions.Items.Add((string)null);
        lvi.Tag = extension;
        lvi.ImageIndex = ImageIndexExtension;
        lvi.SubItems.Add(extension.Name);
        lvi.SubItems.Add(extension.Description);
        var ext = extension;
        extension.Notification += (l, m) =>
        {
          this.SafeInvoke(new Action<ISberBankExtension, LogLevel, string>(OnExtensionNotifiation), ext, l, m);
        };

        //ISupportVerbs sv = extension as ISupportVerbs;
        //if (sv != null)
        //  GetVerbMenuContainer(lvi).Tag = CreateVerbsMenu(sv);
      }

      lvwExtensions.EndUpdate();
      lvwExtensions.ResumeLayout(true);

      UpdateExtensionsGroupCaption();

      if (failedExtensions != null)
      {
        bool anyExtensionsFailed = failedExtensions.Any();
        tabPageExtensions.ImageIndex = anyExtensionsFailed ? ImageIndexWarning : -1;
        lblExtensionsWarning.Visible = anyExtensionsFailed;
        if (anyExtensionsFailed)
        {
          StringBuilder sb = GetPaddedStringBuilder();
          sb.AppendFormat("Не удалось загрузить расширения : {0}", failedExtensions.GetString());
          lblExtensionsWarning.Text = sb.ToString();
        }
      }
    }

    private bool IsExtensionAttached(ListViewItem lvi)
    {
      return (lvi.ForeColor != _colorDisabled);
    }

    private void SetExtensionAttached(ListViewItem lvi, bool state)
    {
      lvi.ForeColor = state ? _colorNormal : _colorDisabled;
      lvi.ImageIndex = state ? ImageIndexExtension : ImageIndexExtensionOff;
    }

    private bool IsExtensionBusy(ListViewItem lvi)
    {
      return (GetExtensionStateContainer(lvi).Tag != null);
    }

    private void SetExtensionBusy(ListViewItem lvi, bool state)
    {
      GetExtensionStateContainer(lvi).Tag = state ? this : null;
    }

    private void EnableExtensionControl(ListViewItem lvi, bool attached)
    {
      btnExtensionsControl.Enabled = ((lvi != null) && (lvi.Tag is ISberBankExtension) && !IsExtensionBusy(lvi));
      btnExtensionsControl.Text = attached ? CaptionExtensionDetach : CaptionExtensionAttach;
    }

    private void UpdateExtensionsGroupCaption()
    {
      StringBuilder sb = new StringBuilder();
      int c = lvwExtensions.Items.Count;
      if (c > 0)
      {
        sb.Append(c);
        int a = lvwExtensions.Items.Cast<ListViewItem>().Count(lvi => IsExtensionAttached(lvi));
        if (a != c)
          sb.Insert(0, String.Format("{0}/", a));
      }

      tabPageExtensions.SetGroupCaption(GroupExtensionsText, sb.ToString());
    }

    private void OnExtensionNotifiation(ISberBankExtension extension, LogLevel level, string message)
    {
      IEnumerable<ListViewItem> items = lvwExtensions.Items.Cast<ListViewItem>();
      ListViewItem lvi = items.FirstOrDefault(i => i.Tag == extension);
      if (lvi == null)
        return;

      var warning = (level == LogLevel.Error);
      lvi.BackColor = warning ? _colorWarningBack : lvwExtensions.BackColor;
      if (!warning)
        warning = items.Any(i => i.BackColor == _colorWarningBack);

      tabPageExtensions.ImageIndex = warning ? ImageIndexWarning : -1;
    }

    private ListViewItem.ListViewSubItem GetExtensionStateContainer(ListViewItem lvi)
    {
      return lvi.SubItems[1];
    }

    #endregion

    #region [ monitoring ]

    private void UpdateMonitoring(bool reset)
    {
      lvwMonitor.SuspendLayout();
      lvwMonitor.BeginUpdate();

      bool isWarning = false;
      foreach (ListViewItem lvi in lvwMonitor.Items)
      {
        var mo = (IMonitoringObject)lvi.Tag;
        if (reset)
          mo.Reset();

        mo.Refresh();
        var lvsi = lvi.SubItems[2];
        lvsi.Text = mo.Value;
        switch (mo.State)
        {
            case MonitoringObjectState.Warning:
                lvsi.ForeColor = _colorWarning;
                break;
            case MonitoringObjectState.Idle:
                lvsi.ForeColor = _colorDisabled;
                break;
        }
        
        if (mo.State == MonitoringObjectState.Warning)
          isWarning = true;
      }

      lvwMonitor.EndUpdate();
      lvwMonitor.ResumeLayout(true);

      tabPageMonitor.ImageIndex = isWarning ? ImageIndexWarning : -1;
    }

    private IEnumerable<IMonitoringObject> GetMonitoringObjects()
    {
      var mos = new List<IMonitoringObject>();

      //server
      mos.AddRange(sberBank.MonitoringObjects);

      //extensions
      foreach (var sm in sberBank.Extensions.OfType<ISupportMonitoring>())
        mos.AddRange(sm.MonitoringObjects);

      return mos;
    }

    private void AppendMonitoringObject(IMonitoringObject mo)
    {
      ListViewItem lvi = lvwMonitor.Items.Add(String.Empty);
      lvi.SubItems.Add(mo.Name);
      lvi.SubItems.Add(String.Empty);
      lvi.Group = lvwMonitor.Groups[((MonitoringObjectCategory)mo.Category).ToString()];
      lvi.ImageIndex = ImageIndexMonitoring;
      lvi.Tag = mo;
      lvi.UseItemStyleForSubItems = false;
    }

    #endregion

    #region [ menu ]

    private void OnMenuClick(string name)
    {
      if (name == GenericTray.SettingsItemName)
        OnMenuSettingsClick();
      else if (name == GenericTray.SettingsItemName)
        OnMenuHelpClick();
    }

    private void OnMenuSettingsClick()
    {
      if (settingsForm == null)
        settingsForm = new SettingsForm();

      settingsForm.Setup(Settings.Default, EditType.Custom);
      if (settingsForm.ShowDialog(this) != DialogResult.OK)
        return;

      settingsForm.GetResult<Settings>().Save();
      sberBank.ApplySettings(new SettingsResult(true, false));
    }

    private void OnMenuHelpClick()
    {
      //
    }

    #endregion


    internal void ShowFromTray()
    {
      var tabPage = tabMain
          .TabPages
          .Cast<TabPage>().FirstOrDefault(tp => (tp.ImageIndex == ImageIndexWarning));

      if (tabPage != null)
        tabMain.SelectedTab = tabPage;

      this.Show();
    }

    private void UpdateCaption()
    {
      this.Text = String.Format("{0} {1}", ApplicationServices.GetApplicationDescription(), new AssemblyInfo().Version.ToString());
    }

    private void UpdateStatusNetwork(INetworkProtocol network)
    {
      tslblStatusNetwork.Text = String.Format("{0} : {1}", StatusNetworkType, network);
    }

    private StringBuilder GetPaddedStringBuilder()
    {
      var sb = new StringBuilder();
      sb.Append(' ', 10);

      return sb;
    }

    private void OnChordPressed(int chordId)
    {
      //get action
      HotKeyAction action = actions[chordId];
      stateTimer.UpdateStateText(String.Format("Выполняется {0}...", action), true);

      //get param
      object param = null;
      if (action == HotKeyAction.LogComment)
      {
        inputForm.Style = InputFormStyle.ShowButtons | InputFormStyle.Multiline;
        inputForm.Setup(Common.LogCommentCaption);
        if (inputForm.ShowDialog(this) != DialogResult.OK)
          return;

        param = inputForm.GetResult();
      }
      else if (action == HotKeyAction.DeleteLogFiles)
      {
        if (MessageBox.Show(this, Common.LogDeleteFilesQuestion, ApplicationServices.GetApplicationName(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
          return;
      }
      else if (action == HotKeyAction.ShowExtensionFilter)
      {
        if (CheckTechnicalPassword())
          ShowExtensionFilter();

        return;
      }

      //run
      HotKeyHelper.Run(action, sberBank, param);
    }

    private bool CheckTechnicalPassword()
    {
//#if DEBUG
      return true;
//#else
      //_inputForm.Style = InputFormStyle.TextBoxOnly | InputFormStyle.Password;
      //_inputForm.Setup(Common.TechnicalPasswordCaption, null, InputFilterOptions.Any);
      //if (_inputForm.ShowDialog(this) != DialogResult.OK)
      //  return false;

      //return new Password(Common.TechnicalPasswordSalt, Common.TechnicalPasswordHash).Verify(_inputForm.GetResult().ToCharArray());
//#endif
    }

    private void ShowExtensionFilter()
    {
      TypeFilterAssemblyLoader loader = new TypeFilterAssemblyLoader(sberBank.PathManager.ExtensionsPath,
        TypeFilterAssemblyLoader.GetExtensionSelector<ISberBank, ISberBankExtension>());

      TypeFilterSettings settings = sberBank.ExtensionFilterSettings;

      TypeFilterForm form = new TypeFilterForm();
      form.Text = "Загружаемые расширения";
      form.Setup(new object[] { settings, loader });
      if (form.ShowDialog() != DialogResult.OK)
        return;

      Exception x = form.GetResult<Exception>();
      if (x != null)
        this.ShowError(x);
      else
        sberBank.Logger.Write(LogLevel.Verbose, String.Format("Установлен фильтр расширений, всего {0}", settings.Count));
    }

  }
}