using AutoMapper;

namespace SupplySync.Mappers
{
    public partial class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Initialize mappings for each domain/model
            ConfigureAuditMappings();
			ConfigureUserMappings();
			ConfigureRoleMappings();
			ConfigureUserRoleMappings();
			ConfigureComplianceRecordMappings();
			ConfigureReportMappings();
			ConfigureAuditLogMappings();
			ConfigureNotificationMappings();
            ConfigureInvoiceMappings();
            ConfigurePaymentMappings();
            ConfigureVendorMappings();
            ConfigureContractMappings();


            ConfigurePurchaseOrderMappings();
            ConfigureDeliveryMappings();



        }
    }
}