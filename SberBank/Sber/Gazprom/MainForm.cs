using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Parking.Data;
using Parking.Gazprom.Extensions;
using Parking.Gazprom.Properties;
using Parking.Monitoring;
using Parking.Network;
using RMLib;
using RMLib.Forms;
using RMLib.Log;
using RMLib.Services;

namespace Parking.Gazprom
{
  /// <summary>
  /// Окно сервера
  /// </summary>
  public partial class MainForm : InvisibleForm
  {
    #region [ const ]

    private const string StatusStateReady = "Готов";
    //private const string StatusStateError = "Ошибка";
    private const string StatusNetworkType = "Сеть";

    private const string GroupExtensionsText = "Расширения";
    private const string GroupMonitorText = "Мониторинг";

    private const string CaptionExtensionAttach = "Подключить";
    private const string CaptionExtensionDetach = "Отключить";
    private const string CaptionVerbInput = "Введите параметры команды";

    private const int ImageIndexOK = 0;
    private const int ImageIndexError = 1;
    private const int ImageIndexExtension = 2;
    private const int ImageIndexExtensionOff = 3;
    private const int ImageIndexMonitoring = 4;
    private const int ImageIndexWarning = 5;

    private static Color ColorDisabled = Color.LightGray;
    private static Color ColorNormal = SystemColors.ControlText;
    private static Color ColorWarning = Color.Red;
    private static Color ColorWarningBack = Color.FromArgb(248, 220, 220);

    #endregion

    private IGazpromApplication _gazprom;
    private SettingsForm _settingsForm;
    private AboutForm _aboutForm;
    private InputForm _inputForm;
    private ToolStripStateTimer _stateTimer;
    private ChordKeyHandler _chordHandler;
    private Dictionary<int, HotKeyAction> _actions;
    private ContextMenuStrip _menuMain;
    //private ITraceWindowSource _traceSource;

    #region [ properties ]

    [InjectionProperty]
    private IApplication Gazprom
    {
      set
      {
        _gazprom = (IGazpromApplication)value;

        _gazprom.AfterRun += (state) =>
        {
          this.SafeInvoke(new Action<object>(OnAfterRun), state);
        };
        _gazprom.BeforeShutdown += OnBeforeShutdown;
        _gazprom.SettingsChanged += OnSettingsChanged;
        _gazprom.RuntimeExtensionsLoaded += (e, f) =>
        {
          this.SafeInvoke(new Action<IEnumerable<IGazpromExtension>, int>(OnRuntimeExtensionsLoaded), e, f);
        };
        _gazprom.ExtensionControlCompleted += (e, a, r) =>
        {
          this.SafeInvoke(new Action<IGazpromExtension, bool, bool>(OnExtensionControlCompleted), e, a, r);
        };
        _gazprom.GetThreadState += () =>
        {
          return (ThreadState)this.SafeInvoke(new Func<ThreadState>(OnGetThreadState));
        };
      }
    }

    [InjectionProperty(Required = false, Name = ApplicationFormServices.AboutWindowName)]
    private Form AboutForm
    {
      set { _aboutForm = (AboutForm)value; }
    }

    [InjectionProperty(ExactBinding = false, Required = false)]
    private GenericTray Tray
    {
      set
      {
        if (value == null)
          return;

        _menuMain = value.Menu;
        if (_menuMain != null)
        {
          value.MenuClick += OnMenuClick;
          linkMenu.Enabled = true;
        }
      }
    }

    //[InjectionProperty(Required = false)]
    //private ITraceWindowSource TraceSource
    //{
    //  set { _traceSource = value; }
    //}

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

      ListViewGroupCollection lvgc = lvwMonitor.Groups;
      lvgc.Clear();
      foreach (MonitoringObjectCategory mt in RuntimeHelper.GetEnumElements<MonitoringObjectCategory>())
        lvgc.Add(mt.ToString(), mt.GetString());

