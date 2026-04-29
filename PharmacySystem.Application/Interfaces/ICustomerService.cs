using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Customer;

namespace PharmacySystem.Application.Interfaces;

public interface ICustomerService
{
    Task<ApiResponse<PagedResult<CustomerListDto>>>  GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponseDto>>           GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponseDto>>           GetByPhoneAsync(string phone, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponseDto>>           CreateAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponseDto>>           UpdateAsync(UpdateCustomerDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                          DeleteAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<List<CustomerListDto>>>         GetTopCustomersAsync(int count = 10, CancellationToken ct = default);
}
