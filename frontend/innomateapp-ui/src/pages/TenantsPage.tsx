import { useState, useEffect } from 'react';
import {
    Plus,
    Search,
    Building,
    Mail,
    User,
    X,
    Shield,
    Loader2,
    RefreshCw
} from 'lucide-react';
import { useTheme } from '@/hooks/useTheme.js';
import MainLayout from '@/components/layout/MainLayout.js';
import {
    useTenants,
    useTenantsLoading,
    useTenantsError,
    useTenantActions
} from '@/store/useTenantStore.js';

export default function TenantsPage() {
    const { isDark } = useTheme();
    const tenants = useTenants();
    const isLoading = useTenantsLoading();
    const error = useTenantsError();
    const { fetchTenants, onboardTenant, clearError } = useTenantActions();

    const [searchQuery, setSearchQuery] = useState('');
    const [isCreating, setIsCreating] = useState(false);
    const [formData, setFormData] = useState({
        tenantName: '',
        tenantCode: '',
        adminUsername: '',
        adminEmail: '',
        adminPassword: ''
    });

    const theme = {
        bg: isDark ? 'bg-gray-900' : 'bg-gray-50',
        bgCard: isDark ? 'bg-gray-800' : 'bg-white',
        bgSecondary: isDark ? 'bg-gray-700' : 'bg-gray-50',
        text: isDark ? 'text-gray-100' : 'text-gray-900',
        textSecondary: isDark ? 'text-gray-400' : 'text-gray-600',
        border: isDark ? 'border-gray-700' : 'border-gray-200',
        input: isDark
            ? 'bg-gray-700 border-gray-600 text-gray-100 placeholder-gray-400 focus:border-blue-500'
            : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-blue-500',
        buttonPrimary: 'bg-blue-600 hover:bg-blue-700 text-white disabled:bg-blue-800 disabled:opacity-50',
        buttonSecondary: isDark
            ? 'bg-gray-700 hover:bg-gray-600 text-gray-100'
            : 'bg-gray-200 hover:bg-gray-300 text-gray-700',
    };

    useEffect(() => {
        fetchTenants();
    }, [fetchTenants]);

    const handleOnboard = async () => {
        try {
            await onboardTenant(formData);
            setIsCreating(false);
            setFormData({
                tenantName: '',
                tenantCode: '',
                adminUsername: '',
                adminEmail: '',
                adminPassword: ''
            });
        } catch (err) {
            // Error handled by store
        }
    };

    const filteredTenants = tenants.filter(t =>
        t.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        t.code.toLowerCase().includes(searchQuery.toLowerCase()) ||
        t.adminUsername.toLowerCase().includes(searchQuery.toLowerCase())
    );

    return (
        <MainLayout>
            <div className={`h-full flex flex-col p-6 ${theme.bg}`}>
                <div className="flex justify-between items-center mb-6">
                    <div>
                        <h2 className={`text-xl font-bold ${theme.text}`}>Tenant Management</h2>
                        <p className={`text-sm mt-1 ${theme.textSecondary}`}>
                            Onboard and manage business tenants.
                        </p>
                    </div>
                    <button
                        onClick={() => setIsCreating(true)}
                        className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${theme.buttonPrimary}`}
                    >
                        <Plus size={20} />
                        Onboard New Tenant
                    </button>
                </div>

                {error && (
                    <div className="mb-4 p-4 rounded-lg bg-red-900/20 border border-red-800 text-red-200 flex justify-between items-center">
                        <span>{error}</span>
                        <button onClick={clearError}><X size={16} /></button>
                    </div>
                )}

                {/* Search */}
                <div className={`p-4 rounded-lg border mb-6 ${theme.bgCard} ${theme.border}`}>
                    <div className="relative">
                        <Search className={`absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 ${theme.textSecondary}`} />
                        <input
                            type="text"
                            placeholder="Search tenants..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className={`w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none ${theme.input}`}
                        />
                    </div>
                </div>

                {/* Tenants List */}
                <div className={`rounded-lg border shadow-sm flex-1 overflow-hidden flex flex-col ${theme.bgCard} ${theme.border}`}>
                    <div className={`grid grid-cols-4 gap-4 px-6 py-3 border-b text-sm font-semibold ${theme.bgSecondary} ${theme.border} ${theme.text}`}>
                        <div>TENANT NAME</div>
                        <div>CODE</div>
                        <div>ADMIN USER</div>
                        <div className="text-right">ACTIONS</div>
                    </div>

                    <div className="flex-1 overflow-y-auto">
                        {isLoading && tenants.length === 0 ? (
                            <div className="flex items-center justify-center p-12">
                                <Loader2 className="animate-spin text-blue-500" size={32} />
                            </div>
                        ) : filteredTenants.length === 0 ? (
                            <div className="flex flex-col items-center justify-center p-12 text-gray-500">
                                <Building size={48} className="mb-2 opacity-20" />
                                <p>No tenants found</p>
                            </div>
                        ) : (
                            filteredTenants.map((tenant) => (
                                <div
                                    key={tenant.tenantId}
                                    className={`grid grid-cols-4 gap-4 px-6 py-4 border-b ${theme.border} hover:bg-gray-50/5 transition-colors`}
                                >
                                    <div className={`font-medium ${theme.text}`}>{tenant.name}</div>
                                    <div className="text-blue-500 font-mono">{tenant.code}</div>
                                    <div className="flex items-center gap-2">
                                        <User size={14} className={theme.textSecondary} />
                                        <span className={theme.text}>{tenant.adminUsername}</span>
                                    </div>
                                    <div className="text-right">
                                        <button className="text-blue-500 hover:text-blue-400 text-sm font-medium">Details</button>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>

                {/* Onboarding Modal */}
                {isCreating && (
                    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
                        <div className={`rounded-xl shadow-2xl w-full max-w-lg overflow-hidden ${theme.bgCard}`}>
                            <div className={`p-6 border-b ${theme.border} flex justify-between items-center`}>
                                <h3 className={`text-lg font-bold ${theme.text}`}>Onboard New Tenant</h3>
                                <button onClick={() => setIsCreating(false)} className={theme.textSecondary}><X size={20} /></button>
                            </div>

                            <div className="p-6 space-y-4">
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <label className={`block text-xs font-bold uppercase mb-2 ${theme.textSecondary}`}>Business Name</label>
                                        <input
                                            type="text"
                                            placeholder="e.g. Acme Corp"
                                            value={formData.tenantName}
                                            onChange={e => setFormData({ ...formData, tenantName: e.target.value })}
                                            className={`w-full p-2 border rounded-lg ${theme.input}`}
                                        />
                                    </div>
                                    <div>
                                        <label className={`block text-xs font-bold uppercase mb-2 ${theme.textSecondary}`}>Tenant Code</label>
                                        <input
                                            type="text"
                                            placeholder="e.g. ACME"
                                            value={formData.tenantCode}
                                            onChange={e => setFormData({ ...formData, tenantCode: e.target.value })}
                                            className={`w-full p-2 border rounded-lg ${theme.input}`}
                                        />
                                    </div>
                                </div>

                                <div className={`p-4 rounded-lg flex items-start gap-3 ${isDark ? 'bg-blue-900/20 text-blue-200' : 'bg-blue-50 text-blue-800'}`}>
                                    <Shield size={18} className="mt-0.5" />
                                    <p className="text-sm font-medium">This will create a new isolated workspace and an administrator account for this tenant.</p>
                                </div>

                                <div className="space-y-4 pt-2">
                                    <h4 className={`text-sm font-bold border-b pb-2 ${theme.text} ${theme.border}`}>Admin Credentials</h4>
                                    <div className="grid grid-cols-2 gap-4">
                                        <div className="col-span-2">
                                            <label className={`block text-xs font-bold uppercase mb-2 ${theme.textSecondary}`}>Username</label>
                                            <input
                                                type="text"
                                                placeholder="Admin username"
                                                value={formData.adminUsername}
                                                onChange={e => setFormData({ ...formData, adminUsername: e.target.value })}
                                                className={`w-full p-2 border rounded-lg ${theme.input}`}
                                            />
                                        </div>
                                        <div>
                                            <label className={`block text-xs font-bold uppercase mb-2 ${theme.textSecondary}`}>Email</label>
                                            <input
                                                type="email"
                                                placeholder="admin@example.com"
                                                value={formData.adminEmail}
                                                onChange={e => setFormData({ ...formData, adminEmail: e.target.value })}
                                                className={`w-full p-2 border rounded-lg ${theme.input}`}
                                            />
                                        </div>
                                        <div>
                                            <label className={`block text-xs font-bold uppercase mb-2 ${theme.textSecondary}`}>Password</label>
                                            <input
                                                type="password"
                                                placeholder="••••••••"
                                                value={formData.adminPassword}
                                                onChange={e => setFormData({ ...formData, adminPassword: e.target.value })}
                                                className={`w-full p-2 border rounded-lg ${theme.input}`}
                                            />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className={`p-6 border-t ${theme.border} flex gap-3`}>
                                <button
                                    onClick={handleOnboard}
                                    disabled={isLoading || !formData.tenantName || !formData.tenantCode || !formData.adminEmail || !formData.adminPassword}
                                    className={`flex-1 py-3 rounded-lg font-bold flex items-center justify-center gap-2 ${theme.buttonPrimary}`}
                                >
                                    {isLoading ? <Loader2 className="animate-spin" size={20} /> : <RefreshCw size={20} />}
                                    {isLoading ? 'Onboarding...' : 'Onboard Business'}
                                </button>
                                <button
                                    onClick={() => setIsCreating(false)}
                                    className={`px-6 py-3 rounded-lg font-bold ${theme.buttonSecondary}`}
                                >
                                    Cancel
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </MainLayout>
    );
}
