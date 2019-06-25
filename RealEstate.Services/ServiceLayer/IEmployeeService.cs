using Microsoft.EntityFrameworkCore;
using RealEstate.Base;
using RealEstate.Base.Enums;
using RealEstate.Services.Database;
using RealEstate.Services.Database.Tables;
using RealEstate.Services.Extensions;
using RealEstate.Services.ServiceLayer.Base;
using RealEstate.Services.ViewModels;
using RealEstate.Services.ViewModels.Input;
using RealEstate.Services.ViewModels.Json;
using RealEstate.Services.ViewModels.ModelBind;
using RealEstate.Services.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Services.ServiceLayer
{
    public interface IEmployeeService
    {
        Task<Employee> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems);

        Task<PresenceInputViewModel> PresenceInputAsync(string id);

        Task<PaginationViewModel<LeaveViewModel>> LeaveListAsync(LeaveSearchViewModel searchModel);

        Task<MethodStatus<Leave>> LeaveAddOrUpdateAsync(LeaveInputViewModel model, bool update, bool save);

        Task<StatusEnum> PresenceRemoveAsync(string id);

        Task<MethodStatus<Presence>> PresenceAddOrUpdateAsync(PresenceInputViewModel model, bool update, bool save);

        Task<PaginationViewModel<PresenceViewModel>> PresenceListAsync(PresenceSearchViewModel searchModel);

        Task<EmployeeInputViewModel> EmployeeInputAsync(string id);

        Task<MethodStatus<Employee>> AddOrUpdateAsync(EmployeeInputViewModel model, bool update, bool save);

        Task<LeaveInputViewModel> LeaveInputAsync(string id);

        Task<MethodStatus<Employee>> UpdateAsync(EmployeeInputViewModel model, bool save);

        Task<StatusEnum> LeaveRemoveAsync(string id);

        Task<List<EmployeeViewModel>> ListAsync(bool justFreeEmployees);

        Task<EmployeeDetailViewModel> DetailAsync(string id);

        Task<MethodStatus<Employee>> AddAsync(EmployeeInputViewModel model, bool save);

        Task<StatusEnum> EmployeeRemoveAsync(string id);

        Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseService _baseService;
        private readonly IPaymentService _paymentService;
        private readonly DbSet<Employee> _employees;
        private readonly DbSet<Leave> _leaves;
        private readonly DbSet<Presence> _presences;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IBaseService baseService,
            IPaymentService paymentService
            )
        {
            _unitOfWork = unitOfWork;
            _baseService = baseService;
            _paymentService = paymentService;
            _employees = _unitOfWork.Set<Employee>();
            _leaves = _unitOfWork.Set<Leave>();
            _presences = _unitOfWork.Set<Presence>();
        }

        public async Task<EmployeeDetailViewModel> DetailAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var employee = await _employees
                .IgnoreQueryFilters()
                .Include(x => x.Payments)
                .ThenInclude(x => x.Checkout)
                .FirstOrDefaultAsync(x => x.Id == id)
                ;
            var viewModel = employee?.Map<EmployeeViewModel>();
            if (viewModel == null)
                return default;

            var (currentMoney, pays) = await _paymentService.PaymentLastStateAsync(employee);
            var result = new EmployeeDetailViewModel
            {
                Id = viewModel.Id,
                Payments = new EmployeeDetailPaymentViewModel
                {
                    Current = currentMoney < 0 ? currentMoney * -1 : currentMoney,
                    Status = currentMoney == 0 ? ObligStatusEnum.None : currentMoney < 0 ? ObligStatusEnum.Obligor : ObligStatusEnum.Obligee,
                    List = pays?.Select(item => item.Map<PaymentViewModel>()).ToList()
                }
            };

            return result;
        }

        public async Task<List<EmployeeViewModel>> ListAsync(bool justFreeEmployees)
        {
            var query = _employees.AsQueryable();

            if (justFreeEmployees)
                query = query.Where(x => x.Users.Count == 0);

            var employees = await query.ToListAsync();
            return employees.Map<Employee, EmployeeViewModel>();
        }

        public async Task<StatusEnum> EmployeeRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var employee = await _employees.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(employee,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> PresenceRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _presences.FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<StatusEnum> LeaveRemoveAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return StatusEnum.ParamIsNull;

            var user = await _leaves.FirstOrDefaultAsync(x => x.Id == id);
            var result = await _baseService.RemoveAsync(user,
                    new[]
                    {
                        Role.SuperAdmin, Role.Admin
                    })
                ;

            return result;
        }

        public async Task<PaginationViewModel<EmployeeViewModel>> ListAsync(EmployeeSearchViewModel searchModel)
        {
            var query = _baseService.CheckDeletedItemsPrevillege(_employees, searchModel, out var currentUser);
            if (query == null)
                return new PaginationViewModel<EmployeeViewModel>();

            query = query.AsNoTracking()
                .Include(x => x.Insurances)
                .Include(x => x.FixedSalaries)
                .Include(x => x.Payments)
                .Include(x => x.Users)
                .Include(x => x.EmployeeDivisions)
                .ThenInclude(x => x.Division);

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Id))
                    query = query.Where(x => x.Id == searchModel.Id);

                if (!string.IsNullOrEmpty(searchModel.FirstName))
                    query = query.Where(x => EF.Functions.Like(x.FirstName, searchModel.FirstName.Like()));

                if (!string.IsNullOrEmpty(searchModel.LastName))
                    query = query.Where(x => EF.Functions.Like(x.LastName, searchModel.LastName.Like()));

                if (!string.IsNullOrEmpty(searchModel.Mobile))
                    query = query.Where(x => EF.Functions.Like(x.Mobile, searchModel.Mobile.Like()));

                if (!string.IsNullOrEmpty(searchModel.Address))
                    query = query.Where(x => EF.Functions.Like(x.Address, searchModel.Address.Like()));

                if (!string.IsNullOrEmpty(searchModel.UserId))
                    query = query.Where(x => x.Users.Any(c => c.Id == searchModel.UserId));

                if (!string.IsNullOrEmpty(searchModel.DivisionId))
                    query = query.Where(x => x.EmployeeDivisions.Any(c => c.DivisionId == searchModel.DivisionId));

                if (!string.IsNullOrEmpty(searchModel.Phone))
                    query = query.Where(x => EF.Functions.Like(x.Phone, searchModel.Phone.Like()));

                query = _baseService.AdminSeachConditions(query, searchModel);
            }

            var result = await _baseService.PaginateAsync(query, searchModel,
                item => item.Map<EmployeeViewModel>(ent =>
                {
                    ent.IncludeAs<Employee, Insurance, InsuranceViewModel>(_unitOfWork, x => x.Insurances);
                    ent.IncludeAs<Employee, FixedSalary, FixedSalaryViewModel>(_unitOfWork, x => x.FixedSalaries);
                    ent.IncludeAs<Employee, Payment, PaymentViewModel>(_unitOfWork, x => x.Payments);
                    ent.IncludeAs<Employee, User, UserViewModel>(_unitOfWork, x => x.Users);
                    ent.IncludeAs<Employee, EmployeeDivision, EmployeeDivisionViewModel>(_unitOfWork, x => x.EmployeeDivisions,
                        ent2 => ent2.IncludeAs<EmployeeDivision, Division, DivisionViewModel>(_unitOfWork, x => x.Division));
                }), Task.FromResult(false), currentUser);

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<PaginationViewModel<PresenceViewModel>> PresenceListAsync(PresenceSearchViewModel searchModel)
        {
            var models = _presences.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel,
                item => item.Map<PresenceViewModel>(), Task.FromResult(false));

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<PaginationViewModel<LeaveViewModel>> LeaveListAsync(LeaveSearchViewModel searchModel)
        {
            var models = _leaves.AsQueryable();

            var result = await _baseService.PaginateAsync(models, searchModel,
                item => item.Map<LeaveViewModel>(), Task.FromResult(false));

            if (result?.Items?.Any() != true)
                return result;

            return result;
        }

        public async Task<Employee> EntityAsync(string id, params Role[] allowedRolesshowDeletedItems)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var result = await _baseService.QueryByRole(_employees, allowedRolesshowDeletedItems).FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public Task<MethodStatus<Employee>> AddOrUpdateAsync(EmployeeInputViewModel model, bool update, bool save)
        {
            return update
                ? UpdateAsync(model, save)
                : AddAsync(model, save);
        }

        public async Task<MethodStatus<Employee>> UpdateAsync(EmployeeInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Employee>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Employee>(StatusEnum.IdIsNull, null);

            var entity = await _employees
                .Include(x => x.FixedSalaries)
                .Include(x => x.Insurances)
                .FirstOrDefaultAsync(x => x.Id == model.Id)
                ;
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
                }, false, StatusEnum.UserIsNull);

            await SyncAsync(updatedEmployee, model, false);
            return await _baseService.SaveChangesAsync(updatedEmployee, save);
        }

        public async Task<MethodStatus<Employee>> AddAsync(EmployeeInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Employee>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.DivisionsJson))
                return new MethodStatus<Employee>(StatusEnum.DivisionIdIsNull, null);

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
            }, false);

            await SyncAsync(newEmployee, model, false);
            return await _baseService.SaveChangesAsync(newEmployee, save);
        }

        public Task<MethodStatus<Presence>> PresenceAddOrUpdateAsync(PresenceInputViewModel model, bool update, bool save)
        {
            return update
                ? PresenceUpdateAsync(model, save)
                : PresenceAddAsync(model, save);
        }

        public Task<MethodStatus<Leave>> LeaveAddOrUpdateAsync(LeaveInputViewModel model, bool update, bool save)
        {
            return update
                ? LeaveUpdateAsync(model, save)
                : LeaveAddAsync(model, save);
        }

        public async Task<MethodStatus<Leave>> LeaveUpdateAsync(LeaveInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Leave>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Leave>(StatusEnum.IdIsNull, null);

            var entity = await _leaves.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return new MethodStatus<Leave>(StatusEnum.LeaveIsNull, null);

            var creationDate = entity.Audits.Find(x => x.Type == LogTypeEnum.Create);
            if (creationDate == null)
                return new MethodStatus<Leave>(StatusEnum.EntityIsNull, null);

            if (DateTime.Now.Subtract(creationDate.DateTime) >= TimeSpan.FromHours(1))
                return new MethodStatus<Leave>(StatusEnum.Forbidden, null);

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
                    }, false, StatusEnum.UserIsNull);

            return updateStatus;
        }

        public async Task<MethodStatus<Presence>> PresenceUpdateAsync(PresenceInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Presence>(StatusEnum.ModelIsNull, null);

            if (model.IsNew)
                return new MethodStatus<Presence>(StatusEnum.IdIsNull, null);

            var entity = await _presences.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return new MethodStatus<Presence>(StatusEnum.LeaveIsNull, null);

            var creationDate = entity.Audits.Find(x => x.Type == LogTypeEnum.Create);
            if (creationDate == null)
                return new MethodStatus<Presence>(StatusEnum.EntityIsNull, null);

            if (DateTime.Now.Subtract(creationDate.DateTime) >= TimeSpan.FromMinutes(30))
                return new MethodStatus<Presence>(StatusEnum.Forbidden, null);

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
                }, save, StatusEnum.UserIsNull);

            return updateStatus;
        }

        public async Task<MethodStatus<Presence>> PresenceAddAsync(PresenceInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Presence>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new MethodStatus<Presence>(StatusEnum.EmployeeIsNull, null);

            var employee = await EntityAsync(model.EmployeeId);
            if (employee == null)
                return new MethodStatus<Presence>(StatusEnum.EmployeeIsNull, null);

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
            }, save);
            return addStatus;
        }

        public async Task<MethodStatus<Leave>> LeaveAddAsync(LeaveInputViewModel model, bool save)
        {
            if (model == null)
                return new MethodStatus<Leave>(StatusEnum.ModelIsNull, null);

            if (string.IsNullOrEmpty(model.EmployeeId))
                return new MethodStatus<Leave>(StatusEnum.EmployeeIsNull, null);

            var employee = await EntityAsync(model.EmployeeId);
            if (employee == null)
                return new MethodStatus<Leave>(StatusEnum.EmployeeIsNull, null);

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
            }, save);
            return addStatus;
        }

        private async Task<StatusEnum> SyncAsync(Employee employee, EmployeeInputViewModel model, bool save)
        {
            if (model.FixedSalary != null && model.FixedSalary > 0)
            {
                var lastFixedSalary = employee.FixedSalaries?.OrderDescendingByCreationDateTime().FirstOrDefault();
                if (lastFixedSalary?.Value.Equals(model.FixedSalary) != true)
                {
                    var addFixed = await _baseService.AddAsync(new FixedSalary
                    {
                        EmployeeId = employee.Id,
                        Value = (double)model.FixedSalary
                    },
                        null,
                        false);
                }
            }
            if (model.Insurance != null && model.Insurance > 0)
            {
                var lastInsurance = employee.Insurances?.OrderDescendingByCreationDateTime().FirstOrDefault();
                if (lastInsurance?.Price.Equals(model.Insurance) != true)
                {
                    var addFixed = await _baseService.AddAsync(new Insurance
                    {
                        EmployeeId = employee.Id,
                        Price = (double)model.Insurance
                    },
                        null,
                        false);
                }
            }

            await _baseService.SyncAsync(
                employee.EmployeeDivisions,
                model.Divisions,
                (division, currentUser) => new EmployeeDivision
                {
                    DivisionId = division.DivisionId,
                    EmployeeId = employee.Id
                },
                inDb => inDb.DivisionId,
                (inDb, inModel) => inDb.DivisionId == inModel.DivisionId,
                null,
                null,
                null);
            return await _baseService.SaveChangesAsync();
        }

        public async Task<LeaveInputViewModel> LeaveInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await _leaves.FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = model?.Map<LeaveViewModel>();
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

            var model = await _presences.FirstOrDefaultAsync(x => x.Id == id);
            var viewModel = model?.Map<PresenceViewModel>();
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

        public async Task<EmployeeInputViewModel> EmployeeInputAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return default;

            var model = await EntityAsync(id, null);
            var viewModel = model?.Map<EmployeeViewModel>(ent =>
            {
                ent.IncludeAs<Employee, EmployeeDivision, EmployeeDivisionViewModel>(_unitOfWork, x => x.EmployeeDivisions,
                    ent2 => ent2.IncludeAs<EmployeeDivision, Division, DivisionViewModel>(_unitOfWork, x => x.Division));
                ent.IncludeAs<Employee, Insurance, InsuranceViewModel>(_unitOfWork, x => x.Insurances);
                ent.IncludeAs<Employee, FixedSalary, FixedSalaryViewModel>(_unitOfWork, x => x.FixedSalaries);
            });
            if (viewModel == null)
                return default;

            var fixedSalary = viewModel.CurrentFixedSalary;
            var insurance = viewModel.CurrentInsurance;
            var result = new EmployeeInputViewModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Mobile = viewModel.Mobile,
                Address = viewModel.Address,
                Phone = viewModel.Phone,
                Id = viewModel.Id,
                FixedSalary = fixedSalary?.Value ?? 0,
                Divisions = viewModel.EmployeeDivisions?.Select(x => new DivisionJsonViewModel
                {
                    DivisionId = x.Division?.Id,
                    Name = x.Division?.Name
                }).ToList(),
                Insurance = insurance?.Price ?? 0
            };
            return result;
        }
    }
}