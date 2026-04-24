import React from 'react';
import { motion } from 'framer-motion';
import { ArrowRight, HeartPulse, Activity, ShieldCheck } from 'lucide-react';
import { Link } from 'react-router-dom';
import { Button } from '../components/ui/Button';
import { Navbar } from '../components/layout/Navbar';

export default function Home() {
  const containerVariants = {
    hidden: { opacity: 0 },
    visible: {
      opacity: 1,
      transition: { staggerChildren: 0.1 }
    }
  };

  const itemVariants = {
    hidden: { y: 20, opacity: 0 },
    visible: {
      y: 0,
      opacity: 1,
      transition: { type: 'spring', stiffness: 100 }
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col overflow-hidden relative">
      {/* Background decoration */}
      <div className="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] rounded-full bg-blue-400/20 blur-[120px]" />
      <div className="absolute bottom-[-10%] right-[-10%] w-[40%] h-[40%] rounded-full bg-indigo-400/20 blur-[120px]" />

      <Navbar />

      <main className="flex-1 flex flex-col items-center justify-center pt-24 pb-16 px-6 relative z-10">
        <motion.div
          variants={containerVariants}
          initial="hidden"
          animate="visible"
          className="max-w-4xl text-center"
        >
          <motion.div variants={itemVariants} className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-100 text-blue-700 font-medium text-sm mb-8 shadow-sm">
            <span className="relative flex h-2 w-2">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-blue-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-blue-500"></span>
            </span>
            MediLink v2.0 is now live
          </motion.div>

          <motion.h1 variants={itemVariants} className="text-5xl md:text-7xl font-extrabold tracking-tight text-gray-900 mb-6 leading-tight">
            The Modern Operating System for <br className="hidden md:block" />
            <span className="text-gradient">Medical Practices</span>
          </motion.h1>

          <motion.p variants={itemVariants} className="text-xl text-gray-600 mb-10 max-w-2xl mx-auto leading-relaxed">
            Streamline your clinic's workflow. Seamlessly connect doctors, nurses, and specialists on a single, secure platform designed for modern healthcare.
          </motion.p>

          <motion.div variants={itemVariants} className="flex flex-col sm:flex-row items-center justify-center gap-4">
            <Link to="/register">
              <Button size="lg" className="w-full sm:w-auto group">
                Get Started Today
                <ArrowRight className="ml-2 h-5 w-5 transition-transform group-hover:translate-x-1" />
              </Button>
            </Link>
            <Link to="/login">
              <Button size="lg" variant="outline" className="w-full sm:w-auto bg-white/50 backdrop-blur-sm">
                Sign in to Dashboard
              </Button>
            </Link>
          </motion.div>
        </motion.div>

        {/* Feature Cards */}
        <motion.div
          variants={containerVariants}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: "-100px" }}
          className="grid grid-cols-1 md:grid-cols-3 gap-6 w-full max-w-5xl mt-24"
        >
          <div className="glass p-8 rounded-2xl">
            <div className="h-12 w-12 rounded-xl bg-blue-100 flex items-center justify-center mb-6">
              <HeartPulse className="h-6 w-6 text-blue-600" />
            </div>
            <h3 className="text-xl font-bold text-gray-900 mb-3">Patient Reception</h3>
            <p className="text-gray-600">Quickly admit patients, record vitals, and assign them to doctors seamlessly.</p>
          </div>
          
          <div className="glass p-8 rounded-2xl relative overflow-hidden">
            <div className="absolute inset-0 bg-gradient-to-br from-indigo-50 to-blue-50 opacity-50" />
            <div className="relative z-10">
              <div className="h-12 w-12 rounded-xl bg-indigo-100 flex items-center justify-center mb-6">
                <Activity className="h-6 w-6 text-indigo-600" />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-3">Live Queues</h3>
              <p className="text-gray-600">Real-time waiting room management to reduce wait times and improve satisfaction.</p>
            </div>
          </div>

          <div className="glass p-8 rounded-2xl">
            <div className="h-12 w-12 rounded-xl bg-teal-100 flex items-center justify-center mb-6">
              <ShieldCheck className="h-6 w-6 text-teal-600" />
            </div>
            <h3 className="text-xl font-bold text-gray-900 mb-3">Role-Based Security</h3>
            <p className="text-gray-600">Strictly isolated access for Nurses, Generalists, and Specialists protecting patient data.</p>
          </div>
        </motion.div>
      </main>
    </div>
  );
}
