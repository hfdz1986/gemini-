export enum CustomerStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Lead = 'Lead'
}

export interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  company: string;
  status: CustomerStatus;
  notes: string;
  createdAt: string;
}

export interface CustomerFormData {
  name: string;
  email: string;
  phone: string;
  company: string;
  status: CustomerStatus;
  notes: string;
}
