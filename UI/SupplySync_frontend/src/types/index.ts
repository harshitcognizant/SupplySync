export interface AuthUser {
  token: string;
  userId: string;
  fullName: string;
  email: string;
  role: string;
  vendorId?: number;
}

export interface Vendor {
  id: number;
  vendorCode: string;
  companyName: string;
  contactEmail: string;
  contactPhone: string;
  address: string;
  taxNumber: string;
  licenseNumber: string;
  documentPath: string;
  status: string;
  rejectionReason?: string;
  createdAt: string;
}

export interface Contract {
  id: number;
  contractNumber: string;
  vendorId: number;
  vendorName: string;
  startDate: string;
  endDate: string;
  paymentTerms: string;
  deliveryTerms: string;
  itemPricing: string;
  status: string;
  createdAt: string;
}

export interface POItem {
  id: number;
  itemName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface PurchaseOrder {
  id: number;
  poNumber: string;
  vendorId: number;
  vendorName: string;
  contractId: number;
  contractNumber: string;
  expectedDeliveryDate: string;
  status: string;
  createdAt: string;
  items: POItem[];
}

export interface GoodsReceiptItem {
  itemName: string;
  receivedQuantity: number;
  condition: string;
}

export interface GoodsReceipt {
  id: number;
  grNumber: string;
  purchaseOrderId: number;  // ← must match exactly what API returns
  poNumber: string;
  receivedDate: string;
  remarks: string;
  status: string;
  items: GoodsReceiptItem[];
}

export interface InventoryItem {
  id: number;
  itemName: string;
  sku: string;
  quantityInStock: number;
  unit: string;
  lastUpdated: string;
}

export interface ItemIssue {
  id: number;
  itemName: string;
  quantityIssued: number;
  issuedTo: string;
  issueDate: string;
  remarks: string;
}

export interface Invoice {
  id: number;
  invoiceNumber: string;
  vendorId: number;
  vendorName: string;
  purchaseOrderId: number;
  poNumber: string;
  goodsReceiptId: number;
  totalAmount: number;
  status: string;
  rejectionReason?: string;
  submittedAt: string;
isPaid: boolean;
  paymentReference?: string;
  paymentDate?: string;
  
}

export interface ComplianceCheck {
  id: number;
  entityType: string;
  entityId: number;
  status: string;
  remarks: string;
  checkedAt: string;
}

export interface Notification {
  id: number;
  message: string;
  isRead: boolean;
  type: string;
  createdAt: string;
}

export interface AppUser {
  id: string;
  fullName: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  roles: string[];
}