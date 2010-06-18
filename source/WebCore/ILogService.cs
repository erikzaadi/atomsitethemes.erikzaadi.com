/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.WebCore
{
  using System;
  using System.Diagnostics;

  public interface ILogService
  {
    void Info(string message);
    void Info(string message, params object[] formatParameters);

    void Warn(string message);
    void Warn(string message, params object[] formatParameters);

    void Debug(string message);
    void Debug(string message, params object[] formatParameters);

    void Error(string message);
    void Error(string message, params object[] formatParameters);
    void Error(Exception x);

    void Fatal(string message);
    void Fatal(string message, params object[] formatParameters);
    void Fatal(Exception x);
  }

  public class DiagLogService : ILogService
  {
    #region ILogService Members

    public void Info(string message)
    {
      System.Diagnostics.Debug.WriteLine(message);
    }

    public void Info(string message, params object[] formatParameters)
    {
      System.Diagnostics.Debug.WriteLine(string.Format(message, formatParameters));
    }

    public void Warn(string message)
    {
      Trace.WriteLine(message);
    }

    public void Warn(string message, params object[] formatParameters)
    {
      Trace.WriteLine(string.Format(message, formatParameters));
    }

    public void Debug(string message)
    {
      System.Diagnostics.Debug.WriteLine(message);
    }

    public void Debug(string message, params object[] formatParameters)
    {
      System.Diagnostics.Debug.WriteLine(string.Format(message, formatParameters));
    }

    public void Error(string message)
    {
      Trace.WriteLine(message);
    }

    public void Error(string message, params object[] formatParameters)
    {
      Trace.WriteLine(string.Format(message, formatParameters));
    }

    public void Error(Exception x)
    {
      Trace.WriteLine(x);
    }

    public void Fatal(string message)
    {
      Trace.WriteLine(message);
    }

    public void Fatal(string message, params object[] formatParameters)
    {
      Trace.WriteLine(string.Format(message, formatParameters));
    }

    public void Fatal(Exception x)
    {
      Trace.WriteLine(x);
    }

    #endregion
  }

  public class TraceSourceLogService : ILogService
  {
    protected TraceSource TraceSource;

    public TraceSourceLogService(TraceSource trace)
    {
      TraceSource = trace;
    }
    #region ILogService Members

    public void Info(string message)
    {
      TraceSource.TraceInformation(message);
    }

    public void Info(string message, params object[] formatParameters)
    {
      TraceSource.TraceInformation(message, formatParameters);
    }

    public void Warn(string message)
    {
      TraceSource.TraceEvent(TraceEventType.Warning, 0, message);
    }

    public void Warn(string message, params object[] formatParameters)
    {
      TraceSource.TraceEvent(TraceEventType.Warning, 0, message, formatParameters);
    }

    public void Debug(string message)
    {
      TraceSource.TraceEvent(TraceEventType.Verbose, 0, message);
    }

    public void Debug(string message, params object[] formatParameters)
    {
      TraceSource.TraceEvent(TraceEventType.Verbose, 0, message, formatParameters);
    }

    public void Error(string message)
    {
      TraceSource.TraceEvent(TraceEventType.Error, 0, message);
    }

    public void Error(string message, params object[] formatParameters)
    {
      TraceSource.TraceEvent(TraceEventType.Error, 0, message, formatParameters);
    }

    public void Error(Exception x)
    {
      TraceSource.TraceEvent(TraceEventType.Error, 0, x.ToString());
    }

    public void Fatal(string message)
    {
      TraceSource.TraceEvent(TraceEventType.Critical, 0, message);
    }

    public void Fatal(string message, params object[] formatParameters)
    {
      TraceSource.TraceEvent(TraceEventType.Critical, 0, message, formatParameters);
    }

    public void Fatal(Exception x)
    {
      TraceSource.TraceEvent(TraceEventType.Critical, 0, x.ToString());
    }

    #endregion
  }
}
