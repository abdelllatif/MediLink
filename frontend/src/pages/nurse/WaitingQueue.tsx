import React, { useEffect, useState } from 'react';
import { Clock, User, ArrowRight, Stethoscope } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { motion } from 'framer-motion';
import api from '../../services/api';
import toast from 'react-hot-toast';

interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  status: string;
  patient: { firstName: string; lastName: string };
  doctor: { firstName: string; lastName: string };
  bloodPressure?: string;
  heartRate?: number;
  timeSlot: { startTime: string };
}

export default function WaitingQueue() {
  const [queue, setQueue] = useState<Appointment[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchQueue = async () => {
    try {
      const response = await api.get('/appointments/today');
      setQueue(response.data);
    } catch (error) {
      toast.error('Failed to fetch waiting queue');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchQueue();
    // In a real app, we would use SignalR here to listen for real-time updates
    const interval = setInterval(fetchQueue, 30000); // Polling every 30s as fallback
    return () => clearInterval(interval);
  }, []);

  const handleCallNext = async (appointmentId: string) => {
    try {
      await api.post(`/appointments/${appointmentId}/complete`);
      toast.success('Patient marked as completed.');
      fetchQueue();
    } catch (error) {
      toast.error('Failed to complete appointment.');
    }
  };

  if (isLoading) {
    return <div className="p-8 text-center text-gray-500">Loading queue...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Waiting Queue</h1>
          <p className="text-gray-500 mt-1">Manage patients currently in the clinic waiting area.</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <Card className="bg-gradient-to-br from-blue-500 to-indigo-600 text-white border-0">
          <h3 className="text-blue-100 font-medium">Total Waiting</h3>
          <p className="text-4xl font-bold mt-2">{queue.length}</p>
        </Card>
        <Card className="bg-gradient-to-br from-teal-500 to-emerald-600 text-white border-0">
          <h3 className="text-teal-100 font-medium">Avg. Wait Time</h3>
          <p className="text-4xl font-bold mt-2">--</p>
        </Card>
        <Card className="bg-gradient-to-br from-orange-500 to-red-600 text-white border-0">
          <h3 className="text-orange-100 font-medium">Completed Today</h3>
          <p className="text-4xl font-bold mt-2">--</p>
        </Card>
      </div>

      <div className="space-y-4">
        {queue.length === 0 ? (
          <div className="text-center p-12 bg-gray-50 rounded-2xl border border-gray-100 border-dashed">
            <p className="text-gray-500 font-medium">The waiting queue is currently empty.</p>
          </div>
        ) : (
          queue.map((appointment, index) => (
            <motion.div 
              key={appointment.id}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <Card className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 transition-all hover:shadow-md">
                <div className="flex items-center gap-4">
                  <div className="h-12 w-12 rounded-full flex items-center justify-center bg-blue-100 text-blue-600">
                    <User className="h-6 w-6" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900 text-lg flex items-center gap-2">
                      {appointment.patient.firstName} {appointment.patient.lastName}
                    </h3>
                    <div className="flex items-center gap-3 text-sm text-gray-500 mt-1">
                      <span className="flex items-center"><Clock className="h-4 w-4 mr-1" /> Scheduled: {appointment.timeSlot?.startTime}</span>
                      <span>•</span>
                      <span className="flex items-center"><Stethoscope className="h-4 w-4 mr-1" /> Dr. {appointment.doctor?.lastName}</span>
                    </div>
                  </div>
                </div>
                
                <div className="flex items-center gap-6">
                  {(appointment.bloodPressure || appointment.heartRate) && (
                    <div className="hidden md:block text-sm text-gray-600 bg-gray-50 px-4 py-2 rounded-lg border border-gray-100">
                      <p><span className="font-medium">BP:</span> {appointment.bloodPressure || 'N/A'}</p>
                      <p><span className="font-medium">HR:</span> {appointment.heartRate || 'N/A'} bpm</p>
                    </div>
                  )}
                  <Button className="shrink-0 w-full sm:w-auto" onClick={() => handleCallNext(appointment.id)}>
                    Mark Completed
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </div>
              </Card>
            </motion.div>
          ))
        )}
      </div>
    </div>
  );
}
