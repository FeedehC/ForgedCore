﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Maps
{
    public class MapUpdater
    {
        AutoResetEvent _mapUpdateComplete = new AutoResetEvent(false);
        AutoResetEvent _resetEvent = new AutoResetEvent(false);
        AutoResetEvent _queueChanged = new AutoResetEvent(false);
        ConcurrentQueue<MapUpdateRequest> _queue = new();
        uint _workCount = 0;
        volatile bool _cancelationToken;
        int _numThreads;
        public MapUpdater(int numThreads)
        {
            _numThreads = numThreads;
            Task.Run(Dispatcher);
        }

        public void Deactivate()
        {
            _cancelationToken = true;

            Wait();

            // ensure we are all clear and tasks exit.
            _queue.Clear();
            _mapUpdateComplete.Set();
            _resetEvent.Set();
        }

        public void Wait()
        {
            while (_workCount > 0)
                _mapUpdateComplete.WaitOne();
        }

        public void ScheduleUpdate(Map map, uint diff)
        {
            _queue.Enqueue(new MapUpdateRequest(map, diff));
            _resetEvent.Set();
        }

        void Dispatcher()
        {
            while (!_cancelationToken)
            {
                _resetEvent.WaitOne(100);

                while (_queue.Count > 0)
                {
                    if (!_queue.TryDequeue(out MapUpdateRequest request) || request == null)
                        continue;

                    Interlocked.Increment(ref _workCount);

                    Task.Run(() =>
                    {
                        try
                        {
                            request.Call();
                        }
                        catch (Exception ex)
                        {
                            Log.outException(ex);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _workCount);
                            _queueChanged.Set();

                            if (_workCount == 0)
                                _mapUpdateComplete.Set();
                        }
                    });

                    while (_workCount >= _numThreads)
                        _queueChanged.WaitOne(100);
                }
            }
        }
    }

    public class MapUpdateRequest
    {
        Map m_map;
        uint m_diff;

        public MapUpdateRequest(Map m, uint d)
        {
            m_map = m;
            m_diff = d;
        }

        public void Call()
        {
            m_map.Update(m_diff);
        }
    }
}