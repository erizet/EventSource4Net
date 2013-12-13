using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace EventSource4Net
{
    class ConnectedState : IConnectionState
    {
        private static readonly slf4net.ILogger _logger = slf4net.LoggerFactory.GetLogger(typeof(ConnectedState));

        private HttpWebResponse mResponse;
        public EventSourceState State { get { return EventSourceState.OPEN; } }

        public ConnectedState(HttpWebResponse resp)
        {
            mResponse = resp;
        }

        public Task<IConnectionState> Run(Action<ServerSentEvent> msgReceived, CancellationToken cancelToken)
        {
            int i = 0;

            Task<IConnectionState> t = new Task<IConnectionState>(() =>
            {
                //using (mResponse)
                {
                    //using (var stream = mResponse.GetResponseStream())
                    var stream = mResponse.GetResponseStream();
                    {
                        byte[] buffer = new byte[1024 * 8];
                        var taskRead = stream.ReadAsync(buffer, 0, buffer.Length, cancelToken);

                         try
                        {
                            taskRead.Wait(cancelToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.Trace(ex, "ConnectedState.Run");
                        }
                        if (!cancelToken.IsCancellationRequested)
                        {
                            int bytesRead = taskRead.Result;
                            if (bytesRead > 0) // stream has not reached the end yet
                            {
                                //Console.WriteLine("ReadCallback {0} bytesRead", bytesRead);
                                string text = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                string[] lines = StringSplitter.SplitIntoLines(text);
                                ServerSentEvent sse = null;
                                foreach (string line in lines)
                                {
                                    if (cancelToken.IsCancellationRequested) break;

                                    // Dispatch message if empty lne
                                    if (string.IsNullOrEmpty(line.Trim()) && sse != null)
                                    {
                                        _logger.Trace("Message received");
                                        msgReceived(sse);
                                        sse = null;
                                    }
                                    else if (line.StartsWith(":"))
                                    {
                                        // This a comment, just log it.
                                        _logger.Trace("A comment was received: " + line);
                                    }
                                    else
                                    {
                                        string fieldName = String.Empty;
                                        string fieldValue = String.Empty;
                                        if (line.Contains(':'))
                                        {
                                            int index = line.IndexOf(':');
                                            fieldName = line.Substring(0, index);
                                            fieldValue = line.Substring(index + 1).TrimStart();
                                        }
                                        else
                                            fieldName = line;

                                        if (String.Compare(fieldName, "event", true) == 0)
                                        {
                                            sse = sse ?? new ServerSentEvent();
                                            sse.EventType = fieldValue;
                                        }
                                        else if (String.Compare(fieldName, "data", true) == 0)
                                        {
                                            sse = sse ?? new ServerSentEvent();
                                            sse.Data = fieldValue + '\n';
                                        }
                                        else if (String.Compare(fieldName, "id", true) == 0)
                                        {
                                            sse = sse ?? new ServerSentEvent();
                                            sse.LastEventId = fieldValue;
                                        }
                                        else if (String.Compare(fieldName, "retry", true) == 0)
                                        {
                                            int parsedRetry;
                                            if (int.TryParse(fieldValue, out parsedRetry))
                                            {
                                                sse = sse ?? new ServerSentEvent();
                                                sse.Retry = parsedRetry;
                                            }
                                        }
                                        else
                                        {
                                            // Ignore this, just log it
                                            _logger.Warn("A unknown line was received: " + line);
                                        }
                                    }
                                }

                                if (!cancelToken.IsCancellationRequested)
                                    return this;
                            }
                            else // end of the stream reached
                            {
                                _logger.Trace("No bytes read. End of stream.");
                            }
                        }

                        //stream.Dispose()
                        //stream.Close();
                        //mResponse.Close();
                        //mResponse.Dispose();
                        return new DisconnectedState(mResponse.ResponseUri);
                    }
                }
            });

            t.Start();
            return t;
        }
    }
}
