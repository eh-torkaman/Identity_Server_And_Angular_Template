using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Extensions
{
    public static class JsonPatchExtensions_local
    {
        public static ActionResult ApplyToAndCheckModel<T>(this JsonPatchDocument<T> patchDoc,
            T objectToApplyTo, ControllerBase controller) where T : class
        {
            //ModelStateDictionary modelState,
            var modelState = controller.ModelState;
            patchDoc.ApplyTo(objectToApplyTo, modelState);
            if (!controller.TryValidateModel(objectToApplyTo))
            {
                var option = controller.HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
                var rs = (ActionResult)option.Value.InvalidModelStateResponseFactory(controller.ControllerContext);

                return rs;
            }
            return null;
        }


    }
}
