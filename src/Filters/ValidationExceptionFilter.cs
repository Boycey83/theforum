using Microsoft.AspNetCore.Mvc;

namespace theforum.Filters;

using Microsoft.AspNetCore.Mvc.Filters;
using NHibernate;
using System.Net;
using System.Threading.Tasks;
using ISession = NHibernate.ISession;

public class ValidationExceptionFilter : IAsyncExceptionFilter
{
    private readonly ISession _session;

    public ValidationExceptionFilter(ISession session) => 
        _session = session;

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var transaction = _session.GetCurrentTransaction();
        if (transaction.IsActive)
        {
            await transaction.RollbackAsync();
        }
        _session.Dispose();
        context.Result = new ObjectResult(context.Exception.Message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
        context.ExceptionHandled = true; // mark exception as handled
    }
}