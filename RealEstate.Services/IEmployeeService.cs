using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Services.ViewModels.Json;

namespace RealEstate.Services
{
    public interface IEmployeeService
    {
        Task<Employee> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems);

        Task<EmployeeInputViewModel> FindInputAsync(string id);

        Task<(StatusEnum, Employee)> AddOrUpdateAsync(EmployeeInputViewModel model, bool update, bool save);

        Task<(StatusEnum, Employee)> UpdateAsync(EmployeeInputViewModel model, bool save);

        Task<(StatusEnum, Employee)> AddAsync(EmployeeInputViewModel model, bool save);

        Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IDivisionService _divisionService;
        private readonly DbSet<Employee> _employees;
        private readonly DbSet<FixedSalary> _fixedSalaries;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IDivisionService divisionService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _divisionService = divisionService;
            _fixedSalaries = _unitOfWork.Set<FixedSalary>();
            _employees = _unitOfWork.Set<Employee>();
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel)
        {
            var models = _employees.AsQueryable();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    models = models.Where(x => EF.Functions.Like(x.Id, searchModel.Id.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.FirstName))
                    models = models.Where(x => EF.Functions.Like(x.FirstName, searchModel.FirstName.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.LastName))
                    models = models.Where(x => EF.Functions.Like(x.LastName, searchModel.LastName.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Mobile))
                    models = models.Where(x => EF.Functions.Like(x.Mobile, searchModel.Mobile.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.Address))
                    models = models.Where(x => EF.Functions.Like(x.Address, searchModel.Address.LikeExpression()));

                if (!string.IsNullOrEmpty(searchModel.UserId))
                    models = models.Where(x => x.Users.Any(c => c.Id == searchModel.UserId));

                if (!string.IsNullOrEmpty(searchModel.DivisionId))
                    models = models.Where(x => x.EmployeeDivisions.Any(c => c.DivisionId == searchModel.DivisionId));
            }

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Employee, EmployeeViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetEmployeeDivisions(false, act2 => act2.GetDivision());
                    act.GetUsers();
                })).ConfigureAwait(false);

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<Employee> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _baseService.QueryByRole(_employees, allowedRolesshowDeletedItems).FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            return result;
        }

        public Task<(StatusEnum, Employee)> AddOrUpdateAsync(EmployeeInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<(StatusEnum, Employee)> UpdateAsync(EmployeeInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Employee>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Employee>(StatusEnum.IdIsNull, null);

            var entity = await EntityAsync(model.Id, null).ConfigureAwait(false);
            var (updateStatus, updatedEmployee) = await _baseService.UpdateAsync(entity,
                () =>
                {
                    entity.Address = model.Address;
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Mobile = model.Mobile;
                    entity.Phone = model.Phone;
                }, new[]
                {
                    Role.SuperAdmin
                }, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            await SyncAsync(updatedEmployee, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(updatedEmployee, save).ConfigureAwait(false);
        }

        public async Task<(StatusEnum, Employee)> AddAsync(EmployeeInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Employee>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.DivisionsJson))
                return new ValueTuple<StatusEnum, Employee>(StatusEnum.DivisionIdIsNull, null);

            var (employeeAddStatus, newEmployee) = await _baseService.AddAsync(new Employee
            {
                Address = model.Address,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                Mobile = model.Mobile,
            }, new[]
            {
                Role.SuperAdmin
            }, false).ConfigureAwait(false);

            await SyncAsync(newEmployee, model, false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(newEmployee, save).ConfigureAwait(false);
        }

        public async Task<StatusEnum> SyncAsync(Employee employee, EmployeeInputViewModel model, bool save)
        {
            var lastFixedSalary = employee.FixedSalaries.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastFixedSalary?.Value.Equals(model.FixedSalary) == false)
            {
                var addFixed = await _baseService.AddAsync(new FixedSalary
                {
                    EmployeeId = employee.Id,
                    Value = model.FixedSalary
                }, null, false).ConfigureAwait(false);
            }

            var result = await _baseService.SyncAsync(
                employee.EmployeeDivisions,
                model.Divisions,
                division => new EmployeeDivision
                {
                    DivisionId = division.Id,
                    EmployeeId = employee.Id
                },
                (inDb, inModel) => inDb.DivisionId == inModel.Id,
                null,
                save).ConfigureAwait(false);
            return result;
        }

        public async Task<EmployeeInputViewModel> FindInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await EntityAsync(id, null).ConfigureAwait(false);
            var viewModel = model?.Into<Employee, EmployeeViewModel>(false, act =>
            {
                act.GetFixedSalaries();
                act.GetEmployeeDivisions(false, act2=>act2.GetDivision());
            });
            if (viewModel == null)
                return default;

            var result = new EmployeeInputViewModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Mobile = viewModel.Mobile,
                Address = viewModel.Address,
                Phone = viewModel.Phone,
                Id = viewModel.Id,
                FixedSalary = viewModel.CurrentFixedSalary?.Value ?? 0,
                Divisions = viewModel.EmployeeDivisions?.Select(x => new DivisionJsonViewModel
                {
                    Id = x.Division.Id,
                    Name = x.Division?.Name
                }).ToList(),
            };
            return result;
        }
    }
}