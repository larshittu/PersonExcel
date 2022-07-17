using PersonalExce.Helper.Data.Dto;
using PersonalExcel.Core.Interface;
using PersonalExeclReader.Data.DAL;
using PersonalExeclReader.Data.Entity;
using PersonalExeclReader.Helper.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalExcel.Core
{
    /// <summary>
    /// Person class for CRUD operation
    /// </summary>
    public class PersonExcel : IPersonExcel
    {
        private readonly PersonalContext _context;

        public PersonExcel(PersonalContext context)
        {
            _context = context;
        }

        public Task<int> Create(PersonalModel model)
        {
            _context.Persons.Add(new Personal
            {
                Id = model.Id,
                FirstName = model.FirstName,
                Sex = model.Sex,
                Surname = model.Surname,
                Active = model.Active,
                Age = model.Age,
                Mobile = model.Mobile,
            });
            var status = _context.SaveChanges();
            return Task.FromResult(status);
        }

        public Task<int> Delete(int id)
        {
            var getPerson = _context.Persons.Find(id);
            _context.Persons.Remove(getPerson);
            var status = _context.SaveChanges();
            return Task.FromResult(status);
        }

        public Task<List<PersonDto>> Fetch()
        {
            var list = _context.Persons.Select(p => new PersonDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                Sex = p.Sex,
                Surname = p.Surname,
                Active = p.Active,
                Age = p.Age,
                Mobile = p.Mobile,
            }).ToList();
            return Task.FromResult(list);
        }

        public Task<int> ImportData(List<PersonalModel> model)
        {
            var newRecord = model.Select(p => new Personal
            {
                FirstName = p.FirstName,
                Sex = p.Sex,
                Surname = p.Surname,
                Active = p.Active,
                Age = p.Age,
                Mobile = p.Mobile,
            });

            _context.Persons.AddRange(newRecord);
            var status = _context.SaveChanges();
            return Task.FromResult(status);
        }

        public Task<int> Update(PersonalModel model)
        {
            var getPerson = _context.Persons.SingleOrDefault(p => p.Id == model.Id);
            if (getPerson != null)
            {
                getPerson.Id = model.Id;
                getPerson.FirstName = model.FirstName;
                getPerson.Sex = model.Sex;
                getPerson.Surname = model.Surname;
                getPerson.Active = model.Active;
                getPerson.Age = model.Age;
                getPerson.Mobile = model.Mobile;

                var status = _context.SaveChanges();
                return Task.FromResult(status);
            }
            return Task.FromResult(0);
        }
    }
}
