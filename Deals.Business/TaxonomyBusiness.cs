using Deals.Business.Interface;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using ExcelDataReader;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Deals.Business
{
    public class TaxonomyBusiness : ITaxonomyBusiness
    {
        private readonly ITaxonomyRepository _taxonomyRepository;
        private readonly ILogger<TaxonomyBusiness> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public TaxonomyBusiness(ITaxonomyRepository taxonomyRepository,
            ILogger<TaxonomyBusiness> logger)
        {
            _taxonomyRepository = taxonomyRepository;
            _logger = logger;
        }
        //ro we can remove below code (read excel file 
        public async Task<bool> BulkUploadFile(IFormFile formFile, Guid createdBy)
        {
            bool isRes = false;
            List<DataDump> lstDump = new List<DataDump>();
            if (formFile?.Length > 0)
            {

                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data

                    Stream stream = new MemoryStream(fileBytes);

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    int i = 0;
                    while (reader.Read())
                    {
                        if (i != 0)
                        {
                            string projCode = reader[0].ToString();
                            if (!string.IsNullOrEmpty(projCode))
                            {
                                DataDump data = ConstructModal(isRes, reader);
                                lstDump.Add(data);

                            }
                        }
                        i++;
                    }
                }
            }
            var records = await _taxonomyRepository.AddDumpData(lstDump).ConfigureAwait(false);

            return records > 0;
        }

        private DataDump ConstructModal(bool isRes, IExcelDataReader reader)
        {



            return new DataDump()
            {
                ProjectCode = reader[0].ToString(),
                TaskCode = reader[1].ToString(),
                ClientName = reader[2].ToString(),
                ClienteMail = reader[3].ToString(),
                ProjectPartner = reader[4].ToString(),
                TaskManager = reader[5].ToString(),
                HoursBooked = reader[6].ToString(),
                BillingAmount = reader[7].ToString(),
                Sbu = reader[8].ToString(),
                LegalEntity = reader[9].ToString(),
                ProjectName = reader[10].ToString(),
                EngagementType = reader[11].ToString(),
                TargetEntityName = reader[12].ToString(),
                BusinessDescription = reader[13].ToString(),
                ShortDescription = reader[14].ToString(),
                CompletedOn = reader[15].ToString(),
                WebsiteUrl = reader[16].ToString(),
                QuotedinAnnouncements = reader[17].ToString(),
                NatureofEngagement = reader[18].ToString(),
                DealType = reader[19].ToString(),
                DealValue = reader[20].ToString(),
                SubSector = reader[21].ToString(),
                Services = reader[22].ToString(),
                TransactionStatus = reader[23].ToString(),
                ClientEntityType = reader[24].ToString(),
                DomicileCountryRegion = reader[25].ToString(),
                WorkCountryRegion = reader[26].ToString(),
                EntityNameDisclosed = reader[27].ToString(),
                TargetEntityType = reader[28].ToString(),
            };

        }


        public async Task<IList<TaxonomyDTO>> GetTaxonomy()
        {
            LoggingHelper.Log(_logger, LogLevel.Information, "TaxonomyDetailsBusiness Get All Taxonomy");

            var records = await _taxonomyRepository.GetTaxonomyComposite().ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"TaxonomyDetailsBusiness Get All TaxonomyDetails records count:{records.Count()}");
                return records;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"TaxonomyDetailsBusiness Get All no records found");

                return null;
            }
        }

        public async Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds)
        {
            return await _taxonomyRepository.GetTaxonomyByCategory(categoryIds).ConfigureAwait(false);
        }
    }

}
