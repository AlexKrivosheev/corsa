using Corsa.Domain.Models.Requests;
using Corsa.Domain.Processing.Moduls;
using System;

namespace Corsa.Domain.Tasks
{
    public interface ITask
    {
        int Id { get; }

        TaskStateDetails State { get; set; }

        bool Start();

        bool Load();

        bool Save();
    }
}
