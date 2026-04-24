import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../../components/layout/Sidebar';
import { Bell, Search } from 'lucide-react';
import { Input } from '../../components/ui/Input';

export default function DashboardLayout() {
  return (
    <div className="flex h-screen bg-gray-50 overflow-hidden">
      <Sidebar />
      
      <div className="flex-1 flex flex-col h-full overflow-hidden relative">
        <header className="h-20 bg-white/80 backdrop-blur-md border-b border-gray-200 px-8 flex items-center justify-between z-10 shrink-0">
          <div className="w-96">
            <Input 
              placeholder="Search patients, doctors..." 
              icon={<Search className="h-4 w-4" />}
              className="bg-gray-50/50"
            />
          </div>
          <div className="flex items-center gap-4">
            <button className="relative p-2 text-gray-500 hover:bg-gray-100 rounded-full transition-colors">
              <Bell className="h-5 w-5" />
              <span className="absolute top-1 right-1 h-2 w-2 rounded-full bg-red-500 border-2 border-white"></span>
            </button>
            <div className="flex items-center gap-3 border-l border-gray-200 pl-4 ml-2">
              <div className="h-10 w-10 rounded-full bg-gradient-to-br from-blue-100 to-indigo-100 flex items-center justify-center text-blue-700 font-bold">
                NR
              </div>
              <div>
                <p className="text-sm font-semibold text-gray-900">Nurse Rosa</p>
                <p className="text-xs text-gray-500">Clinique Nord</p>
              </div>
            </div>
          </div>
        </header>

        <main className="flex-1 overflow-y-auto p-8 relative">
          {/* Subtle background gradient */}
          <div className="absolute inset-0 bg-gradient-to-br from-blue-50/30 to-indigo-50/30 pointer-events-none" />
          
          <div className="relative z-10 max-w-6xl mx-auto h-full">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
