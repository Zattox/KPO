using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Application.Abstractions
{
    // Defines the contract for enclosure repository
    public interface IEnclosureRepository
    {
        void Add(Enclosure enclosure);
        void Update(Enclosure enclosure);
        void Remove(Guid enclosureId);
        Enclosure GetById(Guid enclosureId);
        IEnumerable<Enclosure> GetAll();
    }
}