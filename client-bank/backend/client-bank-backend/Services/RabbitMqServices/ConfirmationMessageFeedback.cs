using System.Collections.Concurrent;

namespace client_bank_backend.Services.RabbitMqServices
{
    public class ScopedConfirmationMessageFeedbackTracker
    {
        private ConcurrentDictionary<string, ConfirmationMessage?> _messages = new();
        private ConfirmationMessageFeedback _feedback => ConfirmationMessageFeedback.Instance;
        public void Track(string id)
        {
            _messages.TryAdd(id, null);
            _feedback.Subscribe(id, x =>
                _messages.AddOrUpdate(x.MessageTrackNumber, x, (id, value) => x)
            );
        }

        public void TrackMany(List<string> ids)
        {
            foreach (var id in ids)
            {
                _messages.TryAdd(id, null);
            }
            _feedback.Subscribe(ids, x =>
                _messages.AddOrUpdate(x.MessageTrackNumber, x, (id, value) => x)
            );
        }

        public async Task WaitForAll(TimeSpan timeout)
        {
            await Task.Run(() =>
            {
                var unconfirmed = _messages.Where(x => x.Value is null).Select(x => x.Key).ToList();
                var time = TimeSpan.Zero;
                while (unconfirmed.Any())
                {
                    foreach (var id in unconfirmed)
                    {
                        if (_messages[id] is not null)
                        {
                            unconfirmed.Remove(id);
                        }
                    }
                    Thread.Sleep(200);
                    time += TimeSpan.FromMilliseconds(200);
                    if (time >= timeout)
                    {
                        throw new TimeoutException("Failed to process all confirmation messages: timeout reached");
                    }
                }
            });
        }

        public async Task WaitFor(string id, TimeSpan timeout)
        {
            await Task.Run(() =>
            {
                var time = TimeSpan.Zero;
                while (_messages[id] is null)
                {
                    Thread.Sleep(200);
                    time += TimeSpan.FromMilliseconds(200);
                    if (time >= timeout)
                    {
                        throw new TimeoutException("Failed to process all confirmation messages: timeout reached");
                    }
                }
            });
        }
        public ConfirmationMessage? Get(string id) =>
            _messages.TryGetValue(id, out var message) == true
                ? message 
                : null;
    }
    public class ConfirmationMessageFeedback
    {
        private static ConfirmationMessageFeedback instance = null;
        private static object lockObject = new object();

        private readonly Dictionary<string, Action<ConfirmationMessage>> actions = new();

        private ConfirmationMessageFeedback() { }

        public static ConfirmationMessageFeedback Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new ConfirmationMessageFeedback();
                    }
                }

                return instance;
            }
        }

        public void Subscribe(List<string> trackingIds, Action<ConfirmationMessage> action)
        {
            lock (lockObject)
            {
                foreach (var trackingId in trackingIds)
                {
                    if (actions.ContainsKey(trackingId))
                    {
                        actions[trackingId] += action;
                    }
                    else
                    {
                        actions.Add(trackingId, action);
                    }
                }
            }
        }

        public void Subscribe(string trackingId, Action<ConfirmationMessage> action)
        {
            lock (lockObject)
            {
                if (actions.ContainsKey(trackingId))
                {
                    actions[trackingId] += action;
                }
                else
                {
                    actions.Add(trackingId, action);
                }
            }
        }

        public void Receive(ConfirmationMessage message)
        {
            lock (lockObject)
            {
                //TODO string conversion?
                var key = message.MessageTrackNumber;
                if (actions.ContainsKey(key) == false)
                {
                    return;
                }

                actions[key].Invoke(message);
                actions.Remove(key);
            }
        }
    }
}
