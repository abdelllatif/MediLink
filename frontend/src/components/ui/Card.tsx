import React from 'react';
import { motion } from 'framer-motion';
import { cn } from './Button';

export function Card({ className, children, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className={cn("rounded-2xl border border-gray-100 bg-white shadow-sm p-6", className)} 
      {...props}
    >
      {children}
    </motion.div>
  );
}

export function GlassCard({ className, children, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className={cn("glass rounded-2xl p-6", className)} 
      {...props}
    >
      {children}
    </motion.div>
  );
}
