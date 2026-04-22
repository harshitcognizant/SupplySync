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
			ConfigureAuditLogMappings();
			ConfigureNotificationMappings();
            ConfigureComplianceMappings();
            ConfigureReportMappings();
            ConfigureWarehouseMappings();
            ConfigureInventoryMappings();
            ConfigureReceiptMappings();
            ConfigureVendorApplicationMappings();

            ConfigureInvoiceMappings();
            ConfigurePaymentMappings();
            ConfigureVendorMappings();
            ConfigureContractMappings();


            ConfigurePurchaseOrderMappings();
            ConfigureDeliveryMappings();



        }
    }
}