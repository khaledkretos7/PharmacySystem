using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Customer;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IUnitOfWork uow, IMapper mapper, ILogger<CustomerService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<CustomerListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Customers.GetPagedAsync(request.PageNumber, request.PageSize, ct: ct);
        var dtos = _mapper.Map<List<CustomerListDto>>(items);
        return ApiResponse<PagedResult<CustomerListDto>>.Ok(
            PagedResult<CustomerListDto>.Create(dtos, total, request.PageNumber, request.PageSize));
    }

    public async Task<ApiResponse<CustomerResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Customer), id);
        return ApiResponse<CustomerResponseDto>.Ok(_mapper.Map<CustomerResponseDto>(entity));
    }

    public async Task<ApiResponse<CustomerResponseDto>> GetByPhoneAsync(string phone, CancellationToken ct = default)
    {
        var entity = await _uow.Customers.GetByPhoneAsync(phone, ct)
                     ?? throw new NotFoundException(nameof(Customer), phone);
        return ApiResponse<CustomerResponseDto>.Ok(_mapper.Map<CustomerResponseDto>(entity));
    }

    public async Task<ApiResponse<CustomerResponseDto>> CreateAsync(CreateCustomerDto dto, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            var exists = await _uow.Customers.AnyAsync(c => c.Phone == dto.Phone && !c.IsDeleted, ct);
            if (exists) throw new DuplicateEntryException("Phone", dto.Phone);
        }

        var entity = _mapper.Map<Customer>(dto);
        await _uow.Customers.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<CustomerResponseDto>.Created(_mapper.Map<CustomerResponseDto>(entity));
    }

    public async Task<ApiResponse<CustomerResponseDto>> UpdateAsync(UpdateCustomerDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(Customer), dto.Id);
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Customers.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<CustomerResponseDto>.Ok(_mapper.Map<CustomerResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Customers.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Customer), id);
        entity.IsDeleted = true;
        entity.IsActive  = false;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Customers.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<List<CustomerListDto>>> GetTopCustomersAsync(int count = 10, CancellationToken ct = default)
    {
        var list = await _uow.Customers.GetTopCustomersAsync(count, ct);
        return ApiResponse<List<CustomerListDto>>.Ok(_mapper.Map<List<CustomerListDto>>(list));
    }
}
