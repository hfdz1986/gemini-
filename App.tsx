import React, { useState, useEffect, useMemo } from 'react';
import { 
  Users, 
  Plus, 
  Search, 
  MoreVertical, 
  Edit2, 
  Trash2, 
  Phone, 
  Mail, 
  Building2,
  FileText,
  LogOut,
  LayoutDashboard
} from 'lucide-react';
import { Customer, CustomerFormData, CustomerStatus } from './types';
import { getStoredCustomers, saveStoredCustomers } from './services/storageService';
import Modal from './components/Modal';
import CustomerForm from './components/CustomerForm';
import LoginPage from './components/LoginPage';

function App() {
  // Auth State
  const [currentUser, setCurrentUser] = useState<string | null>(() => {
    return localStorage.getItem('crm_current_user');
  });

  // App Data State
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState<Customer | undefined>(undefined);
  const [searchTerm, setSearchTerm] = useState('');
  const [showMenuId, setShowMenuId] = useState<string | null>(null);

  // Load initial data when user changes
  useEffect(() => {
    if (currentUser) {
      const stored = getStoredCustomers(currentUser);
      setCustomers(stored);
    } else {
      setCustomers([]);
    }
  }, [currentUser]);

  // Persist data on change
  useEffect(() => {
    if (currentUser) {
      saveStoredCustomers(currentUser, customers);
    }
  }, [customers, currentUser]);

  // Handle Login
  const handleLogin = (username: string) => {
    localStorage.setItem('crm_current_user', username);
    setCurrentUser(username);
  };

  // Handle Logout
  const handleLogout = () => {
    if (window.confirm('确定要退出登录吗？')) {
      localStorage.removeItem('crm_current_user');
      setCurrentUser(null);
      setCustomers([]);
    }
  };

  // Handle Create/Update
  const handleSaveCustomer = (formData: CustomerFormData) => {
    if (editingCustomer) {
      // Update
      setCustomers(prev => prev.map(c => 
        c.id === editingCustomer.id 
          ? { ...c, ...formData } 
          : c
      ));
    } else {
      // Create
      const newCustomer: Customer = {
        id: crypto.randomUUID(),
        createdAt: new Date().toISOString(),
        ...formData
      };
      setCustomers(prev => [newCustomer, ...prev]);
    }
    handleCloseModal();
  };

  // Handle Delete
  const handleDeleteCustomer = (id: string) => {
    if (window.confirm('确定要删除这位客户吗？此操作无法撤销。')) {
      setCustomers(prev => prev.filter(c => c.id !== id));
    }
    setShowMenuId(null);
  };

  const handleEditClick = (customer: Customer) => {
    setEditingCustomer(customer);
    setIsModalOpen(true);
    setShowMenuId(null);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingCustomer(undefined);
  };

  // Filter Logic
  const filteredCustomers = useMemo(() => {
    if (!searchTerm) return customers;
    const lowerTerm = searchTerm.toLowerCase();
    return customers.filter(c => 
      c.name.toLowerCase().includes(lowerTerm) ||
      c.company.toLowerCase().includes(lowerTerm) ||
      c.email.toLowerCase().includes(lowerTerm)
    );
  }, [customers, searchTerm]);

  // Status Badge Helper
  const getStatusColor = (status: CustomerStatus) => {
    switch (status) {
      case CustomerStatus.Active: return 'bg-green-100 text-green-700 ring-green-600/20';
      case CustomerStatus.Inactive: return 'bg-slate-100 text-slate-700 ring-slate-600/20';
      case CustomerStatus.Lead: return 'bg-blue-100 text-blue-700 ring-blue-600/20';
      default: return 'bg-slate-100 text-slate-700';
    }
  };

  const getStatusLabel = (status: CustomerStatus) => {
    switch (status) {
      case CustomerStatus.Active: return '活跃';
      case CustomerStatus.Inactive: return '停用';
      case CustomerStatus.Lead: return '潜在';
      default: return status;
    }
  };

  // Render Login Page if not logged in
  if (!currentUser) {
    return <LoginPage onLogin={handleLogin} />;
  }

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col">
      {/* Header */}
      <header className="bg-white border-b border-slate-200 sticky top-0 z-20 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <div className="flex items-center gap-2.5">
            <div className="bg-indigo-600 p-2 rounded-lg shadow-sm shadow-indigo-200">
              <LayoutDashboard className="text-white" size={20} />
            </div>
            <div>
              <h1 className="text-lg font-bold text-slate-800 leading-tight">客户管理系统</h1>
              <div className="text-xs text-slate-500 font-medium">当前用户: {currentUser}</div>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <button 
              onClick={() => setIsModalOpen(true)}
              className="flex items-center gap-2 bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg font-medium transition-all shadow-sm shadow-indigo-200 active:scale-95 text-sm"
            >
              <Plus size={18} />
              <span className="hidden sm:inline">新建客户</span>
            </button>
            <div className="h-6 w-px bg-slate-200 mx-1"></div>
            <button 
              onClick={handleLogout}
              className="p-2 text-slate-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
              title="退出登录"
            >
              <LogOut size={20} />
            </button>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
        
        {/* Stats Row */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white p-6 rounded-xl border border-slate-100 shadow-[0_2px_10px_-4px_rgba(6,81,237,0.1)] hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-2">
              <div className="text-sm font-medium text-slate-500">总客户数</div>
              <div className="p-2 bg-indigo-50 rounded-lg text-indigo-600">
                <Users size={18} />
              </div>
            </div>
            <div className="text-3xl font-bold text-slate-800">{customers.length}</div>
          </div>
          <div className="bg-white p-6 rounded-xl border border-slate-100 shadow-[0_2px_10px_-4px_rgba(6,81,237,0.1)] hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-2">
              <div className="text-sm font-medium text-slate-500">活跃客户</div>
              <div className="p-2 bg-green-50 rounded-lg text-green-600">
                <Building2 size={18} />
              </div>
            </div>
            <div className="text-3xl font-bold text-slate-800">
              {customers.filter(c => c.status === CustomerStatus.Active).length}
            </div>
          </div>
          <div className="bg-white p-6 rounded-xl border border-slate-100 shadow-[0_2px_10px_-4px_rgba(6,81,237,0.1)] hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-2">
              <div className="text-sm font-medium text-slate-500">潜在机会</div>
              <div className="p-2 bg-blue-50 rounded-lg text-blue-600">
                <Phone size={18} />
              </div>
            </div>
            <div className="text-3xl font-bold text-slate-800">
              {customers.filter(c => c.status === CustomerStatus.Lead).length}
            </div>
          </div>
        </div>

        {/* Search & List */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Toolbar */}
          <div className="p-4 border-b border-slate-100 flex items-center justify-between bg-white gap-4">
            <div className="relative max-w-md w-full">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
              <input 
                type="text" 
                placeholder="搜索姓名、公司或邮箱..." 
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2.5 border border-slate-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 transition-all text-sm"
              />
            </div>
          </div>

          {/* Table */}
          <div className="overflow-x-auto min-h-[400px]">
            {filteredCustomers.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-20 text-slate-400">
                <div className="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mb-4">
                  <Users size={32} className="text-slate-300" />
                </div>
                <p className="text-lg font-medium text-slate-600">暂无数据</p>
                <p className="text-sm mt-1">点击右上角添加客户开始使用</p>
              </div>
            ) : (
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="bg-slate-50/80 border-b border-slate-200 text-xs font-semibold text-slate-500 uppercase tracking-wider">
                    <th className="px-6 py-4">客户信息</th>
                    <th className="px-6 py-4">联系方式</th>
                    <th className="px-6 py-4">状态</th>
                    <th className="px-6 py-4 text-right">操作</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {filteredCustomers.map((customer) => (
                    <tr key={customer.id} className="hover:bg-slate-50/80 transition-colors group">
                      <td className="px-6 py-4">
                        <div className="flex items-start gap-3">
                          <div className="w-10 h-10 rounded-full bg-indigo-100 text-indigo-600 flex items-center justify-center font-bold text-sm shrink-0 ring-4 ring-white shadow-sm">
                            {customer.name.charAt(0)}
                          </div>
                          <div>
                            <div className="font-semibold text-slate-800">{customer.name}</div>
                            <div className="flex items-center gap-1.5 text-xs text-slate-500 mt-0.5">
                              <Building2 size={12} />
                              {customer.company}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex flex-col gap-1.5 text-sm text-slate-600">
                          <div className="flex items-center gap-2 group/link hover:text-indigo-600 transition-colors cursor-pointer">
                            <Mail size={14} className="text-slate-400 group-hover/link:text-indigo-500" />
                            {customer.email || '未填写'}
                          </div>
                          <div className="flex items-center gap-2">
                            <Phone size={14} className="text-slate-400" />
                            {customer.phone || '未填写'}
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        <span className={`inline-flex items-center px-2.5 py-1 rounded-md text-xs font-medium ring-1 ring-inset ${getStatusColor(customer.status)}`}>
                          {getStatusLabel(customer.status)}
                        </span>
                        {customer.notes && (
                           <div className="mt-2 text-xs text-slate-400 flex items-center gap-1 truncate max-w-[150px]" title={customer.notes}>
                             <FileText size={12} />
                             {customer.notes}
                           </div>
                        )}
                      </td>
                      <td className="px-6 py-4 text-right relative">
                        <div className="relative inline-block text-left">
                          <button 
                            onClick={() => setShowMenuId(showMenuId === customer.id ? null : customer.id)}
                            className="p-2 text-slate-400 hover:text-slate-600 rounded-full hover:bg-slate-100 transition-colors"
                          >
                            <MoreVertical size={18} />
                          </button>
                          
                          {showMenuId === customer.id && (
                            <>
                              <div 
                                className="fixed inset-0 z-10" 
                                onClick={() => setShowMenuId(null)}
                              ></div>
                              <div className="absolute right-0 mt-2 w-32 origin-top-right rounded-lg bg-white shadow-lg ring-1 ring-black ring-opacity-5 z-20 overflow-hidden py-1 border border-slate-100">
                                <button
                                  onClick={() => handleEditClick(customer)}
                                  className="flex w-full items-center px-4 py-2.5 text-sm text-slate-700 hover:bg-slate-50 transition-colors"
                                >
                                  <Edit2 size={14} className="mr-2.5 text-indigo-600" />
                                  编辑
                                </button>
                                <button
                                  onClick={() => handleDeleteCustomer(customer.id)}
                                  className="flex w-full items-center px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 transition-colors"
                                >
                                  <Trash2 size={14} className="mr-2.5" />
                                  删除
                                </button>
                              </div>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer className="py-6 text-center text-slate-400 text-sm">
        &copy; {new Date().getFullYear()} Intelligent CRM System. All rights reserved.
      </footer>

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        title={editingCustomer ? "编辑客户" : "添加新客户"}
      >
        <CustomerForm 
          initialData={editingCustomer} 
          onSubmit={handleSaveCustomer} 
          onCancel={handleCloseModal}
        />
      </Modal>
    </div>
  );
}

export default App;