import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Activity } from 'lucide-react';
import { Button } from '../ui/Button';

export function Navbar() {
  const location = useLocation();
  const isAuthPage = location.pathname === '/login' || location.pathname === '/register';

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 glass border-b-0 px-6 py-4 transition-all duration-300">
      <div className="mx-auto flex max-w-7xl items-center justify-between">
        <Link to="/" className="flex items-center gap-2">
          <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-md">
            <Activity className="h-6 w-6 text-white" />
          </div>
          <span className="text-2xl font-bold tracking-tight text-gray-900">Medi<span className="text-blue-600">Link</span></span>
        </Link>
        
        {!isAuthPage && (
          <div className="flex items-center gap-4">
            <Link to="/login">
              <Button variant="ghost" className="font-semibold">Log In</Button>
            </Link>
            <Link to="/register">
              <Button>Get Started</Button>
            </Link>
          </div>
        )}
      </div>
    </nav>
  );
}
