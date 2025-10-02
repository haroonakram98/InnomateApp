namespace InnomateApp.Application.Interfaces;

using InnomateApp.Application.Logging;

public interface IAuditSink
{
    void Enqueue(AuditEntry entry); // fire-and-forget enqueue
}