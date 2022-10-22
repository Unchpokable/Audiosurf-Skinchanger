﻿using ASCommander;
using SkinChangerRestyle.Core;
using SkinChangerRestyle.Core.Extensions;
using SkinChangerRestyle.MVVM.Model;
using System;
using System.IO;
using System.Windows.Forms;

namespace SkinChangerRestyle.MVVM.ViewModel
{
    public enum SettingsFields
    {
        TexturesPath,
        AddSkinsPath
    }

    internal class SettingViewModel : ObservableObject
    {
        public SettingViewModel()
        {
            SetConfigurationValue = new RelayCommand(AskAndSetConfigValue);
            SelectTempFile = new RelayCommand(SelectTempFileInternal);
            DuplicateTempFile = new RelayCommand(DuplicateTempFileInternal);
            TexturesFolderPath = SettingsProvider.GameTexturesPath;
            AdditionalSkinsFolderPath = SettingsProvider.SkinsFolderPath;
            IsHotReload = SettingsProvider.HotReload;
            IsShouldCheckTextures = SettingsProvider.ControlSystemActive;
            WatcherTempFile = SettingsProvider.WatcherTempFile;
            _isWatcherEnabled = SettingsProvider.WatcherEnabled;

            if (_isWatcherEnabled)
            {
                Watcher = new TexturesWatcher
                {
                    TargetPath = SettingsProvider.GameTexturesPath
                };

                IsShouldStoreTextures = SettingsProvider.WatcherShouldStoreTextures;
                IsTempFileOverrided = SettingsProvider.WatcherTempFileOverrided;

                if (IsShouldStoreTextures)
                {
                    Watcher.InitializeTempFile(IsTempFileOverrided ? WatcherTempFile : @"store\temp.tasp");
                }

                Watcher.DiskOperationCompleted += (s, e) =>
                {
                    IsGuiActive = true;
                    OnPropertyChanged(nameof(IsWatcherActive));
                };

                Watcher.Triggered += async (s, e) =>
                {
                    AudiosurfHandle.Instance.Command("ascommand reloadtextures");
                    if (IsShouldStoreTextures)
                    {
                        await Watcher.OverwriteTempFile();
                    }
                };
            }
        }

        public bool IsFastPreview
        {
            get => SettingsProvider.UseFastPreview;
            set
            {
                var isRestart = MessageBox.Show("Turning this parameter needs to restart applicaton. Would you like to continue?", "Module parameter changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (isRestart == DialogResult.Yes)
                {
                    SettingsProvider.UseFastPreview = value;
                    ApplySettings();
                    Extensions.Cmd($"taskkill /f /im \"{AppDomain.CurrentDomain.FriendlyName}\" && timeout /t 1 && {AppDomain.CurrentDomain.FriendlyName}");
                }
                else
                    return;
            }
        }
        public string TexturesFolderPath
        {
            get => _texturesPath;
            set
            {
                _texturesPath = value;
                SettingsProvider.GameTexturesPath = value;
                ApplySettings();
                OnPropertyChanged();
            }
        }
        public string AdditionalSkinsFolderPath
        {
            get => _additionalSkinsPath;
            set
            {
                _additionalSkinsPath = value;
                SettingsProvider.SkinsFolderPath = value;
                ApplySettings();
                OnPropertyChanged();
            }
        }

        public bool IsShouldCheckTextures
        {
            get => _isShouldCheckTextures;
            set
            {
                _isShouldCheckTextures = value;
                SettingsProvider.ControlSystemActive = value;
                ApplySettings();
                OnPropertyChanged();
            }
        }

        public bool IsHotReload
        {
            get => _isHotReload;
            set
            {
                _isHotReload = value;
                SettingsProvider.HotReload = value;
                ApplySettings();
                OnPropertyChanged();
            }
        }
        
        public bool IsShouldUseSafetyInstallation
        {
            get => _isSafeInstall;
            set
            {
                _isSafeInstall = value;
                SettingsProvider.SafeInstall = value;
                ApplySettings();
                OnPropertyChanged();
            }
        }

        public bool IsWatcherActive
        {
            get => Watcher != null && IsGuiActive;
            set
            {
                var isRestart = MessageBox.Show("Turning this parameter needs to restart applicaton. Would you like to continue?", "Module parameter changed", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (isRestart == DialogResult.Yes)
                {
                    _isWatcherEnabled = value;
                    SettingsProvider.WatcherEnabled = value;
                    ApplySettings();
                    Extensions.Cmd($"taskkill /f /im \"{AppDomain.CurrentDomain.FriendlyName}\" && timeout /t 1 && {AppDomain.CurrentDomain.FriendlyName}");
                }
                else
                    return;
            }
        }

        public string WatcherTempFile
        {
            get => IsTempFileOverrided ? _watcherTempFile : @"store\temp.tasp";
            set
            {
                _watcherTempFile = value;

                if (!File.Exists(_watcherTempFile))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(_watcherTempFile)))
                        Directory.CreateDirectory(Path.GetDirectoryName(_watcherTempFile));

                    File.Create(_watcherTempFile);
                }

                SettingsProvider.WatcherTempFile = value;
                ApplySettings();
            }
        }

