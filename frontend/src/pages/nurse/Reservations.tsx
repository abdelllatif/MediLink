import React, { useState, useEffect } from 'react';
import { Calendar as CalendarIcon, User, Clock, CheckCircle, Search } from 'lucide-react';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Modal } from '../../components/ui/Modal';
import { Input } from '../../components/ui/Input';
import api from '../../services/api';
import toast from 'react-hot-toast';
import { format } from 'date-fns';

interface Appointment {
  id: string;
  patient: { firstName: string; lastName: string };
  doctor: { firstName: string; lastName: string };
  timeSlot: { startTime: string; date: string };
  status: string;
}

interface Doctor {
  id: string;
  firstName: string;
  lastName: string;
  specialty: string;
}

interface TimeSlot {
  id: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

interface Patient {
  id: string;
  firstName: string;
  lastName: string;
  cin?: string;
}

export default function Reservations() {
  const [isBookModalOpen, setIsBookModalOpen] = useState(false);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // Booking Form State
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<Patient[]>([]);
  const [selectedPatientId, setSelectedPatientId] = useState('');
  const [selectedDoctorId, setSelectedDoctorId] = useState('');
  const [selectedDate, setSelectedDate] = useState(format(new Date(), 'yyyy-MM-dd'));
  const [availableSlots, setAvailableSlots] = useState<TimeSlot[]>([]);
  const [selectedSlotId, setSelectedSlotId] = useState('');
  const [isBooking, setIsBooking] = useState(false);

  const fetchData = async () => {
    try {
      const [apptsRes, docsRes] = await Promise.all([
        api.get('/appointments/today'),
        api.get('/doctors')
      ]);
      setAppointments(apptsRes.data);
      setDoctors(docsRes.data);
    } catch (error) {
      toast.error('Failed to load reservations data');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handlePatientSearch = async () => {
    if (!searchQuery.trim()) return;
    try {
      const res = await api.get(`/patients/search?query=${searchQuery}`);
      setSearchResults(res.data);
      if (res.data.length === 0) toast.error('No patients found');
    } catch (error) {
      toast.error('Search failed');
    }
  };

  useEffect(() => {
    const fetchSlots = async () => {
      if (!selectedDoctorId || !selectedDate) {
        setAvailableSlots([]);
        return;
      }
      try {
        const res = await api.get(`/doctors/${selectedDoctorId}/available-time-slots?date=${selectedDate}`);
        setAvailableSlots(res.data);
      } catch (error) {
        toast.error('Failed to load time slots');
      }
    };
    fetchSlots();
  }, [selectedDoctorId, selectedDate]);

  const handleBook = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedPatientId || !selectedSlotId) {
      toast.error('Please select a patient and a time slot');
      return;
    }

    setIsBooking(true);
    try {
      await api.post('/appointments/book', {
        patientId: selectedPatientId,
        timeSlotId: selectedSlotId
      });
      toast.success('Appointment booked successfully!');
      setIsBookModalOpen(false);
      fetchData(); // Refresh list
    } catch (error) {
      toast.error('Failed to book appointment');
    } finally {
      setIsBooking(false);
    }
  };

  const handleCancel = async (id: string) => {
    try {
      await api.post(`/appointments/${id}/cancel`);
      toast.success('Appointment cancelled');
      fetchData();
    } catch (error) {
      toast.error('Failed to cancel appointment');
    }
  };

  if (isLoading) return <div className="p-8 text-center text-gray-500">Loading reservations...</div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 tracking-tight">Appointments & Reservations</h1>
          <p className="text-gray-500 mt-1">Book and manage patient appointments for doctors.</p>
        </div>
        <Button onClick={() => setIsBookModalOpen(true)}>
          <CalendarIcon className="mr-2 h-4 w-4" />
          Book Appointment
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-4">
          <h3 className="text-lg font-semibold text-gray-900">Today's Schedule</h3>
          
          {appointments.length === 0 ? (
            <div className="p-8 text-center bg-gray-50 rounded-xl border border-dashed border-gray-200">
              <p className="text-gray-500">No appointments scheduled for today.</p>
            </div>
          ) : (
            appointments.map((appt) => (
              <Card key={appt.id} className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div className="flex items-start gap-4">
                  <div className="h-12 w-12 rounded-xl bg-indigo-50 border border-indigo-100 flex flex-col items-center justify-center shrink-0">
                    <span className="text-xs font-semibold text-indigo-500 uppercase">
                      {format(new Date(appt.timeSlot?.date || new Date()), 'MMM')}
                    </span>
                    <span className="text-lg font-bold text-indigo-700 leading-none">
                      {format(new Date(appt.timeSlot?.date || new Date()), 'dd')}
                    </span>
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-900 text-lg">Consultation</h4>
                    <p className="text-sm text-gray-500 flex items-center gap-4 mt-1">
                      <span className="flex items-center"><User className="h-4 w-4 mr-1" /> {appt.patient.firstName} {appt.patient.lastName}</span>
                      <span className="flex items-center"><Clock className="h-4 w-4 mr-1" /> {appt.timeSlot?.startTime}</span>
                    </p>
                    <p className="text-sm text-blue-600 font-medium mt-1">Dr. {appt.doctor.lastName}</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <Button variant="ghost" size="sm" className="text-red-600 hover:text-red-700 hover:bg-red-50" onClick={() => handleCancel(appt.id)}>Cancel</Button>
                </div>
              </Card>
            ))
          )}
        </div>

        <div>
          <Card className="bg-gray-50 border-0">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Doctor Availability</h3>
            <div className="space-y-3">
              {doctors.map(doc => (
                <div key={doc.id} className="p-3 bg-white rounded-xl border border-gray-100 flex items-center justify-between">
                  <div>
                    <p className="font-medium text-gray-900">Dr. {doc.firstName} {doc.lastName}</p>
                    <p className="text-xs text-gray-500">{doc.specialty || 'Generalist'}</p>
                  </div>
                  <span className="inline-flex items-center rounded-full bg-green-50 px-2 py-1 text-xs font-medium text-green-700">
                    <span className="h-1.5 w-1.5 rounded-full bg-green-500 mr-1.5"></span>
                    Available
                  </span>
                </div>
              ))}
            </div>
          </Card>
        </div>
      </div>

      <Modal isOpen={isBookModalOpen} onClose={() => setIsBookModalOpen(false)} title="Book New Appointment">
        <form className="space-y-4" onSubmit={handleBook}>
          
          <div className="flex gap-2 items-end">
            <div className="flex-1">
              <Input 
                label="Search Patient" 
                placeholder="Name or CIN..." 
                icon={<User className="h-4 w-4"/>} 
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
            <Button type="button" onClick={handlePatientSearch} variant="secondary">Search</Button>
          </div>

          {searchResults.length > 0 && (
            <div className="space-y-1.5 ml-1">
              <label className="block text-sm font-medium text-gray-700">Select Patient</label>
              <select 
                className="flex h-11 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
                value={selectedPatientId}
                onChange={(e) => setSelectedPatientId(e.target.value)}
                required
              >
                <option value="">-- Choose Patient --</option>
                {searchResults.map(p => (
                  <option key={p.id} value={p.id}>{p.firstName} {p.lastName} (CIN: {p.cin || 'N/A'})</option>
                ))}
              </select>
            </div>
          )}

          <div className="space-y-1.5 ml-1">
            <label className="block text-sm font-medium text-gray-700">Select Doctor</label>
            <select 
              className="flex h-11 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
              value={selectedDoctorId}
              onChange={(e) => setSelectedDoctorId(e.target.value)}
              required
            >
              <option value="">-- Choose Doctor --</option>
              {doctors.map(d => (
                <option key={d.id} value={d.id}>Dr. {d.firstName} {d.lastName}</option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <Input 
              label="Date" 
              type="date" 
              required 
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
            />
            <div className="space-y-1.5 ml-1">
              <label className="block text-sm font-medium text-gray-700">Time Slot</label>
              <select 
                className="flex h-11 w-full rounded-xl border border-gray-200 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 disabled:bg-gray-50"
                value={selectedSlotId}
                onChange={(e) => setSelectedSlotId(e.target.value)}
                required
                disabled={availableSlots.length === 0}
              >
                <option value="">{availableSlots.length === 0 ? 'No slots available' : '-- Choose Time --'}</option>
                {availableSlots.filter(s => s.isAvailable).map(slot => (
                  <option key={slot.id} value={slot.id}>{slot.startTime} - {slot.endTime}</option>
                ))}
              </select>
            </div>
          </div>

          <div className="flex justify-end gap-3 mt-6">
            <Button variant="ghost" type="button" onClick={() => setIsBookModalOpen(false)}>Cancel</Button>
            <Button type="submit" isLoading={isBooking}>Confirm Booking</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
