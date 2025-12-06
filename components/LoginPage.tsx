import React, { useState } from 'react';
import { LayoutDashboard, ArrowRight, CheckCircle2 } from 'lucide-react';

interface LoginPageProps {
  onLogin: (username: string) => void;
}

const LoginPage: React.FC<LoginPageProps> = ({ onLogin }) => {
  const [username, setUsername] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!username.trim()) return;
    
    setLoading(true);
    // 模拟登录延迟，增加真实感
    setTimeout(() => {
      onLogin(username);
      setLoading(false);
    }, 800);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50">
      <div className="max-w-md w-full mx-4">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-xl bg-indigo-600 text-white mb-4 shadow-lg shadow-indigo-200">
            <LayoutDashboard size={32} />
          </div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">智能客户管理系统</h1>
          <p className="text-slate-500 mt-2">为您的企业打造的 AI 驱动 CRM</p>
        </div>

        <div className="bg-white rounded-2xl shadow-xl border border-slate-100 overflow-hidden">
          <div className="p-8">
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label htmlFor="username" className="block text-sm font-medium text-slate-700 mb-2">
                  账号 / 企业ID
                </label>
                <input
                  id="username"
                  type="text"
                  required
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
                  placeholder="请输入您的账号 (例如: demo)"
                />
              </div>

              <div className="space-y-4">
                <button
                  type="submit"
                  disabled={loading}
                  className="w-full flex items-center justify-center gap-2 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold py-3 px-4 rounded-xl transition-all shadow-md shadow-indigo-200 active:scale-95 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {loading ? (
                    '登录中...'
                  ) : (
                    <>
                      进入系统 <ArrowRight size={18} />
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
          
          <div className="bg-slate-50 px-8 py-6 border-t border-slate-100">
            <h4 className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-3">系统亮点</h4>
            <div className="space-y-2">
              <div className="flex items-center gap-2 text-sm text-slate-600">
                <CheckCircle2 size={16} className="text-green-500" />
                <span>AI 智能生成客户画像</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-slate-600">
                <CheckCircle2 size={16} className="text-green-500" />
                <span>本地数据安全存储</span>
              </div>
              <div className="flex items-center gap-2 text-sm text-slate-600">
                <CheckCircle2 size={16} className="text-green-500" />
                <span>极速响应，无需部署</span>
              </div>
            </div>
          </div>
        </div>

        <p className="text-center text-slate-400 text-sm mt-8">
          &copy; {new Date().getFullYear()} CRM System. All rights reserved.
        </p>
      </div>
    </div>
  );
};

export default LoginPage;