﻿using System;
using System.Windows;
using ASCommander;
using SkinChangerRestyle.Core;
using SkinChangerRestyle.Core.Extensions;
using System.Windows.Media;
using SkinChangerRestyle.Core.Utils;
using SkinChangerRestyle.Properties;

namespace SkinChangerRestyle.MVVM.ViewModel
{

    class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            ConfigurationManager.InitializationFaultCallback += async (e) =>
            {
#if ASD
                await Task.Run(() =>
                {
                    MessageBox.Show($"{e.Message}\nPlease, check your settings tab", "Default Configuration initialization fault", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
#endif
            };

            ConfigurationManager.SetUpDefaultSettings();
            ConfigurationManager.InitializeEnvironment();
            _asHandle = AudiosurfHandle.Instance;
            _asHandle.StateChanged += OnASHandleStateChanged;
            SkinsGridVM = SkinChangerViewModel.Instance;
            TweakerVM = new TweakerViewModel();
            SettingsVM = new SettingViewModel();
            ColorsVM = ColorsConfiguratorViewModel.Instance;
            ServerSwapperVM = new ServerSwapperViewModel();

            CurrentView = SkinsGridVM;
            SetChangerView = new RelayCommand(o => CurrentView = SkinsGridVM);
            ConnectAudiosurfWindow = new RelayCommand(ConnectAudiosurfWindowInternal);
            SetCommandCenterView = new RelayCommand(o => CurrentView = TweakerVM);
            SetSettingsView = new RelayCommand(o => CurrentView = SettingsVM);
            SetColorsView = new RelayCommand(o => CurrentView = ColorsVM);
            SetServerSwapperView = new RelayCommand(o => CurrentView = ServerSwapperVM);
            EnableAutoHandling = new RelayCommand(o => _asHandle.StartAutoHandling());
            ResetWndProcService = new RelayCommand(o => _asHandle.ReinitializeWndProcMessageService());
            Utils.DisposeAndClear();
            MainIcon = Resources.TweakerIcon.ToImageSource();
        }

        public ImageSource MainIcon { get; set; }

        public RelayCommand SetCommandCenterView { get; set; }
        public RelayCommand SetChangerView { get; set; }
        public RelayCommand SetColorsView { get; set; }
        public RelayCommand ConnectAudiosurfWindow { get; set; }
        public RelayCommand SetSettingsView { get; set; }
        public RelayCommand SetServerSwapperView { get; set; }
        public RelayCommand EnableAutoHandling { get; set; }
        public RelayCommand ResetWndProcService { get; set; }

        public SkinChangerViewModel SkinsGridVM { get; set; }
        public TweakerViewModel TweakerVM { get; set; }
        public SettingViewModel SettingsVM { get; set; }
        public ColorsConfiguratorViewModel ColorsVM { get; set; }
        public ServerSwapperViewModel ServerSwapperVM { get; set; }
        public string AudiosurfStatusMessage => _asHandle?.StateMessage;
        public SolidColorBrush AudiosurfStatusBackgroundColor => (SolidColorBrush)new BrushConverter().ConvertFromString(_asHandle?.StateColor);

        private object _currentView;
        private AudiosurfHandle _asHandle;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentViewScrollAllowed));
            }
        }

        public bool CurrentViewScrollAllowed => ((ObservableObject)_currentView).ScrollAllowed;

        private void OnASHandleStateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AudiosurfStatusMessage));
            OnPropertyChanged(nameof(AudiosurfStatusBackgroundColor));
        }

        private void ConnectAudiosurfWindowInternal(object param)
        {
            try
            {
                new ProcessSelectionWindow().Show();
            }
            catch
            {
                MessageBox.Show("Error occurred during opening process browser. Try again", "Windows process browser error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
