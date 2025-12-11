// Application/Services/SupplierService.cs
using InnomateApp.Application.Common;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.DTOs.Requests;
using InnomateApp.Application.Interfaces;
using InnomateApp.Application.Interfaces.IServices;
using InnomateApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(
            ISupplierRepository supplierRepository,
            ILogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        public async Task<Result<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new supplier: {SupplierName}", request.Name);

                // Validate request using data annotations
                var validationContext = new ValidationContext(request);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

                if (!isValid)
                {
                    var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for supplier creation: {Errors}", string.Join(", ", errors));
                    return Result<SupplierDto>.ValidationError(errors);
                }

                // Check for duplicate supplier
                if (await _supplierRepository.SupplierExistsAsync(request.Name.Trim(), request.Email.Trim().ToLower()))
                {
                    _logger.LogWarning("Supplier creation failed: Duplicate supplier found with Name: {Name} or Email: {Email}",
                        request.Name, request.Email);
                    return Result<SupplierDto>.Failure("A supplier with the same name or email already exists.");
                }

                // Create and save supplier
                var supplier = new Supplier
                {
                    Name = request.Name.Trim(),
                    Email = request.Email.Trim().ToLower(),
                    Phone = request.Phone.Trim(),
                    Address = request.Address?.Trim(),
                    ContactPerson = request.ContactPerson?.Trim(),
                    Notes = request.Notes?.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                // Validate entity business rules
                if (!supplier.IsValid())
                {
                    _logger.LogWarning("Supplier entity validation failed for: {SupplierName}", request.Name);
                    return Result<SupplierDto>.Failure("Supplier data is invalid. Please check name, email, and phone.");
                }

                var createdSupplier = await _supplierRepository.AddAsync(supplier);
                var supplierDto = MapToDto(createdSupplier);

                _logger.LogInformation("Supplier created successfully: {SupplierName} (ID: {SupplierId})",
                    supplier.Name, supplier.SupplierId);

                return Result<SupplierDto>.Success(supplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier {SupplierName}", request.Name);
                return Result<SupplierDto>.Failure("An unexpected error occurred while creating the supplier. Please try again.");
            }
        }

        public async Task<Result<SupplierDto>> UpdateSupplierAsync(int id, UpdateSupplierRequest request)
        {
            try
            {
                _logger.LogInformation("Updating supplier ID: {SupplierId}", id);

                // Validate request using data annotations
                var validationContext = new ValidationContext(request);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

                if (!isValid)
                {
                    var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for supplier update: {Errors}", string.Join(", ", errors));
                    return Result<SupplierDto>.ValidationError(errors);
                }

                // Get existing supplier
                var existingSupplier = await _supplierRepository.GetByIdAsync(id);
                if (existingSupplier == null)
                {
                    _logger.LogWarning("Supplier update failed: Supplier with ID {SupplierId} not found", id);
                    return Result<SupplierDto>.NotFound($"Supplier with ID {id} not found.");
                }

                // Check if the supplier is being updated to a duplicate name or email
                if (await _supplierRepository.SupplierExistsAsync(id, request.Name.Trim(), request.Email.Trim().ToLower()))
                {
                    _logger.LogWarning("Supplier update failed: Duplicate supplier found with Name: {Name} or Email: {Email}",
                        request.Name, request.Email);
                    return Result<SupplierDto>.Failure("Another supplier with the same name or email already exists.");
                }

                // Store original values for logging
                var originalName = existingSupplier.Name;
                var originalEmail = existingSupplier.Email;

                // Update supplier entity
                existingSupplier.Update(
                    request.Name.Trim(),
                    request.Email.Trim().ToLower(),
                    request.Phone.Trim(),
                    request.Address?.Trim(),
                    request.ContactPerson?.Trim(),
                    request.Notes?.Trim()
                );

                // Validate updated entity
                if (!existingSupplier.IsValid())
                {
                    _logger.LogWarning("Updated supplier entity validation failed for ID: {SupplierId}", id);
                    return Result<SupplierDto>.Failure("Updated supplier data is invalid. Please check name, email, and phone.");
                }

                // Save changes
                await _supplierRepository.UpdateAsync(existingSupplier);

                // Map to DTO using the updated existingSupplier
                var supplierDto = MapToDto(existingSupplier);

                _logger.LogInformation("Supplier updated successfully: {SupplierName} (ID: {SupplierId}). " +
                    "Changed from Name: {OriginalName} to {NewName}, Email: {OriginalEmail} to {NewEmail}",
                    supplierDto.Name, id, originalName, supplierDto.Name, originalEmail, supplierDto.Email);

                return Result<SupplierDto>.Success(supplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier ID: {SupplierId}", id);
                return Result<SupplierDto>.Failure("An unexpected error occurred while updating the supplier. Please try again.");
            }
        }

        public async Task<Result<bool>> DeleteSupplierAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting supplier ID: {SupplierId}", id);

                var supplier = await _supplierRepository.GetSupplierWithPurchasesAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier deletion failed: Supplier with ID {SupplierId} not found", id);
                    return Result<bool>.NotFound($"Supplier with ID {id} not found.");
                }

                // Check if supplier has purchases
                if (supplier.Purchases.Any())
                {
                    _logger.LogWarning("Supplier deletion failed: Supplier {SupplierName} has {PurchaseCount} purchases",
                        supplier.Name, supplier.Purchases.Count);
                    return Result<bool>.Failure("Cannot delete supplier with existing purchases. Consider deactivating instead.");
                }

                await _supplierRepository.DeleteAsync(supplier);

                _logger.LogInformation("Supplier deleted successfully: {SupplierName} (ID: {SupplierId})",
                    supplier.Name, supplier.SupplierId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier ID: {SupplierId}", id);
                return Result<bool>.Failure("An unexpected error occurred while deleting the supplier. Please try again.");
            }
        }

        public async Task<Result<SupplierDto>> GetSupplierByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting supplier by ID: {SupplierId}", id);

                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier not found with ID: {SupplierId}", id);
                    return Result<SupplierDto>.NotFound($"Supplier with ID {id} not found.");
                }

                var supplierDto = MapToDto(supplier);
                return Result<SupplierDto>.Success(supplierDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier by ID: {SupplierId}", id);
                return Result<SupplierDto>.Failure("An unexpected error occurred while retrieving the supplier. Please try again.");
            }
        }

        public async Task<Result<SupplierDetailDto>> GetSupplierWithPurchasesAsync(int id)
        {
            try
            {
                _logger.LogDebug("Getting supplier with purchases by ID: {SupplierId}", id);

                var supplier = await _supplierRepository.GetSupplierWithPurchasesAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier not found with ID: {SupplierId}", id);
                    return Result<SupplierDetailDto>.NotFound($"Supplier with ID {id} not found.");
                }

                var supplierDetailDto = MapToDetailDto(supplier);
                return Result<SupplierDetailDto>.Success(supplierDetailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier with purchases by ID: {SupplierId}", id);
                return Result<SupplierDetailDto>.Failure("An unexpected error occurred while retrieving the supplier details. Please try again.");
            }
        }

        public async Task<Result<IEnumerable<SupplierDto>>> GetAllSuppliersAsync()
        {
            try
            {
                _logger.LogDebug("Getting all suppliers");

                var suppliers = await _supplierRepository.GetAllAsync();
                var supplierDtos = suppliers.Select(MapToDto).ToList();

                _logger.LogDebug("Retrieved {Count} suppliers", supplierDtos.Count);
                return Result<IEnumerable<SupplierDto>>.Success(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all suppliers");
                return Result<IEnumerable<SupplierDto>>.Failure("An unexpected error occurred while retrieving suppliers. Please try again.");
            }
        }

        public async Task<Result<IEnumerable<SupplierDto>>> GetActiveSuppliersAsync()
        {
            try
            {
                _logger.LogDebug("Getting active suppliers");

                var suppliers = await _supplierRepository.GetActiveSuppliersAsync();
                var supplierDtos = suppliers.Select(MapToDto).ToList();

                _logger.LogDebug("Retrieved {Count} active suppliers", supplierDtos.Count);
                return Result<IEnumerable<SupplierDto>>.Success(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active suppliers");
                return Result<IEnumerable<SupplierDto>>.Failure("An unexpected error occurred while retrieving active suppliers. Please try again.");
            }
        }

        public async Task<Result<IEnumerable<SupplierDto>>> SearchSuppliersAsync(string searchTerm)
        {
            try
            {
                _logger.LogDebug("Searching suppliers with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllSuppliersAsync();

                var suppliers = await _supplierRepository.SearchSuppliersAsync(searchTerm);
                var supplierDtos = suppliers.Select(MapToDto).ToList();

                _logger.LogDebug("Found {Count} suppliers matching search term: {SearchTerm}", supplierDtos.Count, searchTerm);
                return Result<IEnumerable<SupplierDto>>.Success(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching suppliers with term: {SearchTerm}", searchTerm);
                return Result<IEnumerable<SupplierDto>>.Failure("An unexpected error occurred while searching suppliers. Please try again.");
            }
        }

        public async Task<Result<bool>> ToggleSupplierStatusAsync(int id)
        {
            try
            {
                _logger.LogInformation("Toggling supplier status for ID: {SupplierId}", id);

                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier status toggle failed: Supplier with ID {SupplierId} not found", id);
                    return Result<bool>.NotFound($"Supplier with ID {id} not found.");
                }

                var oldStatus = supplier.IsActive;
                supplier.ToggleStatus();
                await _supplierRepository.UpdateAsync(supplier);

                _logger.LogInformation("Supplier status changed from {OldStatus} to {NewStatus} for {SupplierName} (ID: {SupplierId})",
                    oldStatus ? "Active" : "Inactive",
                    supplier.IsActive ? "Active" : "Inactive",
                    supplier.Name,
                    supplier.SupplierId);

                return Result<bool>.Success(supplier.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling supplier status for ID: {SupplierId}", id);
                return Result<bool>.Failure("An unexpected error occurred while toggling supplier status. Please try again.");
            }
        }

        public async Task<Result<SupplierStatsResponse>> GetSupplierStatsAsync(int supplierId)
        {
            try
            {
                _logger.LogDebug("Getting supplier stats for ID: {SupplierId}", supplierId);

                var supplier = await _supplierRepository.GetSupplierWithPurchasesAsync(supplierId);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier stats failed: Supplier with ID {SupplierId} not found", supplierId);
                    return Result<SupplierStatsResponse>.NotFound($"Supplier with ID {supplierId} not found.");
                }

                var receivedPurchases = supplier.Purchases.Where(p => p.Status == "Received").ToList();
                var totalPurchases = supplier.Purchases.Count;
                var totalAmount = receivedPurchases.Sum(p => p.TotalAmount);
                var completedPurchases = receivedPurchases.Count;

                var stats = new SupplierStatsResponse
                {
                    TotalPurchases = totalPurchases,
                    TotalPurchaseAmount = totalAmount,
                    PendingPurchases = supplier.Purchases.Count(p => p.Status == "Pending"),
                    LastPurchaseDate = supplier.Purchases
                        .OrderByDescending(p => p.PurchaseDate)
                        .FirstOrDefault()?.PurchaseDate,
                    AveragePurchaseAmount = completedPurchases > 0 ? totalAmount / completedPurchases : 0,
                    CompletedPurchases = completedPurchases
                };

                _logger.LogDebug("Retrieved stats for supplier {SupplierName}: {TotalPurchases} purchases, ${TotalAmount} total",
                    supplier.Name, stats.TotalPurchases, stats.TotalPurchaseAmount);

                return Result<SupplierStatsResponse>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier stats for ID: {SupplierId}", supplierId);
                return Result<SupplierStatsResponse>.Failure("An unexpected error occurred while retrieving supplier statistics. Please try again.");
            }
        }

        public async Task<Result<IEnumerable<SupplierDto>>> GetTopSuppliersAsync(int count = 10)
        {
            try
            {
                _logger.LogDebug("Getting top {Count} suppliers", count);

                var allSuppliers = await _supplierRepository.GetSuppliersWithPurchasesAsync();
                var topSuppliers = allSuppliers
                    .Select(s => new
                    {
                        Supplier = s,
                        TotalAmount = s.Purchases
                            .Where(p => p.Status == "Received")
                            .Sum(p => p.TotalAmount)
                    })
                    .Where(x => x.TotalAmount > 0)
                    .OrderByDescending(x => x.TotalAmount)
                    .Take(count)
                    .Select(x => x.Supplier)
                    .ToList();

                var supplierDtos = topSuppliers.Select(MapToDto).ToList();
                return Result<IEnumerable<SupplierDto>>.Success(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top suppliers");
                return Result<IEnumerable<SupplierDto>>.Failure("An unexpected error occurred while retrieving top suppliers. Please try again.");
            }
        }

        public async Task<Result<IEnumerable<SupplierDto>>> GetSuppliersWithRecentPurchasesAsync(int days = 30)
        {
            try
            {
                _logger.LogDebug("Getting suppliers with purchases in last {Days} days", days);

                var cutoffDate = DateTime.Now.AddDays(-days);
                var allSuppliers = await _supplierRepository.GetSuppliersWithPurchasesAsync();

                var recentSuppliers = allSuppliers
                    .Where(s => s.Purchases.Any(p => p.PurchaseDate >= cutoffDate))
                    .ToList();

                var supplierDtos = recentSuppliers.Select(MapToDto).ToList();
                return Result<IEnumerable<SupplierDto>>.Success(supplierDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting suppliers with recent purchases");
                return Result<IEnumerable<SupplierDto>>.Failure("An unexpected error occurred while retrieving suppliers with recent purchases. Please try again.");
            }
        }

        // Private mapping methods
        private SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                ContactPerson = supplier.ContactPerson,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        private SupplierDetailDto MapToDetailDto(Supplier supplier)
        {
            var receivedPurchases = supplier.Purchases.Where(p => p.Status == "Received").ToList();
            var totalPurchases = supplier.Purchases.Count;
            var totalAmount = receivedPurchases.Sum(p => p.TotalAmount);

            return new SupplierDetailDto
            {
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                ContactPerson = supplier.ContactPerson,
                Notes = supplier.Notes,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt,
                TotalPurchases = totalPurchases,
                TotalPurchaseAmount = totalAmount,
                PendingPurchases = supplier.Purchases.Count(p => p.Status == "Pending"),
                LastPurchaseDate = supplier.Purchases
                    .OrderByDescending(p => p.PurchaseDate)
                    .FirstOrDefault()?.PurchaseDate
            };
        }
    }
}