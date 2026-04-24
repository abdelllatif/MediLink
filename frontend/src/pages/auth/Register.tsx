import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Activity, User, Mail, Lock, Phone } from 'lucide-react';
import { motion } from 'framer-motion';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { GlassCard } from '../../components/ui/Card';

export default function Register() {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setTimeout(() => {
      setIsLoading(false);
      navigate('/login');
    }, 1500);
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 relative overflow-hidden px-4 py-12">
      <div className="absolute bottom-[-20%] right-[-10%] w-[60%] h-[60%] rounded-full bg-indigo-300/20 blur-[120px]" />
      
      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        className="w-full max-w-xl z-10"
      >
        <Link to="/" className="flex justify-center mb-8">
          <div className="flex items-center gap-2">
            <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-lg">
              <Activity className="h-7 w-7 text-white" />
            </div>
            <span className="text-3xl font-bold tracking-tight text-gray-900">Medi<span className="text-blue-600">Link</span></span>
          </div>
        </Link>

        <GlassCard className="p-8">
          <div className="text-center mb-8">
            <h1 className="text-2xl font-bold text-gray-900">Create Staff Account</h1>
            <p className="text-sm text-gray-500 mt-2">Join MediLink to manage your clinic efficiently.</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <Input label="First Name" placeholder="Jane" icon={<User className="h-5 w-5" />} required />
              <Input label="Last Name" placeholder="Doe" icon={<User className="h-5 w-5" />} required />
            </div>
            
            <Input type="email" label="Email Address" placeholder="jane.doe@medilink.com" icon={<Mail className="h-5 w-5" />} required />
            <Input type="tel" label="Phone Number" placeholder="+212 600 000000" icon={<Phone className="h-5 w-5" />} required />
            
            <div className="space-y-1.5 ml-1">
              <label className="block text-sm font-medium text-gray-700">Role</label>
              <select className="flex h-11 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 hover:border-blue-300">
                <option value="Nurse">Nurse</option>
                <option value="Generalist">Generalist Doctor</option>
                <option value="Specialist">Specialist Doctor</option>
              </select>
            </div>

            <Input type="password" label="Password" placeholder="••••••••" icon={<Lock className="h-5 w-5" />} required />

            <Button type="submit" className="w-full h-12 text-base mt-2" isLoading={isLoading}>
              Create Account
            </Button>
          </form>

          <p className="mt-6 text-center text-sm text-gray-600">
            Already have an account?{' '}
            <Link to="/login" className="font-semibold text-blue-600 hover:text-blue-700">
              Sign in
            </Link>
          </p>
        </GlassCard>
      </motion.div>
    </div>
  );
}
