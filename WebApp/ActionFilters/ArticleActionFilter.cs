using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Protocol;
using WebApp.DataAccess;

namespace WebApp.ActionFilters
{
    public class ArticleActionFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //validating whether the accessing user has permission to the article
            //being accessed

            try
            {
                string articleId = context.HttpContext.Request.Query["id"].ToString();

                string emailOfLoggedInUser = context.HttpContext.User.Identity.Name;

                ArticleRepository articleRepository 
                    = context.HttpContext.RequestServices.GetService<ArticleRepository>();

              var permissionRecord=
                    articleRepository.GetPermissions(new Guid(articleId)).SingleOrDefault(x=>x.User== emailOfLoggedInUser);

                if (permissionRecord != null)
                {
                    if (permissionRecord.HasAccess==true)
                    {
                        base.OnActionExecuting(context);
                    }
                    else context.Result = new ForbidResult();
                }
                else
                context.Result = new ForbidResult();
            }
            catch (Exception ex) {
                context.Result = new ForbidResult();
            }
        }
    }
}
