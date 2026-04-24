import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Activity, Users, Calendar, Clock, LogOut, Settings } from 'lucide-react';
import { cn } from '../ui/Button';

export function Sidebar() {
  const location = useLocation();
  
  const navItems = [
    { name: 'Reception', path: '/nurse', icon: Users },
    { name: 'Waiting Queue', path: '/nurse/queue', icon: Clock },
    { name: 'Reservations', path: '/nurse/reservations', icon: Calendar },
  ];

  return (
    <div className="flex h-screen w-64 flex-col border-r border-gray-200 bg-white shadow-sm">
      <div className="flex h-20 items-center gap-2 border-b border-gray-100 px-6">
        <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-md">
          <Activity className="h-6 w-6 text-white" />
        </div>
        <span className="text-xl font-bold tracking-tight text-gray-900">Medi<span className="text-blue-600">Link</span></span>
      </div>

      <div className="flex-1 overflow-y-auto py-6 px-4">
        <p className="mb-4 px-2 text-xs font-semibold uppercase tracking-wider text-gray-400">Nurse Dashboard</p>
        <nav className="flex flex-col gap-1.5">
          {navItems.map((item) => {
            const isActive = location.pathname === item.path;
            const Icon = item.icon;
            return (
              <Link
                key={item.name}
                to={item.path}
                className={cn(
                  "flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition-all duration-200",
                  isActive 
                    ? "bg-blue-50 text-blue-700 shadow-sm" 
                    : "text-gray-600 hover:bg-gray-50 hover:text-gray-900"
                )}
              >
                <Icon className={cn("h-5 w-5", isActive ? "text-blue-600" : "text-gray-400")} />
                {item.name}
              </Link>
            );
          })}
        </nav>
      </div>

      <div className="border-t border-gray-100 p-4">
        <button className="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-50 hover:text-gray-900">
          <Settings className="h-5 w-5 text-gray-400" />
          Settings
        </button>
        <button className="mt-1 flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-red-600 transition-colors hover:bg-red-50">
          <LogOut className="h-5 w-5 text-red-500" />
          Log out
        </button>
      </div>
    </div>
  );
}
