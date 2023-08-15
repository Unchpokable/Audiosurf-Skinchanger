﻿using SkinChangerRestyle.Core;
using SkinChangerRestyle.Core.NetworkTools;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SkinChangerRestyle.MVVM.Model
{
    internal class ServerSwapCard : ObservableObject
    {
        public ServerSwapCard(string baseDirectory)
        {
            BasePackagePath = baseDirectory;
        }

        public string ServerName
        {
            get => _serverName;
            set
            {
                _serverName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ServerName));
            }
        }

        public bool Installed
        {
            get => string.Equals(ServerName, SettingsProvider.InstalledServerPackageName, StringComparison.OrdinalIgnoreCase);
            set
            {
                _installed = value;
                OnPropertyChanged();
            }
        }

        public string ServerHost
        {
            get => !string.IsNullOrEmpty(RemoteStats?.Domain) ? RemoteStats?.Domain : SpecsServerRemote;
            set
            {
                Task.Run(async () =>
                {
                    RemoteStats = await PingHelper.Instance.PingHostAsync(value);
                    NotifyStatisticsChanged();
                });
            }
        }

        public string ServerPing => RemoteStats?.Ping.ToString();
        public string SpecsServerRemote { get; set; }

        public bool IsAvailable => RemoteStats?.IsAvailable ?? false;

        public IReadOnlyRemoteServerPingStats RemoteStats { get; set; }

        public string BasePackagePath { get; private set; }

        private string _serverName;
        private bool _installed;

        public async void ActualizeRemoteStats()
        {
            RemoteStats = await PingHelper.Instance.PingHostAsync(RemoteStats.IP);
            NotifyStatisticsChanged();
        }
        
        private void NotifyStatisticsChanged()
        {
            OnPropertyChanged(nameof(ServerName));
            OnPropertyChanged(nameof(ServerHost));
            OnPropertyChanged(nameof(ServerPing));
            OnPropertyChanged(nameof(IsAvailable));
            OnPropertyChanged(nameof(SpecsServerRemote));
        }
    }
}
