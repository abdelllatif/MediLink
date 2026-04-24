import React, { useState, useEffect } from 'react';
import { Search, UserPlus, HeartPulse, ActivitySquare } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Modal } from '../../components/ui/Modal';
import { motion } from 'framer-motion';
import api from '../../services/api';
import toast from 'react-hot-toast';
import { format } from 'date-fns';

interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  cin?: string;
  dateOfBirth: string;
}

interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  specialty: string;
}

export default function Reception() {
  const [isNewPatientModalOpen, setIsNewPatientModalOpen] = useState(false);
  const [isQueueModalOpen, setIsQueueModalOpen] = useState(false);
  const [showVitalsForm, setShowVitalsForm] = useState(false);
  
  // Search state
  const [searchQuery, setSearchQuery] = useState('');
  const [patients, setPatients] = useState<Patient[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  // Doctors
  const [doctors, setDoctors] = useState<Doctor[]>([]);

  // New Patient Form state
  const [newPatient, setNewPatient] = useState({ firstName: '', lastName: '', cin: '', phone: '', dob: '' });
  const [isCreating, setIsCreating] = useState(false);

  // Queue/Vitals state
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [selectedDoctorId, setSelectedDoctorId] = useState('');
  const [vitals, setVitals] = useState({ bp: '', hr: '', temp: '', weight: '' });
  const [isQueueing, setIsQueueing] = useState(false);

  useEffect(() => {
    const fetchDoctors = async () => {
      try {
        const res = await api.get('/doctors');
        setDoctors(res.data);
      } catch (error) {
        console.error('Failed to fetch doctors', error);
      }
    };
    fetchDoctors();
  }, []);

  const handleSearch = async () => {
    if (!searchQuery.trim()) return;
    setIsSearching(true);
    try {
      const response = await api.get(`/patients/search?query=${searchQuery}`);
      setPatients(response.data);
      if (response.data.length === 0) {
        toast.error('No patient found. Please register a new patient.');
      }
    } catch (error) {
      toast.error('Search failed. Check your connection or login status.');
    } finally {
      setIsSearching(false);
    }
  };

  const handleCreatePatient = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsCreating(true);
    try {
      const response = await api.post('/patients', {
        firstName: newPatient.firstName,
        lastName: newPatient.lastName,
        dateOfBirth: newPatient.dob,
        cabinetId: "00000000-0000-0000-0000-000000000000" // Placeholder until user gets assigned a specific cabinet
      });
      toast.success('Patient created successfully!');
      setIsNewPatientModalOpen(false);
      setPatients([response.data]);
    } catch (error) {
      toast.error('Failed to create patient.');
    } finally {
      setIsCreating(false);
    }
  };

  const handleAddToQueue = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedPatient || !selectedDoctorId) {
      toast.error("Please select a doctor.");
      return;
    }

    setIsQueueing(true);
    try {
      // 1. Get available timeslots for today
      const today = format(new Date(), 'yyyy-MM-dd');
      const slotsRes = await api.get(`/doctors/${selectedDoctorId}/available-time-slots?date=${today}`);
      const availableSlots = slotsRes.data.filter((s: any) => s.isAvailable);

      if (availableSlots.length === 0) {
        toast.error("No available timeslots for this doctor today.");
        setIsQueueing(false);
        return;
      }

      // Pick the earliest available timeslot
      const timeSlotId = availableSlots[0].id;

      // 2. Book appointment
      const bookRes = await api.post('/appointments/book', {
        patientId: selectedPatient.id,
        timeSlotId: timeSlotId
      });

      const appointmentId = bookRes.data.id;

      // 3. Record Vitals if provided
      if (showVitalsForm && (vitals.bp || vitals.hr || vitals.temp || vitals.weight)) {
        await api.put(`/appointments/${appointmentId}/vitals`, {
          bloodPressure: vitals.bp,
          heartRate: vitals.hr ? parseInt(vitals.hr) : null,
          temperature: vitals.temp ? parseFloat(vitals.temp) : null,
          weight: vitals.weight ? parseFloat(vitals.weight) : null
        });
      }

      toast.success('Patient added to the waiting queue successfully!');
      setIsQueueModalOpen(false);
    } catch (error) {
      toast.error('Failed to add to queue.');
    } finally {
      setIsQueueing(false);
    }
  };

  const openQueueModal = (patient: Patient, withVitals: boolean) => {
    setSelectedPatient(patient);
    setShowVitalsForm(withVitals);
    setIsQueueModalOpen(true);
    setVitals({ bp: '', hr: '', temp: '', weight: '' });
    setSelectedDoctorId('');
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Patient Reception</h1>
          <p className="text-gray-500 mt-1">Search existing patients or register new arrivals.</p>
        </div>
        <Button onClick={() => setIsNewPatientModalOpen(true)}>
          <UserPlus className="mr-2 h-4 w-4" />
          New Patient
        </Button>
      </div>

      <Card className="flex items-center gap-4 p-4 border-blue-100 bg-blue-50/50">
        <div className="flex-1">
          <Input 
            placeholder="Search by name, CIN, or phone number..." 
            icon={<Search className="h-5 w-5" />}
            className="bg-white"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
          />
        </div>
        <Button variant="secondary" onClick={handleSearch} isLoading={isSearching}>
          Search Dossier
        </Button>
      </Card>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-6">
        {patients.map((patient) => (
          <motion.div key={patient.id} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
            <Card className="hover:border-blue-300 transition-colors cursor-pointer group">
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center gap-3">
                  <div className="h-12 w-12 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold text-lg uppercase">
                    {patient.firstName[0]}{patient.lastName[0]}
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">
                      {patient.firstName} {patient.lastName}
                    </h3>
                    <p className="text-sm text-gray-500">CIN: {patient.cin || 'N/A'}</p>
                  </div>
                </div>
                <span className="inline-flex items-center rounded-full bg-green-50 px-2.5 py-0.5 text-xs font-medium text-green-700 ring-1 ring-inset ring-green-600/20">
                  Found
                </span>
              </div>
              
              <div className="flex items-center gap-2 mt-4 pt-4 border-t border-gray-100">
                <Button size="sm" variant="outline" className="flex-1" onClick={() => openQueueModal(patient, true)}>
                  <HeartPulse className="mr-2 h-4 w-4" />
                  Record Vitals
                </Button>
                <Button size="sm" className="flex-1" onClick={() => openQueueModal(patient, false)}>
                  <ActivitySquare className="mr-2 h-4 w-4" />
                  Add to Queue
                </Button>
              </div>
            </Card>
          </motion.div>
        ))}
      </div>

      {/* New Patient Modal */}
      <Modal isOpen={isNewPatientModalOpen} onClose={() => setIsNewPatientModalOpen(false)} title="Register New Patient">
        <form className="space-y-4" onSubmit={handleCreatePatient}>
          <div className="grid grid-cols-2 gap-4">
            <Input label="First Name" required value={newPatient.firstName} onChange={(e) => setNewPatient({ ...newPatient, firstName: e.target.value })} />
            <Input label="Last Name" required value={newPatient.lastName} onChange={(e) => setNewPatient({ ...newPatient, lastName: e.target.value })} />
          </div>
          <Input label="CIN / ID Number" value={newPatient.cin} onChange={(e) => setNewPatient({ ...newPatient, cin: e.target.value })} />
          <Input label="Phone Number" type="tel" required value={newPatient.phone} onChange={(e) => setNewPatient({ ...newPatient, phone: e.target.value })} />
          <Input label="Date of Birth" type="date" required value={newPatient.dob} onChange={(e) => setNewPatient({ ...newPatient, dob: e.target.value })} />
          <div className="flex justify-end gap-3 mt-6">
            <Button variant="ghost" type="button" onClick={() => setIsNewPatientModalOpen(false)}>Cancel</Button>
            <Button type="submit" isLoading={isCreating}>Create Dossier</Button>
          </div>
        </form>
      </Modal>

      {/* Queue / Record Vitals Modal */}
      <Modal isOpen={isQueueModalOpen} onClose={() => setIsQueueModalOpen(false)} title={showVitalsForm ? `Record Vitals - ${selectedPatient?.firstName}` : `Add to Queue - ${selectedPatient?.firstName}`}>
        <form className="space-y-4" onSubmit={handleAddToQueue}>
          {showVitalsForm && (
            <div className="grid grid-cols-2 gap-4">
              <Input label="Blood Pressure (mmHg)" placeholder="120/80" value={vitals.bp} onChange={e => setVitals({...vitals, bp: e.target.value})} />
              <Input label="Heart Rate (bpm)" placeholder="72" type="number" value={vitals.hr} onChange={e => setVitals({...vitals, hr: e.target.value})} />
              <Input label="Temperature (°C)" placeholder="37.0" type="number" step="0.1" value={vitals.temp} onChange={e => setVitals({...vitals, temp: e.target.value})} />
              <Input label="Weight (kg)" placeholder="75" type="number" value={vitals.weight} onChange={e => setVitals({...vitals, weight: e.target.value})} />
            </div>
          )}
          
          <div className="space-y-1.5 ml-1 mt-2">
            <label className="block text-sm font-medium text-gray-700">Assign To Doctor</label>
            <select 
              className="flex h-11 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
              value={selectedDoctorId}
              onChange={e => setSelectedDoctorId(e.target.value)}
              required
            >
              <option value="">-- Select a Doctor --</option>
              {doctors.map(d => (
                <option key={d.id} value={d.id}>Dr. {d.firstName} {d.lastName} ({d.specialty || 'Generalist'})</option>
              ))}
            </select>
          </div>
          <div className="flex justify-end gap-3 mt-6">
            <Button variant="ghost" type="button" onClick={() => setIsQueueModalOpen(false)}>Cancel</Button>
            <Button type="submit" isLoading={isQueueing}>Save & Add to Queue</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
