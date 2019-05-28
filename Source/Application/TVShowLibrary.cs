using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using PerfectCode.FileSystemIO;
using PerfectCode.Logging;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager
{
    public class TVShowLibrary : ITVShowLibrary
    {
        private static readonly ILogger Log = new Logger(typeof(TVShowLibrary));

        private const string TVShowsXmlFilename = "TV Shows.xml";

        public event TVShowAddedEvent TVShowAdded;
        public event TVShowRemovedEvent TVShowRemoved;
        public event TVShowLibraryUpdateStartedEvent TVShowLibraryUpdateStarted;
        public event TVShowProgressChangedEvent ProgressChanged;

        private readonly IFileSystem _fileSystem;
        private readonly ITVShowRestClient _restApiClient;

        public List<ITVShow> TVShows { get; }

        public string ImportFolder { get; set; }
        public string TVShowLocationPath { get; set; }

        private readonly object _syncObject;

        private TVShowLibraryData _tvShowLibraryData;
        private ITVShow activeTvShow;

        public TVShowLibrary(IFileSystem fileSystem, ITVShowRestClient restApiClient)
        {
            _fileSystem = fileSystem;
            _restApiClient = restApiClient;

            _tvShowLibraryData = new TVShowLibraryData();

            TVShows = new List<ITVShow>();

            _syncObject = new object();

            LoadLibraryData();
        }

        public void Add(ITVShow tvShow)
        {
            if (tvShow != null)
            {
                if (AddTvShow(tvShow))
                {
                    Store();
                }
            }
        }

        public void AddMany(List<ITVShow> tvShows)
        {
            lock (_syncObject)
            {
                var anyAdded = false;
                foreach (var tvShow in tvShows)
                {
                    if (!TVShows.Contains(tvShow))
                    {
                        anyAdded |= AddTvShow(tvShow);
                    }
                }

                if (anyAdded)
                {
                    Store();
                }
            }
        }

        public void RemoveTVShow(ITVShow tvShow)
        {
            lock (_syncObject)
            {
                if (TVShows.Contains(tvShow))
                {
                    TVShows.Remove(tvShow);
                    FireTVShowRemovedEvent(tvShow);

                    Store();
                }
            }
        }

        public void Store()
        {
            lock (_syncObject)
            {
                var serializer = new DataContractSerializer(typeof(TVShowLibraryData));

                var settings = new XmlWriterSettings { Indent = true };

                var libraryDataPath = GetLibraryDataFileFullPath();
                _fileSystem.EnsureDirectory(libraryDataPath);
                using (var writer = XmlWriter.Create(libraryDataPath, settings))
                {
                    _tvShowLibraryData.ImportEpisodesPath = ImportFolder;
                    _tvShowLibraryData.TVShowLocationPath = TVShowLocationPath;
                    _tvShowLibraryData.TVShows = TVShows;

                    serializer.WriteObject(writer, _tvShowLibraryData);
                }
            }

            Log.Info("Stored TV Shows to disk");
        }

        public void UpdateTvShow(ITVShow tvShow)
        {
            activeTvShow = tvShow;

            if (!tvShow.Updating)
            {
                tvShow.CheckForUpdates(_restApiClient);
            }
        }

        public void UpdateSeason(ITVShow show, ISeason season)
        {
            if (Equals(show, activeTvShow))
            {
                show.UpdateSeason(_restApiClient, season);
            }
        }

        private string GetLibraryDataFileFullPath()
        {
            var appDataFolder = _fileSystem.CombinePaths(Environment.ExpandEnvironmentVariables("%APPDATA%"), "TV Show Manager");

            return _fileSystem.CombinePaths(appDataFolder, TVShowsXmlFilename);
        }

        private bool AddTvShow(ITVShow tvShow)
        {
            if (!TVShows.Contains(tvShow))
            {
                TVShows.Add(tvShow);
                FireTVShowAddedEvent(tvShow);
                return true;
            }

            return false;
        }

        private void FireTVShowAddedEvent(ITVShow tvShow)
        {
            TVShowAdded?.Invoke(this, new TVShowEventArgs(tvShow));
        }

        private void FireTVShowRemovedEvent(ITVShow tvShow)
        {
            TVShowRemoved?.Invoke(this, new TVShowEventArgs(tvShow));
        }

        private void LoadLibraryData()
        {
            var libraryDataPath = GetLibraryDataFileFullPath();

            if (!_fileSystem.FileExists(libraryDataPath))
            {
                Log.Info("No TV Shows library on disk found, TV Show Library is empty");
            }
            else
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(TVShowLibraryData));

                    using (var xmlReader = XmlReader.Create(_fileSystem.ReadFile(libraryDataPath)))
                    {
                        _tvShowLibraryData = (TVShowLibraryData)serializer.ReadObject(xmlReader);

                        ImportFolder = _tvShowLibraryData.ImportEpisodesPath;
                        TVShowLocationPath = _tvShowLibraryData.TVShowLocationPath;

                        foreach (var tvShow in _tvShowLibraryData.TVShows.OrderBy(show => show.Name))
                        {
                            if (_fileSystem.DirectoryExists(tvShow.Path))
                            {
                                tvShow.CheckFileSystem(new SeasonsFactory(_fileSystem));
                                TVShows.Add(tvShow);
                            }
                            else
                            {
                                Log.Info($"TV Show {tvShow.Name} no longer exists on disk, removed from library");
                            }
                        }
                    }
                }
                catch (SerializationException)
                {
                    Log.Error("Corrupted Library file, starting empty");

                    _fileSystem.DeleteFile(libraryDataPath);
                }
            }
        }
    }
}