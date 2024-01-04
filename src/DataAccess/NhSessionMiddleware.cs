namespace theforum.DataAccess;

public class NhSessionMiddleware
{
    private readonly RequestDelegate _next;

    public NhSessionMiddleware(RequestDelegate next) => 
        _next = next;

    public async Task InvokeAsync(HttpContext context, NHibernate.ISession session)
    {
        // Start a transaction
        var transaction = session.BeginTransaction();

        try
        {
            // Continue the pipeline
            await _next(context);

            // Commit the transaction if there was no exception and the response status code is 2xx
            if (context.Response.StatusCode is >= 200 and <= 302)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
        }
        catch (Exception)
        {
            // Ensure the transaction is rolled back on an exception
            if (transaction is { IsActive: true })
            {
                await transaction.RollbackAsync();
            }
            throw;
        }
        finally
        {
            // Close the session
            if (session.IsOpen)
            {
                session.Close();
            }
        }
    }
}

// Extension method to make it easy to add the middleware
public static class NhSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseNhSessionMiddleware(this IApplicationBuilder builder) => 
        builder.UseMiddleware<NhSessionMiddleware>();
}
