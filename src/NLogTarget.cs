/******************************************************************************
*
* Copyright (c) ReflectSoftware, Inc. All rights reserved. 
*
* See License.rtf in the solution root for license information.
*
******************************************************************************/
using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using NLog;
using NLog.Targets;

using ReflectSoftware.Insight;
using ReflectSoftware.Insight.Common;

using RI.Utils.ExceptionManagement;

namespace ReflectSoftware.Insight.Extensions.NLog
{
    [Target("ReflectInsight")]
    public sealed class NLogTarget : TargetWithLayout
    {
        class ActiveStates
        {
            public IReflectInsight RI { get; set; }
            public Boolean DisplayLevel { get; set; }
            public Boolean DisplayLocation { get; set; }
        }

        static private readonly String FLine;
        static private readonly MethodInfo FSendInternalErrorMethodInfo;

        private ActiveStates CurrentActiveStates { get; set; }
        
        public String InstanceName { get; set; }
        public String DisplayLevel { get; set; }
        public String DisplayLocation { get; set; }

        /// <summary>
        /// Initializes the <see cref="NLogTarget"/> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        static NLogTarget()
        {
            FLine = String.Format("{0,40}", String.Empty).Replace(" ", "-");
            FSendInternalErrorMethodInfo = typeof(ReflectInsight).GetMethod("SendInternalError", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogTarget"/> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public NLogTarget()
        {
            InstanceName = String.Empty;
            DisplayLevel = String.Empty;
            DisplayLocation = String.Empty;
            CurrentActiveStates = new ActiveStates();
            RIEventManager.OnServiceConfigChange += DoOnConfigChange;
        }

        /// <summary>
        /// Initializes the target. Can be used by inheriting classes
        /// to initialize logging.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            OnConfigChange();
        }

        /// <summary>
        /// Closes the target and releases any unmanaged resources.
        /// </summary>
        protected override void CloseTarget()
        {
            RIEventManager.OnServiceConfigChange -= DoOnConfigChange;
            base.CloseTarget();
        }

        /// <summary>
        /// Does the on configuration change.
        /// </summary>
        private void DoOnConfigChange()
        {
            OnConfigChange();
        }

        /// <summary>
        /// Called when [configuration change].
        /// </summary>
        private void OnConfigChange()
        {
            try
            {
                lock (this)
                {                    
                    ActiveStates states = new ActiveStates();
                    states.RI = RILogManager.Get(InstanceName) ?? RILogManager.Default;
                    states.DisplayLevel = String.Compare(DisplayLevel.ToLower().Trim(), "true", false) == 0;
                    states.DisplayLocation = String.Compare(DisplayLocation.ToLower().Trim() , "true", false) == 0;

                    CurrentActiveStates = states;
                }
            }
            catch (Exception ex)
            {
                RIExceptionManager.Publish(ex, "Failed during: NLogTarget.OnConfigChange()");
            }
        }

        /// <summary>
        /// Sends the internal error.
        /// </summary>
        /// <param name="ri">The ri.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        static private Boolean SendInternalError(IReflectInsight ri, MessageType mType, Exception ex)
        {
            return (Boolean)FSendInternalErrorMethodInfo.Invoke(ri, new object[] { mType, ex });
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="logEvent">The log event.</param>
        private static void SendMessage(ActiveStates states, MessageType mType, LogEventInfo logEvent)
        {
            try
            {
                // build details
                StringBuilder sb = null;

                if (logEvent.Exception != null)
                {
                    sb = new StringBuilder();
                    sb.Append(ExceptionBasePublisher.ConstructIndentedMessage(logEvent.Exception));
                    sb.AppendLine();
                    sb.AppendLine();
                }

                if (states.DisplayLevel || states.DisplayLocation)
                {
                    sb = sb ?? new StringBuilder();

                    sb.AppendLine("NLog Details:");
                    sb.AppendLine(FLine);

                    if (states.DisplayLevel)
                        sb.AppendFormat("{0,10}: {1}{2}", "Level", logEvent.Level.Name.ToUpper(), Environment.NewLine);

                    StackFrame frame = logEvent.UserStackFrame;
                    if (states.DisplayLocation && logEvent.UserStackFrame != null)
                    {
                        MethodBase method = frame.GetMethod();

                        String className = method != null ? method.ReflectedType.FullName : null;
                        String methodName = method != null ? method.Name : null;
                        String fileName = frame.GetFileName();

                        if (className != null) sb.AppendFormat("{0,10}: {1}{2}", "ClassName", className, Environment.NewLine);
                        if (methodName != null) sb.AppendFormat("{0,10}: {1}{2}", "MethodName", methodName, Environment.NewLine);
                        if (fileName != null) sb.AppendFormat("{0,10}: {1}{2}", "FileName", fileName, Environment.NewLine);
                        if (fileName != null) sb.AppendFormat("{0,10}: {1}{2}", "LineNumber", frame.GetFileLineNumber(), Environment.NewLine);
                    }
                }

                String details = sb != null ? sb.ToString() : null;
                states.RI.Send(mType, logEvent.FormattedMessage, details);
            }
            catch (Exception ex)
            {
                if (!SendInternalError(states.RI, mType, ex)) throw;
            }
        }

        /// <summary>
        /// Writes logging event to the log target. Must be overridden in inheriting
        /// classes.
        /// </summary>
        /// <param name="logEvent">Logging event to be written out.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            ActiveStates states = CurrentActiveStates;
            MessageType mType = MessageType.SendMessage;

            if (logEvent.Level == LogLevel.Info)
            {
                if (logEvent.FormattedMessage.StartsWith("[Enter]"))
                {
                    states.RI.EnterMethod(logEvent.FormattedMessage.Replace("[Enter]", String.Empty));
                    return;
                }
                if (logEvent.FormattedMessage.StartsWith("[Exit]"))
                {
                    states.RI.ExitMethod(logEvent.FormattedMessage.Replace("[Exit]", String.Empty));
                    return;
                }

                mType = MessageType.SendInformation;
            }
            else if (logEvent.Level == LogLevel.Trace)
            {
                mType = MessageType.SendTrace;
            }
            else if (logEvent.Level == LogLevel.Debug)
            {                
                mType = MessageType.SendDebug;
            }
            else if (logEvent.Level == LogLevel.Warn)
            {                
                mType = MessageType.SendWarning;
            }
            else if (logEvent.Level == LogLevel.Error)
            {
                mType = MessageType.SendError;
            }
            else if (logEvent.Level == LogLevel.Fatal)
            {
                mType = MessageType.SendFatal;
            }

            SendMessage(states, mType, logEvent);
        }
    }     
}
