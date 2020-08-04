using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Sample.pokedngnwii.web.rescue
{
    /// <summary>
    /// Summary description for rescueRegist
    /// </summary>
    public class rescueRegist : GamestatsHandler
    {
        public rescueRegist() : base("zjzrhOVXZKLHNspYpGoR0001c7850000620b0000000820556356pokedngnwii",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version2)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            // replay response from Sending Friend Mission Request.pcapng
            // request data:
            // 0000: 8b9ca41cac000000 1ca49c8bce6a330a
            // 0010: 24b4a64800000000 0000000000000000
            // 0020: 0000000000000044 0000000100b3979b
            // 0030: 0000000100000000 0000000000000000
            // 0040: 0000000000000000 0000000000000000
            // 0050: 0000000000000000 0000000000000000
            // 0060: 0000000000000000 0000000000000000
            // 0070: 0000000000000000 0000000000000000
            // 0080: 0000000000000000 0000000000000000
            // 0090: 0000000000000000 0000000000000000
            // 00a0: 0000000000000000 0000000000000000
            // 00b0: 00000000

            // request 0000-0007: gamestats boilerplate, pid and blob length
            // 0008-000b: pid in big endian

            // request is a rescue mail payload, response is the ID it has been uploaded with in big endian
            // the below response corresponds to 0000-0011-0926

            response.Write(new byte[] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x01, 0xb1, 0x4e },
                0, 6);
        }
    }
}