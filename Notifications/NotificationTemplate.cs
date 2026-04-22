using System;
using System.Collections.Generic;
using System.Linq;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;

namespace SupplySync.Notifications
{
    public sealed record NotificationTemplateDefinition(
        NotificationEvent Event,
        NotificationCategory Category,
        RoleType? FromRole,
        IReadOnlyList<RoleType>? ToRoles,
        Func<IDictionary<string, object>, string> MessageFormatter,
        bool UseRoleTargets = true // if false, expect ToUsers provided by caller
    );

    public static class NotificationTemplates
    {
        // Simple helper to perform token replacement from payload dictionary
        private static string ReplacePlaceholders(string template, IDictionary<string, object> payload)
        {
            if (string.IsNullOrEmpty(template)) return template;
            if (payload == null || payload.Count == 0) return template;

            var result = template;
            foreach (var kv in payload)
            {
                var token = "{" + kv.Key + "}";
                var value = kv.Value?.ToString() ?? string.Empty;
                result = result.Replace(token, value, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        // Central registry of templates requested in the prompt
        public static IReadOnlyDictionary<NotificationEvent, NotificationTemplateDefinition> Registry { get; }
            = new Dictionary<NotificationEvent, NotificationTemplateDefinition>
            {
                // Vendor / application
                [NotificationEvent.VendorApplicationSubmitted] = new NotificationTemplateDefinition(
                    NotificationEvent.VendorApplicationSubmitted,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("New vendor application received: {VendorName}", payload)
                ),

                [NotificationEvent.VendorApplicationApproved] = new NotificationTemplateDefinition(
                    NotificationEvent.VendorApplicationApproved,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: _ => "Your vendor application has been approved."
                ),

                [NotificationEvent.VendorApplicationRejected] = new NotificationTemplateDefinition(
                    NotificationEvent.VendorApplicationRejected,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: payload => ReplacePlaceholders("Your vendor application was rejected. Reason: {Reason}", payload)
                ),

                [NotificationEvent.VendorApprovedInternal] = new NotificationTemplateDefinition(
                    NotificationEvent.VendorApprovedInternal,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.Admin, RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Vendor '{VendorName}' has been approved.", payload)
                ),

                // Admin & user
                [NotificationEvent.UserAccountCreated] = new NotificationTemplateDefinition(
                    NotificationEvent.UserAccountCreated,
                    NotificationCategory.System,
                    FromRole: RoleType.Admin,
                    ToRoles: null, // will target created user id, caller will supply ToUsers
                    MessageFormatter: payload => ReplacePlaceholders("Your account has been created. Role: {Role}", payload),
                    UseRoleTargets: false
                ),

                [NotificationEvent.UserRoleChanged] = new NotificationTemplateDefinition(
                    NotificationEvent.UserRoleChanged,
                    NotificationCategory.System,
                    FromRole: RoleType.Admin,
                    ToRoles: null,
                    MessageFormatter: payload => ReplacePlaceholders("Your role has been updated to {NewRole}", payload),
                    UseRoleTargets: false
                ),

                // Contract & PO
                [NotificationEvent.ContractCreated] = new NotificationTemplateDefinition(
                    NotificationEvent.ContractCreated,
                    NotificationCategory.System,
                    FromRole: RoleType.ProcurementOfficer,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: payload => ReplacePlaceholders("A new contract has been created for you (Start: {StartDate})", payload)
                ),

                [NotificationEvent.PurchaseOrderCreated] = new NotificationTemplateDefinition(
                    NotificationEvent.PurchaseOrderCreated,
                    NotificationCategory.System,
                    FromRole: RoleType.ProcurementOfficer,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: payload => ReplacePlaceholders("New Purchase Order #{PONumber} has been issued.", payload)
                ),

                // Delivery & warehouse
                [NotificationEvent.DeliveryMarkedDone] = new NotificationTemplateDefinition(
                    NotificationEvent.DeliveryMarkedDone,
                    NotificationCategory.System,
                    FromRole: RoleType.VendorUser,
                    ToRoles: new List<RoleType> { RoleType.WarehouseManager },
                    MessageFormatter: payload => ReplacePlaceholders("Delivery done for PO #{PONumber}", payload)
                ),

                [NotificationEvent.GoodsReceivedConfirmation] = new NotificationTemplateDefinition(
                    NotificationEvent.GoodsReceivedConfirmation,
                    NotificationCategory.System,
                    FromRole: RoleType.WarehouseManager,
                    ToRoles: new List<RoleType> { RoleType.VendorUser, RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Goods received for PO #{PONumber}", payload)
                ),

                // Inventory
                [NotificationEvent.InventoryUpdated] = new NotificationTemplateDefinition(
                    NotificationEvent.InventoryUpdated,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.WarehouseManager },
                    MessageFormatter: payload => ReplacePlaceholders("Inventory updated for item: {ItemName}", payload)
                ),

                // Invoice & finance
                [NotificationEvent.InvoiceSubmitted] = new NotificationTemplateDefinition(
                    NotificationEvent.InvoiceSubmitted,
                    NotificationCategory.Payment,
                    FromRole: RoleType.VendorUser,
                    ToRoles: new List<RoleType> { RoleType.FinanceOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("New invoice submitted for PO #{PONumber}", payload)
                ),

                [NotificationEvent.InvoicePendingApprovalReminder] = new NotificationTemplateDefinition(
                    NotificationEvent.InvoicePendingApprovalReminder,
                    NotificationCategory.Payment,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.FinanceOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Invoice #{InvoiceNumber} pending approval.", payload)
                ),

                [NotificationEvent.InvoiceApproved] = new NotificationTemplateDefinition(
                    NotificationEvent.InvoiceApproved,
                    NotificationCategory.Payment,
                    FromRole: RoleType.FinanceOfficer,
                    ToRoles: new List<RoleType> { RoleType.VendorUser, RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Invoice #{InvoiceNumber} has been approved.", payload)
                ),

                [NotificationEvent.InvoiceRejected] = new NotificationTemplateDefinition(
                    NotificationEvent.InvoiceRejected,
                    NotificationCategory.Payment,
                    FromRole: RoleType.FinanceOfficer,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: payload => ReplacePlaceholders("Invoice #{InvoiceNumber} rejected. Reason: {Reason}", payload)
                ),

                [NotificationEvent.PaymentCompleted] = new NotificationTemplateDefinition(
                    NotificationEvent.PaymentCompleted,
                    NotificationCategory.Payment,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.VendorUser },
                    MessageFormatter: payload => ReplacePlaceholders("Payment of {Amount} has been completed.", payload)
                ),

                // Compliance & Audit
                [NotificationEvent.ComplianceReviewStarted] = new NotificationTemplateDefinition(
                    NotificationEvent.ComplianceReviewStarted,
                    NotificationCategory.System,
                    FromRole: null,
                    ToRoles: new List<RoleType> { RoleType.ComplianceOfficer },
                    MessageFormatter: _ => "New compliance review task assigned."
                ),

                [NotificationEvent.CompliancePassed] = new NotificationTemplateDefinition(
                    NotificationEvent.CompliancePassed,
                    NotificationCategory.System,
                    FromRole: RoleType.ComplianceOfficer,
                    ToRoles: new List<RoleType> { RoleType.Admin, RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Compliance review passed for Contract/PO #{RefId}", payload)
                ),

                [NotificationEvent.ComplianceFailed] = new NotificationTemplateDefinition(
                    NotificationEvent.ComplianceFailed,
                    NotificationCategory.System,
                    FromRole: RoleType.ComplianceOfficer,
                    ToRoles: new List<RoleType> { RoleType.Admin, RoleType.ProcurementOfficer },
                    MessageFormatter: payload => ReplacePlaceholders("Compliance review failed for #{RefId}. Action required.", payload)
                )
            };
    }
}