﻿using ChangerAPI.Engine;
using SkinChangerRestyle.Core;
using SkinChangerRestyle.Core.Extensions;
using SkinChangerRestyle.MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace SkinChangerRestyle.MVVM.Model
{
    class SkinCard : ObservableObject
    {
        public bool RenameActive
        {
            get => _renameActive;
            set
            {
                _renameActive = value;
                OnPropertyChanged();
            }
        }

        public Visibility RenameVisible
        {
            get => _renameVisible;
            set
            {
                _renameVisible = (Visibility)Enum.Parse(typeof(Visibility), value.ToString());
                OnPropertyChanged();
            }
        }


        public string NewName
        {
            get => _renameName;
            set
            {
                _renameName = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ImageSource Cover => Screenshots.FirstOrDefault()?.Image;

        public string InstallTooltip => "Clear Installation";
        public string ExportCopyTooltip => "Export copy of this skin";
        public string RenameTooltip => "Rename this skin";
        public string EditOnDiskTooltip => "Turn ASTweaker into EditOnDisk mode";
        public string PathToOrigin => _pathToOriginFile;

        public ImageSource InstallIcon { get; set; }
        public ImageSource ExportCopyIcon { get; set; }
        public ImageSource RenameIcon { get; set; }
        public ImageSource EditOnDiskIcon { get; set; }

        public RelayCommand InstallCommand { get; set; }
        public RelayCommand ExportCopyCommand { get; set; }
        public RelayCommand EditOnDiskCommand { get; set; }
        public RelayCommand EnableRename { get; set; }
        public RelayCommand ApplyRename { get; set; }

        public ObservableCollection<InteractableScreenshot> Screenshots
        {
            get => _screenshots;
            set
            {
                _screenshots = value;
                OnPropertyChanged();
            }
        }

        private string _pathToOriginFile;
        private string _name;
        private string _renameName;
        private SkinChangerViewModel _rootVM;
        private ObservableCollection<InteractableScreenshot> _screenshots;
        private bool _renameActive;
        private Visibility _renameVisible;

        public SkinCard(AudiosurfSkinExtended skin, SkinChangerViewModel root = null)
        {
            _rootVM = root;
            AssignSkin(skin);
            InstallIcon = Properties.Resources.install.ToImageSource();
            ExportCopyIcon = Properties.Resources.export.ToImageSource();
            RenameIcon = Properties.Resources.edit.ToImageSource();
            EditOnDiskIcon = Properties.Resources.editondisk.ToImageSource();
            _renameVisible = Visibility.Hidden;
            _renameActive = false;

            EnableRename = new RelayCommand(EnableRenameInternal);
            ApplyRename = new RelayCommand(ApplyRenameInternal);
            InstallCommand = new RelayCommand(Install);
            EditOnDiskCommand = new RelayCommand(EditOnDisk);
            ExportCopyCommand = new RelayCommand(ExportCopyInternal);
        }

        private void AssignSkin(AudiosurfSkinExtended skin)
        {
            if (skin == null) return;

            _pathToOriginFile = $"{skin.Source}";
            Name = $"{skin.Name}";
            Screenshots = new ObservableCollection<InteractableScreenshot>(skin.Previews.Group.Select(screenshot => new InteractableScreenshot(((System.Drawing.Bitmap)screenshot).ToImageSource())));
        }

        private void Install(object frameworkRequieredParameter)
        {
            _rootVM.InstallSkin(_pathToOriginFile, SettingsProvider.GameTexturesPath, forced: true, clearInstall: true);
        }

        private void EnableRenameInternal(object frameworkRequieredParameter)
        {
            NewName = "";
            RenameVisible = Visibility.Visible;
            RenameActive = true;
        }

        private void ApplyRenameInternal(object frameworkRequieredParameter)
        {
            var newName = NewName;
            var skinObject = SkinPackager.Decompile(_pathToOriginFile);
            skinObject.Name = newName;
            Name = newName;
            var newFile = $@"Skins\{newName}.askin2";
            SkinPackager.CompileTo(skinObject, "Skins");
            File.Delete(_pathToOriginFile);
            _pathToOriginFile = newFile;
            RenameActive = false;
            RenameVisible = Visibility.Hidden;
        }

        private void EditOnDisk(object frameworkRequieredParameter)
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            _rootVM.InstallSkin(_pathToOriginFile, tempDirectory, forced: true, unpackScreenshots: true);
            var dirproc = Process.Start(tempDirectory);
            new EditOnDiskLockWindow().ShowDialog();
            var redactedSkin = SkinPackager.CreateSkinFromFolder(tempDirectory);
            redactedSkin.Name = Name;
            SkinPackager.RewriteCompile(redactedSkin, _pathToOriginFile);
            AssignSkin(redactedSkin);
            dirproc?.Close();
            Directory.Delete(tempDirectory, true);
        }

        private void ExportCopyInternal(object frameworkRequieredParameter)
        {
            var output = new FolderBrowserDialog();

            if (output.ShowDialog() == DialogResult.OK)
            {
                var skin = SkinPackager.Decompile(_pathToOriginFile);
                SkinPackager.CompileTo(skin, output.SelectedPath);
            }
        }
    }

    internal class DebugSkinCard
    {
        public string Name => _name;

        public string InstallTooltip => "Full texture set replacement with clear installation";
        public string ExportCopyTooltip => "Export copy of this skin";
        public string RenameTooltip => "Rename this skin";
        public string EditOnDiskTooltip => "Edit this skin on disk";

        public ImageSource InstallIcon { get; set; }
        public ImageSource ExportCopyIcon { get; set; }
        public ImageSource RenameIcon { get; set; }
        public ImageSource EditOnDiskIcon { get; set; }
        public ImageSource Cover { get; set; }

        public RelayCommand InstallCommand { get; set; }
        public RelayCommand ExportCopyCommand { get; set; }
        public RelayCommand RenameCommand { get; set; }
        public RelayCommand EditOnDiskCommand { get; set; }

        public ObservableCollection<InteractableScreenshot> Screenshots { get; set; }

        private string _name;

        public DebugSkinCard(string name)
        {
            _name = name;
            Screenshots = new ObservableCollection<InteractableScreenshot>();
            for (int i = 0; i < 10; i++)
            {
                Screenshots.Add(new InteractableScreenshot(Properties.Resources.Pintman.ToImageSource()));
            }

            InstallIcon = Properties.Resources.install.ToImageSource();
            ExportCopyIcon = Properties.Resources.export.ToImageSource();
            RenameIcon = Properties.Resources.edit.ToImageSource();
            EditOnDiskIcon = Properties.Resources.editondisk.ToImageSource();
            Cover = Screenshots?.First().Image;
        }
    }
}
