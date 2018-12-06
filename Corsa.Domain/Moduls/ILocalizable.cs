using Corsa.Domain.Models.Requests;
using Corsa.Domain.Processing.Moduls;
using Microsoft.Extensions.Localization;

namespace Corsa.Domain.Moduls
{
    public interface ILocalizable
    {
        void AssignLocalizer<TLocalizer>(IStringLocalizer<TLocalizer> localizer);        
    }
}
