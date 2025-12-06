import { Customer } from '../types';

// 修改存储逻辑，支持传入 userId，实现不同用户数据隔离
export const getStoredCustomers = (userId: string): Customer[] => {
  if (!userId) return [];
  try {
    const key = `crm_data_${userId}`;
    const data = localStorage.getItem(key);
    return data ? JSON.parse(data) : [];
  } catch (error) {
    console.error('Failed to load customers from storage', error);
    return [];
  }
};

export const saveStoredCustomers = (userId: string, customers: Customer[]): void => {
  if (!userId) return;
  try {
    const key = `crm_data_${userId}`;
    localStorage.setItem(key, JSON.stringify(customers));
  } catch (error) {
    console.error('Failed to save customers to storage', error);
  }
};