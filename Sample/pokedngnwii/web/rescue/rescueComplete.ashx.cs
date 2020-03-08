using GamestatsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Sample.pokedngnwii.web.rescue
{
    /// <summary>
    /// Summary description for rescueComplete
    /// </summary>
    public class rescueComplete : GamestatsHandler
    {
        public rescueComplete() : base("zjzrhOVXZKLHNspYpGoR0001c7850000620b0000000820556356pokedngnwii",
            GamestatsRequestVersions.Version2, GamestatsResponseVersions.Version2)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            // replay response from A-OK Mission.pcapng
            // request data:
            // 0000: 8b9ca41cd4000000 1ca49c8b00000000
            // 0010: 0001b14ace6a330a 41f73da200000000
            // 0020: 0000000000000000 0000000000490000
            // 0030: 0000000000000000 0000000000000000
            // 0040: 0000000000000000 0000000000000000
            // 0050: 0000000000000000 0000000000000000
            // 0060: 0000000000000000 0000000000000000
            // 0070: 306f306f306f306f 306f306f306f304c
            // 0080: 304c304c304c304c 304c304c30823082
            // 0090: 30823082306f306f 306f306f306f306f
            // 00a0: 306f306f304c304c 304c304c304c304c
            // 00b0: 304c304c30823082 3082308230823082
            // 00c0: 30823082304c304c 304c304c304c304c
            // 00d0: 304c304c306f306f 306f306f

            response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                                        0x00, 0x00, 0x00, 0x00 },
                0, 12);

        }
    }
}