        public bool IsShouldStoreTextures
        {
            get => _isShouldStoreTempTextures;
            set
            {
                if (!_isWatcherEnabled) return;
                _isShouldStoreTempTextures = value;
                OnPropertyChanged();
                SettingsProvider.WatcherShouldStoreTextures = value;
                ApplySettings();
                if (value)
                {
                    IsGuiActive = false;
                    OnPropertyChanged(nameof(IsWatcherActive));
                    if (!string.IsNullOrEmpty(Watcher.TempFilePath))
                        Watcher.OverwriteTempFile();
                    else
                        Watcher.InitializeTempFile(WatcherTempFile);
                }
            }
        }

        public bool IsTempFileOverrided
        {
            get => _isWatcherEnabled && _isTempFileOverrided;
            set
            {
                IsGuiActive = false;
                _isTempFileOverrided = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsWatcherActive));
                SettingsProvider.WatcherTempFileOverrided = value;
                Watcher.InitializeTempFile(value ? WatcherTempFile : SettingsProvider.WatcherDefaultTemp);
                ApplySettings();
            }
        }

        public bool IsGuiActive
        {
            get => _isGuiActive; 
            set 
            { 
                _isGuiActive = value; 
                OnPropertyChanged(); 
            }
        }


        public RelayCommand SetConfigurationValue { get; set; }
        public RelayCommand SelectTempFile { get; set; }
        public RelayCommand DuplicateTempFile { get; set; }
        public TexturesWatcher Watcher { get; private set; }

        private string _texturesPath;
        private string _additionalSkinsPath;
        private string _watcherTempFile;
        private bool _isWatcherEnabled;
        private bool _isShouldCheckTextures;
        private bool _isShouldStoreTempTextures;
        private bool _isHotReload;
        private bool _isSafeInstall;
        private bool _isTempFileOverrided;
        private bool _isGuiActive;

        private void AskAndSetConfigValue(object parameter)
        {
            var field = (SettingsFields)parameter;

            var pathSelectionDialog = new FolderBrowserDialog();
            if (pathSelectionDialog.ShowDialog() == DialogResult.OK)
            {
                switch (field)
                {
                    case SettingsFields.TexturesPath:
                        TexturesFolderPath = pathSelectionDialog.SelectedPath;
                        SettingsProvider.GameTexturesPath = TexturesFolderPath;
                        break;
                    case SettingsFields.AddSkinsPath:
                        AdditionalSkinsFolderPath = pathSelectionDialog.SelectedPath;
                        SettingsProvider.SkinsFolderPath = AdditionalSkinsFolderPath;
                        break;
                }
                ApplySettings();
            }
        }

        private void ApplySettings()
        {
            InternalWorker.RewriteSettings();
        }

        private void SelectTempFileInternal(object o)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select new temp file";
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(WatcherTempFile));

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                WatcherTempFile = saveFileDialog.FileName;
                OnPropertyChanged(nameof(WatcherTempFile));
                Watcher.InitializeTempFile(WatcherTempFile);
            }
        }

        private void DuplicateTempFileInternal(object o)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select duplicate output";
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(WatcherTempFile));

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(WatcherTempFile, saveFileDialog.FileName);
            }
        }
    }
}
