using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Models;

namespace api.Interface
{
    public interface IQuotationRepository
    {
        Task<String> GenerateQuotationNumberAsync();
        Task<Quotation> CreateQuotationAsync(Quotation quotation);
        Task<Quotation> CreateQuotationWithItemsAsync(Quotation quotationModel, List<CreateQuotationItemDto> itemDtos);

        Task<List<Quotation>?> GetAllQuotationAsync(int companyId);
    }
}