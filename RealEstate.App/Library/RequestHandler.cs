using System;
using System.Diagnostics;
using CefSharp;
using CefSharp.Handler;

namespace RealEstate.App.Library
{
    /// <summary>
    /// Handle events related to browser requests.
    /// </summary>
    public class RequestHandler : DefaultRequestHandler
    {
        /// <summary>
        /// Called when the render process terminates unexpectedly.
        /// </summary>
        /// <param name="browserControl">The ChromiumWebBrowser control</param>
        /// <param name="browser">the browser object</param>
        /// <param name="status">indicates how the process terminated.</param>
        /// <remarks>
        /// Remember that <see cref="browserControl"/> is likely on a different thread so care should be used
        /// when accessing its properties.
        /// </remarks>
        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            switch (status)
            {
                case CefTerminationStatus.AbnormalTermination:
                    Debug.Print("Browser terminated abnormally.");
                    break;

                case CefTerminationStatus.ProcessWasKilled:
                    Debug.Print("Browser was killed.");
                    break;

                case CefTerminationStatus.ProcessCrashed:
                    Debug.Print("Browser crashed while.");
                    break;

                case CefTerminationStatus.OutOfMemory:
                    Debug.Print($"Browser terminated because of out of memory.");
                    break;

                default:
                    Debug.Print($"Browser terminated with unhandled status '{status}' while at address.");
                    break;
            }

            RenderProcessTerminated?.Invoke(browserControl, status);
        }

        /// <summary>
        /// Fires when the render process terminates unexpectedly.
        /// </summary>
        public event EventHandler<CefTerminationStatus> RenderProcessTerminated;
    }
}