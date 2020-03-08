using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sample.pokedngnwii.web.common
{
    /// <summary>
    /// Summary description for setProfile
    /// </summary>
    public class setProfile : GamestatsHandler
    {
        public setProfile() : base("zjzrhOVXZKLHNspYpGoR0001c7850000620b0000000820556356pokedngnwii",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version2)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                0, 8);

        }
    }
}
