import React, { useState, useEffect } from 'react';
import { Sparkles, Loader2 } from 'lucide-react';
import { CustomerFormData, CustomerStatus } from '../types';
import { generateFakeCustomerProfile } from '../services/geminiService';

interface CustomerFormProps {
  initialData?: CustomerFormData;
  onSubmit: (data: CustomerFormData) => void;
  onCancel: () => void;
}

const emptyForm: CustomerFormData = {
  name: '',
  email: '',
  phone: '',
  company: '',
  status: CustomerStatus.Lead,
  notes: ''
};

const CustomerForm: React.FC<CustomerFormProps> = ({ initialData, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState<CustomerFormData>(emptyForm);
  const [isGenerating, setIsGenerating] = useState(false);

  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
    } else {
      setFormData(emptyForm);
    }
  }, [initialData]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const handleAiGenerate = async () => {
    setIsGenerating(true);
    try {
      const aiData = await generateFakeCustomerProfile();
      setFormData(aiData);
    } catch (error) {
      alert("AI 生成失败，请检查 API Key 配置或稍后重试。");
    } finally {
      setIsGenerating(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {!initialData && (
        <div className="mb-6 p-4 bg-indigo-50 border border-indigo-100 rounded-lg flex items-center justify-between">
          <div className="text-sm text-indigo-700">
            <span className="font-semibold">AI 助手:</span> 自动生成一个虚拟客户资料以供测试。
          </div>
          <button
            type="button"
            onClick={handleAiGenerate}
            disabled={isGenerating}
            className="flex items-center gap-2 px-3 py-1.5 bg-indigo-600 text-white text-xs font-medium rounded-md hover:bg-indigo-700 transition-colors disabled:opacity-50"
          >
            {isGenerating ? <Loader2 size={14} className="animate-spin" /> : <Sparkles size={14} />}
            一键生成
          </button>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-slate-700 mb-1">姓名</label>
          <input
            required
            name="name"
            value={formData.name}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            placeholder="请输入姓名"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700 mb-1">公司</label>
          <input
            required
            name="company"
            value={formData.company}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            placeholder="公司名称"
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-slate-700 mb-1">邮箱</label>
          <input
            type="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            placeholder="example@company.com"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700 mb-1">电话</label>
          <input
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
            placeholder="联系电话"
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">状态</label>
        <select
          name="status"
          value={formData.status}
          onChange={handleChange}
          className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all bg-white"
        >
          <option value={CustomerStatus.Lead}>潜在客户 (Lead)</option>
          <option value={CustomerStatus.Active}>活跃客户 (Active)</option>
          <option value={CustomerStatus.Inactive}>非活跃客户 (Inactive)</option>
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1">备注</label>
        <textarea
          name="notes"
          value={formData.notes}
          onChange={handleChange}
          rows={3}
          className="w-full px-3 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all resize-none"
          placeholder="关于客户的备注信息..."
        />
      </div>

      <div className="flex justify-end gap-3 pt-4 border-t border-slate-100 mt-6">
        <button
          type="button"
          onClick={onCancel}
          className="px-4 py-2 text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors font-medium text-sm"
        >
          取消
        </button>
        <button
          type="submit"
          className="px-4 py-2 text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 shadow-sm shadow-indigo-200 transition-colors font-medium text-sm"
        >
          {initialData ? '保存修改' : '创建客户'}
        </button>
      </div>
    </form>
  );
};

export default CustomerForm;
