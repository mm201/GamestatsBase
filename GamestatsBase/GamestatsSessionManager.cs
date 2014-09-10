﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GamestatsBase
{
    public class GamestatsSessionManager
    {
        public readonly Dictionary<String, GamestatsSession> Sessions 
            = new Dictionary<String, GamestatsSession>();

        public GamestatsSessionManager()
        {
        }

        private void PruneSessions()
        {
            Dictionary<String, GamestatsSession> sessions = Sessions;
            DateTime now = DateTime.UtcNow;

            lock (sessions)
            {
                Queue<String> toRemove = new Queue<String>();
                foreach (KeyValuePair<String, GamestatsSession> session in sessions)
                {
                    if (session.Value.ExpiryDate < now) toRemove.Enqueue(session.Key);
                }
                while (toRemove.Count > 0)
                {
                    sessions.Remove(toRemove.Dequeue());
                }
            }
        }

        public void Add(GamestatsSession session)
        {
            Sessions.Add(session.Hash, session);
        }

        public void Remove(GamestatsSession session)
        {
            Sessions.Remove(session.Hash);
        }

        /// <summary>
        /// Retrieves the GamestatsSessionManager from the context's application state.
        /// </summary>
        public static GamestatsSessionManager FromContext(HttpContext context)
        {
            object oManager = context.Application["GamestatsSessionManager"];
            GamestatsSessionManager manager = oManager as GamestatsSessionManager;

            if (manager == null)
            {
                manager = new GamestatsSessionManager();
                context.Application.Add("GamestatsSessionManager", manager);
            }

            return manager;
        }

        public GamestatsSession FindSession(int pid, String url)
        {
            // todo: keep a hash table of pids that maps them onto session objects
            GamestatsSession result = null;

            foreach (GamestatsSession sess in Sessions.Values)
            {
                if (sess.PID == pid && sess.URL == url)
                {
                    if (result != null)
                    {
                        // todo: there's more than one matching session... delete them all.
                    }
                    return sess; // temp until I get it to cleanup old sessions
                    result = sess;
                }
            }
            return result;
        }

        
    }
}
