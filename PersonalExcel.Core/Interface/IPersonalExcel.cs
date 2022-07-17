using PersonalExce.Helper.Data.Dto;
using PersonalExeclReader.Data.Entity;
using PersonalExeclReader.Helper.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalExcel.Core.Interface
{
    /// <summary>
    /// Person Interface for CRUD and abstraction
    /// </summary>
    public interface IPersonExcel
    {
        Task<int> ImportData(List<PersonalModel> model);

        Task<int> Create(PersonalModel model);

        Task<int> Update(PersonalModel model);

        Task<int> Delete(int Id);

        Task<List<PersonDto>> Fetch();
    }
}
