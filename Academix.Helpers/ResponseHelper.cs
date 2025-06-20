using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Academix.Helpers
{
    public class ResponseHelper
    {
        public bool status { get; set; } = false;
        public string Massage { get; set; }
        public int? StatusCode { get; set; }
        public object? Data { get; set; }
        public Dictionary<string, List<string>>? Validation { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now.ToLocalTime();
        public string Culture { get; set; } = "en";

        public ResponseHelper WithMassage(string message)
        {
            Massage = message?.Trim() ?? string.Empty;
            return this;
        }

       
        public ResponseHelper()
        {
            status = false;
            Massage = "no content";
        }

        public ResponseHelper WithStatus(bool s)
        {
            status = s;
            return this;
        }
        public ResponseHelper WithData(object viewModel)
        {
            Data = viewModel;
            return this;
        }

        public ResponseHelper WithStatusCode(int code)
        {
            StatusCode = code;
            return this;

        }
        public ResponseHelper WithValidation(ModelStateDictionary keyValuePairs)
        {
            Validation = new Dictionary<string, List<string>>();
            keyValuePairs.ToList().ForEach(keyValue =>
            {
                Validation[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage).ToList();
            });
            status = false;
            StatusCode = 400;
            Massage = Massage ?? "Validation failed";
            return this;

        }

        public ResponseHelper WithIdentityErrors(IEnumerable<IdentityError> errors)
        {
            Validation = new Dictionary<string, List<string>>();
            foreach (var error in errors)
            {
                if (!Validation.ContainsKey(error.Code))
                    Validation[error.Code] = new List<string>();

                Validation[error.Code].Add(error.Description);
            }
            status = false;
            StatusCode = 400;
            Massage = Massage ?? "Validation failed";
            return this;
        }


        public ResponseHelper Success(object data = null, string culture = "en")
        {
            status = true;
            StatusCode = 200;
            Culture = culture;
            Massage = culture == "ar" ? "تمت العملية بنجاح" : "Operation completed successfully";
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Created(object data = null, string culture = "en")
        {
            status = true;
            StatusCode = 201;
            Culture = culture;
            Massage = culture == "ar" ? "تم إنشاء المورد بنجاح" : "Resource created successfully";
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Updated(object data = null, string culture = "en")
        {
            status = true;
            StatusCode = 200;
            Culture = culture;
            Massage = culture == "ar" ? "تم تحديث المورد بنجاح" : "Resource updated successfully";
            Data = data;
            Validation = null;
            return this;
        }
        public ResponseHelper NotFound(string message = null, string culture = "en")
        {
            status = false;
            StatusCode = 404;
            Culture = culture;
            Massage = message ?? (culture == "ar" ? "المورد غير موجود" : "Resource not found");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper BadRequest(string message = null, string culture = "en")
        {
            status = false;
            StatusCode = 400;
            Culture = culture;
            Massage = message ?? (culture == "ar" ? "طلب غير صحيح" : "Invalid request");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Unauthorized(string message = null, string culture = "en")
        {
            status = false;
            StatusCode = 401;
            Culture = culture;
            Massage = message ?? (culture == "ar" ? "وصول غير مصرح به" : "Unauthorized access");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Forbidden(string message = null, string culture = "en")
        {
            status = false;
            StatusCode = 403;
            Culture = culture;
            Massage = message ?? (culture == "ar" ? "الوصول محظور" : "Access forbidden");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper ServerError(string message = null, string culture = "en")
        {
            status = false;
            StatusCode = 500;
            Culture = culture;
            Massage = message ?? (culture == "ar" ? "خطأ داخلي في الخادم" : "Internal server error");
            Data = null;
            Validation = null;
            return this;
        }

        public static IDictionary<string, string> ModelStateToValidation(ModelStateDictionary modelState)
        {
            return modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid value"
            );
        }

        public static implicit operator ActionResult(ResponseHelper response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode ?? 200
            };
        }

    }
}
