using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EP._6._2A_Assignment.Repositories;

public class ValidateUserCanApproveAttribute : ActionFilterAttribute
{
    private readonly ItemsDbRepository _dbRepo;

    public ValidateUserCanApproveAttribute(ItemsDbRepository dbRepo)
    {
        _dbRepo = dbRepo;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var email = user.Identity?.Name; // email of user 
        var ids = context.ActionArguments["selectedIds"] as Guid[];

        if (ids == null || ids.Length == 0)
        {
            context.Result = new BadRequestResult(); // if no id 
            return;
        }
        var dbRepo = context.HttpContext.RequestServices.GetService<IItemsRepository>();

        if (dbRepo == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        // Method GetValidators() which returns owners/admin emails
        var allowedEmails = _dbRepo.GetValidators(ids);

        if (!allowedEmails.Contains(email))
        {
            context.Result = new ForbidResult(); // if not allowed email = 403
            return;
        }

        base.OnActionExecuting(context);
    }
}
