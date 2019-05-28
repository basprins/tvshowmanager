using System.Collections.Generic;
using System.Runtime.Serialization;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager
{
    [DataContract]
    [KnownType(typeof (TVShow))]
    [KnownType(typeof (FileSystem))]
    [KnownType(typeof (TVShowRestClient))]
    public class TVShowLibraryData
    {
        [DataMember]
        public string ImportEpisodesPath { get; set; }
        [DataMember]
        public string TVShowLocationPath { get; set; }
        [DataMember]
        public List<ITVShow> TVShows { get; set; }

        public TVShowLibraryData()
        {
            TVShows = new List<ITVShow>();
        }
    }
}
