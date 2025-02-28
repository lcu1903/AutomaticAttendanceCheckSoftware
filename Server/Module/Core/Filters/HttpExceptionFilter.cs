using Microsoft.AspNetCore.Mvc.Filters;

namespace Core.Filters;

public class HttpExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        // var exception = context.Exception;
        // switch (false)
        // {
        //     case true when exception is DbUpdateException:
        //         context.Result = new ObjectResult(new
        //         {
        //             success = false,
        //             errors = new List<string>()
        //             {
        //                 "message.errorSaveData"
        //             }
        //         })
        //         {
        //             StatusCode = StatusCodes.Status409Conflict
        //         };
        //         break;


        //     case true when exception is PostgresException:
        //         context.Result = new ObjectResult(new
        //         {
        //             success = false,
        //             errors = new List<string>()
        //             {
        //                 "message.errorSaveData"
        //             }
        //         })
        //         {
        //             StatusCode = StatusCodes.Status404NotFound
        //         };
        //         context.HttpContext.Response.Redirect("");
        //         break;

        //     case true when exception is ObjectNotFoundException:
        //         context.Result = new ObjectResult(new
        //         {
        //             success = false,
        //             errors = new List<string>()
        //             {
        //                 "message.notFound"
        //             }
        //         })
        //         {
        //             StatusCode = StatusCodes.Status404NotFound
        //         };
        //         context.HttpContext.Response.Redirect("");
        //         break;


        //     case true when exception is InvalidOperationException:
        //         if (exception.Source != null && exception.Source.Contains("Microsoft.EntityFrameworkCore"))
        //         {
        //             context.Result = new ObjectResult(new
        //             {
        //                 success = false,
        //                 errors = new List<string>()
        //                 {
        //                     "message.errorSaveData"
        //                 }
        //             })
        //             {
        //                 StatusCode = StatusCodes.Status409Conflict
        //             };
        //         }

        //         break;
            
            
        //     case true when exception is InvalidOperationException:
        //         if (exception.Source != null && exception.Source.Contains("Microsoft.EntityFrameworkCore"))
        //         {
        //             context.Result = new ObjectResult(new
        //             {
        //                 success = false,
        //                 errors = new List<string>()
        //                 {
        //                     "message.errorSaveData"
        //                 }
        //             })
        //             {
        //                 StatusCode = StatusCodes.Status409Conflict
        //             };
        //         }
        //         break;
                
        // }
    }
}