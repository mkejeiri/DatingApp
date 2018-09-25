using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static HttpResponse AddApplicationError(this HttpResponse response, string message){
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error"); 
            response.Headers.Add("Access-Control-Allow-Origin","*");
            return response;
        }

        public static int CalculateAge(this DateTime dateOfBirth){            
            var age = DateTime.Now.Year-dateOfBirth.Year;
            if (dateOfBirth.AddYears(age) > DateTime.Now)
            {
                age--;
            }
            return age;
        }
    }
}