﻿namespace SkinChangerRestyle.MVVM.ViewModel
{
    using System;
    using System.Windows.Forms;
    using SkinChangerRestyle.Core;
    using SkinChangerRestyle.MVVM.Model;
    using SkinChangerRestyle.Core.Extensions;

    class MainViewModel : ObservableObject
    {
        public RelayCommand SetCommandCenterView { get; set; }
        public RelayCommand SetChangerView { get; set; }
        public RelayCommand ConnectAudiosurfWindow { get; set; }
        public RelayCommand SetSettingsView { get; set; }

        public SkinChangerViewModel SkinsGridVM { get; set; }
        public TweakerViewModel TweakerVM { get; set; }
        public SettingViewModel SettingsVM { get; set; }
        public object AudiosurfStatusMessage => _asHandle?.StateMessage;
        public object AudiosurfStatusBackgroundColor => _asHandle?.StateColor;

        private object _currentView;
        private AudiosurfHandle _asHandle;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            InternalWorker.InitializationFaultCallback += async (e) =>
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    MessageBox.Show($"{e.Message}\nPlease, check your settings tab", "Default Configuration initialization fault", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });
            };

            InternalWorker.SetUpDefaultSettings();
            InternalWorker.InitializeEnvironment();
            _asHandle = AudiosurfHandle.Instance;
            _asHandle.StateChanged += OnASHandleStateChanged;
            SkinsGridVM = SkinChangerViewModel.Instance;
            TweakerVM = new TweakerViewModel();
            SettingsVM = new SettingViewModel();
            CurrentView = SkinsGridVM;
            SetChangerView = new RelayCommand(o => CurrentView = SkinsGridVM);
            ConnectAudiosurfWindow = new RelayCommand(o => { _asHandle.TryConnect(); });
            SetCommandCenterView = new RelayCommand(o => CurrentView = TweakerVM);
            SetSettingsView = new RelayCommand(o => CurrentView = SettingsVM);
            Extensions.DisposeAndClear();
            
        }

        private void OnASHandleStateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AudiosurfStatusMessage));
            OnPropertyChanged(nameof(AudiosurfStatusBackgroundColor));
        }
    }
}
