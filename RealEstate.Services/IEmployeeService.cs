using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Base;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IEmployeeService
    {
        Task<Employee> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems);

        Task<PresenceInputViewModel> PresenceInputAsync(string id);

        Task<PaginationViewModel<LeaveViewModel>> LeaveListAsync(LeaveSearchViewModel searchModel);

        Task<(StatusEnum, Leave)> LeaveAddOrUpdateAsync(LeaveInputViewModel model, bool update, bool save);

        Task<StatusEnum> PresenceRemoveAsync(string id);

        Task<(StatusEnum, Presence)> PresenceAddOrUpdateAsync(PresenceInputViewModel model, bool update, bool save);

        Task<PaginationViewModel<PresenceViewModel>> PresenceListAsync(PresenceSearchViewModel searchModel);

        Task<EmployeeInputViewModel> FindInputAsync(string id);

        Task<(StatusEnum, Employee)> AddOrUpdateAsync(EmployeeInputViewModel model, bool update, bool save);

        Task<LeaveInputViewModel> LeaveInputAsync(string id);

        Task<(StatusEnum, Employee)> UpdateAsync(EmployeeInputViewModel model, bool save);

        Task<StatusEnum> LeaveRemoveAsync(string id);

        Task<List<EmployeeViewModel>> ListAsync();

        Task<(StatusEnum, Employee)> AddAsync(EmployeeInputViewModel model, bool save);

        Task<StatusEnum> EmployeeRemoveAsync(string id);

        Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IDivisionService _divisionService;
        private readonly DbSet<Employee> _employees;
        private readonly DbSet<FixedSalary> _fixedSalaries;
        private readonly DbSet<Leave> _leaves;
        private readonly DbSet<Presence> _presences;

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
            _leaves = _unitOfWork.Set<Leave>();
            _presences = _unitOfWork.Set<Presence>();
        }

        public async Task<List<EmployeeViewModel>> ListAsync()
        {
            var employees = await _employees.WhereNotDeleted().ToListAsync().ConfigureAwait(false);
            return employees.Into<Employee, EmployeeViewModel>();
        }

        public async Task<StatusEnum> EmployeeRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var employee = await _employees.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (employee == null)
                return StatusEnum.EmployeeIsNull;

            var statuses = employee.EmployeeStatuses;
            var lastStatus = statuses?.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastStatus == null)
            {
                await _baseService.AddAsync(new EmployeeStatus
                {
                    EmployeeId = employee.Id,
                    Status = EmployeeStatusEnum.Start
                }, null, false).ConfigureAwait(false);
            }
            else
            {
                await _baseService.AddAsync(new EmployeeStatus
                {
                    EmployeeId = employee.Id,
                    Status = lastStatus.Status == EmployeeStatusEnum.End ? EmployeeStatusEnum.Start : EmployeeStatusEnum.End
                }, null, false).ConfigureAwait(false);
            }

            return await _baseService.SaveChangesAsync(true).ConfigureAwait(false);
        }

        public async Task<StatusEnum> PresenceRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _presences.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<StatusEnum> LeaveRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _leaves.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    },
                    true,
                    true)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel)
        {
            var models = _employees.AsQueryable();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    models = models.Where(x => x.Id == searchModel.Id);

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
                    act.GetFixedSalaries();
                    act.GetInsurances();
                    act.GetEmployeeStatuses();
                })).ConfigureAwait(false);

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<PaginationViewModel<PresenceViewModel>> PresenceListAsync(PresenceSearchViewModel searchModel)
        {
            var models = _presences.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Presence, PresenceViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetEmployee();
                })).ConfigureAwait(false);

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<PaginationViewModel<LeaveViewModel>> LeaveListAsync(LeaveSearchViewModel searchModel)
        {
            var models = _leaves.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel?.PageNo ?? 1,
                item => item.Into<Leave, LeaveViewModel>(_baseService.IsAllowed(Role.SuperAdmin, Role.Admin), act =>
                {
                    act.GetEmployee();
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

            var entity = await _employees
                .Include(x => x.FixedSalaries)
                .Include(x => x.Insurances)
                .FirstOrDefaultAsync(x => x.Id == model.Id)
                .ConfigureAwait(false);
            var (updateStatus, updatedEmployee) = await _baseService.UpdateAsync(entity,
                _ =>
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

        public Task<(StatusEnum, Presence)> PresenceAddOrUpdateAsync(PresenceInputViewModel model, bool update, bool save)
        {
            return update
                ? PresenceUpdateAsync(model, save)
                : PresenceAddAsync(model, save);
        }

        public Task<(StatusEnum, Leave)> LeaveAddOrUpdateAsync(LeaveInputViewModel model, bool update, bool save)
        {
            return update
                ? LeaveUpdateAsync(model, save)
                : LeaveAddAsync(model, save);
        }

        public async Task<(StatusEnum, Leave)> LeaveUpdateAsync(LeaveInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.IdIsNull, null);

            var entity = await _leaves.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (entity == null)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.LeaveIsNull, null);

            var creationDate = entity.Audits.Find(x => x.Type == LogTypeEnum.Create);
            if (creationDate == null)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.EntityIsNull, null);

            if (DateTime.Now.Subtract(creationDate.DateTime) >= TimeSpan.FromHours(1))
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.Forbidden, null);

            var fromDt = model.FromDate.PersianToGregorian();
            var fromDDD = new DateTime(fromDt.Year, fromDt.Month, fromDt.Day, model.FromHour, 0, 0);

            var toDt = model.ToDate.PersianToGregorian();
            var toDDD = new DateTime(toDt.Year, toDt.Month, toDt.Day, model.ToHour, 0, 0);

            var updateStatus = await _baseService.UpdateAsync(entity,
                    _ =>
                    {
                        entity.EmployeeId = model.EmployeeId;
                        entity.From = fromDDD;
                        entity.Reason = model.Reason;
                        entity.To = toDDD;
                    }, new[]
                    {
                    Role.SuperAdmin
                    }, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            return updateStatus;
        }

        public async Task<(StatusEnum, Presence)> PresenceUpdateAsync(PresenceInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.IdIsNull, null);

            var entity = await _presences.FirstOrDefaultAsync(x => x.Id == model.Id).ConfigureAwait(false);
            if (entity == null)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.LeaveIsNull, null);

            var creationDate = entity.Audits.Find(x => x.Type == LogTypeEnum.Create);
            if (creationDate == null)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.EntityIsNull, null);

            if (DateTime.Now.Subtract(creationDate.DateTime) >= TimeSpan.FromMinutes(30))
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.Forbidden, null);

            var timeFa = model.Time.Split(':');
            var hour = int.Parse(timeFa[0]);
            var minute = int.Parse(timeFa[1]);

            var updateStatus = await _baseService.UpdateAsync(entity,
                _ =>
                {
                    entity.EmployeeId = model.EmployeeId;
                    entity.Date = model.Date.PersianToGregorian();
                    entity.Hour = hour;
                    entity.Minute = minute;
                }, new[]
                {
                    Role.SuperAdmin
                }, false, StatusEnum.UserIsNull).ConfigureAwait(false);

            return updateStatus;
        }

        public async Task<(StatusEnum, Presence)> PresenceAddAsync(PresenceInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.EmployeeIsNull, null);

            var employee = await EntityAsync(model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new ValueTuple<StatusEnum, Presence>(StatusEnum.EmployeeIsNull, null);

            var timeFa = model.Time.Split(':');
            var hour = int.Parse(timeFa[0]);
            var minute = int.Parse(timeFa[1]);

            var addStatus = await _baseService.AddAsync(new Presence
            {
                EmployeeId = model.EmployeeId,
                Date = model.Date.PersianToGregorian(),
                Hour = hour,
                Minute = minute
            }, new[]
            {
                Role.SuperAdmin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        public async Task<(StatusEnum, Leave)> LeaveAddAsync(LeaveInputViewModel model, bool save)
        {
            if (model == null)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.EmployeeIsNull, null);

            var employee = await EntityAsync(model.EmployeeId).ConfigureAwait(false);
            if (employee == null)
                return new ValueTuple<StatusEnum, Leave>(StatusEnum.EmployeeIsNull, null);

            var fromDt = model.FromDate.PersianToGregorian();
            var fromDDD = new DateTime(fromDt.Year, fromDt.Month, fromDt.Day, model.FromHour, 0, 0);

            var toDt = model.ToDate.PersianToGregorian();
            var toDDD = new DateTime(toDt.Year, toDt.Month, toDt.Day, model.ToHour, 0, 0);

            var addStatus = await _baseService.AddAsync(new Leave
            {
                EmployeeId = model.EmployeeId,
                From = fromDDD,
                Reason = model.Reason,
                To = toDDD
            }, new[]
            {
                Role.SuperAdmin
            }, save).ConfigureAwait(false);
            return addStatus;
        }

        private async Task<StatusEnum> SyncAsync(Employee employee, EmployeeInputViewModel model, bool save)
        {
            var lastFixedSalary = employee.FixedSalaries?.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastFixedSalary == null)
            {
                if (model.FixedSalary > 0)
                {
                    var addFixed = await _baseService.AddAsync(new FixedSalary
                    {
                        EmployeeId = employee.Id,
                        Value = model.FixedSalary
                    },
                        null,
                        false).ConfigureAwait(false);
                }
            }
            else
            {
                if (!lastFixedSalary.Value.Equals(model.FixedSalary))
                {
                    var addFixed = await _baseService.AddAsync(new FixedSalary
                    {
                        EmployeeId = employee.Id,
                        Value = model.FixedSalary
                    },
                        null,
                        false).ConfigureAwait(false);
                }
            }

            var lastInsurance = employee.Insurances?.OrderDescendingByCreationDateTime().FirstOrDefault();
            if (lastInsurance == null)
            {
                if (model.Insurance > 0)
                {
                    var addFixed = await _baseService.AddAsync(new Insurance
                    {
                        EmployeeId = employee.Id,
                        Price = model.Insurance
                    },
                        null,
                        false).ConfigureAwait(false);
                }
            }
            else
            {
                if (!lastInsurance.Price.Equals(model.Insurance))
                {
                    var addFixed = await _baseService.AddAsync(new Insurance
                    {
                        EmployeeId = employee.Id,
                        Price = model.Insurance
                    },
                        null,
                        false).ConfigureAwait(false);
                }
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
                false).ConfigureAwait(false);
            return await _baseService.SaveChangesAsync(save).ConfigureAwait(false);
        }

        public async Task<LeaveInputViewModel> LeaveInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await _leaves.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = model?.Into<Leave, LeaveViewModel>(false, act =>
            {
                act.GetEmployee();
            });
            if (viewModel == null)
                return default;

            var result = new LeaveInputViewModel
            {
                Id = viewModel.Id,
                EmployeeId = viewModel.Employee?.Id,
                Reason = viewModel.Reason,
                FromDate = viewModel.From.Date.GregorianToPersian(true),
                FromHour = viewModel.From.Hour,
                ToDate = viewModel.To.Date.GregorianToPersian(true),
                ToHour = viewModel.To.Hour
            };
            return result;
        }

        public async Task<PresenceInputViewModel> PresenceInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await _presences.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            var viewModel = model?.Into<Presence, PresenceViewModel>(false, act =>
            {
                act.GetEmployee();
            });
            if (viewModel == null)
                return default;

            var result = new PresenceInputViewModel
            {
                Id = viewModel.Id,
                Status = viewModel.Status,
                Time = $"{viewModel.Hour}:{viewModel.Minute}",
                Date = viewModel.Date.GregorianToPersian(true)
            };
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
                act.GetInsurances();
                act.GetEmployeeDivisions(false, act2 => act2.GetDivision());
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
                Insurance = viewModel.CurrentInsurance?.Price ?? 0
            };
            return result;
        }
    }
}