      _gazprom = null;
      _settingsForm = null;
      _aboutForm = null;
      _inputForm = new InputForm();
      _stateTimer = new ToolStripStateTimer(tslblStatusState, TimeSpan.FromSeconds(2));
      _stateTimer.DefaultText = StatusStateReady;
      _stateTimer.UpdateStateText();

      _chordHandler = new ChordKeyHandler(this);
      _actions = new Dictionary<int, HotKeyAction>();
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.F), HotKeyAction.OpenLogFolder);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.D), HotKeyAction.DeleteLogFiles);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.O), HotKeyAction.OpenLogFile);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.W), HotKeyAction.LogComment);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.R), HotKeyAction.LogRuntimeInfo);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.D, Keys.N), HotKeyAction.NetworkControl);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.S, Keys.E), HotKeyAction.ShowExtensionFilter);
      _actions.Add(_chordHandler.AddChord(Keys.Control, Keys.L, Keys.C), HotKeyAction.LogControl);
      _chordHandler.ChordPressed += OnChordPressed;

      _menuMain = null;
      //_traceSource = null;
    }

    #region [ window events ]

    private void picTop_Click(object sender, EventArgs e)
    {
      if (_aboutForm != null)
        _aboutForm.ShowAbout(this);
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

      EnableExtensionControl(e.IsSelected ? lvi : null, (lvi != null) ? IsExtensionAttached(lvi) : true);
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
      _gazprom.ExtensionControl((IGazpromExtension)lvi.Tag, !attached);
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
      _stateTimer.UpdateStateText();
      _gazprom.LoadExtensionsRuntime(ofd.FileName);
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
      if (!this.Visible)
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
      _menuMain.ShowAtControl(linkMenu);
    }

    #endregion

    #region [ application events ]

    private void OnAfterRun(object state)
    {
      UpdateCaption();

      //status
      UpdateStatusNetwork(_gazprom.Network);

      //extensions tab
      AppendExtensionsToList(_gazprom.Extensions.Cast<IGazpromExtension>().OrderBy<IGazpromExtension, int>(ext => ext.Priority), ((GazpromApplicationState)state).FailedExtensions);
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

    private void OnRuntimeExtensionsLoaded(IEnumerable<IGazpromExtension> extensions, int failedCount)
    {
      AppendExtensionsToList(extensions, null);

      foreach (ISupportMonitoring me in extensions.OfType<ISupportMonitoring>())
        foreach (IMonitoringObject mo in me.MonitoringObjects)
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

      _stateTimer.UpdateStateText(sb.ToString(), true);
      btnExtensionsLoad.Enabled = true;
    }

    private void OnExtensionControlCompleted(IGazpromExtension extension, bool attached, bool result)
    {
      ListViewItem lvi = lvwExtensions.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == extension);
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

    private void AppendExtensionsToList(IEnumerable<IGazpromExtension> extensions, IEnumerable<string> failedExtensions)
    {
      lvwExtensions.SuspendLayout();
      lvwExtensions.BeginUpdate();

      foreach (IGazpromExtension extension in extensions)
      {
        ListViewItem lvi = lvwExtensions.Items.Add((string)null);
        lvi.Tag = extension;
        lvi.ImageIndex = ImageIndexExtension;
        lvi.SubItems.Add(extension.Name);
        lvi.SubItems.Add(extension.Description);
        extension.Notification += (l, m) =>
        {
          this.SafeInvoke(new Action<IGazpromExtension, LogLevel, string>(OnExtensionNotifiation), extension, l, m);
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
      return (lvi.ForeColor != ColorDisabled);
    }

    private void SetExtensionAttached(ListViewItem lvi, bool state)
    {
      lvi.ForeColor = state ? ColorNormal : ColorDisabled;
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
      btnExtensionsControl.Enabled = ((lvi != null) && (lvi.Tag is IGazpromExtension) && !IsExtensionBusy(lvi));
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

    private void OnExtensionNotifiation(IGazpromExtension extension, LogLevel level, string message)
    {
      IEnumerable<ListViewItem> items = lvwExtensions.Items.Cast<ListViewItem>();
      ListViewItem lvi = items.FirstOrDefault(i => i.Tag == extension);
      if (lvi == null)
        return;

      bool warning = (level == LogLevel.Error);
      lvi.BackColor = warning ? ColorWarningBack : lvwExtensions.BackColor;
      if (!warning)
        warning = items.Any(i => i.BackColor == ColorWarningBack);

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
        IMonitoringObject mo = (IMonitoringObject)lvi.Tag;
        if (reset)
          mo.Reset();

        mo.Refresh();
        ListViewItem.ListViewSubItem lvsi = lvi.SubItems[2];
        lvsi.Text = mo.Value;
        if (mo.State == MonitoringObjectState.Warning)
          lvsi.ForeColor = ColorWarning;
        else if (mo.State == MonitoringObjectState.Idle)
          lvsi.ForeColor = ColorDisabled;
        
        if (mo.State == MonitoringObjectState.Warning)
          isWarning = true;
      }

      lvwMonitor.EndUpdate();
      lvwMonitor.ResumeLayout(true);

      tabPageMonitor.ImageIndex = isWarning ? ImageIndexWarning : -1;
    }

    private IEnumerable<IMonitoringObject> GetMonitoringObjects()
    {
      List<IMonitoringObject> mos = new List<IMonitoringObject>();

      //server
      mos.AddRange(_gazprom.MonitoringObjects);

      //extensions
      foreach (ISupportMonitoring sm in _gazprom.Extensions.OfType<ISupportMonitoring>())
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
      if (_settingsForm == null)
        _settingsForm = new SettingsForm();

      _settingsForm.Setup(Settings.Default, EditType.Custom);
      if (_settingsForm.ShowDialog(this) != DialogResult.OK)
        return;

      _settingsForm.GetResult<Settings>().Save();
      _gazprom.ApplySettings(new SettingsResult(true, false));
    }

    private void OnMenuHelpClick()
    {
      //
    }

    #endregion

    #region [ verbs ]

//    private ListViewItem.ListViewSubItem GetVerbMenuContainer(ListViewItem lvi)
//    {
//      return lvi.SubItems[2];
//    }

//    private ContextMenuStrip CreateVerbsMenu(ISupportVerbs sv)
//    {
//      ContextMenuStrip menu = new ContextMenuStrip();
//      menu.Tag = sv;

//      foreach (Verb v in sv.GetVerbs())
//      {
//        ToolStripMenuItem mi = new ToolStripMenuItem();
//        mi.Text = v.Text;
//        mi.Tag = v;
//        mi.Click += OnVerbMenuClick;
//        menu.Items.Add(mi);
//      }

//      return menu;
//    }

//    private void OnVerbMenuClick(object sender, EventArgs e)
//    {
//      if (!CheckTechnicalPassword())
//        return;

//      ToolStripMenuItem tmi = (ToolStripMenuItem)sender;
//      Verb v = (Verb)tmi.Tag;
//      string p = GetVerbParameters(v);
//      if (p == null)
//        return;

//      InvokeVerb((ISupportVerbs)tmi.Owner.Tag, v, p);
//    }

//    private string GetVerbParameters(Verb v)
//    {
//      string s = String.Empty;
//      if (v.NeedInput)
//      {
//        _inputForm.Style = InputFormStyle.TextBoxOnly;
//        _inputForm.Setup(CaptionVerbInput, null, InputFilterOptions.Any);
//        s = (_inputForm.ShowDialog(this) == DialogResult.OK) ? _inputForm.GetResult() : null;
//      }

//      return s;
//    }

//    private void InvokeVerb(ISupportVerbs sv, Verb v, object param)
//    {
//      string s = String.Empty;
//      try
//      {
//        object result = sv.InvokeVerb(v.ID, param);
//        if (result != null)
//        {
//          s = result.ToString();
//#if DEBUG
//          this.ShowInformation(s);
//#endif
//        }
//      }
//      catch (Exception e)
//      {
//        this.ShowError(e);
//      }
//      finally
//      {
//        _gazprom.Logger.Write(LogLevel.Error, String.Format("Verb #{0} invoked on {1}\r\nResult = {2}", v.ID, sv, s), GazpromLogCategories.Runtime);
//      }
//    }

    #endregion

    internal void ShowFromTray()
    {
      TabPage tabPage = tabMain
          .TabPages
          .Cast<TabPage>()
          .Where(tp => (tp.ImageIndex == ImageIndexWarning))
          .FirstOrDefault();

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
      StringBuilder sb = new StringBuilder();
      sb.Append(' ', 10);

      return sb;
    }

    private void OnChordPressed(int chordID)
    {
      //get action
      HotKeyAction action = _actions[chordID];
      _stateTimer.UpdateStateText(String.Format("Выполняется {0}...", action), true);

      //get param
      object param = null;
      if (action == HotKeyAction.LogComment)
      {
        _inputForm.Style = InputFormStyle.ShowButtons | InputFormStyle.Multiline;
        _inputForm.Setup(Common.LogCommentCaption);
        if (_inputForm.ShowDialog(this) != DialogResult.OK)
          return;

        param = _inputForm.GetResult();
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
      HotKeyHelper.Run(action, _gazprom, param);
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
      TypeFilterAssemblyLoader loader = new TypeFilterAssemblyLoader(_gazprom.PathManager.ExtensionsPath,
        TypeFilterAssemblyLoader.GetExtensionSelector<IGazprom, IGazpromExtension>());

      TypeFilterSettings settings = _gazprom.ExtensionFilterSettings;

      TypeFilterForm form = new TypeFilterForm();
      form.Text = "Загружаемые расширения";
      form.Setup(new object[] { settings, loader });
      if (form.ShowDialog() != DialogResult.OK)
        return;

      Exception x = form.GetResult<Exception>();
      if (x != null)
        this.ShowError(x);
      else
        _gazprom.Logger.Write(LogLevel.Verbose, String.Format("Установлен фильтр расширений, всего {0}", settings.Count));
    }

    //private void ProcessListViewDoubleClick(ListView lvw, MouseEventArgs e)
    //{
    //  if (e.Button != MouseButtons.Left)
    //    return;

    //  ListViewItem lvi = lvw.HitTest(e.Location).Item;
    //  if (lvi == null)
    //    return;

    //  ISupportLogCategory f = lvi.Tag as ISupportLogCategory;
    //  if (f == null)
    //    return;

    //  ShowTraceWindow(lvw.Parent, f);
    //}

    //private void ProcessListViewClick(ListView lvw, MouseEventArgs e)
    //{
    //  if (e.Button != MouseButtons.Right)
    //    return;

    //  ListViewItem lvi = lvw.HitTest(e.Location).Item;
    //  if (lvi == null)
    //    return;

    //  ContextMenuStrip m = GetVerbMenuContainer(lvi).Tag as ContextMenuStrip;
    //  if (m == null)
    //    return;

    //  m.Show(lvw, e.Location);
    //}

    //private void ShowTraceWindow(Control owner, ISupportLogCategory filter)
    //{
    //  if (_traceSource.Filter != null)
    //    return;

    //  _traceSource.Filter = filter;
    //  LogTraceForm f = new LogTraceForm();
    //  f.Text = String.Format("Трассировка - [{0}]", filter.GetLogCategoryName());
    //  f.FormClosed += HideTraceWindow;
    //  f.Setup(_traceSource);
    //  f.ShowOnControl(owner);
    //}

    //private void HideTraceWindow(object sender, FormClosedEventArgs e)
    //{
    //  LogTraceForm f = (LogTraceForm)sender;
    //  f.HideOnControl();
    //  f.GetResult<ITraceWindowSource>();
    //  f.FormClosed -= HideTraceWindow;
    //  _traceSource.Filter = null;
    //}
  }
}