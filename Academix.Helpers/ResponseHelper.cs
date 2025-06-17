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
        public ResponseHelper WithMassage(string massage)
        {
            Massage = massage.Trim();
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


        public ResponseHelper Success(object data = null)
        {
            status = true;
            StatusCode = 200;
            Massage = "Operation completed successfully";
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Created(object data = null)
        {
            status = true;
            StatusCode = 201;
            Massage = "Resource created successfully";
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Updated(object data = null)
        {
            status = true;
            StatusCode = 200;
            Massage = "Resource updated successfully";
            Data = data;
            Validation = null;
            return this;
        }
        public ResponseHelper NotFound(string message = "Resource not found")
        {
            status = false;
            StatusCode = 404;
            Massage = message;
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper BadRequest(string message = "Invalid request")
        {
            status = false;
            StatusCode = 400;
            Massage = message;
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Unauthorized(string message = "Unauthorized access")
        {
            status = false;
            StatusCode = 401;
            Massage = message;
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Forbidden(string message = "Access forbidden")
        {
            status = false;
            StatusCode = 403;
            Massage = message;
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper ServerError(string message = "Internal server error")
        {
            status = false;
            StatusCode = 500;
            Massage = message;
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
