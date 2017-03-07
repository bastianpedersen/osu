﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Database;

namespace osu.Game.IPC
{
    public class BeatmapImporter : IDisposable
    {
        private IpcChannel<BeatmapImportMessage> channel;
        private BeatmapDatabase beatmaps;

        public BeatmapImporter(GameHost host, BeatmapDatabase beatmaps = null)
        {
            this.beatmaps = beatmaps;

            channel = new IpcChannel<BeatmapImportMessage>(host);
            channel.MessageReceived += messageReceived;
        }

        public async Task ImportAsync(string path)
        {
            if (beatmaps != null)
                beatmaps.Import(path);
            else
            {
                await channel.SendMessageAsync(new BeatmapImportMessage { Path = path });
            }
        }

        private void messageReceived(BeatmapImportMessage msg)
        {
            Debug.Assert(beatmaps != null);

            ImportAsync(msg.Path).ContinueWith(t => Logger.Error(t.Exception, @"error during async import"), TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class BeatmapImportMessage
    {
        public string Path;
    }
}
