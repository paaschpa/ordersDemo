using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Common;
using ServiceStack.Common.ServiceClient.Web;
using ServiceStack.FluentValidation;

namespace OrdersDemo.Models
{
    public class MyRegistrationValidator : AbstractValidator<Registration>
    {
        //This will be injected
        public IUserAuthRepository UserAuthRepo { get; set; }

        public MyRegistrationValidator()
        {
            //User must supply a UserName and Password. Username cannot already exist
            RuleSet(ApplyTo.Post, () =>
            {
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty().When(x => x.Email.IsNullOrEmpty());
                RuleFor(x => x.UserName).Length(3, 35);
                RuleFor(x => x.UserName)
                    .Must(x => UserAuthRepo.GetUserAuthByUserName(x) == null)
                    .WithErrorCode("AlreadyExists")
                    .WithMessage("UserName already exists")
                    .When(x => !x.UserName.IsNullOrEmpty());
            });
            RuleSet(ApplyTo.Put, () =>
            {
                RuleFor(x => x.UserName).NotEmpty();
            });
        }
    }
}