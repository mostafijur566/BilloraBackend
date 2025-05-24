using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Helper;
using api.Models;

namespace api.Interface
{
    public interface IQuotationRepository
    {
        Task<String> GenerateQuotationNumberAsync();
        Task<Quotation> CreateQuotationWithItemsAsync(Quotation quotationModel, List<CreateQuotationItemDto> itemDtos);
        Task<List<Quotation>?> GetAllQuotationAsync(int companyId, QuotationQueryObject query);
        Task<Quotation?> GetQuotationByIdAsync(int quotationId, int companyId);
        Task<Quotation?> UpdateQuotationWithItemsAsync(int quotationId, int companyId, UpdateQuotationDto quotationDto);
        Task<Quotation?> DeleteQuotationAsync(int quotationId, int companyId);
    }
}